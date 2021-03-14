using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip32ActivityBL
    {
        public BO.p32Activity Load(int pid);
        public IEnumerable<BO.p32Activity> GetList(BO.myQueryP32 mq);
        public int Save(BO.p32Activity rec, int intP32ID_SystemDefault = 0);
       

    }
    class p32ActivityBL : BaseBL, Ip32ActivityBL
    {

        public p32ActivityBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*," + _db.GetSQL1_Ocas("p32"));
            sb(",p34.p34Name,p95.p95Name,x15.x15Name,p34.p33ID,p34.p34IncomeStatementFlag,p38.p38Name,p38.p38Ordinary");
            sb(" FROM p32Activity a INNER JOIN p34ActivityGroup p34 ON a.p34ID=p34.p34ID");
            sb(" LEFT OUTER JOIN p95InvoiceRow p95 ON a.p95ID=p95.p95ID LEFT OUTER JOIN p38ActivityTag p38 ON a.p38ID=p38.p38ID");
            sb(" LEFT OUTER JOIN x15VatRateType x15 ON a.x15ID=x15.x15ID");
            sb(strAppend);
            return sbret();
        }

        public BO.p32Activity Load(int pid)
        {
            return _db.Load<BO.p32Activity>(GetSQL1(" WHERE a.p32ID=@pid"), new { pid = pid });
        }
        public IEnumerable<BO.p32Activity> GetList(BO.myQueryP32 mq)
        {
            if (mq.explicit_orderby == null) mq.explicit_orderby = "a.p32Ordinary,a.p32Name";
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p32Activity>(fq.FinalSql, fq.Parameters);
        }

       

        public int Save(BO.p32Activity rec, int intP32ID_SystemDefault = 0)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }

            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.pid);
            p.AddEnumInt("p32SystemFlag", rec.p32SystemFlag);
            p.AddInt("p34ID", rec.p34ID,true);
            p.AddInt("p95ID", rec.p95ID, true);
            p.AddInt("p35ID", rec.p35ID, true);
            p.AddEnumInt("x15ID", rec.x15ID, true);

            p.AddInt("p38ID", rec.p38ID, true);

            p.AddString("p32Name", rec.p32Name);
            p.AddString("p32Code", rec.p32Code);
            p.AddString("p32DefaultWorksheetText", rec.p32DefaultWorksheetText);

            p.AddInt("p32Ordinary", rec.p32Ordinary);
            
            
            p.AddBool("p32IsBillable", rec.p32IsBillable);
            p.AddBool("p32IsTextRequired", rec.p32IsTextRequired);

            p.AddString("p32HelpText", rec.p32HelpText);
            p.AddString("p32Color", rec.p32Color);

            p.AddString("p32Name_BillingLang1", rec.p32Name_BillingLang1);
            p.AddString("p32Name_BillingLang2", rec.p32Name_BillingLang2);
            p.AddString("p32Name_BillingLang3", rec.p32Name_BillingLang3);
            p.AddString("p32Name_BillingLang4", rec.p32Name_BillingLang4);

            p.AddString("p32FreeText01", rec.p32FreeText01);
            p.AddString("p32FreeText02", rec.p32FreeText02);
            p.AddString("p32FreeText03", rec.p32FreeText03);

            p.AddString("p32DefaultWorksheetText_Lang1", rec.p32DefaultWorksheetText_Lang1);
            p.AddString("p32DefaultWorksheetText_Lang2", rec.p32DefaultWorksheetText_Lang2);
            p.AddString("p32DefaultWorksheetText_Lang3", rec.p32DefaultWorksheetText_Lang3);
            p.AddString("p32DefaultWorksheetText_Lang4", rec.p32DefaultWorksheetText_Lang4);

            p.AddDouble("p32Value_Default", rec.p32Value_Default);
            p.AddDouble("p32Value_Minimum", rec.p32Value_Minimum);
            p.AddDouble("p32Value_Maximum", rec.p32Value_Maximum);

            p.AddString("p32ExternalPID", rec.p32ExternalPID);
            p.AddEnumInt("p32AttendanceFlag", rec.p32AttendanceFlag);
            p.AddInt("p32ManualFeeFlag", rec.p32ManualFeeFlag);
            
            p.AddDouble("p32ManualFeeDefAmount", rec.p32ManualFeeDefAmount);
            p.AddDouble("p32MarginHidden", rec.p32MarginHidden);
            p.AddDouble("p32MarginTransparent", rec.p32MarginTransparent);

            p.AddString("p32AccountingIDS", rec.p32AccountingIDS);
            p.AddString("p32ActivityIDS", rec.p32ActivityIDS);

            p.AddBool("p32IsCP", rec.p32IsCP);
            p.AddBool("p32IsSupplier", rec.p32IsSupplier);

            int intPID = _db.SaveRecord("p32Activity", p, rec);

            return intPID;
        }


        private bool ValidateBeforeSave(BO.p32Activity rec)
        {

            if (string.IsNullOrEmpty(rec.p32Name))
            {
                this.AddMessage("Chybí název aktivity."); return false;
            }

            if (rec.p34ID == 0)
            {
                this.AddMessage("Chybí vazba na sešit."); return false;
            }

            if (rec.p32IsBillable && rec.p95ID==0)
            {
                this.AddMessage("U fakturovatelné aktivity musíte doplnit vazbu na fakturační oddíl. Jinak by mohlo dojít k zobrazování prázdných míst v cenovém rozpisu faktury!"); return false;
            }

            return true;
        }
    }
}
