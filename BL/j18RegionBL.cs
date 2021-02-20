using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ij18RegionBL
    {
        public BO.j18Region Load(int pid);
        public BO.j18Region LoadByCode(string strCode, int pid_exclude);
        public IEnumerable<BO.j18Region> GetList(BO.myQuery mq);
        public int Save(BO.j18Region rec);

    }
    class j18RegionBL : BaseBL, Ij18RegionBL
    {
        public j18RegionBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,j17.j17Name,");
            sb(_db.GetSQL1_Ocas("j18"));
            sb(" FROM j18Region a LEFT OUTER JOIN j17Country j17 ON a.j17ID=j17.j17ID");
            sb(strAppend);
            return sbret();
        }
        public BO.j18Region Load(int pid)
        {
            return _db.Load<BO.j18Region>(GetSQL1(" WHERE a.j18ID=@pid"), new { pid = pid });
        }
        public BO.j18Region LoadByCode(string strCode, int pid_exclude)
        {
            if (pid_exclude > 0)
            {
                return _db.Load<BO.j18Region>(GetSQL1(" WHERE a.j18Code LIKE @code AND a.j18ID<>@pid_exclude"), new { code = strCode, pid_exclude = pid_exclude });
            }
            else
            {
                return _db.Load<BO.j18Region>(GetSQL1(" WHERE a.j18Code LIKE @code"), new { code = strCode });
            }
        }

        public IEnumerable<BO.j18Region> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j18Region>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.j18Region rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            using (var sc = new System.Transactions.TransactionScope())     //ukládání podléhá transakci{
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddInt("j17ID", rec.j17ID, true);
                p.AddString("j18Name", rec.j18Name);
                p.AddInt("j18Ordinary", rec.j18Ordinary);
                p.AddString("j18Code", rec.j18Code);

                int intPID = _db.SaveRecord("j18Region", p.getDynamicDapperPars(), rec);
                if (intPID > 0)
                {
                    var pars = new Dapper.DynamicParameters();
                    {
                        pars.Add("j18id", intPID, System.Data.DbType.Int32);
                        pars.Add("j03id_sys", _mother.CurrentUser.pid, System.Data.DbType.Int32);                        
                    }

                    if (_db.RunSp("j18_aftersave", ref pars, false) == "1")
                    {
                        sc.Complete();
                        return intPID;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return intPID;
            }
            

        }
        private bool ValidateBeforeSave(BO.j18Region rec)
        {
            if (string.IsNullOrEmpty(rec.j18Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (LoadByCode(rec.j18Code, rec.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("Kód [{0}] již obsadilo jiné středisko."), rec.j18Code));
                return false;
            }

            return true;
        }

    }
}
