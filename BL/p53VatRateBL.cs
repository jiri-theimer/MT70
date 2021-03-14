using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip53VatRateBL
    {
        public BO.p53VatRate Load(int pid);        
        public IEnumerable<BO.p53VatRate> GetList(BO.myQuery mq);
        public int Save(BO.p53VatRate rec, DateTime d1, DateTime d2);

    }
    class p53VatRateBL : BaseBL, Ip53VatRateBL
    {
        public p53VatRateBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,x15.x15Name,j27.j27Code,j17.j17Name,");
            sb(_db.GetSQL1_Ocas("p53"));
            sb(" FROM p53VatRate a INNER JOIN x15VatRateType x15 ON a.x15ID=x15.x15ID INNER JOIN j27Currency j27 ON a.j27ID=j27.j27ID");
            sb(" LEFT OUTER JOIN j17Country j17 ON a.j17ID=j17.j17ID");
            sb(strAppend);
            return sbret();
        }
        public BO.p53VatRate Load(int pid)
        {
            return _db.Load<BO.p53VatRate>(GetSQL1(" WHERE a.p53ID=@pid"), new { pid = pid });
        }
       
        public IEnumerable<BO.p53VatRate> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p53VatRate>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.p53VatRate rec,DateTime d1,DateTime d2)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            using (var sc = new System.Transactions.TransactionScope())     //ukládání podléhá transakci{
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddInt("j27ID", rec.j27ID, true);
                p.AddEnumInt("x15ID", rec.x15ID, true);
                p.AddInt("j17ID", rec.j17ID, true);
                p.AddDouble("p53Value", rec.p53Value);
                p.AddDateTime("p53ValidFrom", d1);
                p.AddDateTime("p53ValidUntil", d2);

                int intPID = _db.SaveRecord("p53VatRate", p, rec,false);
                if (intPID > 0)
                {
                    sc.Complete();                    
                }
                return intPID;
            }


        }
        private bool ValidateBeforeSave(BO.p53VatRate rec)
        {
            if (rec.j27ID==0)
            {
                this.AddMessage("Chybí vyplnit [Měna]."); return false;
            }
            if (rec.x15ID == BO.x15IdEnum.Nic)
            {
                this.AddMessage("Chybí vyplnit [Hladina DPH]."); return false;
            }

            return true;
        }

    }
}
