using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Controllers
{
    public class p91operController : BaseController
    {
        public IActionResult p94(int p91id)
        {
            var v = new p94ViewModel() { p91ID = p91id };
            return View(v);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult p94(p94ViewModel v)
        {

            if (ModelState.IsValid)
            {
                var c = new BO.p94Invoice_Payment() { p91ID = v.p91ID };
                c.pid = Factory.p91InvoiceBL.SaveP94(c);
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }

            this.Notify_RecNotSaved();
            return View(v);
        }

    }
}
