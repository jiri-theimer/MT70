using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p28Record:BaseRecordViewModel
    {
        public int IsCompany { get; set; }  //0,1
        public int p51Flag { get; set; }    //1 - nemá ceník, 2 - přiřazený ceník, 3 - ceník na míru
        public BO.p28Contact Rec { get; set; }

        public BO.o38Address RecFirstAddress { get; set; }
        public string SelectedComboP29Name { get; set; }
        public string SelectedComboP92Name { get; set; }
        public string SelectedComboP87Name { get; set; }
        public string SelectedComboJ61Name { get; set; }
        public string SelectedComboP63Name { get; set; }
        public string SelectedComboOwner { get; set; }
        public string SelectedComboP51Name { get; set; }
        public string SelectedComboParentP28Name { get; set; }

        public FreeFieldsViewModel ff1 { get; set; }

        public string TempGuid { get; set; }

        public IEnumerable<BO.o38Address> lisO38 { get; set; }
    }
}
