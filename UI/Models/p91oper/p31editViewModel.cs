using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p91oper
{
    public class p31editViewModel:BaseViewModel
    {        
        public int p31ID { get; set; }
        public BO.p31Worksheet Rec { get; set; }

        public BO.p91Invoice RecP91 { get; set; }

        public BO.p70IdENUM SelectedP70ID { get; set; }        
        public double p31Value_Invoiced { get; set; }
        public double p31Rate_Billing_Invoiced { get; set; }
        public double p31Amount_WithoutVat_Invoiced { get; set; }
        public double p31VatRate_Invoiced { get; set; }
        public double p31Value_FixPrice { get; set; }
        public string p31Text { get; set; }
        public string p31Code { get; set; }

        public string CssDisplayMoney
        {
            get
            {
                if (this.SelectedP70ID == BO.p70IdENUM.Vyfakturovano)
                {
                    return "table-row";
                }else
                {
                    return "none";
                }
            }
        }

    }
}
