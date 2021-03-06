using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip42ProjectTypeBL
    {
        public BO.p42ProjectType Load(int pid);
        public IEnumerable<BO.p42ProjectType> GetList(BO.myQuery mq);
        public int Save(BO.p42ProjectType rec, List<int> p34ids);


    }
    class p42ProjectTypeBL : BaseBL, Ip42ProjectTypeBL
    {

        public p42ProjectTypeBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,b01.b01Name,p07.p07NameSingular as p07Name,x38.x38Name," + _db.GetSQL1_Ocas("p42") + " FROM p42ProjectType a");
            sb(" LEFT OUTER JOIN b01WorkflowTemplate b01 ON a.b01ID=b01.b01ID");
            sb(" LEFT OUTER JOIN p07ProjectLevel p07 ON a.p07ID=p07.p07ID");
            sb(" LEFT OUTER JOIN x38CodeLogic x38 ON a.x38ID=x38.x38ID");
            sb(strAppend);
            return sbret();
        }

        public BO.p42ProjectType Load(int pid)
        {
            return _db.Load<BO.p42ProjectType>(GetSQL1(" WHERE a.p42ID=@pid"), new { pid = pid });
        }
        public IEnumerable<BO.p42ProjectType> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p42ProjectType>(fq.FinalSql, fq.Parameters);
        }


        public int Save(BO.p42ProjectType rec,List<int>p34ids)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();

                p.AddInt("pid", rec.pid);
                p.AddInt("b01ID", rec.b01ID, true);
                p.AddInt("p07ID", rec.p07ID, true);
                p.AddInt("x38ID", rec.x38ID, true);
                p.AddEnumInt("p42ArchiveFlag", rec.p42ArchiveFlag);
                p.AddEnumInt("p42ArchiveFlagP31", rec.p42ArchiveFlagP31);
                p.AddString("p42Name", rec.p42Name);
                p.AddString("p42Code", rec.p42Code);
                p.AddInt("p42Ordinary", rec.p42Ordinary);

                int intPID = _db.SaveRecord("p42ProjectType", p.getDynamicDapperPars(), rec);
                if (intPID > 0 && p34ids != null)
                {
                    if (rec.pid > 0)
                    {
                        _db.RunSql("DELETE FROM p43ProjectType_Workload WHERE p42ID=@pid", new { pid = intPID });
                    }
                    if (p34ids.Count > 0)
                    {
                        _db.RunSql("INSERT INTO p43ProjectType_Workload(p42ID,p34ID) SELECT @pid,p34ID FROM p34ActivityGroup WHERE p34ID IN (" + string.Join(",", p34ids) + ")", new { pid = intPID });
                    }
                }
                sc.Complete();
                return intPID;
            }
                
        }


        private bool ValidateBeforeSave(BO.p42ProjectType rec)
        {

            if (string.IsNullOrEmpty(rec.p42Name))
            {
                this.AddMessage("Chybí název."); return false;
            }

            if (rec.x38ID==0)
            {
                this.AddMessage("Chybí vazba na číselnou řadu projektu."); return false;
            }
            if (rec.p07ID == 0)
            {
                this.AddMessage("Chybí vazba na úroveň projektu."); return false;
            }


            return true;
        }
    }
}
