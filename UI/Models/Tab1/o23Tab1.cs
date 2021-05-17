using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab1
{
    public class o23Tab1:BaseTab1ViewModel
    {
        public BO.o23Doc Rec { get; set; }
        public IEnumerable<BO.o27Attachment> lisO27 { get; set; }
        public IEnumerable<BO.x16EntityCategory_FieldSetting> lisX16 { get; set; }

        public IEnumerable<BO.x19EntityCategory_Binding> lisX19 { get; set; }
    }
}
