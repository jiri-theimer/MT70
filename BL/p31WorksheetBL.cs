using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public interface Ip31WorksheetBL
    {
        public BO.p31Worksheet Load(int pid);
        public BO.p31Worksheet LoadByExternalPID(string externalpid);
        public BO.p31Worksheet LoadTempRecord(int pid, string guidTempData);

        public int SaveOrigRecord(BO.p31WorksheetEntryInput rec, BO.p33IdENUM p33ID, List<BO.FreeField> lisFF);
        public BO.p31ValidateBeforeSave ValidateBeforeSaveOrigRecord(BO.p31WorksheetEntryInput rec);
        public IEnumerable<BO.p31Worksheet> GetList(BO.myQuery mq);
        

    }
    class p31WorksheetBL : BaseBL, Ip31WorksheetBL
    {
        
        public p31WorksheetBL(BL.Factory mother) : base(mother)
        {

        }

      


        private string GetSQL1(string strAppend = null,int intTopcRecs=0)
        {
            if (intTopcRecs > 0)
            {
                sb("SELECT TOP "+intTopcRecs.ToString());
            }
            else
            {
                sb("SELECT");
            }
            sb(" a.p41ID,a.j02ID,a.p32ID,a.p56ID,a.j02ID_Owner,a.j02ID_ApprovedBy,a.p31Code,a.p70ID,a.p71ID,a.p72ID_AfterApprove,a.p72ID_AfterTrimming,a.j27ID_Billing_Orig,a.j27ID_Billing_Invoiced,a.j27ID_Billing_Invoiced_Domestic,a.j27ID_Internal,a.p91ID,a.c11ID,a.p31Date,a.p31DateUntil,a.p31HoursEntryFlag,a.p31Approved_When,a.p31IsPlanRecord,a.p31Text,a.p31Value_Orig");
            sb(",dbo.p31_get_syscolumn(a.p72ID_AfterApprove,a.p70ID,a.p71ID,a.p91ID,a.o23ID_First,a.p72ID_AfterTrimming,p91.p91IsDraft,a.p31TimerTimestamp) as sc,ISNULL(a.p31Rate_Billing_Orig,0) as BillingRateOrig");
            sb(",a.p31Value_Trimmed,a.p31Value_Approved_Billing,a.p31Value_Approved_Internal,a.p31Value_Invoiced,a.p31Amount_WithoutVat_Orig,a.p31Amount_WithVat_Orig,a.p31Amount_Vat_Orig,a.p31VatRate_Orig,a.p31Amount_WithoutVat_FixedCurrency,a.p31Amount_WithoutVat_Invoiced,a.p31Amount_WithVat_Invoiced,a.p31Amount_Vat_Invoiced,a.p31VatRate_Invoiced,a.p31Amount_WithoutVat_Invoiced_Domestic,a.p31Amount_WithVat_Invoiced_Domestic,a.p31Amount_Vat_Invoiced_Domestic,a.p31Minutes_Orig,a.p31Minutes_Trimmed,a.p31Minutes_Approved_Billing,a.p31Minutes_Approved_Internal,a.p31Minutes_Invoiced");
            sb(",a.p31Hours_Orig,a.p31Hours_Trimmed,a.p31Hours_Approved_Billing,a.p31Hours_Approved_Internal,a.p31Hours_Invoiced,a.p31HHMM_Orig,a.p31HHMM_Trimmed,a.p31HHMM_Approved_Billing,a.p31HHMM_Approved_Internal,a.p31HHMM_Invoiced,a.p31Rate_Billing_Orig,a.p31Rate_Internal_Orig,a.p31Rate_Billing_Approved,a.p31Rate_Internal_Approved,a.p31Rate_Billing_Invoiced,a.p31Amount_WithoutVat_Approved,a.p31Amount_WithVat_Approved,a.p31Amount_Vat_Approved,a.p31VatRate_Approved,a.p31ExchangeRate_Domestic,a.p31ExchangeRate_Invoice,a.p31Amount_Internal");

            sb(",a.p31DateTimeFrom_Orig,a.p31DateTimeUntil_Orig,a.p31Value_Orig_Entried,a.p31Calc_Pieces,a.p31Calc_PieceAmount,a.p35ID,a.j19ID");
            sb(",j02.j02LastName+' '+j02.j02FirstName as Person,p32.p32Name,p32.p34ID,p32.p32IsBillable,p32.p32ManualFeeFlag,p34.p33ID,p34.p34Name,p34.p34IncomeStatementFlag,isnull(p41.p41NameShort,p41.p41Name) as p41Name,p41.p28ID_Client,p28Client.p28Name as ClientName,p28Client.p28CompanyShortName,p56.p56Name,p56.p56Code,j02owner.j02LastName+' '+j02owner.j02FirstName as Owner");
            sb(",p91.p91Code,p70.p70Name,p71.p71Name,p72trim.p72Name as trim_p72Name,p72approve.p72Name as approve_p72Name,j27billing_orig.j27Code as j27Code_Billing_Orig,p32.p95ID,p95.p95Name,a.p31ApprovingLevel,a.p31Value_FixPrice,a.o23ID_First,a.p28ID_Supplier,supplier.p28Name as SupplierName,a.p49ID,a.j02ID_ContactPerson,cp.j02LastName+' '+cp.j02FirstName as ContactPerson,a.p31IsInvoiceManual");
            sb(",a.p31MarginHidden,a.p31MarginTransparent,a.p31PostRecipient,a.p31PostCode,a.p31PostFlag,p41.p48ID,p48.p48Name,a.p31TimerTimestamp,a.p31Value_Off");

            sb(",");
            sb(_db.GetSQL1_Ocas("p31"));

            sb(" FROM p31Worksheet a");

            sb(" INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID");
            sb(" INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID");
            sb(" LEFT OUTER JOIN p28Contact p28Client ON p41.p28ID_Client=p28Client.p28ID LEFT OUTER JOIN p48ProjectGroup p48 ON p41.p48ID=p48.p48ID");
            sb(" LEFT OUTER JOIN p56Task p56 ON a.p56ID=p56.p56ID LEFT OUTER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID");
            sb(" LEFT OUTER JOIN p70BillingStatus p70 ON a.p70ID=p70.p70ID LEFT OUTER JOIN p71ApproveStatus p71 ON a.p71ID=p71.p71ID LEFT OUTER JOIN p72PreBillingStatus p72trim ON a.p72ID_AfterTrimming=p72trim.p72ID LEFT OUTER JOIN p72PreBillingStatus p72approve ON a.p72ID_AfterApprove=p72approve.p72ID");
            sb(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID LEFT OUTER JOIN j02Person cp ON a.j02ID_ContactPerson=cp.j02ID LEFT OUTER JOIN p28Contact supplier ON a.p28ID_Supplier=supplier.p28ID");
            sb(" LEFT OUTER JOIN j27Currency j27billing_orig ON a.j27ID_Billing_Orig=j27billing_orig.j27ID");
            sb(" LEFT OUTER JOIN p95InvoiceRow p95 ON p32.p95ID=p95.p95ID LEFT OUTER JOIN p38ActivityTag p38 ON p32.p38ID=p38.p38ID");


            sb(strAppend);
            return sbret();
        }
        public BO.p31Worksheet Load(int pid)
        {
            return _db.Load<BO.p31Worksheet>(GetSQL1(" WHERE a.p31ID=@pid"), new { pid = pid });
        }
        public BO.p31Worksheet LoadByExternalPID(string externalpid)
        {
            return _db.Load<BO.p31Worksheet>(GetSQL1(" WHERE a.p31ExternalPID=@externalpid"), new { externalpid = externalpid });
        }
        public BO.p31Worksheet LoadTempRecord(int pid,string guidTempData)
        {
            return _db.Load<BO.p31Worksheet>(GetSQL1(" WHERE a.p31ID=@p31id AND a.p31GUID=@guid"), new {p31id=pid, guid = guidTempData });
        }
        public BO.p31Worksheet LoadMyLastCreated(bool bolLoadTheSameProjectTypeIfNoData, int intP41ID = 0, int intP34ID = 0)
        {
            string s = " WHERE a.j02ID_Owner=@j02id";
            var pars = new Dapper.DynamicParameters();
            pars.Add("j02id", _mother.CurrentUser.j02ID,System.Data.DbType.Int32);
            if (intP41ID > 0)
            {
                s += " AND a.p41ID=@p41id";
                pars.Add("p41id", intP41ID, System.Data.DbType.Int32);                
            }
            if (intP34ID > 0)
            {
                s += " AND p32.p34ID=@p34id";
                pars.Add("p34id", intP34ID, System.Data.DbType.Int32);
            }

            s += " ORDER BY a.p31ID DESC";
            var rec = _db.Load<BO.p31Worksheet>(GetSQL1(s,1),pars);
            if (bolLoadTheSameProjectTypeIfNoData && rec==null && intP41ID>0)
            {
                s = " WHERE a.j02ID_Owner=@j02id AND p41.p42ID IN (SELECT p42ID FROM p41Project WHERE p41ID=@p41id) ORDER BY a.p31ID DESC";
                rec = _db.Load<BO.p31Worksheet>(GetSQL1(s, 1), pars);
            }

            return rec;
        }

        public IEnumerable<BO.p31Worksheet> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p31Worksheet>(fq.FinalSql, fq.Parameters);
        }


        public int SaveOrigRecord(BO.p31WorksheetEntryInput rec, BO.p33IdENUM p33ID, List<BO.FreeField> lisFF)
        {
            if (rec.p41ID==0 && rec.p56ID > 0)  //dohledat projekt podle úkolu
            {
                
            }
            if (rec.p41ID == 0)
            {
                this.AddMessage("Chybí projekt.");return 0;
            }
            if (rec.j02ID == 0)
            {
                this.AddMessage("Chybí osobní profil."); return 0;
            }
            if (rec.p34ID == 0)
            {
                this.AddMessage("Chybí sešit aktivity."); return 0;
            }
            if (rec.p32ID == 0)
            {
                var recP34 = _mother.p34ActivityGroupBL.Load(rec.p34ID);
                if (recP34.p34ActivityEntryFlag == BO.p34ActivityEntryFlagENUM.AktivitaJePovinna)
                {
                    this.AddMessage(String.Format("Sešit [{0}] vyžaduje zadat aktivitu.", recP34.p34Name));return 0;
                }
                //zkusit najít výchozí systémovou aktivitu, pokud se aktivita nemá zadávat
                var mq = new BO.myQueryP32() { p34id = rec.p34ID,IsRecordValid=true };
                var lisP32 = _mother.p32ActivityBL.GetList(mq).Where(p => p.p32IsSystemDefault == true);
                if (lisP32.Count() > 0)
                {
                    rec.p32ID = lisP32.First().pid;
                }
                
                
            }
            if (rec.pid == 0 && string.IsNullOrEmpty(rec.Value_Orig_Entried))
            {
                rec.Value_Orig_Entried = rec.Value_Orig;
            }
            if (rec.p31RecordSourceFlag > 0)
            {
                rec.ValidateEntryTime(0);   //hodiny zadané mimo web aplikaci
            }

            var vld = BL.bas.p31Support.ValidateBeforeSaveOrigRecord(_mother, _db, rec);
            if (!string.IsNullOrEmpty(vld.ErrorMessage))
            {
                this.AddMessage(vld.ErrorMessage); return 0;
            }
            switch (vld.p33ID)
            {
                case BO.p33IdENUM.Cas:
                    if (!rec.ValidateEntryTime(vld.Round2Minutes))
                    {
                        this.AddMessage(rec.ErrorMessage); return 0;
                    }
                    if (rec.p72ID_AfterTrimming != BO.p72IdENUM._NotSpecified)
                    {
                        if (!rec.ValidateTrimming(rec.p72ID_AfterTrimming, rec.Value_Trimmed,vld.p33ID))
                        {
                            this.AddMessage(rec.ErrorMessage); return 0;
                        }
                    }
                    if (rec.p72ID_AfterTrimming==BO.p72IdENUM.Fakturovat && rec.p31Hours_Orig == rec.p31Hours_Trimmed)
                    {
                        
                    }
                    if (rec.p72ID_AfterTrimming==BO.p72IdENUM.Fakturovat && rec.p32ID > 0)
                    {
                        
                    }
                    break;
                case BO.p33IdENUM.Kusovnik:
                    if (!rec.ValidateEntryKusovnik())
                    {
                        this.AddMessage(rec.ErrorMessage); return 0;
                    }
                    break;
                case BO.p33IdENUM.PenizeBezDPH:
                    if (rec.Value_OffBilling == 0)
                    {
                        if (rec.j27ID_Billing_Orig == 0)
                        {
                            this.AddMessage("Chybí měna"); return 0;
                        }
                        if (rec.Amount_WithoutVat_Orig == 0)
                        {
                            this.AddMessage("Částka nesmí být NULA."); return 0;
                        }
                    }
                    rec.RecalcEntryAmount(rec.Amount_WithoutVat_Orig, vld.VatRate); //dopočítat částku vč. DPH
                    rec.VatRate_Orig = vld.VatRate;
                    break;
                case BO.p33IdENUM.PenizeVcDPHRozpisu:
                    if (rec.Value_OffBilling == 0)
                    {
                        if (rec.j27ID_Billing_Orig == 0)
                        {
                            this.AddMessage("Chybí měna"); return 0;
                        }
                        if (rec.Amount_WithoutVat_Orig == 0 && rec.Amount_WithVat_Orig==0)
                        {
                            this.AddMessage("Částka nesmí být NULA."); return 0;
                        }
                    }
                    rec.SetAmounts();
                    if (rec.VatRate_Orig != 0 && (rec.Amount_WithVat_Orig==0 || rec.Amount_Vat_Orig == 0)){
                        rec.RecalcEntryAmount(rec.Amount_WithoutVat_Orig, rec.VatRate_Orig);
                    }
                    if (Math.Abs((rec.p31Amount_WithoutVat_Orig + rec.p31Amount_Vat_Orig) - rec.p31Amount_WithVat_Orig) > 0.02)
                    {
                        this.AddMessage(string.Format("Součet základu a částky DPH se liší od celkové částky vč. DPH! Rozdíl: {0}",rec.p31Amount_WithoutVat_Orig+rec.p31Amount_Vat_Orig-rec.p31Amount_WithVat_Orig));
                        return 0;
                    }
                  
                    break;
            }


            return BL.bas.p31Support.SaveOrigRecord(_mother,_db,rec, p33ID, lisFF);
        }


        public BO.p31ValidateBeforeSave ValidateBeforeSaveOrigRecord(BO.p31WorksheetEntryInput rec)
        {
            return BL.bas.p31Support.ValidateBeforeSaveOrigRecord(_mother, _db, rec);
        }

    }
}
