﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ij05MasterSlaveBL
    {
        public BO.j05MasterSlave Load(int pid);
        public IEnumerable<BO.j05MasterSlave> GetList(BO.myQuery mq);
        public int Save(BO.j05MasterSlave rec);

    }
    class j05MasterSlaveBL : BaseBL, Ij05MasterSlaveBL
    {
        public j05MasterSlaveBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,j02master.j02LastName+' '+j02master.j02FirstName+isnull(' '+j02master.j02TitleBeforeName,'') as PersonMaster,");
            sb("j02slave.j02LastName+' '+j02slave.j02FirstName+isnull(' '+j02slave.j02TitleBeforeName,'') as PersonSlave,");
            sb("j11slave.j11Name as TeamSlave,");
            sb(_db.GetSQL1_Ocas("j05"));
            sb(" FROM j05MasterSlave a INNER JOIN j02Person j02master ON a.j02ID_Master=j02master.j02ID");
            sb(" LEFT OUTER JOIN j02Person j02slave ON a.j02ID_Slave=j02slave.j02ID LEFT OUTER JOIN j11Team j11slave ON a.j11ID_Slave=j11slave.j11ID");
            sb(strAppend);
            return sbret();
        }
        public BO.j05MasterSlave Load(int pid)
        {
            return _db.Load<BO.j05MasterSlave>(GetSQL1(" WHERE a.j05ID=@pid"), new { pid = pid });
        }
        

        public IEnumerable<BO.j05MasterSlave> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j05MasterSlave>(fq.FinalSql, fq.Parameters);
        }

        public int Save(BO.j05MasterSlave rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddInt("j02ID_Master", rec.j02ID_Master,true);
            p.AddInt("j02ID_Slave", rec.j02ID_Slave,true);
            p.AddInt("j11ID_Slave", rec.j11ID_Slave, true);
            p.AddEnumInt("j05Disposition_p31", rec.j05Disposition_p31);
            p.AddBool("j05IsCreate_p31", rec.j05IsCreate_p31);

            return _db.SaveRecord("j05MasterSlave", p, rec);

        }
        private bool ValidateBeforeSave(BO.j05MasterSlave rec)
        {
            if (rec.j02ID_Master==0)
            {
                this.AddMessage("Chybí nadřízená osoba."); return false;
            }
            if (rec.j02ID_Slave == 0 && rec.j11ID_Slave==0)
            {
                this.AddMessage("Chybí podřízená osoba nebo tým."); return false;
            }
            if (rec.j02ID_Slave > 0 && rec.j11ID_Slave > 0)
            {
                this.AddMessage("Za podřízenou stranu vyberte buď osobu nebo pouze tým."); return false;
            }
            if (rec.j02ID_Master == rec.j02ID_Slave)
            {
                this.AddMessage("Vazba není logická."); return false;
            }
            if (rec.j02ID_Slave > 0)
            {
                if (GetList(new BO.myQuery("j05")).Where(p=>p.pid != rec.pid).Where(p=>p.j02ID_Master==rec.j02ID_Master && p.j02ID_Slave==rec.j02ID_Slave && p.pid !=rec.pid).Count() > 0)
                {
                    this.AddMessage("Tento vztah podřízený/nadřízený je již uložen v jiném záznamu!");return false;
                }
            }
            if (rec.j11ID_Slave > 0)
            {
                if (GetList(new BO.myQuery("j05")).Where(p =>p.j02ID_Master==rec.j02ID_Master && p.j11ID_Slave==rec.j11ID_Slave && p.pid != rec.pid).Count() > 0)
                {
                    this.AddMessage("Tento vztah podřízený/nadřízený je již uložen v jiném záznamu!"); return false;
                }
            }
            return true;
        }

    }
}
