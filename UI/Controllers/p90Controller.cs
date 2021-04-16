using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;

namespace UI.Controllers
{
    public class p90Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new p90RecPage() { Factory = this.Factory, prefix = "p90", pid = pid };
            v.SetGridUrl();
            RefreshStateInfo(v);
            return View(v);
        }
        public IActionResult Tab1(int pid)
        {
            var v = new p90RecPage() { Factory = this.Factory, prefix = "p90", pid = pid };
            RefreshStateInfo(v);
            return View(v);
        }
        private void RefreshStateInfo(p90RecPage v)
        {
            v.Rec = Factory.p90ProformaBL.Load(v.pid);
            if (v.Rec != null)
            {
                //v.RecSum = Factory.p28ContactBL.LoadSumRow(v.Rec.pid);

                v.SetTagging();
                v.lisP82 = Factory.p82Proforma_PaymentBL.GetList(v.pid);
                
                v.SetFreeFields(0);
            }
        }

        public IActionResult RecPage(int pid)
        {
            var v = new p90RecPage() { Factory = this.Factory, pid = pid, prefix = "p90" };

            v.NavTabs = new List<NavTab>();

            if (v.pid == 0)
            {
                v.pid = v.LoadLastUsedPid();
            }
            if (v.pid > 0)
            {
                RefreshStateInfo(v);

                if (v.Rec == null)
                {
                    this.Notify_RecNotFound();
                    v.pid = 0;
                }
                else
                {
                    v.SetGridUrl();
                    v.MenuCode = v.Rec.p90Code;
                    v.SaveLastUsedPid();

                    RefreshNavTabs(v);

                }

            }

            if (v.pid == 0)
            {
                v.Rec = new BO.p90Proforma();
            }

            return View(v);

        }

        private void RefreshNavTabs(p90RecPage v)
        {

            if (v.PanelHeight == "none")
            {
                v.NavTabs.Add(AddTab("Tab1", "tab1", "/p90/Tab1?pid=" + v.pid.ToString(), false, null));
            }

            string strBadge = null;

            v.NavTabs.Add(AddTab("Poznámky", "b07", "/b07/List?source=recpage", true, strBadge));

            string strDefTab = Factory.CBL.LoadUserParam("recpage-tab-p90");
            var deftab = v.NavTabs[0];

            foreach (var tab in v.NavTabs)
            {
                tab.Url += "&master_entity=p90Proforma&master_pid=" + v.pid.ToString();
                if (strDefTab != null && tab.Entity == strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka                    
                }
            }
            deftab.CssClass += " active";
            v.DefaultNavTabUrl = deftab.Url;
        }
    }
}
