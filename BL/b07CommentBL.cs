using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ib07CommentBL
    {
        public BO.b07Comment Load(int pid);
        public IEnumerable<BO.b07Comment> GetList(BO.myQuery mq);
        public int Save(BO.b07Comment rec);

    }
    class b07CommentBL : BaseBL, Ib07CommentBL
    {
        public b07CommentBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("b07"));
            sb(",j02owner.j02FirstName+' '+j02owner.j02LastName as Author");
            sb(" FROM b07Comment a INNER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID");
            sb(strAppend);
            return sbret();
        }
        public BO.b07Comment Load(int pid)
        {
            return _db.Load<BO.b07Comment>(GetSQL1(" WHERE a.b07ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.b07Comment> GetList(BO.myQueryB07 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.b07Comment>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.b07Comment rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddEnumInt("x29ID", rec.x29ID,true);
            p.AddInt("b07RecordPID", rec.b07RecordPID);
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("b07ID_Parent", rec.b07ID_Parent, true);
            p.AddDateTime("b07Date", rec.b07Date);
            p.AddString("b07Value", rec.b07Value);

            return _db.SaveRecord("b07Comment", p, rec);
        }
        private bool ValidateBeforeSave(BO.b07Comment rec)
        {
            if (string.IsNullOrEmpty(rec.b07Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (string.IsNullOrEmpty(rec.b07Code))
            {
                this.AddMessage("Chybí vyplnit [Kód]."); return false;
            }
            return true;
        }

    }
}
