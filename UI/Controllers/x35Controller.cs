using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class x35Controller : BaseController
    {
        public IActionResult x35Params()
        {
            var v = new x35ParamsViewModel();
            v.AppName = Factory.x35GlobalParamBL.LoadParam("AppName");
            v.AppHost = Factory.x35GlobalParamBL.LoadParam("AppHost");
            v.j27ID_Domestic = Factory.x35GlobalParamBL.LoadParamInt("j27ID_Domestic",0);
            if (v.j27ID_Domestic > 0)
            {
                v.ComboJ27_Domestic = Factory.FBL.LoadCurrencyByID(v.j27ID_Domestic).j27Code;
            }
            v.Round2Minutes= Factory.x35GlobalParamBL.LoadParamInt("Round2Minutes",0);
            v.DefMaturityDays = Factory.x35GlobalParamBL.LoadParamInt("DefMaturityDays",0);
            if (Factory.x35GlobalParamBL.LoadParamInt("IsAllowPasswordRecovery", 0) == 1)
            {
                v.IsAllowPasswordRecovery = true;
            }
            v.COUNTRY_CODE = Factory.x35GlobalParamBL.LoadParam("COUNTRY_CODE");
            v.Upload_Folder = Factory.x35GlobalParamBL.LoadParam("Upload_Folder");
            v.cp_odesilatel= Factory.x35GlobalParamBL.LoadParam("cp_odesilatel");
            v.cp_podavatel = Factory.x35GlobalParamBL.LoadParam("cp_podavatel");           
            
            var lisP87 = Factory.FBL.GetListP87();
            foreach (var c in lisP87)
            {
                switch (c.p87LangIndex)
                {
                    case 1:
                        v.BillingLang1 = c.p87Name; v.BillingIcon1 = c.p87Icon;break;
                    case 2:
                        v.BillingLang2 = c.p87Name; v.BillingIcon2 = c.p87Icon; break;
                    case 3:
                        v.BillingLang3 = c.p87Name; v.BillingIcon3 = c.p87Icon; break;
                    case 4:
                        v.BillingLang4 = c.p87Name; v.BillingIcon4 = c.p87Icon; break;

                }
            }

            RefreshState(v);

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

        private void RefreshState(x35ParamsViewModel v)
        {
            v.LangFlags = BO.BASFILE.GetFileListFromDir(Factory.App.AppRootFolder + "\\wwwroot\\images\\flags", "*.gif");
            
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult x35Params(x35ParamsViewModel v,string oper)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {
                bool bolOK = true;
                
                Factory.x35GlobalParamBL.Save("AppName", v.AppName);
                Factory.x35GlobalParamBL.Save("AppHost", v.AppHost);
                Factory.x35GlobalParamBL.Save("j27ID_Domestic", v.j27ID_Domestic.ToString());
                Factory.x35GlobalParamBL.Save("Round2Minutes", v.Round2Minutes.ToString());
                Factory.x35GlobalParamBL.Save("COUNTRY_CODE", v.COUNTRY_CODE);
                Factory.x35GlobalParamBL.Save("DefMaturityDays", v.DefMaturityDays.ToString());
                Factory.x35GlobalParamBL.Save("IsAllowPasswordRecovery", BO.BAS.GB(v.IsAllowPasswordRecovery));
                Factory.x35GlobalParamBL.Save("Upload_Folder", v.Upload_Folder);
                Factory.x35GlobalParamBL.Save("cp_odesilatel", v.cp_odesilatel);

                var lisP87 = Factory.FBL.GetListP87();
                foreach(var c in lisP87)
                {
                    switch (c.p87LangIndex)
                    {
                        case 1:
                            c.p87Name = v.BillingLang1; c.p87Icon = v.BillingIcon1; break;
                        case 2:
                            c.p87Name = v.BillingLang2; c.p87Icon = v.BillingIcon2; break;
                        case 3:
                            c.p87Name = v.BillingLang3; c.p87Icon = v.BillingIcon3; break;
                        case 4:
                            c.p87Name = v.BillingLang4; c.p87Icon = v.BillingIcon4; break;
                    }
                    Factory.FBL.SaveP87(c);
                }
                if (bolOK)
                {

                    v.SetJavascript_CallOnLoad(1);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}
