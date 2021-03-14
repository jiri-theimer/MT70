using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip29ContactTypeBL
    {
        public BO.p29ContactType Load(int pid);
        public IEnumerable<BO.p29ContactType> GetList(BO.myQuery mq);
        public int Save(BO.p29ContactType rec);


    }
    class p29ContactTypeBL : BaseBL, Ip29ContactTypeBL
    {

        public p29ContactTypeBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,b01.b01Name,x38.x38Name," + _db.GetSQL1_Ocas("p29") + " FROM p29ContactType a");
            sb(" LEFT OUTER JOIN b01WorkflowTemplate b01 ON a.b01ID=b01.b01ID");            
            sb(" LEFT OUTER JOIN x38CodeLogic x38 ON a.x38ID=x38.x38ID");
            sb(strAppend);
            return sbret();
        }

        public BO.p29ContactType Load(int pid)
        {
            return _db.Load<BO.p29ContactType>(GetSQL1(" WHERE a.p29ID=@pid"), new { pid = pid });
        }
        public IEnumerable<BO.p29ContactType> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p29ContactType>(fq.FinalSql, fq.Parameters);
        }


        public int Save(BO.p29ContactType rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }

            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.pid);
            p.AddInt("b01ID", rec.b01ID, true);            
            p.AddInt("x38ID", rec.x38ID, true);
            p.AddString("p29Name", rec.p29Name);          
            p.AddInt("p29Ordinary", rec.p29Ordinary);


            int intPID = _db.SaveRecord("p29ContactType", p, rec);
            

            return intPID;
        }


        private bool ValidateBeforeSave(BO.p29ContactType rec)
        {

            if (string.IsNullOrEmpty(rec.p29Name))
            {
                this.AddMessage("Chybí název."); return false;
            }

           
            return true;
        }
    }
}
