using System;
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
                AMI("Upravit položku vyúčtování", $"javascript: _window_open('/p91oper/p31edit?p31id={pid}')", "k-i-edit");
                AMI("Vyjmout položku z vyúčtování", $"javascript: _window_open('/p91oper/p31remove?p31id={pid}')", "k-i-cut");
                AMI("Přesunout do jiného vyúčtování", $"javascript: _window_open('/p91oper/p31move2invoice?p31id={pid}')", "k-i-insert-up");
                
            }
            else
            {
                if (disp.OwnerAccess && disp.RecordState==BO.p31RecordState.Editing)
                {
                    AMI("Upravit", $"javascript:_edit('p31',{pid})", "k-i-edit");

                }
                else
                {
                    AMI("Detail", $"javascript:_edit('p31',{pid})", "k-i-edit");
                }
            }

            if (disp.OwnerAccess || disp.CanApproveAndEdit)
            {
                AMI_Clone("p31", pid);
            }



            AMI("Další", null, null, null, "more");
                        
            AMI_SendMail("p31", pid, "more");
            AMI_ChangeLog("p31", pid, "more");

        }
    }
}
