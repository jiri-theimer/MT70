using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Info
{
    public class p31Info:BaseInfoViewModel
    {
        public bool IsRecord { get; set; }
        public BO.p31Worksheet Rec { get; set; }
        
        public BO.p91Invoice RecP91 { get; set; }
    }
}
