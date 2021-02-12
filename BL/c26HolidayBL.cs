using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ic26HolidayBL
    {
        public BO.c26Holiday Load(int pid);
        public IEnumerable<BO.c26Holiday> GetList(BO.myQuery mq);
        public int Save(BO.c26Holiday rec);

    }
    class c26HolidayBL : BaseBL, Ic26HolidayBL
    {
        public c26HolidayBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,j17.j17Name,");
            sb(_db.GetSQL1_Ocas("c26"));
            sb(" FROM c26Holiday a LEFT OUTER JOIN j17Country j17 ON a.j17ID=j17.j17ID");
            sb(strAppend);
            return sbret();
        }
        public BO.c26Holiday Load(int pid)
        {
            return _db.Load<BO.c26Holiday>(GetSQL1(" WHERE a.c26ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.c26Holiday> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.c26Holiday>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.c26Holiday rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("c26Name", rec.c26Name);
            p.AddInt("j17ID", rec.j17ID,true);
            p.AddDateTime("c26Date", rec.c26Date);
            
            int intPID = _db.SaveRecord("c26Holiday", p.getDynamicDapperPars(), rec);
            if (intPID > 0)
            {
                _db.RunSql("exec dbo.c26_aftersave @c26id,@j03id_sys", new { c26id = intPID, j03id_sys = _mother.CurrentUser.pid });

            }
            return intPID;
        }
        public bool ValidateBeforeSave(BO.c26Holiday rec)
        {
            if (string.IsNullOrEmpty(rec.c26Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            

            return true;
        }

    }
}
