using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class o53Controller : BaseController
        
    {
        private readonly BL.TheColumnsProvider _cp;
        public o53Controller(BL.TheColumnsProvider cp)
        {
            _cp = cp;
        }
        ///KATEGORIE
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new o53Record() { rec_pid = pid, rec_entity = "o53" };
            v.Rec = new  BO.o53TagGroup();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.o53TagGroupBL.Load(v.rec_pid);
                v.SelectedEntities = BO.BAS.ConvertString2ListInt(v.Rec.x29IDs);
                

            }
            else
            {                
                v.Rec.o53IsMultiSelect = true;
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);

            
            v.ApplicableEntities = GetApplicableEntities();
           
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v,BO.x53PermValEnum.GR_O51_Admin);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(o53Record v)
        {
            if (ModelState.IsValid)
            {
                BO.o53TagGroup c = new BO.o53TagGroup();
                if (v.rec_pid > 0) c = Factory.o53TagGroupBL.Load(v.rec_pid);

                
                c.o53Name = v.Rec.o53Name;               
                c.x29IDs = String.Join(",", v.SelectedEntities.Where(p=>p>0));
                c.o53IsMultiSelect = v.Rec.o53IsMultiSelect;
                c.o53Ordinary = v.Rec.o53Ordinary;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.o53TagGroupBL.Save(c);
                if (c.pid > 0)
                {
                    _cp.Refresh();   //obnovit názvy sloupců kategorií
                   
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }
                            
            }
            
            
            v.ApplicableEntities = GetApplicableEntities();
            this.Notify_RecNotSaved();
            return View(v);
            
        }


        private List<BO.TheEntity> GetApplicableEntities()
        {
            var lis = new List<BO.TheEntity>();
            lis.Add(Factory.EProvider.ByPrefix("p41"));
            lis.Add(Factory.EProvider.ByPrefix("p28"));
            lis.Add(Factory.EProvider.ByPrefix("j02"));
            lis.Add(Factory.EProvider.ByPrefix("p31"));
            lis.Add(Factory.EProvider.ByPrefix("o23"));
            lis.Add(Factory.EProvider.ByPrefix("p91"));
            lis.Add(Factory.EProvider.ByPrefix("p90"));

            return lis;
        }



    }
}