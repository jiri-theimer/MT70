using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip89InvoiceTypeBL
    {
        public BO.p89ProformaType Load(int pid);
        public IEnumerable<BO.p89ProformaType> GetList(BO.myQuery mq);
        public int Save(BO.p89ProformaType rec);

    }
    class p89InvoiceTypeBL : BaseBL, Ip89InvoiceTypeBL
    {
        public p89InvoiceTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,j27.j27Code,p93.p93Name,");
            sb(_db.GetSQL1_Ocas("p89"));
            sb(" FROM p89ProformaType a LEFT OUTER JOIN j27Currency j27 ON a.j27ID=j27.j27ID");
            sb(" LEFT OUTER JOIN p93InvoiceHeader p93 ON a.p93ID=p93.p93ID");
            sb(strAppend);
            return sbret();
        }
        public BO.p89ProformaType Load(int pid)
        {
            return _db.Load<BO.p89ProformaType>(GetSQL1(" WHERE a.p89ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.p89ProformaType> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p89ProformaType>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.p89ProformaType rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("p89Name", rec.p89Name);
            p.AddString("p89Code", rec.p89Code);
 
            p.AddInt("x31ID", rec.x31ID, true);
            p.AddInt("x31ID_Payment", rec.x31ID_Payment, true);
            p.AddInt("x38ID", rec.x38ID, true);
            p.AddInt("x38ID_Payment", rec.x38ID_Payment, true);
            
            p.AddInt("p93ID", rec.p93ID, true);
            
            
            p.AddString("p89DefaultText1", rec.p89DefaultText1);
            p.AddString("p89DefaultText2", rec.p89DefaultText2);
           

            int intPID = _db.SaveRecord("p89InvoiceType", p.getDynamicDapperPars(), rec);

            return intPID;


        }
        private bool ValidateBeforeSave(BO.p89ProformaType rec)
        {
            if (string.IsNullOrEmpty(rec.p89Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.x38ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Číselná řada]."); return false;
            }
           

            return true;
        }

    }
}
