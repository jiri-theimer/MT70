using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ix38CodeLogicBL
    {
        public BO.x38CodeLogic Load(int pid);
        public IEnumerable<BO.x38CodeLogic> GetList(BO.myQuery mq);
        public int Save(BO.x38CodeLogic rec);
        

    }
    class x38CodeLogicBL : BaseBL, Ix38CodeLogicBL
    {
        public x38CodeLogicBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("select a.*,");
            sb(_db.GetSQL1_Ocas("x38"));
            sb(" FROM x38CodeLogic a inner join x29Entity x29 on a.x29id=x29.x29id");            
            sb(strAppend);
            return sbret();
        }
        public BO.x38CodeLogic Load(int pid)
        {
            return _db.Load<BO.x38CodeLogic>(GetSQL1(" WHERE a.x38ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.x38CodeLogic> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x38CodeLogic>(fq.FinalSql, fq.Parameters);
        }
      

        public int Save(BO.x38CodeLogic rec)
        {
            
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("x38Name", rec.x38Name);
            
            p.AddString("x38MaskSyntax", rec.x38MaskSyntax);
            p.AddEnumInt("x38EditModeFlag", rec.x38EditModeFlag);
            p.AddEnumInt("x29id", rec.x29ID);
           
            p.AddString("x38ConstantBeforeValue", rec.x38ConstantBeforeValue);
            p.AddString("x38ConstantAfterValue", rec.x38ConstantAfterValue);
            p.AddString("x38MaskSyntax", rec.x38MaskSyntax);
            p.AddString("x38Description", rec.x38Description);


            p.AddInt("x38Scale", rec.x38Scale);
            p.AddInt("x38ExplicitIncrementStart", rec.x38ExplicitIncrementStart);
            p.AddBool("x38IsUseDbPID", rec.x38IsUseDbPID);
        

            int intPID = _db.SaveRecord("x38CodeLogic", p, rec);
          

            return intPID;
        }
        private bool ValidateBeforeSave(BO.x38CodeLogic rec)
        {
           
            if (string.IsNullOrEmpty(rec.x38Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.x29ID == BO.x29IdEnum._NotSpecified)
            {
                this.AddMessage("Chybí vyplnit [Entita]."); return false;
            }


            return true;
        }



    }
}
