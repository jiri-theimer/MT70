﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public class p31ContextMenu: BaseContextMenu
    {
        public p31ContextMenu(BL.Factory f, int pid, string source,string master_entity) : base(f, pid)
        {
            var rec = f.p31WorksheetBL.Load(pid);
            var disp = f.p31WorksheetBL.InhaleRecDisposition(rec);
            
            if (master_entity == "p91Invoice")
            {
                AMI("Upravit položku vyúčtování", $"javascript: _window_open('/p91oper/p31edit?p31id={pid}')", "edit_note");
                AMI("Vyjmout položku z vyúčtování", $"javascript: _window_open('/p91oper/p31remove?p31id={pid}')", "content_cut");
                AMI("Přesunout do jiného vyúčtování", $"javascript: _window_open('/p91oper/p31move2invoice?p31id={pid}')", "switch_right");
                
            }
            else
            {
                if (disp.OwnerAccess && disp.RecordState==BO.p31RecordState.Editing)
                {
                    AMI("Upravit", $"javascript:window.parent._edit('p31',{pid})", "edit_note");

                }
                else
                {
                    AMI("Detail", $"javascript:_edit('p31',{pid})", "edit_note");
                }
            }

            if (disp.OwnerAccess || disp.CanApproveAndEdit)
            {
                AMI_Clone("p31", pid);
            }

            if (rec.p91ID==0 && ( disp.CanApprove || disp.CanApproveAndEdit))
            {
                AMI(BO.BAS.IIFS(rec.p71ID == BO.p71IdENUM.Nic, "Schválit/Vyúčtovat","Pře-schválit/Vyúčtovat"), $"javascript: _window_open('/p31approve/Index?prefix=p31&pids={pid}', 2)", "approval");


            }
            

            AMI("Další", null, null, null, "more");
                        
            AMI_SendMail("p31", pid, "more");
            AMI_ChangeLog("p31", pid, "more");

            
            if (_f.CurrentUser.j04IsMenu_Project)
            {
                AMI("Vazby", null, null, null, "rel");
                var recP41 = _f.p41ProjectBL.Load(rec.p41ID);
                if (f.CurrentUser.p07LevelsCount > 1)
                {
                    AMI_RecPage(recP41.FullName, "le" + recP41.p07Level.ToString(), recP41.pid, "rel", "work_outline");
                }
                else
                {
                    AMI_RecPage(recP41.FullName, "p41", recP41.pid, "rel", "work_outline");

                }
                if (recP41.p28ID_Client > 0 && _f.CurrentUser.j04IsMenu_Contact)
                {
                    AMI_RecPage(_f.p28ContactBL.Load(recP41.p28ID_Client).p28name, "p28", recP41.p28ID_Client, "rel", "business");
                }
                if (rec.p56ID > 0 && _f.CurrentUser.j04IsMenu_Task)
                {
                    AMI_RecPage(_f.p56TaskBL.Load(rec.p56ID).FullName, "p56", rec.p56ID, "rel", "task");
                }
                if (_f.CurrentUser.j04IsMenu_People)
                {
                    AMI_RecPage(_f.j02PersonBL.Load(rec.j02ID).FullNameAsc, "j02", rec.j02ID, "rel", "face");
                }

            }
            
            

        }
    }
}
