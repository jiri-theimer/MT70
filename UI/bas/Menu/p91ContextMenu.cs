using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public class p91ContextMenu: BaseContextMenu
    {
        public p91ContextMenu(BL.Factory f, int pid, string source) : base(f, pid)
        {
            var rec = f.p91InvoiceBL.Load(pid);
            var disp = f.p91InvoiceBL.InhaleRecDisposition(rec);

            if (!disp.ReadAccess) return;   //bez oprávnění

            if (source != "recpage")
            {
                HEADER(rec.p92Name + ": " + rec.p91Code);
                AMI_RecPage("Stránka vyúčtování", "p91", pid);
            }
            if (source != "grid")
            {
                AMI_RecGrid("Přejít do GRIDu", "p91", pid);
            }

            DIV();

            if (disp.OwnerAccess)
            {
                AMI("Upravit kartu vyúčtování", $"javascript:_edit('p91',{pid})", "k-i-edit");
                
            }
            AMI("Zapsat úhradu", $"javascript: _window_open('/p82/Record?pid=0&p90id={pid}')");

            DIV();

            AMI_Report("91", pid, null);

            AMI("Další", null, null, null, "more");
            AMI_Memo("p91", pid, "more");
            AMI_Doc("p91", pid, "more");
            AMI_SendMail("p91", pid, "more");


        }
    }
}
