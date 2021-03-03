using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.basUI
{
    public class TheMenuSupport
    {
        private BL.Factory _f { get; set; }
        
        public TheMenuSupport(BL.Factory f)
        {
            _f = f;            
            
            
        }

        public List<MenuItem> getUserMenuLinks()
        {
            var ret = new List<MenuItem>();
            if (_f.CurrentUser.j03SiteMenuMyLinksV7 == null)
            {
                return ret;
            }
            var lis = getAllMainMenuLinks();

            var urls = BO.BAS.ConvertString2List(_f.CurrentUser.j03SiteMenuMyLinksV7);

            foreach(string strUrl in urls)
            {
                if (lis.Where(p => p.Url == strUrl).Any())
                {
                    ret.Add(lis.Where(p => p.Url == strUrl).First());
                }
            }
            //ret = lis.Where(p => urls.Contains(p.Url)).ToList();

            

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
                ret.Add(GRD("p31"));
                ret.Add(new MenuItem() { Name = _f.tra("Kalendář"), Url = "/p31/Calendar",ID="cmdCalendar" });
                ret.Add(new MenuItem() { Name = "Dayline",Url="/p31/Dayline",ID="cmdDayline"});
                ret.Add(new MenuItem() { Name = _f.tra("Součty"), Url = "/p31/Totals",ID="cmdTotals" });
            }
            if (_f.CurrentUser.j04IsMenu_Project)
            {
                ret.Add(GRD("p41"));
            }
            if (_f.CurrentUser.j04IsMenu_Contact)
            {
                ret.Add(GRD("p28"));
            }
            if (_f.CurrentUser.j04IsMenu_Invoice)
            {
                ret.Add(GRD("p91"));
            }
            if (_f.CurrentUser.j04IsMenu_Proforma)
            {
                ret.Add(GRD("p90"));
            }
            if (_f.CurrentUser.j04IsMenu_People)
            {
                ret.Add(GRD("j02"));
            }
            if (_f.CurrentUser.j04IsMenu_Notepad)
            {
                ret.Add(GRD("o23"));
            }
            if (_f.CurrentUser.j04IsMenu_Report)
            {
                ret.Add(new MenuItem() { Name = _f.tra("Tiskové sestavy"), Url = "/x31/ReportNoContextFramework",ID="cmdReports" });
            }

            return ret;
        }

        private MenuItem GRD(string prefix)
        {
            var c = new MenuItem();
           
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
                    c.Name = _f.tra("LIDÉ");
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

            c.ID = "cmd" + prefix;

            return c;
            
        }


        
    }
}
