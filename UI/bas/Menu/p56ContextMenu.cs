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
            var recP41 = _f.p41ProjectBL.Load(rec.p41ID);

            if (disp.IsReceiver)
            {                
                AMI_Vykazat(recP41,pid);
            }
            
            AMI_Report("p56", pid);

            DIV();

            
            AMI("Další", null, null, null, "more");

            AMI_Memo("p56", pid, "more");
            AMI_Doc("p56", pid, "more");
            AMI_SendMail("p56", pid, "more");

            AMI("Exportovat do Kalendáře (iCalendar)", "/iCalendar/p56?pids=" + rec.pid.ToString(), "event", "more");


            AMI("Vazby", null, null, null, "bind");
            
            AMI_RecPage(recP41.TypePlusName, "le" + recP41.p07Level.ToString(), rec.p41ID, "bind");
            if (recP41.p28ID_Client > 0)
            {
                AMI_RecPage(_f.tra("Klient")+": "+recP41.Client, "p28", recP41.p28ID_Client, "bind");
            }
        }
    }
}
