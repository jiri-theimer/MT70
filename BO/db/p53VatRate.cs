using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class p53VatRate:BaseBO
    {
        public x15IdEnum x15ID { get; set; }
        public double p53Value { get; set; }
        public int j27ID { get; set; }
        public int j17ID { get; set; }

        public string x15Name { get; }
       
        public string j27Code { get; }
        
        public string j17Name { get; }
        
    }
}
