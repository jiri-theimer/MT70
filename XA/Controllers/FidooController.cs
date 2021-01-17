using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using XA.Models.fidoo;
using System.Text.Json;

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


        public string Spustit()
        {
            
            var strJson = System.IO.File.ReadAllText(_app.AppRootFolder + "\\FidooSettings.json");

            var cSetting = JsonSerializer.Deserialize<FidooSettings>(strJson);

            cSetting.members.First().LastRun = DateTime.Now;

            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            
            var strJson2Save = JsonSerializer.Serialize(cSetting, options);

            System.IO.File.WriteAllText(_app.AppRootFolder + "\\FidooSettings.json", strJson2Save);

            return cSetting.members.First().Name+": "+cSetting.members.First().ApiKey;
        }

        public int Pokus()
        {
            var ru = new BO.RunningUser() { j03Login = "lama@marktime50.net" };
            var f = new BL.Factory(ru,_app,_ep,_tt);
            

            var strediska = ListStrediska();
            var projekty = ListProjekty();

            //return strediska.Result.costCenterList.Count() + projekty.Result.projects.Count();

            return f.j02PersonBL.GetList(new BO.myQueryJ02()).Count() + projekty.Result.projects.Count();
        }

        public async Task<ResponseStrediska> ListStrediska()
        {

            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/settings/get-cost-centers"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", "pak1jgnBUswEVDGTkdT86v21XKMT0UYZSK0rYAq8Ecb1baMI5JskcAIZC32sMbw6");

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


        public async Task<ResponseProjekty> ListProjekty()
        {

            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/settings/get-projects"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", "pak1jgnBUswEVDGTkdT86v21XKMT0UYZSK0rYAq8Ecb1baMI5JskcAIZC32sMbw6");

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
