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

            foreach(var cJob in cJobs.jobs.Where(p=>p.Closed==false))
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

            
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            
            var strJson2Save = JsonSerializer.Serialize(cJobs, options);
            System.IO.File.WriteAllText(strJobsFile, strJson2Save);   //uložit LastRun jobů do json souboru

            return "Launched jobs: "+string.Join(" ### ", lisLaunched.Select(p => p.Name));
            
        }

        public int Pokus()
        {
           
            var ru = new BO.RunningUser() { j03Login = "lama@marktime50.net" };
            var f = new BL.Factory(ru,_app,_ep,_tt);

            var rec = new BO.p31WorksheetEntryInput() { p31HoursEntryflag=BO.p31HoursEntryFlagENUM.Hodiny, p31Date = DateTime.Today, p41ID = 7196, j02ID = 1,Value_Orig="00:45",p32ID= 1003,p34ID=1 };
            rec.p31Text = "Jsi hovado.";

            int intP31ID = f.p31WorksheetBL.SaveOrigRecord(rec, BO.p33IdENUM.Cas, null);
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
