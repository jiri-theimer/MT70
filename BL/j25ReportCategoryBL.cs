using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ij25ReportCategoryBL
    {
        public BO.j25ReportCategory Load(int pid);
        public IEnumerable<BO.j25ReportCategory> GetList(BO.myQuery mq);
        public int Save(BO.j25ReportCategory rec);

    }
    class j25ReportCategoryBL : BaseBL, Ij25ReportCategoryBL
    {
        public j25ReportCategoryBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j25"));
            sb(" FROM j25ReportCategory a");
            sb(strAppend);
            return sbret();
        }
        public BO.j25ReportCategory Load(int pid)
        {
            return _db.Load<BO.j25ReportCategory>(GetSQL1(" WHERE a.j25ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.j25ReportCategory> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j25ReportCategory>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.j25ReportCategory rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("j25Name", rec.j25Name);
            p.AddString("j25Code", rec.j25Code);
            p.AddInt("j25Ordinary", rec.j25Ordinary);

            return _db.SaveRecord("j25ReportCategory", p, rec);

        }
        private bool ValidateBeforeSave(BO.j25ReportCategory rec)
        {
            if (string.IsNullOrEmpty(rec.j25Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }


            return true;
        }

    }
}
