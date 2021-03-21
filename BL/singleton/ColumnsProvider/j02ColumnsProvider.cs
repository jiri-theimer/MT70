using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class j02ColumnsProvider:ColumnsProviderBase
    {
        public j02ColumnsProvider()
        {
            this.EntityName = "j02Person";

            oc=AF("fullname_desc", "Příjmení+Jméno", "a.j02LastName+' '+a.j02FirstName+isnull(' '+a.j02TitleBeforeName,'')", "string",false);oc.DefaultColumnFlag = gdc1;oc.NotShowRelInHeader = true;

            oc=AF("fullname_asc", "Jméno+Příjmení", "isnull(a.j02TitleBeforeName+' ','')+a.j02FirstName+' '+a.j02LastName+isnull(' '+a.j02TitleAfterName,'')", "string");oc.NotShowRelInHeader = true;

            AF("j02Email", "E-mail").DefaultColumnFlag = gdc1;
            AF("j02FirstName", "Jméno");
            AF("j02LastName", "Příjmení");
            AF("j02TitleBeforeName", "Titul před");
            AF("j02TitleAfterName", "Titul za");
            AF("j02Phone", "TEL");
            AF("j02Mobile", "Mobil");
            AF("j02Code", "Kód");
            
            
            AFBOOL("j02IsIntraPerson", "Interní osoba");
            AF("j02InvoiceSignatureFile", "Grafický podpis");
            AF("j02Salutation", "Oslovení");
            AF("j02EmailSignature", "E-mail podpis");            

            AF("TagsHtml", "Štítky", "dbo.tag_values_inline_html(102,a.j02ID)");
            AF("TagsText", "Štítky (text)", "dbo.tag_values_inline(102,a.j02ID)");


            this.CurrentFieldGroup = "Kontaktní osoba klienta/projektu";
            AF("j02JobTitle", "Pozice na vizitce");
            AF("j02Office", "Adresa/Kancelář");
            AF("VazbaKlient", "Vazba na klienta", "dbo.j02_clients_inline(a.j02ID)");

            AppendTimestamp();
        }
    }
}
