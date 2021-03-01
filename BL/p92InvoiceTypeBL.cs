using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip92InvoiceTypeBL
    {
        public BO.p92InvoiceT Load(int pid);
        public IEnumerable<BO.p92InvoiceT> GetList(BO.myQuery mq);
        public int Save(BO.p92InvoiceT rec);

    }
    class p92InvoiceTypeBL : BaseBL, Ip92InvoiceTypeBL
    {
        public p92InvoiceTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,x15.x15Name,j27.j27Code,j61.j61Name,j17.j17Name,p93.p93Name,");
            sb(_db.GetSQL1_Ocas("p92"));
            sb(" FROM p92InvoiceType a LEFT OUTER JOIN x15VatRateType x15 ON a.x15ID=x15.x15ID LEFT OUTER JOIN j27Currency j27 ON a.j27ID=j27.j27ID");
            sb(" LEFT OUTER JOIN j17Country j17 ON a.j17ID=j17.j17ID LEFT OUTER JOIN p93InvoiceHeader p93 ON a.p93ID=p93.p93ID LEFT OUTER JOIN j61TextTemplate j61 ON a.j61ID=j61.j61ID");
            sb(strAppend);
            return sbret();
        }
        public BO.p92InvoiceT Load(int pid)
        {
            return _db.Load<BO.p92InvoiceT>(GetSQL1(" WHERE a.p92ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.p92InvoiceT> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p92InvoiceT>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.p92InvoiceT rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("p92Name", rec.p92Name);
            p.AddString("p92Code", rec.p92Code);
            p.AddEnumInt("p92InvoiceType", rec.p92InvoiceType);
            p.AddInt("p92Ordinary", rec.p92Ordinary);

            p.AddInt("j27ID", rec.j27ID, true);
            p.AddEnumInt("x15ID", rec.x15ID,true);
            p.AddInt("j17ID", rec.j17ID, true);
            p.AddInt("x38ID", rec.x38ID, true);
            p.AddInt("p98ID", rec.p98ID, true);
            p.AddInt("x38ID_Draft", rec.x38ID_Draft, true);

            p.AddInt("p93ID", rec.p93ID, true);
            p.AddInt("j19ID", rec.j19ID, true);
            p.AddInt("b01ID", rec.b01ID, true);
            p.AddInt("j61ID", rec.j61ID, true);
            p.AddInt("x31ID_Invoice", rec.x31ID_Invoice, true);
            p.AddInt("x31ID_Attachment", rec.x31ID_Attachment, true);
            p.AddInt("x31ID_Letter", rec.x31ID_Letter, true);
            p.AddInt("p80ID", rec.p80ID, true);
            
            
            p.AddString("p92ReportConstantPreText1", rec.p92ReportConstantPreText1);
            p.AddString("p92InvoiceDefaultText1", rec.p92InvoiceDefaultText1);
            p.AddString("p92InvoiceDefaultText2", rec.p92InvoiceDefaultText2);
            p.AddString("p92ReportConstantText", rec.p92ReportConstantText);
            p.AddString("p92AccountingIDS", rec.p92AccountingIDS);
            p.AddString("p92ClassificationVATIDS", rec.p92ClassificationVATIDS);

            
            int intPID = _db.SaveRecord("p92InvoiceType", p.getDynamicDapperPars(), rec);

            return intPID;


        }
        private bool ValidateBeforeSave(BO.p92InvoiceT rec)
        {
            if (string.IsNullOrEmpty(rec.p92Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.x38ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Číselná řada]."); return false;
            }
            if (rec.j27ID == 0)
            {
                this.AddMessage("Chybí vyplnit výchozí měnu faktury."); return false;
            }

            return true;
        }

    }
}
