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
        public FidooController(IHttpClientFactory hcf, BL.TheEntitiesProvider ep, BL.RunningApp app, BL.TheTranslator tt)
        {
            _httpclientfactory = hcf;
            _ep = ep;
            _app = app;
            _tt = tt;
        }

        private JsonSerializerOptions GetJsonOptions()
        {
            var options = new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault, NumberHandling = JsonNumberHandling.AllowReadingFromString };

            options.WriteIndented = true;

            return options;
        }

        public string Doba()
        {

            
            var d1 = DateTime.Now;
            var d2 = d1.AddMinutes(1.4);
            return BO.basTime.DurationFormatted(d1, d2);
        }
        public string zch()
        {
            string apikey = "pak1lHKgSvgcSbMdtz2Erf2OzvZSVNepzPEXswV87WZMB7eUehY3WI09nLMrs85K";

            var ru = new BO.RunningUser() { j03Login = "admin1" };
            var f = new BL.Factory(ru, _app, _ep, _tt);

            var lisVydaje = LoadVydaje(apikey,true);    //fidoo výdaje k otestování
            var lisUzivatele = UzivateleFromVydaje(apikey, lisVydaje.Result, f);

            return string.Join(", ", lisUzivatele.Select(p => p.employeeNumber + ": " + p.j02ID.ToString()));
        }

        public string RunJobs()
        {

            var strJobsFile = _app.AppRootFolder + "\\Settings\\FidooJobs.json";
            if (!System.IO.File.Exists(strJobsFile))
            {
                return "File FidooJobs.json not exists";
            }
            var strJson = System.IO.File.ReadAllText(strJobsFile);
            var cJobs = JsonSerializer.Deserialize<FidooJobs>(strJson, GetJsonOptions());
            var lisLaunched = new List<FidooJob>();

            foreach (var cJob in cJobs.jobs.Where(p => p.Closed == false))   //projet všechny fidoo joby
            {
                if (cJob.LastRun == null || Convert.ToDateTime(cJob.LastRun).AddMinutes(cJob.RepeatMinuteInterval) < DateTime.Now)
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
            foreach (var onejob in lisLaunched.Where(p => p.ExportProjects == true))
            {
                SynchroProjekty(onejob);    //export projektů do fidoo
            }

            foreach (var onejob in lisLaunched.Where(p => p.ImportExpenses == true))
            {
                Handle_Import_Vydaje_OneJob(onejob);    //import fidoo výdajů pro joby s ImportExpenses=true
            }

            


            //aktualizovat čas posledního jetí jobu v konfiguračním souboru FidooJobs.json
            //var options = new JsonSerializerOptions();
            //options.WriteIndented = true;

            var strJson2Save = JsonSerializer.Serialize(cJobs, GetJsonOptions());
            System.IO.File.WriteAllText(strJobsFile, strJson2Save);   //uložit LastRun jobů do json souboru

            return "Launched jobs: " + string.Join(" ### ", lisLaunched.Select(p => p.Name));

        }

        private List<ResponseUzivatel> UzivateleFromVydaje(string apikey, ResponseVydaje vydaje, BL.Factory f)
        {
            //vrátí ze seznamu výdajů seznam uživatelských fidoo profilů
            var ret = new List<ResponseUzivatel>();
            foreach (var userid in vydaje.expenseList.Select(p => p.ownerUserId).Distinct())
            {
                var uzivatel = LoadUzivatel(apikey, userid);    //načíst profil fidoo uživatele
                ret.Add(uzivatel.Result);

                var recJ02 = f.j02PersonBL.LoadByCode(ret[ret.Count - 1].employeeNumber, 0);
                if (recJ02 != null)
                {
                    ret[ret.Count - 1].j02ID = recJ02.pid;
                }

            }

            return ret;
        }




        public int Pokus()
        {

            var ru = new BO.RunningUser() { j03Login = "lama@marktime50.net" };
            var f = new BL.Factory(ru, _app, _ep, _tt);

            //var rec = new BO.p31WorksheetEntryInput() { p31HoursEntryflag=BO.p31HoursEntryFlagENUM.Hodiny, p31Date = DateTime.Today, p41ID = 7196, j02ID = 1,Value_Orig="00:45",p32ID= 1003,p34ID=1 };
            //rec.p31Text = "Jsi hovado.";

            var rec = new BO.p31WorksheetEntryInput() { p31HoursEntryflag = BO.p31HoursEntryFlagENUM.NeniCas, p31Date = DateTime.Today, p41ID = 7196, j02ID = 1, p34ID = 6, p32ID = 2, Amount_WithoutVat_Orig = 1000, VatRate_Orig = 21, Amount_WithVat_Orig = 1222, j27ID_Billing_Orig = 2 };
            rec.p31Text = "Testovací výdaj: Expense.";

            int intP31ID = f.p31WorksheetBL.SaveOrigRecord(rec, BO.p33IdENUM.PenizeVcDPHRozpisu, null);
            //foreach(var cc in f.CurrentUser.Messages4Notify)
            //{
            //    System.IO.File.WriteAllText("c:\\temp\\hovado.txt", cc.Key + ": " + cc.Value + " #### ");
            //}

            return intP31ID;
        }


        public async Task<ResponseVydaje> LoadVydaje(string apikey,bool? closedonly)
        {

            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/expense/get-expenses"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", apikey);

                var input = new RequestVydaje();
                input.from = DateTime.Now.AddDays(-100).ToLocalTime();
                input.to = DateTime.Now.ToLocalTime();
                input.lastModifyFrom = DateTime.Now.AddDays(-100).ToLocalTime();
                if (closedonly !=null && closedonly == true)
                {
                    input.closed = true;
                }
                
                var s = JsonSerializer.Serialize(input);

                request.Content = new StringContent(s);
                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


                var response = await httpclient.SendAsync(request);

                var strJson = await response.Content.ReadAsStringAsync();

                //System.IO.File.WriteAllText(_app.LogFolder+"\\hovado.txt", strJson);

                //var options = new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,NumberHandling=JsonNumberHandling.AllowReadingFromString };

                var xx = JsonSerializer.Deserialize<ResponseVydaje>(strJson, GetJsonOptions());

                return xx;

                //return await response.Content.ReadAsStringAsync();
            }

        }

        public async Task<ResponseUzivatel> LoadUzivatel(string apikey, string userid)
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


        public List<string> SynchroProjektyManual(string apikey,string login)
        {
            var onejob = new FidooJob() { ApiKey = apikey, RobotUser = login };
            return SynchroProjekty(onejob);
        }

        public List<string> SynchroProjekty(FidooJob onejob)
        {
            var ru = new BO.RunningUser() { j03Login = onejob.RobotUser };    //robot login
            var f = new BL.Factory(ru, _app, _ep, _tt);

            var lisFidooProjekty = ListProjekty(onejob.ApiKey);
            var lisP41 = f.p41ProjectBL.GetList(new BO.myQueryP41("p41") { IsRecordValid = null });
            var ret = new List<string>();
            foreach(var recP41 in lisP41)
            {
                string strProjectName = recP41.p41Name;
                if (recP41.p28ID_Client > 0)
                {
                    strProjectName = recP41.Client + " - " + strProjectName;
                }
                if (strProjectName.Length > 50) strProjectName = strProjectName.Substring(0, 49);
                string strState = "active";
                if (recP41.isclosed)
                {
                    strState = "deleted";
                }

                var qry = lisFidooProjekty.Where(p => p.code == recP41.p41Code);
                if (qry.Count() > 0)
                {
                    //projekt již existuje ve fidoo db
                    string strProjectId = qry.First().id;
                    
                    if (recP41.isclosed)
                    {
                        if (qry.First().state != "deleted")
                        {
                            //nahodit projekt jako deleted
                            var strRet = DeleteOneProject(onejob.ApiKey, strProjectId).Result.info;
                            ret.Add("DELETE: " + recP41.p41Code + "/" + recP41.FullName + ": " + strRet);
                        }
                            
                    }
                    else
                    {
                        if (qry.First().name != strProjectName)
                        {
                            var strRet = UpdateOneProject(onejob.ApiKey, strProjectId, recP41.p41Code, strProjectName, strState).Result.info;
                            ret.Add("UPDATE: " + recP41.p41Code + "/" + recP41.FullName + ": " + strRet);
                        }
                        
                    }
                    
                }
                else
                {
                    //projekt ještě neexistuje ve fidoo db
                    if (!recP41.isclosed)
                    {
                        var strRet = InsertOneProject(onejob.ApiKey, recP41.p41Code, strProjectName, strState).Result.projectId;
                        ret.Add("INSERT: " + recP41.p41Code + "/" + recP41.FullName + ": " + strRet);
                    }
                }
            }

            return ret;
        }
        private async Task<ResponseUpdateProject> DeleteOneProject(string apikey, string strProjectId)
        {
            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/settings/delete-project"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", apikey);

                var req = new DeleteOneProject();
                req.id = strProjectId;
                

                var s = JsonSerializer.Serialize(req);

                request.Content = new StringContent(s);

                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


                var response = await httpclient.SendAsync(request);

                var strJson = await response.Content.ReadAsStringAsync();

                var xx = JsonSerializer.Deserialize<ResponseUpdateProject>(strJson);

                return xx;

            }
        }
        private async Task<ResponseAddProject> InsertOneProject(string apikey, string strCode, string strName, string strState)
        {
            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/settings/add-project"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", apikey);

                var req = new Project();               
                req.code = strCode;
                req.name = strName;
                req.state = strState;


                var s = JsonSerializer.Serialize(req);

                request.Content = new StringContent(s);

                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


                var response = await httpclient.SendAsync(request);

                var strJson = await response.Content.ReadAsStringAsync();

                var xx = JsonSerializer.Deserialize<ResponseAddProject>(strJson);

                return xx;

            }
        }
        private async Task<ResponseUpdateProject> UpdateOneProject(string apikey,string strProjectId,string strCode,string strName,string strState)
        {
            var httpclient = _httpclientfactory.CreateClient();
            
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/settings/update-project"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", apikey);

                var req = new Project();
                req.id = strProjectId;
                req.code = strCode;
                req.name = strName;
                req.state = strState;

                
                var s = JsonSerializer.Serialize(req);

                request.Content = new StringContent(s);

                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


                var response = await httpclient.SendAsync(request);

                var strJson = await response.Content.ReadAsStringAsync();

                var xx = JsonSerializer.Deserialize<ResponseUpdateProject>(strJson);

                return xx;

            }
        }

        public List<Project> ListProjekty(string apikey, string projectId = null)
        {
            bool bolCompleted = false;
            var ret = new List<Project>();
            int intOffset = 0;

            while (!bolCompleted)
            {
                var c = LoadProjekty(apikey, intOffset, projectId);                
                ret.InsertRange(0, c.Result.projects);

                bolCompleted = c.Result.complete;
                intOffset += 100;
            }

            return ret;
        }
        public async Task<ResponseProjekty> LoadProjekty(string apikey, int offset = 0,string projectId=null)
        {
            //offset je počet záznamů, které se mají přeskočit

            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/settings/get-projects"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", apikey);

                var req = new RequestRoot();

                req.queryRequest = new Queryrequest() { offset = offset, limit = 100 };
                if (projectId != null)
                {
                    req.projectIds = new List<string>();
                    req.projectIds.Add(projectId);
                }
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

        private List<BO.o58FieldBag> GetFieldBag(BL.Factory f)
        {
            var lisBag = f.o58FieldBagBL.GetList(new BO.myQuery("o58"));
            var b = false;
            if (!lisBag.Any(p=>p.o58Code== "fidoo_merchantName"))
            {
                CreateBag(f, "fidoo_merchantName", "Název obchodníka");b = true;
            }
            if (!lisBag.Any(p => p.o58Code == "fidoo_merchantVatId"))
            {
                CreateBag(f, "fidoo_merchantVatId", "DIČ obchodníka");b = true;
            }
            if (!lisBag.Any(p => p.o58Code == "fidoo_merchantVatId"))
            {
                CreateBag(f, "fidoo_merchantAddress", "Adresa obchodníka");b = true;
            }
            if (!lisBag.Any(p => p.o58Code == "fidoo_merchantVatId"))
            {
                CreateBag(f, "fidoo_receiptUrl", "Obrázek účtenky");b = true;
            }
            if (b)
            {
                lisBag = f.o58FieldBagBL.GetList(new BO.myQuery("o58"));
            }
            return lisBag.ToList();
        }
       
        private void CreateBag(BL.Factory f,string strCode,string strName,BO.x24IdENUM x24id=BO.x24IdENUM.tString)
        {
            var c = new BO.o58FieldBag() { o58Code = strCode, o58Name = strName, x24ID = x24id };
            f.o58FieldBagBL.Save(c);
        }
        private void Handle_Import_Vydaje_OneJob(FidooJob onejob)
        {
            var ru = new BO.RunningUser() { j03Login = onejob.RobotUser };    //robot login
            var f = new BL.Factory(ru, _app, _ep, _tt);
            var lisVydaje = LoadVydaje(onejob.ApiKey,onejob.ExpenseImportClosedOnly);    //fidoo výdaje k otestování
            var lisUzivatele = UzivateleFromVydaje(onejob.ApiKey, lisVydaje.Result, f);
            var lisBag = GetFieldBag(f);

            
            BO.p33IdENUM p33id = BO.p33IdENUM.PenizeBezDPH;

            foreach (var vydaj in lisVydaje.Result.expenseList)    //importovat pouze uzavřené fidoo výdaje
            {
                
                bool bolGO = false;
                if (string.IsNullOrEmpty(onejob.ExpenseImportClassState) || vydaj.classState == onejob.ExpenseImportClassState)
                {
                    bolGO = true;
                }
                if (bolGO)
                {
                    var recExist = f.p31WorksheetBL.LoadByExternalPID(vydaj.expenseId);
                    if (recExist != null)
                    {
                        bolGO = false;  //výdaj už byl dříve importován
                    }
                    
                }                
                int intP41ID = onejob.DefaultP41ID;
                if (bolGO && vydaj.projectIds != null && vydaj.projectIds.Count() > 0)
                {
                    var lisProjekty = ListProjekty(onejob.ApiKey, vydaj.projectIds.First());
                    if (lisProjekty.Count() > 0)
                    {                        
                        var recP41 = f.p41ProjectBL.LoadByCode(lisProjekty.First().code, 0);
                        if (recP41 != null)
                        {
                            intP41ID = recP41.pid;
                        }
                        else
                        {
                            LogInfo("Projekt nebyl nalezen: " + lisProjekty.First().code+" ## "+ lisProjekty.First().id);                            
                        }
                    }
                    if (intP41ID == 0) bolGO = false;
                }
                
               
                if (bolGO)
                {
                    //výdaj chybí v MT

                    var recP31 = new BO.p31WorksheetEntryInput() { p31HoursEntryflag = BO.p31HoursEntryFlagENUM.NeniCas, p41ID= intP41ID, j27ID_Billing_Orig = 2 };

                    if (lisUzivatele.Where(p => p.userId == vydaj.ownerUserId && p.j02ID > 0).Any())
                    {
                        recP31.j02ID = lisUzivatele.Where(p => p.userId == vydaj.ownerUserId).First().j02ID;
                    }

                    recP31.p31Date = vydaj.dateTime;
                    recP31.Amount_WithVat_Orig = vydaj.amount;
                    if (vydaj.vatRate != null && vydaj.vatAmount != null)
                    {
                        recP31.VatRate_Orig = Convert.ToDouble(vydaj.vatRate * 100);
                        recP31.Amount_WithoutVat_Orig = recP31.Amount_WithVat_Orig - Convert.ToDouble(vydaj.vatAmount);
                        p33id = BO.p33IdENUM.PenizeVcDPHRozpisu;

                    }
                    else
                    {
                        recP31.Amount_WithoutVat_Orig = Convert.ToDouble(vydaj.amount);
                        recP31.VatRate_Orig = 0;
                        recP31.x15ID = BO.x15IdEnum.BezDPH;
                    }
                    recP31.p31Text = vydaj.name;
                    if (!string.IsNullOrEmpty(vydaj.description))
                    {
                        recP31.p31Text += " ### " + vydaj.description.Replace(vydaj.name,"");
                    }
                    

                    if (!string.IsNullOrEmpty(vydaj.currency))
                    {
                        var recJ27 = f.FBL.LoadCurrencyByCode(vydaj.currency);
                        recP31.j27ID_Billing_Orig = recJ27.j27ID;
                    }


                    if (onejob.DefaultP32ID > 0)
                    {
                        recP31.p32ID = onejob.DefaultP32ID;
                        recP31.p34ID = f.p32ActivityBL.Load(onejob.DefaultP32ID).p34ID;
                    }
                    if (onejob.DefaultP41ID > 0)
                    {
                        recP31.p41ID = onejob.DefaultP41ID;
                    }
                    
                    string curlogin = f.CurrentUser.j03Login;
                    f.CurrentUser.j03Login = "fidoo";
                    int intPID = f.p31WorksheetBL.SaveOrigRecord(recP31, p33id, null);
                    if (intPID > 0)
                    {
                        f.p31WorksheetBL.UpdateExternalPID(intPID, vydaj.expenseId);
                        f.o58FieldBagBL.SetValue_String(lisBag.Where(p => p.o58Code == "fidoo_merchantName").First().pid,"p31",intPID,vydaj.merchantName);
                        f.o58FieldBagBL.SetValue_String(lisBag.Where(p => p.o58Code == "fidoo_merchantVatId").First().pid, "p31", intPID, vydaj.merchantVatId);
                        f.o58FieldBagBL.SetValue_String(lisBag.Where(p => p.o58Code == "fidoo_merchantAddress").First().pid, "p31", intPID, vydaj.merchantAddress);
                        if (vydaj.receiptUrls !=null && vydaj.receiptUrls.Count() > 0)
                        {
                            foreach(var s in vydaj.receiptUrls)
                            {
                                f.o58FieldBagBL.SetValue_String(lisBag.Where(p => p.o58Code == "fidoo_receiptUrl").First().pid, "p31", intPID, s);
                            }
                        }
                        

                        LogInfo(vydaj.expenseId + ", saved to pid: " + intPID.ToString());

                        
                    }
                    else
                    {
                        LogInfo(vydaj.expenseId + ", error: " + recP31.ErrorMessage);
                    }
                    f.CurrentUser.j03Login = curlogin;

                    //System.IO.File.AppendAllText("c:\\temp\\hovado.txt", recP31.p31Text);

                }
            }
            //výdaje jobu zpracovány

        }

        private void LogInfo(string strMessage)
        {
            var strPath = string.Format("{0}\\fidoo-{1}.log", _app.LogFolder, DateTime.Now.ToString("yyyy.MM.dd"));

            System.IO.File.AppendAllLines(strPath, new List<string>() { "", DateTime.Now.ToString() + ": ", strMessage });
        }


    }
}
