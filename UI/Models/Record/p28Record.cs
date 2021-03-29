using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p28Record:BaseRecordViewModel
    {
        public int IsCompany { get; set; }  //0,1
        public BO.p28Contact Rec { get; set; }

        public int o38ID_First { get; set; }
        public BO.o38Address RecFirstAddress { get; set; }
        public string SelectedComboP29Name { get; set; }
        public string SelectedComboP92Name { get; set; }
        public string SelectedComboP87Name { get; set; }
        public string SelectedComboJ61Name { get; set; }
        public string SelectedComboP63Name { get; set; }
        public string SelectedComboOwner { get; set; }
    }
}
