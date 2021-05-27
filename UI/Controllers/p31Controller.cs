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
        public IActionResult Info(int pid)
        {
            var v = new p31Info() { pid = pid };
            v.Rec = Factory.p31WorksheetBL.Load(v.pid);
            if (v.Rec.p91ID > 0)
            {
                v.RecP91 = Factory.p91InvoiceBL.Load(v.Rec.p91ID);
            }
            return View(v);
        }

        public IActionResult Record(int pid, bool isclone,int p41id, int j02id, int p34id)
        {
            var v = new p31Record() { rec_pid = pid, rec_entity = "p31"};
            v.Rec = new BO.p31WorksheetEntryInput() {pid=pid, p41ID = p41id, p34ID = p34id, j02ID = j02id };
            Handle_Defaults(v);
            if (v.rec_pid > 0)            
            {
                var recP31 = Factory.p31WorksheetBL.Load(v.rec_pid);
                v.Rec = Factory.p31WorksheetBL.CovertRec2Input(recP31);
                v.p31Date = v.Rec.p31Date;
                v.SelectedComboP32Name = recP31.p32Name;
                v.SelectedComboP34Name = recP31.p34Name;
                v.SelectedComboPerson = recP31.Person;
                v.SelectedComboProject = recP31.Project;
                v.SelectedComboTask = recP31.p56Name;
                v.SelectedComboJ27Code = recP31.j27Code_Billing_Orig;

                v.SetTagging(Factory.o51TagBL.GetTagging("p31", v.rec_pid));

                if (!InhalePermissions(v,recP31))
                {
                    return this.StopPage(true, "Nemáte oprávnění k záznamu.");
                }
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
            }
            if (v.Rec.p34ID > 0)
            {
                v.RecP34 = Factory.p34ActivityGroupBL.Load(v.Rec.p34ID);
                if (string.IsNullOrEmpty(v.SelectedComboP34Name))
                {
                    v.SelectedComboP34Name = v.RecP34.p34Name;
                }
            }

            if (v.ff1 == null)
            {
                v.ff1 = new FreeFieldsViewModel();
                v.ff1.InhaleFreeFieldsView(Factory, v.rec_pid, "p31");
            }
            v.ff1.RefreshInputsVisibility(Factory, v.rec_pid, "p31", v.Rec.p34ID);


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p31Record v, string oper)
        {
            RefreshState(v);
           
            switch (oper)
            {
                case "p34id":
                    v.Rec.p32ID = 0; v.SelectedComboP32Name = null;
                    break;
                case "levelindex":
                    v.Rec.p41ID = 0;v.SelectedComboProject = null;
                    break;
            }
            if (!string.IsNullOrEmpty(oper))
            {
                return View(v);
            }


            if (ModelState.IsValid)
            {
                BO.p31WorksheetEntryInput c = new BO.p31WorksheetEntryInput();
                if (v.rec_pid > 0)
                {
                    c.SetPID(v.rec_pid);                    
                }
                c.p31Date = v.Rec.p31Date;
                c.j02ID = v.Rec.j02ID;
                c.p41ID = v.Rec.p41ID;
                c.p56ID = v.Rec.p56ID;
                c.p34ID = v.Rec.p34ID;
                c.p32ID = v.Rec.p32ID;

                c.Value_Orig = v.Rec.Value_Orig;

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

        private bool InhalePermissions(p31Record v,BO.p31Worksheet recP31)
        {
            var mydisp = Factory.p31WorksheetBL.InhaleRecDisposition(recP31);
            if (!mydisp.ReadAccess)
            {
                return false;
            }
            
            return true;
        }
    }
}
