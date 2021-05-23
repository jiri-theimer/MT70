using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class FsmViewModel:BaseViewModel
    {
        public string entity { get; set; }
        
        public TheGridInput gridinput { get; set; }
        
        public string entityTitle { get; set; }
        public string prefix { get; set; }
      
        public int master_pid { get; set; }        
        public string myqueryinline { get; set; } //explicitní myquery ve tvaru název@typ@hodnota, lze předávat více parametrů najednou

        public List<NavTab> NavTabs;

        public string go2pid_url_in_iframe { get; set; }

        public PeriodViewModel period { get; set; } //fixní filtr v horním pruhu

        public bool IsP31StateQuery { get; set; }
        public string P31StateQueryAlias { get; set; }
        public string P31StateQueryCssClass { get; set; }
        public bool IsCanbeMasterView { get; set; }
        public string dblClickSetting { get; set; }

        

    }


    
}
