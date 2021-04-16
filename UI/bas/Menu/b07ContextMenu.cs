using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public class b07ContextMenu: BaseContextMenu
    {
        public b07ContextMenu(BL.Factory f, int pid, string source) : base(f, pid)
        {
            var rec = f.b07CommentBL.Load(pid);
           
            
            
            if (rec.j02ID_Owner==f.CurrentUser.j02ID || f.CurrentUser.IsAdmin)
            {
                AMI("Upravit/Odstranit poznámku", $"javascript:_edit('b07',{pid})", "k-i-edit");
                AMI_Clone("b07", pid);
            }

            DIV();


            AMI_SendMail("b07", pid, null);
            
            

            if (rec.b07RecordPID>0 && rec.x29ID != BO.x29IdEnum._NotSpecified)
            {
                DIV();
                string prefix = BO.BASX29.GetPrefix(rec.x29ID);
                string strAlias = f.CBL.GetObjectAlias(prefix, rec.b07RecordPID);

                //AMI("Vazby", null, null, null, "binding");
                AMI(strAlias, "/"+prefix+"/RecPage?pid="+rec.b07RecordPID.ToString(),null,null,null,"_top");

            }
            

            

        }
    }
}
