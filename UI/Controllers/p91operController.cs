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

        //hromadné operace nad položkami vyúčtování
        public IActionResult p31operbatch(string baseoper,int p91id,string p31ids)
        {
            var v = new p31operbatchViewModel() { p91ID = p91id,p31ids=p31ids,BaseOper= baseoper };
            if (v.p91ID == 0 || string.IsNullOrEmpty(v.BaseOper))
            {
                return this.StopPage(true,"p91id or baseoper missing");
            }
            if (string.IsNullOrEmpty(v.p31ids))
            {
                return this.StopPage(true, "Na vstupu chybí úkony.");
            }
            if (v.BaseOper == "p70-6") v.SelectedOper = 6;
            if (v.BaseOper == "p70-2") v.SelectedOper = 2;
            if (v.BaseOper == "p70-3") v.SelectedOper = 3;
            RefreshStateOperBatch(v);

            return View(v);
        }
        private void RefreshStateOperBatch(p31operbatchViewModel v)
        {
            v.RecP91 = Factory.p91InvoiceBL.Load(v.p91ID);
            var pids = BO.BAS.ConvertString2ListInt(v.p31ids);
            v.lisP31 = Factory.p31WorksheetBL.GetList(new BO.myQueryP31() { pids = pids });

            v.gridinput = new TheGridInput() { entity = "p31Worksheet", master_entity = "inform", myqueryinline = "pids|list_int|" + v.p31ids, oncmclick = "", ondblclick = "" };
            v.gridinput.query = new BO.InitMyQuery().Load("p31", null, 0, "pids|list_int|" + v.p31ids);
            v.gridinput.fixedcolumns = "p31Date,p31_j02__j02Person__fullname_desc,p31_p41__p41Project__p41Name,p31_p32__p32Activity__p32Name,p31Rate_Billing_Invoiced,p31Amount_WithoutVat_Invoiced,p31VatRate_Invoiced,p31Text";

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult p31operbatch(p31operbatchViewModel v, string oper)
        {
            RefreshStateOperBatch(v);
            if (oper != null)
            {
                return View(v);
            }
            
            if (ModelState.IsValid)
            {
                var p31ids = v.lisP31.Select(p => p.pid).ToList();
                if (v.BaseOper == "remove")
                {
                    if (v.SelectedOper == 0)
                    {
                        this.AddMessage("Chybí cílový stav úkonu."); return View(v);
                    }
                    
                    if (!Factory.p31WorksheetBL.RemoveFromInvoice(v.RecP91.pid, p31ids))
                    {
                        return View(v);
                    }
                    switch (v.SelectedOper)
                    {
                        case 1:
                            break;
                        case 2:
                            Factory.p31WorksheetBL.RemoveFromApprove(p31ids);
                            break;
                        case 3:
                            Factory.p31WorksheetBL.RemoveFromApprove(p31ids);
                            Factory.p31WorksheetBL.Move2Bin(true, p31ids);
                            break;
                    }
                }
                if (v.BaseOper.Substring(0, 3) == "p70")
                {
                    var lis = new List<BO.p31WorksheetInvoiceChange>();
                    foreach(var c in v.lisP31)
                    {
                        lis.Add(new BO.p31WorksheetInvoiceChange() { p31ID = c.pid, p70ID =(BO.p70IdENUM) v.SelectedOper });
                    }
                    if (!Factory.p31WorksheetBL.UpdateInvoice(v.p91ID, lis, null))
                    {
                        return View(v);
                    }
                }
                
                v.SetJavascript_CallOnLoad(p31ids.First());
                return View(v);
            }

            this.Notify_RecNotSaved();
            return View(v);
        }


        //přesunout položku do jiného vyúčtování
        public IActionResult p31move2invoice(int p31id)
        {
            var v = new p31move2invoiceViewModel() { p31ID = p31id };
            if (v.p31ID == 0)
            {
                return this.StopPage(true, "Na vstupu chybí úkon.");
            }
            RefreshStateMove2Invoice(v);

            return View(v);
        }
        private void RefreshStateMove2Invoice(p31move2invoiceViewModel v)
        {
            v.RecP91 = Factory.p91InvoiceBL.LoadByP31ID(v.p31ID);
            v.Rec = Factory.p31WorksheetBL.Load(v.p31ID);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult p31move2invoice(p31move2invoiceViewModel v, string oper)
        {
            RefreshStateMove2Invoice(v);
            if (oper != null)
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {                               
                if (!Factory.p31WorksheetBL.Move2Invoice(v.SelectedP91ID, v.p31ID))
                {
                    return View(v);
                }
               
                v.SetJavascript_CallOnLoad(v.p31ID);
                return View(v);
            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        //vyjmout položku z vyúčtování
        public IActionResult p31remove(int p31id)
        {
            var v = new p31removeViewModel() { p31ID = p31id };
            if (v.p31ID == 0)
            {
                return this.StopPage(true, "Na vstupu chybí úkon.");
            }
            RefreshStateRemove(v);
                        
            return View(v);
        }
        private void RefreshStateRemove(p31removeViewModel v)
        {
            v.RecP91 = Factory.p91InvoiceBL.LoadByP31ID(v.p31ID);
            v.Rec = Factory.p31WorksheetBL.Load(v.p31ID);            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult p31remove(p31removeViewModel v, string oper)
        {
            RefreshStateRemove(v);
            if (oper != null)
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {
                if (v.SelectedOper == 0)
                {
                    this.AddMessage("Chybí cílový stav úkonu."); return View(v);
                }
                var p31ids = new List<int> { v.p31ID };
                if (!Factory.p31WorksheetBL.RemoveFromInvoice(v.RecP91.pid, p31ids))
                {
                    return View(v);
                }

                switch (v.SelectedOper)
                {
                    case 1:                        
                        break;
                    case 2:                        
                        Factory.p31WorksheetBL.RemoveFromApprove(p31ids);
                        break;
                    case 3:
                        Factory.p31WorksheetBL.RemoveFromApprove(p31ids);
                        Factory.p31WorksheetBL.Move2Bin(true, p31ids);
                        break;
                }

                
                
                v.SetJavascript_CallOnLoad(v.p31ID);
                return View(v);
            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        //upravit položku vyúčtování
        public IActionResult p31edit(int p31id)
        {
            var v = new p31editViewModel() { p31ID = p31id};
            if (v.p31ID == 0)
            {
                return this.StopPage(true, "Na vstupu chybí úkon.");
            }
            RefreshStateEdit(v);
            v.p31Text = v.Rec.p31Text;
            v.SelectedP70ID = v.Rec.p70ID;
            switch (v.Rec.p33ID)
            {
                case BO.p33IdENUM.Cas:
                    v.p31Value_Invoiced = v.Rec.p31Hours_Invoiced;
                    v.Hours = v.Rec.p31Hours_Invoiced.ToString();
                    v.Hours_FixPrice = v.Rec.p31Value_FixPrice.ToString();
                    v.p31Rate_Billing_Invoiced = v.Rec.p31Rate_Billing_Invoiced;

                    break;
                case BO.p33IdENUM.Kusovnik:
                    v.p31Value_Invoiced = v.Rec.p31Hours_Invoiced;
                    v.p31Rate_Billing_Invoiced = v.Rec.p31Rate_Billing_Invoiced;
                    break;
                default:                    
                    break;
            }
            
            v.p31VatRate_Invoiced = v.Rec.p31VatRate_Invoiced;
            v.p31Amount_WithoutVat_Invoiced = v.Rec.p31Amount_WithoutVat_Invoiced;
            v.p31Text = v.Rec.p31Text;
            var tg = Factory.o51TagBL.GetTagging("p31", v.p31ID);
            v.TagPids = tg.TagPids;
            v.TagNames = tg.TagNames;
            v.TagHtml = tg.TagHtml;
            return View(v);
        }
        private void RefreshStateEdit(p31editViewModel v)
        {
            v.RecP91 = Factory.p91InvoiceBL.LoadByP31ID(v.p31ID);
            v.Rec = Factory.p31WorksheetBL.Load(v.p31ID);

            if (v.ff1 == null)
            {
                v.ff1 = new FreeFieldsViewModel();
                v.ff1.InhaleFreeFieldsView(Factory, v.p31ID, "p31");
            }
            v.ff1.RefreshInputsVisibility(Factory, v.p31ID, "p31", v.Rec.p34ID);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult p31edit(p31editViewModel v, string oper)
        {
            RefreshStateEdit(v);
            if (oper != null)
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {                
                var c = new BO.p31WorksheetInvoiceChange() { p31ID = v.Rec.pid,p33ID=v.Rec.p33ID,p32ManualFeeFlag=v.Rec.p32ManualFeeFlag };
                c.p31IsInvoiceManual = true;    //částky úkonu se odvíjejí z tohoto formuláře a nikoliv ze schválených hodnot
                c.TextUpdate = v.p31Text;
                c.p70ID = v.SelectedP70ID;
                c.p31Code = v.p31Code;

                if (v.SelectedP70ID == BO.p70IdENUM.Vyfakturovano)
                {
                    c.InvoiceVatRate = v.p31VatRate_Invoiced;

                    switch (v.Rec.p33ID)
                    {
                        case BO.p33IdENUM.Cas:
                            c.InvoiceValue = BO.basTime.ShowAsDec(v.Hours);
                            c.InvoiceRate = v.p31Rate_Billing_Invoiced;                           
                            break;
                        case BO.p33IdENUM.Kusovnik:
                            c.InvoiceValue = v.p31Value_Invoiced;
                            c.InvoiceRate = v.p31Rate_Billing_Invoiced;
                            break;
                        default:
                            c.InvoiceValue = v.p31Amount_WithoutVat_Invoiced;
                            break;
                    }
                }
                if (v.SelectedP70ID == BO.p70IdENUM.ZahrnutoDoPausalu)
                {
                    if (v.Rec.p33ID == BO.p33IdENUM.Cas)
                    {
                        c.FixPriceValue = BO.basTime.ShowAsDec(v.Hours_FixPrice);
                    }
                }
                
                

                var lis = new List<BO.p31WorksheetInvoiceChange>();
                lis.Add(c);

                if (Factory.p31WorksheetBL.UpdateInvoice(v.RecP91.pid,lis, v.ff1.inputs))
                {
                    Factory.o51TagBL.SaveTagging("p31", c.p31ID, v.TagPids);

                    v.SetJavascript_CallOnLoad(v.p31ID);
                    return View(v);
                }
                
                
            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        //odstranit vyúčtování
        public IActionResult p91delete(int p91id)
        {
            var v = new p91deleteViewModel() { p91ID = p91id,TempGuid=BO.BAS.GetGuid() };
            if (v.p91ID == 0)
            {
                return this.StopPage(true, "Na vstupu chybí faktura.");
            }
            RefreshStateDelete(v);


            return View(v);
        }
        private void RefreshStateDelete(p91deleteViewModel v)
        {
            v.RecP91 = Factory.p91InvoiceBL.Load(v.p91ID);
            v.gridinput = new TheGridInput() { entity = "p31Worksheet", master_entity = "inform", myqueryinline = "p91id|int|" + v.p91ID.ToString(), oncmclick = "", ondblclick = "" };
            v.gridinput.query = new BO.InitMyQuery().Load("p31", null, 0, "p91id|int|" + v.p91ID.ToString());
            v.gridinput.fixedcolumns = "p31Date,p31_j02__j02Person__fullname_desc,p31_p41__p41Project__p41Name,p31_p32__p32Activity__p32Name,p31Rate_Billing_Invoiced,p31Amount_WithoutVat_Invoiced,p31VatRate_Invoiced,p31Text";
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult p91delete(p91deleteViewModel v, string oper)
        {
            RefreshStateDelete(v);
            if (oper != null)
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {
                if (v.SelectedOper == 0)
                {
                    this.AddMessage("Musíte zvolit, jak naložit s úkony ve vyúčtování."); return View(v);
                }

              
                if (!Factory.p91InvoiceBL.Delete(v.p91ID,v.TempGuid,v.SelectedOper))
                {
                    return View(v);
                }
                v.SetJavascript_CallOnLoad(v.p91ID);
                return View(v);
            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        //Vytvořit dobropis
        public IActionResult creditnote(int p91id)
        {
            var v = new creditnoteViewModel() { p91ID = p91id };
            if (v.p91ID == 0)
            {
                return this.StopPage(true, "Na vstupu chybí faktura.");
            }
            RefreshStateCreditNote(v);


            return View(v);
        }
        private void RefreshStateCreditNote(creditnoteViewModel v)
        {
            v.RecP91 = Factory.p91InvoiceBL.Load(v.p91ID);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult creditnote(creditnoteViewModel v, string oper)
        {
            RefreshStateCreditNote(v);
            if (oper != null)
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {
                if (v.SelectedP92ID == 0)
                {
                    this.AddMessage("Chybí typ opravného dokladu."); return View(v);
                }

                int intPID = Factory.p91InvoiceBL.CreateCreditNote(v.p91ID, v.SelectedP92ID, true);
                if (intPID > 0)
                {
                    v.SetJavascript_CallOnLoad(v.p91ID);
                    return View(v);
                }

            }

            this.Notify_RecNotSaved();
            return View(v);
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
                    this.AddMessage("Musíte vybrat cílovou DPH hladinu."); return View(v);
                }

                if (!Factory.p91InvoiceBL.ChangeVat(v.p91ID, (int)v.SelectedX15ID, v.VatRate))
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
                Factory.p91InvoiceBL.ClearExchangeDate(v.p91ID, true);
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
                    this.AddMessage("Musíte vybrat cílovou měnu vyúčtování."); return View(v);
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
            v.Rec = new BO.p94Invoice_Payment() { p94Date = DateTime.Today, p94Amount = v.RecP91.p91Amount_Debt };


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
        public IActionResult p94(p94ViewModel v, string oper, int p94id)
        {
            RefreshStateP94(v);

            if (oper == "delete" && p94id > 0)
            {
                if (Factory.p91InvoiceBL.DeleteP94(p94id, v.p91ID))
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
                var c = new BO.p94Invoice_Payment() { p91ID = v.p91ID, p94Amount = v.Rec.p94Amount, p94Date = v.Rec.p94Date, p94Description = v.Rec.p94Description };
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
