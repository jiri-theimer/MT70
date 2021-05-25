using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p41Record:BaseRecordViewModel
    {
        public BO.p41Project Rec { get; set; }
        public BO.p42ProjectType RecP42 { get; set; }
        public BO.p41Project RecParent { get; set; }
        public string TempGuid { get; set; }
        public int p51Flag { get; set; }    //1 - nemá ceník, 2 - přiřazený ceník, 3 - ceník na míru
        public RoleAssignViewModel roles { get; set; }
        public FreeFieldsViewModel ff1 { get; set; }
        public IEnumerable<BO.p07ProjectLevel> lisParentLevels { get; set; }
        public int SelectedParentLevelIndex { get; set; }
        
        public bool CanEditRecordCode { get; set; }
        public string SelectedComboParent { get; set; }
        public string SelectedComboJ18Name { get; set; }
        public string SelectedComboP42Name { get; set; }
        public string SelectedComboP92Name { get; set; }
        public string SelectedComboP87Name { get; set; }
        public string SelectedComboP61Name { get; set; }
        public string SelectedComboOwner { get; set; }
        public string SelectedComboP51Name { get; set; }
        public int SelectedP51ID_Flag3 { get; set; }
        public int SelectedP51ID_Flag2 { get; set; }

        public string SelectedComboClient { get; set; }
        public string SelectedComboOdberatel { get; set; }
    }
}
