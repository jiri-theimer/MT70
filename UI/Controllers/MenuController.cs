using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BO;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class MenuController : BaseController
    {
        

        private List<UI.Models.MenuItem> _lis;

        public MenuController()
        {
            _lis = new List<UI.Models.MenuItem>();
        }


        public IActionResult Index()
        {
            return View();
        }

        public string GlobalNavigateMenu()
        {
            

            DIV();
            AMI("Odhlásit se", "/Home/logout", "k-i-logout");

            return FlushResult_NAVLINKs();
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
            AMI("Můj profil", "/Home/MyProfile", "k-i-user");
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
        public string AdminMenu()
        {
            AMI("Správa uživatelů", "/Admin/Users", "k-i-user");            
            AMI("Vykazování úkonů", "/Admin/Worksheet", "k-i-clock");
            AMI("Vyúčtování", "/Admin/Billing", "k-i-dollar");
            AMI("Organizace projektů", "/Admin/Projects", "k-i-wrench");
            AMI("Ostatní", "/Admin/Misc", "k-i-gear");
            return FlushResult_UL(true,false);
        }
       
        public string AdminUsers(string prefix)
        {
            //DIV_TRANS("Správa uživatelů");
            AMI("Uživatelské účty", url_users("j03"));
            AMI("Aplikační role", url_users("j04"));
            DIV();
            AMI("Přihlásit se pod jinou identitou", "/Admin/LogAsUser");
            
            DIV_TRANS("Osobní profily");
            AMI("Osobní profily", url_users("j02"));
            AMI("Pozice", url_users("j07"));
            AMI("Týmy osob", url_users("j11"));
            AMI("Nadřízení/Podřízení", url_users("j05"));

            DIV_TRANS("Časový fond");
            AMI("Pracovní fondy", url_users("c21"));
            AMI("Dny svátků", url_users("c26"));

            DIV_TRANS("Dashboard");
            AMI("Widgety", url_users("x55"));

            DIV_TRANS("Provoz");
            AMI("PING Log", url_users("j92"));
            
            AMI("LOGIN Log", url_users("j90"));
            AMI("WORKFLOW Log", url_users("b05"));
            DIV_TRANS("Pošta");
            AMI("Poštovní účty", url_users("j40"));
            AMI("OUTBOX", url_users("x40"));
            AMI("MAIL fronta", "/mail/MailBatchFramework");
                       
            handle_selected_item(prefix);

            return FlushResult_UL(false,true);
        }
        public string AdminWorksheet(string prefix)
        {
            AMI("Sešity", url_admin_worksheet("p34"));
            AMI("Aktivity", url_admin_worksheet("p32"));
            DIV();
            AMI("Fakturační oddíly", url_admin_worksheet("p95"));
            AMI("Odvětví aktivit", url_admin_worksheet("p38"));
            AMI("Klastry aktivit", url_admin_worksheet("p61"));
            
            DIV();
            AMI("Uzamknutá období", url_admin_worksheet("p36"));
            AMI("Jednotky kusovníkových úkonů", url_admin_worksheet("p35"));

            DIV_TRANS("Hodinové sazby");
            AMI("Ceníky sazeb", url_admin_worksheet("p51"));

            handle_selected_item(prefix);

            return FlushResult_UL(false,true);
        }
        public string AdminBilling(string prefix)
        {
            AMI("Typy faktur", url_admin_billing("p92"));
            AMI("Bankovní účty", url_admin_billing("p86"));            
            AMI("Vystavovatelé faktur", url_admin_billing("p93"));
            DIV();
            AMI("Měnové kurzy", url_admin_billing("m62"));
            AMI("DPH sazby", url_admin_billing("p53"));
            AMI("Fakturační oddíly", url_admin_billing("p95"));
            AMI("Zaokrouhlovací pravidla", url_admin_billing("p98"));
            AMI("Struktury rozpisu faktury", url_admin_billing("p80"));
            AMI("Režijní přirážky k fakturaci", url_admin_billing("p63"));
            DIV();
            AMI("Typy záloh", url_admin_billing("p89"));
            DIV_TRANS("Hodinové sazby");
            AMI("Ceníky sazeb", url_admin_billing("p51"));

            handle_selected_item(prefix);

            return FlushResult_UL(false,true);
        }
        public string AdminProjects(string prefix)
        {
            AMI("Úrovně", url_admin_projects("p40"));
            DIV();
            AMI("Typy", url_admin_projects("p42"));
            AMI("Role osob v projektech", url_admin_projects("x67"));

            DIV_TRANS("Hodinové sazby");
            AMI("Ceníky sazeb", url_admin_projects("p51"));

            handle_selected_item(prefix);

            return FlushResult_UL(false,true);
        }
        public string AdminCiselniky(string prefix)
        {                        
                        
            DIV_TRANS("Pevné tiskové sestavy");
            AMI("Report šablony", url_ciselniky("x31"));

            AMI("Uživatelská nápověda", url_ciselniky("x51"));
            AMI("Aplikační překlad", url_ciselniky("x91"));

            handle_selected_item(prefix);

            return FlushResult_UL(false,true);
        }

        public string MainMenu(string prefix)
        {
            
            AMI("Přehled úkonů", "/TheGrid/FlatView?prefix=p31");
            AMI("Kalendář", "/TheGrid/FlatView?prefix=p31");
            AMI("Dayline", "/TheGrid/FlatView?prefix=p31");
            AMI("Součty", "/TheGrid/FlatView?prefix=p31");
            DIV();
            AMI("Klienti", "/TheGrid/FlatView?prefix=p28");

            return FlushResult_UL(true, false);
        }
        public string MenuNewRecord(string prefix)
        {
            DIV();
            AMI("Klient", "_edit('p28',0)");
            AMI("Projekt", "_edit('p41',0)");
            AMI("Dokument", "_edit('o23',0)");
            DIV();
            AMI("Poznámka/Odkaz/Příloha", "_edit('b07',0)");
            DIV();
            AMI("Osoba - uživatel", "_edit('j02',0)");
            AMI("Kontaktní osoba klienta", "_edit('j02',0)");
            return FlushResult_UL(true, false);
        }
        private string url_ciselniky(string prefix)
        {
            return "/Admin/Ciselniky?prefix=" + prefix;
        }
       
        private string url_users(string prefix)
        {
            return "/Admin/Users?prefix=" + prefix;
        }
        private string url_admin_projects(string prefix)
        {
            return "/Admin/Projects?prefix=" + prefix;
        }
        private string url_admin_worksheet(string prefix)
        {
            return "/Admin/Worksheet?prefix=" + prefix;
        }
        private string url_admin_billing(string prefix)
        {
            return "/Admin/Billing?prefix=" + prefix;
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

        
       
        

        private void AMI(string strName,string strUrl,string icon=null, string strParentID = null,string strID=null, string strTarget = null)
        {            
            _lis.Add(new UI.Models.MenuItem() { Name = Factory.tra(strName), Url = strUrl,Target=strTarget,ID=strID,ParentID=strParentID,Icon=icon });
        }
        private void AMI_NOTRA(string strName, string strUrl,string icon=null, string strParentID = null, string strID = null, string strTarget = null)
        {           
            _lis.Add(new UI.Models.MenuItem() { Name = strName, Url = strUrl, Target = strTarget, ID = strID, ParentID = strParentID,Icon=icon });
        }
        private void DIV(string strName=null, string strParentID = null)
        {
            _lis.Add(new UI.Models.MenuItem() { IsDivider = true, Name = BO.BAS.OM2(strName,30),ParentID=strParentID });
        }
        private void DIV_TRANS(string strName = null)
        {
            _lis.Add(new UI.Models.MenuItem() { IsDivider = true, Name = BO.BAS.OM2(Factory.tra(strName), 30) });
        }
        private void HEADER(string strName)
        {
            _lis.Add(new UI.Models.MenuItem() { IsHeader = true, Name = BO.BAS.OM2(strName, 100)+":" });
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
                        string strStyle = "";
                        string strImg = "<span style='margin-left:10px;'></span>";
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
                            sb.Append(string.Format("<li{0}><a class='dropdown-item px-0' href=\"{1}\"{2}>{3}{4}</a>", strStyle, c.Url, c.Target,strImg, c.Name));
                                                       
                            
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
        private string FlushResult_NAVLINKs()
        {
            var sb = new System.Text.StringBuilder();

            foreach (var c in _lis)
            {
                if (c.Name == null)
                {
                    sb.Append("<hr>");  //divider
                }
                else
                {
                    if (c.Url == null)
                    {
                        sb.Append(string.Format("<span>{0}</span>", c.Name));
                    }
                    else
                    {
                        if (c.Target != null) c.Target = " target='" + c.Target + "'";
                        sb.Append(string.Format("<a class='nav-link' style='color:black;' href=\"{0}\"{1}>{2}</a>", c.Url, c.Target, c.Name));
                    }
                }

            }


            return sb.ToString();
        }
    }
}