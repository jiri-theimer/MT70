using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Menu
{
    public class p28ContextMenu: BaseContextMenu
    {
        public p28ContextMenu(BL.Factory f, int pid, string source) : base(f, pid)
        {
            var rec = f.p28ContactBL.Load(pid);
            var disp = f.p28ContactBL.InhaleRecDisposition(rec);
            
            if (source != "recpage")
            {
                HEADER(rec.p28name);
                AMI_RecPage("Stránka klienta", "p28", pid);
            }
            if (source != "grid")
            {                
                AMI_RecGrid("Přejít do GRIDu", "p28", pid);
            }
            
            DIV();
            if (disp.OwnerAccess)
            {
                AMI("Upravit kartu klienta", $"javascript:_edit('p28',{pid})", "k-i-edit");
                AMI_Clone("p28", pid);
            }

            DIV();
            AMI_Report("p28", pid);

            DIV();

            var lisP30 = f.p30Contact_PersonBL.GetList(new BO.myQueryP30() { p28id = pid });
            AMI(string.Format("Kontaktní osoby ({0})",lisP30.Count()), null, "☺", null, "contactpersons");
            AMI("Nová kontaktní osoba", $"javascript: _window_open('/j02/Record?pid=0&isintraperson=false&p28id={pid}')", null, "contactpersons");
            
            foreach(var c in lisP30)
            {
                AMI_RecPage(c.FullNameDesc, "j02", c.j02ID).ParentID = "contactpersons";
            }

            AMI("Další", null, null, null, "more");

            AMI("Nová poznámka ke klientovi", $"javascript: _window_open('/b07/Record?prefix=p28&pid=0&recordpid={pid}')", "k-i-comment", "more");
            AMI_Doc("p28", pid);
            AMI("Odeslat zprávu", $"javascript: _window_open('/Mail/SendMail?x29id=328&x40datapid={pid}',2)", null, "more");

            if (rec.p28RegID != null)
            {
                AMI("Rejstříky", null, null, null, "regs");
                AMI("JUSTICE", BO.basRejstriky.Justice(rec.p28RegID), null, "regs",null,"_blank");
                AMI("ARES", BO.basRejstriky.Ares(rec.p28RegID), null, "regs", null, "_blank");
                AMI("INSOLVENCE", BO.basRejstriky.Insolvence(rec.p28RegID), null, "regs", null, "_blank");                
                AMI("OBCHODNÝ REGISTER (SK)", BO.basRejstriky.ObchodnyRegister(rec.p28RegID), null, "regs", null, "_blank");
            }
            
        }
    }
}
