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
        public BO.b07Comment LoadByRecord(int x29id, int recpid);
        public int Save(BO.b07Comment rec, string uploadguid, List<int> o27ids_delete);
        public IEnumerable<BO.b07Comment> GetList(BO.myQueryB07 mq);

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
        public BO.b07Comment LoadByRecord(int x29id,int recpid)
        {
            return _db.Load<BO.b07Comment>(GetSQL1(" WHERE a.x29ID=@x29id AND a.b07RecordPID=@recpid"), new { x29id = x29id,recpid=recpid });
        }

        public IEnumerable<BO.b07Comment> GetList(BO.myQueryB07 mq)
        {
            if (mq.explicit_orderby == null) mq.explicit_orderby = "a.b07ID DESC";
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.b07Comment>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.b07Comment rec, string uploadguid,List<int> o27ids_delete)
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
            p.AddString("b07WorkflowInfo", rec.b07WorkflowInfo);
            p.AddString("b07LinkUrl", rec.b07LinkUrl);
            p.AddString("b07LinkName", rec.b07LinkName);
            p.AddString("b07ExternalPID", rec.b07ExternalPID);
            p.AddDateTime("b07ReminderDate", rec.b07ReminderDate);


            int intPID = _db.SaveRecord("b07Comment", p, rec);
            if (intPID>0 && !string.IsNullOrEmpty(uploadguid))
            {
                _mother.o27AttachmentBL.SaveChangesAndUpload(uploadguid, 607, intPID);
                if (o27ids_delete != null)
                {
                    foreach(int intO27ID in o27ids_delete)
                    {                        
                        _mother.o27AttachmentBL.Move2Deleted(_mother.o27AttachmentBL.Load(intO27ID));
                        _mother.CBL.DeleteRecord("o27Attachment", intO27ID);
                        
                    }
                }
                
            }
            return intPID;
        }
        private bool ValidateBeforeSave(BO.b07Comment rec)
        {
            if (rec.x29ID == BO.x29IdEnum._NotSpecified)
            {
                this.AddMessage("Chybí vyplnit [Entita]."); return false;
            }
            if (rec.b07RecordPID==0)
            {
                this.AddMessage("Chybí vyplnit [Záznam entity]."); return false;
            }
            if (!string.IsNullOrEmpty(rec.b07LinkUrl))
            {
                rec.b07LinkUrl = rec.b07LinkUrl.Replace("http://http://", "http://");
                rec.b07LinkUrl = rec.b07LinkUrl.Replace("http://https://", "https://");
                if (!rec.b07LinkUrl.Contains("://"))
                {
                    rec.b07LinkUrl = "http://" + rec.b07LinkUrl;
                }
            }
            if (string.IsNullOrEmpty(rec.b07LinkUrl) && !string.IsNullOrEmpty(rec.b07LinkName))
            {
                this.AddMessage("Chybí [Adresa odkazu]."); return false;
            }
            if (!string.IsNullOrEmpty(rec.b07LinkUrl) && string.IsNullOrEmpty(rec.b07LinkName))
            {
                this.AddMessage("Chybí [Název odkazu]."); return false;
            }
            if (string.IsNullOrEmpty(rec.b07Value) && string.IsNullOrEmpty(rec.b07LinkUrl))
            {
                this.AddMessage("Chybí [Poznámka] nebo [URL odkaz]."); return false;
            }

            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _mother.CurrentUser.j02ID;

            return true;
        }

    }
}
