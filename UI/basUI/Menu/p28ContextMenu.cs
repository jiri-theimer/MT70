using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.basUI
{
    public class p28ContextMenu: BaseContextMenu
    {
        public p28ContextMenu(BL.Factory f, int pid) : base(f, pid)
        {
           
            AMI_RecPage("Stránka klienta", "p28", pid);
            DIV();
        }
    }
}
