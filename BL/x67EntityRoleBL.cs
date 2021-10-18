using System;
using System.Collections.Generic;

namespace BL
{
    public interface Ix67EntityRoleBL
    {
        public BO.x67EntityRole Load(int pid);
        public IEnumerable<BO.x67EntityRole> GetList(BO.myQuery mq);
        public int Save(BO.x67EntityRole rec, List<int> x53ids, List<int> x67ids_slave);
        public IEnumerable<BO.x53Permission> GetList_BoundX53(int x67id);
        public void SaveO28(int x67id, List<BO.o28ProjectRole_Workload> lisO28);

        public IEnumerable<BO.o28ProjectRole_Workload> GetListO28(int x67id);
        public IEnumerable<BO.x69EntityRole_Assign> GetList_X69(int x29id, int recpid);        
        public IEnumerable<BO.x67EntityRole> GetList_BoundSlaves(int x67id);
        public bool IamReceiverOfList(IEnumerable<BO.x69EntityRole_Assign> lis);
        

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



        public int Save(BO.x67EntityRole rec, List<int> x53ids,List<int> x67ids_slave)
        {
            if (!ValidateBeforeSave(rec, x53ids))
            {
                return 0;
            }
            var strRoleValue = new String('0', 40);
            if (x53ids !=null && x53ids.Count > 0)
            {
                var x53values = _db.GetList<BO.GetInteger>("select x53Value as Value FROM x53Permission WHERE x53ID IN (" + string.Join(",", x53ids) + ") ORDER BY x53Ordinary");
                foreach (BO.GetInteger c in x53values)
                {
                    strRoleValue = strRoleValue.Substring(0, c.Value - 1) + "1" + strRoleValue.Substring(c.Value, strRoleValue.Length - c.Value);
                }
            }
            

            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddEnumInt("x29ID", rec.x29ID);
                p.AddString("x67Name", rec.x67Name);
                p.AddInt("x67Ordinary", rec.x67Ordinary);
                p.AddString("x67RoleValue", strRoleValue);

                int intPID = _db.SaveRecord("x67EntityRole", p, rec);
                if (intPID > 0)
                {
                    if (rec.pid > 0)
                    {
                        _db.RunSql("DELETE FROM x68EntityRole_Permission WHERE x67ID=@pid", new { pid = intPID });
                    }
                    if (x53ids !=null && x53ids.Count > 0)
                    {
                        _db.RunSql("INSERT INTO x68EntityRole_Permission(x67ID,x53ID) SELECT @pid,x53ID FROM x53Permission WHERE x53ID IN (" + string.Join(",", x53ids) + ")", new { pid = intPID });
                    }
                    

                    if (x67ids_slave != null)
                    {
                        if (rec.pid > 0)
                        {
                            _db.RunSql("DELETE FROM x66EntityRole_Delegate WHERE x67ID_Master=@pid", new { pid = intPID });
                        }
                        _db.RunSql($"INSERT INTO x66EntityRole_Delegate(x67ID_Master,x67ID_Slave) SELECT @pid,x67ID FROM x67EntityRole WHERE x67ID IN ({string.Join(",",x67ids_slave)})",new { pid = intPID });
                    }

                    sc.Complete();
                    _db.RunSql("exec dbo.x73_recovery_all");

                    return intPID;
                }
            }


            return 0;
            
        }
        public bool ValidateBeforeSave(BO.x67EntityRole rec, List<int> x53ids)
        {
            //if (x53ids==null || x53ids.Count == 0)
            //{
            //    this.AddMessage("K roli musí být přiřazeno minimálně jedno oprávnění!"); return false;
            //}
            if (string.IsNullOrEmpty(rec.x67Name))
            {
                this.AddMessage("Chybí vyplnit [Název role]."); return false;
            }

            return true;
        }

        public IEnumerable<BO.x53Permission> GetList_BoundX53(int x67id)
        {
            string s = "SELECT * FROM x53Permission a inner join x68EntityRole_Permission b On a.x53ID=b.x53ID WHERE b.x67ID=@x67id ORDER BY x53Ordinary";
            return _db.GetList<BO.x53Permission>(s, new { x67id = x67id });
        }

        public IEnumerable<BO.x67EntityRole> GetList_BoundSlaves(int x67id)
        {
            
            return _db.GetList<BO.x67EntityRole>(GetSQL1()+ " inner join x66EntityRole_Delegate b On a.x67ID=b.x67ID_Slave WHERE a.x67ParentID IS NULL AND b.x67ID_Master=@pid", new { pid = x67id });
        }

        public void SaveO28(int x67id,List<BO.o28ProjectRole_Workload> lisO28)
        {
            _db.RunSql("DELETE FROM o28ProjectRole_Workload WHERE x67ID=@x67id", new { x67id = x67id });
            foreach(var c in lisO28)
            {
                _db.RunSql("INSERT INTO o28ProjectRole_Workload(x67ID,p34ID,o28EntryFlag,o28PermFlag) VALUES(@x67id,@p34id,@entryflag,@permflag)", new { x67id = x67id, p34id = c.p34ID, entryflag = (int)c.o28EntryFlag, permflag = (int)c.o28PermFlag });
            }
        }

        public IEnumerable<BO.o28ProjectRole_Workload> GetListO28(int x67id)
        {
            return _db.GetList<BO.o28ProjectRole_Workload>("select a.*,p34.p34Name from o28ProjectRole_Workload a INNER JOIN p34ActivityGroup p34 ON a.p34ID=p34.p34ID WHERE a.x67ID=@pid" , new { pid = x67id });
        }

        public IEnumerable<BO.x69EntityRole_Assign> GetList_X69(int x29id,int recpid)
        {
            sb("SELECT a.*,j02.j02LastName+' '+j02.j02FirstName as Person,j11.j11IsAllPersons as IsAllPersons,x67.x67Name,j11.j11Name,x67.x29ID");
            sb(" FROM x69EntityRole_Assign a INNER JOIN x67EntityRole x67 ON a.x67ID=x67.x67ID");
            sb(" LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID LEFT OUTER JOIN j11Team j11 ON a.j11ID=j11.j11ID");
            sb(" WHERE a.x69RecordPID=@recpid AND x67.x29ID=@x29id");
            sb(" ORDER BY x67.x67Ordinary,a.x67ID");

            return _db.GetList<BO.x69EntityRole_Assign>(sbret(), new { recpid = recpid,x29id=x29id });
        }

        

        public bool IamReceiverOfList(IEnumerable<BO.x69EntityRole_Assign> lis)
        {
            foreach(var c in lis)
            {
                if (c.j02ID == _mother.CurrentUser.j02ID) return true;
                if (c.j11ID>0 && _mother.CurrentUser.j11IDs != null)
                {
                    if ((","+ _mother.CurrentUser.j11IDs + ",").Contains(c.j11ID.ToString()))
                    {
                        return true;
                    }
                        
                }
            }

            return false;
        }
    }
}
