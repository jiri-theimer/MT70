﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;

namespace UI.Controllers
{
    public class p91Controller : BaseController
    {
        

        public IActionResult Info(int pid)
        {
            var v = new p91RecPage() { Factory = this.Factory, prefix = "p91", pid = pid };            
            v.SetGridUrl();
            RefreshStateInfo(v);
            return View(v);
        }
        public IActionResult Tab1(int pid)
        {
            var v = new p91RecPage() { Factory = this.Factory, prefix = "p91", pid = pid };
            v.StatByPrefix = Factory.CBL.LoadUserParam("recpage-p91-statprefix", "p41");
            RefreshStateInfo(v);
            return View(v);
        }
        private void RefreshStateInfo(p91RecPage v)
        {
            v.Rec = Factory.p91InvoiceBL.Load(v.pid);
            if (v.Rec != null)
            {
                //v.RecSum = Factory.p28ContactBL.LoadSumRow(v.Rec.pid);
                if (v.Rec.p93ID > 0)
                {
                    v.RecP93 = Factory.p93InvoiceHeaderBL.Load(v.Rec.p93ID);
                }
                v.RecP86 = Factory.p86BankAccountBL.LoadInvoiceAccount(v.pid);

                v.SetTagging();
                v.lisCenovyRozpis = Factory.p91InvoiceBL.GetList_CenovyRozpis(v.pid,true,true,Factory.CurrentUser.j03LangIndex);
                //v.lisP41 = Factory.p41ProjectBL.GetList(new BO.myQueryP41() { p91id = v.pid });
                v.lisP31 = Factory.p31WorksheetBL.GetList(new BO.myQueryP31() { p91id = v.pid });
                v.SetFreeFields(0);
            }
        }

        public IActionResult RecPage(int pid)
        {
            var v = new p91RecPage() { Factory = this.Factory, pid = pid, prefix = "p91" };
            v.StatByPrefix = Factory.CBL.LoadUserParam("recpage-p91-statprefix", "p41");

            v.NavTabs = new List<NavTab>();

            if (v.pid == 0)
            {
                v.pid = v.LoadLastUsedPid();
            }
            if (v.pid > 0)
            {
                RefreshStateInfo(v);

                if (v.Rec == null)
                {
                    this.Notify_RecNotFound();
                    v.pid = 0;
                }
                else
                {
                    v.SetGridUrl();
                    v.MenuCode = v.Rec.p91Code;
                    v.SaveLastUsedPid();

                    RefreshNavTabs(v);

                }

            }

            if (v.pid == 0)
            {
                v.Rec = new BO.p91Invoice();
            }

            return View(v);

        }

        private void RefreshNavTabs(p91RecPage v)
        {
            if (v.PanelHeight == "none")
            {
                v.NavTabs.Add(AddTab("Tab1", "tab1", "/p91/Tab1?pid=" + v.pid.ToString(), false, null));
            }

            string strBadge = null;

            v.NavTabs.Add(AddTab("Poznámky", "b07", "/b07/List?source=recpage", true, strBadge));

            string strDefTab = Factory.CBL.LoadUserParam("recpage-tab-p91");
            var deftab = v.NavTabs[0];

            foreach (var tab in v.NavTabs)
            {
                tab.Url += "&master_entity=p91Invoice&master_pid=" + v.pid.ToString();
                if (strDefTab != null && tab.Entity == strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka                    
                }
            }
            deftab.CssClass += " active";
            v.DefaultNavTabUrl = deftab.Url;
        }


