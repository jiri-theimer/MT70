using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class o59FieldBagValue:BaseBO
    {
        public int o58ID { get; set; }
        public int o59RecPid { get; set; }
        public string o59RecPrefix { get; set; }
        public string o59ValueString { get; set; }
        public DateTime? o59ValueDate { get; set; }
        public double o59ValueNum { get; set; }
        public bool? o59ValueBoolean { get; set; }

        public string o58Name { get; }
        public string o58Code { get; }
    }
}
