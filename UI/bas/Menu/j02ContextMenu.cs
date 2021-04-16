using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public class j02ContextMenu:BaseContextMenu
    {
        public j02ContextMenu(BL.Factory f,int pid, string source) : base(f,pid)
        {

            var rec = f.j02PersonBL.Load(pid);
            HEADER(rec.FullNameAsc);
            if (source != "recpage")
            {
                AMI_RecPage("Stránka osoby", "j02", pid);
            }
            if (source != "grid")
            {
                AMI_RecGrid("Přejít do GRIDu", "j02", pid);
            }
            
            DIV();

            if (f.CurrentUser.IsAdmin)
            {
                if (rec.j02VirtualParentID == 0)
                {
                    AMI("Upravit kartu osoby", $"javascript:_edit('j02',{pid})", "k-i-edit");
                    AMI_Clone("j02", pid);
                }
                else
                {
                    AMI("Nastavení virtuální osoby", $"javascript: _window_open('/j02/VirtualPerson?pid={pid}')");
                }
                if (rec.j02IsIntraPerson)
                {
                    DIV();
                    if (rec.j03ID > 0)
                    {
                        AMI("Uživatelský účet", $"javascript:_edit('j03',{rec.j03ID})", "k-i-user");
                    }
                    else
                    {
                        AMI("Založit uživatelský účet", $"javascript: _window_open('/j03/Record?pid=0&j02id={pid}')");
                    }
                }
                
            }

            DIV();
            AMI_Report("j02", pid);

            AMI("Další", null, null,null, "more");
            AMI_Memo("j02", pid,"more");
            AMI_Doc("j02", pid,"more");

            AMI_SendMail("j02", pid,"more");
            

            
        }
    }
}
