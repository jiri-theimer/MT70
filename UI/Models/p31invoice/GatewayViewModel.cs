using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p31invoice
{
    public class GatewayViewModel: BaseViewModel
    {
        public string tempguid { get; set; }

        public int OneP28ID { get; set; }
        public string OneP28Name { get; set; }
        public IEnumerable<BO.p31Worksheet> lisP31 { get; set; }

        public List<BO.p91Create> lisP91 { get; set; }
        public DateTime? p91DateSupply { get; set; }
        public DateTime? p91Date { get; set; }
        public DateTime? p91Datep31_From { get; set; }
        public DateTime? p91Datep31_Until { get; set; }

        public bool IsDraft { get; set; }
        public int BillingScale { get; set; }   //1: vše dohromady, 2: za každého klienta, 3: za každý projekt

        
        
    }
}
