using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class p28ColumnsProvider:ColumnsProviderBase
    {
        public p28ColumnsProvider()
        {
            this.EntityName = "p28Contact";

            oc = AF("p28Name", "Klient",null, "string");oc.NotShowRelInHeader = true;oc.DefaultColumnFlag = gdc1;
            AF("p28Code", "Kód");
            AF("p28CompanyShortName", "Zkrácený název");

            AF("p28RegID", "IČ").DefaultColumnFlag = gdc2;
            AF("p28VatID", "DIČ").DefaultColumnFlag = gdc1;
            AF("p28BillingMemo", "Fakturační poznámka");

            AFNUM0("p28Round2Minutes", "Zaokrouhlování času");

            AF("p28ICDPH_SK", "IČ DPH (SK)");
            AF("p28SupplierID", "Kód dodavatele");

            AF("TagsHtml", "Štítky", "dbo.tag_values_inline_html(328,a.p28ID)");
            AF("TagsText", "Štítky (text)", "dbo.tag_values_inline(328,a.p28ID)");
            AppendTimestamp();

            this.EntityName = "view_PrimaryAddress";
            oc = AF("FullAddress", "Fakturační adresa", "isnull(a.o38City+', ','')+isnull('<code>'+a.o38Street+'</code>','')+isnull(', '+a.o38ZIP,'')+isnull(' <var>'+a.o38Country+'</var>','')", "string");oc.DefaultColumnFlag = gdc2;oc.NotShowRelInHeader = true;
            AF("o38Street", "Ulice").DefaultColumnFlag = gdc2;
            AF("o38City", "Město").DefaultColumnFlag = gdc1;
            AF("o38ZIP", "PSČ");
            AF("o38Country", "Stát");
            AF("o38Name", "Název");
        }
    }
}
