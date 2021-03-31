using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class FreeFieldInput : x28EntityField
    {
        public string StringInput { get; set; }
        public double NumInput { get; set; }
        public DateTime? DateInput { get; set; }
        public bool CheckInput { get; set; }

        public bool IsExternalDataSource { get; set; }


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
