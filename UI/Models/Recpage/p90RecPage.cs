using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class p90RecPage:BaseRecPageViewModel
    {
        public BO.p90Proforma Rec { get; set; }
        public IEnumerable<BO.p82Proforma_Payment> lisP82 { get; set; }
        public IEnumerable<BO.p99Invoice_Proforma> lisP99 { get; set; }

    }
}
