﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using UI.Models;


namespace UI.Controllers
{
    public class TheGridController : BaseController        
    {              
        private readonly BL.TheColumnsProvider _colsProvider;
        private readonly BL.ThePeriodProvider _pp;

        public TheGridController(BL.TheColumnsProvider cp, BL.ThePeriodProvider pp)
        {
            _colsProvider = cp;
            _pp = pp;
        }

        //-----------Začátek GRID událostí-------------
        public TheGridOutput HandleTheGridFilter(TheGridUIContext tgi,List<BO.StringPair> pathpars, List<BO.TheGridColumnFilter> filter) //TheGrid povinná metoda: sloupcový filtr
        {
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity, tgi.master_pid, tgi.myqueryinline,tgi.isperiodovergrid);
            v.gridinput.ondblclick = tgi.ondblclick;
            v.gridinput.oncmclick = tgi.oncmclick;
            v.gridinput.fixedcolumns = tgi.fixedcolumns;
            var c = new UI.TheGridSupport(v.gridinput, Factory, _colsProvider);

            return c.Event_HandleTheGridFilter(tgi, filter);
        }
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi, List<BO.StringPair> pathpars)    //TheGrid povinná metoda: změna třídění, pageindex, změna stránky
        {
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity, tgi.master_pid, tgi.myqueryinline,tgi.isperiodovergrid);
            v.gridinput.ondblclick = tgi.ondblclick;
            v.gridinput.oncmclick = tgi.oncmclick;
            v.gridinput.fixedcolumns = tgi.fixedcolumns;            
            var c = new UI.TheGridSupport(v.gridinput, Factory, _colsProvider);
            
            return c.Event_HandleTheGridOper(tgi);

        }
        public string HandleTheGridMenu(TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda: zobrazení grid menu
        {
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity, tgi.master_pid, tgi.myqueryinline,tgi.isperiodovergrid);
            v.gridinput.ondblclick = tgi.ondblclick;
            v.gridinput.oncmclick = tgi.oncmclick;
            v.gridinput.fixedcolumns = tgi.fixedcolumns;
            var c = new UI.TheGridSupport(v.gridinput, Factory, _colsProvider);
            return c.Event_HandleTheGridMenu(tgi.j72id);
            
        }

        public TheGridExportedFile HandleTheGridExport(string format, string pids, TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda pro export dat
        {
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity, tgi.master_pid, tgi.myqueryinline,tgi.isperiodovergrid);
            v.gridinput.ondblclick = tgi.ondblclick;
            v.gridinput.oncmclick = tgi.oncmclick;
            v.gridinput.fixedcolumns = tgi.fixedcolumns;
            var c = new UI.TheGridSupport(v.gridinput, Factory, _colsProvider);
            return c.Event_HandleTheGridExport(format, tgi.j72id, pids);
        }        
        //-----------Konec GRID událostí-------------

        public IActionResult FlatView(string prefix, int go2pid, string myqueryinline,string tab)    //pouze grid bez subform
        {
            if (!TestGridPermissions(prefix))
            {
                return this.StopPage(false, "Nemáte oprávnění pro tento GRID přehled.");
            }
            if (go2pid == 0 && (prefix=="p28" || prefix=="p91" || prefix=="p41" || prefix=="j02" || prefix == "o23" || prefix=="p56" || prefix=="p90"))
            {
                go2pid = LoadLastUsedPid(prefix);
            }
            
            if (tab==null && (prefix == "p31" || prefix == "j02"))
            {
                tab = Factory.CBL.LoadUserParam("overgrid-tab-" + prefix,"zero",1);
                if (tab != null && tab != "zero")
                {
                    myqueryinline = "tabquery|string|" + tab;
                }
            }
            
            FsmViewModel v = LoadFsmViewModel(prefix, go2pid,"flatview",null,0, myqueryinline, TestIfPeriodOverGrid(prefix));
            
            v.gridinput.j72id = Factory.CBL.LoadUserParamInt("flatview-j72id-" + prefix);

            var cTabs = new NavTabsSupport(Factory);
            v.OverGridTabs = cTabs.getOverGridTabs(v.prefix, tab,true);
            

            return View(v);
        }

        

        public IActionResult MasterView(string prefix,int go2pid,string myqueryinline, string tab)    //grid horní + spodní panel, není zde filtrovací pruh s fixním filtrem
        {            
            if (!TestGridPermissions(prefix))
            {
                return this.StopPage(false, "Nemáte oprávnění pro tento GRID přehled.");
            }
            if (go2pid == 0)
            {
                go2pid = LoadLastUsedPid(prefix);
            }
            if (tab == null && (prefix == "j02"))
            {
                tab = Factory.CBL.LoadUserParam("overgrid-tab-" + prefix, "zero", 1);
                if (tab != null && tab != "zero")
                {
                    myqueryinline = "tabquery|string|" + tab;
                }
            }
            FsmViewModel v = LoadFsmViewModel(prefix, go2pid,"masterview",null,0, myqueryinline, TestIfPeriodOverGrid(prefix));
                      
            v.gridinput.j72id = Factory.CBL.LoadUserParamInt("masterview-j72id-" + prefix);

            var cTabs = new NavTabsSupport(Factory);
            v.OverGridTabs = cTabs.getOverGridTabs(v.prefix, tab,false);
            v.NavTabs = cTabs.getMasterTabs(v.prefix, go2pid,false);

          
            string strDefTab = Factory.CBL.LoadUserParam("masterview-tab-" + prefix);
            NavTab deftab = v.NavTabs[0];
            
            foreach (var ntab in v.NavTabs)
            {
               if (!ntab.Url.Contains("?pid"))
                {
                    ntab.Url += "&master_entity=" + v.entity + "&master_pid=" + AppendPid2Url(v.gridinput.go2pid);
                   
                }
                ntab.Url += "&caller=grid";
             
                if (strDefTab !="" && ntab.Entity== strDefTab)
                {
                    deftab = ntab;  //uživatelem naposledy vybraná záložka
                }
            }
            deftab.CssClass += " active";
            if (go2pid > 0)
            {
                v.go2pid_url_in_iframe = deftab.Url;
                
            }

            
            return View(v);
        }

        private string AppendPid2Url(int go2pid)
        {
            if (go2pid > 0)
            {
                return go2pid.ToString();
            }
            else
            {
                return  "@pid";
            }
        }
        private bool TestIfPeriodOverGrid(string prefix)
        {
            return true;    //filtrování období mají všechny entity
            //if (prefix == "p31" || prefix == "p41" || prefix=="p28" || prefix == "p91" || prefix=="j02" || prefix == "p56" || prefix == "p90")
            //{
            //    return true;
            //}
            //return false;
        }
        public IActionResult SlaveView(string master_entity,int master_pid, string prefix, int go2pid,string myqueryinline,string caller)    //podřízený subform v rámci MasterView
        {            
            FsmViewModel v = LoadFsmViewModel(prefix, go2pid, "slaveview", master_entity, master_pid, myqueryinline, TestIfPeriodOverGrid(prefix));
            

            v.gridinput.j72id = Factory.CBL.LoadUserParamInt("slaveview-j72id-" + prefix + "-" + master_entity);

            if (String.IsNullOrEmpty(master_entity) || master_pid == 0)
            {
                AddMessage("Musíte vybrat záznam z nadřízeného panelu.");
            }

            if (caller != null)
            {
                Factory.CBL.SaveLastCallingRecPid(master_entity.Substring(0,3), master_pid, caller, true, false);
            }


            return View(v);
        }
        


        private bool TestGridPermissions(string prefix)
        {

            switch (prefix)
            {
                case "p31":
                    return Factory.CurrentUser.j04IsMenu_Worksheet;
                case "p41":
                    return Factory.CurrentUser.j04IsMenu_Project;
                case "p28":
                    return Factory.CurrentUser.j04IsMenu_Contact;
                case "p56":
                    return Factory.CurrentUser.j04IsMenu_Task;
                case "p90":
                    return Factory.CurrentUser.j04IsMenu_Proforma;                
                case "p91":
                    return Factory.CurrentUser.j04IsMenu_Invoice;
                case "j02":
                    return Factory.CurrentUser.j04IsMenu_People;
                case "o23":
                    return Factory.CurrentUser.j04IsMenu_Notepad;
                default:
                    return true;
            }
        }
        private FsmViewModel LoadFsmViewModel(string prefix,int go2pid,string pagename,string masterentity,int master_pid,string myqueryinline,bool isperiodovergrid)
        {
            var v = new FsmViewModel() { prefix = prefix,master_pid=master_pid, myqueryinline = myqueryinline };
            
            BO.TheEntity c = Factory.EProvider.ByPrefix(prefix);
            v.entity = c.TableName;
            v.entityTitle = c.AliasPlural;

            bool bolMasterEntity = false;
            if (!string.IsNullOrEmpty(masterentity)) bolMasterEntity = true;



            v.gridinput = new TheGridInput() {entity=v.entity, go2pid = go2pid, master_entity = masterentity,master_pid=master_pid,myqueryinline=v.myqueryinline,ondblclick= "grid_dblclick",isperiodovergrid= isperiodovergrid };
            

            if (v.entity == "")
            {
                AddMessage("Grid entita nebyla nalezena.");
            }

            
            v.gridinput.query = new BO.InitMyQuery().Load(prefix, masterentity, master_pid, myqueryinline);
            v.gridinput.query.IsRecordValid = null;
            
            if (!Factory.CurrentUser.IsAdmin)
            {
                v.gridinput.query.MyRecordsDisponible = true;
            }
            
           
            
            
            if (isperiodovergrid)
            {
                v.period = new PeriodViewModel() { prefix = v.prefix, IsShowButtonRefresh=true };
                if (!bolMasterEntity)
                {
                    v.period.UserParamKey = $"grid-period-{v.prefix}";
                }
                else
                {
                    v.period.UserParamKey = $"grid-period-{v.prefix}-{masterentity}";
                }
                
                v.period.InhaleUserPeriodSetting(_pp, Factory);
                
                v.gridinput.query.period_field = v.period.PeriodField;
                v.gridinput.query.global_d1 = v.period.d1;
                v.gridinput.query.global_d2 = v.period.d2;            
            }

            if (v.prefix=="p28" || v.prefix == "j02" || v.prefix == "p91" || v.prefix=="o23" || v.prefix=="p90" || v.prefix=="p56" || v.gridinput.query.PrefixDb=="p41")
            {
                v.IsCanbeMasterView = true;
                v.dblClickSetting = Factory.CBL.LoadUserParam("grid-" + v.prefix + "-dblclick", "edit");                
            }

            Handle_P31StateQuery(v, bolMasterEntity, masterentity);



            return v;

        }


        private int LoadLastUsedPid(string prefix)
        {
            if (prefix == "le5")
            {
                return Factory.CBL.LoadUserParamInt($"recpage-p41-pid", 0, 12);
            }
            else
            {
                return Factory.CBL.LoadUserParamInt($"recpage-{prefix}-pid", 0, 12);
            }
            
            
        }

        public List<NavTab> getTabs(string prefix,int pid)  //volá se z javascriptu při změně řádky v gridu
        {
            var cTabs = new NavTabsSupport(Factory);
            return cTabs.getMasterTabs(prefix, pid,true);
        }

        private void Handle_P31StateQuery(FsmViewModel v, bool bolMasterEntity, string masterentity)
        {
            if (v.prefix == "p31" || v.prefix == "p41" || v.prefix == "p28" || v.prefix == "j02" || v.prefix == "p56" || v.prefix == "le5" || v.prefix=="le4" || v.prefix == "le3" || v.prefix == "le2" || v.prefix == "le1")
            {
                v.IsP31StateQuery = true;
                if (!bolMasterEntity)
                {
                    v.gridinput.query.p31statequery = Factory.CBL.LoadUserParamInt("grid-" + v.prefix + "-p31statequery", 0);
                }
                else
                {
                    v.gridinput.query.p31statequery = Factory.CBL.LoadUserParamInt("grid-" + v.prefix + "-" + masterentity + "-p31statequery", 0);
                }

                v.P31StateQueryAlias = Factory.tra("Stav úkonů");
                v.P31StateQueryCssClass = "btn btn-light dropdown-toggle";
                if (v.gridinput.query.p31statequery > 0)
                {
                    v.P31StateQueryCssClass = "btn btn-light dropdown-toggle filtered";
                    v.P31StateQueryAlias = new UI.Menu.TheMenuSupport(Factory).getP31StateQueryAlias(v.gridinput.query.p31statequery);
                }




            }
        }

    }
}