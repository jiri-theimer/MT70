using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class o23Controller : BaseController
    {
        public IActionResult SelectDocType()
        {
            var v = new SelectDocTypeViewModel();
            v.lisX18 = Factory.x18EntityCategoryBL.GetList(new BO.myQuery("x18")).OrderBy(p=>p.x18Ordinary);
            return View(v);
        }
        public IActionResult Record(int pid, bool isclone,int x18id)
        {
            var v = new o23Record() { rec_pid = pid, rec_entity = "o23",x18ID=x18id };
            if (v.x18ID == 0)
            {
                return RedirectToAction("SelectDocType");
            }
            v.Rec = new BO.o23Doc();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.o23DocBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            RefreshState(v);

            v.lisFields = new List<DocFieldInput>();
            foreach(var c in v.lisX16)
            {
                var cc = new DocFieldInput() { x16Field = c.x16Field,x16Name=c.x16Name, x16DataSource=c.x16DataSource,x16IsEntryRequired=c.x16IsEntryRequired,x16IsFixedDataSource=c.x16IsFixedDataSource };
                v.lisFields.Add(cc);
            }

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

        private void RefreshState(o23Record v)
        {
            v.RecX18 = Factory.x18EntityCategoryBL.Load(v.x18ID);
            if (v.lisX16 == null)
            {
                v.lisX16 = Factory.x18EntityCategoryBL.GetList_x16(v.x18ID);
            }
            if (v.lisX20 == null)
            {
                v.lisX20 = Factory.x18EntityCategoryBL.GetList_x20(v.x18ID);
            }
            if (v.SelectedX20ID==0 && v.lisX20.Count() > 0)
            {
                v.SelectedX20ID = v.lisX20.First().pid;
                Handle_ChangeX20ID(v, v.lisX20.First());                
            }

        }

        private void Handle_ChangeX20ID(Models.Record.o23Record v,BO.x20EntiyToCategory recX20)
        {
            v.SelectedBindName = recX20.BindName;
            v.SelectedBindEntity = BO.BASX29.GetEntity(recX20.BindPrefix);
            v.SelectedBindPid = 0;
            v.SelectedBindText = null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.o23Record v,string oper)
        {
            RefreshState(v);

            if (oper == "x20id")
            {
                var cx20 = v.lisX20.Where(p => p.pid == v.SelectedX20ID).First();
                Handle_ChangeX20ID(v, cx20);
                return View(v);
            }
            if (oper== "bindrec")
            {
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
                                this.AddMessageTranslated(Factory.tra(string.Format("Pole [{0}] je povinné k vyplnění.", cc.x16Name)));return View(v);
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

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.o23DocBL.Save(c,v.x18ID,v.lisX19);
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
