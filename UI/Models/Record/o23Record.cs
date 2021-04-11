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
        public List<BO.x19EntityCategory_Binding> lisX19 { get; set; }
        public IEnumerable<BO.x16EntityCategory_FieldSetting> lisX16 { get; set; }

        public List<DocFieldInput> lisFields { get; set; }
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
}
