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
            var c = new UI.Menu.TheMenuSupport(Factory);
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

        

        public IActionResult About()
        {

            

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
            var c = new BL.bas.PasswordChecker();
            var res = c.CheckPassword(v.NewPassword);
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
                var mq = new BO.myQueryJ11() { j02id = Factory.CurrentUser.j02ID };                
                v.Teams = string.Join(", ", Factory.j11TeamBL.GetList(mq).Where(p=>p.j11IsAllPersons==false).Select(p => p.j11Name));
            }

            RefreshState_MyProfile(v);


            return View(v);
        }
        private void RefreshState_MyProfile(Models.MyProfileViewModel v)
        {
            string strMyQuery = "j02id|int|"+Factory.CurrentUser.j02ID.ToString();
          
            v.gridinputO40 = new TheGridInput() { entity = "o40SmtpAccount", myqueryinline = strMyQuery };
            v.gridinputO40.query = new BO.InitMyQuery().Load("o40", null, 0, strMyQuery);

            var c = new UI.Menu.TheMenuSupport(Factory);
            v.lisMenuLinks = c.getUserMenuLinks();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MyProfile(Models.MyProfileViewModel v, string oper)
        {
            RefreshState_MyProfile(v);
            if (oper == "clearparams")
            {
                Factory.j03UserBL.TruncateUserParams(0);
                this.AddMessage("Server cache vyčištěna.", "info");
                return MyProfile();
            }
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
                if (Factory.j02PersonBL.Save(c,null,null) > 0)
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
