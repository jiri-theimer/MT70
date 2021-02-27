using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BO;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class MenuController : BaseController
    {
        

        private List<MenuItem> _lis;

        public MenuController()
        {
            _lis = new List<MenuItem>();
        }


        public IActionResult Index()
        {
            return View();
        }

        
        public string CurrentUserMyProfile()
        {                
            AMI("Aktuální stránku uložit jako domovskou", "javascript:_save_as_home_page()", "k-i-heart-outline");
            if (Factory.CurrentUser.j03HomePageUrl !=null)
            {
                AMI("Vyčistit odkaz na domovskou stránku", "javascript:_clear_home_page()", "k-i-heart-outline k-flip-v");
            }
            AMI("Tovární HOME stránka (Dashboard)", "/Dashboard/Index", "k-i-star-outline");

            DIV();
            
            if (Factory.CurrentUser.j04IsMenu_MyProfile)
            {
                AMI("Můj profil", "/Home/MyProfile", "k-i-user");
            }
            
            AMI("Odeslat zprávu", "javascript:_sendmail()", "k-i-email");

            AMI("Změnit přístupové heslo", "/Home/ChangePassword", "k-i-password");
            
            DIV();
            AMI("Nápověda", "javascript: _window_open('/x51/Index')", "k-i-question");
            AMI("O aplikaci", "/Home/About", "k-i-information");
            DIV();
            AMI("Odhlásit se", "/Home/logout", "k-i-logout");



            return FlushResult_UL(true,false);
        }
        public string CurrentUserLangIndex()
        {
            for (int i = 0; i <= 4; i++)
            {
                string s = "<img src='/images/small_czechrepublic.gif'/> Česky";
                if (i == 1) s = "<img src='/images/small_uk.gif'/> English";
                if (i == 2) s = "Deutch";
                if (i == 4) s = "<img src='/images/small_slovakia.gif'/> Slovenčina";
                if (Factory.CurrentUser.j03LangIndex == i) s += "&#10004;";
                if (i != 2 && i!=3) //jazyk 2 a 3 zatím neukazovat!
                {
                    AMI_NOTRA(s, string.Format("javascript: _save_langindex_menu({0})", i));
                }
                
            }
            return FlushResult_UL(true,false);            
        }
        public string CurrentUserFontSize()
        {            
            for (int i = 1; i <= 3; i++)
            {
                string s = "<span style='font-size:80%;'>" + Factory.tra("Malé písmo") + "</span>";
                if (i == 2) s = Factory.tra("Výchozí velikost písma");
                if (i == 3) s = "<span style='font-size:130%;'>" + Factory.tra("Velké písmo") + "</span>";
                
                if (Factory.CurrentUser.j03GlobalCssFlag == i) s += "&#10004;";
                AMI_NOTRA(s, string.Format("javascript: _save_fontsize({0})", i));


            }
          
            return FlushResult_UL(true,false);
        }
        
        private string tmclass(string area,string curarea)
        {
            if (area == curarea)
            {
                return "topmenulink_active";
            }
            else
            {
                return "topmenulink";
            }
        }
        public string AdminMenu(string area, string prefix)
        {
            MenuItem c = AMI("Úvod", "/Admin/Index", "k-i-home");
            c.CssClass = BO.BAS.IIFS(area == null, "topmenulink_active", "topmenulink");
            c =AMI("Správa uživatelů", aurl("users"), "k-i-user");
            c.CssClass = tmclass("users", area);
            c =AMI("Vykazování úkonů", aurl("worksheet"), "k-i-clock");
            c.CssClass = tmclass("worksheet", area);
            c =AMI("Vyúčtování", aurl("billing"), "k-i-dollar");
            c.CssClass = tmclass("billing", area);
            c =AMI("Projekty", aurl("projects"), "k-i-wrench");
            c.CssClass = tmclass("projects", area);
            c =AMI("Klienti", aurl("clients"), "k-i-wrench");
            c.CssClass = tmclass("clients", area);
            c =AMI("Různé", aurl("misc"), "k-i-more-vertical");
            c.CssClass = tmclass("misc", area);

            //AMI("Globální parametry", "javascript: _window_open('/x35/x35Params',1)", "k-i-gear");

            switch (area)
            {
                case "projects":
                    Handle_AdminProjects(prefix);break;
                case "clients":
                    Handle_AdminClients(prefix);break;
                case "users":
                    Handle_AdminUsers(prefix); break;
                case "worksheet":
                    Handle_AdminWorksheet(prefix);break;
                case "billing":
                    Handle_AdminBilling(prefix);break;
                case "misc":
                    Handle_AdminMisc(prefix);break;

            }
            
            // FlushResult_UL(true,false);

            return FlushResult_UL(true,true);
        }
       
        private void Handle_AdminUsers(string prefix)
        {
            //DIV_TRANS("Správa uživatelů");
            AMI("Uživatelské účty", aurl("users","j03"));
            AMI("Aplikační role", aurl("users","j04"));
            DIV();
            AMI("Přihlásit se pod jinou identitou", "/Admin/LogAsUser");
            
            DIV_TRANS("Osobní profily");
            AMI("Osobní profily", aurl("users","j02"));
            AMI("Pozice", aurl("users","j07"));
            AMI("Týmy osob", aurl("users","j11"));
            AMI("Nadřízení/Podřízení", aurl("users","j05"));

            DIV_TRANS("Časový fond");
            AMI("Pracovní fondy", aurl("users","c21"));
            AMI("Dny svátků", aurl("users","c26"));

            DIV_TRANS("Dashboard");
            AMI("Widgety", aurl("users","x55"));

            DIV_TRANS("Provoz");
            AMI("PING Log", aurl("users","j92"));
            
            AMI("LOGIN Log", aurl("users","j90"));
            
            DIV_TRANS("Pošta");
            AMI("SMTP poštovní účty", aurl("users","o40"));
            AMI("OUTBOX", aurl("users","x40"));
            AMI("MAIL fronta", "/mail/MailBatchFramework");
                       
            handle_selected_item(prefix);
            //return FlushResult_UL(false,true);
        }

        public string aurl(string area,string prefix)
        {
            return "/Admin/Page?area=" + area + "&prefix=" + prefix;
        }
        public string aurl(string area, string prefix,string ocas)
        {
            return "/Admin/Page?area=" + area + "&prefix=" + prefix+"&"+ocas;
        }
        public string aurl(string area)
        {
            return "/Admin/Page?area=" + area;
        }

        private void Handle_AdminWorksheet(string prefix)
        {
            //DIV_TRANS("Vykazování úkonů");
            AMI("Sešity", aurl("worksheet","p34"));
            AMI("Aktivity", aurl("worksheet","p32"));
            DIV();
            AMI("Fakturační oddíly", aurl("worksheet","p95"));
            AMI("Odvětví aktivit", aurl("worksheet","p38"));
            AMI("Klastry aktivit", aurl("worksheet","p61"));

            DIV();
            AMI("Uzamknutá období", aurl("worksheet","p36"));
            AMI("Jednotky kusovníkových úkonů", aurl("worksheet","p35"));

            DIV_TRANS("Hodinové sazby");
            AMI("Ceníky sazeb", aurl("worksheet","p51"));

            AMI("Uživatelská pole", aurl("worksheet","x28","myqueryinline=x29id|int|331"));
            handle_selected_item(prefix);

            //return FlushResult_UL(false,true);
        }
        public void Handle_AdminBilling(string prefix)
        {            
            AMI("Typy faktur", aurl("billing","p92"));
            AMI("Bankovní účty", aurl("billing","p86"));            
            AMI("Vystavovatelé faktur", aurl("billing","p93"));
            DIV();
            AMI("Měnové kurzy", aurl("billing","m62"));
            AMI("DPH sazby", aurl("billing","p53"));
            AMI("Fakturační oddíly", aurl("billing","p95"));
            AMI("Zaokrouhlovací pravidla", aurl("billing","p98"));
            AMI("Struktury rozpisu faktury", aurl("billing","p80"));
            AMI("Režijní přirážky k fakturaci", aurl("billing","p63"));
            DIV();
            AMI("Typy záloh", aurl("billing","p89"));
            DIV_TRANS("Hodinové sazby");
            AMI("Ceníky sazeb", aurl("billing","p51"));

            AMI("Uživatelská pole", aurl("billing","x28","myqueryinline=x29id|int|391"));

            handle_selected_item(prefix);

            //return FlushResult_UL(false,true);
        }
        public void Handle_AdminProjects(string prefix)
        {            
            AMI("Úrovně projektů", aurl("projects","p07"));
            DIV();
            AMI("Typy projektů", aurl("projects","p42"));
            AMI("Role osob v projektech", aurl("projects","x67","myqueryinline=x29id|int|141"));


            AMI("Uživatelská pole", aurl("projects","x28","myqueryinline=x29id|int|141"));

            handle_selected_item(prefix);

            //return FlushResult_UL(false,true);
        }
        private void Handle_AdminClients(string prefix)
        {
            
            AMI("Typy klientů", aurl("clients","p29"));
            AMI("Role osob v klientech", aurl("clients","x67","myqueryinline=x29id|int|328"));

            AMI("Uživatelská pole", aurl("clients","x28","myqueryinline=x29id|int|328"));

            handle_selected_item(prefix);

            //return FlushResult_UL(false, true);
        }
        private void Handle_AdminMisc(string prefix)
        {            
            
            AMI("Katalog uživatelských polí", aurl("misc","x28"));
            AMI("Skupiny uživatelských polí", aurl("misc","x27"));

            DIV_TRANS("Pevné tiskové sestavy");
            AMI("Report šablony", aurl("misc","x31"));
            AMI("Kategorie sestav", aurl("misc","j25"));

            DIV_TRANS("Ostatní");
            AMI("Číselné řady", aurl("misc","x38"));
            AMI("Střediska", aurl("misc","j18"));
            AMI("Daňové regiony", aurl("misc","j17"));
            AMI("Notifikace událostí", aurl("misc","x46"));

            AMI("Uživatelská nápověda", aurl("misc","x51"));
            AMI("Aplikační překlad", aurl("misc","x91"));

            handle_selected_item(prefix);

            //return FlushResult_UL(false,true);
        }

        public string MainMenu(string prefix)
        {
           
            AMI("Přehled úkonů", "/TheGrid/FlatView?prefix=p31");
            AMI("Kalendář", "/TheGrid/FlatView?prefix=p31");
            AMI("Dayline", "/TheGrid/FlatView?prefix=p31");
            AMI("Součty", "/TheGrid/FlatView?prefix=p31");
            DIV();
            AMI("Klienti", "/TheGrid/FlatView?prefix=p28");
            
            
            var s = "<ul style='list-style-type:none; columns:2;-webkit-columns: 2;-moz-columns:2;'>";
            s += FlushResult_UL(true, false);
            s += "</ul>";
            s += "<hr>";
            s += "<div><button type='button' class='btn btn-sm btn-light' style='width:100%;' onclick=\"_window_open('/Home/MyMainMenuLinks',1)\"><span class='k-icon k-i-gear'></span>" + Factory.tra("Nastavit odkazy pro mé hlavní menu") + "</button></div>";
           

            return s;
        }
        public string MenuNewRecord(string prefix)
        {
            DIV();
            AMI("Klient", "javascript:_edit('p28',0)");
            AMI("Projekt", "javascript:_edit('p41',0)");
            AMI("Dokument", "javascript:_edit('o23',0)");
            DIV();
            AMI("Poznámka/Odkaz/Příloha", "javascript:_edit('b07',0)");
            DIV();
            AMI("Interní osoba s uživatelským účtem", "javascript:_window_open('/j02/Record?pid=0&isintraperson=true', 1)");
            AMI("Kontaktní osoba klienta", "javascript:_window_open('/j02/Record?pid=0&isintraperson=false', 1)");
            return FlushResult_UL(true, false);
        }

        public string TheGridSelMenu(TheGridUIContext tgi)  //menu pro označené grid záznamy
        {
            AMI("MS EXCEL Export", "javascript:tg_export('xlsx','selected')", "k-i-file-excel");
            AMI("CSV Export", "javascript:tg_export('csv','selected')", "k-i-file-csv");


            if ("p31,p41,p28,j02".Contains(tgi.prefix))
            {
                DIV();
                AMI("Hromadná kategorizace záznamů", "javascript:tg_tagging()", "k-i-categorize");




            }

            return FlushResult_UL(true, false);
        }
        
       
        
        
        
       
        private void handle_selected_item(string prefix)
        {
            if (prefix != null)
            {
                if (_lis.Where(p => p.Url != null && p.Url.Contains("=" + prefix)).Count() > 0)
                {
                    _lis.Where(p => p.Url != null && p.Url.Contains("=" + prefix)).First().IsActive = true;
                }
            }
        }


        


        private MenuItem AMI(string strName,string strUrl,string icon=null, string strParentID = null,string strID=null, string strTarget = null)
        {
            var c = new MenuItem() { Name = Factory.tra(strName), Url = strUrl, Target = strTarget, ID = strID, ParentID = strParentID, Icon = icon };
            _lis.Add(c);
            return c;
        }
        private void AMI_NOTRA(string strName, string strUrl,string icon=null, string strParentID = null, string strID = null, string strTarget = null)
        {           
            _lis.Add(new MenuItem() { Name = strName, Url = strUrl, Target = strTarget, ID = strID, ParentID = strParentID,Icon=icon });
        }
        private void DIV(string strName=null, string strParentID = null)
        {
            _lis.Add(new MenuItem() { IsDivider = true, Name = BO.BAS.OM2(strName,30),ParentID=strParentID });
        }
        private void DIV_TRANS(string strName = null)
        {
            _lis.Add(new MenuItem() { IsDivider = true, Name = BO.BAS.OM2(Factory.tra(strName), 30) });
        }
        private void HEADER(string strName)
        {
            _lis.Add(new MenuItem() { IsHeader = true, Name = BO.BAS.OM2(strName, 100)+":" });
        }

        private string FlushResult_UL(bool bolSupportIcons, bool bolRenderUlContainer)
        {
            var sb = new System.Text.StringBuilder();

            if (bolRenderUlContainer)
            {
                sb.AppendLine("<ul style='border:0px;'>");
            }
            foreach (var c in _lis.Where(p=>p.ParentID==null))
            {                
                if (c.IsDivider==true)
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
                            strImg = "<span class='k-icon' style='width:30px;'></span>";
                            if (c.Icon != null)
                            {
                                strImg = string.Format("<span class='k-icon {0}' style='width:30px;'></span>", c.Icon);
                            }
                        }
                        
                        if (c.IsActive == true)
                        {
                            strStyle = " style='background-color: #ADD8E6;' id='menu_active_item'";
                        }
                        bool bolHasChilds = false;
                        if (c.ID != null && _lis.Where(p => p.ParentID == c.ID).Count() > 0)
                        {
                            bolHasChilds = true;
                            c.Name += "<span style='float:right;'> ▶</span>";
                        }
                        

                        if (c.Url == null)
                        {
                            sb.Append(string.Format("<li{0}><a>{1}</a>", strStyle, c.Name));
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
                            foreach (var cc in _lis.Where(p => p.ParentID == c.ID))
                            {
                                if (cc.IsDivider)
                                {
                                    sb.Append("<li><hr></li>");  //divider
                                }
                                else
                                {
                                    if (cc.Target != null) cc.Target = " target='" + cc.Target + "'";
                                    sb.Append(string.Format("<li><a class='dropdown-item' href=\"{0}\"{1}>{2}</a></li>", cc.Url, cc.Target, cc.Name));
                                }

                            }
                            sb.Append("</ul>");
                        }

                        sb.Append("</li>");
                    }
                    
                }                                

            }

            //sb.AppendLine("</ul>");

            return sb.ToString();
        }
        //private string FlushResult_NAVLINKs()
        //{
        //    var sb = new System.Text.StringBuilder();

        //    foreach (var c in _lis)
        //    {
        //        if (c.Name == null)
        //        {
        //            sb.Append("<hr>");  //divider
        //        }
        //        else
        //        {
        //            if (c.Url == null)
        //            {
        //                sb.Append(string.Format("<span>{0}</span>", c.Name));
        //            }
        //            else
        //            {
        //                if (c.Target != null) c.Target = " target='" + c.Target + "'";
        //                sb.Append(string.Format("<a class='nav-link' style='color:black;' href=\"{0}\"{1}>{2}</a>", c.Url, c.Target, c.Name));
        //            }
        //        }

        //    }


        //    return sb.ToString();
        //}
    }
}