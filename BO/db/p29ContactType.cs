using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class p29ContactType:BaseBO
    {        
        public int b01ID { get; set; }
        public int x38ID { get; set; }
        public string b01Name { get; }
        public string x38Name { get; }

        public string p29Name { get; set; }        
        public int p29Ordinary { get; set; }
    }
}
