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
    public class p56Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            return Tab1(pid, "info");
        }
        public IActionResult Tab1(int pid, string caller)
        {
            var v = new p56Tab1() { Factory = this.Factory, prefix = "p56", pid = pid, caller = caller };

            RefreshStateTab1(v);
            return View(v);
        }
        private void RefreshStateTab1(p56Tab1 v)
        {
            v.Rec = Factory.p56TaskBL.Load(v.pid);
            if (v.Rec != null)
            {
                v.RecSum = Factory.p56TaskBL.LoadSumRow(v.Rec.pid);

                v.SetTagging();
                
                v.SetFreeFields(0);
            }
        }
    }
}
