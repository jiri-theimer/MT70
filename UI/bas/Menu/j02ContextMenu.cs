using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public class j02ContextMenu:BaseContextMenu
    {
        public j02ContextMenu(BL.Factory f,int pid) : base(f,pid)
        {

            var rec = f.j02PersonBL.Load(pid);

            AMI_RecPage("Stránka osoby", "j02", pid);        
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
            
            AMI("Nová poznámka/odkaz/příloha", $"javascript: _window_open('/b07/Record?prefix=j02&pid=0&recordpid={pid}')", "k-i-comment", "more");
            AMI("Nový dokument", $"javascript: _window_open('/o23/Record?prefix=j02&pid=0&recordpid={pid}')",null,"more");
            

            AMI("Odeslat zprávu", $"javascript: _window_open('/Mail/SendMail?j02id={0}&x29id=102&x40datapid={pid}',2)",null,"more");


            
        }
    }
}
