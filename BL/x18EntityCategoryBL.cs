using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ix18EntityCategoryBL
    {
        public BO.x18EntityCategory Load(int pid);
        public IEnumerable<BO.x18EntityCategory> GetList(BO.myQuery mq);
        public int Save(BO.x18EntityCategory rec);

    }
    class x18EntityCategoryBL : BaseBL, Ix18EntityCategoryBL
    {
        public x18EntityCategoryBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("x18"));
            sb(" FROM x18EntityCategory a");
            sb(strAppend);
            return sbret();
        }
        public BO.x18EntityCategory Load(int pid)
        {
            return _db.Load<BO.x18EntityCategory>(GetSQL1(" WHERE a.x18ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.x18EntityCategory> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x18EntityCategory>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.x18EntityCategory rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("x18Name", rec.x18Name);
            p.AddString("x18NameShort", rec.x18NameShort);


            return _db.SaveRecord("x18EntityCategory", p, rec);
        }
        private bool ValidateBeforeSave(BO.x18EntityCategory rec)
        {
            if (string.IsNullOrEmpty(rec.x18Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (string.IsNullOrEmpty(rec.x18Code))
            {
                this.AddMessage("Chybí vyplnit [Kód]."); return false;
            }
            return true;
        }

    }
}
