using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip28ContactBL
    {
        public BO.p28Contact Load(int pid);
        public IEnumerable<BO.p28Contact> GetList(BO.myQueryP28 mq);
        public IEnumerable<BO.o32Contact_Medium> GetList_o32(int p28id);
        public int Save(BO.p28Contact rec, List<BO.o37Contact_Address> lisO37, List<BO.o32Contact_Medium> lisO32, List<BO.FreeFieldInput> lisFFI,List<int> j02ids);
        public IEnumerable<BO.o37Contact_Address> GetList_o37(int p28id);
        public BO.p28RecDisposition InhaleRecDisposition(BO.p28Contact rec);
        public BO.p28ContactSum LoadSumRow(int pid);
    }
    class p28ContactBL : BaseBL, Ip28ContactBL
    {

        public p28ContactBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.p29ID,a.p92ID,a.j02ID_Owner,a.p87ID,a.p51ID_Billing,a.p51ID_Internal,a.b02ID,a.p63ID,a.p28IsCompany,a.p28IsDraft,a.p28Code,a.p28FirstName,a.p28LastName,a.p28TitleBeforeName,a.p28TitleAfterName,a.p28RegID,a.p28VatID,a.p28Person_BirthRegID,a.p28CompanyName,a.p28CompanyShortName,a.p28InvoiceDefaultText1,a.p28InvoiceDefaultText2,a.p28InvoiceMaturityDays,a.p28LimitHours_Notification,a.p28LimitFee_Notification");
            sb(",a.p28Name,p29.p29Name,p92.p92Name,b02.b02Name,p87.p87Name,a.p28RobotAddress,a.p28SupplierID,a.p28SupplierFlag,a.p28ExternalPID,a.p28Round2Minutes,a.p28ICDPH_SK");
            sb(",a.p28TreeLevel,a.p28TreeIndex,a.p28TreePrev,a.p28TreeNext,a.p28TreePath");
            sb(",p51billing.p51Name as p51Name_Billing,j02owner.j02LastName+' '+j02owner.j02FirstName as Owner,a.p28ParentID,a.p28BillingMemo,a.p28Pohoda_VatCode,a.j02ID_ContactPerson_DefaultInWorksheet,a.j02ID_ContactPerson_DefaultInInvoice,a.j61ID_Invoice");
            sb(",p29.x38ID");
            sb(","+_db.GetSQL1_Ocas("p28") + " FROM p28Contact a");
            sb(" LEFT OUTER JOIN p29ContactType p29 ON a.p29ID=p29.p29ID");
            sb(" LEFT OUTER JOIN p87BillingLanguage p87 ON a.p87ID=p87.p87ID");
            sb(" LEFT OUTER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID");
            sb(" LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID");
            sb(" LEFT OUTER JOIN p51PriceList p51billing ON a.p51ID_Billing=p51billing.p51ID");
            sb(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID");
            //sb(" LEFT OUTER JOIN p28Contact_FreeField p28free ON a.p28ID=p28free.p28ID");
            sb(strAppend);
            return sbret();
        }

        public BO.p28Contact Load(int pid)
        {
            return _db.Load<BO.p28Contact>(GetSQL1(" WHERE a.p28ID=@pid"), new { pid = pid });
        }
        public IEnumerable<BO.p28Contact> GetList(BO.myQueryP28 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p28Contact>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.o32Contact_Medium> GetList_o32(int p28id)
        {            
            return _db.GetList<BO.o32Contact_Medium>("select a.*,"+ _db.GetSQL1_Ocas("o32", false, false)+", o33.o33Name FROM o32Contact_Medium a INNER JOIN o33MediumType o33 ON a.o33ID=o33.o33ID WHERE a.p28ID=@p28id",new { p28id = p28id });
        }
        public IEnumerable<BO.o37Contact_Address> GetList_o37(int p28id)
        {
            sb("SELECT a.o36ID,a.p28ID,o38.*,");
            sb(_db.GetSQL1_Ocas("o37",false,false,false));
            sb(" FROM o37Contact_Address a INNER JOIN o38Address o38 ON a.o38ID=o38.o38ID");
            sb(" WHERE a.p28ID=@p28id");

            return _db.GetList<BO.o37Contact_Address>(sbret(), new { p28id = p28id });
        }
        public int Save(BO.p28Contact rec,List<BO.o37Contact_Address> lisO37,List<BO.o32Contact_Medium> lisO32, List<BO.FreeFieldInput> lisFFI, List<int> j02ids)
        {
            if (!ValidateBeforeSave(rec, lisO37,lisO32))
            {
                return 0;
            }

            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();
               
                p.AddInt("pid", rec.pid);
                if (rec.p28Code == null)
                {
                    rec.p28Code = "TEMP" + BO.BAS.GetGuid();
                    p.AddString("p28Code", rec.p28Code);
                }
                p.AddInt("p29ID", rec.p29ID, true);
                p.AddInt("p92ID", rec.p92ID, true);
                p.AddInt("p87ID", rec.p87ID, true);
                p.AddInt("p51ID_Billing", rec.p51ID_Billing, true);
                p.AddInt("p51ID_Internal", rec.p51ID_Internal, true);
                p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
                p.AddInt("j02ID_ContactPerson_DefaultInWorksheet", rec.j02ID_ContactPerson_DefaultInWorksheet, true);
                p.AddInt("j02ID_ContactPerson_DefaultInInvoice", rec.j02ID_ContactPerson_DefaultInInvoice, true);
                p.AddInt("p63ID", rec.p63ID, true);
                p.AddInt("p28ParentID", rec.p28ParentID, true);
                p.AddInt("j61ID_Invoice", rec.j61ID_Invoice, true);

                p.AddEnumInt("p28SupplierFlag", rec.p28SupplierFlag);
                p.AddInt("j61ID_Invoice", rec.j61ID_Invoice, true);

                p.AddBool("p28IsCompany", rec.p28IsCompany);
                p.AddString("p28CompanyName", rec.p28CompanyName);
                p.AddString("p28CompanyShortName", rec.p28CompanyShortName);                
                p.AddString("p28FirstName", rec.p28FirstName);
                p.AddString("p28LastName", rec.p28LastName);
                p.AddString("p28TitleBeforeName", rec.p28TitleBeforeName);
                p.AddString("p28TitleAfterName", rec.p28TitleAfterName);

                p.AddString("p28RegID", rec.p28RegID);
                p.AddString("p28VatID", rec.p28VatID);
                p.AddString("p28RegID", rec.p28RegID);
                p.AddString("p28ICDPH_SK", rec.p28ICDPH_SK);
                p.AddString("p28Person_BirthRegID", rec.p28Person_BirthRegID);
                p.AddString("p28ExternalPID", rec.p28ExternalPID);                
                p.AddString("p28SupplierID", rec.p28SupplierID);

                p.AddString("p28BillingMemo", rec.p28BillingMemo);
                p.AddString("p28InvoiceDefaultText1", rec.p28InvoiceDefaultText1);
                p.AddString("p28InvoiceDefaultText2", rec.p28InvoiceDefaultText2);
                p.AddString("p28Pohoda_VatCode", rec.p28Pohoda_VatCode);
                p.AddString("p28Pohoda_VatCode", rec.p28Pohoda_VatCode);
                p.AddString("p28Pohoda_VatCode", rec.p28Pohoda_VatCode);
                p.AddString("p28Pohoda_VatCode", rec.p28Pohoda_VatCode);

                p.AddInt("p28InvoiceMaturityDays", rec.p28InvoiceMaturityDays);
                p.AddInt("p28Round2Minutes", rec.p28Round2Minutes);
                p.AddDouble("p28LimitHours_Notification", rec.p28LimitHours_Notification);
                p.AddDouble("p28LimitFee_Notification", rec.p28LimitFee_Notification);


                int intPID = _db.SaveRecord("p28Contact", p, rec);

                if (intPID > 0)
                {
                    if (!DL.BAS.SaveFreeFields(_db, intPID, lisFFI))
                    {
                        return 0;
                    }                    

                    if (lisO37 != null)
                    {
                        foreach(var adr in lisO37)
                        {
                            if (adr.IsSetAsDeleted)
                            {
                                if (adr.pid > 0)
                                {
                                    _db.RunSql("DELETE FROM o37Contact_Address WHERE o37ID=@pid", new { pid = adr.pid });
                                    _db.RunSql("DELETE FROM o38Address WHERE o38ID=@pid", new { pid = adr.o38ID });
                                }
                            }
                            else
                            {
                                var recO38 = new BO.o38Address();
                                if (adr.pid > 0)
                                {                                    
                                    recO38 = _mother.o38AddressBL.Load(adr.o38ID);                                                                        
                                }
                                recO38.o38City = adr.o38City;recO38.o38Street = adr.o38Street;recO38.o38ZIP = adr.o38ZIP;recO38.o38Country = adr.o38Country;recO38.o38Name = adr.o38Name;
                                int intO38ID=_mother.o38AddressBL.Save(recO38, intPID, (int)adr.o36ID);
                                if (intO38ID == 0)
                                {
                                    return 0;
                                }
                            }
                        }                                              
                    }

                    if (lisO32 != null)
                    {
                        foreach (var med in lisO32)
                        {
                            if (med.IsSetAsDeleted)
                            {
                                if (med.pid > 0)
                                {
                                    _db.RunSql("DELETE FROM o32Contact_Medium WHERE o32ID=@pid", new { pid = med.pid });                                    
                                }
                            }
                            else
                            {
                                var recO32 = new BO.o32Contact_Medium();
                                if (med.pid > 0)
                                {
                                    recO32 = _db.Load<BO.o32Contact_Medium>("select a.*,"+ _db.GetSQL1_Ocas("o32",false,false)+" from o32Contact_Medium a WHERE a.o32ID=@pid", new { pid = med.pid });
                                }
                                recO32.o32Value = med.o32Value; recO32.o32Description = med.o32Description; recO32.o32IsDefaultInInvoice = med.o32IsDefaultInInvoice;recO32.o33ID = med.o33ID;
                                 p = new DL.Params4Dapper();
                                p.AddInt("pid", recO32.pid);
                                p.AddInt("p28ID", intPID, true);
                                p.AddEnumInt("o33ID", recO32.o33ID, true);
                                p.AddString("o32Value", recO32.o32Value);
                                p.AddString("o32Description", recO32.o32Description);
                                p.AddBool("o32IsDefaultInInvoice", recO32.o32IsDefaultInInvoice);
                                int intO32ID = _db.SaveRecord("o32Contact_Medium", p, recO32, false, true);
                                if (intO32ID == 0)
                                {
                                    return 0;
                                }
                            }
                        }
                    }

                    if (j02ids != null)
                    {
                        var lisP30 = _mother.p30Contact_PersonBL.GetList(new BO.myQueryP30() { p28id = intPID });                        
                        foreach(var c in lisP30)
                        {
                            if (!j02ids.Any(p => p == c.j02ID))
                            {
                                _db.RunSql("DELETE FROM p30Contact_Person where p28ID=@p28id AND j02ID=@j02id", new { p28id = intPID, j02id = c.j02ID });
                            }
                        }
                        foreach(int j02id in j02ids)
                        {
                            if (!lisP30.Any(p => p.j02ID == j02id))
                            {
                                _mother.p30Contact_PersonBL.Save(new BO.p30Contact_Person() { p28ID = intPID, j02ID = j02id });
                            }
                        }
                    }
                }

                if (_db.RunSql("exec dbo.p28_aftersave @p28id,@j03id_sys", new { p28id = intPID, j03id_sys = _mother.CurrentUser.pid }))
                {
                    sc.Complete();
                }

                

                return intPID;
            }
                
        }


        private bool ValidateBeforeSave(BO.p28Contact rec, List<BO.o37Contact_Address> lisO37, List<BO.o32Contact_Medium> lisO32)
        {

            if (rec.p28IsCompany && string.IsNullOrEmpty(rec.p28CompanyName))
            {
                this.AddMessage("Chybí název."); return false;
            }
            if (rec.p28IsCompany==false && string.IsNullOrEmpty(rec.p28LastName))
            {
                this.AddMessage("Chybí příjmení fyzické osoby."); return false;
            }
            if (lisO37 != null)
            {
                if (lisO37.Where(p => p.IsSetAsDeleted==false && p.o36ID == BO.o36IdEnum.InvoiceAddress).Count() > 1)
                {
                    this.AddMessage("Klient může mít pouze jednu fakturační adresu."); return false;
                }
                foreach(var c in lisO37.Where(p=>p.IsSetAsDeleted==false))
                {
                    if (string.IsNullOrEmpty(c.o38City) && string.IsNullOrEmpty(c.o38Street))
                    {
                        this.AddMessage("V adrese je třeba vyplnit město nebo ulici."); return false;
                    }
                }
                
            }

            if (lisO32 != null)
            {
                if (lisO32.Where(p => p.IsSetAsDeleted == false && string.IsNullOrEmpty(p.o32Value)).Count() > 0)
                {
                    this.AddMessage("Kontaktní médium musí mít vyplněnou adresu/číslo."); return false;
                }
            }

            return true;
        }

        public BO.p28RecDisposition InhaleRecDisposition(BO.p28Contact rec)
        {
            var c = new BO.p28RecDisposition();

            if (rec.j02ID_Owner==_mother.CurrentUser.j02ID || _mother.CurrentUser.IsAdmin || _mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P28_Owner))
            {
                c.OwnerAccess = true;c.ReadAccess = true;
                return c;
            }
            if (_mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P28_Reader))
            {
                c.ReadAccess = true;
                return c;
            }

            return c;
        }

        public BO.p28ContactSum LoadSumRow(int pid)
        {
            return _db.Load<BO.p28ContactSum>("EXEC dbo.p28_inhale_sumrow @j03id_sys,@pid", new { j03id_sys = _mother.CurrentUser.pid, pid = pid });
        }
    }
}
