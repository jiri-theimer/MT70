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
        public IActionResult Index(string tempguid)
        {
            if (string.IsNullOrEmpty(tempguid))
            {
                return this.StopPage(true, "guid missing.");
            }
            var v = new GatewayViewModel() { tempguid = tempguid, p91Date = DateTime.Today, p91DateSupply = DateTime.Today, BillingScale = 1, IsDraft = true };
            if (v.lisP31.Count() > 0)
            {
                v.p91Datep31_From = v.lisP31.Min(p => p.p31Date);
                v.p91Datep31_Until = v.lisP31.Max(p => p.p31Date);
            }

            RefreshState(v);




            return View(v);
        }

        private void RefreshState(GatewayViewModel v)
        {
            DateTime datMaturityDef = DateTime.Today.AddDays(Factory.x35GlobalParamBL.LoadParamInt("DefMaturityDays", 10));

            var mq = new BO.myQueryP31() { tempguid = v.tempguid };
            v.lisP31 = Factory.p31WorksheetBL.GetList(mq).OrderBy(p => p.Project);
            if (v.BillingScale == 1 && v.lisP91_Scale1 == null)
            {
                v.lisP91_Scale1 = new List<p91CreateItem>();

                var cc = new p91CreateItem() { DateMaturity = datMaturityDef };
                if (v.lisP31.Where(p => p.p28ID_Client > 0).Count() > 0)
                {
                    InhaleClientSetting(v, cc, v.lisP31.Where(p => p.p28ID_Client > 0).First().p28ID_Client);
                }
                foreach (var dbl in v.lisP31.Where(p => p.p31Amount_WithoutVat_Approved != 0).GroupBy(p => p.j27ID_Billing_Orig))
                {
                    cc.WithoutVat += " " + BO.BAS.Number2String(dbl.Sum(p => p.p31Amount_WithoutVat_Approved)) + dbl.First().j27Code_Billing_Orig;
                }
                cc.p41ID = v.lisP31.First().p41ID; cc.p41Name = c.First().p41Name;
                v.lisP91_Scale1.Add(cc);
            }
            if (v.BillingScale == 2 && v.lisP91_Scale2 == null)
            {
                v.lisP91_Scale2 = new List<p91CreateItem>();
                var lis = v.lisP31.GroupBy(p => p.p28ID_Client);
                foreach (var c in lis)
                {
                    var cc = new p91CreateItem() { DateMaturity = datMaturityDef };
                    if (c.First().p28ID_Client > 0)
                    {
                        InhaleClientSetting(v, cc, c.First().p28ID_Client);
                    }
                    foreach (var dbl in c.Where(p => p.p31Amount_WithoutVat_Approved != 0).GroupBy(p => p.j27ID_Billing_Orig))
                    {
                        cc.WithoutVat += " " + BO.BAS.Number2String(dbl.Sum(p => p.p31Amount_WithoutVat_Approved)) + dbl.First().j27Code_Billing_Orig;
                    }
                    cc.p41ID = c.First().p41ID;cc.p41Name = c.First().p41Name;

                    v.lisP91_Scale2.Add(cc);
                }
            }
            if (v.BillingScale == 3 && v.lisP91_Scale3 == null)
            {
                v.lisP91_Scale3 = new List<p91CreateItem>();
                var lis = v.lisP31.GroupBy(p => p.p41ID);
                foreach (var c in lis)
                {
                    var cc = new p91CreateItem() { DateMaturity = datMaturityDef, p41ID = c.First().p41ID, p41Name = c.First().p41Name };
                    cc.p28ID = c.First().p28ID_Client;
                    if (cc.p28ID > 0)
                    {
                        InhaleClientSetting(v, cc, cc.p28ID);

                    }
                    foreach (var dbl in c.Where(p => p.p31Amount_WithoutVat_Approved != 0).GroupBy(p => p.j27ID_Billing_Orig))
                    {
                        cc.WithoutVat += " " + BO.BAS.Number2String(dbl.Sum(p => p.p31Amount_WithoutVat_Approved)) + dbl.First().j27Code_Billing_Orig;
                    }

                    v.lisP91_Scale3.Add(cc);
                }
            }
        }


        [HttpPost]
        public IActionResult Index(GatewayViewModel v, string oper)
        {
            RefreshState(v);


            switch (oper)
            {
                case "save":
                    var lisP91 = GetInvoiceItems(v);
                    if (lisP91.Count() == 0)
                    {
                        this.AddMessage("Na vstupu chybí položky vyúčtování.");
                        return View(v);
                    }
                    if (1 == 1)
                    {
                        v.SetJavascript_CallOnLoad(0);

                    }

                    return View(v);

            }


            return View(v);
        }

        private List<p91CreateItem> GetInvoiceItems(GatewayViewModel v)
        {
            var lis = new List<p91CreateItem>();
            if (v.BillingScale == 1)
            {
                lis = v.lisP91_Scale1;                
                
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
    }
}
