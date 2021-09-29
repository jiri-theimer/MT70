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
        public BO.p31Worksheet LoadMyLastCreated(bool bolLoadTheSameProjectTypeIfNoData, int intP41ID = 0, int intP34ID = 0);
        public BO.p31WorksheetEntryInput CovertRec2Input(BO.p31Worksheet rec, bool time_entry_by_minutes);
        public int SaveOrigRecord(BO.p31WorksheetEntryInput rec, BO.p33IdENUM p33ID, List<BO.FreeFieldInput> lisFF);
        public BO.p31ValidateBeforeSave ValidateBeforeSaveOrigRecord(BO.p31WorksheetEntryInput rec);
        public IEnumerable<BO.p31Worksheet> GetList(BO.myQueryP31 mq);
        public void UpdateExternalPID(int pid, string strExternalPID);
        public BO.p31RecDisposition InhaleRecDisposition(BO.p31Worksheet rec);
        public bool UpdateInvoice(int p91id, List<BO.p31WorksheetInvoiceChange> lisP31, List<BO.FreeFieldInput> lisFFI);
        public bool RemoveFromInvoice(int p91id, List<int> p31ids);
        public bool RemoveFromApprove(List<int> p31ids);
        public bool Move2Invoice(int p91id_dest, int p31id);
        public bool Move2Bin(bool move2bin, List<int> p31ids);
        public bool ValidateVatRate(double vatrate, int p41id, DateTime d, int j27id);
        public IEnumerable<BO.p31WorksheetTimelineDay> GetList_TimelineDays(List<int> j02ids, DateTime d1, DateTime d2, int j70id);
        public BO.p31Rate LoadRate(BO.p51TypeFlagENUM flag, DateTime d, int j02id, int p41id, int p32id);
        public BO.p72IdENUM Get_p72ID_NonBillableWork(int p31id);
        public bool Save_Approving(BO.p31WorksheetApproveInput c, bool istempdata);
        public bool Validate_Before_Save_Approving(BO.p31WorksheetApproveInput c, bool istempdata);
        public void DeleteTempRecord(string guid, int p31id);

    }
    class p31WorksheetBL : BaseBL, Ip31WorksheetBL
    {
        
        public p31WorksheetBL(BL.Factory mother) : base(mother)
        {

        }

      


        private string GetSQL1(string strAppend = null,int intTopcRecs=0,bool istemprecord=false)
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
            sb(",ISNULL(a.p31Rate_Billing_Orig,0) as BillingRateOrig");
            sb(",a.p31Value_Trimmed,a.p31Value_Approved_Billing,a.p31Value_Approved_Internal,a.p31Value_Invoiced,a.p31Amount_WithoutVat_Orig,a.p31Amount_WithVat_Orig,a.p31Amount_Vat_Orig,a.p31VatRate_Orig,a.p31Amount_WithoutVat_FixedCurrency,a.p31Amount_WithoutVat_Invoiced,a.p31Amount_WithVat_Invoiced,a.p31Amount_Vat_Invoiced,a.p31VatRate_Invoiced,a.p31Amount_WithoutVat_Invoiced_Domestic,a.p31Amount_WithVat_Invoiced_Domestic,a.p31Amount_Vat_Invoiced_Domestic,a.p31Minutes_Orig,a.p31Minutes_Trimmed,a.p31Minutes_Approved_Billing,a.p31Minutes_Approved_Internal,a.p31Minutes_Invoiced");
            sb(",a.p31Hours_Orig,a.p31Hours_Trimmed,a.p31Hours_Approved_Billing,a.p31Hours_Approved_Internal,a.p31Hours_Invoiced,a.p31HHMM_Orig,a.p31HHMM_Trimmed,a.p31HHMM_Approved_Billing,a.p31HHMM_Approved_Internal,a.p31HHMM_Invoiced,a.p31Rate_Billing_Orig,a.p31Rate_Internal_Orig,a.p31Rate_Billing_Approved,a.p31Rate_Internal_Approved,a.p31Rate_Billing_Invoiced,a.p31Amount_WithoutVat_Approved,a.p31Amount_WithVat_Approved,a.p31Amount_Vat_Approved,a.p31VatRate_Approved,a.p31ExchangeRate_Domestic,a.p31ExchangeRate_Invoice,a.p31Amount_Internal");
            
            sb(",a.p31DateTimeFrom_Orig,a.p31DateTimeUntil_Orig,a.p31Value_Orig_Entried,a.p31Calc_Pieces,a.p31Calc_PieceAmount,a.p35ID,a.j19ID");
            sb(",j02.j02LastName+' '+j02.j02FirstName as Person,p32.p32Name,p32.p34ID,p32.p32IsBillable,p32.p32ManualFeeFlag,p34x.p33ID,p34x.p34Name,p34x.p34IncomeStatementFlag,isnull(p41x.p41NameShort,p41x.p41Name) as p41Name,p41x.p28ID_Client,p28Client.p28Name as ClientName,p28Client.p28CompanyShortName,p56.p56Name,p56.p56Code,j02owner.j02LastName+' '+j02owner.j02FirstName as Owner");
            sb(",p91.p91Code,p91.p91IsDraft,p70.p70Name,p71.p71Name,p72trim.p72Name as trim_p72Name,p72approve.p72Name as approve_p72Name,j27billing_orig.j27Code as j27Code_Billing_Orig,p32.p95ID,p95.p95Name,a.p31ApprovingLevel,a.p31Value_FixPrice,a.o23ID_First,a.p28ID_Supplier,supplier.p28Name as SupplierName,a.p49ID,a.j02ID_ContactPerson,cp.j02LastName+' '+cp.j02FirstName as ContactPerson,a.p31IsInvoiceManual");
            sb(",a.p31MarginHidden,a.p31MarginTransparent,a.p31PostRecipient,a.p31PostCode,a.p31PostFlag,p41x.p48ID,p48.p48Name,a.p31TimerTimestamp,a.p31Value_Off");
            sb(",p41x.p42ID,p42.p42Name");            
            sb(",");
            sb(_db.GetSQL1_Ocas("p31"));

            if (istemprecord)
            {
                sb(" FROM p31Worksheet_Temp a");
            }
            else
            {
                sb(",a.p31ExternalPID");
                sb(" FROM p31Worksheet a");
            }
            

            sb(" INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID");
            sb(" INNER JOIN p34ActivityGroup p34x ON p32.p34ID=p34x.p34ID INNER JOIN p41Project p41x ON a.p41ID=p41x.p41ID INNER JOIN p42ProjectType p42 ON p41x.p42ID=p42.p42ID");
            sb(" LEFT OUTER JOIN p28Contact p28Client ON p41x.p28ID_Client=p28Client.p28ID LEFT OUTER JOIN p48ProjectGroup p48 ON p41x.p48ID=p48.p48ID");
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
            return _db.Load<BO.p31Worksheet>(GetSQL1(" WHERE a.p31ID=@p31id AND a.p31GUID=@guid",0,true), new {p31id=pid, guid = guidTempData });
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
                s = " WHERE a.j02ID_Owner=@j02id AND p41x.p42ID IN (SELECT p42ID FROM p41Project WHERE p41ID=@p41id) ORDER BY a.p31ID DESC";
                rec = _db.Load<BO.p31Worksheet>(GetSQL1(s, 1), pars);
            }

            return rec;
        }

        public IEnumerable<BO.p31Worksheet> GetList(BO.myQueryP31 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p31Worksheet>(fq.FinalSql, fq.Parameters);
        }

        public void UpdateExternalPID(int pid, string strExternalPID)
        {
            BL.bas.p31Support.UpdateExternalPID(_mother, _db,pid,strExternalPID);
        }
        public int SaveOrigRecord(BO.p31WorksheetEntryInput rec, BO.p33IdENUM p33ID, List<BO.FreeFieldInput> lisFF)
        {

            if (rec.p41ID==0 && rec.p56ID > 0)  //dohledat projekt podle úkolu
            {
                
            }
            if (rec.p41ID == 0)
            {
                rec.SetError("Chybí projekt.");
                this.AddMessage(rec.ErrorMessage);return 0;
            }
            if (rec.j02ID == 0)
            {
                rec.SetError("Chybí osobní profil.");
                this.AddMessage(rec.ErrorMessage); return 0;
            }
            if (rec.p34ID == 0)
            {
                rec.SetError("Chybí sešit aktivity.");
                this.AddMessage(rec.ErrorMessage); return 0;
            }
            if (rec.p32ID == 0)
            {
                var recP34 = _mother.p34ActivityGroupBL.Load(rec.p34ID);
                if (recP34.p34ActivityEntryFlag == BO.p34ActivityEntryFlagENUM.AktivitaJePovinna)
                {
                    rec.SetError(String.Format("Sešit [{0}] vyžaduje zadat aktivitu.", recP34.p34Name));
                    this.AddMessage(rec.ErrorMessage);return 0;
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
                            rec.SetError("Chybí měna.");
                            this.AddMessage(rec.ErrorMessage); return 0;
                        }
                        if (rec.Amount_WithoutVat_Orig == 0)
                        {
                            rec.SetError("Částka nesmí být NULA.");
                            this.AddMessage(rec.ErrorMessage); return 0;
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
                            rec.SetError("Chybí měna.");
                            this.AddMessage(rec.ErrorMessage); return 0;
                        }
                        if (rec.Amount_WithoutVat_Orig == 0 && rec.Amount_WithVat_Orig==0)
                        {
                            rec.SetError("Částka nesmí být NULA.");
                            this.AddMessage(rec.ErrorMessage); return 0;
                        }
                    }
                    rec.SetAmounts();
                    if (rec.VatRate_Orig != 0 && (rec.Amount_WithVat_Orig==0 || rec.Amount_Vat_Orig == 0)){
                        rec.RecalcEntryAmount(rec.Amount_WithoutVat_Orig, rec.VatRate_Orig);
                    }
                    if (Math.Abs((rec.p31Amount_WithoutVat_Orig + rec.p31Amount_Vat_Orig) - rec.p31Amount_WithVat_Orig) > 0.02)
                    {
                        rec.SetError(string.Format("Součet základu a částky DPH se liší od celkové částky vč. DPH! Rozdíl: {0}", rec.p31Amount_WithoutVat_Orig + rec.p31Amount_Vat_Orig - rec.p31Amount_WithVat_Orig));
                        this.AddMessage(rec.ErrorMessage);
                        return 0;
                    }
                  
                    break;
            }

            
            int intPID = BL.bas.p31Support.SaveOrigRecord(_mother,_db,rec, p33ID, lisFF);
            

            return intPID;
        }


        public BO.p31ValidateBeforeSave ValidateBeforeSaveOrigRecord(BO.p31WorksheetEntryInput rec)
        {
            return BL.bas.p31Support.ValidateBeforeSaveOrigRecord(_mother, _db, rec);
        }

        public bool UpdateInvoice(int p91id,List<BO.p31WorksheetInvoiceChange> lisP31, List<BO.FreeFieldInput> lisFFI)
        {
            if (lisP31.Count() == 0)
            {
                this.AddMessage("Na vstupu chybí úkon.");return false;
            }
            if (lisP31.Any(p=>p.p70ID==BO.p70IdENUM.Nic))
            {
                this.AddMessage("Na vstupu je minimálně jeden úkon, který postrádá fakturační status."); return false;
            }
            if (lisP31.Any(p => p.p70ID == BO.p70IdENUM.Vyfakturovano && p.p32ManualFeeFlag==0 && (p.p33ID==BO.p33IdENUM.Cas || p.p33ID==BO.p33IdENUM.Kusovnik) && (p.InvoiceRate==0 || p.InvoiceValue==0)))
            {
                this.AddMessage("Na vstupu je minimálně jeden časový úkon pro fakturaci s nulovou sazbou nebo nulovým počtem hodin."); return false;
            }
            if (lisP31.Any(p => p.p70ID == BO.p70IdENUM.Vyfakturovano && p.p32ManualFeeFlag == 1 && p.p33ID == BO.p33IdENUM.Cas && (p.ManualFee == 0 || p.InvoiceValue == 0)))
            {
                this.AddMessage("Na vstupu je minimálně jeden časový úkon pro fakturaci s nulovým pevným honorářem nebo s nulovým počtem hodin."); return false;
            }
            if (lisP31.Any(p => p.p70ID == BO.p70IdENUM.Vyfakturovano && (p.p33ID == BO.p33IdENUM.PenizeBezDPH || p.p33ID==BO.p33IdENUM.PenizeVcDPHRozpisu) && p.InvoiceValue == 0))
            {
                this.AddMessage("Na vstupu je minimálně jeden peněžní úkon pro fakturaci s nulovou částkou."); return false;
            }
            var recP91 = _mother.p91InvoiceBL.Load(p91id);

            foreach(var c in lisP31.Where(p => p.p70ID==BO.p70IdENUM.Vyfakturovano && p.InvoiceVatRate > 0))
            {
                var recP31 = Load(c.p31ID);
                if (!ValidateVatRate(c.InvoiceVatRate, recP31.p41ID, recP91.p91DateSupply, recP91.j27ID))
                {
                    this.AddMessageTranslated(string.Format("DPH sazba {0}% není platná pro projekt ({1}), datum ({2}) a měnu ({3}).",c.InvoiceVatRate,recP31.p41Name,recP91.p91DateSupply,recP91.j27Code));
                    return false;
                }
            }
            var guid = BO.BAS.GetGuid();
            foreach(var c in lisP31)
            {
                var ctemp = new BO.p85Tempbox() {
                    p85GUID = guid
                    ,p85Prefix = "p31"
                    ,p85DataPID = c.p31ID
                    ,p85OtherKey1 = (int) c.p70ID
                    ,p85Message=c.TextUpdate
                    ,p85FreeFloat01=c.InvoiceValue
                    ,p85FreeFloat02=c.InvoiceRate
                    ,p85FreeFloat03=c.InvoiceVatRate
                    ,p85FreeNumber01 = c.FixPriceValue* 10000000
                    ,p85FreeBoolean01 = c.p31IsInvoiceManual
                    ,p85FreeNumber02 = c.ManualFee* 10000000
                    ,p85FreeText09=c.p31Code
                };
                _mother.p85TempboxBL.Save(ctemp);

                var pars = new Dapper.DynamicParameters();
                pars.Add("p91id", p91id, System.Data.DbType.Int32);
                pars.Add("guid", guid, System.Data.DbType.String);
                pars.Add("j03id_sys", _db.CurrentUser.pid, System.Data.DbType.Int32);                
                pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

                if (_db.RunSp("p31_change_invoice", ref pars,true) != "1")               
                {
                    return false;
                }
                if (lisFFI != null)
                {
                    DL.BAS.SaveFreeFields(_db, c.p31ID, lisFFI);
                }
                
            }

            

            return true;
        }

        public bool RemoveFromApprove(List<int> p31ids)
        {
            if (p31ids == null || p31ids.Count() == 0)
            {
                this.AddMessage("Na vstupu chybí úkon."); return false;
            }
            var guid = BO.BAS.GetGuid();
            _db.RunSql("INSERT INTO p85TempBox(p85GUID,p85Prefix,p85DataPID) SELECT @guid,'p31',p31ID FROM p31Worksheet WHERE p31ID IN (" + string.Join(",", p31ids) + ")",new { guid = guid });

            var pars = new Dapper.DynamicParameters();            
            pars.Add("guid", guid, System.Data.DbType.String);
            pars.Add("j03id_sys", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            if (_db.RunSp("p31_remove_approve", ref pars, true) != "1")
            {
                return false;
            }
            return true;
        }

        public bool RemoveFromInvoice(int p91id, List<int> p31ids)
        {
            if (p31ids==null || p31ids.Count() == 0)
            {
                this.AddMessage("Na vstupu chybí úkon."); return false;
            }
            if (p91id == 0)
            {
                this.AddMessageTranslated("p91id missing");return false;
            }
            var lis = GetList(new BO.myQueryP31() { p91id = p91id });
            if (lis.Count() <= p31ids.Count())
            {
                this.AddMessage("Vyúčtování musí obsahovat minimálně jednu položku. Nemůžete vyjmout všechny úkony z vyúčtování.");return false;
            }

            var guid = BO.BAS.GetGuid();
            _db.RunSql("INSERT INTO p85TempBox(p85GUID,p85Prefix,p85DataPID) SELECT @guid,'p31',p31ID FROM p31Worksheet WHERE p31ID IN (" + string.Join(",", p31ids)+")",new { guid = guid });

            var pars = new Dapper.DynamicParameters();
            pars.Add("p91id", p91id, System.Data.DbType.Int32);
            pars.Add("guid", guid, System.Data.DbType.String);
            pars.Add("j03id_sys", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            if (_db.RunSp("p31_remove_invoice", ref pars, true) != "1")
            {
                return false;
            }
            return true;
        }
        public bool Move2Invoice(int p91id_dest, int p31id)
        {
            if (p31id==0)
            {
                this.AddMessage("Na vstupu chybí úkon."); return false;
            }
            if (p91id_dest == 0)
            {
                this.AddMessageTranslated("Chybí cílové vyúčtování (faktura)."); return false;
            }
            
            var pars = new Dapper.DynamicParameters();
            pars.Add("p31id", p31id, System.Data.DbType.Int32);
            pars.Add("p91id_dest", p91id_dest, System.Data.DbType.Int32);
            pars.Add("j03id_sys", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            if (_db.RunSp("p31_move_to_another_invoice", ref pars, true) != "1")
            {
                return false;
            }
            return true;
        }
        public bool Move2Bin(bool move2bin,List<int> p31ids)
        {
            if (move2bin)
            {
                return _db.RunSql("UPDATE p31Worksheet SET p31ValidUntil=getdate() WHERE p31ID IN (" + string.Join(",", p31ids) + ")");
            }
            else
            {
                return _db.RunSql("UPDATE p31Worksheet SET p31ValidUntil=convert(datetime,'01.01.3000',104) WHERE p31ID IN (" + string.Join(",", p31ids) + ")");
            }
            
        }

        public BO.p31RecDisposition InhaleRecDisposition(BO.p31Worksheet rec)
        {
            var c = new BO.p31RecDisposition();            

            var pars = new Dapper.DynamicParameters();                        
            pars.Add("j03id_sys", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("pid", rec.pid, System.Data.DbType.Int32);
            pars.Add("record_disposition",null,System.Data.DbType.Int32,System.Data.ParameterDirection.Output);
            pars.Add("record_state", null, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            pars.Add("msg_locked", null, System.Data.DbType.String, System.Data.ParameterDirection.Output,1000);

            if (_db.RunSp("p31_inhale_disposition", ref pars,false) == "1")
            {
                c.RecordState = (BO.p31RecordState) pars.Get<Int32>("record_state");
                c.LockedReasonMessage = pars.Get<string>("msg_locked");
                switch (pars.Get<Int32>("record_disposition"))
                {
                    case 1:
                        c.ReadAccess = true;break;
                    case 2:
                        c.OwnerAccess = true;c.ReadAccess = true; break;
                    case 3:
                        c.CanApprove = true; c.ReadAccess = true; break;
                    case 4:
                        c.CanApproveAndEdit = true; c.ReadAccess = true; c.OwnerAccess = true; break;
                }
                
            }
            if (_mother.CurrentUser.IsAdmin)
            {
                c.OwnerAccess = true; c.ReadAccess = true;
            }

            if (c.RecordState==BO.p31RecordState.Editing && !c.OwnerAccess){
                c.OwnerAccess = _mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P31_Owner); //uživatel má právo vlastníka všech worksheet záznamů v db
            }
            



            return c;
        }

        public bool ValidateVatRate(double vatrate,int p41id,DateTime d,int j27id)
        {
            var ret =_db.Load<BO.GetBool>("select dbo.p31_testvat(@vatrate,@p41id,@dat,@j27id) as Value", new { vatrate = vatrate, p41id = p41id, dat = d, j27id = j27id });
            return ret.Value;
        }

        public IEnumerable<BO.p31WorksheetTimelineDay> GetList_TimelineDays(List<int>j02ids,DateTime d1,DateTime d2,int j70id)
        {
            if (j02ids == null || j02ids.Count() == 0) j02ids = new List<int>() { _mother.CurrentUser.j02ID };
            sb("SELECT a.j02ID,min(j02.j02LastName+' '+j02.j02FirstName) as Person");
            sb(",a.p31Date,sum(a.p31Hours_Orig) as Hours,sum(case when p32.p32IsBillable=1 then a.p31Hours_Orig end) as Hours_Billable,sum(case when p32.p32IsBillable=0 then a.p31Hours_Orig end) as Hours_NonBillable,count(case when p34x.p33id in (2,5) then 1 end) as Moneys,count(case when p34x.p33id=3 then 1 end) as Pieces");
            sb(",convert(varchar(10),a.p31Date,104) as p31DateString");
            sb(" FROM p31Worksheet a");
            sb(" INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID");
            sb(" INNER JOIN p34ActivityGroup p34x ON p32.p34ID=p34x.p34ID INNER JOIN p41Project p41x ON a.p41ID=p41x.p41ID");
            

            sb(" WHERE a.p31Date BETWEEN @d1 AND @d2 AND a.p31ValidUntil>GETDATE() AND GETDATE() BETWEEN j02.j02ValidFrom AND j02.j02ValidUntil");
            sb(" AND a.j02ID IN (" + string.Join(",", j02ids) + ")");
            
            sb(" GROUP BY a.j02ID, a.p31Date");
            sb(" ORDER BY min(j02.j02LastName),min(j02.j02FirstName)");

            return _db.GetList<BO.p31WorksheetTimelineDay>(sbret(), new { d1 = d1, d2 = d2 });
        }

        public BO.p31WorksheetEntryInput CovertRec2Input(BO.p31Worksheet rec,bool time_entry_by_minutes)
        {
            if (rec == null) return null;           

            var c = new BO.p31WorksheetEntryInput() { pid = rec.pid, j02ID = rec.j02ID, j02ID_ContactPerson = rec.j02ID_ContactPerson,p41ID=rec.p41ID,p34ID=rec.p34ID,p32ID=rec.p32ID,p56ID=rec.p56ID };
            c.p31Date = rec.p31Date; c.p31Text = rec.p31Text;
            c.Value_Orig = rec.p31Value_Orig.ToString();c.p31HoursEntryflag = rec.p31HoursEntryFlag;
            if (time_entry_by_minutes && (c.p31HoursEntryflag==BO.p31HoursEntryFlagENUM.Hodiny || c.p31HoursEntryflag == BO.p31HoursEntryFlagENUM.Minuty))
            {
                c.Value_Orig = rec.p31Minutes_Orig.ToString();  //hodiny zobrazovat v minutách
            }
            c.TimeFrom = rec.TimeFrom; c.TimeUntil = rec.TimeUntil;
            
            c.j27ID_Billing_Orig = rec.j27ID_Billing_Orig;
            c.Amount_Vat_Orig = rec.p31Amount_Vat_Orig;c.Amount_WithoutVat_Orig = rec.p31Amount_WithoutVat_Orig;c.Amount_WithVat_Orig = rec.p31Amount_WithVat_Orig;
            c.VatRate_Orig = rec.p31VatRate_Orig;
            c.Value_OffBilling = rec.p31Value_Off;
            c.ValidUntil = rec.ValidUntil;c.ValidFrom = rec.ValidFrom;
            c.p35ID = rec.p35ID;

            if (rec.p33ID==BO.p33IdENUM.PenizeBezDPH || rec.p33ID == BO.p33IdENUM.PenizeVcDPHRozpisu)
            {
                c.p31Code = rec.p31Code;
                c.j19ID = rec.j19ID;
                c.p31MarginHidden = rec.p31MarginHidden; c.p31MarginTransparent = rec.p31MarginTransparent;
                c.p28ID_Supplier = rec.p28ID_Supplier;
                c.p31PostCode = rec.p31PostCode;
                c.p31PostRecipient = rec.p31PostRecipient;
                c.p31PostFlag = rec.p31PostFlag;
                c.p31Calc_PieceAmount = rec.p31Calc_PieceAmount;
                c.p31Calc_Pieces = rec.p31Calc_Pieces;
                
            }

            c.p72ID_AfterTrimming = rec.p72ID_AfterTrimming;
            if (rec.p72ID_AfterTrimming != BO.p72IdENUM._NotSpecified)
            {
                c.Value_Trimmed = rec.p31Value_Trimmed.ToString();                        
                if (rec.IsRecommendedHHMM_Trimmed())
                {
                    c.p31HHMM_Trimmed = BO.basTime.ShowAsHHMM(c.Value_Trimmed);
                    c.Value_Trimmed = c.p31HHMM_Trimmed;
                }
            }

            return c;

        }


        public BO.p31Rate LoadRate(BO.p51TypeFlagENUM flag,DateTime d,int j02id,int p41id, int p32id)
        {
            var ret = new BO.p31Rate() { flag = flag };

            var pars = new Dapper.DynamicParameters();
            pars.Add("date_rate", d, System.Data.DbType.DateTime);
            pars.Add("p51TypeFlag", (int)flag, System.Data.DbType.Int32);
            pars.Add("p41id", p41id, System.Data.DbType.Int32);
            pars.Add("j02id", j02id, System.Data.DbType.Int32);
            pars.Add("p32id", p32id, System.Data.DbType.Int32);
            pars.Add("p51id", null, System.Data.DbType.Int32);
            pars.Add("ret_j27id", null, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            pars.Add("ret_rate", null, System.Data.DbType.Double, System.Data.ParameterDirection.Output);

            if (_db.RunSp("p31_getrate_tu", ref pars, false) == "1")
            {
                ret.j27ID= pars.Get<int>("ret_j27id");
                ret.Value = pars.Get<double>("ret_rate");
                if (ret.j27ID > 0) ret.j27Code = _mother.FBL.LoadCurrencyByID(ret.j27ID).j27Code;
            }

            return ret;
        }

        public BO.p72IdENUM Get_p72ID_NonBillableWork(int p31id)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("p31id", p31id, System.Data.DbType.Int32);
            pars.Add("ret_p72id", null, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);

            if (_db.RunSp("p31_inhale_p72id_nonbillable", ref pars, false) == "1")
            {
                return (BO.p72IdENUM) pars.Get<int>("ret_p72id");

            }
            else
            {
                return BO.p72IdENUM.SkrytyOdpis;
            }            
        }

        public bool Validate_Before_Save_Approving(BO.p31WorksheetApproveInput c, bool istempdata)
        {
            if (c.p71id == BO.p71IdENUM.Nic) return true;   //vrátit do rozpracovanosti
            if (c.p71id == BO.p71IdENUM.Neschvaleno) return true;   //nahozeno jako neschváleno
            if (istempdata && string.IsNullOrEmpty(c.Guid))
            {
                this.AddMessage("Pro temp data musí být předán GUID_TempData.");return false;
            }
            if (!istempdata && !string.IsNullOrEmpty(c.Guid))
            {
                this.AddMessage("Je předáván GUID_TempData, ale bolTempData=false."); return false;
            }
            if (c.p71id==BO.p71IdENUM.Schvaleno && c.p72id == BO.p72IdENUM._NotSpecified)
            {
                this.AddMessage("Schválený úkon musí mít přiřazen některý z fakturačních statusů.");return false;
            }

            if (c.p72id==BO.p72IdENUM.Fakturovat || c.p72id == BO.p72IdENUM.FakturovatPozdeji)
            {
                if (c.p33ID==BO.p33IdENUM.Cas && c.Value_Approved_Billing==0)
                {
                    this.AddMessage("Hodiny k fakturaci nesmí být NULA."); return false;
                }
                if (c.p33ID == BO.p33IdENUM.Kusovnik && c.Value_Approved_Billing == 0)
                {
                    this.AddMessage("Počet kusů k fakturaci nesmí být NULA."); return false;
                }
                if ((c.p33ID == BO.p33IdENUM.Cas || c.p33ID==BO.p33IdENUM.Kusovnik) && c.Rate_Billing_Approved == 0)
                {
                    this.AddMessage("Sazba k fakturaci nesmí být NULA."); return false;
                }
                if ((c.p33ID == BO.p33IdENUM.PenizeBezDPH || c.p33ID==BO.p33IdENUM.PenizeVcDPHRozpisu) && c.Value_Approved_Billing == 0)
                {
                    this.AddMessage("Částka bez DPH nesmí být NULA."); return false;
                }
                
            }

            return true;
        }

        public void DeleteTempRecord(string guid,int p31id)
        {
            _db.RunSql("DELETE FROM p31Worksheet_Temp where p31GUID=@guid AND p31ID=@pid", new { guid = guid, pid = p31id });
        }

        public bool Save_Approving(BO.p31WorksheetApproveInput c,bool istempdata)
        {
            if (!Validate_Before_Save_Approving(c, istempdata)) return false;

            var pars = new Dapper.DynamicParameters();
            if (!string.IsNullOrEmpty(c.Guid))
            {
                pars.Add("guid", c.Guid, System.Data.DbType.String);
            }
            pars.Add("p31id", c.p31ID, System.Data.DbType.Int32);
            pars.Add("j03id_sys", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("p71id", (int)c.p71id, System.Data.DbType.Int32);
            pars.Add("p72id", (int)c.p72id, System.Data.DbType.Int32);
            pars.Add("approvingset", null, System.Data.DbType.String);
            pars.Add("value_approved_internal", c.Value_Approved_Internal, System.Data.DbType.Double);
            pars.Add("value_approved_billing", c.Value_Approved_Billing, System.Data.DbType.Double);
            pars.Add("rate_billing_approved", c.Rate_Billing_Approved, System.Data.DbType.Double);
            pars.Add("rate_internal_approved", c.Rate_Internal_Approved, System.Data.DbType.Double);
            pars.Add("p31Text", c.p31Text, System.Data.DbType.String);
            pars.Add("vatrate_approved", c.VatRate_Approved, System.Data.DbType.Double);
            pars.Add("dat_p31date", c.p31Date, System.Data.DbType.DateTime);
            pars.Add("approving_level", c.p31ApprovingLevel, System.Data.DbType.Int32);
            pars.Add("value_fixprice", c.p31Value_FixPrice, System.Data.DbType.Double);
            pars.Add("manualfee_approved", c.ManualFee_Approved, System.Data.DbType.Double);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            if (!string.IsNullOrEmpty(c.Guid))
            {
                //TEMP - dočasná data
                if (_db.RunSp("p31_save_approving_temp", ref pars, true) == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //uložení schvalování do ostrých dat
                if (_db.RunSp("p31_save_approving", ref pars, true) == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
        }

    }
}
