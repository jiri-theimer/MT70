using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class x21DatePeriod:BaseBO
    {        
        public int x21Ordinary { get; set; }  // ukládá se j03ID
        public string x21Name { get; set; }
        
        public DateTime x21ValidFrom { get; set; }
        public DateTime x21ValidUntil { get; set; }
        
    }
}
