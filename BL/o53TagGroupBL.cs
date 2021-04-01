﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public interface Io53TagGroupBL
    {        
        public BO.o53TagGroup Load(int pid);
        public IEnumerable<BO.o53TagGroup> GetList(BO.myQuery mq);
        public int Save(BO.o53TagGroup rec);
    }
    class o53TagGroupBL :BaseBL, Io53TagGroupBL
    {
     
        public o53TagGroupBL(BL.Factory mother):base(mother)
        {
            
        }
      
       
        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("o53"));
            sb(" FROM o53TagGroup a");            
            sb(strAppend);
            return sbret();

        }
        public BO.o53TagGroup Load(int pid)
        {
            return _db.Load<BO.o53TagGroup>(GetSQL1(" WHERE a.o53ID=@pid"), new { pid = pid });
        }
        public IEnumerable<BO.o53TagGroup> GetList(BO.myQuery mq)
        {
            mq.explicit_orderby = "a.o53Ordinary,a.o53Name";
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser) ;
            return _db.GetList<BO.o53TagGroup>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.o53TagGroup rec)
        {
            if (String.IsNullOrEmpty(rec.x29IDs))
            {
                _mother.CurrentUser.AddMessage("Chybí vazba na entity.");
                return 0;
            }
            var p = new DL.Params4Dapper();

            if (rec.o53Field == null)//najít volné pole pro grid
            {
                var lis = GetList(new BO.myQuery("o53TagGroup")).Where(p => p.o53Field != null).OrderByDescending(p=>p.o53Field);
                int intFieldIndex = 1;
                if (lis.Count() > 0)
                {
                    intFieldIndex = 1+Convert.ToInt32(BO.BAS.RightString(lis.First().o53Field, 2));
                }
                rec.o53Field = "o54Group" + BO.BAS.RightString("0" + intFieldIndex.ToString(), 2);

            }
         
            p.AddInt("pid", rec.pid);
            p.AddString("o53Name", rec.o53Name);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddString("x29IDs", rec.x29IDs);
            p.AddInt("o53Ordinary", rec.o53Ordinary);
            p.AddBool("o53IsMultiSelect", rec.o53IsMultiSelect);
            p.AddString("o53Field", rec.o53Field);

            

            return _db.SaveRecord("o53TagGroup", p, rec);

            
        }
    }
}
