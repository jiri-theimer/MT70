using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Io40SmtpAccountBL
    {
        public BO.o40SmtpAccount Load(int pid);
        public BO.o40SmtpAccount LoadGlobalDefault();
        public IEnumerable<BO.o40SmtpAccount> GetList(BO.myQueryO40 mq);
        public int Save(BO.o40SmtpAccount rec);

    }
    class o40SmtpAccountBL : BaseBL, Io40SmtpAccountBL
    {
        public o40SmtpAccountBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("o40"));
            sb(" FROM o40SmtpAccount a");
            sb(strAppend);
            return sbret();
        }
        public BO.o40SmtpAccount Load(int pid)
        {
            return _db.Load<BO.o40SmtpAccount>(GetSQL1(" WHERE a.o40ID=@pid"), new { pid = pid });
        }
        public BO.o40SmtpAccount LoadGlobalDefault()
        {
            return _db.Load<BO.o40SmtpAccount>(GetSQL1(" WHERE a.o40IsGlobalDefault=1 AND GETDATE() BETWEEN a.o40ValidFrom AND a.o40ValidUntil"));
        }

        public IEnumerable<BO.o40SmtpAccount> GetList(BO.myQueryO40 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.o40SmtpAccount>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.o40SmtpAccount rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _mother.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddBool("o40IsGlobalDefault", rec.o40IsGlobalDefault);
            p.AddString("o40Name", rec.o40Name);
            p.AddString("o40EmailAddress", rec.o40EmailAddress);
            p.AddString("o40Server", rec.o40Server);
            p.AddString("o40Login", rec.o40Login);
            p.AddString("o40Password", rec.o40Password);
            p.AddBool("o40IsVerify", rec.o40IsVerify);
            p.AddInt("o40Port", rec.o40Port);
            p.AddEnumInt("o40SslModeFlag", rec.o40SslModeFlag);
            p.AddBool("o40IsUsePersonalReply", rec.o40IsUsePersonalReply);
            

            return _db.SaveRecord("o40SmtpAccount", p, rec);


        }
        private bool ValidateBeforeSave(BO.o40SmtpAccount rec)
        {
            if (string.IsNullOrEmpty(rec.o40Name))
            {
                this.AddMessage("Chybí vyplnit [Jméno odesílatele]."); return false;
            }
            if (string.IsNullOrEmpty(rec.o40EmailAddress))
            {
                this.AddMessage("Chybí vyplnit [E-mail adresa]."); return false;
            }
            if (string.IsNullOrEmpty(rec.o40Server))
            {
                this.AddMessage("Chybí vyplnit [Server]."); return false;
            }

            return true;
        }

    }
}
