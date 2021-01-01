using System;
using System.Collections.Generic;

namespace BL
{
    public interface Ix67EntityRoleBL
    {
        public BO.x67EntityRole Load(int pid);
        public IEnumerable<BO.x67EntityRole> GetList(BO.myQuery mq);
        public int Save(BO.x67EntityRole rec, List<int> x53ids);

    }
    class x67EntityRoleBL:BaseBL, Ix67EntityRoleBL
    {
        public x67EntityRoleBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("x67"));
            sb(" FROM x67EntityRole a");
            sb(strAppend);
            return sbret();
        }
        public BO.x67EntityRole Load(int pid)
        {
            return _db.Load<BO.x67EntityRole>(GetSQL1(" WHERE a.x67ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.x67EntityRole> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x67EntityRole>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.x67EntityRole rec, List<int> x53ids)
        {
            if (!ValidateBeforeSave(rec, x53ids))
            {
                return 0;
            }
            var x53values = _db.GetList<BO.GetInteger>("select x53Value as Value FROM x53Permission WHERE x53ID IN (" + string.Join(",", x53ids)+ ") ORDER BY x53Ordinary");
            var strRoleValue = new String('0', 40);
            foreach (BO.GetInteger c in x53values)
            {
                strRoleValue = strRoleValue.Substring(0, c.Value - 1) + "1" + strRoleValue.Substring(c.Value, strRoleValue.Length - c.Value);
            }
            
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddEnumInt("x29ID", rec.x29ID);
            p.AddString("x67Name", rec.x67Name);
            p.AddInt("x67Ordinary", rec.x67Ordinary);
            p.AddString("x67RoleValue", strRoleValue);

            int intPID = _db.SaveRecord("x67EntityRole", p.getDynamicDapperPars(), rec);
            if (rec.pid > 0)
            {
                _db.RunSql("DELETE FROM x68EntityRole_Permission WHERE x67ID=@pid", new { pid = intPID });
            }
            _db.RunSql("INSERT INTO x68EntityRole_Permission(x67ID,x53ID) SELECT @pid,x53ID FROM x53Permission WHERE x53ID IN (" + string.Join(",", x53ids) + ")", new { pid = intPID });

            

            return intPID;
        }
        public bool ValidateBeforeSave(BO.x67EntityRole rec, List<int> x53ids)
        {
            if (x53ids==null || x53ids.Count == 0)
            {
                this.AddMessage("K roli musí být přiřazeno minimálně jedno oprávnění!"); return false;
            }
            if (string.IsNullOrEmpty(rec.x67Name))
            {
                this.AddMessage("Chybí vyplnit [Název role]."); return false;
            }

            return true;
        }
    }
}
