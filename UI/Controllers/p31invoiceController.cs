using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models.p31invoice;
using UI.Models;

namespace UI.Controllers
{
    public class p31invoiceController : BaseController
    {
        public IActionResult Append2Invoice(string pids)
        {
            if (string.IsNullOrEmpty(pids) || BO.BAS.ConvertString2ListInt(pids).Count()==0)
            {
                return this.StopPage(true, "pids missing.");
            }
            var v = new Append2InvoiceViewModel() { pids=pids };

            RefreshState_Append2Invoice(v);
            if (v.lisP31.Count() == 0)
            {
                return this.StopPage(true, "Na vstupu chybí minimálně jeden nevyúčtovaný úkon.");
            }


            return View(v);
        }

        private void RefreshState_Append2Invoice(Append2InvoiceViewModel v)
        {
            var mq = new BO.myQueryP31() { pids = BO.BAS.ConvertString2ListInt(v.pids) };
            mq.isinvoiced = false;
            
            v.lisP31 = Factory.p31WorksheetBL.GetList(mq);
        }

        [HttpPost]
        public IActionResult Append2Invoice(Append2InvoiceViewModel v, string oper)
        {
            RefreshState_Append2Invoice(v);
            if (v.SelectedInvoiceP91ID == 0)
            {
                this.AddMessage("Chybí cílové vyúčtování (faktura).");
                return View(v);
            }
           
            return RedirectToAction("Index", "p31approve",new { pids=v.pids,prefix="p31",p91id=v.SelectedInvoiceP91ID});
        }


        public IActionResult Index(string tempguid)
        {
            if (string.IsNullOrEmpty(tempguid))
            {
                return this.StopPage(true, "guid missing.");
            }
            var v = new GatewayViewModel() { tempguid = tempguid, p91Date = DateTime.Today, p91DateSupply = DateTime.Today, BillingScale = 1, IsDraft = true };


            RefreshState_Index(v);
            


            return View(v);
        }

        private void RefreshState_Index(GatewayViewModel v)
        {
            DateTime datMaturityDef = DateTime.Today.AddDays(Factory.x35GlobalParamBL.LoadParamInt("DefMaturityDays", 10));

            var mq = new BO.myQueryP31() { tempguid = v.tempguid };
            v.lisP31 = Factory.p31WorksheetBL.GetList(mq).OrderBy(p => p.Project);
            if (v.lisP31.Count() > 0)
            {
                if (v.p91Datep31_From==null) v.p91Datep31_From = v.lisP31.Min(p => p.p31Date);
                if (v.p91Datep31_Until==null) v.p91Datep31_Until = v.lisP31.Max(p => p.p31Date);
            }
            if (v.BillingScale == 1 && v.lisP91_Scale1 == null)
            {
                v.lisP91_Scale1 = new List<p91CreateItem>();

                var cc = new p91CreateItem() { DateMaturity = datMaturityDef,TempGUID=BO.BAS.GetGuid() };
                if (v.lisP31.Where(p => p.p28ID_Client > 0).Count() > 0)
                {
                    InhaleClientSetting(v, cc, v.lisP31.Where(p => p.p28ID_Client > 0).First().p28ID_Client);
                }
                foreach (var dbl in v.lisP31.Where(p => p.p31Amount_WithoutVat_Approved != 0).GroupBy(p => p.j27ID_Billing_Orig))
                {
                    cc.WithoutVat += " " + BO.BAS.Number2String(dbl.Sum(p => p.p31Amount_WithoutVat_Approved)) + dbl.First().j27Code_Billing_Orig;
                }
                cc.p41ID = v.lisP31.First().p41ID; cc.p41Name = v.lisP31.First().p41Name;
                if (cc.p41ID > 0)
                {
                    InhaleProjectSetting(v, cc, cc.p41ID);
                }
                v.lisP91_Scale1.Add(cc);
            }
            if (v.BillingScale == 2 && v.lisP91_Scale2 == null)
            {
                v.lisP91_Scale2 = new List<p91CreateItem>();
                var lis = v.lisP31.GroupBy(p => p.p28ID_Client);
                foreach (var c in lis)
                {
                    var cc = new p91CreateItem() { DateMaturity = datMaturityDef,TempGUID=BO.BAS.GetGuid() };
                    if (c.First().p28ID_Client > 0)
                    {
                        InhaleClientSetting(v, cc, c.First().p28ID_Client);
                    }
                    foreach (var dbl in c.Where(p => p.p31Amount_WithoutVat_Approved != 0).GroupBy(p => p.j27ID_Billing_Orig))
                    {
                        cc.WithoutVat += " " + BO.BAS.Number2String(dbl.Sum(p => p.p31Amount_WithoutVat_Approved)) + dbl.First().j27Code_Billing_Orig;
                    }
                    cc.p41ID = c.First().p41ID;cc.p41Name = c.First().p41Name;
                    if (cc.p41ID > 0)
                    {
                        InhaleProjectSetting(v, cc, cc.p41ID);
                    }
                   
                    v.lisP91_Scale2.Add(cc);
                }
            }
            if (v.BillingScale == 3 && v.lisP91_Scale3 == null)
            {
                v.lisP91_Scale3 = new List<p91CreateItem>();
                var lis = v.lisP31.GroupBy(p => p.p41ID);
                foreach (var c in lis)
                {
                    var cc = new p91CreateItem() { DateMaturity = datMaturityDef, p41ID = c.First().p41ID, p41Name = c.First().p41Name,TempGUID=BO.BAS.GetGuid() };
                    cc.p28ID = c.First().p28ID_Client;
                    if (cc.p28ID > 0)
                    {
                        InhaleClientSetting(v, cc, cc.p28ID);

                    }
                    foreach (var dbl in c.Where(p => p.p31Amount_WithoutVat_Approved != 0).GroupBy(p => p.j27ID_Billing_Orig))
                    {
                        cc.WithoutVat += " " + BO.BAS.Number2String(dbl.Sum(p => p.p31Amount_WithoutVat_Approved)) + dbl.First().j27Code_Billing_Orig;
                    }
                    cc.p41ID = c.First().p41ID; cc.p41Name = c.First().p41Name;
                    if (cc.p41ID > 0)
                    {
                        InhaleProjectSetting(v, cc, cc.p41ID);
                    }
                    v.lisP91_Scale3.Add(cc);
                }
            }
        }


