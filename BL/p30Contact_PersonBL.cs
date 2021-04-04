using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip30Contact_PersonBL
    {
        public BO.p30Contact_Person Load(int pid);
        public BO.p30Contact_Person LoadByp28(int j02id, int p28id);
        public IEnumerable<BO.p30Contact_Person> GetList(BO.myQueryP30 mq);
        public int Save(BO.p30Contact_Person rec);

    }
    class p30Contact_PersonBL : BaseBL, Ip30Contact_PersonBL
    {
        public p30Contact_PersonBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,p28.p28Name,p41.p41Name,p41.p41Code,j02.j02LastName+' '+j02.j02FirstName+isnull(' '+j02.j02TitleBeforeName,'') as FullNameDesc,");
            sb(_db.GetSQL1_Ocas("p30"));
            sb(" FROM p30Contact_Person a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID");
            sb(" LEFT OUTER JOIN p28Contact p28 ON a.p28ID=p28.p28ID");
            sb(" LEFT OUTER JOIN p41Project p41 ON a.p41ID=p41.p41ID");
            sb(strAppend);
            return sbret();
        }
        public BO.p30Contact_Person Load(int pid)
        {
            return _db.Load<BO.p30Contact_Person>(GetSQL1(" WHERE a.p30ID=@pid"), new { pid = pid });
        }

        public BO.p30Contact_Person LoadByp28(int j02id,int p28id)
        {
            return _db.Load<BO.p30Contact_Person>(GetSQL1(" WHERE a.j02ID=@j02id AND a.p28ID=@p28id"), new { j02id = j02id,p28id=p28id });
        }

        public IEnumerable<BO.p30Contact_Person> GetList(BO.myQueryP30 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p30Contact_Person>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.p30Contact_Person rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddInt("j02ID", rec.j02ID, true);
            p.AddInt("p28ID", rec.p28ID, true);
            p.AddInt("p41ID", rec.p41ID, true);

            return _db.SaveRecord("p30Contact_Person", p, rec);


        }
        private bool ValidateBeforeSave(BO.p30Contact_Person rec)
        {
            if (rec.j02ID == 0 )
            {
                this.AddMessage("Chybí vyplnit vazbu na osobní profil."); return false;
            }
            if (rec.p28ID==0 && rec.p41ID==0)
            {
                this.AddMessage("Chybí vyplnit vazbu na klienta nebo projekt."); return false;
            }
           

            return true;
        }

    }
}
