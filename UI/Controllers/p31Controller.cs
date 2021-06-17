﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;
using UI.Models.Info;

namespace UI.Controllers
{
    public class p31Controller : BaseController
    {
        public IActionResult Info(int pid,bool isrecord)
        {
            var v = new p31Info() { pid = pid,IsRecord= isrecord };
            v.Rec = Factory.p31WorksheetBL.Load(v.pid);
            if (v.Rec.p91ID > 0)
            {
                v.RecP91 = Factory.p91InvoiceBL.Load(v.Rec.p91ID);
            }
            return View(v);
        }

        

        public IActionResult Record(int pid, bool isclone,int p41id, int j02id, int p34id,int p56id)
        {            
            var v = new p31Record() { rec_pid = pid, rec_entity = "p31"};
            v.Rec = new BO.p31WorksheetEntryInput() {pid=pid, p41ID = p41id, p34ID = p34id, j02ID = j02id };
            Handle_Defaults(v);
            if (v.rec_pid > 0)            
            {
                return RedirectToAction("Info", new { pid = pid,isrecord=true });
                var recP31 = Factory.p31WorksheetBL.Load(v.rec_pid);
                if (recP31 == null)
                {
                    return this.StopPage(true, "Záznam nebyl nalezen.");
                }
                var disp = InhalePermissions(v, recP31);
                if (!disp.ReadAccess)
                {
                    return this.StopPage(true, "Nemáte oprávnění k záznamu.");
                }
                if (disp.RecordState != BO.p31RecordState.Editing)
                {
                    
                    return RedirectToAction("Info",new { pid = pid,isrecord=true });
                }
                v.Rec = Factory.p31WorksheetBL.CovertRec2Input(recP31);
                v.p31Date = v.Rec.p31Date;
                v.SelectedComboP32Name = recP31.p32Name;
                v.SelectedComboP34Name = recP31.p34Name;
                v.SelectedComboPerson = recP31.Person;
                v.SelectedComboProject = recP31.Project;
                v.SelectedComboTask = recP31.p56Name;
                v.SelectedComboJ27Code = recP31.j27Code_Billing_Orig;
                v.SelectedComboSupplier = recP31.SupplierName;

                v.SetTagging(Factory.o51TagBL.GetTagging("p31", v.rec_pid));

                
            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            RefreshState(v);



            return View(v);
        }

        private void Handle_Defaults(p31Record v)
        {
            v.p31Date = DateTime.Today;v.SelectedLevelIndex = 5;
            if (v.rec_pid == 0)
            {
                //vykázat nový úkon
                if (v.Rec.j02ID == 0)
                {
                    v.Rec.j02ID = Factory.CurrentUser.j02ID;
                }
                if (v.Rec.j02ID == Factory.CurrentUser.j02ID)
                {
                    v.SelectedComboPerson = Factory.CurrentUser.PersonDesc;
                }
                else
                {
                    v.SelectedComboPerson = Factory.j02PersonBL.Load(v.Rec.j02ID).FullNameDesc;
                }
                if (v.Rec.p41ID==0 && v.Rec.p34ID == 0)
                {
                    var recLast = Factory.p31WorksheetBL.LoadMyLastCreated(true, v.Rec.p41ID, v.Rec.p34ID);
                    if (recLast != null)
                    {
                        v.Rec.p41ID = recLast.p41ID;v.Rec.p34ID = recLast.p34ID;v.SelectedComboP34Name = recLast.p34Name;
                        v.Rec.j27ID_Billing_Orig = recLast.j27ID_Billing_Orig;v.SelectedComboJ27Code = recLast.j27Code_Billing_Orig;
                    }
                }
                if (v.Rec.p32ID > 0)
                {
                    v.RecP32 = Factory.p32ActivityBL.Load(v.Rec.p32ID);
                    v.Rec.p31MarginHidden = v.RecP32.p32MarginHidden;
                    v.Rec.p31MarginTransparent = v.RecP32.p32MarginTransparent;
                }
                
            }
            

        }


        private void RefreshState(p31Record v)
        {
            if (v.Rec.j02ID == 0)
            {
                v.Rec.j02ID = Factory.CurrentUser.j02ID;v.SelectedComboPerson = Factory.CurrentUser.PersonDesc;
            }
            if (v.lisLevelIndex == null)
            {
                v.lisLevelIndex = new List<BO.ListItemValue>();
                if (Factory.CurrentUser.p07LevelsCount == 1)
                {
                    v.lisLevelIndex.Add(new BO.ListItemValue() { Text = Factory.CurrentUser.getP07Level(5, true), Value = 5 });
                }
                else
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        if (Factory.CurrentUser.getP07Level(i, true) != null)
                        {
                            v.lisLevelIndex.Add(new BO.ListItemValue() { Text = Factory.CurrentUser.getP07Level(i, true), Value = i });
                        }
                    }
                }                                
            }
            v.ProjectEntity = "p41Project";
            if (v.lisLevelIndex.Count() > 1)
            {
                v.ProjectEntity = "le" + v.SelectedLevelIndex.ToString();
            }
            if (v.Rec.p41ID > 0)
            {
                v.RecP41 = Factory.p41ProjectBL.Load(v.Rec.p41ID);
                if (string.IsNullOrEmpty(v.SelectedComboProject))
                {
                    v.SelectedComboProject = v.RecP41.FullName;
                }
                if (v.ShowTaskComboFlag == 0)
                {
                    var lisP56 = Factory.p56TaskBL.GetList(new BO.myQueryP56() { p41id = v.Rec.p41ID,j02id=v.Rec.j02ID });
                    if (lisP56.Count() > 0)
                    {
                        v.ShowTaskComboFlag = 1;    //zobrazovat nabídku úkolů k projektu
                    }
                    else
                    {
                        v.ShowTaskComboFlag = 2;    //nezobrazovat nabídku úkolů k projektu
                    }
                }
            }
            if (v.Rec.p34ID > 0 && v.RecP34==null)
            {                
                v.RecP34 = Factory.p34ActivityGroupBL.Load(v.Rec.p34ID);                
            }
            if (v.RecP34 != null)
            {
                if (string.IsNullOrEmpty(v.SelectedComboP34Name))
                {
                    v.SelectedComboP34Name = v.RecP34.p34Name;
                }
                if ((v.RecP34.p33ID == BO.p33IdENUM.PenizeBezDPH || v.RecP34.p33ID == BO.p33IdENUM.PenizeVcDPHRozpisu) && v.PiecePriceFlag == 0)
                {
                    v.PiecePriceFlag = Factory.CBL.LoadUserParamInt("p31/record-PiecePriceFlag", 1);

                }
                if (v.RecP34.p34ActivityEntryFlag == BO.p34ActivityEntryFlagENUM.AktivitaSeNezadava && v.Rec.p32ID==0)
                {
                    //výchozí aktivita (skrytá uživateli)
                    var lisP32 = Factory.p32ActivityBL.GetList(new BO.myQueryP32() { p34id = v.RecP34.pid }).Where(p=>p.p32IsSystemDefault);
                    if (lisP32.Count() > 0)
                    {
                        v.RecP32 = lisP32.First();
                        v.Rec.p32ID = v.RecP32.pid;
                        v.SelectedComboP32Name = v.RecP32.p32Name;
                        
                    }
                }
            }
            if (v.Rec.p32ID > 0 && v.RecP32==null)
            {
                v.RecP32 = Factory.p32ActivityBL.Load(v.Rec.p32ID);                
            }
            if (v.RecP32 !=null && string.IsNullOrEmpty(v.SelectedComboP32Name))
            {
                v.SelectedComboP32Name = v.RecP32.p32Name;
            }


