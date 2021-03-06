﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using UI.Controllers;
using UI.Models;

namespace UI.Views.Shared.Components.TheGrid
{
    public class TheGridViewComponent : ViewComponent
    {
        BL.Factory _f;
        private readonly BL.TheColumnsProvider _colsProvider;
        //private readonly BL.ThePeriodProvider _pp;
        public TheGridViewComponent(BL.Factory f, BL.TheColumnsProvider cp)
        {
            _f = f;
            _colsProvider = cp;
            //_pp = pp;
        }

        public IViewComponentResult
            Invoke(TheGridInput input)
        {
            if (input.query == null)
            {
                input.query = new BO.InitMyQuery().Load(input.entity);
            }


            BO.TheGridState gridState = null;
            if (input.j72id > 0)
            {
                gridState = _f.j72TheGridTemplateBL.LoadState(input.j72id, _f.CurrentUser.pid);
            }
            if (gridState == null)
            {
                gridState = _f.j72TheGridTemplateBL.LoadState(input.entity, _f.CurrentUser.pid, input.master_entity);  //výchozí, systémový grid: j72IsSystem=1, pokud tedy již existuje
            }

            if (gridState == null)   //pro uživatele zatím nebyl vygenerován záznam v j72 -> vygenerovat první uživatelovo grid pro daný prefix a masterprefix
            {
                //string strJ72Columns = _colsProvider.getDefaultPalletePreSaved(input.entity, input.master_entity, input.query);
                string strJ72Columns = _f.j72TheGridTemplateBL.getDefaultPalletePreSaved(input.entity, input.master_entity, input.query);
                if (strJ72Columns == null)
                {
                    var cols = _colsProvider.getDefaultPallete(false, input.query,_f);    //výchozí paleta sloupců
                    strJ72Columns = String.Join(",", cols.Select(p => p.UniqueName));
                }

                var recJ72 = new BO.j72TheGridTemplate() { j72IsSystem = true, j72Entity = input.entity, j03ID = _f.CurrentUser.pid, j72Columns = strJ72Columns, j72MasterEntity = input.master_entity };

                var intJ72ID = _f.j72TheGridTemplateBL.Save(recJ72, null, null, null);
                gridState = _f.j72TheGridTemplateBL.LoadState(intJ72ID, _f.CurrentUser.pid);
            }
            if (!string.IsNullOrEmpty(input.fixedcolumns))
            {
                gridState.j72Columns = input.fixedcolumns;
            }
            gridState.j75CurrentRecordPid = input.go2pid;
            gridState.j72MasterEntity = input.master_entity;
            gridState.j75CurrentPagerIndex = 0; //na úvodní zobrazení vždy začínat grid na první stránce!


            

            var cSup = new UI.TheGridSupport(input, _f, _colsProvider);

            var ret = new TheGridViewModel();
            ret.GridInput = input;
            ret.firstdata = cSup.GetFirstData(gridState);

            ret.GridState = gridState;
            ret.Columns = _colsProvider.ParseTheGridColumns(input.entity.Substring(0, 3), gridState.j72Columns, _f);

           

            if (!input.isrecpagegrid)    //pokud není vypnutý sloupcový filtr
            {
                ret.AdhocFilter = _colsProvider.ParseAdhocFilterFromString(gridState.j75Filter, ret.Columns);
            }
            
            if (_f.CurrentUser.p07LevelsCount > 0)
            {               
                foreach(var c in ret.Columns)
                {
                    if (c.Header == "L1") c.Header = _f.CurrentUser.getP07Level(1, true);
                    if (c.Header == "L2") c.Header = _f.CurrentUser.getP07Level(2, true);
                    if (c.Header == "L3") c.Header = _f.CurrentUser.getP07Level(3, true);
                    if (c.Header == "L4") c.Header = _f.CurrentUser.getP07Level(4, true);
                    if (c.Header == "L5") c.Header = _f.CurrentUser.getP07Level(5, true);
                }
            }
            else
            {
                foreach (var c in ret.Columns)
                {                    
                    if (c.Header == "L5") c.Header = _f.CurrentUser.getP07Level(5, true);
                }
            }
            

            return View("Default", ret);




        }
    }
}
