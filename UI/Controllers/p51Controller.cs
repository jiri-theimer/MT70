using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class p51Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone,bool iscustom,string tempguid)
        {
            var v = new p51Record() { rec_pid = pid, rec_entity = "p51",TempGuid=tempguid };
            v.Rec = new BO.p51PriceList();
            if (v.TempGuid !=null && v.rec_pid == 0)
            {
                //zjistit, zda již neexistuje rozdělaný temp ceník
                var c = Factory.p51PriceListBL.LoadByTempGuid(v.TempGuid);
                if (c !=null) v.rec_pid = c.pid;

            }
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p51PriceListBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboJ27Code = v.Rec.j27Code;

                var lis= Factory.p51PriceListBL.GetList_p52(v.rec_pid).ToList();
                v.lisP52 = new List<p52Repeater>();
                foreach (var c in lis)
                {
                    var cc = new p52Repeater() {
                        TempGuid = BO.BAS.GetGuid(),j02ID=c.j02ID,j07ID=c.j07ID,p32ID=c.p32ID,p34ID=c.p34ID,p52Rate=c.p52Rate
                        ,ComboPerson=c.Person,ComboJ07Name=c.j07Name,ComboP32Name=c.p32Name
                        ,ComboP34Name=c.p34Name
                    };
                    cc.RowPrefixWho = "all";
                    if (c.j02ID > 0)
                    {
                        cc.RowPrefixWho = "j02";
                    }
                    if (c.j07ID > 0)
                    {
                        cc.RowPrefixWho = "j07";
                    }                    
                    if (c.p32ID > 0)
                    {
                        cc.RowPrefixActivity = "p32";
                    }
                    else
                    {
                        cc.RowPrefixActivity = "p34";
                    }                    
                    if (c.p52IsPlusAllTimeSheets)
                    {
                        cc.RowPrefixActivity = "all";
                    }
                    v.lisP52.Add(cc);                    
                }

            }
            else
            {
                //nový záznam
                if (iscustom)
                {
                    v.Rec.p51IsCustomTailor = true;
                    
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

        private void RefreshState(p51Record v)
        {
            if (v.lisP52 == null)
            {
                v.lisP52 = new List<p52Repeater>();
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p51Record v, string oper,string guid)
        {
            RefreshState(v);
            switch (oper)
            {
                case "postback":
                    return View(v);
                case "add_row":
                    var c = new p52Repeater() { TempGuid = BO.BAS.GetGuid(),RowPrefixWho="j07",RowPrefixActivity="p34" };
                    var recP34 = Factory.p34ActivityGroupBL.GetList(new BO.myQueryP34() { ismoneyinput = false }).First();
                    c.p34ID = recP34.pid;c.ComboP34Name = recP34.p34Name;
                    v.lisP52.Add(c);
                    return View(v);
                case "clone_row":
                    var row = v.lisP52.Where(p => p.TempGuid == guid).First();
                    var d = new p52Repeater() { TempGuid = BO.BAS.GetGuid(), RowPrefixWho = row.RowPrefixWho, RowPrefixActivity = row.RowPrefixActivity
                        , j02ID = row.j02ID, j07ID = row.j07ID, p32ID = row.p32ID, p34ID = row.p34ID, p52Rate = row.p52Rate
                        ,ComboJ07Name=row.ComboJ07Name,ComboP34Name=row.ComboP34Name,ComboPerson=row.ComboPerson,ComboP32Name=row.ComboP32Name
                    };
                    v.lisP52.Add(d);
                    return View(v);
                case "delete_row":
                    v.lisP52.First(p => p.TempGuid == guid).IsTempDeleted = true;
                    return View(v);
                case "clear_rows":
                    v.lisP52.Clear();
                    return View(v);
            }
            

            if (ModelState.IsValid)
            {
                BO.p51PriceList c = new BO.p51PriceList();
                if (v.rec_pid > 0) c = Factory.p51PriceListBL.Load(v.rec_pid);

                c.p51TypeFlag = v.Rec.p51TypeFlag;
                c.p51Name = v.Rec.p51Name;
                c.p51IsCustomTailor = v.Rec.p51IsCustomTailor;
                if (c.p51IsCustomTailor && !string.IsNullOrEmpty(v.TempGuid))
                {
                    c.p51Name = v.TempGuid;
                }
                c.j27ID = v.Rec.j27ID;
                c.p51DefaultRateT = v.Rec.p51DefaultRateT;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                var lis = new List<BO.p52PriceList_Item>();
                int x = 0;
                foreach(var row in v.lisP52.Where(p => p.IsTempDeleted==false))
                {
                    x += 1;
                    string msg = "Row index #" + x.ToString() + ": ";
                    var cc = new BO.p52PriceList_Item() { p52Rate = row.p52Rate };
                    if (row.RowPrefixWho == "j02")
                    {
                        cc.j02ID = row.j02ID;
                        if (cc.j02ID == 0)
                        {
                            this.AddMessageTranslated(msg+Factory.tra("Chybí vyplnit osobu."));return View(v);
                        }
                    }
                    if (row.RowPrefixWho == "j07")
                    {
                        cc.j07ID = row.j07ID;
                        if (cc.j07ID == 0)
                        {
                            this.AddMessageTranslated(msg + Factory.tra("Chybí vyplnit pozici.")); return View(v);
                        }
                    }
                    if (row.RowPrefixActivity == "p32")
                    {
                        cc.p32ID = row.p32ID;
                        if (cc.p32ID == 0)
                        {
                            this.AddMessageTranslated(msg+Factory.tra("Chybí vyplnit aktivitu.")); return View(v);
                        }
                        cc.p34ID = Factory.p32ActivityBL.Load(cc.p32ID).p34ID;
                    }
                    if (row.RowPrefixActivity == "p34")
                    {
                        cc.p34ID = row.p34ID;
                        if (cc.p34ID == 0)
                        {
                            this.AddMessageTranslated(msg+Factory.tra("Chybí vyplnit sešit.")); return View(v);
                        }
                    }
                    if (row.RowPrefixActivity == "all")
                    {
                        cc.p52IsPlusAllTimeSheets = true;
                        cc.p34ID = Factory.p34ActivityGroupBL.GetList(new BO.myQueryP34() { ismoneyinput = false }).First().pid;
                    }
                    
                    lis.Add(cc);
                }
                c.pid = Factory.p51PriceListBL.Save(c, lis);

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