            if (v.ff1 == null)
            {
                v.ff1 = new FreeFieldsViewModel();
                v.ff1.InhaleFreeFieldsView(Factory, v.rec_pid, "p31");
            }
            v.ff1.RefreshInputsVisibility(Factory, v.rec_pid, "p31", v.Rec.p34ID);
            v.ff1.caller = "p31record";

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p31Record v, string oper)
        {
            switch (oper)
            {
                case "p34id":
                    v.Rec.p32ID = 0; v.RecP32 = null; v.SelectedComboP32Name = null;    //musí být před RefreshState(v)
                    break;
                case "levelindex":
                    v.Rec.p41ID = 0; v.SelectedComboProject = null; v.ShowTaskComboFlag = 0; v.Rec.p56ID = 0; v.SelectedComboTask = null;
                    break;
                case "p41id":
                    v.ShowTaskComboFlag = 0;v.Rec.p56ID = 0;v.SelectedComboTask = null;
                    break;
            }


            RefreshState(v);
           
            switch (oper)
            {
                     
                case "p32id":
                    if (v.rec_pid==0 && v.RecP32 != null)
                    {
                        v.Rec.p31MarginHidden = v.RecP32.p32MarginHidden;
                        v.Rec.p31MarginTransparent = v.RecP32.p32MarginTransparent;
                    }
                    break;
                
            }
            if (!string.IsNullOrEmpty(oper))
            {
                return View(v);
            }


            if (ModelState.IsValid)
            {
                if (!ValidateBeforeSave(v))
                {
                    return View(v);
                }
                BO.p31WorksheetEntryInput c = new BO.p31WorksheetEntryInput();
                if (v.rec_pid > 0)
                {
                    c.SetPID(v.rec_pid);                    
                }
                c.p31Date = Convert.ToDateTime(v.p31Date);
                c.j02ID = v.Rec.j02ID;
                c.p41ID = v.Rec.p41ID;
                c.p56ID = v.Rec.p56ID;
                c.p34ID = v.Rec.p34ID;
                c.p32ID = v.Rec.p32ID;

                c.Value_Orig = v.Rec.Value_Orig;
                switch (v.RecP34.p33ID)
                {
                    case BO.p33IdENUM.Cas:
                        c.p31HoursEntryflag = BO.p31HoursEntryFlagENUM.Hodiny;                        
                        break;
                    case BO.p33IdENUM.PenizeBezDPH:
                    case BO.p33IdENUM.PenizeVcDPHRozpisu:
                        c.j27ID_Billing_Orig = v.Rec.j27ID_Billing_Orig;
                        break;
                    case BO.p33IdENUM.Kusovnik:
                        
                        break;
                }
              
                

                c.Amount_WithoutVat_Orig = v.Rec.Amount_WithoutVat_Orig;
                c.VatRate_Orig = v.Rec.VatRate_Orig;
                c.Amount_Vat_Orig = v.Rec.Amount_Vat_Orig;                
                c.Amount_WithVat_Orig = v.Rec.Amount_WithVat_Orig;

                c.p31Text = v.Rec.p31Text;
                c.TimeFrom = v.Rec.TimeFrom;
                c.TimeUntil = v.Rec.TimeUntil;

              
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                
                c.pid = Factory.p31WorksheetBL.SaveOrigRecord(c,v.RecP34.p33ID, v.ff1.inputs);
                if (c.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p31", c.pid, v.TagPids);

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private bool ValidateBeforeSave(p31Record v)
        {
            if (v.p31Date == null)
            {
                this.AddMessage("Chybí vyplnit datum.");return false;
            }
            return true;
        }

        private BO.p31RecDisposition InhalePermissions(p31Record v,BO.p31Worksheet recP31)
        {
            return Factory.p31WorksheetBL.InhaleRecDisposition(recP31);
            
            
        }
    }
}
