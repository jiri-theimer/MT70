using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XA.Models.fidoo
{
    public class ResponseKodDokladu
    {
        public string code { get; set; }
        public string errorId { get; set; }
        public string message { get; set; }
        public string timestamp { get; set; }
    }

    public class RequestKodDokladu
    {
        public string expenseId { get; set; }
        public string name { get; set; }
        public string externalReferenceId { get; set; }     //kód dokladu
    }
}
