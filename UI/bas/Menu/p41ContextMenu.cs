using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Menu
{
    public class p41ContextMenu: BaseContextMenu
    {
        public p41ContextMenu(BL.Factory f, int pid, string source) : base(f, pid)
        {
            var rec = f.p41ProjectBL.Load(pid);
            var disp = f.p41ProjectBL.InhaleRecDisposition(rec);

            if (source != "recpage")
            {
                HEADER(rec.PrefferedName);
                if (f.CurrentUser.p07LevelsCount > 1)
                {
                    AMI_RecPage("Stránka" + ": " + rec.p07Name, "le"+rec.p07Level.ToString(), pid);
                }
                else
                {
                    AMI_RecPage("Stránka" + ": " + rec.p07Name, "p41", pid);
                }
                
            }
            if (source != "grid")
            {
                if (f.CurrentUser.p07LevelsCount > 1)
                {
                    AMI_RecGrid("Přejít do GRIDu", "le"+rec.p07Level.ToString(), pid);
                }
                else
                {
                    AMI_RecGrid("Přejít do GRIDu", "p41", pid);
                }
                
            }

            DIV();
            if (disp.OwnerAccess)
            {
                AMI("Upravit kartu projektu", $"javascript:_edit('p41',{pid})", "edit_note");
                AMI_Clone("p41", pid);
            }

            DIV();
            AMI_Report("p41", pid);

            DIV();

            
            AMI("Další", null, null, null, "more");

            AMI_Memo("p41", pid, "more");
            AMI_Doc("p41", pid, "more");
            AMI_SendMail("p41", pid, "more");


            AMI("Vazby", null, null, null, "bind");
            if (rec.p28ID_Client > 0)
            {
                AMI(rec.Client, "/Record/RecPage?prefix=p28&pid=" + rec.p28ID_Client.ToString(), null, "bind", null, "_top");
            }
            if (rec.p41ID_P07Level1 > 0 && rec.p41ID_P07Level1 != rec.pid)
            {
                AMI(f.CBL.GetObjectAlias("p41", rec.p41ID_P07Level1), "/Record/RecPage?prefix=le1&pid=" + rec.p41ID_P07Level1.ToString(), null, "bind", null, "_top");
            }
            if (rec.p41ID_P07Level2 > 0 && rec.p41ID_P07Level2 != rec.pid)
            {
                AMI(f.CBL.GetObjectAlias("p41", rec.p41ID_P07Level2), "/Record/RecPage?prefix=le2&pid=" + rec.p41ID_P07Level2.ToString(), null, "bind", null, "_top");
            }
            if (rec.p41ID_P07Level3 > 0 && rec.p41ID_P07Level3 != rec.pid)
            {
                AMI(f.CBL.GetObjectAlias("p41", rec.p41ID_P07Level3), "/Record/RecPage?prefix=le3&pid=" + rec.p41ID_P07Level3.ToString(), null, "bind", null, "_top");
            }
            if (rec.p41ID_P07Level4 > 0 && rec.p41ID_P07Level4 != rec.pid)
            {
                AMI(f.CBL.GetObjectAlias("p41", rec.p41ID_P07Level4), "/Record/RecPage?prefix=le4&pid=" + rec.p41ID_P07Level4.ToString(), null, "bind", null, "_top");
            }

        }
    }
}
