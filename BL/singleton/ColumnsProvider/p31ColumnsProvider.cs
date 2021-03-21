using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class p31ColumnsProvider:ColumnsProviderBase
    {        
        public p31ColumnsProvider()
        {
            this.EntityName = "p31Worksheet";

            this.CurrentFieldGroup = "Root";//-----------Root---------------------
            oc =AF("p31Text", "Text");oc.DefaultColumnFlag = gdc1;
            AF("p31Code", "Kód dokladu");
            AF("TagsHtml", "Štítky", "dbo.tag_values_inline_html(331,a.p31ID)");
            AF("TagsText", "Štítky (text)", "dbo.tag_values_inline(331,a.p31ID)");
            AF("p31RecordSourceFlag_Alias", "Zdrojová aplikace", "case a.p31RecordSourceFlag when 1 then 'Mobil' else 'MT' end");
            AF("p31DateTimeUntil_Orig", "Čas zapnutí stopek", null, "datetime");


            this.CurrentFieldGroup = "Datum a čas úkonu";//-----------Datum a čas úkonu---------------------
            oc =AFDATE("p31Date", "Datum");oc.DefaultColumnFlag = gdc1;
            AF("UkonYear", "Rok", "convert(varchar(4),a.p31Date)");
            AF("UkonMesic", "Měsíc", "convert(varchar(7),a.p31Date,126)");
            AF("UkonTyden", "Týden", "convert(varchar(4),year(a.p31Date))+'-'+convert(varchar(10),DATEPART(week,a.p31Date))");
            AF("p31DateTimeFrom_Orig", "Čas od", null, "time");
            AF("p31DateTimeUntil_Orig", "Čas do", null, "time");


            
            this.CurrentFieldGroup = "Vykázáno";//-----------Vykázáno---------------------
            AF("p31Value_Orig", "Vykázaná hodnota", null, "num");
            oc=AF("p31Hours_Orig", "Vykázané hodiny", null, "num", true); oc.DefaultColumnFlag = gdc1;
            AFNUM_OCAS("Vykazano_Hodiny_Fa", "Vykázané hodiny Fa", "p31_ocas.Vykazano_Hodiny_Fa", true);
            AFNUM_OCAS("Vykazano_Hodiny_NeFa", "Vykázané hodiny NeFa", "p31_ocas.Vykazano_Hodiny_NeFa", true);


            AF("p31HHMM_Orig", "Hodiny HH:MM");
            oc = AF("p31Rate_Billing_Orig", "Výchozí hodinová sazba", null, "num");oc.IHRC = true;
            oc = AF("p31Amount_WithoutVat_Orig", "Vykázáno bez DPH", null, "num", true); oc.IHRC = true;
            oc = AF("p31Amount_WithVat_Orig", "Vykázáno vč. DPH", null, "num", true); oc.IHRC = true;
            oc = AF("p31Amount_Vat_Orig", "Vykázáno DPH", null, "num", true); oc.IHRC = true;

            oc = AF("trimm_p72Name", "Status korekce", "p72trimm.p72Name");oc.RelSqlInCol = "LEFT OUTER JOIN p72PreBillingStatus p72trimm On a.p72ID_AfterTrimming=p72trimm.p72ID";

            oc = AFNUM_OCAS("VykazanoHodinyFaPoKorekci", "Hodiny Fa po korekci", "case when a.p72ID_AfterTrimming is null then p31_ocas.Vykazano_Hodiny_Fa else a.p31Hours_Trimmed end", true);
            oc = AF("Fakturacni_Honorar_Po_Korekci", "Fakturační honorář po korekci", "case when a.p72ID_AfterTrimming is not null then a.p31Hours_Trimmed*a.p31Rate_Billing_Orig else a.p31Hours_Orig*a.p31Rate_Billing_Orig end", "num", true); oc.IHRC = true;
            oc = AF("p31Amount_WithoutVat_AfterTrimming", "Bez DPH po korekci", "a.p31Amount_WithoutVat_AfterTrimming","num", true); oc.IHRC = true;

            this.CurrentFieldGroup = "Rozpracováno";//-----------Rozpracováno---------------------
            oc = AFNUM_OCAS("WIP_Hodiny", "Rozpr.hodiny", "p31_ocas.WIP_Hodiny",  true);
            oc = AFNUM_OCAS("WIP_Vydaje", "Rozpr.výdaj", "p31_ocas.WIP_Vydaje", true);
            oc = AFNUM_OCAS("WIP_BezDph", "Rozpr.bez DPH", "p31_ocas.WIP_BezDph", true); oc.IHRC = true;
            oc = AFNUM_OCAS("WIP_BezDph_EUR", "Rozpr.bez DPH EUR", "p31_ocas.WIP_BezDph_EUR", true); oc.IHRC = true;
            oc = AFNUM_OCAS("WIP_Honorar", "Rozpr.Honorář", "p31_ocas.WIP_Honorar", true); oc.IHRC = true;
            oc = AFNUM_OCAS("WIP_Vydaje_EUR", "Rozpr.výdaje EUR", "p31_ocas.WIP_Vydaje_EUR", true); oc.IHRC = true;
            oc = AFNUM_OCAS("WIP_Pausaly", "Rozpr.pevná odměna", "p31_ocas.WIP_Pausaly", true); oc.IHRC = true;
            oc = AFNUM_OCAS("WIP_Pausaly_EUR", "Rozpr.pevná odměna EUR", "p31_ocas.WIP_Pausaly_EUR", true); oc.IHRC = true;

            this.CurrentFieldGroup = "Nevyúčtováno";//-----------Nevyúčtováno---------------------
            oc = AFNUM_OCAS("Nevyfakturovano_BezDph", "Nevyúčtováno bez DPH", "p31_ocas.Nevyfakturovano_BezDph", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Nevyfakturovano_Hodiny", "Nevyúčtováné hodiny", "p31_ocas.Nevyfakturovano_Hodiny", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Nevyfakturovano_Vydaje", "Nevyúčtováný výdaj", "p31_ocas.Nevyfakturovano_Vydaje", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Nevyfakturovano_Pausaly", "Nevyúčtováná pevná odměna", "p31_ocas.Nevyfakturovano_Pausaly", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Nevyfakturovano_Schvalene_Hodiny", "Schválené hodiny - čeká na vyúčtování", "p31_ocas.Nevyfakturovano_Schvalene_Hodiny",  true); oc.IHRC = true;
            oc = AFNUM_OCAS("Nevyfakturovano_Schvalene_Hodiny_Pausal", "Schválené hodiny PAU - čeká na vyúčtování", "p31_ocas.Nevyfakturovano_Schvalene_Hodiny_Pausal",  true); oc.IHRC = true;
            oc = AFNUM_OCAS("Nevyfakturovano_Schvalene_Hodiny_Odpis", "Schválené hodiny ODPIS - čeká na vyúčtování", "p31_ocas.Nevyfakturovano_Schvalene_Hodiny_Odpis", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Nevyfakturovano_Schvaleno_BezDph", "Schváleno bez DPH - čeká na vyúčtování", "p31_ocas.Nevyfakturovano_Schvaleno_BezDph", true); oc.IHRC = true;


            this.CurrentFieldGroup = "Vyúčtováno";//-----------Vyúčtováno---------------------
            oc = AF("p31Hours_Invoiced", "Vyúčtované hodiny", null, "num", true);
            oc = AF( "p31HHMM_Invoiced", "Vyúčtováno HH:mm", null, "num");
            oc = AF( "p31Rate_Billing_Invoiced", "Vyúčtovaná hodinová sazba", null, "num"); oc.IHRC = true;
            oc = AF( "p70Name", "Fakturační status", "p70.p70Name"); oc.RelSqlInCol = "LEFT OUTER JOIN p70BillingStatus p70 On a.p70ID=p70.p70ID"; oc.IHRC = true;
            oc = AF( "p70Name_BillingLang1", "Fakturační status L1", "p70.p70Name_BillingLang1"); oc.RelSqlInCol = "LEFT OUTER JOIN p70BillingStatus p70 On a.p70ID=p70.p70ID"; oc.IHRC = true;
            oc = AF( "p70Name_BillingLang2", "Fakturační status L2", "p70.p70Name_BillingLang2"); oc.RelSqlInCol = "LEFT OUTER JOIN p70BillingStatus p70 On a.p70ID=p70.p70ID"; oc.IHRC = true;
            oc = AF( "p31Amount_WithoutVat_Invoiced", "Vyúčtováno bez DPH", null, "num", true); oc.IHRC = true;
            oc = AF( "p31Amount_WithVat_Invoiced", "Vyúčtováno vč. DPH", null, "num", true); oc.IHRC = true;
            oc = AF( "p31VatRate_Invoiced", "Vyúčtovaná DPH sazba", null, "num"); oc.IHRC = true;
            oc = AF( "p31Amount_WithoutVat_Invoiced_Domestic", "Vyúčtováno bez DPH x Kurz", null, "num", true); oc.IHRC = true;
            oc = AF( "j27Code_Billing_Invoice", "Měna vyúčtování", "j27billing_invoice.j27Code", "string"); oc.IHRC = true; oc.RelSqlInCol = "LEFT OUTER JOIN j27Currency j27billing_invoice ON a.j27ID_Billing_Invoiced=j27billing_invoice.j27ID";
            oc = AFNUM_OCAS( "Vyfakturovano_Hodiny_Fakturovat", "Vyúčt.hodiny [Fakturovat]", "p31_ocas.Vyfakturovano_Hodiny_Fakturovat",  true); oc.IHRC = true;
            oc = AFNUM_OCAS( "Vyfakturovano_Hodiny_Pausal", "Vyúčt.hodiny [Paušál]", "p31_ocas.Vyfakturovano_Hodiny_Pausal", true); oc.IHRC = true;
            oc = AFNUM_OCAS( "Vyfakturovano_Hodiny_Odpis", "Vyúčt.hodiny [Odpis]", "p31_ocas.Vyfakturovano_Hodiny_Odpis", true); oc.IHRC = true;
            oc = AFNUM_OCAS( "Vyfakturovano_Honorar", "Vyúčtovaný honorář", "p31_ocas.Vyfakturovano_Honorar", true); oc.IHRC = true;


            this.CurrentFieldGroup = "Nákladová cena";//-----------Nákladová cena---------------------
            oc = AF("p31Rate_Internal_Orig", "Nákladová sazba", null, "num"); oc.IHRC = true;
            oc = AF("p31Amount_Internal", "Nákladový honorář", null, "num", true); oc.IHRC = true;
            oc = AF("p31Rate_Overhead", "Režijní sazba", null, "num"); oc.IHRC = true;
            oc = AF("p31Amount_Overhead", "Režijní honorář", null, "num", true); oc.IHRC = true;
            oc = AF("p31Value_Off", "Off billing hodnota", null, "num", true); oc.IHRC = true;

            this.CurrentFieldGroup = "Přepočet podle fixního kurzu";//-----------Přepočet podle fixního kurzu---------------------
            oc = AF("p31ExchangeRate_Fixed", "Fixní kurz", null, "num"); oc.IHRC = true;
            oc = AF("p31Amount_WithoutVat_FixedCurrency", "Vykázáno bez DPH FK", "a.p31ExchangeRate_Fixed*a.p31Amount_WithoutVat_Orig", "num", true); oc.IHRC = true;
            oc = AFNUM_OCAS("WIP_BezDph_FK", "Rozpracováno bez DPH FK", "a.p31ExchangeRate_Fixed*p31_ocas.WIP_BezDph", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Nevyfakturovano_BezDph_FK", "Nevyúčtováno bez DPH FK", "a.p31ExchangeRate_Fixed*p31_ocas.Nevyfakturovano_BezDph", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Vykazano_Naklad_FK", "Vykázaný náklad FK", "p31_ocas.Vykazano_Naklad_FK",true); oc.IHRC = true;
            oc = AFNUM_OCAS("Vykazano_Vynos_FK", "Vykázaný výnos FK", "a.p31ExchangeRate_Fixed*p31_ocas.Vykazano_Vynos", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Vykazano_Zisk_FK", "Vykázaný zisk FK", "p31_ocas.Vykazano_Zisk_FK", true); oc.IHRC = true;


            this.CurrentFieldGroup = "Výsledovka z vykázaných hodnot";//-----------Výsledovka z vykázaných hodnot---------------------
            oc = AFNUM_OCAS("Vykazano_Naklad", "Vykázaný náklad", "p31_ocas.Vykazano_Naklad", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Vykazano_Vynos", "Vykázaný výnos", "p31_ocas.Vykazano_Vynos", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Vykazano_Zisk", "Vykázaný zisk", "p31_ocas.Vykazano_Zisk", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Vykazano_Naklad_Rezije", "Vykázaný režijní náklad", "p31_ocas.Vykazano_Naklad_Rezije", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Vykazano_Zisk_Rezije", "Vykázaný režijní zisk", "p31_ocas.Vykazano_Zisk_Rezije", true); oc.IHRC = true;

            this.CurrentFieldGroup = "Výsledovka z vyúčtovaných hodnot";//-----------Výsledovka z vyúčtovaných hodnot---------------------
            oc = AFNUM_OCAS("Vyfakturovano_Puvodni_Naklad_Domestic", "Vykázaný náklad x Kurz", "p31_ocas.Vyfakturovano_Puvodni_Naklad_Domestic", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Vyfakturovano_Vynos", "Vyúčtovaný výnos", "p31_ocas.Vyfakturovano_Vynos", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Vyfakturovano_Vynos_Domestic", "Vyúčtovaný výnos x Kurz", "p31_ocas.Vyfakturovano_Vynos_Domestic", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Vyfakturovano_Zisk", "Zisk po vyúčtování", "p31_ocas.Vyfakturovano_Zisk", true); oc.IHRC = true;
            oc = AFNUM_OCAS("Vyfakturovano_Zisk_Rezije", "Režijní zisk po vyúčtování", "p31_ocas.Vyfakturovano_Zisk_Rezije", true); oc.IHRC = true;

            this.CurrentFieldGroup = "Expense marže";//-----------Expense marže---------------------
            AF("p31MarginHidden", "Skrytá marže", null, "num");
            AF("p31MarginTransparent", "Přiznaná marže%", null, "num");
            AF("ExpenseAfterMarginHidden", "Výdaj po skryté marži", "a.p31Amount_WithoutVat_Orig+(a.p31Amount_WithoutVat_Orig*a.p31MarginHidden/100)", "num", true);
            AF("ExpenseAfterAllMargins", "Výdaj po obou maržích", "dbo.p31_get_expense_with_margins(a.p31Amount_WithoutVat_Orig,a.p31MarginHidden,a.p31MarginTransparent)", "num", true);
            AF("Odmena_Minus_Vydaj_Minus_HonorarR", "Odměna - Výdaj s marží - Režijní honorář", "(case when p34.p33ID IN (2,5) and p34.p34IncomeStatementFlag=2 then a.p31Amount_WithoutVat_Orig else 0 end) - (case when p34.p33ID IN (2,5) and p34.p34IncomeStatementFlag=1 then dbo.p31_get_expense_with_margins(a.p31Amount_WithoutVat_Orig,a.p31MarginHidden,a.p31MarginTransparent) else 0 end) - (case when p34.p33ID IN (1,3) then a.p31Hours_Orig*a.p31Rate_Overhead else 0 end)", "num", true);

            AppendTimestamp(true);
        }
    }
}
