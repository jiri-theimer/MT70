using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class p61Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new p61Record() { rec_pid = pid, rec_entity = "p61" };
            v.Rec = new BO.p61ActivityCluster();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p61ActivityClusterBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }


                v.p32IDs = string.Join(",", Factory.p32ActivityBL.GetList(new BO.myQueryP32() { p61id = v.rec_pid }).Select(p => p.pid));
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            RefreshState(v);

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.p61Record v, string oper, string pids, string prefix)
        {
            var lisp32IDs = BO.BAS.ConvertString2ListInt(v.p32IDs);
            if (oper == "add" && prefix == "p32")
            {
                lisp32IDs.AddRange(BO.BAS.ConvertString2ListInt(pids));
            }
            if (oper == "add" && prefix == "p34")
            {
                var lis = Factory.p32ActivityBL.GetList(new BO.myQueryP32() { p34id = BO.BAS.InInt(pids) });
                lisp32IDs.AddRange(lis.Select(p => p.pid).ToList());
            }
            if (oper == "add" && prefix == "p38")
            {
                var lis = Factory.p32ActivityBL.GetList(new BO.myQueryP32() { p38id = BO.BAS.InInt(pids) });
                lisp32IDs.AddRange(lis.Select(p => p.pid).ToList());
            }
            if (oper == "remove" && prefix == "p32")
            {
                foreach (int x in BO.BAS.ConvertString2ListInt(pids))
                {
                    lisp32IDs.Remove(x);

                }
            }

            if (oper != null)
            {
                v.p32IDs = string.Join(",", lisp32IDs);
                RefreshState(v);
                return View(v);
            }

            RefreshState(v);

            if (ModelState.IsValid)
            {
                BO.p61ActivityCluster c = new BO.p61ActivityCluster();
                if (v.rec_pid > 0) c = Factory.p61ActivityClusterBL.Load(v.rec_pid);
                c.p61Name = v.Rec.p61Name;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                List<int> p32IDs = BO.BAS.ConvertString2ListInt(v.p32IDs);
                c.pid = Factory.p61ActivityClusterBL.Save(c, p32IDs);
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshState(Models.Record.p61Record v)
        {
            string strMyQuery = "pids@list_int@-1";
            if (!string.IsNullOrEmpty(v.p32IDs))
            {
                strMyQuery = "pids@list_int@" + v.p32IDs;
            }
            v.gridinput = new TheGridInput() { entity = "p32Activity", master_entity = "inform", myqueryinline = strMyQuery, oncmclick = "", ondblclick = "" };
            v.gridinput.query = new BO.InitMyQuery().Load("p32", null, 0, strMyQuery);


        }

       
    }
}
