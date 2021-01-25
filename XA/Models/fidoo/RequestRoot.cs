using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XA.Models.fidoo
{
    public class RequestRoot
    {
        public Queryrequest queryRequest { get; set; }
    }

    public class Queryrequest
    {
        public int limit { get; set; } = 100;
        public int offset { get; set; }
        public List<Sort> sort { get; set; }
    }

    public class Sort
    {
        public bool ascendant { get; set; }
        public string property { get; set; }
    }
}