        [HttpPost]
        public IActionResult Index(GatewayViewModel v, string oper)
        {
            RefreshState_Index(v);


            switch (oper)
            {
                case "save":
                    
                    var lisP91 = GetInvoiceItems(v);
                    if (lisP91.Count() == 0)
                    {
                        this.AddMessage("Na vstupu chybí položky vyúčtování.");
                        return View(v);
                    }
                    
                    if (Save(v))
                    {
                        v.SetJavascript_CallOnLoad(1);
                        return View(v);
                    }

                    return View(v);
                case "append2invoice":
                    if (v.SelectedInvoiceP91ID == 0)
                    {
                        this.AddMessage("Chybí cílové vyúčtování (faktura).");
                        return View(v);
                    }
                    var recP91 = Factory.p91InvoiceBL.Load(v.SelectedInvoiceP91ID);
                    var dispP91 = Factory.p91InvoiceBL.InhaleRecDisposition(recP91);
                    if (!dispP91.OwnerAccess)
                    {
                        foreach(var rec in v.lisP31)
                        {
                            var dispP41 = Factory.p41ProjectBL.InhaleRecDisposition(Factory.p41ProjectBL.Load(rec.p41ID));
                            if (!dispP41.p91_DraftCreate)
                            {
                                this.AddMessageTranslated(rec.Project+": "+Factory.tra("V projektu nemáte oprávnění vytvářet vyúčtování."));                                
                            }
                            if (!recP91.p91IsDraft && rec.p71ID==BO.p71IdENUM.Schvaleno && rec.p72ID_AfterApprove==BO.p72IdENUM.Fakturovat)
                            {
                                this.AddMessageTranslated(rec.Project + ": " + Factory.tra("S vaším oprávněním můžete do tohoto vyúčtování vkládat pouze úkony s nulovou fakturační cenou!"));
                            }
                        }
                    }
                    if (Factory.p31WorksheetBL.Append2Invoice(v.SelectedInvoiceP91ID, v.lisP31.Select(p => p.pid).ToList()))
                    {
                        v.SetJavascript_CallOnLoad(1);
                        return View(v);
                    }
                    return View(v);

            }


            return View(v);
        }

        private List<p91CreateItem> GetInvoiceItems(GatewayViewModel v)
        {
            var lis = new List<p91CreateItem>();
            switch (v.BillingScale)
            {
                case 1:
                    lis = v.lisP91_Scale1;
                    break;
                case 2:
                    lis = v.lisP91_Scale2;
                    break;
                case 3:
                    lis = v.lisP91_Scale3;
                    break;

            }

            return lis;
        }

       

