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
            if (_f.CBL.LoadUserParamBool("grid-" + item.ID.Substring(3, 3) + "-show11", false))
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

            ret.Add(new MenuItem() { Name = _f.tra("Dashboard"), Url = "/Dashboard/Index",ID="cmdDashboard" });
            if (_f.CurrentUser.IsAdmin)
            {
                ret.Add(new MenuItem() { Name = _f.tra("Administrace"), Url = "/Admin/Index?signpost=true", ID = "cmdAdmin" });
            }
            if (_f.CurrentUser.j04IsMenu_Worksheet)
            {
                ret.Add(GRD("p31",false));
                ret.Add(new MenuItem() { Name = _f.tra("Kalendář"), Url = "/p31/Calendar",ID="cmdCalendar" });
                ret.Add(new MenuItem() { Name = "Dayline",Url="/p31/Dayline",ID="cmdDayline"});
                ret.Add(new MenuItem() { Name = _f.tra("Součty"), Url = "/p31/Totals",ID="cmdTotals" });
            }
            if (_f.CurrentUser.j04IsMenu_Project)
            {
                ret.Add(GRD("p41",true));
            }
            if (_f.CurrentUser.j04IsMenu_Contact)
            {
                ret.Add(GRD("p28",true));
            }
            if (_f.CurrentUser.j04IsMenu_Invoice)
            {
                ret.Add(GRD("p91",true));
            }
            if (_f.CurrentUser.j04IsMenu_Proforma)
            {
                ret.Add(GRD("p90",true));
            }
            if (_f.CurrentUser.j04IsMenu_People)
            {
                ret.Add(GRD("j02",true));
            }
            if (_f.CurrentUser.j04IsMenu_Notepad)
            {
                ret.Add(GRD("o23",true));
            }
            if (_f.CurrentUser.j04IsMenu_Report)
            {
                ret.Add(new MenuItem() { Name = _f.tra("Tiskové sestavy"), Url = "/x31/ReportNoContextFramework",ID="cmdReports" });
            }
            ret.Add(GRD("b07",false));


            ret.Add(new MenuItem() { Name = "Stránka projektu", Url = "/p41/RecPage", ID = "recpagep41" });
            ret.Add(new MenuItem() { Name = "Stránka klienta", Url = "/p28/RecPage", ID = "recpagep28" });
            ret.Add(new MenuItem() { Name = "Stránka vyúčtování", Url = "/p91/RecPage", ID = "recpagep91" });
            ret.Add(new MenuItem() { Name = "Stránka osoby", Url = "/j02/RecPage", ID = "recpagej02" });
            

            return ret;
        }

        private MenuItem GRD(string prefix,bool bolIsCanBeGrid11)
        {
            var c = new MenuItem() { ID = "cmd" + prefix,IsCanBeGrid11= bolIsCanBeGrid11 };
           
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


        
    }
}
