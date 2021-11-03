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
            var v = new GatewayViewModel() { tempguid = tempguid,p91Date=DateTime.Today,p91DateSupply=DateTime.Today,lisP91=new List<BO.p91Create>(), BillingScale = 1,IsDraft=true };


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
            v.lisP31 = Factory.p31WorksheetBL.GetList(mq);
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
