using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using XA.Models.fidoo;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XA.Controllers
{
    public class FidooController : Controller
    {
        private readonly IHttpClientFactory _httpclientfactory;
        private readonly BL.TheEntitiesProvider _ep;
        private readonly BL.RunningApp _app;
        private readonly BL.TheTranslator _tt;
        public FidooController(IHttpClientFactory hcf, BL.TheEntitiesProvider ep,BL.RunningApp app,BL.TheTranslator tt)
        {
            _httpclientfactory = hcf;
            _ep = ep;
            _app = app;
            _tt = tt;
        }

        public string Doba()
        {
            
            var c = new BO.CLS.TimeSupport();
            var d1 = DateTime.Now;
            var d2 = d1.AddMinutes(1.4);
            return c.DurationFormatted(d1, d2);
        }
        public string zch()
        {
            string apikey = "pak1lHKgSvgcSbMdtz2Erf2OzvZSVNepzPEXswV87WZMB7eUehY3WI09nLMrs85K";

            var ru = new BO.RunningUser() { j03Login = "admin1" };
            var f = new BL.Factory(ru, _app, _ep, _tt);

            var lisVydaje = LoadVydaje(apikey);    //fidoo výdaje k otestování
            var lisUzivatele = ListUzivateleFromVydaje(apikey, lisVydaje.Result);

            return lisUzivatele.Count().ToString()+" ### "+ lisVydaje.Result.expenseList.Count().ToString();
        }

        public string Index()
        {
            var strJobsFile = _app.AppRootFolder + "\\Settings\\FidooJobs.json";
            if (!System.IO.File.Exists(strJobsFile))
            {
                return "File FidooJobs.json not exists";
            }
            var strJson = System.IO.File.ReadAllText(strJobsFile);
            var cJobs = JsonSerializer.Deserialize<FidooJobs>(strJson);
            var lisLaunched = new List<FidooJob>();

            foreach(var cJob in cJobs.jobs.Where(p=>p.Closed==false))   //projet všechny fidoo joby
            {
                if (cJob.LastRun==null || Convert.ToDateTime(cJob.LastRun).AddMinutes(cJob.RepeatMinuteInterval) < DateTime.Now)
                {
                    //nastal čas spustit job
                    lisLaunched.Add(cJob);
                    cJob.LastRun = DateTime.Now;    //aktualizovat poslední čas spuštění jobu
                }
            }


            if (lisLaunched.Count() == 0)
            {
                return "no job launched"; //nebyl spuštěn žádný job -> odchod
            }


            Handle_Vydaje(lisLaunched);

            //aktualizovat čas posledního jetí jobu v konfiguračním souboru FidooJobs.json
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            
            var strJson2Save = JsonSerializer.Serialize(cJobs, options);
            System.IO.File.WriteAllText(strJobsFile, strJson2Save);   //uložit LastRun jobů do json souboru

            return "Launched jobs: "+string.Join(" ### ", lisLaunched.Select(p => p.Name));
            
        }

        private List<ResponseUzivatel> ListUzivateleFromVydaje(string apikey,ResponseVydaje vydaje)
        {
            //vrátí ze seznamu výdajů seznam uživatelských fidoo profilů
            var ret = new List<ResponseUzivatel>();
            foreach (var userid in vydaje.expenseList.Select(p=>p.ownerUserId).Distinct())
            {
                var uzivatel = LoadUzivatel(apikey,userid);    //načíst profil fidoo uživatele
                ret.Add(uzivatel.Result);
                
            }

            return ret;
        }
        private void Handle_Vydaje(List<FidooJob> lisJobs)  //zpracovat načtení výdajů z fidoo
        {
            foreach (var cJob in lisJobs)
            {
                var ru = new BO.RunningUser() { j03Login = cJob.RobotUser };    //robot login
                var f = new BL.Factory(ru, _app, _ep, _tt);
                var lisVydaje = LoadVydaje(cJob.ApiKey);    //fidoo výdaje k otestování
                var lisUzivatele = ListUzivateleFromVydaje(cJob.ApiKey, lisVydaje.Result);

               

                foreach (var vydaj in lisVydaje.Result.expenseList)
                {
                    if (f.p31WorksheetBL.LoadByExternalPID(vydaj.expenseId) == null)
                    {
                        //výdaj chybí v MT                               
                        var recP31 = new BO.p31WorksheetEntryInput() { p31HoursEntryflag = BO.p31HoursEntryFlagENUM.NeniCas, p34ID = 6, j27ID_Billing_Orig = 2 };                        
                        recP31.p31ExternalPID = vydaj.expenseId;

                        string strOsobniCislo = lisUzivatele.Where(p => p.userId == vydaj.ownerUserId).First().employeeNumber;
                        recP31.j02ID = f.j02PersonBL.LoadByCode(strOsobniCislo, 0).pid;

                        recP31.p31Date = vydaj.dateTime;
                        recP31.Amount_WithVat_Orig = vydaj.amount;
                        if (vydaj.vatRate !=null && vydaj.vatAmount !=null)
                        {
                            recP31.VatRate_Orig = Convert.ToDouble(vydaj.vatRate);
                            recP31.Amount_WithoutVat_Orig = recP31.Amount_WithVat_Orig-Convert.ToDouble(vydaj.vatAmount);
                        }
                        recP31.p31Text = vydaj.name;
                        if (!string.IsNullOrEmpty(vydaj.description))
                        {
                            recP31.p31Text += " ### " + vydaj.description;
                        }

                        var recJ27 = f.FBL.LoadCurrencyByCode(vydaj.currency);
                        recP31.j27ID_Billing_Orig = recJ27.j27ID;

                        if (cJob.DefaultP32ID > 0)
                        {
                            recP31.p32ID = cJob.DefaultP32ID;
                        }
                        if (cJob.DefaultP41ID > 0)
                        {
                            recP31.p41ID = cJob.DefaultP41ID;
                        }
                        
                    }
                }

            }
        }

        public int Pokus()
        {
           
            var ru = new BO.RunningUser() { j03Login = "lama@marktime50.net" };
            var f = new BL.Factory(ru,_app,_ep,_tt);

            //var rec = new BO.p31WorksheetEntryInput() { p31HoursEntryflag=BO.p31HoursEntryFlagENUM.Hodiny, p31Date = DateTime.Today, p41ID = 7196, j02ID = 1,Value_Orig="00:45",p32ID= 1003,p34ID=1 };
            //rec.p31Text = "Jsi hovado.";

            var rec = new BO.p31WorksheetEntryInput() { p31HoursEntryflag = BO.p31HoursEntryFlagENUM.NeniCas, p31Date = DateTime.Today, p41ID = 7196, j02ID = 1, p34ID=6,p32ID=2,Amount_WithoutVat_Orig=1000,VatRate_Orig=21,Amount_WithVat_Orig=1222, j27ID_Billing_Orig=2 };
            rec.p31Text = "Testovací výdaj: Expense.";

            int intP31ID = f.p31WorksheetBL.SaveOrigRecord(rec, BO.p33IdENUM.PenizeVcDPHRozpisu, null);
            //foreach(var cc in f.CurrentUser.Messages4Notify)
            //{
            //    System.IO.File.WriteAllText("c:\\temp\\hovado.txt", cc.Key + ": " + cc.Value + " #### ");
            //}
           
            return intP31ID;
        }

        
        public async Task<ResponseVydaje> LoadVydaje(string apikey)
        {

            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/expense/get-expenses"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", apikey);

                var input = new RequestVydaje();
                input.from = DateTime.Now.AddDays(-100).ToLocalTime();
                input.to = DateTime.Now.ToLocalTime();
                input.lastModifyFrom = DateTime.Now.AddDays(-5).ToLocalTime();

                var s = JsonSerializer.Serialize(input);

                request.Content = new StringContent(s);
                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


                var response = await httpclient.SendAsync(request);

                var strJson = await response.Content.ReadAsStringAsync();

                //System.IO.File.WriteAllText("c:\\temp\\hovado.txt", strJson);

                var options = new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,NumberHandling=JsonNumberHandling.AllowReadingFromString };
               
                var xx = JsonSerializer.Deserialize<ResponseVydaje>(strJson,options);

                return xx;

                //return await response.Content.ReadAsStringAsync();
            }

        }

        public async Task<ResponseUzivatel> LoadUzivatel(string apikey,string userid)
        {

            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/user/get-user"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", apikey);

                var s = JsonSerializer.Serialize(new RequestUzivatel() { userId = userid });

                request.Content = new StringContent(s);

                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


                var response = await httpclient.SendAsync(request);

                var strJson = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ResponseUzivatel>(strJson);

                
            }

        }

        public async Task<ResponseStrediska> LoadStrediska(string apikey)
        {

            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/settings/get-cost-centers"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", apikey);

                var s = JsonSerializer.Serialize(new RequestRoot());
                
                request.Content = new StringContent(s);

                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


                var response = await httpclient.SendAsync(request);

                var strJson = await response.Content.ReadAsStringAsync();

                var xx = JsonSerializer.Deserialize<ResponseStrediska>(strJson);
                
                return xx;

                //return await response.Content.ReadAsStringAsync();
            }

        }

        public List<Project> ListProjekty(string apikey)
        {
            bool bolCompleted = false;
            var ret = new List<Project>();
            int intOffset = 0;

            while (!bolCompleted)
            {
                var c = LoadProjekty(apikey, intOffset);
                ret.InsertRange(0,c.Result.projects.Where(p=>p.state=="active"));
                bolCompleted = c.Result.complete;
                intOffset += 100;
            }

            return ret;
        }
        public async Task<ResponseProjekty> LoadProjekty(string apikey,int offset=0)
        {
            //offset je počet záznamů, které se mají přeskočit

            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/settings/get-projects"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", apikey);

                var req = new RequestRoot();
                
                req.queryRequest = new Queryrequest() { offset = offset, limit = 100 };
                req.queryRequest.sort = new List<Sort>();
                req.queryRequest.sort.Add(new Sort() { ascendant = false, property = "id" });
                var s = JsonSerializer.Serialize(req);

                request.Content = new StringContent(s);

                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


                var response = await httpclient.SendAsync(request);

                var strJson = await response.Content.ReadAsStringAsync();

                var xx = JsonSerializer.Deserialize<ResponseProjekty>(strJson);

                return xx;

                //return await response.Content.ReadAsStringAsync();
            }

        }


    }
}
