using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using UI.Models;
using Winnovative.PDFMerge;

namespace UI.Controllers
{
    public class HomeController : BaseController
    {
        private BL.TheReportOnFly _reponlfy;
        public HomeController(BL.TheReportOnFly reponfly)
        {
            _reponlfy = reponfly;
        }
        //private basUI.ReportOnflyConfiguration _reponlfy;
        //public HomeController(basUI.ReportOnflyConfiguration reponfly)
        //{
        //    _reponlfy = reponfly;
        //}
        public IActionResult Index()
        {

            if (HttpContext.Request.Path.Value.Length <= 1)
            {
                //úvodní spuštění: otestovat nastavení domovské stránky
                if (!string.IsNullOrEmpty(Factory.CurrentUser.j03HomePageUrl))
                {

                    return Redirect(Factory.CurrentUser.j03HomePageUrl);  //pryč na jinou stránku
                }
            }

            //return RedirectToAction("Widgets", "Dashboard");

            return View(new BaseViewModel());
        }
        public IActionResult MyMainMenuLinks()
        {
            var v = new MyMainMenuLinks();
            v.SelectedItems = Factory.CurrentUser.j03SiteMenuMyLinksV7;
            RefreshState_MyMainMenuLinks(v);
            

            return View(v);
        }

        private void RefreshState_MyMainMenuLinks(MyMainMenuLinks v)
        {
            var c = new basUI.TheMenuSupport(Factory);
            v.lisCil = c.getUserMenuLinks();
            v.lisZdroj = c.getAllMainMenuLinks();
            foreach (var rec in v.lisCil)
            {
                if (v.lisZdroj.Any(p => p.Url == rec.Url))
                {
                    v.lisZdroj.Remove(v.lisZdroj.Where(p => p.Url == rec.Url).First());
                }
            }

            
        }
        [HttpPost]
        public IActionResult MyMainMenuLinks(MyMainMenuLinks v)
        {
            RefreshState_MyMainMenuLinks(v);
            if (string.IsNullOrEmpty(v.SelectedItems))
            {
                this.AddMessage("Musíte vybrat minimálně jeden menu odkaz.");
                return View(v);
            }
            if (ModelState.IsValid)
            {
                var rec = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
                rec.j03SiteMenuMyLinksV7 = v.SelectedItems;
                if (Factory.j03UserBL.Save(rec)>0)
                {
                    v.SetJavascript_CallOnLoad(rec.pid);
                }
                
            }

            return View(v);
        }

        private string GeneratePdfReport(string strTrdxPath,string strPdfFileName)
        {
            var uriReportSource = new Telerik.Reporting.UriReportSource();
            //uriReportSource.Uri = Factory.x35GlobalParamBL.ReportFolder() + "\\report_j02_nastaveni.trdx";
            uriReportSource.Uri = strTrdxPath;

            uriReportSource.Parameters.Add("j02id", Factory.CurrentUser.j02ID);
            DateTime? d1 = Factory.CBL.LoadUserParamDate("report-period-d1").Value;
            if (d1 == null) d1 = new DateTime(2000, 1, 1);
            DateTime? d2 = Factory.CBL.LoadUserParamDate("report-period-d2").Value;
            if (d2 == null) d2 = new DateTime(3000, 1, 1);

            uriReportSource.Parameters.Add("datfrom", d1);
            uriReportSource.Parameters.Add("datuntil", d2);

            Telerik.Reporting.Processing.ReportProcessor processor = new Telerik.Reporting.Processing.ReportProcessor(_reponlfy.Configuration);



            var result = processor.RenderReport("PDF", uriReportSource, null);
            //var result = processor.RenderReport("PDF", uriReportSource, null);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            ms.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);

            ms.Seek(0, System.IO.SeekOrigin.Begin);

            BO.BASFILE.SaveStream2File("c:\\temp\\"+ strPdfFileName, ms);

            return strPdfFileName;
        }

