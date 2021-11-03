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
            var v = new GatewayViewModel() { tempguid = tempguid,p91Date=DateTime.Today,p91DateSupply=DateTime.Today,BillingScale = 1,IsDraft=true };


            RefreshState(v);

            if (v.lisP31.Count() > 0)
            {
                v.p91Datep31_From = v.lisP31.Min(p => p.p31Date);
                v.p91Datep31_Until = v.lisP31.Max(p => p.p31Date);
            }
            

            return View(v);
        }

        private void RefreshState(GatewayViewModel v)
        {
            var mq = new BO.myQueryP31() { tempguid = v.tempguid };
            v.lisP31 = Factory.p31WorksheetBL.GetList(mq).OrderBy(p => p.Project);
            if (v.BillingScale == 1 && v.lisP91_Scale1 == null)
            {
                v.lisP91_Scale1 = new List<p91CreateItem>();
                var cc = new p91CreateItem();
                if (v.lisP31.Where(p => p.p28ID_Client > 0).Count() > 0)
                {
                    cc.p28ID = v.lisP31.Where(p => p.p28ID_Client > 0).First().p28ID_Client;
                    var recP28 = Factory.p28ContactBL.Load(cc.p28ID);
                    cc.p28Name = recP28.p28name;
                }                
                v.lisP91_Scale1.Add(cc);
            }
            if (v.BillingScale==2 && v.lisP91_Scale2 == null)
            {
                v.lisP91_Scale2 = new List<p91CreateItem>();
                var lis = v.lisP31.GroupBy(p => p.p28ID_Client);
                foreach(var c in lis)
                {
                    var cc = new p91CreateItem();
                    if (c.First().p28ID_Client > 0)
                    {
                        cc.p28ID = c.First().p28ID_Client;
                        var recP28 = Factory.p28ContactBL.Load(c.First().p28ID_Client);
                        cc.p28Name = recP28.p28name;
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
                    var cc = new p91CreateItem() {p41ID=c.First().p41ID,p41Name=c.First().p41Name };
                    cc.p28ID = c.First().p28ID_Client;
                    if (cc.p28ID > 0)
                    {
                        var recP28 = Factory.p28ContactBL.Load(cc.p28ID);
                        cc.p28Name = recP28.p28name;
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
               
                    if (1==1)
                    {
                        v.SetJavascript_CallOnLoad(0);
                        
                    }

                    return View(v);

            }


            return View(v);
        }
    }
}
