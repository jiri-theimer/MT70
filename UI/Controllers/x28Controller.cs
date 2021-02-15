using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class x28Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new x28Record() { rec_pid = pid, rec_entity = "x28" };
            v.Rec = new BO.x28EntityField();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x28EntityFieldBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }


            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

        private void RefreshState(x28Record v)
        {
            

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(x28Record v,string oper)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.x28EntityField c = new BO.x28EntityField();
                if (v.rec_pid > 0) c = Factory.x28EntityFieldBL.Load(v.rec_pid);
                c.x28Name = v.Rec.x28Name;                
                c.x24ID = v.Rec.x24ID;
                c.x29ID = v.Rec.x29ID;
                c.x23ID = v.Rec.x23ID;                
                c.x27ID = v.Rec.x27ID;

                c.x28Ordinary = v.Rec.x28Ordinary;
                c.x28Flag = v.Rec.x28Flag;
                c.x28DataSource = v.Rec.x28DataSource;
                c.x28IsFixedDataSource = v.Rec.x28IsFixedDataSource;
                c.x28IsRequired = v.Rec.x28IsRequired;
                
                c.x28IsAllEntityTypes = v.Rec.x28IsAllEntityTypes;
                c.x28IsPublic = v.Rec.x28IsPublic;
                c.x28NotPublic_j04IDs = v.Rec.x28NotPublic_j04IDs;
                c.x28NotPublic_j07IDs = v.Rec.x28NotPublic_j07IDs;

                c.x28Grid_Field = v.Rec.x28Grid_Field;
                c.x28Grid_SqlSyntax = v.Rec.x28Grid_SqlSyntax;
                c.x28Grid_SqlFrom = v.Rec.x28Grid_SqlFrom;
                //c.x28Pivot_SelectSql = v.Rec.x28Pivot_SelectSql;
                //c.x28Pivot_GroupBySql = v.Rec.x28Pivot_GroupBySql;
                //c.x28Query_SqlSyntax = v.Rec.x28Query_SqlSyntax;
                //c.x28Query_Field = v.Rec.x28Query_Field;
                //c.x28Query_sqlComboSource = v.Rec.x28Query_sqlComboSource;

                c.x28TextboxHeight = v.Rec.x28TextboxHeight;
                
                c.x28HelpText = v.Rec.x28HelpText;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.x28EntityFieldBL.Save(c);
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
