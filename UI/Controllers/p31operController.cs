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
        public IActionResult RateSimulation(int flag,int p41id,int j02id,int p32id,string d)
        {
            if (!Factory.CurrentUser.IsRatesAccess)
            {
                return this.StopPage(true, "Nemáte oprávnění vidět sazby úkonů.");
            }
            var v = new p31RateViewModel() { j02ID = j02id, p41ID = p41id, p32ID = p32id, d = BO.BAS.String2Date(d) };
            if (v.p41ID==0 || v.p32ID == 0)
            {
                return this.StopPage(true, "Na vstupu chybí projekt nebo aktivita.");
            }
            v.RecP41 = Factory.p41ProjectBL.Load(v.p41ID);
            v.p51ID_BillingRate = v.RecP41.p51ID_Billing;
            v.p51ID_CostRate =v.RecP41.p51ID_Internal;
            if (v.p51ID_BillingRate == 0 && v.RecP41.p28ID_Client>0)
            {
                v.p51ID_BillingRate = Factory.p28ContactBL.Load(v.RecP41.p28ID_Client).p51ID_Billing;
            }

            var c = Factory.p31WorksheetBL.LoadRate(BO.p51TypeFlagENUM.BillingRates, BO.BAS.String2Date(d), j02id, p41id, p32id);
            v.BillRate = c.Value;
            v.j27Code_BillingRate = c.j27Code;
            if (c.Value != 0 && v.p51ID_BillingRate==0)
            {
                //root ceník fakturačních sazeb
                var recP51Root=Factory.p51PriceListBL.LoadRootPriceList(DateTime.Now);
                v.p51ID_BillingRate = recP51Root.pid;

            }
            c = Factory.p31WorksheetBL.LoadRate(BO.p51TypeFlagENUM.CostRates, BO.BAS.String2Date(d), j02id, p41id, p32id);
            v.CostRate = c.Value;
            v.j27Code_CostRate = c.j27Code;

            

            return View(v);
        }

        //Uživatelovo nastavení formátu vykazování hodin
        public IActionResult hes()
        {
            var v = new hesViewModel() {HoursFormat = Factory.CurrentUser.j03DefaultHoursFormat, TotalFlagValue = Factory.CurrentUser.j03HoursEntryFlagV7 };
            v.InhaleSetting();

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

                if (v.ActivityFlag == 99) v.TotalFlagValue += 1024;
                if (v.ActivityFlag == 0) v.TotalFlagValue += 2048;
                if (v.ActivityFlag == 1) v.TotalFlagValue += 4096;
                if (v.ActivityFlag == 2) v.TotalFlagValue += 8196;

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


        public BO.Result UpdateText(int p31id, string s)
        {
            var rec = Factory.p31WorksheetBL.Load(p31id);
            var disp=Factory.p31WorksheetBL.InhaleRecDisposition(rec);
            if (disp.OwnerAccess && disp.RecordState == BO.p31RecordState.Editing)
            {
                if (Factory.p31WorksheetBL.UpdateText(p31id, s))
                {
                    return new BO.Result(false);
                }
                else
                {
                    return new BO.Result(true, Factory.GetFirstNotifyMessage());
                }
            }
            else
            {
                return new BO.Result(true, Factory.tra("Chybí oprávnění k editaci úkonu"));
            }
            
        }
    }


    
}
