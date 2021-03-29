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
        public IEnumerable<BO.o38Address> GetList(BO.myQuery mq);
        public int Save(BO.o38Address rec);

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

        public IEnumerable<BO.o38Address> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.o38Address>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.o38Address rec)
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
            

            return _db.SaveRecord("o38Address", p, rec);
        }
        private bool ValidateBeforeSave(BO.o38Address rec)
        {
            if (string.IsNullOrEmpty(rec.o38City) && string.IsNullOrEmpty(rec.o38Street))
            {
                this.AddMessage("V adrese je třeba vyplnit město nebo ulici."); return false;
            }
           
            return true;
        }

    }
}
