using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;
using System.Net.Http;


namespace UI.Controllers
{
    public class p28Controller : BaseController
    {
        public IActionResult VatInfo(string dic)
        {
            var v = new VatInfoViewModel() { DIC = dic };
            if (string.IsNullOrEmpty(v.DIC))
            {
                return View(v);
            }
            string strDic = BO.BAS.RightString(v.DIC, v.DIC.Length - 2);
            string strCountryCode = v.DIC.Substring(0, 2);            

            var vatQuery = new TriggerMe.VAT.VATQuery();
            try
            {
                var c = vatQuery.CheckVATNumberAsync(strCountryCode, strDic);
                if (c.Result.Valid)
                {
                    v.IsViesValid = true;
                    v.CompanyAddress = c.Result.Address;
                    v.CompanyName = c.Result.Name;
                }
            }
            catch(Exception e)
            {
                this.AddMessageTranslated(e.Message);
            }

            
            //string[] vatnum = new string[1];
            //ADIS.InformaceOPlatciType[] res = new ADIS.InformaceOPlatciType[1];
            //vatnum[0] = strDic;

            //var adisreg = new ADIS.rozhraniCRPDPHClient();
            //adisreg.getStatusNespolehlivyPlatce(vatnum, out res);
            //if (res.Count() == 0)
            //{
            //    this.AddMessage("Registr nedokázal ověřit DIČ.");
            //}
            //else
            //{
            //    switch (res[0].nespolehlivyPlatce)
            //    {
            //        case ADIS.NespolehlivyPlatceType.ANO:
            //            v.ADIS_Veta = "<h3 style='color:red;'>NESPOLEHLIVÝ PLÁTCE</h3>";
            //            break;
            //        case ADIS.NespolehlivyPlatceType.NE:
            //            v.ADIS_Veta = "<h3>SPOLEHLIVÝ</h3>";
            //            break;
            //        case ADIS.NespolehlivyPlatceType.NENALEZEN:
            //            v.ADIS_Veta = "NENALEZEN";
            //            break;
            //    }
            //}


            return View(v);
        }
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new p28Record() { rec_pid = pid, rec_entity = "p28",IsCompany=1,p51Flag=1,TempGuid=BO.BAS.GetGuid() };
            v.Rec = new BO.p28Contact();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p28ContactBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.SetTagging(Factory.o51TagBL.GetTagging("p28", v.rec_pid));
                
                if (!v.Rec.p28IsCompany)
                {
                    v.IsCompany = 0;
                }
                if (v.Rec.j61ID_Invoice > 0)
                {
                    v.SelectedComboJ61Name = Factory.j61TextTemplateBL.Load(v.Rec.j61ID_Invoice).j61Name;
                }
                if (v.Rec.p63ID > 0)
                {
                    v.SelectedComboP63Name = Factory.p63OverheadBL.Load(v.Rec.p63ID).p63Name;
                }
                v.SelectedComboP92Name = v.Rec.p92Name;                
                v.SelectedComboP29Name = v.Rec.p29Name;
                v.SelectedComboP87Name = v.Rec.p87Name;
                v.SelectedComboOwner = v.Rec.Owner;

                if (!InhalePermissions(v))
                {
                    return this.StopPage(true, "Nemáte oprávnění k záznamu.");
                }

                var lisO37 = Factory.p28ContactBL.GetList_o37(v.rec_pid);
                v.lisO37 = new List<o37Repeater>();
                foreach(var c in lisO37)
                {                    
                    v.lisO37.Add(new o37Repeater() {TempGuid = BO.BAS.GetGuid(), pid = c.pid, o38ID = c.o38ID, o36ID = c.o36ID
                        , o38Name = c.o38Name, o38City = c.o38City, o38Street = c.o38Street, o38Country = c.o38Country, o38ZIP = c.o38ZIP});
                }
                var lisO32 = Factory.p28ContactBL.GetList_o32(v.rec_pid);
                v.lisO32 = new List<o32Repeater>();
                foreach (var c in lisO32)
                {
                    v.lisO32.Add(new o32Repeater()
                    {
                        TempGuid = BO.BAS.GetGuid(),
                        pid = c.pid,
                        o33ID = c.o33ID,
                        o32Value = c.o32Value,
                        o32Description = c.o32Description,
                        o32IsDefaultInInvoice = c.o32IsDefaultInInvoice                        
                    });
                }

                
                if (v.Rec.p51ID_Billing > 0)
                {
                    var recP51 = Factory.p51PriceListBL.Load(v.Rec.p51ID_Billing);
                    v.SelectedComboP51Name = recP51.p51Name;
                    if (recP51.p51IsCustomTailor)
                    {
                        v.p51Flag = 3;  //ceník na míru
                        v.SelectedP51ID_Flag3 = v.Rec.p51ID_Billing;
                    }
                    else
                    {
                        v.p51Flag = 2;
                        v.SelectedP51ID_Flag2 = v.Rec.p51ID_Billing;
                    }                                        
                    
                }

            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

