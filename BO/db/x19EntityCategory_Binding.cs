using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class x19EntityCategory_Binding:BaseBO
    {
        public int x20ID { get; set; }
        public int o23ID { get; set; }
        public int x19RecordPID { get; set; }
        public string RecordAlias { get; }
        public int x29ID { get; set; }
        public int x18ID { get; }
        public string x18Name { get; }
        public string o23Name { get; }
        public string x20Name { get; }
        public bool x20IsMultiselect { get; }

        public bool IsSetAsDeleted { get; set; }
    }
}
