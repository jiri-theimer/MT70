using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class j03Controller : BaseController
    {
        
        public IActionResult Record(int pid, bool isclone,int j02id)
        {
            var v = new j03Record() { rec_pid = pid, rec_entity = "j03" };
            if (v.rec_pid == 0)
            {
                v.IsDefinePassword = true;
                v.user_profile_oper = "create";
                var c = new BO.CLS.PasswordChecker();
                v.NewPassword = c.RandomPassword(Factory.App.PasswordMinLength);
                v.VerifyPassword = v.NewPassword;                
            }
            else
            {
                v.user_profile_oper = "bind";
            }
            v.lisB65 = Factory.b65WorkflowMessageBL.GetList(new BO.myQuery("b65")).Where(p => p.x29ID == 503);

            v.Rec = new BO.j03User();
            v.Rec.j03LangIndex = Factory.App.DefaultLangIndex;
            v.RecJ02 = new BO.j02Person();            

            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j03UserBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboPerson = v.Rec.PersonDesc;
                v.ComboJ04Name = v.Rec.j04Name;
                
                if (v.Rec.j02ID == 0)
                {
                    v.user_profile_oper = "nobind"; //uživatel bez osobního profilu
                }
            }
            else
            {
                if (j02id > 0)
                {
                    v.RecJ02 = Factory.j02PersonBL.Load(j02id);                    
                    v.ComboPerson = v.RecJ02.FullNameDesc;
                    v.user_profile_oper = "bind";
                    v.Rec.j02ID = v.RecJ02.pid;
                }
            }

           

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.j03Record v,string oper,int b65id)
        {
            v.lisB65 = Factory.b65WorkflowMessageBL.GetList(new BO.myQuery("b65")).Where(p => p.x29ID == 503);
            if (oper == "postback")
            {
                return View(v);
            }
            
            if (oper== "newpwd")
            {
                v.IsDefinePassword = true;
                var c = new BO.CLS.PasswordChecker();
                v.NewPassword = c.RandomPassword(Factory.App.PasswordMinLength);
                v.VerifyPassword = v.NewPassword;
                return View(v);
            }
            if (oper == "changelogin")
            {
                v.IsDefinePassword = true;
                v.IsChangeLogin = true;
                var c = new BO.CLS.PasswordChecker();
                v.NewPassword = c.RandomPassword(Factory.App.PasswordMinLength);
                v.VerifyPassword = v.NewPassword;
                this.AddMessage("Se změnou přihlašovacího jména je třeba resetovat i přístupové heslo.","info");
                return View(v);
            }
            if (ModelState.IsValid)
            {
                if (v.user_profile_oper=="bind" && v.Rec.j02ID==0)
                {
                    this.AddMessage("U uživatelského účtu chybí vazba na osobní profil."); return View(v);
                }
                
                BO.j03User c = new BO.j03User();
                if (v.rec_pid > 0) c = Factory.j03UserBL.Load(v.rec_pid);                
                c.j03Login = v.Rec.j03Login;
                c.j04ID = v.Rec.j04ID;
                c.j03LangIndex = v.Rec.j03LangIndex;
                
                c.j03IsMustChangePassword = v.Rec.j03IsMustChangePassword;
                c.j03IsSystemAccount = v.Rec.j03IsSystemAccount;
                c.j03LangIndex = v.Rec.j03LangIndex;
                c.j03IsDebugLog = v.Rec.j03IsDebugLog;
               
                    
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                if (!ValidatePreSave(c))
                {
                    return View(v);
                }
                if (v.IsDefinePassword)
                {
                    if (ValidateUserPassword(v) == false)
                    {
                        return View(v);
                    }
                    var lu = new BO.LoggingUser();
                    c.j03PasswordHash = lu.Pwd2Hash(v.NewPassword, c);
                }
               
                switch (v.user_profile_oper){
                    case "create":
                        c.j02ID = 0;    //zakládáme nový osobní profil
                        var cJ02 = new BO.j02Person()
                        {
                            j02Email = v.RecJ02.j02Email
                            ,
                            j02TitleBeforeName = v.RecJ02.j02TitleBeforeName
                            ,
                            j02FirstName = v.RecJ02.j02FirstName
                            ,
                            j02LastName = v.RecJ02.j02LastName
                            ,
                            j02TitleAfterName = v.RecJ02.j02TitleAfterName
                            ,
                            j02Code = v.RecJ02.j02Code
                            ,
                            j02Mobile = v.RecJ02.j02Mobile
                            ,
                            j02Phone = v.RecJ02.j02Phone
                        };
                        if (!Factory.j02PersonBL.ValidateBeforeSave(cJ02))
                        {
                            return View(v);
                        }
                        c.pid = Factory.j03UserBL.SaveWithNewPersonalProfile(c, cJ02);                        
                        break;
                    case "bind":
                        c.j02ID = v.Rec.j02ID;  //uložení s existujícím osobním profilem                        
                        c.pid = Factory.j03UserBL.Save(c);
                        break;
                    default:
                        //účet bez vazby na osobní profil
                        c.j02ID = 0;
                        c.j03IsSystemAccount = true;
                        c.pid = Factory.j03UserBL.Save(c);
                        break;
                }

                

                if (v.rec_pid == 0 && c.pid>0 && v.IsDefinePassword)
                {
                    c = Factory.j03UserBL.Load(c.pid);  //zakládáme nový účet - je třeba pře-generovat j03PasswordHash
                    var lu = new BO.LoggingUser();
                    c.j03PasswordHash = lu.Pwd2Hash(v.NewPassword, c);
                    c.pid = Factory.j03UserBL.Save(c);
                }
                
                if (c.pid > 0)
                {
                    if (oper == "save_and_send")
                    {
                        return Redirect("/Mail/SendMail?x29id=503&j02id=" + c.j02ID.ToString() + "&recpid=" + c.pid.ToString() + "&b65id=" + b65id.ToString()+ "&param1="+v.NewPassword);
                    }
                    else
                    {
                        v.SetJavascript_CallOnLoad(c.pid);
                        return View(v);
                    }
                    
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        private bool ValidateUserPassword(j03Record v)
        {            
            var c = new BO.CLS.PasswordChecker();
            var res=c.CheckPassword(v.NewPassword, Factory.App.PasswordMinLength, Factory.App.PasswordMaxLength, Factory.App.PasswordRequireDigit, Factory.App.PasswordRequireUppercase, Factory.App.PasswordRequireLowercase, Factory.App.PasswordRequireNonAlphanumeric);
            if (res.Flag == BO.ResultEnum.Failed)
            {
                this.AddMessage(res.Message);return false;
            }
            if (v.NewPassword != v.VerifyPassword)
            {
                this.AddMessage("Heslo nesouhlasí s jeho ověřením.");return false;
            }
            
            return true;
        }
        private bool ValidatePreSave(BO.j03User recJ03)
        {
            if (string.IsNullOrEmpty(recJ03.j03Login) || recJ03.j04ID==0)
            {
                this.AddMessage("Přihlašovací jméno (login) a Aplikační role jsou povinná pro uživatelský účet.");return false;
            }
            
            return true;
        }

        
        
    }
}