        public IActionResult Record(int pid)
        {
            var v = new p91Record() { rec_pid = pid, rec_entity = "p91" };
            if (v.rec_pid == 0)
            {
                return this.StopPage(true, "Na vstupu chybí ID faktury.");
            }
            v.Rec = Factory.p91InvoiceBL.Load(v.rec_pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            if (BO.BAS.bit_compare_or(v.Rec.p91LockFlag, 2)) v.Isp91LockFlag2 = true;
            if (BO.BAS.bit_compare_or(v.Rec.p91LockFlag, 4)) v.Isp91LockFlag4 = true;
            if (BO.BAS.bit_compare_or(v.Rec.p91LockFlag, 8)) v.Isp91LockFlag8 = true;


            v.SetTagging(Factory.o51TagBL.GetTagging("p91", v.rec_pid));
            
            v.ComboP28Name = v.Rec.p28Name;
            v.ComboOwner = v.Rec.Owner;
            v.ComboJ17Name = v.Rec.j17Name;
            
            
            if (v.Rec.p98ID > 0)
            {
                v.ComboP98Name = Factory.p98Invoice_Round_Setting_TemplateBL.Load(v.Rec.p98ID).p98Name;
            }
            if (v.Rec.p63ID > 0)
            {
                v.ComboP63Name = Factory.p63OverheadBL.Load(v.Rec.p63ID).p63Name;
            }
            if (v.Rec.p80ID > 0)
            {
                v.ComboP80Name = Factory.p80InvoiceAmountStructureBL.Load(v.Rec.p80ID).p80Name;
            }
            if (v.Rec.j19ID > 0)
            {
                v.ComboJ19Name = Factory.FBL.LoadJ19(v.Rec.j19ID).j19Name;
            }
            
            
            RefreshStateRecord(v);
            if (!InhalePermissions(v))
            {
                return this.StopPage(true, "Nemáte oprávnění k záznamu.");
            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            
            return View(v);
        }

        private void RefreshStateRecord(p91Record v)
        {
            if (v.RecP92 == null)
            {
                v.RecP92 = Factory.p92InvoiceTypeBL.Load(v.Rec.p92ID);
            }
            if (v.ff1 == null)
            {
                v.ff1 = new FreeFieldsViewModel();
                v.ff1.InhaleFreeFieldsView(Factory, v.rec_pid, "p91");
            }
            v.ff1.RefreshInputsVisibility(Factory, v.rec_pid, "p91", v.Rec.p92ID);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p91Record v, string oper)
        {
            RefreshStateRecord(v);
            if (oper == "p28id" && v.Rec.p28ID>0)
            {
                var recO38 = Factory.o38AddressBL.LoadInvoiceAddress(v.Rec.p28ID);
                if (recO38 == null)
                {
                    v.Rec.o38ID_Primary = 0;
                    this.AddMessage("Klient nemá zavedenou fakturační adresu.");
                }
                else
                {
                    v.Rec.o38ID_Primary = recO38.pid;
                }
                return View(v);
            }
            
            if (oper != null)
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.p91Invoice c = Factory.p91InvoiceBL.Load(v.rec_pid);                               
                c.p91Date = v.Rec.p91Date;
                c.p91DateMaturity = v.Rec.p91DateMaturity;
                c.p91DateSupply = v.Rec.p91DateSupply;
                c.p91Datep31_From = v.Rec.p91Datep31_From;
                c.p91Datep31_Until = v.Rec.p91Datep31_Until;

                c.j19ID = v.Rec.j19ID;
                c.p80ID = v.Rec.p80ID;
                c.p63ID = v.Rec.p63ID;
                c.p98ID = v.Rec.p98ID;
                c.j17ID = v.Rec.j17ID;
                c.p91Text1 = v.Rec.p91Text1;               
                c.p91Text2 = v.Rec.p91Text2;

                c.p28ID = v.Rec.p28ID;
                c.o38ID_Primary = v.Rec.o38ID_Primary;
                c.o38ID_Delivery = v.Rec.o38ID_Delivery;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                c.j02ID_Owner = v.Rec.j02ID_Owner;

                c.p91LockFlag = 0;
                if (v.Isp91LockFlag2) c.p91LockFlag += 2;
                if (v.Isp91LockFlag4) c.p91LockFlag += 4;
                if (v.Isp91LockFlag8) c.p91LockFlag += 8;

                c.p91Client = v.Rec.p91Client;
                c.p91Client_RegID = v.Rec.p91Client_RegID;
                c.p91Client_VatID = v.Rec.p91Client_VatID;
                c.p91Client_ICDPH_SK = v.Rec.p91Client_ICDPH_SK;
                c.p91ClientAddress1_City = v.Rec.p91ClientAddress1_City;
                c.p91ClientAddress1_Street = v.Rec.p91ClientAddress1_Street;
                c.p91ClientAddress1_ZIP = v.Rec.p91ClientAddress1_ZIP;
                c.p91ClientAddress1_Country = v.Rec.p91ClientAddress1_Country;
                c.p91ClientPerson = v.Rec.p91ClientPerson;

                c.pid = Factory.p91InvoiceBL.Update(c, v.ff1.inputs);
                if (c.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p91", c.pid, v.TagPids);

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }




        private bool InhalePermissions(p91Record v)
        {
            var mydisp = Factory.p91InvoiceBL.InhaleRecDisposition(v.Rec);
            if (!mydisp.ReadAccess)
            {
                return false;
            }
            if (!v.Rec.p91IsDraft)
            {
                if (v.RecP92.x38ID > 0)
                {
                    v.CanEditRecordCode = Factory.x38CodeLogicBL.CanEditRecordCode(v.RecP92.x38ID, mydisp);
                }
                else
                {
                    v.CanEditRecordCode = mydisp.OwnerAccess;
                }
            }
            
            return true;
        }

        public BO.o38Address LoadClientAddress(int p28id,int o36id)
        {            
            if (o36id == 1)
            {
                return Factory.o38AddressBL.LoadInvoiceAddress(p28id);
            }
            else
            {
                return Factory.o38AddressBL.LoadPostAddress(p28id);
            }
            
        }
        public BO.p28Contact LoadClientProfile(int p28id)
        {
            return Factory.p28ContactBL.Load(p28id);
        }
    }
}