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
        //private readonly BL.TheEntitiesProvider _ep;
        private readonly BL.RunningApp _app;
        //private readonly BL.TheTranslator _tt;

        private BL.Factory _f;

        public RobotController(IHttpClientFactory hcf, BL.RunningApp app, BL.TheEntitiesProvider ep, BL.TheTranslator tt)
        {
            _httpclientfactory = hcf;
            //_ep = ep;
            _app = app;
            //_tt = tt;

            var ru = new BO.RunningUser() { j03Login = "lamos" };
            _f = new BL.Factory(ru, _app, ep, tt);

        }

       

        public IActionResult Index()
        {

            var v = new RobotViewModel();

            v.MessageToUser = "Current time: " + System.DateTime.Now.ToString();


            return View(v);
        }
        public IActionResult Ping()
        {
            LogInfo("Ping",BO.j91RobotTaskFlag.PingTestOnly);
            return View();
        }
        public string Run()    //úvodní metoda pro spuštění robota
        {

            

            if (IsTime4Run(BO.j91RobotTaskFlag.CnbKurzy, 15, 19,60))
            {
                string ss=Handle_Cnb().Result;
            }

            return "Current time: " + System.DateTime.Now.ToString();

        }

        private bool IsTime4Run(BO.j91RobotTaskFlag flag,double hour_from,double hour_until, double dblPoKolikaMinutachPoustet)
        {
            if (!(DateTime.Today.AddHours(hour_from)<=DateTime.Now && DateTime.Today.AddHours(hour_until) >= DateTime.Now))
            {
                return false;
            }
            var c = _f.FBL.GetLastRobotRun(flag);
            if (c == null)
            {
                return true;
            }
            if (c.j91Date.AddMinutes(dblPoKolikaMinutachPoustet) > DateTime.Now)
            {
                return false;
            }
            return true;
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

                LogInfo("čnb",BO.j91RobotTaskFlag.CnbKurzy);

               

                return strRet;

            }

            
        }


        private void LogInfo(string strMessage,BO.j91RobotTaskFlag flag,string strError=null)
        {
            var c = new BO.j91RobotLog() {j91InfoMessage=strMessage, j91Date = DateTime.Now,j91TaskFlag=flag,j91ErrorMessage=strError,j91Account=_f.CurrentUser.j03Login };

            _f.FBL.AppendRobotLog(c);

            var strPath = string.Format("{0}\\RobotController-{1}.log", _app.LogFolder, DateTime.Now.ToString("yyyy.MM.dd"));

            System.IO.File.AppendAllLines(strPath, new List<string>() { "", DateTime.Now.ToString() + ": ", strMessage });
        }
    }
}
