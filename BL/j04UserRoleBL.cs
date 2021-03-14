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
        public IEnumerable<BO.x53Permission> GetList_BoundX53(int j04id);


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
        public IEnumerable<BO.j04UserRole> GetList(BO.myQueryJ04 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j04UserRole>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.x53Permission> GetList_BoundX53(int j04id)
        {
            string s = "SELECT a.* FROM x53Permission a inner join x68EntityRole_Permission b On a.x53ID=b.x53ID INNER JOIN x67EntityRole c ON b.x67ID=c.x67ID INNER JOIN j04UserRole d ON c.x67ID=d.x67ID WHERE d.j04ID=@j04id ORDER BY x53Ordinary";
            return _db.GetList<BO.x53Permission>(s, new { j04id = j04id });
        }

        public int Save(BO.j04UserRole rec,List<int>x53ids)
        {
            if (ValidateBeforeSave(rec, x53ids) == false)
            {
                return 0;
            }
            var recX67 = new BO.x67EntityRole() { x29ID = BO.x29IdEnum.j03User, x67Name = rec.j04Name };
            if (rec.pid > 0)
            {
                recX67 = _mother.x67EntityRoleBL.Load(rec.x67ID);
            }
            rec.x67ID = _mother.x67EntityRoleBL.Save(recX67, x53ids);

            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.pid);
            p.AddInt("x67ID", rec.x67ID, true);
            p.AddString("j04Name", rec.j04Name);
           
            
            p.AddBool("j04IsMenu_Worksheet", rec.j04IsMenu_Worksheet);
            p.AddBool("j04IsMenu_Project", rec.j04IsMenu_Project);          
            p.AddBool("j04IsMenu_Contact", rec.j04IsMenu_Contact);            
            p.AddBool("j04IsMenu_People", rec.j04IsMenu_People);
            p.AddBool("j04IsMenu_Report", rec.j04IsMenu_Report);
            p.AddBool("j04IsMenu_Invoice", rec.j04IsMenu_Invoice);
            p.AddBool("j04IsMenu_Proforma", rec.j04IsMenu_Proforma);
            p.AddBool("j04IsMenu_Notepad", rec.j04IsMenu_Notepad);
            p.AddBool("j04IsMenu_Task", rec.j04IsMenu_Task);
            p.AddBool("j04IsMenu_MyProfile", rec.j04IsMenu_MyProfile);

            int intPID= _db.SaveRecord("j04UserRole", p, rec);
                                   

            if (intPID > 0)    //vyčistit uživatelskou cache pro účty s vazbou na tuto roli
            {
                _db.RunSql("UPDATE j03User set j03Cache_TimeStamp=null WHERE j04ID IN (select a.j04ID FROM j04UserRole a INNER JOIN x67EntityRole b ON a.x67ID=b.x67ID WHERE b.x67ID=@pid)", new { pid = intPID });
            }

            return intPID;
        }

       
        private bool ValidateBeforeSave(BO.j04UserRole rec, List<int> x53ids)
        {
            if (x53ids != null && x53ids.Count==0)
            {
                this.AddMessage("K roli musí být přiřazeno minimálně jedno oprávnění!"); return false;
            }
            if (string.IsNullOrEmpty(rec.j04Name) == true)
            {
                this.AddMessage("Název aplikační role je povinné pole."); return false;
            }
          
            

            return true;
        }
    }
}
