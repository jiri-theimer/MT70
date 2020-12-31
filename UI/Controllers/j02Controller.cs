using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;

namespace UI.Controllers
{
    public class j02Controller : BaseController
    {
        
       
        public IActionResult Info(int pid)
        {
            var v = new j02RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.j02PersonBL.Load(v.pid);
                if (v.Rec != null)
                {
                   
                    
                    if (v.Rec.j03ID > 0)
                    {
                        v.RecJ03 = Factory.j03UserBL.Load(v.Rec.j03ID);
                    }

                    
                }
            }
            return View(v);
        }
        public IActionResult RecPage(int pid, string login)
        {
            var v = new j02RecPage();
            v.NavTabs = new List<NavTab>();
            if (pid == 0 && String.IsNullOrEmpty(login) == false)
            {
                var recJ03 = Factory.j03UserBL.LoadByLogin(login,0);
                if (recJ03 != null && recJ03.j02ID > 0)
                {
                    pid = recJ03.j02ID;
                }
            }
            v.pid = pid;
            if (v.pid == 0)
            {
                v.pid = Factory.CBL.LoadUserParamInt("j02-RecPage-pid");
            }
            if (v.pid > 0)
            {
                v.Rec = Factory.j02PersonBL.Load(v.pid);
                if (v.Rec == null)
                {
                    this.Notify_RecNotFound();
                    v.pid = 0;
                }
                else
                {
                    v.MenuCode = v.Rec.FullNameAsc;
                   
                    if (pid > 0)
                    {
                        Factory.CBL.SetUserParam("j02-RecPage-pid", pid.ToString());
                    }
                 
                    if (v.Rec.j03ID > 0)
                    {
                        v.RecJ03 = Factory.j03UserBL.Load(v.Rec.j03ID);
                        
                    }
                   

                    RefreshNavTabs(v);
                    
                }

            }

            if (v.pid == 0)
            {
                v.Rec = new BO.j02Person();
            }

            return View(v);

        }

        private void RefreshNavTabs(j02RecPage v)
        {
            var c = Factory.j02PersonBL.LoadSumRow(v.pid);

            v.NavTabs.Add(AddTab(v.Rec.FullNameAsc, "info", "/j02/Info?pid="+v.pid.ToString(), false, null));
            string strBadge = null;
            if (c.p31_Wip_Time_Count > 0 || c.p31_Wip_Expense_Count>0 || c.p31_Wip_Fee_Count>0)
            {
                strBadge = c.p31_Wip_Time_Count.ToString() + c.p31_Wip_Expense_Count.ToString() + "+" + c.p31_Wip_Fee_Count.ToString();
            }

            v.NavTabs.Add(AddTab("Úkony", "p31", "/TheGrid/SlaveView?prefix=p31", false,strBadge));
            
            
            v.NavTabs.Add(AddTab("PING Log", "j92PingLog", "/TheGrid/SlaveView?prefix=j92",true,strBadge));
           

            string strDefTab = Factory.CBL.LoadUserParam("recpage-tab-j02");
            var deftab = v.NavTabs[0];

            foreach (var tab in v.NavTabs)
            {
                tab.Url += "&master_entity=j02Person&master_pid=" + v.pid.ToString();
                if (strDefTab != null && tab.Entity == strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka                    
                }
            }
            deftab.CssClass += " active";
            v.DefaultNavTabUrl = deftab.Url;
        }


        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j02Record() { rec_pid = pid, rec_entity = "j02" };
            v.Rec = new BO.j02Person();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j02PersonBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboC21Name = v.Rec.c21Name;
                v.ComboJ07Name = v.Rec.j07Name;
                v.ComboJ18Name = v.Rec.j18Name;
                if (v.Rec.j02TimesheetEntryDaysBackLimit_p34IDs != null)
                {
                    
                }
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.j02Record v)
        {

            if (ModelState.IsValid)
            {
                BO.j02Person c = new BO.j02Person();
                if (v.rec_pid > 0) c = Factory.j02PersonBL.Load(v.rec_pid);
                c.j07ID = v.Rec.j07ID;
                c.c21ID = v.Rec.c21ID;
                c.j18ID = v.Rec.j18ID;
                c.j02TitleBeforeName = v.Rec.j02TitleBeforeName;
                c.j02FirstName = v.Rec.j02FirstName;
                c.j02LastName = v.Rec.j02LastName;
                c.j02TitleAfterName = v.Rec.j02TitleAfterName;

                c.j02Code = v.Rec.j02Code;
                c.j02Email = v.Rec.j02Email;
                c.j02Phone = v.Rec.j02Phone;
                c.j02Mobile = v.Rec.j02Mobile;
                c.j02JobTitle = v.Rec.j02JobTitle;
                c.j02Office = v.Rec.j02Office;
                c.j02EmailSignature = v.Rec.j02EmailSignature;

                c.j02TimesheetEntryDaysBackLimit_p34IDs = v.Rec.j02TimesheetEntryDaysBackLimit_p34IDs;
                c.j02TimesheetEntryDaysBackLimit = v.Rec.j02TimesheetEntryDaysBackLimit;
                c.j02WorksheetAccessFlag = v.Rec.j02WorksheetAccessFlag;
                c.p72ID_NonBillable = v.Rec.p72ID_NonBillable;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.j02PersonBL.Save(c);
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