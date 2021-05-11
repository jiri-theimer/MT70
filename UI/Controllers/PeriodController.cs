using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Controllers
{
    public class PeriodController : BaseController
    {
        private readonly BL.ThePeriodProvider _pp;
        public PeriodController(BL.ThePeriodProvider pp)
        {
            _pp = pp;
        }

        public IActionResult PeriodIFrame(string prefix,string masterentity)
        {
            var v = new PeriodViewModel() { prefix = prefix,masterentity=masterentity };

            v.InhaleUserPeriodSetting(_pp,Factory, v.prefix, v.masterentity);

            
            return View(v);
        }

        

        
    }
}