        private void RefreshState(p28Record v)
        {
            if (v.ff1 == null)
            {
                v.ff1 = new FreeFieldsViewModel();
                v.ff1.InhaleFreeFieldsView(Factory, v.rec_pid, "p28");                
            }
            v.ff1.RefreshInputsVisibility(Factory, v.rec_pid, "p28", v.Rec.p29ID);

            if (v.lisO37 == null)
            {
                v.lisO37 = new List<o37Repeater>();
            }
            if (v.lisO32 == null)
            {
                v.lisO32 = new List<o32Repeater>();
            }
            v.lisJ02 = Factory.j02PersonBL.GetList_InP28Form(v.rec_pid, v.TempGuid);

            if (v.Rec.j02ID_Owner == 0)
            {
                v.Rec.j02ID_Owner = Factory.CurrentUser.j02ID;
                v.SelectedComboOwner = Factory.CurrentUser.PersonDesc;
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p28Record v,string oper, string guid,int j02id,string j02ids)
        {
            RefreshState(v);

            if (oper == "postback")
            {
                return View(v);
            }
            if (oper== "ares")
            {                
                AresImport(v);
                return View(v);
            }
            if (oper == "vies")
            {
                ViesImport(v);
                return View(v);
            }
            if (oper == "add_o37")
            {
                var c = new o37Repeater() { TempGuid = BO.BAS.GetGuid(),o36ID=BO.o36IdEnum.InvoiceAddress };
                if (v.lisO37.Where(p=>p.IsTempDeleted==false && p.o36ID == BO.o36IdEnum.InvoiceAddress).Count() > 0)
                {
                    c.o36ID = BO.o36IdEnum.PostalAddress;
                }
                v.lisO37.Add(c);
                return View(v);
            }
            if (oper == "delete_o37")
            {
                v.lisO37.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper == "add_o32")
            {
                var c = new o32Repeater() { TempGuid = BO.BAS.GetGuid(),o33ID=BO.o33FlagEnum.Email };                
                v.lisO32.Add(c);
                return View(v);
            }
            if (oper == "delete_o32")
            {
                v.lisO32.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper== "delete_j02")
            {
                var lis = Factory.p85TempboxBL.GetList(v.TempGuid, true, "p30");
                var c = new BO.p85Tempbox() { p85GUID = v.TempGuid, p85Prefix = "p30", p85DataPID = j02id };
                if (lis.Where(p=>p.p85DataPID == j02id && p.p85Prefix == "p30").Count() > 0)
                {
                    c = lis.Where(p => p.p85DataPID == j02id && p.p85Prefix == "p30").First();
                }
                c.p85IsDeleted = true;
                Factory.p85TempboxBL.Save(c);
                v.lisJ02 = Factory.j02PersonBL.GetList_InP28Form(v.rec_pid, v.TempGuid);
                return View(v);
            }
            if (oper== "append_j02ids")
            {
                foreach (int intJ02ID in BO.BAS.ConvertString2ListInt(j02ids))
                {
                    var c = new BO.p85Tempbox() { p85GUID = v.TempGuid, p85Prefix = "p30", p85DataPID = intJ02ID };
                    Factory.p85TempboxBL.Save(c);
                }                
                v.lisJ02 = Factory.j02PersonBL.GetList_InP28Form(v.rec_pid, v.TempGuid);
                return View(v);
            }

            if (ModelState.IsValid)
            {
                BO.p28Contact c = new BO.p28Contact();
                if (v.rec_pid > 0) c = Factory.p28ContactBL.Load(v.rec_pid);
                                
                if (v.IsCompany == 1)
                {
                    c.p28IsCompany = true;
                    c.p28CompanyName = v.Rec.p28CompanyName;
                    c.p28FirstName = null;c.p28LastName = null;c.p28TitleAfterName = null;c.p28TitleBeforeName = null;c.p28Person_BirthRegID = null;
                }
                else
                {
                    c.p28IsCompany = false;
                    c.p28CompanyName = null;
                    c.p28FirstName = v.Rec.p28FirstName;
                    c.p28LastName = v.Rec.p28LastName;
                    c.p28TitleAfterName = v.Rec.p28TitleAfterName;
                    c.p28TitleBeforeName = v.Rec.p28TitleBeforeName;
                    c.p28Person_BirthRegID = v.Rec.p28Person_BirthRegID;
                }
                c.p28RegID = v.Rec.p28RegID;
                c.p28VatID = v.Rec.p28VatID;
                c.p28ICDPH_SK = v.Rec.p28ICDPH_SK;

                c.p28SupplierFlag = v.Rec.p28SupplierFlag;
                c.p28CompanyShortName = v.Rec.p28CompanyShortName;
                c.j02ID_Owner = v.Rec.j02ID_Owner;
                c.p29ID = v.Rec.p29ID;
                c.p63ID = v.Rec.p63ID;
                c.p87ID = v.Rec.p87ID;
                c.j61ID_Invoice = v.Rec.j61ID_Invoice;
                if (v.p51Flag == 2)
                {
                    c.p51ID_Billing = v.SelectedP51ID_Flag2;
                    if (c.p51ID_Billing == 0)
                    {
                        this.AddMessage("Chybí vybrat ceník fakturačních hodinových sazeb.");return View(v);
                    }
                }
                if (v.p51Flag == 3)
                {
                    if (v.SelectedP51ID_Flag3 == 0)
                    {
                        var recP51 = Factory.p51PriceListBL.LoadByTempGuid(v.TempGuid);
                        if (recP51 == null)
                        {
                            this.AddMessage("Chybí ceník hodinových sazeb na míru."); return View(v);
                        }
                        c.p51ID_Billing = recP51.pid;
                    }
                    else
                    {
                        c.p51ID_Billing = v.SelectedP51ID_Flag3;
                    }
                }
                

                c.p28BillingMemo = v.Rec.p28BillingMemo;                
                c.p28InvoiceDefaultText1 = v.Rec.p28InvoiceDefaultText1;
                c.p28InvoiceDefaultText2 = v.Rec.p28InvoiceDefaultText2;

                c.p28InvoiceMaturityDays = v.Rec.p28InvoiceMaturityDays;
                c.p28Round2Minutes = v.Rec.p28Round2Minutes;
                c.p28LimitFee_Notification = v.Rec.p28LimitFee_Notification;
                c.p28LimitHours_Notification = v.Rec.p28LimitHours_Notification;
                
                c.p28Pohoda_VatCode = v.Rec.p28Pohoda_VatCode;
                c.p28ExternalPID = v.Rec.p28ExternalPID;


                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                var lisO37 = new List<BO.o37Contact_Address>();
                foreach(var cc in v.lisO37)
                {
                    lisO37.Add(new BO.o37Contact_Address()
                    { IsSetAsDeleted = cc.IsTempDeleted, pid = cc.pid,o36ID=cc.o36ID
                        , o38ID = cc.o38ID, o38City = cc.o38City,o38Street=cc.o38Street,o38ZIP=cc.o38ZIP,o38Country=cc.o38Country,o38Name=cc.o38Name
                    });
                }
                var lisO32 = new List<BO.o32Contact_Medium>();
                foreach (var cc in v.lisO32)
                {
                    lisO32.Add(new BO.o32Contact_Medium()
                    {
                        IsSetAsDeleted = cc.IsTempDeleted,
                        pid = cc.pid,
                        o33ID = cc.o33ID,                        
                        o32Value = cc.o32Value,
                        o32Description = cc.o32Description,
                        o32IsDefaultInInvoice = cc.o32IsDefaultInInvoice
                    });
                }

                c.pid = Factory.p28ContactBL.Save(c,lisO37,lisO32,v.ff1.inputs,v.lisJ02.Select(p=>p.pid).ToList());
                if (c.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p28", c.pid, v.TagPids);

                    

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private bool InhalePermissions(p28Record v)
        {
            var mydisp = Factory.p28ContactBL.InhaleRecDisposition(v.Rec);
            if (!mydisp.ReadAccess)
            {
                return false;
            }
            if (v.Rec.x38ID > 0)
            {
                v.CanEditRecordCode = Factory.x38CodeLogicBL.CanEditRecordCode(v.Rec.x38ID, mydisp);
            }
            else
            {
                v.CanEditRecordCode = mydisp.OwnerAccess;
            }
            return true;
        }

        private void ViesImport(p28Record v)
        {
            if (string.IsNullOrEmpty(v.Rec.p28VatID))
            {
                this.AddMessage("Chybí vyplnit DIČ."); return;
            }
            v.Rec.p28VatID = v.Rec.p28VatID.ToUpper();
            string strDic = BO.BAS.RightString(v.Rec.p28VatID, v.Rec.p28VatID.Length - 2);
            string strCountryCode = v.Rec.p28VatID.Substring(0, 2);
            var vatQuery =new TriggerMe.VAT.VATQuery();
            var c = vatQuery.CheckVATNumberAsync(strCountryCode, strDic);
            if (!c.Result.Valid)
            {
                this.AddMessageTranslated(v.Rec.p28VatID+": "+Factory.tra("Subjekt nelze ověřit.")); return;
            }
            v.Rec.p28CompanyName = c.Result.Name;
            if (string.IsNullOrEmpty(c.Result.Address) || c.Result.Address.Length<10)
            {
                this.AddMessageTranslated(v.Rec.p28VatID + ": " + Factory.tra("Tento subjekt nemá uloženou adresu ve VIES rejstříku.")); return;
            }
            var arr = BO.BAS.ConvertString2List(c.Result.Address, "\n");
            var adresa = new o37Repeater() { TempGuid = BO.BAS.GetGuid(), o36ID = BO.o36IdEnum.InvoiceAddress,o38Street=arr[0],o38City=arr[1] };
            if (strCountryCode == "CZ")
            {
                adresa.o38Country = "Česká republika";
                if (arr[2].Length >= 6)
                {
                    adresa.o38ZIP = arr[2].Substring(0, 6).Trim();
                }
            }
            if (strCountryCode == "SK")
            {
                adresa.o38Country = "Slovenská republika";
                adresa.o38ZIP = adresa.o38City.Substring(0, 6);   //psč u slováků brát z města
                adresa.o38City = adresa.o38City.Replace(adresa.o38ZIP, "").Trim();
            }
            if (v.lisO37.Where(p => p.o36ID == BO.o36IdEnum.InvoiceAddress && p.IsTempDeleted == false).Count() > 0)
            {
                var adr = v.lisO37.First(p => p.o36ID == BO.o36IdEnum.InvoiceAddress && p.IsTempDeleted == false);
                adresa.pid = adr.pid;
                adresa.o38ID = adr.o38ID;
                adr.IsTempDeleted = true;
                adr.o38ID = 0;
                adr.pid = 0;
            }

            v.lisO37.Add(adresa);
        }

        private void AresImport(p28Record v)
        {
            if (string.IsNullOrEmpty(v.Rec.p28RegID))
            {
                this.AddMessage("Chybí vyplnit IČ.");return;
            }
            var c = new UI.Ares.clsAresImport().LoadByIco(v.Rec.p28RegID);
            if (c.Error != null)
            {
                this.AddMessage(c.Error);
                return;
            }

            v.Rec.p28CompanyName = c.Company;
            if (!string.IsNullOrEmpty(c.DIC))
            {
                v.Rec.p28VatID = c.DIC;
            }

            var adresa = new o37Repeater() { TempGuid = BO.BAS.GetGuid(), o36ID = BO.o36IdEnum.InvoiceAddress };
            if (v.lisO37.Where(p => p.o36ID == BO.o36IdEnum.InvoiceAddress && p.IsTempDeleted==false).Count() > 0)
            {                
                var adr = v.lisO37.First(p => p.o36ID == BO.o36IdEnum.InvoiceAddress && p.IsTempDeleted==false);
                adresa.pid = adr.pid;
                adresa.o38ID = adr.o38ID;                
                adr.IsTempDeleted = true;
                adr.o38ID = 0;
                adr.pid = 0;                             
            }
            adresa.o38City = c.City;
            adresa.o38Street = c.Street;
            adresa.o38ZIP = c.PostCode;
            adresa.o38Country = c.Country;            
            v.lisO37.Add(adresa);

        }
    }
}
