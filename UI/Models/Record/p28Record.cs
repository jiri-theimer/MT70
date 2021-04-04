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
        public string MultiselectJ02IDs { get; set; }

        public List<o37Repeater> lisO37 { get; set; }
        public List<o32Repeater> lisO32 { get; set; }
        public IEnumerable<BO.j02Person> lisJ02 { get; set; }

        public bool CanEditRecordCode { get; set; }
    }

    public class o37Repeater : BO.o37Contact_Address
    {       
        public bool IsTempDeleted { get; set; }
        public string TempGuid { get; set; }
        public string CssTempDisplay
        {
            get
            {
                if (this.IsTempDeleted)
                {
                    return "display:none;";
                }
                else
                {
                    return "display:table-row;";
                }
            }
        }
    }

    public class o32Repeater : BO.o32Contact_Medium
    {
        public bool IsTempDeleted { get; set; }
        public string TempGuid { get; set; }
        public string CssTempDisplay
        {
            get
            {
                if (this.IsTempDeleted)
                {
                    return "display:none;";
                }
                else
                {
                    return "display:table-row;";
                }
            }
        }
    }

    
}
