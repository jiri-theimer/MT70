using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class x18Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new x18Record() { rec_pid = pid, rec_entity = "x18" };
            v.Rec = new BO.x18EntityCategory();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x18EntityCategoryBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var lis = Factory.x18EntityCategoryBL.GetList_x16(v.rec_pid);
                v.lisX16 = new List<x16Repeater>();
                foreach (var c in lis)
                {
                    var cc = new x16Repeater()
                    {
                        TempGuid = BO.BAS.GetGuid(),
                        x16IsEntryRequired = c.x16IsEntryRequired,
                        x16Name = c.x16Name,
                        x16Field = c.x16Field,
                        x16Ordinary = c.x16Ordinary,
                        x16DataSource = c.x16DataSource,
                        x16IsFixedDataSource = c.x16IsFixedDataSource,
                        x16IsGridField = c.x16IsGridField,
                        x16TextboxHeight = c.x16TextboxHeight,
                        x16Format = c.x16Format
                    };
                    v.lisX16.Add(cc);
                }

            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }
        private void RefreshState(x18Record v)
        {
            if (v.lisX16 == null)
            {
                v.lisX16 = new List<x16Repeater>();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]        
        public IActionResult Record(x18Record v,string oper, string guid)
        {
            switch (oper)
            {
                case "postback":
                    return View(v);
                case "x16_add_row":
                    var c = new x16Repeater() { TempGuid = BO.BAS.GetGuid() };                                       
                    v.lisX16.Add(c);
                    return View(v);
                case "x16_delete_row":
                    v.lisX16.First(p => p.TempGuid == guid).IsTempDeleted = true;
                    return View(v);
            }
           
            if (ModelState.IsValid)
            {
                BO.x18EntityCategory c = new BO.x18EntityCategory();
                if (v.rec_pid > 0) c = Factory.x18EntityCategoryBL.Load(v.rec_pid);
                c.x18Name = v.Rec.x18Name;
                c.x18NameShort = v.Rec.x18NameShort;
                c.x18Ordinary = v.Rec.x18Ordinary;
                c.x18EntryCodeFlag = v.Rec.x18EntryCodeFlag;
                c.x18EntryNameFlag = v.Rec.x18EntryNameFlag;
                c.x18UploadFlag = v.Rec.x18UploadFlag;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                var lisX16 = new List<BO.x16EntityCategory_FieldSetting>();
                var lisX20 = new List<BO.x20EntiyToCategory>();
                
                c.pid = Factory.x18EntityCategoryBL.Save(c,lisX20,lisX16);
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
