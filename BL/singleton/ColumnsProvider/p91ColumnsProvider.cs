using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class p91ColumnsProvider:ColumnsProviderBase
    {
        public p91ColumnsProvider()
        {
            this.EntityName = "p91Invoice";


            this.CurrentFieldGroup = "Root";
            oc=AF("p91Code", "Číslo", null, "string");oc.DefaultColumnFlag = gdc1;oc.NotShowRelInHeader = true;oc.FixedWidth = 110;

            
            AF( "p91Text1", "Text faktury");
            AF( "p91Text2", "Technický text");
            AF( "ZapojeneOsoby", "Zapojené osoby","dbo.j02_invoiced_persons_inline(a.p91ID)");

            AFBOOL("p91IsDraft", "Draft");
            AF("TagsHtml", "Štítky", "dbo.tag_values_inline_html(391,a.p91ID)");
            AF("TagsText", "Štítky (text)", "dbo.tag_values_inline(391,a.p91ID)");

            this.CurrentFieldGroup = "Datum";
            oc=AFDATE("p91Date", "Vystaveno");oc.DefaultColumnFlag = gdc2;
            oc=AFDATE("p91DateSupply", "Datum plnění");oc.DefaultColumnFlag = gdc1;
            oc=AFDATE("p91DateMaturity", "Splatnost");oc.DefaultColumnFlag = gdc2;
            AF("DnuPoSplatnosti", "Dnů do splatnosti", "case When a.p91Amount_Debt=0 Then null Else datediff(day, p91DateMaturity, dbo.get_today()) End", "num0");
            AFDATE("p91DateBilled", "Datum úhrady");
            AFDATE("p91DateExchange", "Datum měn.kurzu");

            this.CurrentFieldGroup = "Částka";            
            oc =AF("p91Amount_WithoutVat", "Bez dph", null, "num", true);oc.DefaultColumnFlag = gdc1;
            AF("BezDphKratKurz", "Bez dph x Kurz", "case When a.j27ID=a.j27ID_Domestic Then p91Amount_WithoutVat Else p91Amount_WithoutVat*p91ExchangeRate End", "num", true);
            AF("p91Amount_Debt", "Dluh", null, "num", true);
            AF("DluhKratKurz", "Dluh x Kurz", "case When a.j27ID=a.j27ID_Domestic Then p91Amount_Debt Else p91Amount_Debt*p91ExchangeRate End", "num", true);
            oc=AF("p91Amount_TotalDue", "Celkem", null, "num", true);oc.DefaultColumnFlag = gdc1;
            AF("CelkemKratKurz", "Celkem x Kurz", "case When a.j27ID = a.j27ID_Domestic Then p91Amount_TotalDue Else p91Amount_TotalDue*p91ExchangeRate End", "num", true);
            AF("p91Amount_Vat", "Celkem dph", null, "num", true);
            AF("p91Amount_WithVat", "Vč.dph", null, "num", true);
            AF("p91RoundFitAmount", "Haléřové zaokrouhlení", null, "num", true);
            AF("p91Amount_WithoutVat_None", "Základ v nulové DPH", null, "num", true);
            AF("p91Amount_WithoutVat_Standard", "Základ ve standardní sazbě", null, "num", true);
            AF("p91Amount_WithoutVat_Low", "Základ ve snížené sazbě", null, "num", true);
            AF("p91Amount_WithoutVat_Special", "Základ ve speciální sazbě",null, "num", true);
            AF("p91Amount_Vat_Standard", "DPH ve standardní sazbě", null, "num", true);
            AF("p91Amount_Vat_Low", "DPH ve snížené sazbě", null, "num", true);
            AF("p91Amount_Vat_Special", "DPH ve speciální sazbě",null, "num", true);
            AF("p91VatRate_Standard", "DPH sazba standardní",null, "num", true);
            AF("p91VatRate_Low", "DPH sazba snížená",null, "num", true);
            AF("p91VatRate_Special", "DPH sazba speciální",null, "num", true);

            AF("p91ProformaBilledAmount", "Uhrazené zálohy", null, "num");
            AF("p91ExchangeRate", "Měnový kurz",null, "num");




            this.CurrentFieldGroup = "Klient ve vyúčtování";
            oc = AF("p91Client", "Klient"); oc.DefaultColumnFlag = gdc1;
            oc =AF( "p91Client_RegID", "IČ klienta"); oc.FixedWidth = 100;
            oc =AF("p91Client_VatID", "DIČ klienta");oc.FixedWidth = 100;
            oc=AF("p91Client_ICDPH_SK", "IČ DPH (SK)"); oc.FixedWidth = 100;
            AF("p91ClientAddress1_Street", "Ulice klienta");
            AF("p91ClientAddress1_City", "Město klienta");
            oc=AF("p91ClientAddress1_ZIP", "PSČ klienta"); oc.FixedWidth = 70;
            AF("p91ClientAddress1_Country", "Stát klienta");

            this.CurrentFieldGroup = "Elektronicky odesláno";
            oc=AF("VomKdyOdeslano", "Čas odeslání", "vom.Kdy_Odeslano","datetime");oc.RelSql = "LEFT OUTER JOIN view_p91_sendbyemail vom On a.p91ID = vom.p91ID";
            oc = AF("VomStav", "Stav odeslání", "vom.AktualniStav"); oc.RelSql = "LEFT OUTER JOIN view_p91_sendbyemail vom On a.p91ID = vom.p91ID";
            oc = AF("VomKomu", "Komu odesláno", "vom.Komu"); oc.RelSql = "LEFT OUTER JOIN view_p91_sendbyemail vom On a.p91ID = vom.p91ID";
            oc = AF("VomDateInsert", "Vloženo do fronty", "vom.Kdy_Zahajeno", "datetime"); oc.RelSql = "LEFT OUTER JOIN view_p91_sendbyemail vom On a.p91ID = vom.p91ID";

            oc =AF("BindClient", "Klient (vazba)", "p28client.p28Name");oc.RelSql = "LEFT OUTER JOIN p28Contact p28client ON a.p28ID=p28client.p28ID";
            oc = AF("ProjectClient", "Klient projektu", "projectclient.p28Name"); oc.RelSql = "LEFT OUTER JOIN p28Contact projectclient ON p41.p28ID_Client=projectclient.p28ID";


            AppendTimestamp();

        }
    }
}
