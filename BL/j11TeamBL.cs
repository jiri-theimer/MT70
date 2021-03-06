﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij11TeamBL
    {
        public BO.j11Team Load(int pid);
        public BO.j11Team LoadTeamOfAllPersons();
        public IEnumerable<BO.j11Team> GetList(BO.myQueryJ11 mq);
        public int Save(BO.j11Team rec, List<int> j02ids);

    }
    class j11TeamBL : BaseBL, Ij11TeamBL
    {
        public j11TeamBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j11"));            
            sb(" FROM j11Team a");            
            sb(strAppend);
            return sbret();
        }
        public BO.j11Team Load(int pid)
        {
            return _db.Load<BO.j11Team>(GetSQL1(" WHERE a.j11ID=@pid"), new { pid = pid });
        }

        public BO.j11Team LoadTeamOfAllPersons()
        {
            return _db.Load<BO.j11Team>(GetSQL1(" WHERE a.j11IsAllPersons=1"));
        }

        public IEnumerable<BO.j11Team> GetList(BO.myQueryJ11 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j11Team>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.j11Team rec,List<int> j02ids)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);            
            p.AddString("j11Name", rec.j11Name);            
            p.AddBool("j11IsAllPersons", rec.j11IsAllPersons);

            int intPID = _db.SaveRecord("j11Team", p, rec);
            if (rec.pid > 0)
            {
                _db.RunSql("DELETE FROM j12Team_Person WHERE j11ID=@pid", new { pid = intPID });
            }
            if (j02ids.Count > 0)
            {
                _db.RunSql("INSERT INTO j12Team_Person(j11ID,j02ID) SELECT @pid,j02ID FROM j02Person WHERE j02ID IN (" + string.Join(",", j02ids) + ")", new { pid = intPID });
            }
            if (intPID > 0)    //vyčistit uživatelskou cache pro účty s vazbou na tento tým
            {
                _db.RunSql("UPDATE j03User_CacheData set j03DateCache=convert(datetime,'01.01.2000',104) WHERE j03ID IN (select a.j03ID FROM j03User a INNER JOIN j12Team_Person b ON a.j02ID=b.j02ID where b.j11ID=@pid)", new { pid = intPID });
            }

            return intPID;
        }
        public bool ValidateBeforeSave(BO.j11Team rec)
        {
            if (string.IsNullOrEmpty(rec.j11Name))
            {
                this.AddMessage("Chybí vyplnit [Název týmu]."); return false;
            }
            
            return true;
        }

    }
}
