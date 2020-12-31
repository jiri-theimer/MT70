﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using UI.Models;

namespace UI.Controllers
{
    public class HomeController : BaseController
    {
       

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
    }
}
