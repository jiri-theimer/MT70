using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip36LockPeriodBL
    {
        public BO.p36LockPeriod Load(int pid);
        
        public IEnumerable<BO.p36LockPeriod> GetList(BO.myQuery mq);
        public int Save(BO.p36LockPeriod rec, List<int> p34ids);

    }
    class p36LockPeriodBL : BaseBL, Ip36LockPeriodBL
    {
        public p36LockPeriodBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("p36"));
            sb(" FROM p36LockPeriod a");
            sb(strAppend);
            return sbret();
        }
        public BO.p36LockPeriod Load(int pid)
        {
            return _db.Load<BO.p36LockPeriod>(GetSQL1(" WHERE a.p36ID=@pid"), new { pid = pid });
        }
        
        public IEnumerable<BO.p36LockPeriod> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p36LockPeriod>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.p36LockPeriod rec, List<int> p34ids)
        {
            if (!ValidateBeforeSave(rec,p34ids))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
           
            p.AddInt("j02ID", rec.j02ID,true);
            p.AddInt("j11ID", rec.j11ID, true);
            p.AddBool("p36IsAllSheets", rec.p36IsAllSheets);
            p.AddBool("p36IsAllSheets", rec.p36IsAllSheets);
            p.AddDateTime("p36DateFrom", rec.p36DateFrom);
            p.AddDateTime("p36DateUntil", rec.p36DateUntil);

            return _db.SaveRecord("p36LockPeriod", p.getDynamicDapperPars(), rec);


        }
        private bool ValidateBeforeSave(BO.p36LockPeriod rec,List<int> p34ids)
        {
            if (rec.p36DateFrom==null)
            {
                this.AddMessage("Chybí vyplnit [Datum od]."); return false;
            }
            if (rec.p36DateUntil == null)
            {
                this.AddMessage("Chybí vyplnit [Datum do]."); return false;
            }
            if (rec.p36DateUntil <rec.p36DateFrom)
            {
                this.AddMessage("Rozsah období není korektní."); return false;
            }
            if (!rec.p36IsAllPersons)
            {
                if (rec.j02ID==0 && rec.j11ID == 0)
                {
                    this.AddMessage("Musíte specifikovat osobu nebo tým osob."); return false;
                }
            }
            if (!rec.p36IsAllSheets)
            {
                if (p34ids ==null || p34ids.Count==0)
                {
                    this.AddMessage("Musíte vybrat minimálně jeden worksheet sešit."); return false;
                }
            }

            return true;
        }

    }
}
