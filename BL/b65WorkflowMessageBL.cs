﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ib65WorkflowMessageBL
    {
        public BO.b65WorkflowMessage Load(int pid);
        
        public IEnumerable<BO.b65WorkflowMessage> GetList(BO.myQuery mq);
        public int Save(BO.b65WorkflowMessage rec);

        public BO.b65WorkflowMessage MailMergeRecord(BO.b65WorkflowMessage recB65, int datapid, string param1);
        public BO.b65WorkflowMessage MailMergeRecord(int b65id, int datapid, string param1);

    }
    class b65WorkflowMessageBL : BaseBL, Ib65WorkflowMessageBL
    {
        public b65WorkflowMessageBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,x29.x29Name,LEFT(x29.x29TableName,3) as Prefix,b01.b01Name,");
            sb(_db.GetSQL1_Ocas("b65"));
            sb(" FROM b65WorkflowMessage a INNER JOIN b01WorkflowTemplate b01 ON a.b01ID=b01.b01ID LEFT OUTER JOIN x29Entity x29 ON b01.x29ID=x29.x29ID");
            sb(strAppend);
            return sbret();
        }
        public BO.b65WorkflowMessage Load(int pid)
        {
            return _db.Load<BO.b65WorkflowMessage>(GetSQL1(" WHERE a.b65ID=@pid"), new { pid = pid });
        }
        


        public IEnumerable<BO.b65WorkflowMessage> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.b65Name"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.b65WorkflowMessage>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.b65WorkflowMessage rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.b65ID);
            p.AddInt("b01ID", rec.b01ID, true);
            p.AddString("b65Name", rec.b65Name);
            p.AddString("b65MessageSubject", rec.b65MessageSubject);           
            p.AddString("b65MessageBody", rec.b65MessageBody);
            
            int intPID = _db.SaveRecord("b65WorkflowMessage", p, rec);
           
            return intPID;
        }

        public bool ValidateBeforeSave(BO.b65WorkflowMessage rec)
        {
            if (string.IsNullOrEmpty(rec.b65Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (string.IsNullOrEmpty(rec.b65MessageSubject))
            {
                this.AddMessage("Chybí vyplnit [Předmět zprávy]."); return false;
            }

            if (rec.x29ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Entita]."); return false;
            }
            


            return true;
        }

        public BO.b65WorkflowMessage MailMergeRecord(int b65id, int datapid, string param1)
        {
            if (b65id == 0) return null;
            return MailMergeRecord(Load(b65id), datapid, param1);
        }
        public BO.b65WorkflowMessage MailMergeRecord(BO.b65WorkflowMessage recB65, int datapid, string param1)
        {
            if (recB65 == null) return null;
            var dt = _mother.gridBL.GetList4MailMerge(recB65.Prefix, datapid);
            if (dt.Rows.Count == 0) return recB65;

            var cMerge = new BO.CLS.MergeContent();
            recB65.b65MessageBody = cMerge.GetMergedContent(recB65.b65MessageBody, dt).Replace("#param1", param1, StringComparison.OrdinalIgnoreCase).Replace("#password#", param1);
            recB65.b65MessageSubject = cMerge.GetMergedContent(recB65.b65MessageSubject, dt).Replace("#param1", param1, StringComparison.OrdinalIgnoreCase);
            return recB65;
        }

    }
}
