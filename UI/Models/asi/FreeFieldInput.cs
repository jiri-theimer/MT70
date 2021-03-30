using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class FreeFieldInput
    {
        public BO.x28EntityField Field { get; set; }
        public double NumInput { get; set; }
        public string StringInput { get; set; }
        public DateTime? DateInput { get; set; }

    }
}
