﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Io38AddressBL
    {
        public BO.o38Address Load(int pid);
        public BO.o38Address LoadFirstP28Address(int p28id);
        public IEnumerable<BO.o38Address> GetList(BO.myQueryO38 mq);
        public int Save(BO.o38Address rec, int p28id, int o36id, string tempguid);
        public bool ValidateBeforeSave(BO.o38Address rec, int p28id, int o36id);
        public IEnumerable<BO.o38Address> GetList_TempOrClientInForm(int p28id, string tempguid);

    }
    class o38AddressBL : BaseBL, Io38AddressBL
    {
        public o38AddressBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,o37.o36ID,o37.p28ID,");
            sb(_db.GetSQL1_Ocas("o38"));
            sb(" FROM o38Address a LEFT OUTER JOIN (select o38ID,p28ID,o36ID FROM o37Contact_Address) o37 ON a.o38ID=o37.o38ID");
            sb(strAppend);
            return sbret();
        }
        public BO.o38Address Load(int pid)
        {
            return _db.Load<BO.o38Address>(GetSQL1(" WHERE a.o38ID=@pid"), new { pid = pid });
        }
        public BO.o38Address LoadFirstP28Address(int p28id)
        {
            return _db.Load<BO.o38Address>(GetSQL1(" WHERE a.o38ID IN (select o38ID FROM o37Contact_Address WHERE p28ID=@p28id AND o36ID=1)"), new { p28id = p28id });
        }
        public IEnumerable<BO.o38Address> GetList(BO.myQueryO38 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.o38Address>(fq.FinalSql, fq.Parameters);
        }

        public IEnumerable<BO.o38Address> GetList_TempOrClientInForm(int p28id,string tempguid)
        {
            
            return _db.GetList<BO.o38Address>(GetSQL1(" WHERE a.o38UserUpdate=@guid OR a.o38ID IN (select o38ID FROM o37Contact_Address WHERE p28ID=@p28id AND o36ID>1)"), new { guid = tempguid, p28id = p28id });
        }


        public int Save(BO.o38Address rec, int p28id,int o36id,string tempguid)
        {
            if (!ValidateBeforeSave(rec,p28id,o36id))
            {
                return 0;
            }
            if (o36id == 0) o36id = 2;            
            
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("o38Name", rec.o38Name);
            p.AddString("o38Street", rec.o38Street);
            p.AddString("o38City", rec.o38City);
            p.AddString("o38ZIP", rec.o38ZIP);
            p.AddString("o38Country", rec.o38Country);            
            
            p.AddString("o38AresID", rec.o38AresID);
            

            int intPID = _db.SaveRecord("o38Address", p, rec);
            if (intPID>0 && p28id > 0)
            {
                int intO37ID = _db.GetIntegerFromSql("select o37ID FROM o37Contact_Address WHERE o38ID="+intPID.ToString()+" AND p28ID="+p28id.ToString());
                if (intO37ID == 0)
                {
                    _db.RunSql("INSERT INTO o37Contact_Address(p28ID,o38ID,o36ID) VALUES(@p28id,@o38id,@o36id)", new { p28id = p28id,o38id=intPID,o36id=o36id });
                }
                else
                {
                    _db.RunSql("UPDATE o37Contact_Address set o36ID=@o36id WHERE o37ID=@pid", new { pid = intO37ID,o36id=o36id });
                }
            }
            if (!string.IsNullOrEmpty(tempguid))
            {
                _db.RunSql("UPDATE o38Address set o38UserUpdate=@guid,o38Description=@o36id WHERE o38ID=@pid", new { guid = tempguid,o36id=o36id.ToString(), pid=intPID });
            }
            else
            {
                _db.RunSql("UPDATE o38Address set o38UserUpdate=@login,o38Description=null WHERE o38ID=@pid", new { login = _mother.CurrentUser.j03Login, pid = intPID });
            }

            return intPID;
        }
        public bool ValidateBeforeSave(BO.o38Address rec,int p28id,int o36id)
        {
            if (string.IsNullOrEmpty(rec.o38City) && string.IsNullOrEmpty(rec.o38Street))
            {
                this.AddMessage("V adrese je třeba vyplnit město nebo ulici."); return false;
            }

            if (o36id==1 && p28id>0)
            {                
                var recExist = LoadFirstP28Address(p28id);
                if (recExist != null && recExist.pid != rec.pid)
                {
                    this.AddMessage("Tento klient má již uloženou fakturační adresu."); return false;
                }
            }
           
            return true;
        }

    }
}
