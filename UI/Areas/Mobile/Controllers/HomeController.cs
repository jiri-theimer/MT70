using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Areas.Mobile.Controllers
{
    [Area("Mobile")]
   
    public class HomeController : Controller
    {
        private BL.RunningApp _app;
        public HomeController(BL.RunningApp app)
        {
            _app = app;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            
            return View();
        }

        public IActionResult xx1()
        {
            return View();
        }
    }
}