        private p91CreateItem InhaleClientSetting(GatewayViewModel v, p91CreateItem ret, int p28id)
        {

            if (p28id == 0)
            {
                return ret;
            }

            var recP28 = Factory.p28ContactBL.Load(p28id);
            ret.p28ID = recP28.pid;
            ret.p28Name = recP28.p28name;
            ret.p28ID_Invoice = ret.p28ID;
            ret.p28Name_Invoice = ret.p28Name;

            if (recP28.p28InvoiceMaturityDays > 0)
            {
                ret.DateMaturity = DateTime.Today.AddDays(recP28.p28InvoiceMaturityDays);
            }
            if (recP28.p92ID > 0)
            {
                var recP92 = Factory.p92InvoiceTypeBL.Load(recP28.p92ID);
                ret.p92ID = recP92.pid;
                ret.p92Name = recP92.p92Name;
                ret.InvoiceText1 = recP92.p92InvoiceDefaultText1;
                ret.InvoiceText2 = recP92.p92InvoiceDefaultText2;
            }
            if (recP28.p28InvoiceDefaultText1 != null)
            {
                ret.InvoiceText1 = recP28.p28InvoiceDefaultText1;
            }
            if (recP28.p28InvoiceDefaultText2 != null)
            {
                ret.InvoiceText2 = recP28.p28InvoiceDefaultText2;
            }
            ret.BillingMemo = recP28.p28BillingMemo;

            if (ret.InvoiceText1 != null && v.p91Datep31_From != null)
            {
                ret.InvoiceText1 = Factory.CBL.MergeTextWithOneDate(ret.InvoiceText1, Convert.ToDateTime(v.p91Datep31_From), Convert.ToDateTime(v.p91Datep31_Until));
            }
            if (ret.InvoiceText2 != null && v.p91Datep31_From != null)
            {
                ret.InvoiceText2 = Factory.CBL.MergeTextWithOneDate(ret.InvoiceText2, Convert.ToDateTime(v.p91Datep31_From), Convert.ToDateTime(v.p91Datep31_Until));
            }

            return ret;
        }

        private void InhaleProjectSetting(GatewayViewModel v, p91CreateItem ret,int p41id)
        {
            var recP41 = Factory.p41ProjectBL.Load(p41id);
            if (recP41.p28ID_Billing > 0)
            {
                ret.p28ID_Invoice = recP41.p28ID_Billing;
                ret.p28Name_Invoice = Factory.p28ContactBL.Load(recP41.p28ID_Billing).p28name;
            }
               

        }


        private bool Save(GatewayViewModel v)
        {
            var lis = GetInvoiceItems(v);
            
            if (v.p91Date == null || v.p91Datep31_From == null || v.p91Datep31_Until == null || v.p91DateSupply == null)
            {
                this.AddMessage("Datumy [Vystavení], [Plnění], [Začátek] a [Konec] jsou povinná k vyplnění.");
                return false;
            }
            foreach(var rec in lis)
            {
                if (rec.p28ID == 0)
                {
                    this.AddMessage("Ve vyúčtování chybí vazba na klienta.");
                    return false;
                }
                if (rec.p92ID == 0)
                {
                    this.AddMessage("Ve vyúčtování chybí vazba na typ faktury.");
                    return false;
                }
            }
            int intLastP28ID = 0;
            int intLastP41ID = 0;
            string strGUID = v.tempguid;string strLastGUID = null;
            IEnumerable<BO.p85Tempbox> lisTemp = null;
            foreach (var c in v.lisP31.OrderBy(p => p.p28ID_Client).ThenBy(p => p.p41ID))
            {
                if (v.BillingScale == 2 && c.p28ID_Client != intLastP28ID)
                {
                    strGUID = lis.Where(p => p.p28ID == c.p28ID_Client).First().TempGUID;
                }
                if (v.BillingScale == 3 && c.p41ID != intLastP41ID)
                {
                    strGUID = lis.Where(p => p.p41ID == c.p41ID).First().TempGUID;
                }
                if (strGUID != strLastGUID)
                {
                    lisTemp = Factory.p85TempboxBL.GetList(strGUID, false, "p31");
                }
                if (lisTemp.Where(p => p.p85DataPID == c.pid).Count() == 0)
                {
                    var rec = new BO.p85Tempbox() { p85GUID = strGUID, p85DataPID = c.pid, p85Prefix = "p31" };
                    Factory.p85TempboxBL.Save(rec);
                }

                intLastP28ID = c.p28ID_Client;
                intLastP41ID = c.p41ID;
                strLastGUID = strGUID;
            }

            var errs = new List<int>();

            foreach(var rec in lis)
            {
                var c = new BO.p91Create() {TempGUID=rec.TempGUID, DateIssue = Convert.ToDateTime(v.p91Date),DateSupply=Convert.ToDateTime(v.p91DateSupply),DateP31_From=Convert.ToDateTime(v.p91Datep31_From),DateP31_Until=Convert.ToDateTime(v.p91Datep31_Until) };
                c.p28ID = rec.p28ID_Invoice;
                
                c.p92ID = rec.p92ID;
                c.InvoiceText1 = rec.InvoiceText1;c.InvoiceText2 = rec.InvoiceText2;
                c.DateMaturity = rec.DateMaturity;
                c.IsDraft = v.IsDraft;
                if (Factory.p91InvoiceBL.Create(c) == 0)
                {
                    errs.Add(1);
                }
            }

            if (errs.Count() > 0)
            {
                this.AddMessage("Při generování vyúčtování došlo k chybám.");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
