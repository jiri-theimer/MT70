﻿using System;
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
                AMI("Upravit kartu dokumentu", $"javascript:_edit('o23',{pid})", "k-i-edit");
                AMI_Clone("o23", pid);
            }

            DIV();
            
            AMI("Další", null, null, null, "more");
            AMI_Memo("o23", pid, "more");
            AMI_SendMail("o23", pid,"more");            
            AMI_Report("o23", pid,"more");
        }
    }
}
