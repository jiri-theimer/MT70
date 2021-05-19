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
        
        public List<NavTab> getTabs(string prefix,int pid)
        {
            _tabs = new List<NavTab>();
            
            switch (prefix)
            {
                case "j02":                    
                    _tabs.Add(AddTab("Tab1", "tab1", "/j02/Tab1?pid=" + AppendPid2Url(pid)));
                    var cpj02 = _f.j02PersonBL.LoadSumRow(pid);

                    _tabs.Add(AddTab("Úkony", "p31Worksheet", "/TheGrid/SlaveView?prefix=p31", true, Badge3(cpj02.p31_Wip_Time_Count, cpj02.p31_Wip_Expense_Count, cpj02.p31_Wip_Fee_Count, "badge bg-warning text-dark")));
                    _tabs.Add(AddTab("Hodiny", "p31time", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|time", true, Badge2(cpj02.p31_Wip_Time_Count, cpj02.p31_Approved_Time_Count)));
                    _tabs.Add(AddTab("Výdaje", "p31expense", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|expense", true, Badge2(cpj02.p31_Wip_Expense_Count, cpj02.p31_Approved_Expense_Count)));
                    _tabs.Add(AddTab("Odměny", "p31fee", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|fee", true, Badge2(cpj02.p31_Wip_Fee_Count, cpj02.p31_Approved_Fee_Count)));
                    _tabs.Add(AddTab("Vyúčtování", "p91Invoice", "/TheGrid/SlaveView?prefix=p91", true, Badge1(cpj02.p91_Count)));

                    _tabs.Add(AddTab("Úkoly", "p56Task", "/TheGrid/SlaveView?prefix=p56", true, Badge2(cpj02.p56_Actual_Count, cpj02.p56_Closed_Count, "badge bg-primary", "badge bg-primary")));
                    _tabs.Add(AddTab("Poznámky", "b07Comment", "/b07/List?source=master"));
                    _tabs.Add(AddTab("Dokumenty", "o23Doc", "/TheGrid/SlaveView?prefix=o23"));
                    _tabs.Add(AddTab("Outbox", "x40MailQueue", "/TheGrid/SlaveView?prefix=x40"));
                    _tabs.Add(AddTab("PING Log", "j92PingLog", "/TheGrid/SlaveView?prefix=j92"));
                    _tabs.Add(AddTab("LOGIN Log", "j90LoginAccessLog", "/TheGrid/SlaveView?prefix=j90"));

                    break;

                case "p28":                    
                    _tabs.Add(AddTab("Tab1", "tab1", "/p28/Tab1?pid=" + AppendPid2Url(pid), false));
                    var cp28 = _f.p28ContactBL.LoadSumRow(pid);

                    _tabs.Add(AddTab("Úkony", "p31Worksheet", "/TheGrid/SlaveView?prefix=p31",true,Badge3(cp28.p31_Wip_Time_Count,cp28.p31_Wip_Expense_Count,cp28.p31_Wip_Fee_Count,"badge bg-warning text-dark")));
                    _tabs.Add(AddTab("Hodiny", "p31time", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|time",true, Badge2(cp28.p31_Wip_Time_Count,cp28.p31_Approved_Time_Count)));
                    _tabs.Add(AddTab("Výdaje", "p31expense", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|expense",true, Badge2(cp28.p31_Wip_Expense_Count, cp28.p31_Approved_Expense_Count)));
                    _tabs.Add(AddTab("Odměny", "p31fee", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|fee",true, Badge2(cp28.p31_Wip_Fee_Count, cp28.p31_Approved_Fee_Count)));
                    _tabs.Add(AddTab("Vyúčtování", "p91Invoice", "/TheGrid/SlaveView?prefix=p91",true,Badge1(cp28.p91_Count)));
                    _tabs.Add(AddTab("Zálohy", "p90Proforma", "/TheGrid/SlaveView?prefix=p90",true, Badge1(cp28.p90_Count)));
                    _tabs.Add(AddTab("Poznámky", "b07Comment", "/b07/List?source=master",true, Badge1(cp28.b07_Count)));
                    _tabs.Add(AddTab("Dokumenty", "o23Doc", "SlaveView?prefix=o23",true, Badge1(cp28.o23_Count)));
                    break;
                case "p41":
                    var cp41 = _f.p41ProjectBL.LoadSumRow(pid);
                    _tabs.Add(AddTab("Tab1", "tab1", "/p41/Tab1?pid=" + AppendPid2Url(pid), false));
                    _tabs.Add(AddTab("Úkony", "p31Worksheet", "/TheGrid/SlaveView?prefix=p31",true, Badge3(cp41.p31_Wip_Time_Count, cp41.p31_Wip_Expense_Count, cp41.p31_Wip_Fee_Count, "badge bg-warning text-dark")));
                    _tabs.Add(AddTab("Hodiny", "p31time", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|time",true, Badge2(cp41.p31_Wip_Time_Count, cp41.p31_Approved_Time_Count)));
                    _tabs.Add(AddTab("Výdaje", "p31expense", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|expense",true, Badge2(cp41.p31_Wip_Expense_Count, cp41.p31_Approved_Expense_Count)));
                    _tabs.Add(AddTab("Odměny", "p31fee", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|fee",true, Badge2(cp41.p31_Wip_Fee_Count, cp41.p31_Approved_Fee_Count)));
                    _tabs.Add(AddTab("Vyúčtování", "p91Invoice", "/TheGrid/SlaveView?prefix=p91", true, Badge1(cp41.p91_Count)));
                    _tabs.Add(AddTab("Úkoly", "p56Task", "/TheGrid/SlaveView?prefix=p56", true, Badge2(cp41.p56_Actual_Count,cp41.p56_Closed_Count, "badge bg-primary", "badge bg-primary")));
                    _tabs.Add(AddTab("Poznámky", "b07Comment", "/b07/List?source=master",true, Badge1(cp41.b07_Count)));
                    _tabs.Add(AddTab("Dokumenty", "o23Doc", "/TheGrid/SlaveView?prefix=o23",true, Badge1(cp41.o23_Count)));
                    break;
                case "p56":
                    var cp56 = _f.p56TaskBL.LoadSumRow(pid);
                    _tabs.Add(AddTab("Tab1", "tab1", "/p56/Tab1?pid=" + AppendPid2Url(pid), false));
                    _tabs.Add(AddTab("Úkony", "p31Worksheet", "/TheGrid/SlaveView?prefix=p31", true, Badge3(cp56.p31_Wip_Time_Count, cp56.p31_Wip_Expense_Count, cp56.p31_Wip_Fee_Count, "badge bg-warning text-dark")));
                    _tabs.Add(AddTab("Hodiny", "p31time", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|time", true, Badge2(cp56.p31_Wip_Time_Count, cp56.p31_Approved_Time_Count)));
                    _tabs.Add(AddTab("Výdaje", "p31expense", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|expense", true, Badge2(cp56.p31_Wip_Expense_Count, cp56.p31_Approved_Expense_Count)));
                    _tabs.Add(AddTab("Odměny", "p31fee", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|fee", true, Badge2(cp56.p31_Wip_Fee_Count, cp56.p31_Approved_Fee_Count)));
                    _tabs.Add(AddTab("Vyúčtování", "p91Invoice", "/TheGrid/SlaveView?prefix=p91", true, Badge1(cp56.p91_Count)));
                    _tabs.Add(AddTab("Dokumenty", "o23Doc", "/TheGrid/SlaveView?prefix=o23", true, Badge1(cp56.o23_Count)));
                    break;
                case "o23":
                    _tabs.Add(AddTab("Tab1", "tab1", "/o23/Tab1?pid=" + AppendPid2Url(pid)));
                    _tabs.Add(AddTab("Poznámky", "b07Comment", "/b07/List?source=master"));
                    break;
                case "p90":
                    var cp90 = _f.p90ProformaBL.LoadSumRow(pid);
                    _tabs.Add(AddTab("Tab1", "tab1", "/p90/Tab1?pid=" + AppendPid2Url(pid)));
                    _tabs.Add(AddTab("Faktury (Vyúčtování)", "p91Invoice", "/TheGrid/SlaveView?prefix=p91", true, Badge1(cp90.p91_count)));
                    _tabs.Add(AddTab("Poznámky", "b07Comment", "/b07/List?source=master",true, Badge1(cp90.b07_count)));
                    break;
                case "p91":
                    _tabs.Add(AddTab("Tab1", "tab1", "/p91/Tab1?pid=" + AppendPid2Url(pid)));
                    var cp91 = _f.p91InvoiceBL.LoadSumRow(pid);                   
                    _tabs.Add(AddTab("Úkony", "p31Worksheet", "/TheGrid/SlaveView?prefix=p31",true, Badge3(cp91.p31_time_count,cp91.p31_expense_count,cp91.p31_fee_count)));
                    _tabs.Add(AddTab("Hodiny", "p31time", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|time",true, Badge1(cp91.p31_time_count)));
                    _tabs.Add(AddTab("Výdaje", "p31expense", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|expense",true, Badge1(cp91.p31_expense_count)));
                    _tabs.Add(AddTab("Odměny", "p31fee", "/TheGrid/SlaveView?prefix=p31&myqueryinline=tabquery|string|fee",true, Badge1(cp91.p31_fee_count)));
                    _tabs.Add(AddTab("Zálohy", "p90Proforma", "/TheGrid/SlaveView?prefix=p90", true, Badge1(cp91.p90_count)));
                    _tabs.Add(AddTab("Projekty", "p41Project", "/TheGrid/SlaveView?prefix=p41",true, Badge1(cp91.p41_count)));
                    _tabs.Add(AddTab("Zapojené osoby", "j02Person", "/TheGrid/SlaveView?prefix=j02", true, Badge1(cp91.j02_count)));
                    _tabs.Add(AddTab("Úkoly", "p56Task", "/TheGrid/SlaveView?prefix=p56", true, Badge1(cp91.p56_count)));
                    

                    _tabs.Add(AddTab("Poznámky", "b07Comment", "/b07/List?source=master", true, Badge1(cp91.b07_count)));
                    break;

            }

            return _tabs;
            
        }

        private string Badge1(int num1, string cssclassname = "badge bg-primary")
        {
            if (num1 == 0)
            {
                return null;
            }
            return $"<span class='{cssclassname}'>{num1}</span>";
        }
        private string Badge2(int num1, int num2, string cssclassname1 = "badge bg-warning text-dark", string cssclassname2 = "badge bg-success")
        {
            string s = null;
            if (cssclassname1== cssclassname2)
            {
                return $"<span class='{cssclassname1}'>{num1}+{num2}</span>";
            }
            if (num1 !=0)
            {
                s = $"<span class='{cssclassname1}'>{num1}</span>";
            }
            if (num2 != 0)
            {
                s += $"<span class='{cssclassname2}'>{num2}</span>";
            }
            return s;
        }
        private string Badge3(int num1,int num2,int num3,string cssclassname= "badge bg-primary")
        {            
            if (num1==0 && num2==0 && num3 == 0)
            {
                return null;
            }

            return $"<span class='{cssclassname}'>{num1}+{num2}+{num3}</span>";
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
