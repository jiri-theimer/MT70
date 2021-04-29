using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Controllers
{
    public class p31Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new p31Info() { pid = pid };
            v.Rec = Factory.p31WorksheetBL.Load(v.pid);

            return View(v);
        }
    }
}
