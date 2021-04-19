using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class p91RecPage:BaseRecPageViewModel
    {
        public BO.p91Invoice Rec { get; set; }
        public BO.p93InvoiceHeader RecP93 { get; set; }
        public BO.p86BankAcc RecP86 { get; set; }
        public IEnumerable<BO.p91_CenovyRozpis> lisCenovyRozpis { get; set; }

        public IEnumerable<BO.p31Worksheet> lisP31 { get; set; }

        public string StatByPrefix { get; set; }

        public BO.p91Invoice RecOpravovanyDoklad { get; set; }
        
    }

   
}
