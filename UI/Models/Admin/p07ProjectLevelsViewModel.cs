using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p07ProjectLevelsViewModel:BaseViewModel
    {
        public BO.p07ProjectLevel RecL1 { get; set; }
        public BO.p07ProjectLevel RecL2 { get; set; }
        public BO.p07ProjectLevel RecL3 { get; set; }

        public bool UseL1 { get; set; }
        public bool UseL2 { get; set; }
        public bool UseL3 { get; set; }
    }
}
