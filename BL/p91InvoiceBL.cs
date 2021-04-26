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
        public BO.p91Invoice LoadByCode(string code);
        public BO.p91Invoice LoadMyLastCreated();
        public BO.p91Invoice LoadCreditNote(int p91id);
        public BO.p91Invoice LoadLastOfClient(int p28id);
        public BO.p91RecDisposition InhaleRecDisposition(BO.p91Invoice rec);
        public IEnumerable<BO.p91Invoice> GetList(BO.myQueryP91 mq);
        public int Update(BO.p91Invoice rec, List<BO.FreeFieldInput> lisFFI);
        public int Create(BO.p91Create rec);
        public bool ChangeVat(int p91id, int x15id, double newvatrate);        
        public int CreateCreditNote(int p91id, int p92id_creditnote, bool bolJistota);
        public bool ChangeCurrency(int p91id, int j27id);
        public bool ConvertFromDraft(int p91id);
        public bool SaveP99(int p91id, int p90id, int p82id, double percentage);
        public bool DeleteP99(int p99id);
        public IEnumerable<BO.p99Invoice_Proforma> GetList_p99(int p90id, int p91id, int p82id);
        public bool RecalcFPR(DateTime d1, DateTime d2, int p51id = 0);
        public int SaveP94(BO.p94Invoice_Payment rec);
        public IEnumerable<BO.p94Invoice_Payment> GetList_p94(int p91id);
        public bool DeleteP94(int p94id, int p91id);
        public void ClearExchangeDate(int p91id, bool recalc);
        public IEnumerable<BO.p91_CenovyRozpis> GetList_CenovyRozpis(int p91id, bool bolIncludeRounding, bool bolIncludeProforma, int langindex);
        public bool Delete(int p91id, string guid, int selectedoper);

    }
    class p91InvoiceBL : BaseBL, Ip91InvoiceBL
    {
        public p91InvoiceBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null, int toprec = 0)
        {
            if (toprec == 0)
            {
                sb("SELECT ");
            }
            else
            {
                sb($"SELECT TOP {toprec} ");
            }            
            sb(_db.GetSQL1_Ocas("p91"));
            sb(",a.p92ID,a.p28ID,a.j27ID,a.j19ID,a.j02ID_Owner,a.p41ID_First,a.p91ID_CreditNoteBind,a.j17ID,a.p98ID,a.p63ID,a.p80ID,a.o38ID_Primary,a.o38ID_Delivery,a.x15ID,a.b02ID,a.j02ID_ContactPerson,a.p91FixedVatRate,a.p91Code,a.p91IsDraft,a.p91Date,a.p91DateBilled,a.p91DateMaturity,a.p91DateSupply,a.p91DateExchange,a.p91ExchangeRate");
            
            sb(",a.p91Datep31_From,a.p91Datep31_Until,a.p91Amount_WithoutVat,a.p91Amount_Vat,a.p91Amount_Billed,a.p91Amount_WithVat,a.p91Amount_Debt,a.p91RoundFitAmount,a.p91Text1,a.p91Text2,a.p91ProformaAmount,a.p91ProformaBilledAmount,a.p91Amount_WithoutVat_None,a.p91VatRate_Low,a.p91Amount_WithVat_Low,a.p91Amount_WithoutVat_Low,a.p91Amount_Vat_Low");
            sb(",a.p91VatRate_Standard,a.p91Amount_WithVat_Standard,a.p91Amount_WithoutVat_Standard,a.p91Amount_Vat_Standard,a.p91VatRate_Special,a.p91Amount_WithVat_Special,a.p91Amount_WithoutVat_Special,a.p91Amount_Vat_Special,a.p91Amount_TotalDue");            
            sb(",p28client.p28Name,p92.p92Name,p92.p93ID,isnull(p41.p41NameShort,p41.p41Name) as p41Name,b02.b02Name,j02owner.j02LastName+' '+j02owner.j02FirstName as Owner,j17.j17Name,j27.j27Code,p92.p92InvoiceType,p92.b01ID,p28client.p28CompanyName");
            sb(",a.p91Client,a.p91ClientPerson,a.p91ClientPerson_Salutation,a.p91Client_RegID,a.p91Client_VatID,a.p91ClientAddress1_Street,a.p91ClientAddress1_City,a.p91ClientAddress1_ZIP,a.p91ClientAddress1_Country,a.p91ClientAddress2,a.p91LockFlag,a.p91Client_ICDPH_SK");
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
        public BO.p91Invoice LoadByCode(string code)
        {
            return _db.Load<BO.p91Invoice>(GetSQL1(" WHERE a.p91Code LIKE @code"), new { code = code });
        }
        public BO.p91Invoice LoadMyLastCreated()
        {
            return _db.Load<BO.p91Invoice>(GetSQL1(" WHERE a.j02ID_Owner=@j02id_owner ORDER BY a.p91ID DESC",1), new { j02id_owner = _mother.CurrentUser.j02ID });
        }
        public BO.p91Invoice LoadCreditNote(int p91id)
        {
            sb(GetSQL1());
            sb(" INNER JOIN p91Invoice sourcedoc ON a.p91ID_CreditNoteBind=sourcedoc.p91ID");
            sb(" WHERE sourcedoc.p91ID=@p91id");
            return _db.Load<BO.p91Invoice>(sbret(),new { p91id = p91id });
        }
        public BO.p91Invoice LoadLastOfClient(int p28id)
        {
            return _db.Load<BO.p91Invoice>(GetSQL1(" WHERE a.p28ID=@p28id AND a.p91Amount_WithoutVat>0 ORDER BY a.p91ID DESC", 1), new { p28id = p28id });
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

        public bool Delete(int p91id, string guid,int selectedoper)
        {
            if (p91id==0 || string.IsNullOrEmpty(guid) || selectedoper==0)
            {
                this.AddMessageTranslated("p91id or guid or selectedoper missing");return false;
            }
            using (var sc = new System.Transactions.TransactionScope())
            {
                _db.RunSql("DELETE FROM p85TempBox WHERE p85GUID=@guid AND p85Prefix='p31'", new { guid = guid });
                _db.RunSql("INSERT INTO p85TempBox(p85GUID,p85Prefix,p85DataPID,p85OtherKey1) SELECT @guid,'p31',p31ID,@oper FROM p31Worksheet WHERE p91ID=@p91id", new { guid = guid, oper = selectedoper,p91id=p91id });

                if (_db.RunSql("exec dbo.p91_delete @j03id_sys,@pid,@guid,@err_ret OUTPUT", new { j03id_sys = _mother.CurrentUser.pid, pid = p91id, guid = guid, err_ret = "" }))
                {
                    sc.Complete();
                    return true;
                }
                else
                {
                    return false;
                }
            }
               
        }

        public int Update(BO.p91Invoice rec, List<BO.FreeFieldInput> lisFFI)
        {
            if (!ValidateBeforeUpdate(rec))
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
        private bool ValidateBeforeUpdate(BO.p91Invoice rec)
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

        public bool ChangeVat(int p91id,int x15id,double newvatrate)
        {
            using (var sc = new System.Transactions.TransactionScope())
            {
                var pars = new Dapper.DynamicParameters();

                pars.Add("p91id", p91id, DbType.Int32);
                pars.Add("j03id_sys", _mother.CurrentUser.pid, DbType.Int32);
                pars.Add("x15id",x15id, DbType.Int32);
                pars.Add("newvatrate", newvatrate, DbType.Double);                
                pars.Add("err_ret", null, DbType.String, ParameterDirection.Output, 1000);

                if (_db.RunSp("p91_change_vat", ref pars) == "1")
                {
                    sc.Complete();

                    return true;
                }
                else
                {
                    return false;
                }



            }
        }


        private void Handle_RecalcAmount(int p91id)
        {
            _db.RunSql("exec dbo.p91_recalc_amount @p91id", new { p91id = p91id });
        }


        
        public int CreateCreditNote(int p91id,int p92id_creditnote,bool bolJistota)
        {
            string strSP = "p91_create_creditnote";
            if (bolJistota) strSP = "p91_create_creditnote_jistota";

            using (var sc = new System.Transactions.TransactionScope())
            {
                var pars = new Dapper.DynamicParameters();

                pars.Add("p91id_bind", p91id, DbType.Int32);
                pars.Add("j03id_sys", _mother.CurrentUser.pid, DbType.Int32);
                pars.Add("p92id_creditnote", p92id_creditnote, DbType.Int32);
                pars.Add("ret_p91id", 0, DbType.Int32, ParameterDirection.Output);
                pars.Add("err_ret", null, DbType.String, ParameterDirection.Output, 1000);

                if (_db.RunSp(strSP, ref pars) == "1")
                {
                    sc.Complete();

                    return pars.Get<int>("ret_p91id");
                }
                else
                {
                    return 0;
                }



            }
        }

        public bool ChangeCurrency(int p91id,int j27id)
        {
            using (var sc = new System.Transactions.TransactionScope())
            {
                var pars = new Dapper.DynamicParameters();

                pars.Add("p91id", p91id, DbType.Int32);
                pars.Add("j03id_sys", _mother.CurrentUser.pid, DbType.Int32);
                pars.Add("j27id", j27id, DbType.Int32);                
                pars.Add("err_ret", null, DbType.String, ParameterDirection.Output, 1000);

                if (_db.RunSp("p91_change_currency", ref pars) == "1")
                {
                    sc.Complete();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool ConvertFromDraft(int p91id)
        {
            using (var sc = new System.Transactions.TransactionScope())
            {
                var pars = new Dapper.DynamicParameters();
                pars.Add("p91id", p91id, DbType.Int32);
                pars.Add("j03id_sys", _mother.CurrentUser.pid, DbType.Int32);               
                pars.Add("err_ret", null, DbType.String, ParameterDirection.Output, 1000);

                if (_db.RunSp("p91_convertdraft", ref pars) == "1")
                {
                    sc.Complete();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool SaveP99(int p91id, int p90id,int p82id,double percentage)
        {
            using (var sc = new System.Transactions.TransactionScope())
            {
                var pars = new Dapper.DynamicParameters();

                pars.Add("p91id", p91id, DbType.Int32);
                pars.Add("p90id", p90id, DbType.Int32);
                pars.Add("p82id", p82id, DbType.Int32);
                pars.Add("percentage", percentage, DbType.Double);
                pars.Add("j03id_sys", _mother.CurrentUser.pid, DbType.Int32);                
                pars.Add("err_ret", null, DbType.String, ParameterDirection.Output, 1000);

                if (_db.RunSp("p91_proforma_save", ref pars) == "1")
                {
                    sc.Complete();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool DeleteP99(int p99id)
        {
            using (var sc = new System.Transactions.TransactionScope())
            {
                var pars = new Dapper.DynamicParameters();

                pars.Add("p99id", p99id, DbType.Int32);                
                pars.Add("j03id_sys", _mother.CurrentUser.pid, DbType.Int32);
                pars.Add("err_ret", null, DbType.String, ParameterDirection.Output, 1000);

                if (_db.RunSp("p91_proforma_delete", ref pars) == "1")
                {
                    sc.Complete();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public IEnumerable<BO.p99Invoice_Proforma> GetList_p99(int p90id, int p91id, int p82id)
        {
            sb("SELECT a.*,p90.p90Code,p91.p91Code,p82.p82Code,p82.p90ID,");
            sb(_db.GetSQL1_Ocas("p99", false, false, true));
            sb(" FROM p99Invoice_Proforma a INNER JOIN p82Proforma_Payment p82 ON a.p82ID=p82.p82ID INNER JOIN p91Invoice p91 ON a.p91ID=p91.p91ID INNER JOIN p90Proforma p90 ON p82.p90ID=p90.p90ID");
            object pars = null;

            if (p90id > 0)
            {
                sb(" WHERE p82.p90ID=@p90id");
                pars = new { p90id = p90id };
            }
            if (p91id > 0)
            {
                sb(" WHERE a.p91ID=@p91id");
                pars = new { p91id = p91id };
            }
            if (p82id > 0)
            {
                sb(" WHERE a.p82ID=@p82id");
                pars = new { p82id = p82id };
            }
            return _db.GetList<BO.p99Invoice_Proforma>(sbret(), pars);
        }

        public bool RecalcFPR(DateTime d1,DateTime d2,int p51id = 0)
        {
            return _db.RunSql("exec dbo.p91_fpr_recalc_all_invoices @d1,@d2,@p51id", new { d1 = d1, d2 = d2, p51id = p51id });
        }


        public int SaveP94(BO.p94Invoice_Payment rec)
        {
            if (rec.p94Amount == 0)
            {
                this.AddMessage("Částka nesmí být nula.");return 0;
            }

            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddInt("p91ID", rec.p91ID,true);
            p.AddDateTime("p94Date", rec.p94Date);
            p.AddDouble("p94Amount", rec.p94Amount);
            p.AddString("p94Code", rec.p94Code);
            p.AddString("p94Description", rec.p94Description);
            
            int intPID = _db.SaveRecord("p94Invoice_Payment", p, rec,false,true);
            if (intPID > 0)
            {
                Handle_RecalcAmount(rec.p91ID);
            }

            return intPID;

        }

        public IEnumerable<BO.p94Invoice_Payment> GetList_p94(int p91id)
        {
            sb("select a.*,");
            sb(_db.GetSQL1_Ocas("p94",false,false,true));
            sb(" FROM p94Invoice_Payment a WHERE a.p91ID=@p91id ORDER BY a.p94Date DESC");

            return _db.GetList<BO.p94Invoice_Payment>(sbret(), new { p91id = p91id });
        }
        public bool DeleteP94(int p94id,int p91id)
        {
            if (_db.RunSql("DELETE FROM p94Invoice_Payment WHERE p94ID=@p94id", new { p94id = p94id }))
            {
                Handle_RecalcAmount(p91id);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ClearExchangeDate(int p91id,bool recalc)
        {
            _db.RunSql("update p91Invoice set p91DateExchange=null,p91ExchangeRate=null WHERE p91ID=@p91id", new { p91id = p91id });
            if (recalc)
            {
                Handle_RecalcAmount(p91id);
            }
        }

        public IEnumerable<BO.p91_CenovyRozpis> GetList_CenovyRozpis(int p91id, bool bolIncludeRounding, bool bolIncludeProforma, int langindex)
        {

            return _db.GetList<BO.p91_CenovyRozpis>("exec dbo.p91_get_cenovy_rozpis @pid,@include_rounding,@include_proforma,@langindex", new { pid = p91id, include_rounding=bolIncludeRounding, include_proforma=bolIncludeProforma, langindex=langindex });
            
        }


        public BO.p91RecDisposition InhaleRecDisposition(BO.p91Invoice rec)
        {
            var c = new BO.p91RecDisposition();
            if (_mother.CurrentUser.IsAdmin)
            {
                c.OwnerAccess = true; c.ReadAccess = true;
                return c;
            }
            if (rec.j02ID_Owner == _mother.CurrentUser.j02ID || _mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P91_Owner)) //je vlastník nebo má globální roli vlastit všechny faktury
            {
                c.OwnerAccess = !BO.BAS.bit_compare_or(rec.p91LockFlag, 8);c.ReadAccess = true;
                return c;
            }           
            
            if (_mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P91_Reader))
            {
                c.ReadAccess = true;
                return c;
            }

            return c;
        }



    }
}
