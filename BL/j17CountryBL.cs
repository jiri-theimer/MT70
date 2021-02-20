using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ij17CountryBL
    {
        public BO.j17Country Load(int pid);
        public BO.j17Country LoadByCode(string strCode, int pid_exclude);
        public IEnumerable<BO.j17Country> GetList(BO.myQuery mq);
        public int Save(BO.j17Country rec);

    }
    class j17CountryBL : BaseBL, Ij17CountryBL
    {
        public j17CountryBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j17"));
            sb(" FROM j17Country a");
            sb(strAppend);
            return sbret();
        }
        public BO.j17Country Load(int pid)
        {
            return _db.Load<BO.j17Country>(GetSQL1(" WHERE a.j17ID=@pid"), new { pid = pid });
        }
        public BO.j17Country LoadByCode(string strCode, int pid_exclude)
        {
            if (pid_exclude > 0)
            {
                return _db.Load<BO.j17Country>(GetSQL1(" WHERE a.j17Code LIKE @code AND a.j17ID<>@pid_exclude"), new { code = strCode, pid_exclude = pid_exclude });
            }
            else
            {
                return _db.Load<BO.j17Country>(GetSQL1(" WHERE a.j17Code LIKE @code"), new { code = strCode });
            }
        }

        public IEnumerable<BO.j17Country> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j17Country>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.j17Country rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("j17Name", rec.j17Name);
            p.AddInt("j17Ordinary", rec.j17Ordinary);
            p.AddString("j17Code", rec.j17Code);

            return _db.SaveRecord("j17Country", p.getDynamicDapperPars(), rec);


        }
        private bool ValidateBeforeSave(BO.j17Country rec)
        {
            if (string.IsNullOrEmpty(rec.j17Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (LoadByCode(rec.j17Code, rec.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("Kód [{0}] již obsadil jiný region."), rec.j17Code));
                return false;
            }

            return true;
        }

    }
}
