using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BO;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Menu;

namespace UI.Controllers
{
    public class MenuController : BaseController
    {
        

        private List<MenuItem> _lis;
        private TheMenuSupport _menusup;

        public MenuController()
        {
            _lis = new List<MenuItem>();
            _menusup = new TheMenuSupport(null);
        }




        public IActionResult Index()
        {
            return View();
        }

        public string ContextMenu(string entity, int pid,string source,string master_entity)
        {
            string prefix = entity.Substring(0, 3);
            var lis = new List<MenuItem>();
            switch (BO.BASX29.GetPrefixDb(prefix))
            {
                case "j02":
                    lis = new j02ContextMenu(Factory,pid,source).GetItems();break;
                case "p28":
                    lis = new p28ContextMenu(Factory, pid,source).GetItems(); break;
                case "p56":
                    lis = new p56ContextMenu(Factory, pid, source).GetItems(); break;
                case "p41":                
                    lis = new p41ContextMenu(Factory, pid, source).GetItems(); break;
                case "p31":
                    lis = new p31ContextMenu(Factory, pid, source,master_entity).GetItems(); break;
                case "o23":
                    lis = new o23ContextMenu(Factory, pid, source).GetItems(); break;
                case "b07":
                    lis = new b07ContextMenu(Factory, pid, source).GetItems(); break;
                case "p90":
                    lis = new p90ContextMenu(Factory, pid, source).GetItems(); break;
                case "p91":
                    lis = new p91ContextMenu(Factory, pid, source).GetItems(); break;
                default:
                    lis = new defContextMenu(Factory, pid, source,prefix).GetItems();
                    break;
                    
            }

            return _menusup.FlushResult_UL(lis, true, true, source);
        }
        public string CurrentUserMyProfile()
        {
            var lis = new NoContext_MyProfileMenu(Factory).GetItems();
            return _menusup.FlushResult_UL(lis, true, false);

           
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
            return _menusup.FlushResult_UL(_lis,true,false);            
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
          
            return _menusup.FlushResult_UL(_lis,true,false);
        }
        
       
        public string AdminMenu(string area, string prefix)
        {
            var lis = new NoContext_AdminMenu(Factory,area,prefix).GetItems();
            return _menusup.FlushResult_UL(lis, true, false);

            
        }
       
        

      

        
        public string MainMenu(string currenturl)
        {
            var cMenu = new UI.Menu.TheMenuSupport(Factory);
            var lis = cMenu.getAllMainMenuLinks();
            if (!string.IsNullOrEmpty(currenturl))
            {
                currenturl = System.Web.HttpUtility.UrlDecode(currenturl);
            }
            
            cMenu.HandleUrlOfGridLinks(lis);
            foreach (var c in lis)
            {
                var mi=AMI(c.Name, c.Url,c.Icon);
                if (currenturl !=null && currenturl.Contains(c.Url))
                {
                    mi.IsActive = true; //aktuální url, na kterém stojí uživatel
                }
                
            }

            var s = "<div>";
            s+= "<ul style='list-style-type:none; columns:2;-webkit-columns: 2;-moz-columns:2;'>";
            s += _menusup.FlushResult_UL(_lis,true, false);
            s += "</ul>";
            s += "<hr>";
            s += "<div><button type='button' class='btn btn-sm btn-outline-primary' onclick=\"_window_open('/Home/MyMainMenuLinks',1)\"><span class='material-icons-outlined-btn'>settings</span>MENU odkazy</button></div>";
            s += "</div>";

            return s;
        }
        public string MenuNewRecord()
        {
            var lis = new NoContext_NewRecMenu(Factory).GetItems();
            return _menusup.FlushResult_UL(lis,true, false);
        }

        public string TheGridSelMenu(TheGridUIContext tgi)  //menu pro označené grid záznamy
        {
            var lis = new NoContext_GridSelMenu(Factory, tgi).GetItems();
            return _menusup.FlushResult_UL(lis, true, false);

        }

        public string TheGridP31StateQuery(TheGridUIContext tgi)  //menu pro zobrazení filtrování stavu úkonů
        {
            _menusup = new TheMenuSupport(Factory);
            string s0 = "radio_button_unchecked"; string s1 = "radio_button_checked";
            string strKey = "grid-" + tgi.prefix;
            if (!string.IsNullOrEmpty(tgi.master_entity))
            {
                strKey += "-" + tgi.master_entity;
            }
            strKey += "-p31statequery";
            
            int val = Factory.CBL.LoadUserParamInt(strKey, 0);

            AMI("Nefiltrovat", "javascript:tg_p31statequery_change('0')", BO.BAS.IIFS(val == 0, s1, s0));
            var arr = new List<int> { 1, 2,16,17, 3,4,5,6,7,8,9,10,11,12,13,14,15 };
            if (tgi.pathname.Contains("p31approve/"))
            {
                //sada filtrovacích stavů pro schvalovací dialog
                arr = new List<int> {4, 5, 6, 7, 8, 9 };
            }
            foreach(int x in arr)
            {
                MenuItem mi=AMI(_menusup.getP31StateQueryAlias(x), $"javascript:tg_p31statequery_change('{x}')", BO.BAS.IIFS(val == x, s1, s0));
               
                if (x == val) mi.IsActive = true;

               
                
            }
            
            return _menusup.FlushResult_UL(_lis, true, false);      
            
        }

        public string TheGridDblclickSetting(TheGridUIContext tgi)  //menu pro nastavení doubleclick grid záznamu
        {
            string strIconEdit = "radio_button_unchecked"; string strIconPage = "radio_button_checked";
            if (Factory.CBL.LoadUserParam("grid-" + tgi.prefix + "-dblclick", "edit") == "edit")
            {
                strIconEdit = "radio_button_checked";
                strIconPage = "radio_button_unchecked";
            }
            HEADER("Nastavit si cíl dvojkliku myší na grid záznamu");
            AMI("Upravit kartu záznamu", "javascript:tg_dblclick_save_setting('edit')", strIconEdit);
            AMI("Otevřít stránku záznamu", "javascript:tg_dblclick_save_setting('recpage')", strIconPage);


            return _menusup.FlushResult_UL(_lis, true, false);
        }

        public string RecPageSetting(string prefix)  //menu pro nastavení stránky záznamu
        {
            string icon0 = "toggle_on";
            string icon1 = "toggle_off";

            HEADER("Nastavit zobrazení stránky");
            string skey = $"recpage-{prefix}-panel-grid";
            if (Factory.CBL.LoadUserParamBool(skey, true))
            {
                AMI("Zobrazovat nahoře GRID", $"javascript:save_page_setting('{skey}','0')", icon1);
            }
            else
            {
                AMI("Zobrazovat nahoře GRID", $"javascript:save_page_setting('{skey}','1')", icon0);
            }
            skey = $"recpage-{prefix}-panel-cm";
            if (Factory.CBL.LoadUserParamBool(skey, true))
            {
                AMI("Zobrazovat vlevo kontextové menu", $"javascript:save_page_setting('{skey}','0')", icon1);
            }
            else
            {
                AMI("Zobrazovat vlevo kontextové menu", $"javascript:save_page_setting('{skey}','1')", icon0);
            }

            return _menusup.FlushResult_UL(_lis, true, false);
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

        
       
    }
}