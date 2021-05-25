using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class x18Record: BaseRecordViewModel
    {
        public BO.x18EntityCategory Rec { get; set; }
        public BO.x29IdEnum SelectedX29ID { get; set; }

        public RoleAssignViewModel roles { get; set; }

        public List<x16Repeater> lisX16 { get; set; }
        public List<x20Repeater> lisX20 { get; set; }
    }



    public class x16Repeater : BO.x16EntityCategory_FieldSetting
    {        
       
        public bool IsTempDeleted { get; set; }
        public string TempGuid { get; set; }
        public string CssTempDisplay
        {
            get
            {
                if (this.IsTempDeleted == true)
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

    public class x20Repeater : BO.x20EntiyToCategory
    {
        public string ComboEntity { get; set; }
        public string ComboSelectedText { get; set; }
        public bool IsTempDeleted { get; set; }
        public string TempGuid { get; set; }
        public string CssTempDisplay
        {
            get
            {
                if (this.IsTempDeleted == true)
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
