using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab1
{
    public class p28Tab1:BaseTab1ViewModel
    {
        public BO.p28Contact Rec { get; set; }
        public BO.p28ContactSum RecSum { get; set; }
        public IEnumerable<BO.o38Address> lisO38 { get; set; }
        public IEnumerable<BO.j02Person> lisJ02 { get; set; }
        public IEnumerable<BO.o32Contact_Medium> lisO32 { get; set; }
    }
}
