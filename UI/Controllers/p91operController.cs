using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.p91oper;

namespace UI.Controllers
{
    public class p91operController : BaseController
    {
        BL.TheColumnsProvider _cp;
        public p91operController(BL.TheColumnsProvider cp)
        {
            _cp = cp;
        }
        //Převést kompletně na jinou sazbu DPH
        public IActionResult vat(int p91id)
        {
            var v = new vatViewModel() { p91ID = p91id };
            if (v.p91ID == 0)
            {
                return this.StopPage(true, "Na vstupu chybí faktura.");
            }
            RefreshStateVAT(v);


            return View(v);
        }

        private void RefreshStateVAT(vatViewModel v)
        {
            v.RecP91 = Factory.p91InvoiceBL.Load(v.p91ID);
            v.gridinput = new TheGridInput() { entity = "p31Worksheet", master_entity = "inform", myqueryinline = "p91id|int|" + v.p91ID.ToString(), oncmclick = "", ondblclick = "" };
            v.gridinput.query = new BO.InitMyQuery().Load("p31", null, 0, "p91id|int|" + v.p91ID.ToString());
            v.gridinput.fixedcolumns = "p31Date,p31_j02__j02Person__fullname_desc,p31_p41__p41Project__p41Name,p31_p32__p32Activity__p32Name,p31Rate_Billing_Invoiced,p31Amount_WithoutVat_Invoiced,p31VatRate_Invoiced,p31Text";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult vat(vatViewModel v, string oper)
        {
            RefreshStateVAT(v);
            if (oper != null)
            {
                return View(v);
            }
            
            if (ModelState.IsValid)
            {
                if ((int)v.SelectedX15ID == 0)
                {
                    this.AddMessage("Musíte vybrat cílovou DPH hladinu.");return View(v);
                }
                
                if (!Factory.p91InvoiceBL.ChangeVat(v.p91ID,(int)v.SelectedX15ID, v.VatRate))
                {
                    return View(v);
                }
                v.SetJavascript_CallOnLoad(v.p91ID);
                return View(v);
            }

            this.Notify_RecNotSaved();
            return View(v);
        }


        //Aktualizovat měnový kurz faktury
        public IActionResult exupdate(int p91id)
        {
            var v = new exupdateViewModel() { p91ID = p91id };
            if (v.p91ID == 0)
            {
                return this.StopPage(true, "Na vstupu chybí faktura.");
            }
            v.RecP91 = Factory.p91InvoiceBL.Load(v.p91ID);
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult exupdate(exupdateViewModel v, string oper)
        {
            v.RecP91 = Factory.p91InvoiceBL.Load(v.p91ID);
            if (oper != null)
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {
                Factory.p91InvoiceBL.ClearExchangeDate(v.p91ID,true);
                v.SetJavascript_CallOnLoad(v.p91ID);
                return View(v);
            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        //Převést na jinou měnu:
        public IActionResult j27(int p91id)
        {
            var v = new j27ViewModel() { p91ID = p91id };
            if (v.p91ID == 0)
            {
                return this.StopPage(true, "Na vstupu chybí faktura.");
            }
            RefreshStateJ27(v);
            
            return View(v);
        }

        private void RefreshStateJ27(j27ViewModel v)
        {            
            if (v.RecP91 == null)
            {
                v.RecP91 = Factory.p91InvoiceBL.Load(v.p91ID);
            }
            v.lisJ27 = Factory.FBL.GetListCurrency();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult j27(j27ViewModel v, string oper)
        {
            RefreshStateJ27(v);
           
            if (oper != null)
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {      
                if (v.SelectedJ27ID == 0)
                {
                    this.AddMessage("Musíte vybrat cílovou měnu vyúčtování.");return View(v);
                }
                if (Factory.p91InvoiceBL.ChangeCurrency(v.p91ID, v.SelectedJ27ID))
                {
                    v.SetJavascript_CallOnLoad(v.p91ID);
                    return View(v);
                }
            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        //Zapsat úhradu faktury:
        public IActionResult p94(int p91id)
        {
            var v = new p94ViewModel() { p91ID = p91id };
            if (v.p91ID == 0)
            {
                return this.StopPage(true, "Na vstupu chybí faktura.");
            }
            RefreshStateP94(v);
            v.Rec = new BO.p94Invoice_Payment() { p94Date = DateTime.Today,p94Amount=v.RecP91.p91Amount_Debt };

            
            return View(v);
        }

        private void RefreshStateP94(p94ViewModel v)
        {
            if (v.lisP94 == null)
            {
                v.lisP94 = Factory.p91InvoiceBL.GetList_p94(v.p91ID);
            }
            if (v.RecP91 == null)
            {
                v.RecP91 = Factory.p91InvoiceBL.Load(v.p91ID);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult p94(p94ViewModel v,string oper,int p94id)
        {
            RefreshStateP94(v);

            if (oper=="delete" && p94id > 0)
            {
                if (Factory.p91InvoiceBL.DeleteP94(p94id,v.p91ID))
                {
                    v.SetJavascript_CallOnLoad(v.p91ID);
                    return View(v);
                }
            }

            if (oper != null)
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {
                var c = new BO.p94Invoice_Payment() { p91ID = v.p91ID,p94Amount=v.Rec.p94Amount,p94Date=v.Rec.p94Date,p94Description=v.Rec.p94Description };
                c.pid = Factory.p91InvoiceBL.SaveP94(c);
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(v.p91ID);
                    return View(v);
                }

            }

            this.Notify_RecNotSaved();
            return View(v);
        }

    }
}
