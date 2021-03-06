using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using UI.Models;

namespace UI.Controllers
{
    public class RobotController : Controller
    {
        private readonly IHttpClientFactory _httpclientfactory;
        private readonly BL.RunningApp _app;
       
        public RobotController(IHttpClientFactory hcf, BL.RunningApp app)
        {
            _httpclientfactory = hcf;
            _app = app;
            
        }

       

        public IActionResult Index()
        {

            var v = new RobotViewModel();

            v.MessageToUser = "Current time: " + System.DateTime.Now.ToString();

            
            v.MessageToUser = Handle_Cnb().Result;



            return View(v);
        }
        public IActionResult Ping()
        {
            LogInfo("Ping");
            return View();
        }
        public string Run()
        {

            return "Current time: " + System.DateTime.Now.ToString() + Handle_Cnb().Result;

        }

        public void RunRobotPingByHttpClient()  //spouští TheRobotUI, aby neusnula IIS website pro MARKTIME, musí být vyplněno v appsettings: RobotHostUrl
        {
            
            if (_app.RobotOnBehind && !string.IsNullOrEmpty(_app.RobotHostUrl)){
                var httpclient = _httpclientfactory.CreateClient();                
                httpclient.GetAsync(_app.RobotHostUrl + "/Robot/Ping");
            }
            
        }

        private async Task<string> Handle_Cnb()
        {
            var httpclient = _httpclientfactory.CreateClient();
            DateTime d0 = DateTime.Now;
            string url = string.Format("http://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.txt?date={0}", BO.BAS.ObjectDate2String(d0, "dd.MM.yyyy"));
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), url))
            {
                
                var response = await httpclient.SendAsync(request);

                var strRet = await response.Content.ReadAsStringAsync();

                LogInfo(strRet);

                return strRet;

            }

            
        }


        private void LogInfo(string strMessage)
        {
            var strPath = string.Format("{0}\\RobotController-{1}.log", _app.LogFolder, DateTime.Now.ToString("yyyy.MM.dd"));

            System.IO.File.AppendAllLines(strPath, new List<string>() { "", DateTime.Now.ToString() + ": ", strMessage });
        }
    }
}
