using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class p30Contact_Person:BaseBO
    {
        public int p28ID { get; set; }
        public int j02ID { get; set; }
        public int p41ID { get; set; }
        
        public string FullNameDesc { get; }
        public string p28Name { get; }
        public string p41Name { get; }
        public string p41Code { get; }
    }
}
