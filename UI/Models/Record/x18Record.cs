using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class x18Record: BaseRecordViewModel
    {
        public BO.x18EntityCategory Rec { get; set; }

        public List<x16Repeater> lisX16 { get; set; }
        public List<BO.x20EntiyToCategory> lisX20 { get; set; }
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
}
