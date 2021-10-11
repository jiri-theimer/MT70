using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Menu
{
    public class TheMenuSupport
    {
        private BL.Factory _f { get; set; }
        
        public TheMenuSupport(BL.Factory f)
        {
            _f = f;            
            
            
        }

        public void HandleUrlOfGridLinks(List<MenuItem> lis)
        {            
            for(int i = 0; i < lis.Count(); i++)
            {            
                if (lis[i].IsCanBeGrid11)
                {
                    Handle_GridUrl(lis[i]);
                }                
            }
           
        }

        private void Handle_GridUrl(MenuItem item)
        {
            if (_f.CBL.LoadUserParamBool("grid-" + item.ID.Substring(3, 3) + "-show11", true))
            {
                item.Url = item.Url.Replace("FlatView", "MasterView");
            }
        }

        public List<MenuItem> getUserMenuLinks()
        {
            var ret = new List<MenuItem>();
            if (_f.CurrentUser.j03SiteMenuMyLinksV7 == null)
            {
                return ret;
            }
            var lis = getAllMainMenuLinks();

            var ids = BO.BAS.ConvertString2List(_f.CurrentUser.j03SiteMenuMyLinksV7);
            IEnumerable<MenuItem> qry = null;MenuItem item = null;
            foreach(string strID in ids)
            {
                qry = lis.Where(p => p.ID == strID);
                if (qry.Count()>0)
                {
                    item = qry.First();
                    if (item.IsCanBeGrid11) Handle_GridUrl(item);
                    ret.Add(item);
                }
            }

            HandleUrlOfGridLinks(ret);
            return ret;

        }
        public List<MenuItem> getAllMainMenuLinks()
        {
            var ret = new List<MenuItem>();

            ret.Add(new MenuItem() { Name = _f.tra("Dashboard"), Url = "/Dashboard/Index",ID="cmdDashboard",Icon="dashboard" });
            if (_f.CurrentUser.IsAdmin)
            {
                ret.Add(new MenuItem() { Name = _f.tra("Administrace"), Url = "/Admin/Index?signpost=true", ID = "cmdAdmin",Icon="settings" });
            }
            if (_f.CurrentUser.j04IsMenu_Worksheet)
            {
                ret.Add(GRD("p31",false, "history_toggle_off"));
                ret.Add(new MenuItem() { Name = _f.tra("Kalendář"), Url = "/p31calendar/Index",ID="cmdCalendar",Icon= "date_range" });
                ret.Add(new MenuItem() { Name = "Dayline",Url="/p31dayline/Index",ID="cmdDayline",Icon= "calendar_view_month"});
                ret.Add(new MenuItem() { Name = _f.tra("Součty"), Url = "/p31/Totals",ID="cmdTotals",Icon= "functions" });
                }
            if (_f.CurrentUser.j04IsMenu_Project)
            {
                if (_f.CurrentUser.p07LevelsCount <= 1)
                {
                    ret.Add(GRD("p41", true, "business"));
                }
                else
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        if (_f.CurrentUser.getP07Level(i,false) != null)
                        {
                                                        
                            ret.Add(GRD("le" + i.ToString(), true, "work_outline"));
                        }
                    }
                        
                }
                
            }
            if (_f.CurrentUser.j04IsMenu_Task)
            {
                ret.Add(GRD("p56", true, "task"));
            }
            if (_f.CurrentUser.j04IsMenu_Contact)
            {
                ret.Add(GRD("p28",true, "business"));
            }
            if (_f.CurrentUser.j04IsMenu_Invoice)
            {
                ret.Add(GRD("p91",true, "receipt_long"));
            }
            if (_f.CurrentUser.j04IsMenu_Proforma)
            {
                ret.Add(GRD("p90",true, "receipt"));
            }
            if (_f.CurrentUser.j04IsMenu_People)
            {
                ret.Add(GRD("j02",true,"face"));
            }
            if (_f.CurrentUser.j04IsMenu_Notepad)
            {
                ret.Add(GRD("o23",true, "file_present"));
            }
            if (_f.CurrentUser.j04IsMenu_Report)
            {
                ret.Add(new MenuItem() { Name = _f.tra("Tiskové sestavy"), Url = "/x31/ReportNoContextFramework",ID="cmdReports",Icon="print" });
            }
            ret.Add(GRD("b07",false, "speaker_notes"));


            //ret.Add(new MenuItem() { Name = "Stránka projektu", Url = "/Record/RecPage?prefix=p41", ID = "recpagep41" });
            //ret.Add(new MenuItem() { Name = "Stránka klienta", Url = "/Record/RecPage?prefix=p28", ID = "recpagep28" });
            //ret.Add(new MenuItem() { Name = "Stránka vyúčtování", Url = "/Record/RecPage?prefix=p91", ID = "recpagep91" });
            //ret.Add(new MenuItem() { Name = "Stránka osoby", Url = "/Record/RecPage?prefix=j02", ID = "recpagej02" });
            //ret.Add(new MenuItem() { Name = "Stránka úkolu", Url = "/Record/RecPage?prefix=p56", ID = "recpagep56" });


            return ret;
        }

        private MenuItem GRD(string prefix,bool bolIsCanBeGrid11,string icon=null)
        {
            var c = new MenuItem() { ID = "cmd" + prefix,IsCanBeGrid11= bolIsCanBeGrid11,Icon=icon };
           
            switch (_f.CurrentUser.j03LangIndex)
            {
                case 1:
                    c.Name = _f.EProvider.ByPrefix(prefix).TranslateLang1; break;
                case 2:
                    c.Name = _f.EProvider.ByPrefix(prefix).TranslateLang2; break;
                case 3:
                    c.Name = _f.EProvider.ByPrefix(prefix).TranslateLang3; break;
                default:
                    c.Name = _f.EProvider.ByPrefix(prefix).AliasPlural;break;
            }
            switch (prefix)
            {
                case "j02":
                    c.Name = _f.tra("Lidé");
                    c.Url = "/TheGrid/FlatView?prefix=" + prefix;                   
                    break;
                case "p41":
                    c.Name = _f.CurrentUser.getP07Level(5,false);
                    c.Url = "/TheGrid/FlatView?prefix="+prefix;
                    break;
                case "le5":
                case "le4":
                case "le3":
                case "le2":
                case "le1":
                    c.Name = _f.CurrentUser.getP07Level(Convert.ToInt32(prefix.Substring(2,1)),false);
                    c.Url = "/TheGrid/FlatView?prefix="+prefix;
                    break;                
                case "p28":
                case "p91":                
                    c.Url = "/TheGrid/FlatView?prefix=" + prefix;
                    break;
                default:
                    c.Url = "/TheGrid/FlatView?prefix=" + prefix;
                    break;
            }

            
            
            return c;
            
        }


        //Název grid filtru [Stav úkonů]
        public string getP31StateQueryAlias(int p31statequery_value)
        {
            switch (p31statequery_value)
            {
                case 1:
                    return _f.tra("Rozpracované (Čeká na schvalování)");
                case 2:
                    return _f.tra("Rozpracované s korekcí (Čeká na schvalování)");
                case 16:
                    return _f.tra("Rozpracované [Fa aktivita] (Čeká na schvalování)");
                case 17:
                    return _f.tra("Rozpracované [NeFa aktivita] (Čeká na schvalování)");
                case 3:
                    return _f.tra("Nevyúčtované (Rozpracované nebo Schválené)");
                case 4:
                    return _f.tra("Schválené (Čeká na vyúčtování)");
                case 5:
                    return _f.tra("Schválené jako [Fakturovat] (Čeká na vyúčtování)");
                case 6:
                    return _f.tra("Schválené jako [Zahrnout do paušálu] (Čeká na vyúčtování)");
                case 7:
                    return _f.tra("Schválené jako [Skrytý/Viditelný odpis] (Čeká na vyúčtování)");
                case 8:
                    return _f.tra("Schválené jako [Fakturovat později] (Čeká na pře-schválení)");
                case 9:
                    return _f.tra("Neschválené (Čeká na pře-schválení)");                
                case 10:
                    return _f.tra("Vyúčtované");
                case 11:
                    return _f.tra("DRAFT vyúčtování");
                case 12:
                    return _f.tra("Vyúčtované jako [Fakturovat]");
                case 13:
                    return _f.tra("Vyúčtované s nulovou cenou jako [Zahrnout do paušálu]");
                case 14:
                    return _f.tra("Vyúčtované s nulovou cenou jako [Skrytý/Viditelný odpis]");

                case 15:
                    return _f.tra("Úkony přesunuté do archivu");

                default:
                    return _f.tra("Stav úkonů");

            }
        }

        //vrátí HTML pro kontextové menu
        public string FlushResult_UL(List<MenuItem> menuitems, bool bolSupportIcons, bool bolRenderUlContainer, string source = null)
        {
            var sb = new System.Text.StringBuilder();

            if (bolRenderUlContainer)
            {
                sb.AppendLine("<ul style='border:0px;'>");
            }
            var qry = menuitems.Where(p => p.ParentID == null);
            if (source == "recpage")
            {
                qry = qry.Where(p => p.IsHeader == false);
            }
            foreach (var c in qry)
            {
                if (c.IsDivider == true)
                {
                    if (c.Name == null)
                    {
                        sb.Append("<li><hr></li>");  //divider
                    }
                    else
                    {
                        sb.Append("<div class='hr-text'>" + c.Name + "</div>");
                    }

                }
                else
                {
                    if (c.IsHeader)
                    {
                        sb.Append("<div style='color:silver;font-style: italic;'>" + c.Name + "</div>");
                    }
                    else
                    {
                        string strStyle = null;
                        string strImg = "<span style='margin-left:10px;'></span>";
                        string strCssClass = "dropdown-item";
                        if (c.CssClass != null)
                        {
                            strCssClass = c.CssClass;
                        }
                        if (bolSupportIcons)
                        {
                            strImg = "<span style='width:30px;'></span>";
                            if (c.Icon != null)
                            {
                                if (c.Icon.Length == 1)
                                {
                                    strImg = "<span style='margin-left:10px;margin-right:5px;font-size:150%;color:royalblue;'>" + c.Icon + "</span>";     //1 unicode character
                                }
                                else
                                {                                    
                                    strImg = string.Format("<span class='material-icons-outlined-btn' style='width:30px;'>{0}</span>", c.Icon);   //google icon
                                }

                            }
                        }

                        if (c.IsActive == true)
                        {
                            strStyle = " style='background-color: #ADD8E6;' id='menu_active_item'";
                        }
                        bool bolHasChilds = false;
                        if (c.ID != null && menuitems.Where(p => p.ParentID == c.ID).Count() > 0)
                        {
                            bolHasChilds = true;                            
                            c.Name += " (" + menuitems.Where(p => p.ParentID == c.ID).Count().ToString() + ")<span style='color:gray;float:right;font-size:150%;'>⇢</span>";

                        }
                        

                        if (c.Url == null)
                        {
                            if (bolHasChilds)
                            {
                                sb.Append($"<li{strStyle}><span class='material-icons-outlined-btn' style='width:30px;'>more_horiz</span>{c.Name}");
                            }
                            else
                            {
                                sb.Append($"<li{strStyle}><a>{c.Name}</a>");
                            }

                        }
                        else
                        {
                            if (c.Target != null) c.Target = " target='" + c.Target + "'";
                            sb.Append($"<li{strStyle}><a class='{strCssClass} px-0' href=\"{c.Url}\"{c.Target}>{strImg}{c.Name}</a>");


                        }
                        if (bolHasChilds)
                        {
                            //podřízené nabídky -> druhá úroveň »
                            sb.Append("<ul class='cm_submenu'>");
                            foreach (var cc in menuitems.Where(p => p.ParentID == c.ID))
                            {
                                if (cc.IsDivider)
                                {
                                    sb.Append("<li><hr></li>");  //divider
                                }
                                else
                                {
                                    if (cc.Target != null) cc.Target = " target='" + cc.Target + "'";
                                    strImg = "<span style='margin-left:10px;'></span>";
                                    if (bolSupportIcons)
                                    {
                                        strImg = "<span style='width:30px;'></span>";
                                        if (cc.Icon != null)
                                        {
                                            if (cc.Icon.Length == 1)
                                            {
                                                strImg = "<span style='margin-left:10px;margin-right:5px;font-size:150%;color:royalblue;'>" + cc.Icon + "</span>";     //1 unicode character
                                            }
                                            else
                                            {
                                                strImg = string.Format("<span class='material-icons-outlined-btn' style='width:30px;'>{0}</span>", cc.Icon);   //google icon
                                            }

                                        }
                                    }
                                    sb.Append($"<li><a class='dropdown-item' href=\"{cc.Url}\"{cc.Target}>{strImg}{cc.Name}</a></li>");
                                                                        
                                    
                                }

                            }
                            sb.Append("</ul>");
                        }

                        sb.Append("</li>");
                    }

                }

            }

            if (bolRenderUlContainer)
            {
                sb.Append("</ul>");
            }
            return sb.ToString();
        }

    }
}
