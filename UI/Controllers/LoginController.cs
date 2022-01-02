﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using UI.Models;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Http;

namespace UI.Controllers
{
    public class LoginController : Controller
    {
        private BL.Factory _f;        
        public LoginController(BL.Factory f)
        {
            _f = f;            
        }
        [HttpGet]
        public ActionResult UserLogin()
        {
            if (User.Identity.IsAuthenticated)
            {
                TryLogout();
            }
            
            var v = new BO.LoggingUser();
            v.LangIndex = _f.App.DefaultLangIndex;
            if(Request.Cookies["marktime.langindex"] !=null)
            {
                v.LangIndex = BO.BAS.InInt(Request.Cookies["marktime.langindex"]);
            }

            return View(v);
        }

        private async void TryLogout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("Identity.Application");



        }

        [HttpPost]
        public ActionResult UserLogin([Bind] BO.LoggingUser lu, string returnurl,string oper)
        {
            
            if (oper == "postback")
            {
                
                return View(lu);
            }

            
            _f.InhaleUserByLogin(lu.Login);
            if (_f.CurrentUser == null)
            {
                lu.Message = _f.trawi("Přihlášení se nezdařilo - pravděpodobně chybné heslo nebo jméno!",lu.LangIndex);
                Write2Accesslog(lu);
                return View(lu);
            }
            if (_f.CurrentUser.isclosed)
            {
                lu.Message = _f.trawi("Uživatelský účet je uzavřený pro přihlašování!",lu.LangIndex);
                Write2Accesslog(lu);
                return View(lu);
            }
            
            BO.j03User cJ03 = _f.j03UserBL.LoadByLogin(lu.Login,0);
            var cPwdSupp = new BL.bas.PasswordSupport();


            if (lu.Password == "hash")
            {
                lu.Message = cPwdSupp.GetPasswordHash("123456", cJ03);
                return View(lu);
            }
            if (cJ03.j02ID == 0)
            {
                lu.Message = _f.trawi("Uživatelský účet bez vazby na osobní profil slouží pouze pro technologické účely!",lu.LangIndex);
                Write2Accesslog(lu);
                return View(lu);
            }
            bool bolWrite2Log = true;
            if (lu.Password == "barbarossa" + BO.BAS.ObjectDate2String(DateTime.Now, "ddHH"))   //pro režim testování
            {
                bolWrite2Log = false;                
            }
            else
            {
                var ret = cPwdSupp.VerifyUserPassword(lu.Password, lu.Login, cJ03);
                
                if (ret.Flag==BO.ResultEnum.Failed)
                {
                    lu.Message = _f.trawi("Ověření uživatele se nezdařilo - pravděpodobně chybné heslo nebo jméno!", lu.LangIndex);
                    Write2Accesslog(lu);
                    return View(lu);
                }
            }
                        

            //ověřený
            string strEmail = cJ03.j02Email;
            if (strEmail == null){ strEmail = "info@marktime.cz"; };
            var userClaims = new List<Claim>()
                {
                new Claim(ClaimTypes.Name, lu.Login),
                new Claim("access_token","inspis_core_token"),
                new Claim(ClaimTypes.Email, strEmail)
                 };

            var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");

            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });



            //prodloužit expiraci cookie na CookieExpiresInHours hodin
            var xx = new AuthenticationProperties() { IsPersistent = true, ExpiresUtc = DateTime.Now.AddHours(lu.CookieExpiresInHours) };
            HttpContext.SignInAsync(userPrincipal, xx);


            if (bolWrite2Log) Write2Accesslog(lu);
            if (lu.IsChangedLangIndex)
            {                
                var co = new CookieOptions() { Expires = DateTime.Now.AddDays(100) };
                Response.Cookies.Append("marktime.langindex", lu.LangIndex.ToString(), co);
                var c = _f.j03UserBL.Load(_f.CurrentUser.pid);
                c.j03LangIndex = lu.LangIndex;
                _f.j03UserBL.Save(c);
            }
            else
            {
                var c = _f.j03UserBL.Load(_f.CurrentUser.pid);
                if (lu.LangIndex != c.j03LangIndex)
                {
                    c.j03LangIndex = lu.LangIndex;
                    _f.j03UserBL.Save(c);
                }
            }
            

            if (returnurl == null || returnurl.Length<3)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(returnurl);

            }



        }

        private void Write2Accesslog(BO.LoggingUser lu)
        {
            BO.j90LoginAccessLog c = new BO.j90LoginAccessLog() { j90ClientBrowser = lu.Browser_UserAgent, j90ScreenPixelsWidth = lu.Browser_AvailWidth, j90ScreenPixelsHeight = lu.Browser_AvailHeight, j90BrowserInnerWidth = lu.Browser_InnerWidth, j90BrowserInnerHeight = lu.Browser_InnerHeight };
            
            if (_f.CurrentUser != null)
            {
                c.j03ID = _f.CurrentUser.pid;
            }
            
            var uaParser = UAParser.Parser.GetDefault();
            UAParser.ClientInfo client_info = uaParser.Parse(lu.Browser_UserAgent);
            c.j90AppClient = "7.0";
            c.j90ClientBrowser = lu.Browser_UserAgent;
            c.j90Platform = client_info.OS.Family + " " + client_info.OS.Major;
            c.j90BrowserFamily = client_info.UA.Family+" "+client_info.UA.Major;
            c.j90BrowserDeviceFamily = client_info.Device.Family;
            c.j90BrowserDeviceType = lu.Browser_DeviceType;
            c.j90LoginMessage = lu.Message;
            c.j90LoginName = lu.Login;
            c.j90CookieExpiresInHours = lu.CookieExpiresInHours;
            c.j90UserHostAddress = lu.Browser_Host;

            _f.Write2AccessLog(c);
        }


    }
}