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

            AMI_Report("p91", pid, null);
            if (rec.p92InvoiceType == BO.p92InvoiceTypeENUM.CreditNote)
            {
                if (rec.p91ID_CreditNoteBind > 0)
                {
                    var recOD = f.p91InvoiceBL.Load(rec.p91ID_CreditNoteBind);
                    var recP92 = f.p92InvoiceTypeBL.Load(recOD.p92ID);
                    AMI("Sestava opravovaného dokladu", $"javascript: _window_open('/x31/ReportContext?pid={recOD.pid}&prefix=p91&x31id={recP92.x31ID_Invoice}')", "k-i-print");
                }
            }
            else
            {
                var recOD = f.p91InvoiceBL.LoadCreditNote(rec.pid);
                if (recOD != null)
                {
                    var recP92 = f.p92InvoiceTypeBL.Load(recOD.p92ID);
                    AMI("Sestava opravného dokladu", $"javascript: _window_open('/x31/ReportContext?pid={recOD.pid}&prefix=p91&x31id={recP92.x31ID_Invoice}')", "k-i-print");
                }
            }

            AMI("Další", null, null, null, "more");
            if (rec.p92InvoiceType == BO.p92InvoiceTypeENUM.ClientInvoice)
            {
                AMI("Spárovat fakturu s úhradou zálohy", $"javascript: _window_open('/p91oper/proforma?p91id={pid}')", "k-i-reset-color", "more");
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


            AMI("Vazby", null, null, null, "bind");
            if (rec.p28ID > 0)
            {
                AMI(rec.p28Name, "/Record/RecPage?prefix=p28&pid=" + rec.p28ID.ToString(), null, "bind", null, "_top");
            }
            if (rec.p41ID_First > 0)
            {
                AMI(f.CBL.GetObjectAlias("p41", rec.p41ID_First), "/Record/RecPage?prefix=p41&pid=" + rec.p41ID_First.ToString(), null, "bind", null, "_top");
            }
           

        }
    }
}
