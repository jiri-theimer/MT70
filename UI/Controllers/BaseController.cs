﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using UI.Models;
using Microsoft.AspNetCore.Http.Extensions;

namespace UI.Controllers    
{
    [Authorize]
    public class BaseController : Controller
    {        
        public BL.Factory Factory;
        public BO.x53PermValEnum MustHavePerm;
        
        


        //Test probíhá před spuštěním každé Akce!
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            //předání přihlášeného uživatele do Factory
            BO.RunningUser ru = (BO.RunningUser)HttpContext.RequestServices.GetService(typeof(BO.RunningUser));
            if (string.IsNullOrEmpty(ru.j03Login))
            {
                ru.j03Login = context.HttpContext.User.Identity.Name;
                
            }
            if (this.Factory == null)
            {
                this.Factory = (BL.Factory)HttpContext.RequestServices.GetService(typeof(BL.Factory));
            }
            

            if (Factory.CurrentUser==null || Factory.CurrentUser.isclosed)
            {
                context.Result = new RedirectResult("~/Login/UserLogin");
                return;
            }
            if (Factory.CurrentUser.j03IsMustChangePassword && context.RouteData.Values["action"].ToString() != "ChangePassword")
            {

                context.Result = new RedirectResult("~/Home/ChangePassword");                
            }

         

            //Příklad přesměrování stránky jinam:
            //context.Result = new RedirectResult("~/Home/Index");

        }
        //Test probíhá po spuštění každé Akce:
        public override void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext context)
        {
            if (ModelState.IsValid == false)
            {
                var modelErrors = new List<string>();
                foreach (var ms in ModelState.Values)
                {
                    foreach (var modelError in ms.Errors)
                    {
                        
                        modelErrors.Add(modelError.ErrorMessage);
                        
                        Factory.CurrentUser.AddMessage(context.HttpContext.Request.Path+" | "+Factory.tra("Kontrola chyb")+": "+modelError.ErrorMessage);
                    }
                }
            }
            
            base.OnActionExecuted(context);
        }


        

        public IActionResult StopPage(bool bolModal,string strMessage)
        {
            var v = new StopPageViewModel() { Message = strMessage, IsModal = bolModal };
            
            return View("_StopPage",v);
        }
        public IActionResult StopPageSubform(string strMessage)
        {
            var v = new StopPageViewModel() { Message = strMessage, IsSubform = true,IsModal=false };

            return View("_StopPage", v);
        }
        
        
        public ViewResult RecNotFound(UI.Models.BaseRecordViewModel v)
        {
            AddMessage("Hledaný záznam neexistuje!","error");            
            return View(v);
        }
        public ViewResult RecNotFound(UI.Models.BaseViewModel v)
        {
            AddMessage("Hledaný záznam neexistuje!", "error");
            return View(v);
        }

        public void Notify_RecNotSaved()
        {
            AddMessage("Záznam zatím nebyl uložen.", "warning");
        }
        public void Notify_RecNotFound()
        {
            AddMessage("Hledaný záznam neexistuje!", "warning");
        }

        public void AddMessage(string strMessage,string template="error")
        {
            
            Factory.CurrentUser.AddMessage(Factory.tra(strMessage), template);
        }
        public void AddMessageTranslated(string strMessage, string template = "error")
        {
            Factory.CurrentUser.AddMessage(strMessage, template);
        }
        public bool TUP(BO.x53PermValEnum oneperm)
        {
            return Factory.CurrentUser.TestPermission(oneperm);
        }

        public virtual ViewResult ViewTup(object model, BO.x53PermValEnum oneperm)
        {
            if (!TUP(oneperm))
            {
                var v = new StopPageViewModel() { Message = "Pro tuto stránku nemáte oprávnění!", IsModal = true };
                return View("_StopPage",v);
            }
            return View(model);
        }
       

        public NavTab AddTab(string strName, string strTabKey, string strUrl,bool istranslate=true,string strBadge=null)
        {
            if (istranslate)
            {
                strName = Factory.tra(strName);
            }
            return new NavTab() { Name = strName, Entity = strTabKey, Url = strUrl,Badge=strBadge };
        }
    }
}