using System;
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
        public int Save(BO.o38Address rec,int p28id,int o36id);
        public bool ValidateBeforeSave(BO.o38Address rec);

    }
    class o38AddressBL : BaseBL, Io38AddressBL
    {
        public o38AddressBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("o38"));
            sb(" FROM o38Address a");
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



        public int Save(BO.o38Address rec, int p28id,int o36id)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("o38Name", rec.o38Name);
            p.AddString("o38Street", rec.o38Street);
            p.AddString("o38City", rec.o38City);
            p.AddString("o38ZIP", rec.o38ZIP);
            p.AddString("o38Country", rec.o38Country);
            p.AddString("o38Description", rec.o38Description);
            p.AddString("o38AresID", rec.o38AresID);
            

            int intPID = _db.SaveRecord("o38Address", p, rec);
            if (intPID>0 && p28id > 0)
            {
                int intO37ID = _db.GetIntegerFromSql("select o37ID FROM o37Contact_Address WHERE o36ID="+o36id.ToString()+" AND o38ID="+intPID.ToString()+" AND p28ID="+p28id.ToString());
                if (intO37ID == 0)
                {
                    _db.RunSql("INSERT INTO o37Contact_Address(p28ID,o38ID,o36ID) VALUES(@p28id,@o38id,@o36id)", new { p28id = p28id,o38id=intPID,o36id=o36id });
                }
                else
                {
                    _db.RunSql("UPDATE o37Contact_Address set o36ID=@o36id WHERE o37ID=@pid", new { pid = intO37ID });
                }
            }

            return intPID;
        }
        public bool ValidateBeforeSave(BO.o38Address rec)
        {
            if (string.IsNullOrEmpty(rec.o38City) && string.IsNullOrEmpty(rec.o38Street))
            {
                this.AddMessage("V adrese je třeba vyplnit město nebo ulici."); return false;
            }
           
            return true;
        }

    }
}
