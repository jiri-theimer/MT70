using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ic21FondCalendarBL
    {
        public BO.c21FondCalendar Load(int pid);
        public IEnumerable<BO.c21FondCalendar> GetList(BO.myQuery mq);
        public int Save(BO.c21FondCalendar rec, List<BO.c28FondCalendar_Log> lisC28);
        public IEnumerable<BO.c28FondCalendar_Log> GetList_c28(int c21id);
        public double GetSumHours(int c21id, int j17id, DateTime d1, DateTime d2);
        public IEnumerable<BO.FondHours> GetSumHoursPerMonth(int c21id, int j17id, DateTime d1, DateTime d2);

    }
    class c21FondCalendarBL : BaseBL, Ic21FondCalendarBL
    {
        public c21FondCalendarBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("c21"));
            sb(" FROM c21FondCalendar a");
            sb(strAppend);
            return sbret();
        }
        public BO.c21FondCalendar Load(int pid)
        {
            return _db.Load<BO.c21FondCalendar>(GetSQL1(" WHERE a.c21ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.c21FondCalendar> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.c21FondCalendar>(fq.FinalSql, fq.Parameters);
        }


        public IEnumerable<BO.c28FondCalendar_Log> GetList_c28(int c21id)
        {
            string s = "select a.*,b.c21Name FROM c28FondCalendar_Log a INNER JOIN c21FondCalendar b ON a.c21ID_Log=b.c21ID WHERE a.c21ID=@c21id";            
            return _db.GetList<BO.c28FondCalendar_Log>(s,new { c21id = c21id});
        }


        public int Save(BO.c21FondCalendar rec,List<BO.c28FondCalendar_Log> lisC28)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("c21Name", rec.c21Name);
            p.AddInt("c21Ordinary", rec.c21Ordinary);
            p.AddEnumInt("c21ScopeFlag", rec.c21ScopeFlag);
            p.AddDouble("c21Day1_Hours", rec.c21Day1_Hours);
            p.AddDouble("c21Day2_Hours", rec.c21Day2_Hours);
            p.AddDouble("c21Day3_Hours", rec.c21Day3_Hours);
            p.AddDouble("c21Day4_Hours", rec.c21Day4_Hours);
            p.AddDouble("c21Day5_Hours", rec.c21Day5_Hours);
            p.AddDouble("c21Day6_Hours", rec.c21Day6_Hours);
            p.AddDouble("c21Day7_Hours", rec.c21Day7_Hours);


            int intPID = _db.SaveRecord("c21FondCalendar", p, rec);
            if (intPID > 0)
            {
                if (lisC28 != null)
                {
                    _db.RunSql("DELETE FROM c28FondCalendar_Log WHERE c21ID=@c21id", new { c21id = intPID });
                    foreach(var c in lisC28)
                    {
                        p = new DL.Params4Dapper();
                        p.AddInt("c21ID", intPID, true);
                        p.AddInt("c21ID_Log", c.c21ID_Log, true);
                        p.AddDateTime("c28ValidFrom", c.c28ValidFrom);
                        p.AddDateTime("c28ValidUntil", c.c28ValidUntil);
                        _db.RunSql("INSERT INTO c28FondCalendar_Log(c21ID,c21ID_Log,c28ValidFrom,c28ValidUntil) VALUES(@c21ID,@c21ID_Log,@c28ValidFrom,@c28ValidUntil)", p.getDynamicDapperPars());
                    }
                }
                _db.RunSql("exec dbo.c21_aftersave @c21id,@j03id_sys", new { c21id = intPID, j03id_sys = _mother.CurrentUser.pid });

            }
            return intPID;
        }
        private bool ValidateBeforeSave(BO.c21FondCalendar rec)
        {
            if (string.IsNullOrEmpty(rec.c21Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }


            return true;
        }


        public double GetSumHours(int c21id,int j17id,DateTime d1,DateTime d2)
        {
            var ret = _db.Load<BO.GetDouble>("SELECT sum(c22Hours_Work) as Value FROM c22FondCalendar_Date WHERE c21ID=@c21id AND isnull(j17ID,0)=@j17id AND c22Date BETWEEN @d1 AND @d2", new { c21id = c21id, j17id = j17id, d1 = d1, d2 = d2 });
            if (ret != null)
            {
                return ret.Value;
            }
            else
            {
                return 0;
            }
        }


        public IEnumerable<BO.FondHours> GetSumHoursPerMonth(int c21id, int j17id, DateTime d1, DateTime d2)
        {
            string s = "SELECT sum(c22Hours_Work) as Hodiny,year(c22Date) as Rok,month(c22Date) as Mesic FROM c22FondCalendar_Date WHERE c21ID=@c21id AND isnull(j17ID,0)=@j17id AND c22Date BETWEEN @d1 AND @d2 GROUP BY year(c22Date),month(c22Date) ORDER BY year(c22Date),month(c22Date)";
            return _db.GetList<BO.FondHours>(s, new { c21id = c21id, j17id = j17id, d1 = d1, d2 = d2 });
        }

    }
}