        public IActionResult About()
        {
            GeneratePdfReport(Factory.x35GlobalParamBL.ReportFolder() + "\\report_p31_wip_podleklientu_vc_honorare.trdx","report1.pdf");
            GeneratePdfReport(Factory.x35GlobalParamBL.ReportFolder() + "\\report_p31_podrobny_vypis_hodin_vc_faktpoznamky_na_vysku.trdx", "report3.pdf");

            

            GeneratePdfReport(Factory.x35GlobalParamBL.ReportFolder() + "\\report_j02_nastaveni.trdx", "report2.pdf");

            //var uriReportSource = new Telerik.Reporting.UriReportSource();
            ////uriReportSource.Uri = Factory.x35GlobalParamBL.ReportFolder() + "\\report_j02_nastaveni.trdx";
            //uriReportSource.Uri = Factory.x35GlobalParamBL.ReportFolder() + "\\report_p31_wip_podleklientu_vc_honorare.trdx";

            //uriReportSource.Parameters.Add("j02id", Factory.CurrentUser.j02ID);
            //DateTime? d1 = Factory.CBL.LoadUserParamDate("report-period-d1").Value;
            //if (d1 == null) d1 = new DateTime(2000, 1, 1);
            //DateTime? d2 = Factory.CBL.LoadUserParamDate("report-period-d2").Value;
            //if (d2 == null) d2 = new DateTime(3000, 1, 1);

            //uriReportSource.Parameters.Add("datfrom", d1);
            //uriReportSource.Parameters.Add("datuntil", d2);

            //Telerik.Reporting.Processing.ReportProcessor processor = new Telerik.Reporting.Processing.ReportProcessor(_reponlfy.Configuration);

            

            //var result = processor.RenderReport("PDF", uriReportSource, null);
            ////var result = processor.RenderReport("PDF", uriReportSource, null);
            
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
           
            //ms.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);            

            //ms.Seek(0, System.IO.SeekOrigin.Begin);

            //BO.BASFILE.SaveStream2File("c:\\temp\\marktime_report.pdf", ms);


            //PDF merge po rumunsku:
            PdfDocumentOptions pdfDocumentOptions = new PdfDocumentOptions();
            pdfDocumentOptions.PdfCompressionLevel = PDFCompressionLevel.Normal;
            pdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            PDFMerge pdfMerge = new PDFMerge(pdfDocumentOptions);

            pdfMerge.AppendPDFFile("c:\\temp\\report1.pdf");
            pdfMerge.AppendPDFFile("c:\\temp\\report3.pdf");
            pdfMerge.AppendPDFFile("c:\\temp\\report2.pdf");

            pdfMerge.SaveMergedPDFToFile("c:\\temp\\result.pdf");

            return View(new BaseViewModel());
        }
        public async Task<IActionResult> Logout()
        {            
            await HttpContext.SignOutAsync("Identity.Application");

            return View();

        }

        public IActionResult ChangePassword()
        {
            var v = new ChangePasswordViewModel();
            if (Factory.CurrentUser.j03IsMustChangePassword)
            {
                this.AddMessage("Administrátor nastavil, že si musíte změnit přihlašovací heslo.", "info");
            }
            return View(v);
        }
        [HttpPost]
        public IActionResult ChangePassword(Models.ChangePasswordViewModel v)
        {
            var c = new BO.CLS.PasswordChecker();
            var res = c.CheckPassword(v.NewPassword, Factory.App.PasswordMinLength, Factory.App.PasswordMaxLength, Factory.App.PasswordRequireDigit, Factory.App.PasswordRequireUppercase, Factory.App.PasswordRequireLowercase, Factory.App.PasswordRequireNonAlphanumeric);
            if (res.Flag == BO.ResultEnum.Failed)
            {
                this.AddMessage(res.Message); return View(v);
            }
            if (v.NewPassword != v.VerifyPassword)
            {
                this.AddMessage("Heslo nesouhlasí s jeho ověřením."); return View(v);
            }

            var cJ03 = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            var lu = new BO.LoggingUser();

            res = lu.VerifyHash(v.CurrentPassword, cJ03.j03Login, cJ03);
            if (res.Flag == BO.ResultEnum.Success)
            {
                cJ03.j03PasswordHash = lu.Pwd2Hash(v.NewPassword, cJ03);
                cJ03.j03IsMustChangePassword = false;
                if (Factory.j03UserBL.Save(cJ03) > 0)
                {
                    this.AddMessage("Heslo bylo změněno.", "info");
                    return RedirectToAction("Index");
                }

            }
            else
            {
                this.AddMessage(res.Message);
            }
            return View(v);

        }

