using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public class o23ContextMenu: BaseContextMenu
    {
        public o23ContextMenu(BL.Factory f, int pid, string source) : base(f, pid)
        {
            var rec = f.o23DocBL.Load(pid);
            var disp = f.o23DocBL.InhaleRecDisposition(rec);

            HEADER(rec.o23Name);
            if (source != "recpage")
            {
                AMI_RecPage("Stránka dokumentu", "o23", pid);
            }
            if (source != "grid")
            {
                AMI_RecGrid("Přejít do GRIDu", "o23", pid);
            }

            DIV();

            if (disp.OwnerAccess)
            {
                AMI("Upravit kartu dokumentu", $"javascript:_edit('o23',{pid})", "edit_note");
                AMI_Clone("o23", pid);
            }

            DIV();
            
            AMI("Další", null, null, null, "more");
            AMI_Memo("o23", pid, "more");
            AMI_SendMail("o23", pid,"more");            
            AMI_Report("o23", pid,"more");


            AMI("Vazby", null, null, null, "bind");
            var lisX19 = _f.o23DocBL.GetList_x19(rec.pid);
            foreach(var c in lisX19)
            {
                if (c.x29ID == 141)
                {
                    var recP41 = _f.p41ProjectBL.Load(c.x19RecordPID);
                    AMI_RecPage(recP41.TypePlusName, "le" + recP41.p07Level.ToString(), c.x19RecordPID, "bind");
                }
                else
                {
                    AMI_RecPage(_f.CBL.GetObjectAlias(BO.BASX29.GetPrefix((BO.x29IdEnum)c.x29ID),c.x19RecordPID), BO.BASX29.GetPrefix((BO.x29IdEnum)c.x29ID), c.x19RecordPID, "bind");
                }
                
            }
        }
    }
}
