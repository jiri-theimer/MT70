using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public class defContextMenu: BaseContextMenu
    {
        public defContextMenu(BL.Factory f, int pid, string source,string prefix) : base(f, pid)
        {

            AMI("Upravit", $"javascript:_edit('{prefix}',{pid})", "k-i-edit");
            AMI_Clone(prefix, pid);

            DIV();
            AMI("Nový", $"javascript:_edit('{prefix}',0)", "k-i-plus-outline");

            
        }
    }
}