        public IActionResult MyProfile()
        {
            if (!Factory.CurrentUser.j04IsMenu_MyProfile)
            {
                return this.StopPage(false, "Nemáte oprávnění k této funkci.");
            }
            var v = new MyProfileViewModel();
            v.userAgent = Request.Headers["User-Agent"];

            var uaParser = UAParser.Parser.GetDefault();
            v.client_info = uaParser.Parse(v.userAgent);
            v.RecJ02 = Factory.j02PersonBL.Load(Factory.CurrentUser.j02ID);
            v.RecJ03 = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            v.CurrentUser = Factory.CurrentUser;
            v.j03ModalWindowsFlag = v.CurrentUser.j03ModalWindowsFlag;
            if (v.CurrentUser.j03GridSelectionModeFlag == 1)
            {
                v.IsGridClipboard = true;
            }
            v.EmailAddres = v.RecJ02.j02Email;
            v.Mobile = v.RecJ02.j02Mobile;
            v.Phone = v.RecJ02.j02Phone;
            v.Office = v.RecJ02.j02Office;
            v.j02NotifySubscriberFlag = v.RecJ02.j02NotifySubscriberFlag;
            v.j02EmailSignature = v.RecJ02.j02EmailSignature;
            if (!string.IsNullOrEmpty(Factory.CurrentUser.j11IDs))
            {
                var mq = new BO.myQuery("j11");
                mq.SetPids(Factory.CurrentUser.j11IDs);
                v.Teams = string.Join(", ", Factory.j11TeamBL.GetList(mq).Select(p => p.j11Name));
            }
            
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MyProfile(Models.MyProfileViewModel v)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(v.EmailAddres) == true)
                {
                    this.AddMessage("Chybí e-mail adresa.");
                    return MyProfile();
                }
                BO.j02Person c = Factory.j02PersonBL.Load(Factory.CurrentUser.j02ID);
                c.j02Email = v.EmailAddres;
                c.j02Mobile = v.Mobile;
                c.j02Phone = v.Phone;
                c.j02Office = v.Office;
                c.j02NotifySubscriberFlag = v.j02NotifySubscriberFlag;
                c.j02EmailSignature = v.j02EmailSignature;
                if (Factory.j02PersonBL.Save(c) > 0)
                {
                    BO.j03User cUser = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
                    if (v.IsGridClipboard == true)
                    {
                        cUser.j03GridSelectionModeFlag = 1;
                    }
                    else
                    {
                        cUser.j03GridSelectionModeFlag = 0;
                    }
                    cUser.j03ModalWindowsFlag = v.j03ModalWindowsFlag;
                    Factory.j03UserBL.Save(cUser);
                    Factory.CurrentUser.AddMessage("Změny uloženy", "info");
                }

            }
            return MyProfile();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public BO.Result SaveCurrentUserHomePage(string homepageurl)
        {
            if (!String.IsNullOrEmpty(homepageurl))
            {
                if (homepageurl.Substring(0, 1) != "/")
                {
                    homepageurl = "/" + homepageurl;
                }
                if (homepageurl.Contains("/RecPage"))
                {   //ořezat parametry za otazníkem
                    homepageurl = homepageurl.Split("?")[0];
                }
                else
                {
                    homepageurl = homepageurl.Replace("pid=", "xxx=");
                }
            }

            var c = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            c.j03HomePageUrl = homepageurl;
            Factory.j03UserBL.Save(c);
            return new BO.Result(false);
        }

        public BO.Result SaveCurrentUserFontSize(int fontsize)
        {
            var c = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            c.j03GlobalCssFlag = fontsize;
            Factory.j03UserBL.Save(c);
            return new BO.Result(false);
        }
        public BO.Result SaveCurrentUserLangIndex(int langindex)
        {
            var c = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            c.j03LangIndex = langindex;
            Factory.j03UserBL.Save(c);
            var co = new CookieOptions() { Expires = DateTime.Now.AddDays(100) };
            Response.Cookies.Append("marktime.langindex", langindex.ToString(), co);
            return new BO.Result(false);
        }

        public BO.Result UpdateCurrentUserPing(BO.j92PingLog c)
        {
            var uaParser = UAParser.Parser.GetDefault();
            UAParser.ClientInfo client_info = uaParser.Parse(c.j92BrowserUserAgent);
            c.j92BrowserOS = client_info.OS.Family + " " + client_info.OS.Major;
            c.j92BrowserFamily = client_info.UA.Family + " " + client_info.UA.Major;
            c.j92BrowserDeviceFamily = client_info.Device.Family;

            Factory.j03UserBL.UpdateCurrentUserPing(c);

            return new BO.Result(false);
        }
    }
}
