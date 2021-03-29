using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;


namespace UI.Controllers
{
    public class p28Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new p28Record() { rec_pid = pid, rec_entity = "p28",IsCompany=1,p51Flag=1 };
            v.Rec = new BO.p28Contact();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p28ContactBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
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
                v.RecFirstAddress = Factory.o38AddressBL.LoadFirstP28Address(v.rec_pid);
                if (v.RecFirstAddress != null)
                {
                    v.o38ID_First = v.RecFirstAddress.pid;
                }
                if (v.Rec.p51ID_Billing > 0)
                {
                    v.SelectedComboP51Name = v.Rec.p51Name_Billing;
                    var recP51 = Factory.p51PriceListBL.Load(v.Rec.p51ID_Billing);
                    if (recP51.p51IsCustomTailor)
                    {
                        v.p51Flag = 3;
                    }
                    else
                    {
                        v.p51Flag = 2;
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
            if (v.RecFirstAddress == null)
            {
                v.RecFirstAddress = new BO.o38Address();
            }
            if (v.Rec.j02ID_Owner == 0)
            {
                v.Rec.j02ID_Owner = Factory.CurrentUser.j02ID;
                v.SelectedComboOwner = Factory.CurrentUser.PersonDesc;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p28Record v,string oper, string guid)
        {
            RefreshState(v);

            if (oper == "postback")
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {
                BO.p28Contact c = new BO.p28Contact();
                if (v.rec_pid > 0) c = Factory.p28ContactBL.Load(v.rec_pid);
                                
                if (v.IsCompany == 1)
                {
                    c.p28CompanyName = v.Rec.p28CompanyName;
                }
                else
                {
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

                v.RecFirstAddress.pid = v.o38ID_First;

                c.pid = Factory.p28ContactBL.Save(c,v.RecFirstAddress,null);
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}
