using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class j02RecPage: BaseRecPageViewModel
    {
        
        public bool IsHover { get; set; }
        public BO.j02Person Rec { get; set; }
        public BO.j03User RecJ03 { get; set; }
       
        
    }
}
