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
        public string master_flag { get; set; } //dodatečný parametr k master_entity+master_pid, příklad: parent/founder

        public List<NavTab> NavTabs;

        public string go2pid_url_in_iframe { get; set; }

        public PeriodViewModel period { get; set; } //fixní filtr v horním pruhu

        

    }


    
}
