using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;
using UI.Models.Tab1;

namespace UI.Controllers
{
    public class p41Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            return Tab1(pid, "info");
        }
        public IActionResult Tab1(int pid, string caller)
        {
            var v = new p41Tab1() { Factory = this.Factory, prefix = "p41", pid = pid, caller = caller };

            RefreshStateTab1(v);
            return View(v);
        }

        private void RefreshStateTab1(p41Tab1 v)
        {
            v.Rec = Factory.p41ProjectBL.Load(v.pid);
            if (v.Rec != null)
            {
                if (v.Rec.p61ID > 0)
                {
                    v.p61Name = Factory.p61ActivityClusterBL.Load(v.Rec.p61ID).p61Name;
                }
                v.RecSum = Factory.p41ProjectBL.LoadSumRow(v.Rec.pid);
                if (v.Rec.p28ID_Client > 0)
                {
                    v.RecClient = Factory.p28ContactBL.Load(v.Rec.p28ID_Client);
                }
                if (v.Rec.p87ID > 0)
                {
                    v.p87Name = Factory.FBL.LoadP87(v.Rec.p87ID).p87Name;
                }
                else
                {
                    if (v.RecClient !=null)
                    {
                        v.p87Name = v.RecClient.p87Name;
                    }
                }
                v.SetTagging();
                
                v.SetFreeFields(0);
            }
        }

        public IActionResult Record(int pid, bool isclone,string levelprefix)
        {
            var v = new p41Record() { rec_pid = pid, rec_entity = "p41", p51Flag = 1, TempGuid = BO.BAS.GetGuid() };
            v.Rec = new BO.p41Project();
            v.roles = new RoleAssignViewModel() { RecPid = v.rec_pid, RecPrefix = "p41" };
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p41ProjectBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.SetTagging(Factory.o51TagBL.GetTagging("p41", v.rec_pid));

                             
                v.SelectedComboP92Name = v.Rec.p92Name;
                v.SelectedComboP42Name = v.Rec.p42name;
                v.SelectedComboJ18Name = v.Rec.j18Name;
                if (v.Rec.p87ID > 0)
                {
                    v.SelectedComboP87Name = Factory.FBL.LoadP87(v.Rec.p87ID).p87Name ;
                }
                
                if (v.Rec.p41ParentID > 0)
                {
                    v.RecParent = Factory.p41ProjectBL.Load(v.Rec.p41ParentID);
                    v.SelectedComboParent = v.RecParent.FullName;
                    v.SelectedParentLevelIndex = v.RecParent.p07Level;
                }
                if (v.Rec.p28ID_Client > 0)
                {
                    v.SelectedComboClient = v.Rec.Client;
                }
                if (v.Rec.p28ID_Billing > 0)
                {
                    v.SelectedComboOdberatel = Factory.p28ContactBL.Load(v.Rec.p28ID_Billing).p28name;
                }
                if (v.Rec.p61ID > 0)
                {
                    v.SelectedComboP61Name = Factory.p61ActivityClusterBL.Load(v.Rec.p61ID).p61Name;
                }
                

                v.SelectedComboOwner = v.Rec.Owner;

                if (!InhalePermissions(v))
                {
                    return this.StopPage(true, "Nemáte oprávnění k záznamu.");
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
            else
            {
                //nový projekt
                int levelindex = 0;
                if (!string.IsNullOrEmpty(levelprefix))
                {
                    levelindex = Convert.ToInt32(levelprefix.Substring(2, 1));
                }
                var recLast = Factory.p41ProjectBL.LoadLastCreated(levelindex);
                if (recLast != null)
                {
                    v.Rec.p42ID = recLast.p42ID;
                    v.SelectedComboP42Name = recLast.p42name;

                    if (recLast.p41ParentID > 0)
                    {
                        var recParent = Factory.p41ProjectBL.Load(recLast.p41ParentID);
                        v.SelectedParentLevelIndex = recParent.p07Level;
                    }
                }
            }
           
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
                
            }

            return View(v);
        }

        private void RefreshState(p41Record v)
        {
            if (v.RecParent==null && v.Rec.p41ParentID > 0)
            {
                v.RecParent = Factory.p41ProjectBL.Load(v.Rec.p41ParentID);
            }
            if (v.Rec.p42ID > 0)
            {
                v.RecP42 = Factory.p42ProjectTypeBL.Load(v.Rec.p42ID);
                v.lisParentLevels = Factory.p07ProjectLevelBL.GetList(new BO.myQuery("p07")).Where(p => p.p07Level<v.RecP42.p07Level);
                v.TagEntity = "le"+v.RecP42.p07Level.ToString();
            }
            else
            {
                v.TagEntity = "p41Project";
            }
            

            if (v.ff1 == null)
            {
                v.ff1 = new FreeFieldsViewModel();
                v.ff1.InhaleFreeFieldsView(Factory, v.rec_pid, "p41");
            }
            v.ff1.RefreshInputsVisibility(Factory, v.rec_pid, "p41", v.Rec.p42ID);
           
            if (v.Rec.j02ID_Owner == 0)
            {
                v.Rec.j02ID_Owner = Factory.CurrentUser.j02ID;
                v.SelectedComboOwner = Factory.CurrentUser.PersonDesc;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p41Record v, string oper)
        {
            RefreshState(v);
            if (oper== "parentlevel")
            {
                v.SelectedComboParent = null;
                v.Rec.p41ParentID = 0;
            }
            if (oper == "p42id" && v.rec_pid==0 && v.RecP42 !=null)
            {
                v.SelectedParentLevelIndex = v.RecP42.p07Level - 1;
            }
         
            if (oper != null)
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.p41Project c = new BO.p41Project();
                if (v.rec_pid > 0) c = Factory.p41ProjectBL.Load(v.rec_pid);
                c.p41Name = v.Rec.p41Name;
                c.p41NameShort = v.Rec.p41NameShort;
                c.p41ParentID = v.Rec.p41ParentID;
                c.p28ID_Client = v.Rec.p28ID_Client;
                c.p28ID_Billing = v.Rec.p28ID_Billing;
                c.p41PlanFrom = v.Rec.p41PlanFrom;
                c.p41PlanUntil = v.Rec.p41PlanUntil;
                c.p42ID = v.Rec.p42ID;
                c.p87ID = v.Rec.p87ID;
                c.j18ID = v.Rec.j18ID;
                c.p41InvoiceMaturityDays = v.Rec.p41InvoiceMaturityDays;
                c.p61ID = v.Rec.p61ID;
                c.p41WorksheetOperFlag = v.Rec.p41WorksheetOperFlag;
                c.p41IsNoNotify = v.Rec.p41IsNoNotify;
                c.p72ID_NonBillable = v.Rec.p72ID_NonBillable;
                if (v.p51Flag == 2)
                {
                    c.p51ID_Billing = v.SelectedP51ID_Flag2;
                    if (c.p51ID_Billing == 0)
                    {
                        this.AddMessage("Chybí vybrat ceník fakturačních hodinových sazeb."); return View(v);
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


                c.p41BillingMemo = v.Rec.p41BillingMemo;
                c.p41InvoiceDefaultText1 = v.Rec.p41InvoiceDefaultText1;
                c.p41InvoiceDefaultText2 = v.Rec.p41InvoiceDefaultText2;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                c.j02ID_Owner = v.Rec.j02ID_Owner;
                c.p41ExternalPID = v.Rec.p41ExternalPID;

                c.pid = Factory.p41ProjectBL.Save(c, v.ff1.inputs, v.roles.getList4Save(Factory));
                if (c.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p41", c.pid, v.TagPids);

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

            private bool InhalePermissions(p41Record v)
        {
            var mydisp = Factory.p41ProjectBL.InhaleRecDisposition(v.Rec);
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

        public IActionResult SelectProject(string source_prefix, int source_pid,int leindex)
        {
            if (leindex == 0) leindex = 5;
            var v = new UI.Models.p41.SelectProjectViewModel() { source_prefix = source_prefix,source_pid=source_pid,leindex=5 };

            var mq = new BO.myQueryP41("le" + v.leindex.ToString());
            switch (v.source_prefix)
            {
                case "p28":
                    mq.p28id = v.source_pid;break;
            }
            v.lisP41 = Factory.p41ProjectBL.GetList(mq);
            return View(v);
        }
    }
}
