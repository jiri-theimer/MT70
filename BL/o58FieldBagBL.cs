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
        public int SetValue_Bool(int o58id, string recprefix, int recpid, bool? val);
        public int SetValue_String(int o58id, string recprefix, int recpid, string val);
        public int SetValue_DateTime(int o58id, string recprefix, int recpid, DateTime? val);

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

        public IEnumerable<BO.o59FieldBagValue> GetValues(string recprefix,int recpid)
        {
            
            return _db.GetList<BO.o59FieldBagValue>("select a.*,"+ _db.GetSQL1_Ocas("o59",false,false,true)+",o58.o58Name,o58.o58Code FROM o59FieldBagValue a INNER JOIN o58FieldBag o58 ON a.o58ID=o58.o58ID WHERE a.o59RecPrefix=@prefix AND a.o59RecPid=@pid ORDER BY o58.o58Ordinary",new { prefix = recprefix, pid = recpid });
        }
        public int SetValue_Bool(int o58id,string recprefix, int recpid,bool? val)
        {
            if (val == null)
            {
                return 0;
            }
            var rec=new BO.o59FieldBagValue() { o58ID = o58id, o59RecPrefix=recprefix, o59RecPid=recpid, o59ValueBoolean =val };
            return SetValue(rec);           
        }
        public int SetValue_String(int o58id, string recprefix, int recpid, string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return 0;
            }
            var rec = new BO.o59FieldBagValue() { o58ID = o58id, o59RecPrefix = recprefix, o59RecPid = recpid, o59ValueString = val };
            return SetValue(rec);
        }
        public int SetValue_DateTime(int o58id, string recprefix, int recpid, DateTime? val)
        {
            if (val == null)
            {
                return 0;
            }
            var rec = new BO.o59FieldBagValue() { o58ID = o58id, o59RecPrefix = recprefix, o59RecPid = recpid, o59ValueDate = val };
            return SetValue(rec);
        }

        private int SetValue(BO.o59FieldBagValue rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("o58ID", rec.o58ID, true);
            p.AddString("o59RecPrefix", rec.o59RecPrefix);
            p.AddInt("o59RecPid", rec.o59RecPid,true);
            p.AddString("o59ValueString", rec.o59ValueString);
            if (rec.o59ValueBoolean != null)
            {
                p.AddBool("o59ValueBoolean",Convert.ToBoolean( rec.o59ValueBoolean));
            }
            if (rec.o59ValueDate != null)
            {
                p.AddDateTime("o59ValueDate", rec.o59ValueDate);
            }
            p.AddDouble("o59ValueNum", rec.o59ValueNum);

            int intPID = _db.SaveRecord("o59FieldBagValue", p.getDynamicDapperPars(), rec,false,true);

            return intPID;
        }
    }
}
