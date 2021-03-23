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
                var lis1 = Factory.x18EntityCategoryBL.GetList_x16(v.rec_pid);
                v.lisX16 = new List<x16Repeater>();
                foreach (var c in lis1)
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
                var lis2 = Factory.x18EntityCategoryBL.GetList_x20(new List<int>() { v.rec_pid });
                v.lisX20 = new List<x20Repeater>();
                foreach(var c in lis2)
                {
                    var cc = new x20Repeater()
                    {
                        TempGuid = BO.BAS.GetGuid(),
                        x20Name=c.x20Name,
                        x20Ordinary=c.x20Ordinary,
                        x29ID=c.x29ID,
                        x29ID_EntityType=c.x29ID_EntityType,
                        x20IsEntryRequired=c.x20IsEntryRequired,
                        x20IsClosed=c.x20IsClosed
                    };
                    v.lisX20.Add(cc);
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
            if (v.lisX20 == null)
            {
                v.lisX20 = new List<x20Repeater>();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]        
        public IActionResult Record(x18Record v,string oper, string guid)
        {
            RefreshState(v);

            switch (oper)
            {
                case "postback":
                    return View(v);
                case "x20_add_row":
                    var cX20 = new x20Repeater() {x29ID=(int)v.SelectedX29ID, TempGuid = BO.BAS.GetGuid() };
                    v.lisX20.Add(cX20);
                    return View(v);
                case "x16_add_row":
                    var cX16 = new x16Repeater() { TempGuid = BO.BAS.GetGuid() };
                    v.lisX16.Add(cX16);
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
                if (c.x18UploadFlag == BO.x18UploadENUM.FileSystemUpload)
                {
                    c.x18MaxOneFileSize = v.Rec.x18MaxOneFileSize;
                    c.x18AllowedFileExtensions = v.Rec.x18AllowedFileExtensions;
                }
                c.x18IsColors = v.Rec.x18IsColors;
                c.x18IsAllowEncryption = v.Rec.x18IsAllowEncryption;
                c.x18ReportCodes = v.Rec.x18ReportCodes;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                var lisX16 = new List<BO.x16EntityCategory_FieldSetting>();
                foreach (var row in v.lisX16.Where(p => p.IsTempDeleted == false))
                {
                    var cc = new BO.x16EntityCategory_FieldSetting() { x16Field = row.x16Field,x16IsGridField=row.x16IsGridField,x16NameGrid=row.x16NameGrid,
                        x16Ordinary=row.x16Ordinary,x16IsEntryRequired=row.x16IsEntryRequired,x16DataSource=row.x16DataSource,x16IsFixedDataSource=row.x16IsFixedDataSource,
                        x16TextboxHeight=row.x16TextboxHeight,x16Format=row.x16Format
                    };
                    lisX16.Add(cc);
                }
                var lisX20 = new List<BO.x20EntiyToCategory>();
                foreach(var row in v.lisX20.Where(p => p.IsTempDeleted == false))
                {
                    var cc = new BO.x20EntiyToCategory() { x20Name = row.x20Name, x20Ordinary = row.x20Ordinary, x29ID = row.x29ID, x29ID_EntityType = row.x29ID_EntityType, x20IsEntryRequired = row.x20IsEntryRequired, x20IsClosed = row.x20IsClosed };
                    lisX20.Add(cc);
                }
                
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
