using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class p90ColumnsProvider:ColumnsProviderBase
    {
        public p90ColumnsProvider()
        {
            this.EntityName = "p90Proforma";


            this.CurrentFieldGroup = "Root";
            oc = AF("p90Code", "Číslo zálohy", null, "string"); oc.DefaultColumnFlag = gdc1; oc.NotShowRelInHeader = true; oc.FixedWidth = 110;


            AF("p90Text1", "Text zálohy").DefaultColumnFlag = gdc2;
            AF("p90Text2", "Technický text");
           
            AFBOOL("p90IsDraft", "Draft");
            AF("TagsHtml", "Štítky", "dbo.tag_values_inline_html(390,a.p90ID)");
            AF("TagsText", "Štítky (text)", "dbo.tag_values_inline(390,a.p90ID)");

            oc = AFDATE("p90Date", "Vystaveno"); oc.DefaultColumnFlag = gdc2;            
            oc = AFDATE("p90DateMaturity", "Splatnost"); oc.DefaultColumnFlag = gdc2;

            AF("p90Amount", "Částka", null, "num", true).DefaultColumnFlag = gdc1;
            AF("p90Amount_Billed", "Uhrazeno", null, "num", true);
            AF("p90Amount_Debt", "Dluh", null, "num", true);
            AF("p90Amount_WithoutVat", "Bez DPH", null, "num", true);
            AF("p90Amount_Vat", "Částka DPH", null, "num", true);
            AFNUM0("p90VatRate", "Sazba DPH");

            this.CurrentFieldGroup = "Párování úhrad zálohy";
            AF("p91codes", "Spárované daňové faktury", "dbo.p90_get_bind_p91codes(a.p90ID)");
            AF("p82codes", "Spárované DPP", "dbo.p90_get_bind_p82codes(a.p90ID)");

            string s = "LEFT OUTER JOIN (select sum(p99Amount_WithoutVat) as JizSparovano,p82.p90ID FROM p99Invoice_Proforma a INNER JOIN p82Proforma_Payment p82 ON a.p82ID=p82.p82ID GROUP BY p82.p90ID) sparovano ON a.p90ID=sparovano.p90ID";
            s += " LEFT OUTER JOIN (select SUM(p82Amount_WithoutVat) as JizUhrazeno,p90ID FROM p82Proforma_Payment GROUP BY p90ID) uhrazeno ON a.p90ID=uhrazeno.p90ID";
            oc=AF("JizSparovano", "Spárované úhrady", "sparovano.JizSparovano", "num", true);oc.RelSqlInCol = s;
            oc = AF("ChybiSparovat", "Zbývá spárovat", "isnull(uhrazeno.JizUhrazeno,0)-isnull(sparovano.JizSparovano,0)", "num", true); oc.RelSqlInCol = s;

            AppendTimestamp();
        }
    }
}
