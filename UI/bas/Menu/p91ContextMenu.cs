using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public class p91ContextMenu: BaseContextMenu
    {
        public p91ContextMenu(BL.Factory f, int pid, string source) : base(f, pid)
        {
            var rec = f.p91InvoiceBL.Load(pid);
            var disp = f.p91InvoiceBL.InhaleRecDisposition(rec);

            if (!disp.ReadAccess) return;   //bez oprávnění

            if (source != "recpage")
            {
                HEADER(rec.p92Name + ": " + rec.p91Code);
                AMI_RecPage("Stránka vyúčtování", "p91", pid);
            }
            if (source != "grid")
            {
                AMI_RecGrid("Přejít do GRIDu", "p91", pid);
            }

            DIV();

            if (disp.OwnerAccess)
            {
                AMI("Upravit kartu vyúčtování", $"javascript:_edit('p91',{pid})", "k-i-edit");
                
            }
            if (rec.p91Amount_Billed > 0)
            {
                AMI("Zapsat/Odstranit úhradu", $"javascript: _window_open('/p91oper/p94?p91id={pid}')", "$");
            }
            else
            {
                AMI("Zapsat úhradu", $"javascript: _window_open('/p91oper/p94?p91id={pid}')", "$");
            }
            

            DIV();

            AMI_Report("91", pid, null);

            AMI("Další", null, null, null, "more");
            if (rec.p92InvoiceType == BO.p92InvoiceTypeENUM.ClientInvoice)
            {
                AMI("Vytvořit k faktuře opravný doklad", $"javascript: _window_open('/p91oper/creditnote?p91id={pid}')", "k-i-reset-color", "more");
            }
            if (!BO.BAS.bit_compare_or(rec.p91LockFlag, 2))
            {
                AMI("Převést vyúčtování na jinou měnu", $"javascript: _window_open('/p91oper/j27?p91id={pid}')", "$", "more");
                if (rec.p91ExchangeRate != 1)
                {
                    AMI("Aktualizovat měnový kurz", $"javascript: _window_open('/p91oper/exupdate?p91id={pid}')", "$", "more");
                }
                AMI("Převést vyúčtování na jinou DPH sazbu", $"javascript: _window_open('/p91oper/vat?p91id={pid}')", "%", "more");
            }
            
            DIV(null, "more");
            AMI_Memo("p91", pid, "more");
            AMI_Doc("p91", pid, "more");
            AMI_SendMail("p91", pid, "more");


        }
    }
}
