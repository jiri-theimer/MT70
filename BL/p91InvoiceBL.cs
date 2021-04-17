using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BL
{
    public interface Ip91InvoiceBL
    {
        public BO.p91Invoice Load(int pid);
        public IEnumerable<BO.p91Invoice> GetList(BO.myQueryP91 mq);
        public int Update(BO.p91Invoice rec, List<BO.FreeFieldInput> lisFFI);
        public int Create(BO.p91Create rec);

    }
    class p91InvoiceBL : BaseBL, Ip91InvoiceBL
    {
        public p91InvoiceBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT ");
            sb(_db.GetSQL1_Ocas("p91"));
            sb(",a.p92ID,a.p28ID,a.j27ID,a.j19ID,a.j02ID_Owner,a.p41ID_First,a.p91ID_CreditNoteBind,a.j17ID,a.p98ID,a.p63ID,a.p80ID,a.o38ID_Primary,a.o38ID_Delivery,a.x15ID,a.b02ID,a.j02ID_ContactPerson,a.p91FixedVatRate,a.p91Code,a.p91IsDraft,a.p91Date,a.p91DateBilled,a.p91DateMaturity,a.p91DateSupply,a.p91DateExchange,a.p91ExchangeRate");
            
            sb(",a.p91Datep31_From,a.p91Datep31_Until,a.p91Amount_WithoutVat,a.p91Amount_Vat,a.p91Amount_Billed,a.p91Amount_WithVat,a.p91Amount_Debt,a.p91RoundFitAmount,a.p91Text1,a.p91Text2,a.p91ProformaAmount,a.p91ProformaBilledAmount,a.p91Amount_WithoutVat_None,a.p91VatRate_Low,a.p91Amount_WithVat_Low,a.p91Amount_WithoutVat_Low,a.p91Amount_Vat_Low");
            sb(",a.p91VatRate_Standard,a.p91Amount_WithVat_Standard,a.p91Amount_WithoutVat_Standard,a.p91Amount_Vat_Standard,a.p91VatRate_Special,a.p91Amount_WithVat_Special,a.p91Amount_WithoutVat_Special,a.p91Amount_Vat_Special,a.p91Amount_TotalDue");            
            sb(",p28client.p28Name,p92.p92Name,p92.p93ID,isnull(p41.p41NameShort,p41.p41Name) as p41Name,b02.b02Name,j02owner.j02LastName+' '+j02owner.j02FirstName as Owner,j17.j17Name,j27.j27Code,p92.p92InvoiceType,p92.b01ID,p28client.p28CompanyName");
                        
            sb(" FROM p91Invoice a");
            sb(" INNER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID");
            sb(" INNER JOIN j27Currency j27 ON a.j27ID=j27.j27ID");
            sb(" LEFT OUTER JOIN p41Project p41 ON a.p41ID_First=p41.p41ID");
            sb(" LEFT OUTER JOIN p28Contact p28client ON a.p28ID=p28client.p28ID");
            sb(" LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID");
            sb(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID");
            sb(" LEFT OUTER JOIN j17Country j17 ON a.j17ID=j17.j17ID");        

            sb(strAppend);
            return sbret();
        }
        public BO.p91Invoice Load(int pid)
        {
            return _db.Load<BO.p91Invoice>(GetSQL1(" WHERE a.p91ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.p91Invoice> GetList(BO.myQueryP91 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p91Invoice>(fq.FinalSql, fq.Parameters);
        }

        public int Create(BO.p91Create rec)
        {
            using (var sc = new System.Transactions.TransactionScope())
            {
                var pars = new Dapper.DynamicParameters();

                pars.Add("guid", rec.TempGUID, DbType.String);
                pars.Add("j03id_sys", _mother.CurrentUser.pid, DbType.Int32);
                pars.Add("p28id", BO.BAS.TestIntAsDbKey(rec.p28ID), DbType.Int32);
                pars.Add("p92id", BO.BAS.TestIntAsDbKey(rec.p92ID), DbType.Int32);
                pars.Add("p91isdraft", rec.IsDraft, DbType.Boolean);
                pars.Add("p91date", rec.DateIssue, DbType.DateTime);
                pars.Add("p91datematurity", rec.DateMaturity, DbType.DateTime);
                pars.Add("p91datesupply", rec.DateSupply, DbType.DateTime);
                pars.Add("p91datep31_from", rec.DateP31_From, DbType.DateTime);
                pars.Add("p91datep31_until", rec.DateP31_Until, DbType.DateTime);
                pars.Add("p91text1", rec.InvoiceText1, DbType.String);
                pars.Add("j02id_contactperson", BO.BAS.TestIntAsDbKey(rec.j02ID_ContactPerson), DbType.Int32);
                pars.Add("ret_p91id",0 , DbType.Int32, ParameterDirection.Output);
                pars.Add("err_ret", null, DbType.String, ParameterDirection.Output,1000);

                
                if (_db.RunSp("p91_create",ref pars) == "1")
                {
                    sc.Complete();

                    return pars.Get<int>("ret_p91id");
                }                
                

                return 0;

            }
        }

        public int Update(BO.p91Invoice rec, List<BO.FreeFieldInput> lisFFI)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddBool("p91IsDraft", rec.p91IsDraft);
                
                p.AddInt("p92ID", rec.p92ID, true);
                p.AddInt("p28ID", rec.p28ID, true);
                p.AddInt("j27ID", rec.j27ID, true);
                p.AddInt("j19ID", rec.j19ID, true);
                p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
                p.AddInt("j17ID", rec.j17ID, true);
                p.AddInt("p98ID", rec.p98ID, true);
                p.AddInt("p63ID", rec.p63ID, true);
                p.AddInt("p80ID", rec.p80ID, true);
                p.AddInt("j02ID_ContactPerson", rec.j02ID_ContactPerson, true);
                p.AddInt("o38ID_Primary", rec.o38ID_Primary, true);
                p.AddInt("o38ID_Delivery", rec.o38ID_Delivery, true);
                p.AddInt("p91LockFlag", rec.p91LockFlag);

                p.AddString("p91Code", rec.p91Code);
                p.AddString("p91Text1", rec.p91Text1);
                p.AddString("p91Text2", rec.p91Text2);

                p.AddDateTime("p91Date", rec.p91Date);
                p.AddDateTime("p91DateMaturity", rec.p91DateMaturity);
                p.AddDateTime("p91DateSupply", rec.p91DateSupply);
                p.AddDateTime("p91Datep31_From", rec.p91Datep31_From);
                p.AddDateTime("p91Datep31_Until", rec.p91Datep31_Until);

                p.AddString("p91Client", rec.p91Client);
                p.AddString("p91ClientPerson", rec.p91ClientPerson);
                p.AddString("p91ClientPerson_Salutation", rec.p91ClientPerson_Salutation);
                p.AddString("p91Client_RegID", rec.p91Client_RegID);
                p.AddString("p91Client_VatID", rec.p91Client_VatID);
                p.AddString("p91ClientAddress1_Street", rec.p91ClientAddress1_Street);
                p.AddString("p91ClientAddress1_City", rec.p91ClientAddress1_City);
                p.AddString("p91ClientAddress1_ZIP", rec.p91ClientAddress1_ZIP);
                p.AddString("p91ClientAddress1_Country", rec.p91ClientAddress1_Country);

                p.AddString("p91ClientAddress2", rec.p91ClientAddress2);
                p.AddString("p91Client_ICDPH_SK", rec.p91Client_ICDPH_SK);


                int intPID = _db.SaveRecord("p91Invoice", p, rec);
                if (intPID > 0)
                {
                    if (!DL.BAS.SaveFreeFields(_db, intPID, lisFFI))
                    {
                        return 0;
                    }
                                       
                    if (_db.RunSql("exec dbo.p91_aftersave @p91id,@j03id_sys,@recalc_amount", new { p91id = intPID, j03id_sys = _mother.CurrentUser.pid, recalc_amount = true }))
                    {
                        sc.Complete();
                        return intPID;
                    }

                     

                }

                return intPID;
            }


        }
        private bool ValidateBeforeSave(BO.p91Invoice rec)
        {
            if (string.IsNullOrEmpty(rec.p91Code))
            {
                rec.p91Code = "TEMP" + BO.BAS.GetGuid();    //dočasný kód, bude později nahrazen
            }
            if (rec.j27ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Měna]."); return false;
            }
            if (rec.p92ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Typ faktury]."); return false;
            }
            if (rec.p28ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Klient]."); return false;
            }


            return true;
        }


        private void Handle_RecalcAmount(int p91id)
        {
            _db.RunSql("exec dbo.p91_recalc_amount @p91id", new { p91id = p91id });
        }

    }
}
