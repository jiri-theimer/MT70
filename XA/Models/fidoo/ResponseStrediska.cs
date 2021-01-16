using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XA.Models.fidoo
{

    public class ResponseStrediska
    {
        public List<Costcenterlist> costCenterList { get; set; }
        public bool complete { get; set; }
    }

    public class Costcenterlist
    {
        public string id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string state { get; set; }
    }

}
