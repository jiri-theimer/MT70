using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridReportViewModel: BaseViewModel
    {
        public int j72id { get; set; }
        public string prefix { get; set; }
        public string master_prefix { get; set; }
        public int master_pid { get; set; }
        public List<int> pids { get; set; }

        public string TrdxRepSourceFileName { get; set; }
        public string TrdxRepDestFileName { get; set; }
        public string guid { get; set; }

        public string Header { get; set; }
        public int MarginLeft { get; set; }
        public int MarginTop { get; set; }
        public int MarginRight { get; set; }
        public int MarginBottom { get; set; }

        public int PageOrientation { get; set; }    //1:portrait, 2: landscape

        public bool IsLandScape
        {
            get
            {
                if (this.PageOrientation == 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
           
        }

        public string GroupByColumn { get; set; }   //sloupec pro souhrny
        public string PageBreakColumn { get; set; } //sloupec pro odstránkování

        public int ZoomPercentage { get; set; }
        public int MaxTopRecs { get; set; }

        public int ScopeRecs { get; set; }  //1:všechny, 2: pouze vybrané
    }
}
