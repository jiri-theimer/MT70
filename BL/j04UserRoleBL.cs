using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij04UserRoleBL
    {
        public BO.j04UserRole Load(int pid);
        public IEnumerable<BO.j04UserRole> GetList(BO.myQueryJ04 mq);
        public int Save(BO.j04UserRole rec, List<int> x53ids);
        
        public IEnumerable<BO.x53Permission> GetListX53(int j04id);
        
    }
    class j04UserRoleBL: BaseBL,Ij04UserRoleBL
    {

        public j04UserRoleBL(BL.Factory mother):base(mother)
        {
            
        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*," + _db.GetSQL1_Ocas("j04") + " FROM j04UserRole a");
            sb(strAppend);
            return sbret();
        }

        public BO.j04UserRole Load(int pid)
        {
            return _db.Load<BO.j04UserRole>(GetSQL1(" WHERE a.j04ID=@pid"), new { pid = pid });            
        }
        public IEnumerable<BO.j04UserRole> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j04UserRole>(fq.FinalSql, fq.Parameters);
        }

    
        public int Save(BO.j04UserRole rec,List<int>x53ids)
        {
            if (ValidateBeforeSave(rec, x53ids) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.pid);
            p.AddString("j04Name", rec.j04Name);
           
            
            p.AddBool("j04IsMenu_Worksheet", rec.j04IsMenu_Worksheet);
            p.AddBool("j04IsMenu_Project", rec.j04IsMenu_Project);          
            p.AddBool("j04IsMenu_Contact", rec.j04IsMenu_Contact);
            
            p.AddBool("j04IsMenu_People", rec.j04IsMenu_People);
            p.AddBool("j04IsMenu_Report", rec.j04IsMenu_Report);
            p.AddBool("j04IsMenu_Invoice", rec.j04IsMenu_Invoice);
            p.AddBool("j04IsMenu_Proforma", rec.j04IsMenu_Proforma);
            p.AddBool("j04IsMenu_Notepad", rec.j04IsMenu_Notepad);
            p.AddBool("j04IsMenu_Scheduler", rec.j04IsMenu_Scheduler);

            int intPID= _db.SaveRecord("j04UserRole", p.getDynamicDapperPars(), rec);
            
            if (j05ids != null)
            {                               
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM j06UserRole_Permission WHERE j04ID=@pid", new { pid = intPID });
                }
                _db.RunSql("INSERT INTO j06UserRole_Permission(j04id,j05id) SELECT @pid,j05ID FROM j05Permission WHERE j05ID IN (" + string.Join(",", j05ids) + ")",new { pid = intPID });

                var lisJ05 = GetListJ05(intPID);
                string strj04RoleValue = new string('0', 50);
                foreach(var c in lisJ05)
                {
                    int x = c.j05Value;
                    strj04RoleValue = strj04RoleValue.Substring(0, x - 1) + "1" + strj04RoleValue.Substring(x, strj04RoleValue.Length - x);
                }
                _db.RunSql("UPDATE j04UserRole SET j04RoleValue=@rolevalue WHERE j04ID=@pid", new { rolevalue = strj04RoleValue, pid = intPID });
            }
            if (lisJ08 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM j08UserRole_EventType WHERE j04ID=@pid", new { pid = intPID });
                }
                foreach (var c in lisJ08)
                {
                    _db.RunSql("INSERT INTO j08UserRole_EventType(j04ID,a10ID,j08IsAllowedCreate,j08IsAllowedRead,j08IsLeader,j08IsMember) VALUES(@pid,@a10id,@iscreate,@isread,@isleader,@ismember)", new { pid = intPID, a10id = c.a10ID, iscreate=c.j08IsAllowedCreate, isread=c.j08IsAllowedRead, isleader=c.j08IsLeader, ismember=c.j08IsMember });
                }                
            }
            if (rec.pid > 0 && intPID>0)    //vyčistit uživatelskou cache pro účty s vazbou na tuto roli
            {
                _db.RunSql("UPDATE j03User_CacheData set j03DateCache=convert(datetime,'01.01.2000',104) WHERE j03ID IN (select j03ID FROM j03User where j04ID=@pid)", new { pid = intPID });
            }

            return intPID;
        }

       
        private bool ValidateBeforeSave(BO.j04UserRole rec, List<int> x53ids)
        {
            if (x53ids != null && x53ids.Count==0)
            {
                this.AddMessage("K aplikační roli musí být přiřazeno minimálně jedno oprávnění!"); return false;
            }
            if (string.IsNullOrEmpty(rec.j04Name) == true)
            {
                this.AddMessage("Název aplikační role je povinné pole."); return false;
            }
          
            

            return true;
        }
    }
}
