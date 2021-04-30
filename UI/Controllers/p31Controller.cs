using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Info;

namespace UI.Controllers
{
    public class p31Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new p31Info() { pid = pid };
            v.Rec = Factory.p31WorksheetBL.Load(v.pid);
            if (v.Rec.p91ID > 0)
            {
                v.RecP91 = Factory.p91InvoiceBL.Load(v.Rec.p91ID);
            }
            return View(v);
        }
    }
}
