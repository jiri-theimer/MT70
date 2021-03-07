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
        public IActionResult Record(int pid, bool isclone,string prefix)
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
                v.ComboX27Name = v.Rec.x27Name;
                v.SelectedJ04IDs = v.Rec.x28NotPublic_j04IDs;                
                if (v.SelectedJ04IDs != null)
                {
                    var mq = new BO.myQueryJ04() { pids = BO.BAS.ConvertString2ListInt(v.SelectedJ04IDs) };
                    v.SelectedJ04Names = string.Join(",", Factory.j04UserRoleBL.GetList(mq).Select(p => p.j04Name));
                }
                v.SelectedJ07IDs = v.Rec.x28NotPublic_j07IDs;
                if (v.SelectedJ07IDs != null)
                {
                    var mq = new BO.myQuery("j07") { pids = BO.BAS.ConvertString2ListInt(v.SelectedJ07IDs) };
                    v.SelectedJ07Names = string.Join(",", Factory.j07PersonPositionBL.GetList(mq).Select(p => p.j07Name));
                }
                

            }
            else
            {
                v.Rec.x28IsAllEntityTypes = true;
                v.Rec.x28Flag = BO.x28FlagENUM.UserField;
                if (prefix != null)
                {
                    v.Rec.x29ID = BO.BASX29.GetEnum(prefix);
                }
                else
                {
                    v.Rec.x29ID=BO.x29IdEnum.p41Project;
                }
                
                v.Rec.x24ID = BO.x24IdENUM.tString;
            }
            RefreshState(v);
            if (v.rec_pid > 0)
            {
                var lisSavedX26 = Factory.x28EntityFieldBL.GetList_x26(v.rec_pid).ToList();
                foreach(var c in v.lisX26)
                {
                    if (lisSavedX26.Any(p => p.x26EntityTypePID == c.x26EntityTypePID))
                    {
                        c.IsChecked = true;
                        c.x26IsEntryRequired = lisSavedX26.Where(p => p.x26EntityTypePID == c.x26EntityTypePID).First().x26IsEntryRequired;
                    }
                }
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

        private void RefreshState(x28Record v)
        {
            if (v.lisX26 == null)
            {
                v.lisX26 = new List<BO.x26EntityField_Binding>();
                switch (v.Rec.x29ID)
                {
                    case BO.x29IdEnum.p41Project:
                        var lis1 = Factory.p42ProjectTypeBL.GetList(new BO.myQuery("p42"));
                        foreach(var c in lis1)
                        {
                            v.lisX26.Add(new BO.x26EntityField_Binding() { EntityTypeName = c.p42Name+" ("+Factory.tra("Typ projektu")+")", x26EntityTypePID = c.pid,x29ID_EntityType=342 });
                        }
                        break;
                    case BO.x29IdEnum.p31Worksheet:
                        var lis2 = Factory.p34ActivityGroupBL.GetList(new BO.myQueryP34());
                        foreach (var c in lis2)
                        {
                            v.lisX26.Add(new BO.x26EntityField_Binding() { EntityTypeName = c.p34Name + " (" + Factory.tra("Sešit") + ")", x26EntityTypePID = c.pid, x29ID_EntityType=334 });
                        }
                        break;
                    case BO.x29IdEnum.p28Contact:
                        var lis3 = Factory.p29ContactTypeBL.GetList(new BO.myQuery("p29"));
                        foreach (var c in lis3)
                        {
                            v.lisX26.Add(new BO.x26EntityField_Binding() { EntityTypeName = c.p29Name, x26EntityTypePID = c.pid, x29ID_EntityType=329 });
                        }
                        break;
                    case BO.x29IdEnum.j02Person:
                        var lis4 = Factory.j07PersonPositionBL.GetList(new BO.myQuery("j07"));
                        foreach (var c in lis4)
                        {
                            v.lisX26.Add(new BO.x26EntityField_Binding() { EntityTypeName = c.j07Name, x26EntityTypePID = c.pid, x29ID_EntityType = 107 });
                        }
                        break;
                    case BO.x29IdEnum.p91Invoice:
                        break;
                    
                }
            }
            
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
            if (oper == "x29id")
            {
                v.lisX26 = null;
                RefreshState(v);
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
                c.x28NotPublic_j04IDs = v.SelectedJ04IDs;
                c.x28NotPublic_j07IDs = v.SelectedJ07IDs;

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

                c.pid = Factory.x28EntityFieldBL.Save(c,v.lisX26);
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
