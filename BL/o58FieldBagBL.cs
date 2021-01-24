using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Io58FieldBagBL
    {
        public BO.o58FieldBag Load(int pid);
        public BO.o58FieldBag LoadByCode(string strCode, int pid_exclude);
        public IEnumerable<BO.o58FieldBag> GetList(BO.myQuery mq);
        public int Save(BO.o58FieldBag rec);


    }
    class o58FieldBagBL : BaseBL, Io58FieldBagBL
    {
        public o58FieldBagBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,parent.o58Name as ParentName,parent.o58Code as ParentCode,");            
            sb(_db.GetSQL1_Ocas("o58"));
            sb(" FROM o58FieldBag a LEFT OUTER JOIN o58FieldBag parent ON a.o58ParentID=parent.o58ID");
            sb(strAppend);


            return sbret();
        }
        public BO.o58FieldBag Load(int pid)
        {
            return _db.Load<BO.o58FieldBag>(GetSQL1(" WHERE a.o58ID=@pid"), new { pid = pid });
        }
        public BO.o58FieldBag LoadByCode(string strCode, int pid_exclude)
        {
            if (pid_exclude > 0)
            {
                return _db.Load<BO.o58FieldBag>(GetSQL1(" WHERE a.o58Code LIKE @code AND a.o58ID<>@pid_exclude"), new { code = strCode, pid_exclude = pid_exclude });
            }
            else
            {
                return _db.Load<BO.o58FieldBag>(GetSQL1(" WHERE a.o58Code LIKE @code"), new { code = strCode });
            }
        }

        public IEnumerable<BO.o58FieldBag> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.o58FieldBag>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.o58FieldBag rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddInt("o58ParentID", rec.o58ParentID, true);
            p.AddEnumInt("x24ID", rec.x24ID);
            p.AddString("o58Name", rec.o58Name);
            p.AddString("o58Code", rec.o58Code);
            p.AddString("o58Description", rec.o58Description);
            p.AddInt("o58Ordinary", rec.o58Ordinary);
            
            int intPID = _db.SaveRecord("o58FieldBag", p.getDynamicDapperPars(), rec);

            return intPID;
        }

        public bool ValidateBeforeSave(BO.o58FieldBag rec)
        {

            if (string.IsNullOrEmpty(rec.o58Name) || string.IsNullOrEmpty(rec.o58Code))
            {
                this.AddMessage("[Název] a [Kód] jsou povinná pole."); return false;
            }

            if (LoadByCode(rec.o58Code, rec.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("Kód [{0}] již je obsazen jiným polem."), rec.o58Code));
                return false;
            }

            return true;
        }

    }
}
