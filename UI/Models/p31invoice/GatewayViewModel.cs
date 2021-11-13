using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p31invoice
{
    public class GatewayViewModel: BaseViewModel
    {
        public string tempguid { get; set; }

       
        public IEnumerable<BO.p31Worksheet> lisP31 { get; set; }

        public List<p91CreateItem> lisP91_Scale1 { get; set; }
        public List<p91CreateItem> lisP91_Scale2 { get; set; }
        public List<p91CreateItem> lisP91_Scale3 { get; set; }

        public DateTime? p91DateSupply { get; set; }
        public DateTime? p91Date { get; set; }
        public DateTime? p91Datep31_From { get; set; }
        public DateTime? p91Datep31_Until { get; set; }

        public bool IsDraft { get; set; }
        public int BillingScale { get; set; }   //1: vše dohromady, 2: za každého klienta, 3: za každý projekt

        public int SelectedInvoiceP91ID { get; set; }
        public string SelectedInvoiceText { get; set; }
    }
}
