using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;


namespace UI.Menu
{
    public class p90ContextMenu: BaseContextMenu
    {
        public p90ContextMenu(BL.Factory f, int pid, string source) : base(f, pid)
        {
            var rec = f.p90ProformaBL.Load(pid);
            var disp = f.p90ProformaBL.InhaleRecDisposition(rec);

            if (!disp.ReadAccess) return;   //bez oprávnění

            if (source != "recpage")
            {
                HEADER(rec.p89Name+": "+rec.p90Code);
                AMI_RecPage("Stránka zálohy", "p90", pid);
            }
            if (source != "grid")
            {
                AMI_RecGrid("Přejít do GRIDu", "p90", pid);
            }

            DIV();

            if (disp.OwnerAccess)
            {
                AMI("Upravit kartu zálohy", $"javascript:_edit('p90',{pid})", "edit_note");
                AMI_Clone("p90", pid);
            }
            AMI("Zapsat úhradu zálohy", $"javascript: _window_open('/p82/Record?pid=0&p90id={pid}')", "payments");

            DIV();
            
            AMI_Report("p90", pid,null);

            AMI("Další", null, null, null, "more");
            AMI_Memo("p90", pid, "more");
            AMI_Doc("p90", pid, "more");
            AMI_SendMail("p90", pid,"more");



            AMI("Vazby", null, null, null, "bind");
            if (rec.p28ID > 0)
            {
                AMI_RecPage(_f.tra("Klient") + ": " + rec.p28Name, "p28", rec.p28ID, "bind");
            }            

        }
    }
}
