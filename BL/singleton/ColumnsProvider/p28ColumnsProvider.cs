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
            this.CurrentFieldGroup = "Root";
            oc = AF("p28Name", "Klient",null, "string");oc.NotShowRelInHeader = true;oc.DefaultColumnFlag = gdc1;
            AF("p28Code", "Kód");
            AF("p28CompanyShortName", "Zkrácený název");
            AF("p28RegID", "IČ").DefaultColumnFlag = gdc2;
            AF("p28VatID", "DIČ").DefaultColumnFlag = gdc1;
            AF("ClientPid", "PID", "convert(varchar(10),a.p28ID)");
            AF("p28ICDPH_SK", "IČ DPH (SK)");
            AF("p28SupplierID", "Kód dodavatele");
            AF("TagsHtml", "Štítky", "dbo.tag_values_inline_html(328,a.p28ID)");
            AF("TagsText", "Štítky (text)", "dbo.tag_values_inline(328,a.p28ID)");
            oc = AF("PocetProjektu", "Počet otevřených projektů", "op.PocetOtevrenychProjektu", "num0", true);oc.RelSqlInCol = "LEFT OUTER JOIN view_p28_projects op ON a.p28ID=op.p28ID";
            AF("p28ExternalPID", "Externí kód");
            AF("p28Person_BirthRegID", "RČ");

            this.CurrentFieldGroup = "Fakturační nastavení";
            AF("p28BillingMemo", "Fakturační poznámka");
            AF("p28Round2Minutes", "Zaokrouhlování hodin na míru", "case when a.p28Round2Minutes=0 then NULL else a.p28Round2Minutes end","num0");
            oc = AF("PrirazenyCenik", "Přiřazený fakturační ceník", "clientp51billing.p51Name");oc.RelSqlInCol = "LEFT OUTER JOIN p51PriceList clientp51billing ON a.p51ID_Billing=clientp51billing.p51ID";
            oc = AF("SkutecnyCenik", "Skutečný fakturační ceník", "dbo.get_billing_pricelist_name(NULL,a.p28ID)");
            oc = AF("FakturacniJazyk", "Fakturační jazyk", "p87.p87Name"); oc.RelSqlInCol = "LEFT OUTER JOIN p87BillingLanguage p87 On a.p87ID=p87.p87ID";
            AF("p28LimitHours_Notification", "WIP limit Fa hodin", "a.p28LimitHours_Notification", "num",true);
            AF("p28LimitFee_Notification", "WIP limitní honorář", "a.p28LimitFee_Notification", "num", true);

            this.CurrentFieldGroup = "Kontakty";
            AF("KontaktniMedia", "Kontaktní média", "dbo.p28_medias_inline(a.p28ID)");
            AF("FakturacniEmail", "Fakturační e-mail", "dbo.p28_medias_inline_invoice(a.p28ID)");
            AF("KontaktniOsoby", "Kontaktní osoby", "dbo.p28_ko_inline(a.p28ID)");
            oc=AF("FakturacniKontaktniOsoba", "Fakt.kontaktní osoba", "fko.Person");oc.RelSqlInCol = "LEFT OUTER JOIN view_p28_contactpersons_invoice fko On a.p28ID=fko.p28ID";

            this.CurrentFieldGroup = "Stromová struktura";
            oc=AF("ParentContact", "Nadřízený klienta", "parent.ParentName");oc.RelSqlInCol = "LEFT OUTER JOIN (select p28ID as ParentPID,isnull(p28TreePath,p28Name) as ParentName FROM p28Contact) parent ON a.p28ParentID=parent.ParentPID";
            AF("p28TreePath", "Stromový název", "a.p28TreePath");
            AF("p28TreeLevel", "Tree Level", "a.p28TreeLevel","num0");
            AF("HasChilds", "Má podřízené?", "convert(bit,case when a.p28TreeNext>a.p28TreePrev then 1 else 0 end)", "bool");
            AF("ChildContactsInline", "Podřízení klienti", "dbo.p28_get_childs_inline_print_version6(a.p28ID)");

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
