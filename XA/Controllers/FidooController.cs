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

            var ru = new BO.RunningUser() { j03Login = "admin1" };
            var f = new BL.Factory(ru, _app, _ep, _tt);

            return f.CurrentUser.PersonAsc;
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

        private void Handle_Vydaje(List<FidooJob> lisLaunched)  //zpracovat načtení výdajů z fidoo
        {
            foreach (var cJob in lisLaunched)
            {
                var ru = new BO.RunningUser() { j03Login = cJob.RobotUser };    //robot login
                var f = new BL.Factory(ru, _app, _ep, _tt);
                var lisVydaje = ListVydaje(cJob.ApiKey);    //fidoo výdaje k otestování
                var lisUzivatele = new List<ResponseUzivatel>();

                foreach (var vydaj in lisVydaje.Result.expenseList)
                {
                    if (f.p31WorksheetBL.LoadByExternalPID(vydaj.expenseId) == null)
                    {
                        //výdaj ještě chybí v MT       
                        if (lisUzivatele.Where(p => p.userId == vydaj.ownerUserId).Count() == 0)
                        {
                            var uzivatel = LoadUzivatel(cJob.ApiKey, vydaj.ownerUserId);
                            if (uzivatel.Result != null)
                            {
                                lisUzivatele.Add(uzivatel.Result);
                            }
                        }
                        var recP31 = new BO.p31WorksheetEntryInput() { p31HoursEntryflag = BO.p31HoursEntryFlagENUM.NeniCas, j02ID = 1, p34ID = 6, j27ID_Billing_Orig = 2 };                        
                        recP31.p31ExternalPID = vydaj.expenseId;
                        if (lisUzivatele.Where(p => p.userId == vydaj.ownerUserId).Count() > 0)
                        {
                            string strUserCode=lisUzivatele.Where(p => p.userId == vydaj.ownerUserId).First().employeeNumber;
                            recP31.j02ID = f.j02PersonBL.LoadByCode(strUserCode,0).pid;
                        }                        
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

        public async Task<ResponseVydaje> ListVydaje(string apikey)
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

        public async Task<ResponseStrediska> ListStrediska(string apikey)
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


        public async Task<ResponseProjekty> ListProjekty(string apikey)
        {

            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/settings/get-projects"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", apikey);

                var s = JsonSerializer.Serialize(new RequestRoot());

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
