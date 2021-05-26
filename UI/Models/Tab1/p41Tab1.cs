using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab1
{
    public class p41Tab1: BaseTab1ViewModel
    {
        public BO.p41Project Rec { get; set; }
        public BO.p28Contact RecClient { get; set; }
        public BO.p41ProjectSum RecSum { get; set; }

        public string p61Name { get; set; }
        public string p87Name { get; set; }
    }
}
