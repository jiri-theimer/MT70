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
    public class p91Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new p91RecPage() { Factory = this.Factory, prefix = "p91", pid = pid };            
            v.SetGridUrl();
            RefreshStateInfo(v);
            return View(v);
        }
        public IActionResult Tab1(int pid)
        {
            var v = new p91RecPage() { Factory = this.Factory, prefix = "p91", pid = pid };
            v.StatByPrefix = Factory.CBL.LoadUserParam("recpage-p91-statprefix", "p41");
            RefreshStateInfo(v);
            return View(v);
        }
        private void RefreshStateInfo(p91RecPage v)
        {
            v.Rec = Factory.p91InvoiceBL.Load(v.pid);
            if (v.Rec != null)
            {
                //v.RecSum = Factory.p28ContactBL.LoadSumRow(v.Rec.pid);
                if (v.Rec.p93ID > 0)
                {
                    v.RecP93 = Factory.p93InvoiceHeaderBL.Load(v.Rec.p93ID);
                }
                v.RecP86 = Factory.p86BankAccountBL.LoadInvoiceAccount(v.pid);

                v.SetTagging();
                v.lisCenovyRozpis = Factory.p91InvoiceBL.GetList_CenovyRozpis(v.pid,true,true,Factory.CurrentUser.j03LangIndex);
                //v.lisP41 = Factory.p41ProjectBL.GetList(new BO.myQueryP41() { p91id = v.pid });
                v.lisP31 = Factory.p31WorksheetBL.GetList(new BO.myQueryP31() { p91id = v.pid });
                v.SetFreeFields(0);
            }
        }

        public IActionResult RecPage(int pid)
        {
            var v = new p91RecPage() { Factory = this.Factory, pid = pid, prefix = "p91" };
            v.StatByPrefix = Factory.CBL.LoadUserParam("recpage-p91-statprefix", "p41");

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
                    v.MenuCode = v.Rec.p91Code;
                    v.SaveLastUsedPid();

                    RefreshNavTabs(v);

                }

            }

            if (v.pid == 0)
            {
                v.Rec = new BO.p91Invoice();
            }

            return View(v);

        }

        private void RefreshNavTabs(p91RecPage v)
        {
            if (v.PanelHeight == "none")
            {
                v.NavTabs.Add(AddTab("Tab1", "tab1", "/p91/Tab1?pid=" + v.pid.ToString(), false, null));
            }

            string strBadge = null;

            v.NavTabs.Add(AddTab("Poznámky", "b07", "/b07/List?source=recpage", true, strBadge));

            string strDefTab = Factory.CBL.LoadUserParam("recpage-tab-p91");
            var deftab = v.NavTabs[0];

            foreach (var tab in v.NavTabs)
            {
                tab.Url += "&master_entity=p91Invoice&master_pid=" + v.pid.ToString();
                if (strDefTab != null && tab.Entity == strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka                    
                }
            }
            deftab.CssClass += " active";
            v.DefaultNavTabUrl = deftab.Url;
        }


        public IActionResult Record(int pid)
        {
            var v = new p91Record() { rec_pid = pid, rec_entity = "p91" };
            if (v.rec_pid == 0)
            {
                return this.StopPage(true, "Na vstupu chybí ID faktury.");
            }
            v.Rec = Factory.p91InvoiceBL.Load(v.rec_pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            

            v.SetTagging(Factory.o51TagBL.GetTagging("p91", v.rec_pid));
            
            v.ComboP28Name = v.Rec.p28Name;
            v.ComboOwner = v.Rec.Owner;
            v.ComboJ17Name = v.Rec.j17Name;
            if (v.Rec.j19ID > 0)
            {
                v.ComboJ19Name = Factory.FBL.LoadJ19(v.Rec.j19ID).j19Name;
            }
            
            RefreshStateRecord(v);
            if (!InhalePermissions(v))
            {
                return this.StopPage(true, "Nemáte oprávnění k záznamu.");
            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            
            return View(v);
        }

        private void RefreshStateRecord(p91Record v)
        {
            if (v.RecP92 == null)
            {
                v.RecP92 = Factory.p92InvoiceTypeBL.Load(v.Rec.p92ID);
            }
            if (v.ff1 == null)
            {
                v.ff1 = new FreeFieldsViewModel();
                v.ff1.InhaleFreeFieldsView(Factory, v.rec_pid, "p91");
            }
            v.ff1.RefreshInputsVisibility(Factory, v.rec_pid, "p91", v.Rec.p92ID);

        }




        private bool InhalePermissions(p91Record v)
        {
            var mydisp = Factory.p91InvoiceBL.InhaleRecDisposition(v.Rec);
            if (!mydisp.ReadAccess)
            {
                return false;
            }
            if (!v.Rec.p91IsDraft)
            {
                if (v.RecP92.x38ID > 0)
                {
                    v.CanEditRecordCode = Factory.x38CodeLogicBL.CanEditRecordCode(v.RecP92.x38ID, mydisp);
                }
                else
                {
                    v.CanEditRecordCode = mydisp.OwnerAccess;
                }
            }
            
            return true;
        }
    }
}
