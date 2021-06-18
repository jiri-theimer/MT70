using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Controllers
{
    public class TheGridReportController : BaseController
    {
        private readonly BL.TheColumnsProvider _colsProvider;
        private readonly BL.ThePeriodProvider _pp;

        public TheGridReportController(BL.TheColumnsProvider cp, BL.ThePeriodProvider pp)
        {
            _colsProvider = cp;
            _pp = pp;
        }
        public IActionResult Index(int j72id,string pids,string master_prefix,int master_pid)
        {            
            var v = new TheGridReportViewModel() { j72id = j72id,master_prefix=master_prefix,master_pid=master_pid };

            var gridState = Factory.j72TheGridTemplateBL.LoadState(j72id, Factory.CurrentUser.pid);
            
            v.prefix = gridState.j72Entity.Substring(0, 3);

            if (!string.IsNullOrEmpty(pids))
            {
                v.pids = BO.BAS.ConvertString2ListInt(pids);
            }

            string myqueryinline = null;
            if (string.IsNullOrEmpty(v.master_prefix) && (v.prefix == "p31" || v.prefix == "j02"))
            {
                var tab = Factory.CBL.LoadUserParam("overgrid-tab-" + v.prefix, "zero", 1);
                if (tab != null && tab != "zero")
                {
                    myqueryinline = "tabquery|string|" + tab;
                }
            }
            
            
            var mq= new BO.InitMyQuery().Load(gridState.j72Entity,v.master_prefix,v.master_pid,myqueryinline);
            mq.IsRecordValid = null;    //brát v potaz i archivované záznamy
            mq.SetPids(pids);
            mq.explicit_columns = _colsProvider.ParseTheGridColumns(v.prefix, gridState.j72Columns, Factory);

            if (string.IsNullOrEmpty(v.master_prefix))
            {
                mq.p31statequery = Factory.CBL.LoadUserParamInt("grid-" + v.prefix + "-p31statequery", 0);
            }
            else
            {
                mq.p31statequery = Factory.CBL.LoadUserParamInt("grid-" + v.prefix + "-" +BO.BASX29.GetEntity(v.master_prefix) + "-p31statequery", 0);
            }
            



            var dt = Factory.gridBL.GetList(mq, false);

            this.AddMessageTranslated(dt.Rows.Count.ToString());

            //TheGridInput input = new TheGridInput() { j72id = v.j72id, entity = gridState.j72Entity };
            
            //var cSup = new UI.TheGridSupport(input, Factory, _colsProvider);

            return View(v);
        }
    }
}
