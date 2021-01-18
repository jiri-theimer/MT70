using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XA.Models.fidoo
{
    public class RequestVydaje
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public DateTime lastModifyFrom { get; set; }
        public int limit { get; set; } = 100;

    }
}
