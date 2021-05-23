using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class x67Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone,string prefix)
        {
            var v = new x67Record() { rec_pid = pid, rec_entity = "x67" };            
            v.Rec = new BO.x67EntityRole();
            
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x67EntityRoleBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                
                v.SelectedX53IDs = Factory.x67EntityRoleBL.GetList_BoundX53(v.rec_pid).Select(p => p.x53ID).ToList();
                prefix = BO.BASX29.GetPrefix(v.Rec.x29ID);
                
            }
            if (prefix == null)
            {
                return this.StopPage(true, "prefix missing");
            }
            v.Rec.x29ID = BO.BASX29.GetEnum(prefix);
            v.x29ID = (int)v.Rec.x29ID;
            RefreshState(v);
            if (prefix == "p41")
            {
                SetupO28List(v);
            }
            

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

        private void RefreshState(x67Record v)
        {
            v.lisAllX53 = Factory.FBL.GetListX53().Where(p => p.x29ID == v.Rec.x29ID);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(x67Record v)
        {
            RefreshState(v);

            if (ModelState.IsValid)
            {
                BO.x67EntityRole c = new BO.x67EntityRole();
                if (v.rec_pid > 0) c = Factory.x67EntityRoleBL.Load(v.rec_pid);
                c.x29ID = (BO.x29IdEnum) v.x29ID;
                c.x67Name = v.Rec.x67Name;
                c.x67Ordinary = v.Rec.x67Ordinary;
                

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.x67EntityRoleBL.Save(c,v.SelectedX53IDs);
                if (c.pid > 0)
                {
                    if (c.x29ID == BO.x29IdEnum.p41Project)
                    {
                        Factory.x67EntityRoleBL.SaveO28(c.pid, v.lisO28);
                    }
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }
                
            }


            this.Notify_RecNotSaved();
            return View(v);
        }


        private void SetupO28List(x67Record v)
        {
            v.lisO28 = new List<BO.o28ProjectRole_Workload>();
            var lisP34 = Factory.p34ActivityGroupBL.GetList(new BO.myQueryP34());

            foreach (var recP34 in lisP34)
            {
                var c = new BO.o28ProjectRole_Workload() { p34ID = recP34.pid, p34Name = recP34.p34Name };

                v.lisO28.Add(c);
            }

            if (v.rec_pid > 0)
            {
                var lisO28 = Factory.x67EntityRoleBL.GetListO28(v.rec_pid);
                foreach (var c in lisO28)
                {
                    if (v.lisO28.Where(p => p.p34ID == c.p34ID).Any())
                    {
                        var rec = v.lisO28.Where(p => p.p34ID == c.p34ID).First();

                        rec.o28EntryFlag = c.o28EntryFlag;
                        rec.o28PermFlag = c.o28PermFlag;
                    }
                }
            }

        }

    }
}
