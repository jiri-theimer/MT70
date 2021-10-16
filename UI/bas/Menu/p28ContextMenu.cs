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
                AMI("Upravit kartu klienta", $"javascript:_edit('p28',{pid})", "edit_note");
                AMI_Clone("p28", pid);
            }

            DIV();
            var mq = new BO.myQueryP41("le5") { MyRecordsDisponible = true, p28id = rec.pid };
            var lisP41 = _f.p41ProjectBL.GetList(mq);
            if (lisP41.Count() > 0)
            {
                if (lisP41.Count() == 1)
                {
                    AMI_Vykazat(lisP41.First());
                }
                else
                {                    
                    AMI("Vykázat úkon", $"javascript: _window_open('/p41/SelectProject?source_prefix=p28&source_pid={rec.pid}')", "more_time");
                }
            }
            

            AMI_Report("p28", pid);

            DIV();

            var lisP30 = f.p30Contact_PersonBL.GetList(new BO.myQueryP30() { p28id = pid });
            AMI(string.Format("Kontaktní osoby ({0})",lisP30.Count()), null, "face", null, "contactpersons");
            AMI("Nová kontaktní osoba", $"javascript: _window_open('/j02/Record?pid=0&isintraperson=false&p28id={pid}')", null, "contactpersons");
            
            foreach(var c in lisP30)
            {
                AMI_RecPage(c.FullNameDesc, "j02", c.j02ID).ParentID = "contactpersons";
            }

            AMI("Další", null, null, null, "more");

            AMI_Memo("p28", pid, "more");
            AMI_Doc("p28", pid,"more");
            AMI_SendMail("p28", pid,"more");
            

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
