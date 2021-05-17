using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab1
{
    public class x51Tab1 : BaseViewModel
    {
        public BO.x51HelpCore Rec { get; set; }
        public string InputViewUrl { get; set; }

        public string HtmlContent { get; set; }

        public string TagHtml { get; set; }

        public string Source { get; set; }
        
        public IEnumerable<BO.x51HelpCore> lisNear { get; set; }
        public int NearListFlag{ get; set; }
    }
}
