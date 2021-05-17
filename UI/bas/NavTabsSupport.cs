using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI
{
    public class NavTabsSupport
    {
        private BL.Factory _f { get; set; }
        private List<NavTab> _tabs { get; set; }
        public NavTabsSupport(BL.Factory f)
        {
            _f = f;            
        }

        public List<NavTab> getTabs(string prefix,int pid,bool bolShowBadges)
        {
            _tabs = new List<NavTab>();
            string strBadge = null;

            switch (prefix)
            {
                case "j02":
                    if (bolShowBadges)
                    {
                        var c = _f.j02PersonBL.LoadSumRow(pid);
                        
                        if (c.p31_Wip_Time_Count > 0 || c.p31_Wip_Expense_Count > 0 || c.p31_Wip_Fee_Count > 0)
                        {
                            strBadge = c.p31_Wip_Time_Count.ToString() + c.p31_Wip_Expense_Count.ToString() + "+" + c.p31_Wip_Fee_Count.ToString();
                        }
                    }
                    _tabs.Add(AddTab("Tab1", "tab1", "/j02/Tab1?pid=" + AppendPid2Url(pid)));

                    _tabs.Add(AddTab("Poznámky", "b07Comment", "/b07/List?source=master"));
                    _tabs.Add(AddTab("Dokumenty", "o23Doc", "/TheGrid/SlaveView?prefix=o23"));
                    _tabs.Add(AddTab("Outbox", "x40MailQueue", "/TheGrid/SlaveView?prefix=x40"));
                    _tabs.Add(AddTab("PING Log", "j92PingLog", "/TheGrid/SlaveView?prefix=j92"));
                    _tabs.Add(AddTab("LOGIN Log", "j90LoginAccessLog", "/TheGrid/SlaveView?prefix=j90"));

                    break;

                case "p28":                    
                    _tabs.Add(AddTab("Tab1", "tab1", "/p28/Tab1?pid=" + AppendPid2Url(pid), false));
                    if (bolShowBadges)
                    {
                        var c = _f.p28ContactBL.LoadSumRow(pid);

                        if (c.p31_Wip_Time_Count > 0 || c.p31_Wip_Expense_Count > 0 || c.p31_Wip_Fee_Count > 0)
                        {
                            strBadge = c.p31_Wip_Time_Count.ToString() + c.p31_Wip_Expense_Count.ToString() + "+" + c.p31_Wip_Fee_Count.ToString();
                        }
                    }
                    _tabs.Add(AddTab("Úkony", "p31Worksheet", "/TheGrid/SlaveView?prefix=p31",true,strBadge));
                    _tabs.Add(AddTab("Hodiny", "p31time", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|time"));
                    _tabs.Add(AddTab("Výdaje", "p31expense", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|expense"));
                    _tabs.Add(AddTab("Odměny", "p31fee", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|fee"));
                    _tabs.Add(AddTab("Vyúčtování", "p91Invoice", "/TheGrid/SlaveView?prefix=p91"));
                    _tabs.Add(AddTab("Zálohy", "p90Proforma", "/TheGrid/SlaveView?prefix=p90"));
                    _tabs.Add(AddTab("Poznámky", "b07Comment", "/b07/List?source=master"));
                    _tabs.Add(AddTab("Dokumenty", "o23Doc", "SlaveView?prefix=o23"));
                    break;
                case "p41":
                    _tabs.Add(AddTab("Tab1", "tab1", "/p41/Tab1?pid=" + AppendPid2Url(pid), false));
                    _tabs.Add(AddTab("Úkony", "p31Worksheet", "/TheGrid/SlaveView?prefix=p31"));
                    _tabs.Add(AddTab("Hodiny", "p31time", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|time"));
                    _tabs.Add(AddTab("Výdaje", "p31expense", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|expense"));
                    _tabs.Add(AddTab("Odměny", "p31fee", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|fee"));
                    _tabs.Add(AddTab("Poznámky", "b07Comment", "/b07/List?source=master"));
                    _tabs.Add(AddTab("Dokumenty", "o23Doc", "/TheGrid/SlaveView?prefix=o23"));
                    break;
                case "o23":
                    _tabs.Add(AddTab("Tab1", "tab1", "/o23/Tab1?pid=" + AppendPid2Url(pid)));
                    _tabs.Add(AddTab("Poznámky", "b07Comment", "/b07/List?source=master"));
                    break;
                case "p90":
                    _tabs.Add(AddTab("Tab1", "tab1", "/p90/Tab1?pid=" + AppendPid2Url(pid)));
                    _tabs.Add(AddTab("Poznámky", "b07Comment", "/b07/List?source=master"));
                    break;
                case "p91":
                    _tabs.Add(AddTab("Tab1", "tab1", "/p91/Tab1?pid=" + AppendPid2Url(pid)));
                    _tabs.Add(AddTab("Úkony", "p31Worksheet", "/TheGrid/SlaveView?prefix=p31"));
                    _tabs.Add(AddTab("Hodiny", "p31time", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|time"));
                    _tabs.Add(AddTab("Výdaje", "p31expense", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|expense"));
                    _tabs.Add(AddTab("Odměny", "p31fee", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|fee"));
                    _tabs.Add(AddTab("Poznámky", "b07Comment", "/b07/List?source=master"));
                    break;

            }

            return _tabs;
            
        }

        private NavTab AddTab(string strName, string strTabKey, string strUrl, bool istranslate = true, string strBadge = null)
        {
            if (istranslate)
            {
                strName = _f.tra(strName);
            }
            return new NavTab() { Name = strName, Entity = strTabKey, Url = strUrl, Badge = strBadge };
        }

        private string AppendPid2Url(int go2pid)
        {
            if (go2pid > 0)
            {
                return go2pid.ToString();
            }
            else
            {
                return "@pid";
            }
        }
    }
}
