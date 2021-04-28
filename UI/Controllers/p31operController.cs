using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.p31oper;

namespace UI.Controllers
{
    public class p31operController : BaseController
    {
        //Nastavení vykazování hodin
        public IActionResult hes(string pagesource)
        {
            var v = new hesViewModel() {PageSource= pagesource, HoursFormat = Factory.CurrentUser.j03DefaultHoursFormat, TotalFlagValue = Factory.CurrentUser.j03HoursEntryFlagV7 };
            if (BO.BAS.bit_compare_or(v.TotalFlagValue, 2)) v.HoursInterval = 30;
            if (BO.BAS.bit_compare_or(v.TotalFlagValue, 4)) v.HoursInterval = 60;
            if (BO.BAS.bit_compare_or(v.TotalFlagValue, 8)) v.HoursInterval = 5;
            if (BO.BAS.bit_compare_or(v.TotalFlagValue, 16)) v.HoursInterval = 10;
            if (BO.BAS.bit_compare_or(v.TotalFlagValue, 32)) v.HoursInterval = 6;
            if (BO.BAS.bit_compare_or(v.TotalFlagValue, 64)) v.HoursInterval = 15;
            if (BO.BAS.bit_compare_or(v.TotalFlagValue, 128)) v.TimesheetEntryByMinutes = true;
            if (BO.BAS.bit_compare_or(v.TotalFlagValue, 256)) v.OfferTrimming = true;
            if (BO.BAS.bit_compare_or(v.TotalFlagValue, 512)) v.OfferContactPerson = true;

            return View(v);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult hes(hesViewModel v, string oper)
        {
            v.TotalFlagValue = Factory.CurrentUser.j03HoursEntryFlagV7;

            if (oper != null)
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {
                v.TotalFlagValue = 0;                
                if (v.HoursInterval == 30) v.TotalFlagValue += 2;
                if (v.HoursInterval == 60) v.TotalFlagValue += 4;
                if (v.HoursInterval == 5) v.TotalFlagValue += 8;
                if (v.HoursInterval == 10) v.TotalFlagValue += 16;
                if (v.HoursInterval == 6) v.TotalFlagValue += 32;
                if (v.HoursInterval == 15) v.TotalFlagValue += 64;
                if (v.TimesheetEntryByMinutes) v.TotalFlagValue += 128;
                if (v.OfferTrimming) v.TotalFlagValue += 256;
                if (v.OfferContactPerson) v.TotalFlagValue += 512;

                Factory.CurrentUser.j03HoursEntryFlagV7 = v.TotalFlagValue;
                Factory.CurrentUser.j03DefaultHoursFormat = v.HoursFormat;

                if (Factory.j03UserBL.Save(Factory.CurrentUser)==0)
                {
                    return View(v);
                }
                v.SetJavascript_CallOnLoad(0);
                return View(v);
            }

            this.Notify_RecNotSaved();
            return View(v);
        }


    }
}
