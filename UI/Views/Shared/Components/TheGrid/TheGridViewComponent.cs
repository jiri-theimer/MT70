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
                gridState = _f.j72TheGridTemplateBL.LoadState(input.entity, _f.CurrentUser.pid, input.master_entity);  //výchozí, systémový grid: j72IsSystem=1
            }

            if (gridState == null)   //pro uživatele zatím nebyl vygenerován záznam v j72 -> vygenerovat
            {
                var cols = _colsProvider.getDefaultPallete(false, input.query);    //výchozí paleta sloupců

                var recJ72 = new BO.j72TheGridTemplate() { j72IsSystem = true, j72Entity = input.entity, j03ID = _f.CurrentUser.pid, j72Columns = String.Join(",", cols.Select(p => p.UniqueName)), j72MasterEntity = input.master_entity };

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




            var cSup = new UI.TheGridSupport(input,_f, _colsProvider);

            var ret = new TheGridViewModel();
            ret.GridInput = input;
            ret.firstdata = cSup.GetFirstData(gridState);
            
            ret.GridState = gridState;
            if (gridState.j72Columns.Contains("Free"))
            {
                var lisFF = new BL.ffColumnsProvider(_f, input.entity.Substring(0, 3));
                ret.Columns = _colsProvider.ParseTheGridColumns(input.entity.Substring(0, 3), gridState.j72Columns, _f.CurrentUser.j03LangIndex,lisFF.getColumns());
            }
            else
            {
                ret.Columns = _colsProvider.ParseTheGridColumns(input.entity.Substring(0, 3), gridState.j72Columns, _f.CurrentUser.j03LangIndex);
            }
            
            ret.AdhocFilter = _colsProvider.ParseAdhocFilterFromString(gridState.j75Filter, ret.Columns);
           
            
            return View("Default", ret);




        }
    }
}
