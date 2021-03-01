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
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity, tgi.master_pid, tgi.myqueryinline);
            v.gridinput.ondblclick = tgi.ondblclick;
            v.gridinput.oncmclick = tgi.oncmclick;
            var c = new UI.TheGridSupport(v.gridinput, Factory, _colsProvider);

            return c.Event_HandleTheGridFilter(tgi, filter);
        }
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi, List<BO.StringPair> pathpars)    //TheGrid povinná metoda: změna třídění, pageindex, změna stránky
        {
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity, tgi.master_pid, tgi.myqueryinline);
            v.gridinput.ondblclick = tgi.ondblclick;
            v.gridinput.oncmclick = tgi.oncmclick;
            var c = new UI.TheGridSupport(v.gridinput, Factory, _colsProvider);

            return c.Event_HandleTheGridOper(tgi);

        }
        public string HandleTheGridMenu(TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda: zobrazení grid menu
        {
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity, tgi.master_pid, tgi.myqueryinline);
            v.gridinput.ondblclick = tgi.ondblclick;
            v.gridinput.oncmclick = tgi.oncmclick;
            var c = new UI.TheGridSupport(v.gridinput, Factory, _colsProvider);
            return c.Event_HandleTheGridMenu(tgi.j72id);
            
        }

        public TheGridExportedFile HandleTheGridExport(string format, string pids, TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda pro export dat
        {
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity, tgi.master_pid, tgi.myqueryinline);
            v.gridinput.ondblclick = tgi.ondblclick;
            v.gridinput.oncmclick = tgi.oncmclick;
            var c = new UI.TheGridSupport(v.gridinput, Factory, _colsProvider);
            return c.Event_HandleTheGridExport(format, tgi.j72id, pids);
        }        
        //-----------Konec GRID událostí-------------

        public IActionResult FlatView(string prefix, int go2pid)    //pouze grid bez subform
        {
            if (!TestGridPermissions(prefix))
            {
                return this.StopPage(false, "Nemáte oprávnění pro tento GRID přehled.");
            }
            FsmViewModel v = LoadFsmViewModel(prefix, go2pid,"flatview",null,0,null);
            
            v.gridinput.j72id = Factory.CBL.LoadUserParamInt("flatview-j72id-" + prefix);

            return View(v);
        }

        

        public IActionResult MasterView(string prefix,int go2pid)    //grid horní + spodní panel, není zde filtrovací pruh s fixním filtrem
        {
            if (!TestGridPermissions(prefix))
            {
                return this.StopPage(false, "Nemáte oprávnění pro tento GRID přehled.");
            }
            FsmViewModel v = LoadFsmViewModel(prefix, go2pid,"masterview",null,0,null);
                      
            v.gridinput.j72id = Factory.CBL.LoadUserParamInt("masterview-j72id-" + prefix);
            
            var tabs = new List<NavTab>();
            
            switch (prefix)
            {
                
                case "j02":
                    tabs.Add(AddTab("Info", "viewInfo", "/j02/Info?pid=" + AppendPid2Url(v.gridinput.go2pid)));
                    tabs.Add(AddTab("Časový plán", "gantt", "/a35/TabPersonalTimeline?pid=" + AppendPid2Url(v.gridinput.go2pid)));

                  
                    tabs.Add(AddTab("Instituce", "a03Institution", "SlaveView?prefix=a03"));
                    tabs.Add(AddTab("Úkoly/Lhůty", "h04ToDo", "SlaveView?prefix=h04"));
                    tabs.Add(AddTab("Outbox", "x40MailQueue", "SlaveView?prefix=x40"));
                    tabs.Add(AddTab("PING Log", "j92PingLog", "SlaveView?prefix=j92"));
                    tabs.Add(AddTab("LOGIN Log", "j90LoginAccessLog", "SlaveView?prefix=j90"));
                    break;
                
                
                
            }
            string strDefTab = Factory.CBL.LoadUserParam("masterview-tab-" + prefix);
            var deftab = tabs[0];
            
            foreach (var tab in tabs)
            {
               if (tab.Url.Contains("?pid")==false)
                {
                    tab.Url += "&master_entity=" + v.entity + "&master_pid=" + AppendPid2Url(v.gridinput.go2pid);
                   
                }
             
                if (strDefTab !="" && tab.Entity== strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka
                }
            }
            deftab.CssClass += " active";
            if (go2pid > 0)
            {
                v.go2pid_url_in_iframe = deftab.Url;
                
            }

            v.NavTabs = tabs;
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
        public IActionResult SlaveView(string master_entity,int master_pid, string prefix, int go2pid,string myqueryinline)    //podřízený subform v rámci MasterView
        {

            FsmViewModel v = LoadFsmViewModel(prefix, go2pid, "slaveview", master_entity, master_pid, myqueryinline);


            v.gridinput.j72id = Factory.CBL.LoadUserParamInt("slaveview-j72id-" + prefix + "-" + master_entity);

            if (String.IsNullOrEmpty(master_entity) || master_pid == 0)
            {
                AddMessage("Musíte vybrat záznam z nadřízeného panelu.");
            }


            return View(v);
        }
        private string get_param_key(string strKey,string strMasterEntity)
        {
            if (strMasterEntity != null)
            {
                return (strKey += "-" + strMasterEntity);
            }
            else
            {
                return strKey;
            }
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
                case "p91":
                    return Factory.CurrentUser.j04IsMenu_Invoice;
                case "j02":
                    return Factory.CurrentUser.j04IsMenu_People;
                default:
                    return true;
            }
        }
        private FsmViewModel LoadFsmViewModel(string prefix,int go2pid,string pagename,string masterentity,int master_pid,string myqueryinline)
        {
            var v = new FsmViewModel() { prefix = prefix,master_pid=master_pid, myqueryinline = myqueryinline };

            BO.TheEntity c = Factory.EProvider.ByPrefix(prefix);
            v.entity = c.TableName;
            v.entityTitle = c.AliasPlural;

            v.gridinput = new TheGridInput() {entity=v.entity, go2pid = go2pid, master_entity = masterentity };
            

            if (v.entity == "")
            {
                AddMessage("Grid entita nebyla nalezena.");
            }
                        
            

            

            switch (v.prefix)
            {
               
                default:
                    v.gridinput.query = new BO.InitMyQuery().Load(prefix, masterentity, master_pid, myqueryinline);
                    v.gridinput.query.IsRecordValid = null;
                    break;
            }

            if (!Factory.CurrentUser.IsAdmin)
            {
                v.gridinput.query.MyRecordsDisponible = true;
            }
            
           


            if (v.prefix == "p31" || v.prefix == "p41")
            {
                
                v.period = new PeriodViewModel();
                v.period.IsShowButtonRefresh = true;
                var per = bas.InhalePeriodDates(_pp,Factory,v.prefix,masterentity);
                v.period.PeriodValue = per.pid;
                v.period.d1 = per.d1;
                v.period.d2 = per.d2;
                v.gridinput.query.global_d1 = v.period.d1;
                v.gridinput.query.global_d2 = v.period.d2;
            }

            if (v.prefix=="p41" || v.prefix=="p28" || v.prefix == "j02")
            {
                if (pagename == "flatview")
                {
                    v.gridinput.extendpagerhtml= "<button type='button' class='btn btn-secondary btn-sm mx-4 nonmobile' onclick='switch_to_master(\"" + v.prefix+"\")'>Zapnout spodní panel</button>";
                }
                if (pagename == "masterview")
                {
                    v.gridinput.extendpagerhtml = "<button type='button' class='btn btn-secondary btn-sm mx-4' onclick='switch_to_flat(\"" + v.prefix + "\")'>" + Factory.tra("Vypnout spodní panel") + "</button>";
                }
            }

            

            return v;

        }

       
        
        

    }
}