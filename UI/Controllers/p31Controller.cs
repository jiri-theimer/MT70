using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Info(int pid, bool isrecord)
        {
            var v = new p31Info() { pid = pid, IsRecord = isrecord };
            v.Rec = Factory.p31WorksheetBL.Load(v.pid);
            if (v.Rec.p91ID > 0)
            {
                v.RecP91 = Factory.p91InvoiceBL.Load(v.Rec.p91ID);
            }
            return View(v);
        }

        

        public IActionResult Record(int pid, bool isclone, int j02id, int p34id,string newrec_prefix, int newrec_pid, string d, string t1, string t2, string approve_guid)
        {
            var v = new p31Record() { rec_pid = pid, rec_entity = "p31", GuidApprove = approve_guid };
            if (pid == 0 && d != null)
            {
                v.p31Date = BO.BAS.String2Date(d);
            }            
            
            v.Rec = new BO.p31WorksheetEntryInput() { pid = pid, p34ID = p34id, j02ID = j02id };
            switch (newrec_prefix)
            {
                case "p41":
                    v.Rec.p41ID = newrec_pid;                    
                    break;
                case "p32":
                    v.RecP32 = Factory.p32ActivityBL.Load(newrec_pid);
                    v.Rec.p32ID = v.RecP32.pid;
                    v.SelectedComboP32Name = v.RecP32.p32Name;
                    break;
                case "p28":
                    break;
            }

            if (pid == 0 && t1 != null)
            {
                v.Rec.TimeFrom = t1;
                if (t2 != null)
                {
                    if (t2 == "24:00") t2 = "23:59";
                    v.Rec.TimeUntil = t2;
                    var xx = Record_RecalcDuration(t1, t2);
                    if (xx.error == null)
                    {
                        v.Rec.Value_Orig = xx.duration;

                    }
                    else
                    {
                        this.AddMessage(xx.error);
                    }

                }
            }
            Handle_Defaults(v);
            if (v.rec_pid > 0)
            {
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

                    return RedirectToAction("Info", new { pid = pid, isrecord = true });
                }
                LoadRecordSetting(v);
                v.Rec = Factory.p31WorksheetBL.CovertRec2Input(recP31,v.Setting.TimesheetEntryByMinutes);
                                
                v.p31Date = v.Rec.p31Date;
                v.SelectedComboP32Name = recP31.p32Name;
                v.SelectedComboP34Name = recP31.p34Name;
                v.SelectedComboPerson = recP31.Person;
                v.SelectedComboProject = recP31.Project;
                v.SelectedComboTask = recP31.p56Name;
                v.SelectedComboJ27Code = recP31.j27Code_Billing_Orig;

                if (v.Rec.p28ID_Supplier > 0)
                {
                    v.SelectedComboSupplier = recP31.SupplierName;
                }
                if (v.Rec.p35ID > 0)
                {
                    v.SelectedComboP35Code = Factory.p35UnitBL.Load(v.Rec.p35ID).p35Code;
                }
                if (v.Rec.j19ID > 0)
                {
                    v.SelectedComboJ19Name = Factory.FBL.LoadJ19(v.Rec.j19ID).j19Name;
                }
                if (v.Rec.p72ID_AfterTrimming != BO.p72IdENUM._NotSpecified)
                {
                    v.IsValueTrimming = true;                   
                }
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
            v.SelectedLevelIndex = 5;
            if (v.p31Date == null)
            {
                v.p31Date = DateTime.Today;
            }
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
                if (v.Rec.p34ID == 0)
                {
                    LoadRecordSetting(v);
                    switch (v.Setting.ActivityFlag)
                    {
                        case 0: //první sešit v nabídce sešitů
                            var mq = new BO.myQueryP34() { j02id_for_p31_entry = v.Rec.j02ID };
                            var lisP34 = Factory.p34ActivityGroupBL.GetList(mq);
                            if (lisP34.Count() > 0)
                            {
                                v.Rec.p34ID = lisP34.First().pid; v.SelectedComboP34Name = lisP34.First().p34Name;
                            }
                            break;
                        case 1: //sešit z naposledy vykazovaného úkonu
                        case 2: //sešit z naposledy vykazovaného úkonu
                            var recLast = Factory.p31WorksheetBL.LoadMyLastCreated(true);
                            if (recLast != null)
                            {
                                v.Rec.p34ID = recLast.p34ID; v.SelectedComboP34Name = recLast.p34Name;
                                if (v.Setting.ActivityFlag == 2)
                                {
                                    v.Rec.p32ID = recLast.p32ID; v.SelectedComboP32Name = recLast.p32Name;
                                }
                            }
                            break;
                        
                    }
                }
                //if (v.Rec.p41ID == 0 && v.Rec.p34ID == 0)
                //{
                //    var recLast = Factory.p31WorksheetBL.LoadMyLastCreated(true, v.Rec.p41ID, v.Rec.p34ID);
                //    if (recLast != null)
                //    {
                //        v.Rec.p41ID = recLast.p41ID; v.Rec.p34ID = recLast.p34ID; v.SelectedComboP34Name = recLast.p34Name;
                //        v.Rec.j27ID_Billing_Orig = recLast.j27ID_Billing_Orig; v.SelectedComboJ27Code = recLast.j27Code_Billing_Orig;
                //    }
                //}
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
            LoadRecordSetting(v);
           
            if (v.Rec.j02ID == 0)
            {
                v.Rec.j02ID = Factory.CurrentUser.j02ID; v.SelectedComboPerson = Factory.CurrentUser.PersonDesc;
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
                if (v.RecP41==null) v.RecP41 = Factory.p41ProjectBL.Load(v.Rec.p41ID);

                if (string.IsNullOrEmpty(v.SelectedComboProject))
                {
                    v.SelectedComboProject = v.RecP41.FullName;
                }
                if (v.ShowTaskComboFlag == 0)
                {
                    var lisP56 = Factory.p56TaskBL.GetList(new BO.myQueryP56() { p41id = v.Rec.p41ID, j02id = v.Rec.j02ID });
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
            if (v.Rec.p34ID > 0 && v.RecP34 == null)
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
                if (v.RecP34.p34ActivityEntryFlag == BO.p34ActivityEntryFlagENUM.AktivitaSeNezadava && v.Rec.p32ID == 0)
                {
                    //výchozí aktivita (skrytá uživateli)
                    var lisP32 = Factory.p32ActivityBL.GetList(new BO.myQueryP32() { p34id = v.RecP34.pid }).Where(p => p.p32IsSystemDefault);
                    if (lisP32.Count() > 0)
                    {
                        v.RecP32 = lisP32.First();
                        v.Rec.p32ID = v.RecP32.pid;
                        v.SelectedComboP32Name = v.RecP32.p32Name;

                    }
                }
                if (v.RecP34.p33ID == BO.p33IdENUM.Cas)
                {
                    if (Factory.CBL.LoadUserParam("p31_TimeInputInterval", "30") == "30")
                    {
                        v.CasOdDoIntervals = "08:00|08:30|09:00|09:30|10:00|10:30|11:00|11:30|12:00|12:30|13:00|13:30|14:00|14:30|15:00|15:30|16:00|16:30|17:00|17:30|18:00";
                    }
                    else
                    {
                        v.CasOdDoIntervals = "08:00|09:00|10:00|11:00|12:00|13:00|14:00|14:30|15:00|15:30|16:00|16:30|17:00|17:30|18:00|19:00";
                    }
                }
            }
            if (v.Rec.p32ID > 0 && v.RecP32 == null)
            {
                v.RecP32 = Factory.p32ActivityBL.Load(v.Rec.p32ID);
            }
            if (v.RecP32 != null && string.IsNullOrEmpty(v.SelectedComboP32Name))
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
                    v.ShowTaskComboFlag = 0; v.Rec.p56ID = 0; v.SelectedComboTask = null;
                    if (v.Rec.p41ID > 0)
                    {
                        v.RecP41 = Factory.p41ProjectBL.Load(v.Rec.p41ID);
                        v.SelectedComboProject = v.RecP41.FullName;
                    }
                    
                    
                    break;
                case "p56id":
                    if (v.Rec.p56ID > 0)
                    {
                        v.RecP56 = Factory.p56TaskBL.Load(v.Rec.p56ID);
                        v.SelectedComboTask = v.RecP56.FullName;                        
                    }
                    break;
                case "isvaluetrimming":
                    if (v.Rec.p72ID_AfterTrimming == BO.p72IdENUM.Fakturovat || v.Rec.p72ID_AfterTrimming == BO.p72IdENUM.FakturovatPozdeji)
                    {
                        v.Rec.Value_Trimmed = v.Rec.Value_Orig;
                    }
                    else
                    {
                        v.Rec.Value_Trimmed = "0";
                    }
                    break;
            }


            RefreshState(v);

            switch (oper)
            {

                case "p32id":
                    if (v.rec_pid == 0 && v.RecP32 != null)
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
                       if (v.Setting.TimesheetEntryByMinutes)
                        {
                            c.p31HoursEntryflag = BO.p31HoursEntryFlagENUM.Minuty;
                        }
                        else
                        {
                            c.p31HoursEntryflag = BO.p31HoursEntryFlagENUM.Hodiny;
                        }
                        
                        break;
                    case BO.p33IdENUM.PenizeBezDPH:
                    case BO.p33IdENUM.PenizeVcDPHRozpisu:
                        c.j27ID_Billing_Orig = v.Rec.j27ID_Billing_Orig;
                        c.p31Code = v.Rec.p31Code;
                        c.j19ID = v.Rec.j19ID;
                        c.p35ID = v.Rec.p35ID;

                        c.p31Calc_PieceAmount = v.Rec.p31Calc_PieceAmount;
                        c.p31Calc_Pieces = v.Rec.p31Calc_Pieces;
                        c.p28ID_Supplier = v.Rec.p28ID_Supplier;

                        c.p31PostCode = v.Rec.p31PostCode;
                        c.p31PostFlag = v.Rec.p31PostFlag;
                        c.p31PostRecipient = v.Rec.p31PostRecipient;

                        break;
                    case BO.p33IdENUM.Kusovnik:
                        c.p35ID = v.Rec.p35ID;
                        break;
                }



                c.Amount_WithoutVat_Orig = v.Rec.Amount_WithoutVat_Orig;
                c.VatRate_Orig = v.Rec.VatRate_Orig;
                c.Amount_Vat_Orig = v.Rec.Amount_Vat_Orig;
                c.Amount_WithVat_Orig = v.Rec.Amount_WithVat_Orig;

                c.p31Text = v.Rec.p31Text;
                c.TimeFrom = v.Rec.TimeFrom;
                c.TimeUntil = v.Rec.TimeUntil;

                if (v.IsValueTrimming)
                {
                    c.p72ID_AfterTrimming = v.Rec.p72ID_AfterTrimming;
                    c.Value_Trimmed = v.Rec.Value_Trimmed;
                }


                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.p31WorksheetBL.SaveOrigRecord(c, v.RecP34.p33ID, v.ff1.inputs);
                if (c.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p31", c.pid, v.TagPids);

                    if (v.GuidApprove != null)
                    {
                        //úprava úkonu, který se právě schvaluje
                        var mq = new BO.myQueryP31();
                        mq.SetPids(c.pid.ToString());                        
                        var lisP31 = this.Factory.p31WorksheetBL.GetList(mq);
                        
                        BO.p72IdENUM p72id = BO.p72IdENUM.Fakturovat;
                        var recTemp = Factory.p31WorksheetBL.LoadTempRecord(c.pid, v.GuidApprove);
                        if (recTemp != null) p72id = recTemp.p72ID_AfterApprove;

                        Factory.p31WorksheetBL.DeleteTempRecord(v.GuidApprove, c.pid);
                        BL.bas.p31Support.SetupTempApproving(this.Factory, lisP31, v.GuidApprove, 0,true, p72id);
                    }

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void LoadRecordSetting(p31Record v)
        {
            if (v.Setting == null)
            {
                v.Setting = new Models.p31oper.hesViewModel() { HoursFormat = Factory.CurrentUser.j03DefaultHoursFormat, TotalFlagValue = Factory.CurrentUser.j03HoursEntryFlagV7 };
                v.Setting.InhaleSetting();
            }
        }

        private bool ValidateBeforeSave(p31Record v)
        {
            if (v.p31Date == null)
            {
                this.AddMessage("Chybí vyplnit datum."); return false;
            }
            return true;
        }

        private BO.p31RecDisposition InhalePermissions(p31Record v, BO.p31Worksheet recP31)
        {
            return Factory.p31WorksheetBL.InhaleRecDisposition(recP31);


        }



        public UI.Models.p31oper.p31CasOdDo Record_RecalcDuration(string timefrom, string timeuntil)
        {
            var ret = new UI.Models.p31oper.p31CasOdDo();

            int t1 = BO.basTime.ConvertTimeToSeconds(timefrom);
            int t2 = BO.basTime.ConvertTimeToSeconds(timeuntil);
            if (t2 > 26 * 60 * 60)
            {
                ret.error = Factory.tra("[Čas do] musí být menší než 24:00.");
            }
            if (t1 > 0 && t2 == 0)
            {
                t2 = t1 + 60 * 60;
            }
            if (t1 > t2)
            {
                ret.error = Factory.tra("[Čas do] musí být větší než [Čas od].");
            }
            if (t1 < 0)
            {
                t1 = BO.basTime.ConvertTimeToSeconds("01:00");
                ret.error = Factory.tra("[Čas do] musí být větší než [Čas od].");
            }
            ret.t1 = BO.basTime.GetTimeFromSeconds(t1);
            ret.t2 = BO.basTime.GetTimeFromSeconds(t2);
            ret.duration = BO.basTime.GetTimeFromSeconds(t2 - t1);
            return ret;
        }
    }
}
