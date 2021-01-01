using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Io13AttachmentTypeBL
    {
        public BO.o13AttachmentType Load(int pid);
        public IEnumerable<BO.o13AttachmentType> GetList(BO.myQuery mq);
        public int Save(BO.o13AttachmentType rec);
        

    }
    class o13AttachmentTypeBL : BaseBL, Io13AttachmentTypeBL
    {
        public o13AttachmentTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,x29.x29Name,");            
            sb("replace(a.o27ArchiveFolder,CHAR(92),CHAR(92)+CHAR(92)) as SharpFolder,");
            sb(_db.GetSQL1_Ocas("o13"));
            sb(" FROM o13AttachmentType a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID");
            sb(strAppend);
            
            
            return sbret();
        }
        public BO.o13AttachmentType Load(int pid)
        {
            return _db.Load<BO.o13AttachmentType>(GetSQL1(" WHERE a.o13ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.o13AttachmentType> GetList(BO.myQuery mq)
        {                        
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.o13AttachmentType>(fq.FinalSql, fq.Parameters);
        }

        

        public int Save(BO.o13AttachmentType rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddInt("x29ID", rec.x29ID, true);
            p.AddString("o13Name", rec.o13Name);            
            p.AddString("o13ArchiveFolder", rec.o13ArchiveFolder);
            p.AddBool("o13IsUniqueArchiveFileName", rec.o13IsUniqueArchiveFileName);
            p.AddBool("o13IsArchiveFolderWithPeriodSuffix", rec.o13IsArchiveFolderWithPeriodSuffix);

            int intPID = _db.SaveRecord("o13AttachmentType", p.getDynamicDapperPars(), rec);
            
            return intPID;
        }

        public bool ValidateBeforeSave(BO.o13AttachmentType rec)
        {
            
            if (string.IsNullOrEmpty(rec.o13Name) || rec.x29ID==0 || string.IsNullOrEmpty(rec.o13ArchiveFolder)==true)
            {
                this.AddMessage("[Název] a [Archiv složka] jsou povinná pole."); return false;
            }
            
            return true;
        }

    }
}
