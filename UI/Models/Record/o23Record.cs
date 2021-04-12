using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class o23Record: BaseRecordViewModel
    {
        public BO.o23Doc Rec { get; set; }

        public int x18ID { get; set; }
        public BO.x18EntityCategory RecX18 { get; set; }
        public List<x19Repeator> lisX19 { get; set; }
        public IEnumerable<BO.x20EntiyToCategory> lisX20 { get; set; }
        public int SelectedX20ID { get; set; }
        public string SelectedBindName { get; set; }
        public IEnumerable<BO.x16EntityCategory_FieldSetting> lisX16 { get; set; }

        public List<DocFieldInput> lisFields { get; set; }

        public int SelectedBindPid { get; set; }
        public string SelectedBindText { get; set; }
        public string SelectedBindEntity { get; set; }
    }


    public class DocFieldInput : BO.x16EntityCategory_FieldSetting
    {
        public string StringInput { get; set; }
        public double NumInput { get; set; }
        public DateTime? DateInput { get; set; }
        public bool CheckInput { get; set; }

        

        public bool IsVisible { get; set; } = true;
        public string CssDisplay
        {
            get
            {
                if (this.IsVisible)
                {
                    return "inline-flex;";
                }
                else
                {
                    return "display:none;";
                }
            }
        }
    }

    public class x19Repeator : BO.x19EntityCategory_Binding
    {
        public string SelectedBindText { get; set; }
        public string SelectedX20Name { get; set; }
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
