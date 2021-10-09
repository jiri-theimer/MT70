using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Menu
{
    public class p56ContextMenu:BaseContextMenu
    {
        public p56ContextMenu(BL.Factory f, int pid, string source) : base(f, pid)
        {
            var rec = f.p56TaskBL.Load(pid);
            var disp = f.p56TaskBL.InhaleRecDisposition(rec);

            if (source != "recpage")
            {
                HEADER(rec.p57Name+": "+rec.p56Code);
                AMI_RecPage("Stránka úkolu", "p56", pid);
            }
            if (source != "grid")
            {
                AMI_RecGrid("Přejít do GRIDu", "p56", pid);
            }

            DIV();
            if (disp.OwnerAccess)
            {
                AMI("Upravit kartu úkolu", $"javascript:_edit('p56',{pid})", "edit_note");
                AMI_Clone("p56", pid);
            }

            DIV();
            AMI_Report("p56", pid);

            DIV();

            
            AMI("Další", null, null, null, "more");

            AMI_Memo("p56", pid, "more");
            AMI_Doc("p56", pid, "more");
            AMI_SendMail("p56", pid, "more");


            

        }
    }
}
