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

        private DateTime _d;

        private BL.Factory _f;

        public RobotController(IHttpClientFactory hcf, BL.RunningApp app, BL.TheEntitiesProvider ep, BL.TheTranslator tt)
        {
            _d = DateTime.Today;
            _httpclientfactory = hcf;            
            _app = app;
         
            var ru = new BO.RunningUser() { j03Login = "lamos" };
            _f = new BL.Factory(ru, _app, ep, tt);

        }


        public IActionResult Ping()
        {
            LogInfo("Ping", BO.j91RobotTaskFlag.PingTestOnly);
            return View();
        }
        public IActionResult Index(int explicitflag,string explicitdate)    //spuštění robota ručně přes prohlížeč
        {
            if (!string.IsNullOrEmpty(explicitdate))
            {
                _d = BO.BAS.String2Date(explicitdate);
            }
            var v = new RobotViewModel();

            v.MessageToUser = "Current time: " + System.DateTime.Now.ToString();

            if (explicitflag == 0)
            {
                Run(BO.j91RobotTaskFlag.Start);
            }
            else
            {
                Run((BO.j91RobotTaskFlag) explicitflag);
            }

            v.lisLast20 = _f.FBL.GetListRobotLast20();
            
            return View(v);
        }
        
        public string Run(BO.j91RobotTaskFlag explicitflag)    //úvodní metoda pro spuštění robota
        {

            if (explicitflag==BO.j91RobotTaskFlag.CnbKurzy || IsTime4Run(BO.j91RobotTaskFlag.CnbKurzy, 15, 19,60))
            {
                Handle_Cnb();
            }
            if (explicitflag == BO.j91RobotTaskFlag.PingTestOnly)
            {
                Ping();
            }

            Handle_ScheduledReports();

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

        private void Handle_Cnb()
        {
            var httpclient = _httpclientfactory.CreateClient();
            var errs = new List<string>();
            var succs = new List<string>();
            var strJ27Codes = _f.x35GlobalParamBL.LoadParam("j27Codes_Import_CNB");
            if (string.IsNullOrEmpty(strJ27Codes)) return;
            foreach(string strJ27Code in BO.BAS.ConvertString2List(strJ27Codes))
            {
                var recJ27 = _f.FBL.LoadCurrencyByCode(strJ27Code);
                if (_f.m62ExchangeRateBL.LoadByQuery(BO.m62RateTypeENUM.InvoiceRate, recJ27.j27ID, 2, _d, 0) == null)
                {
                    if (_f.m62ExchangeRateBL.ImportOneRate(httpclient, _d, recJ27.j27ID) == 0)
                    {
                        errs.Add("ERROR import: " + recJ27.j27Code);
                    }
                    else
                    {
                        succs.Add(strJ27Code);
                    }
                }
                
            }

            LogInfo(string.Join(", ",succs), BO.j91RobotTaskFlag.CnbKurzy,string.Join(", ",errs));
            
        }

        private void Handle_ScheduledReports()
        {

            var lis = _f.x31ReportBL.GetList(new BO.myQuery("x31")).Where(p => p.x31IsScheduling && p.x31SchedulingReceivers != null && p.x29ID == BO.x29IdEnum._NotSpecified);
            foreach (var recX31 in lis)
            {
                if (_f.x31ReportBL.IsReportWaiting4Generate(_d.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute), recX31))
                {
                    string strFullPath = GeneratePdfReport(recX31);

                }
            }

        }
        
        
        private string GeneratePdfReport(BO.x31Report rec)
        {
            var uriReportSource = new Telerik.Reporting.UriReportSource();
            uriReportSource.Uri = _f.x35GlobalParamBL.ReportFolder() + "\\" + _f.x31ReportBL.LoadReportDoc(rec.pid).o27ArchiveFileName;
            DateTime d1 = new DateTime(2000, 1, 1);
            DateTime d2 = new DateTime(3000, 1, 1);

            uriReportSource.Parameters.Add("j02id", _f.CurrentUser.j02ID);                        
            uriReportSource.Parameters.Add("datfrom", d1);
            uriReportSource.Parameters.Add("datuntil", d2);
            
            Telerik.Reporting.Processing.ReportProcessor processor = new Telerik.Reporting.Processing.ReportProcessor(_f.App.Configuration);

            var result = processor.RenderReport("PDF", uriReportSource, null);
            
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ms.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
            ms.Seek(0, System.IO.SeekOrigin.Begin);

            string strPdfFileName = BO.BAS.GetGuid() + ".pdf";
            BO.BASFILE.SaveStream2File(_f.x35GlobalParamBL.TempFolder()+"\\" + strPdfFileName, ms);
            return _f.x35GlobalParamBL.TempFolder()+"\\"+strPdfFileName;
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
