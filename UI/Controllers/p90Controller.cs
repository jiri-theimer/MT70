using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;
using UI.Models.Tab1;


namespace UI.Controllers
{
    public class p90Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            return Tab1(pid);
        }
        public IActionResult Tab1(int pid)
        {
            var v = new p90Tab1() { Factory = this.Factory, prefix = "p90", pid = pid };
            RefreshStateTab1(v);
            return View(v);
        }
        private void RefreshStateTab1(p90Tab1 v)
        {
            v.Rec = Factory.p90ProformaBL.Load(v.pid);
            if (v.Rec != null)
            {
                //v.RecSum = Factory.p28ContactBL.LoadSumRow(v.Rec.pid);

                v.SetTagging();
                v.lisP82 = Factory.p82Proforma_PaymentBL.GetList(v.pid);
                v.lisP99 = Factory.p91InvoiceBL.GetList_p99(v.pid, 0, 0);
                v.SetFreeFields(0);
            }
        }

       

        public IActionResult Record(int pid, bool isclone)
        {
            var v = new p90Record() { rec_pid = pid, rec_entity = "p90" };
            v.Rec = new BO.p90Proforma();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p90ProformaBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                if (!InhalePermissions(v))
                {
                    return this.StopPage(true, "Nemáte oprávnění k záznamu.");
                }

                v.SetTagging(Factory.o51TagBL.GetTagging("p90", v.rec_pid));

                v.ComboP89Name = v.Rec.p89Name;
                v.ComboJ27Code = v.Rec.j27Code;
                v.ComboP28Name = v.Rec.p28Name;
                v.ComboOwner = v.Rec.Owner;
                if (v.Rec.j19ID > 0)
                {                    
                    v.ComboJ19Name = Factory.FBL.LoadJ19(v.Rec.j19ID).j19Name;
                }

            }
            else
            {
                v.Rec.p90Date = DateTime.Today;v.Rec.j02ID_Owner = Factory.CurrentUser.j02ID;v.ComboOwner = Factory.CurrentUser.PersonDesc;
                var recLast = Factory.p90ProformaBL.LoadMyLastCreated();
                if (recLast != null)
                {
                    v.Rec.j27ID = recLast.j27ID;v.ComboJ27Code = recLast.j27Code;v.Rec.p89ID = recLast.p89ID;v.ComboP89Name = recLast.p89Name;v.Rec.p90VatRate = recLast.p90VatRate;
                }
            }

            RefreshStateRecord(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
                v.Rec.p90Code = null;
            }

            return View(v);
        }

        private void RefreshStateRecord(p90Record v)
        {
            if (v.ff1 == null)
            {
                v.ff1 = new FreeFieldsViewModel();
                v.ff1.InhaleFreeFieldsView(Factory, v.rec_pid, "p90");
            }
            v.ff1.RefreshInputsVisibility(Factory, v.rec_pid, "p90", v.Rec.p89ID);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p90Record v, string oper)
        {
            RefreshStateRecord(v);
            
            if (oper == "recalc1" && v.Rec.p90VatRate>0)  //dopočítat z částky bez DPH
            {
                v.Rec.p90Amount_Vat = v.Rec.p90Amount_WithoutVat * v.Rec.p90VatRate / 100;
                v.Rec.p90Amount = v.Rec.p90Amount_WithoutVat + v.Rec.p90Amount_Vat;                
            }
            if (oper == "recalc2" && v.Rec.p90VatRate>0)  //dopočítat z celkové částky
            {
                v.Rec.p90Amount_WithoutVat = v.Rec.p90Amount / (1 + v.Rec.p90VatRate / 100);
                v.Rec.p90Amount_Vat = v.Rec.p90Amount - v.Rec.p90Amount_WithoutVat;                
            }
            if (oper != null)
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.p90Proforma c = new BO.p90Proforma();
                if (v.rec_pid > 0) c = Factory.p90ProformaBL.Load(v.rec_pid);
                c.p28ID = v.Rec.p28ID;
                c.j27ID = v.Rec.j27ID;
                c.p89ID = v.Rec.p89ID;
                c.p90Date = v.Rec.p90Date;
                c.p90DateMaturity = v.Rec.p90DateMaturity;
                c.j19ID = v.Rec.j19ID;

                c.p90Amount = v.Rec.p90Amount;
                c.p90Amount_WithoutVat = v.Rec.p90Amount_WithoutVat;
                c.p90Amount_Vat = v.Rec.p90Amount_Vat;
                c.p90VatRate = v.Rec.p90VatRate;

                c.p90Text1 = v.Rec.p90Text1;
                c.p90Text2 = v.Rec.p90Text2;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                c.j02ID_Owner = v.Rec.j02ID_Owner;

                c.pid = Factory.p90ProformaBL.Save(c, v.ff1.inputs);
                if (c.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p90", c.pid, v.TagPids);

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }


        private bool InhalePermissions(p90Record v)
        {
            var mydisp = Factory.p90ProformaBL.InhaleRecDisposition(v.Rec);
            if (!mydisp.ReadAccess)
            {
                return false;
            }
            if (v.Rec.x38ID > 0)
            {
                v.CanEditRecordCode = Factory.x38CodeLogicBL.CanEditRecordCode(v.Rec.x38ID, mydisp);
            }
            else
            {
                v.CanEditRecordCode = mydisp.OwnerAccess;
            }
            return true;
        }
    }
}
