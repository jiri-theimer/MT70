using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip91InvoiceBL
    {
        public BO.p91Invoice Load(int pid);
        public IEnumerable<BO.p91Invoice> GetList(int p90id);
        public int Save(BO.p91Invoice rec);

    }
    class p91InvoiceBL : BaseBL, Ip91InvoiceBL
    {
        public p91InvoiceBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("p91"));
            sb(" FROM p91Invoice a");
            sb(strAppend);
            return sbret();
        }
        public BO.p91Invoice Load(int pid)
        {
            return _db.Load<BO.p91Invoice>(GetSQL1(" WHERE a.p91ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.p91Invoice> GetList(BO.myQueryP28 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p91Invoice>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.p91Invoice rec)
        {
            var recP90 = _mother.p90ProformaBL.Load(rec.p90ID);

            if (!ValidateBeforeSave(rec, recP90))
            {
                return 0;
            }

            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddInt("p90ID", rec.p90ID, true);
                p.AddString("p91Code", rec.p91Code);
                p.AddDateTime("p91Date", rec.p91Date);
                p.AddDateTime("p91DateIssue", rec.p91DateIssue);
                p.AddDouble("p91Amount", rec.p91Amount);
                if (rec.p91Amount != (rec.p91Amount_WithoutVat + rec.p91Amount_Vat))
                {
                    if (recP90.p90VatRate > 0)
                    {
                        rec.p91Amount_WithoutVat = Math.Round(rec.p91Amount / (1 + recP90.p90VatRate / 100), 1);
                        rec.p91Amount_Vat = rec.p91Amount - rec.p91Amount_WithoutVat;
                    }
                    else
                    {
                        rec.p91Amount_WithoutVat = rec.p91Amount;
                        rec.p91Amount_Vat = 0;
                    }
                }
                p.AddDouble("p91Amount_WithoutVat", rec.p91Amount_WithoutVat);
                p.AddDouble("p91Amount_Vat", rec.p91Amount_Vat);
                p.AddString("p91Text", rec.p91Text);

                int intPID = _db.SaveRecord("p91Invoice", p, rec, false, true);
                if (intPID > 0)
                {
                    if (_db.RunSql("exec dbo.p90_aftersave @p90id,@j03id_sys", new { p90id = rec.p90ID, j03id_sys = _mother.CurrentUser.pid }))
                    {
                        sc.Complete();
                    }
                }

                return intPID;
            }


        }
        private bool ValidateBeforeSave(BO.p91Invoice rec, BO.p90Proforma recP90)
        {
            if (rec.p90ID == 0)
            {
                this.AddMessage("Chybí vazba na zálohu."); return false;
            }
            if (rec.p91Amount <= 0)
            {
                this.AddMessage("Částka úhrady musí být větší než nula."); return false;
            }
            if (rec.p91Date == null)
            {
                this.AddMessage("Chybí datum úhrady."); return false;
            }

            var lisp91 = GetList(rec.p90ID).Where(p => p.pid != rec.pid);
            if ((lisp91.Sum(p => p.p91Amount) + rec.p91Amount) > recP90.p90Amount)
            {
                this.AddMessage("Celková částka úhrad zálohy je vyšší než částka zálohy."); return false;
            }


            return true;
        }

    }
}
