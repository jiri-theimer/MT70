using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;

namespace UI.Controllers
{
    public class o23Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new o23RecPage() { Factory = this.Factory, prefix = "o23", pid = pid };
            v.SetGridUrl();
            RefreshStateInfo(v);
            return View(v);
        }
        public IActionResult Tab1(int pid)
        {
            var v = new o23RecPage() { Factory = this.Factory, prefix = "o23", pid = pid };
            RefreshStateInfo(v);
            return View(v);
        }
        public IActionResult RecPage(int pid)
        {
            var v = new o23RecPage() { Factory = this.Factory, pid = pid, prefix = "o23" };

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
                    v.MenuCode = v.Rec.o23Name;
                    v.SaveLastUsedPid();

                    RefreshNavTabs(v);

                }

            }

            if (v.pid == 0)
            {
                v.Rec = new BO.o23Doc();
            }

            return View(v);

        }

        private void RefreshNavTabs(o23RecPage v)
        {
            
            if (v.PanelHeight == "none")
            {
                v.NavTabs.Add(AddTab("Tab1", "tab1", "/o23/Tab1?pid=" + v.pid.ToString(), false, null));
            }

            string strBadge = null;
            
            v.NavTabs.Add(AddTab("Poznámky", "b07", "/b07/List?source=recpage", true, strBadge));
            
            string strDefTab = Factory.CBL.LoadUserParam("recpage-tab-o23");
            var deftab = v.NavTabs[0];

            foreach (var tab in v.NavTabs)
            {
                tab.Url += "&master_entity=o23Doc&master_pid=" + v.pid.ToString();
                if (strDefTab != null && tab.Entity == strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka                    
                }
            }
            deftab.CssClass += " active";
            v.DefaultNavTabUrl = deftab.Url;
        }

        private void RefreshStateInfo(o23RecPage v)
        {
            v.Rec = Factory.o23DocBL.Load(v.pid);
            if (v.Rec != null)
            {
                
                v.SetTagging();
                v.lisO27 = Factory.o27AttachmentBL.GetList(new BO.myQueryO27() { x29id = 223,recpid=v.pid });
                
            }
        }

        public IActionResult SelectDocType(string prefix,int recpid)
        {
            var v = new SelectDocTypeViewModel() { prefix = prefix, recpid = recpid };
            var mq = new BO.myQueryX18();
            if (prefix != null)
            {
                mq.x29id = BO.BASX29.GetInt(prefix);
            }
            v.lisX18 = Factory.x18EntityCategoryBL.GetList(mq).OrderBy(p=>p.x18Ordinary);

            return View(v);
        }
        public IActionResult Record(int pid, bool isclone,int x18id,string prefix,int recpid)
        {            
            var v = new o23Record() { rec_pid = pid, rec_entity = "o23",x18ID=x18id,UploadGuid=BO.BAS.GetGuid() };
            if (v.x18ID == 0 && v.rec_pid==0)
            {
                return RedirectToAction("SelectDocType",new { prefix = prefix, recpid = recpid });
            }
            v.Rec = new BO.o23Doc();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.o23DocBL.Load(v.rec_pid);                
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.x18ID = v.Rec.x18ID;
                var recB07 = Factory.b07CommentBL.LoadByRecord(223, v.rec_pid); //přílohy jsou v dokumentu uložey přes vazbu na b07comment
                if (recB07 != null)
                {
                    v.b07ID = recB07.pid;
                    var lisO27 = Factory.o27AttachmentBL.GetList(new BO.myQueryO27() { b07id = v.b07ID });
                    v.lisO27 = new List<o27Repeator>();
                    foreach (var c in lisO27)
                    {
                        v.lisO27.Add(new o27Repeator()
                        {
                            TempGuid = BO.BAS.GetGuid(),
                            pid = c.pid,
                            o27OriginalFileName = c.o27OriginalFileName,
                            o27FileSize = c.o27FileSize,
                            o27GUID = c.o27GUID
                        });
                    }
                }

                v.SetTagging(Factory.o51TagBL.GetTagging("o23", v.rec_pid));
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            
            RefreshState(v);

            
            foreach(var c in v.lisX16)
            {
                var cc = new DocFieldInput() { x16Field = c.x16Field,x16Name=c.x16Name, x16DataSource=c.x16DataSource,x16IsEntryRequired=c.x16IsEntryRequired,x16IsFixedDataSource=c.x16IsFixedDataSource };
                if (v.Rec != null)  //načtení uživtelských polí dokumentu
                {
                    if (c.x16Field == "o23HtmlContent")
                    {                        
                        if (v.rec_pid>0) v.HtmlContent = Factory.o23DocBL.LoadHtmlEditor(v.rec_pid);
                        v.HtmlName = c.x16Name;
                        v.IsHtmlEditor = true;
                    }
                    else
                    {
                        if (BO.Reflexe.GetPropertyValue(v.Rec, cc.x16Field) != null)
                        {
                            switch (c.FieldType)
                            {
                                case BO.x24IdENUM.tBoolean:
                                    cc.CheckInput = Convert.ToBoolean(BO.Reflexe.GetPropertyValue(v.Rec, cc.x16Field));
                                    break;
                                case BO.x24IdENUM.tDate:
                                case BO.x24IdENUM.tDateTime:
                                    cc.DateInput = Convert.ToDateTime(BO.Reflexe.GetPropertyValue(v.Rec, cc.x16Field));
                                    break;
                                case BO.x24IdENUM.tDecimal:
                                    cc.NumInput = Convert.ToDouble(BO.Reflexe.GetPropertyValue(v.Rec, cc.x16Field));
                                    break;
                                default:
                                    cc.StringInput = Convert.ToString(BO.Reflexe.GetPropertyValue(v.Rec, cc.x16Field));
                                    break;
                            }
                        }
                    }
                    
                    
                }
                v.lisFields.Add(cc);
            }

            var lisX19 = Factory.o23DocBL.GetList_x19(v.rec_pid);
            v.lisX19 = new List<x19Repeator>();
            foreach (var c in lisX19)
            {
                var cc = new x19Repeator() { TempGuid = BO.BAS.GetGuid(), pid = c.pid, x20ID = c.x20ID, x19RecordPID = c.x19RecordPID, x29ID = c.x29ID, SelectedX20Name = c.x20Name };
                var cx20 = Factory.x18EntityCategoryBL.LoadX20(c.x20ID);
                cc.SelectedX20Name = cx20.BindName;
                cc.SelectedBindText = Factory.CBL.GetObjectAlias(cx20.BindPrefix, c.x19RecordPID);
                v.lisX19.Add(cc);
            }

            if (v.Rec.pid>0 && isclone)
            {
                v.MakeClone();
            }

            if (v.Rec != null)
            {
                
            }

            if (v.Rec.pid==0 && prefix != null && recpid > 0)
            {
                //Založení nového dokumentu z konkrétního záznamu entity
                if (v.lisX20.Where(p => p.BindPrefix == prefix).Count() > 0)
                {
                    v.SelectedX20ID = v.lisX20.First(p => p.BindPrefix == prefix).pid;
                    var cx20 = Factory.x18EntityCategoryBL.LoadX20(v.SelectedX20ID);
                    var c = new x19Repeator() { x20ID = v.SelectedX20ID, TempGuid = BO.BAS.GetGuid(), x19RecordPID = recpid, SelectedBindText = Factory.CBL.GetObjectAlias(prefix, recpid), SelectedX20Name = cx20.BindName };
                    v.lisX19.Add(c);
                }


            }

            return View(v);
        }

        private void RefreshState(o23Record v)
        {
            v.RecX18 = Factory.x18EntityCategoryBL.Load(v.x18ID);
            if (v.lisX16 == null)
            {
                v.lisX16 = Factory.x18EntityCategoryBL.GetList_x16(v.x18ID);
            }
            if (v.lisX19 == null)
            {
                v.lisX19 = new List<x19Repeator>();
            }
            if (v.lisX20 == null)
            {
                v.lisX20 = Factory.x18EntityCategoryBL.GetList_x20(v.x18ID).Where(p => p.x20EntryModeFlag == BO.x20EntryModeENUM.Combo);
            }
            if (v.SelectedX20ID==0 && v.lisX20.Count() > 0)
            {
                v.SelectedX20ID = v.lisX20.First().pid;
                Handle_ChangeX20ID(v, v.lisX20.First());                
            }
            if (v.lisO27 == null)
            {
                v.lisO27 = new List<o27Repeator>();
            }
            if (v.lisFields == null)
            {
                v.lisFields = new List<DocFieldInput>();
            }
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.o23Record v,string oper,string guid)
        {
            RefreshState(v);

            if (oper == "x20id")
            {
                var cx20 = Factory.x18EntityCategoryBL.LoadX20(v.SelectedX20ID);
                Handle_ChangeX20ID(v, cx20);
                v.IsAutoCollapseX20 = true;
                return View(v);
            }
            if (oper== "bindrec")
            {
                v.IsAutoCollapseX20 = true;
                if (v.lisX19.Any(p=>p.x20ID==v.SelectedX20ID && p.x19RecordPID == v.SelectedBindPid && !p.IsTempDeleted))
                {
                    this.AddMessage("Tato vazba již existuje.");return View(v);
                }                
                var c = new x19Repeator() { x20ID = v.SelectedX20ID, TempGuid = BO.BAS.GetGuid(), x19RecordPID = v.SelectedBindPid, SelectedBindText = v.SelectedBindText, SelectedX20Name = v.SelectedBindName };
                var cx20 = Factory.x18EntityCategoryBL.LoadX20(v.SelectedX20ID);
                if (!cx20.x20IsMultiSelect)
                {
                    //není povolen vícenásobný výběr entity
                    if (v.lisX19.Any(p => p.x20ID == v.SelectedX20ID && !p.IsTempDeleted))
                    {
                        c = v.lisX19.First(p => p.x20ID == v.SelectedX20ID && !p.IsTempDeleted);
                        c.x19RecordPID = v.SelectedBindPid;
                        c.SelectedBindText = v.SelectedBindText;
                        return View(v);
                    }
                }
                v.lisX19.Add(c);                
                return View(v);
            }
            if (oper== "delete_x19")
            {
                v.lisX19.First(p => p.TempGuid == guid).IsTempDeleted = true;
                v.IsAutoCollapseX20 = true;
                return View(v);
            }
            if (oper == "delete_o27")
            {
                v.lisO27.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper == "postback")
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {
                BO.o23Doc c = new BO.o23Doc();
                if (v.rec_pid > 0) c = Factory.o23DocBL.Load(v.rec_pid);
                c.o23Name = v.Rec.o23Name;
                c.o23Code = v.Rec.o23Code;

                foreach(var cc in v.lisFields)
                {
                    if (cc.x16Field== "o23HtmlContent")
                    {
                        //ukládá se do jiné tabulky
                    }
                    else
                    {
                        switch (cc.FieldType)
                        {
                            case BO.x24IdENUM.tBoolean:
                                BO.Reflexe.SetPropertyValue(c, cc.x16Field, cc.CheckInput);
                                break;
                            case BO.x24IdENUM.tDate:
                            case BO.x24IdENUM.tDateTime:
                                BO.Reflexe.SetPropertyValue(c, cc.x16Field, cc.DateInput);
                                if (cc.x16IsEntryRequired && cc.DateInput == null)
                                {
                                    this.AddMessageTranslated(Factory.tra(string.Format("Pole [{0}] je povinné k vyplnění.", cc.x16Name))); return View(v);
                                }
                                break;
                            case BO.x24IdENUM.tDecimal:
                                BO.Reflexe.SetPropertyValue(c, cc.x16Field, cc.NumInput);
                                if (cc.x16IsEntryRequired && cc.NumInput == 0)
                                {
                                    this.AddMessageTranslated(Factory.tra(string.Format("Pole [{0}] je povinné k vyplnění.", cc.x16Name))); return View(v);
                                }
                                break;
                            default:
                                BO.Reflexe.SetPropertyValue(c, cc.x16Field, cc.StringInput);
                                if (cc.x16IsEntryRequired && string.IsNullOrEmpty(cc.StringInput))
                                {
                                    this.AddMessageTranslated(Factory.tra(string.Format("Pole [{0}] je povinné k vyplnění.", cc.x16Name))); return View(v);
                                }
                                break;
                        }
                    }
                    
                    
                }

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                var lisX19 = new List<BO.x19EntityCategory_Binding>();
                foreach (var cc in v.lisX19)
                {
                    lisX19.Add(new BO.x19EntityCategory_Binding()
                    {
                        IsSetAsDeleted = cc.IsTempDeleted,
                        pid = cc.pid,
                        x20ID = cc.x20ID,
                        x19RecordPID=cc.x19RecordPID,                                                
                    });
                }
                

                c.pid = Factory.o23DocBL.Save(c,v.x18ID, lisX19,v.UploadGuid, v.lisO27.Where(p => p.IsTempDeleted).Select(p => p.pid).ToList());
                if (c.pid > 0)
                {
                    if (v.IsHtmlEditor)
                    {
                        Factory.o23DocBL.SaveHtmlEditor(c.pid,v.HtmlContent);
                    }
                    Factory.o51TagBL.SaveTagging("o23", c.pid, v.TagPids);

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void Handle_ChangeX20ID(Models.Record.o23Record v, BO.x20EntiyToCategory recX20)
        {
            v.SelectedBindName = recX20.BindName;
            v.SelectedBindEntity = BO.BASX29.GetEntity(recX20.BindPrefix);
            v.SelectedBindPid = 0;
            v.SelectedBindText = null;
        }

        
    }
}
