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

            if (!rec.isclosed && !rec.p41IsDraft && _f.CurrentUser.j04IsMenu_Worksheet)
            {
                DIV();
                AMI_Vykazat(rec);
                
                if (_f.p41ProjectBL.TestIfApprovingPerson(rec))
                {
                    var mq = new BO.myQueryP31() { isinvoiced = false };
                    if (rec.p07Level < 5)
                    {
                        mq.leindex = rec.p07Level;
                        mq.lepid = rec.pid;
                    }
                    else
                    {
                        mq.p41id = rec.pid;
                    }
                    var lisP31 = _f.p31WorksheetBL.GetList(mq);
                    if (lisP31.Count() > 0)
                    {
                        int intWIP = lisP31.Where(p => p.p71ID == BO.p71IdENUM.Nic).Count();
                        int intAPP = lisP31.Where(p => p.p71ID != BO.p71IdENUM.Nic).Count();
                        AMI("Schválit/Vyúčtovat", $"javascript: _window_open('/p31approve/Index?prefix=p41&pids={pid}', 2)", "approval", null, null, null, "(" + intWIP.ToString() + "x + " + intAPP.ToString() + "x)");
                    }
                }
                                

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
                AMI_RecPage(_f.tra("Klient") + ": " + rec.Client, "p28", rec.p28ID_Client, "bind");                
            }
            if (rec.p41ID_P07Level1 > 0 && rec.p41ID_P07Level1 != rec.pid)
            {
                
                AMI_RecPage(_f.p41ProjectBL.Load(rec.p41ID_P07Level1).TypePlusName, "le1", rec.p41ID_P07Level1, "bind");
                
            }
            if (rec.p41ID_P07Level2 > 0 && rec.p41ID_P07Level2 != rec.pid)
            {
                AMI_RecPage(_f.p41ProjectBL.Load(rec.p41ID_P07Level2).TypePlusName, "le2", rec.p41ID_P07Level2, "bind");                               
            }
            if (rec.p41ID_P07Level3 > 0 && rec.p41ID_P07Level3 != rec.pid)
            {
                AMI_RecPage(_f.p41ProjectBL.Load(rec.p41ID_P07Level3).TypePlusName, "le3", rec.p41ID_P07Level3, "bind");                                
            }
            if (rec.p41ID_P07Level4 > 0 && rec.p41ID_P07Level4 != rec.pid)
            {
                AMI_RecPage(_f.p41ProjectBL.Load(rec.p41ID_P07Level4).TypePlusName, "le4", rec.p41ID_P07Level4, "bind");
            }

        }
    }
}
