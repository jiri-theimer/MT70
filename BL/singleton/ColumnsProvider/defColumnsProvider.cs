using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class defColumnsProvider:ColumnsProviderBase
    {
        public defColumnsProvider()
        {
            this.EntityName = "j03User"; //j03 = uživatelé
            AA("j03Login", "Login", gdc1, null, "string", false, true);
            AA("j04Name", "Role",gdc1, "j03_j04.j04Name", "string", false, true);
            AA("Lang", "Jazyk",gdc0, "case isnull(a.j03LangIndex,0) when 0 then 'Česky' when 1 then 'English' when 4 then 'Slovenčina' end");
            AA("j03Ping_TimeStamp", "Last ping", gdc0, "a.j03PingTimestamp", "datetime");
            AFBOOL("j03IsDebugLog", "Debug log");
            AppendTimestamp();

            this.EntityName = "j04UserRole";
            AA("j04Name", "Aplikační role", gdc1, null, "string", false, true);
            AppendTimestamp();

            this.EntityName = "j05MasterSlave";
            AA("MasterPerson", "Nadřízený", gdc1, "j02master.j02LastName+' '+j02master.j02FirstName+isnull(' '+j02master.j02TitleBeforeName,'')", "string", false, true);
            AA("SlavePerson", "Podřízený (jednotlivec)", gdc1, "j02slave.j02LastName+' '+j02slave.j02FirstName+isnull(' '+j02slave.j02TitleBeforeName,'')", "string");
            AA("SlaveTeam", "Podřízený tým", gdc1, "j11slave.j11Name", "string");
            AppendTimestamp();

            this.EntityName = "j07PersonPosition";
            AA("j07Name", "Pozice", gdc1, null, "string", false, true);
            AFNUM0("j07Ordinary", "#").DefaultColumnFlag = gdc2;
            AA("j07Name_BillingLang1", "Fakturační jazyk #1");
            AA("j07Name_BillingLang2", "Fakturační jazyk #2");
            AA("j07Name_BillingLang3", "Fakturační jazyk #3");
            AA("j07Name_BillingLang4", "Fakturační jazyk #4");
            AA("j07FreeText01", "Volné pole #1");
            AA("j07FreeText02", "Volné pole #2");
            AppendTimestamp();

            this.EntityName = "j18Region";
            AA("j18Name", "Středisko", gdc1, null, "string", false, true);
            AFNUM0("j18Ordinary", "#").DefaultColumnFlag = gdc2;
            AA("j18Code", "Kód");
            AppendTimestamp();

            this.EntityName = "j17Country";
            AA("j17Name", "Region", gdc1, null, "string", false, true);
            AFNUM0("j17Ordinary", "#");
            AA("j17Code", "Kód");
            AppendTimestamp();

            this.EntityName = "j19PaymentType";
            AA("j19Name", "Forma úhrady", gdc1, null, "string", false, true);
            AFNUM0("j19Ordinary", "#");

            this.EntityName = "c21FondCalendar";
            AA("c21Name", "Název fondu", gdc1, null, "string", false, true);
            AFNUM0("c21Ordinary", "#").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            this.EntityName = "c26Holiday";
            AFDATE("c26Date", "Datum").DefaultColumnFlag = gdc1;
            AA("c26Name", "Název svátku", gdc1, null, "string", false, true);
            AppendTimestamp();

            this.EntityName = "j11Team";
            AA("j11Name", "Tým osob", gdc1, null, "string", false, true);
            AppendTimestamp();

            this.EntityName = "j25ReportCategory";
            AA("j25Name", "Kategorie sestav", gdc1, null, "string", false, true);
            AFNUM0("j25Ordinary", "#").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            this.EntityName = "j27Currency";
            oc = AA("j27Code", "Měna", gdc1, null, "string", false, true);oc.FixedWidth = 70;
            AA("j27Name", "Název měny");

            this.EntityName = "j61TextTemplate";
            AA("j61Name", "Šablona zprávy", gdc1, null, "string", false, true);
            AA("j61MailSubject", "Předmět zprávy", gdc2);
            AA("j61MailTO", "TO");
            AA("j61MailCC", "CC");
            AA("j61MailBCC", "BCC");
            AppendTimestamp();

            //b01 = workflow šablona     
            this.EntityName = "b01WorkflowTemplate";
            oc=AA("b01Name", "Workflow šablona", gdc1, null, "string", false, true);
            oc = AA("b01Code", "Kód");oc.FixedWidth = 70;
            AppendTimestamp();

            //b02 = workflow stav                        
            this.EntityName = "b02WorkflowStatus";
            AA("b02Name", "Stav", gdc1);
            oc = AA("b02Code", "Kód stavu", gdc1);oc.FixedWidth = 70;
            AFBOOL("b02IsDefaultStatus", "Výchozí stav").DefaultColumnFlag = gdc2;
            AFNUM0("b02Order", "#").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            this.EntityName = "j90LoginAccessLog";
            AA("j90Date", "Čas", gdc1, null, "datetime");
            AA("j90BrowserFamily", "Prohlížeč", gdc1);
            AA("j90Platform", "OS", gdc1);
            AA("j90BrowserDeviceType", "Device");
            AA("j90ScreenPixelsWidth", "Šířka (px)", gdc1);
            AA("j90ScreenPixelsHeight", "Výška (px)", gdc1);
            AA("j90UserHostAddress", "Host", gdc1);
            AA("j90LoginMessage", "Chyba", gdc1);
            oc=AFNUM0("j90CookieExpiresInHours", "Expirace přihlášení");oc.DefaultColumnFlag = gdc1;
            AA("j90LoginName", "Login", gdc1);

            //j92 = ping log uživatelů
            this.EntityName = "j92PingLog";
            AA("j92Date", "Čas", gdc1, null, "datetime");
            AA("j92BrowserFamily", "Prohlížeč", gdc1);
            AA("j92BrowserOS", "OS", gdc1);
            AA("j92BrowserDeviceType", "Device", gdc1);
            AA("j92BrowserAvailWidth", "Šířka (px)", gdc1);
            AA("j92BrowserAvailHeight", "Výška (px)", gdc1);
            AA("j92RequestURL", "Url", gdc1);

            this.EntityName = "o15AutoComplete";
            AA("o15Value", "Hodnota", gdc1);
            AA("o15Flag", "Typ dat", gdc1, "case a.o15Flag when 1 then 'Titul před' when 2 then 'Titul za' when 3 then 'Pracovní funkce' when 328 then 'Stát' when 427 then 'URL adresa' end");
            oc=AFNUM0("o15Ordinary", "#");oc.DefaultColumnFlag = gdc2;

            this.EntityName = "p07ProjectLevel";
            AA("p07NameSingular", "Úroveň", gdc1, null, "string", false, true);
            oc=AFNUM0("p07Level", "Index úrovně");oc.DefaultColumnFlag = gdc1;
            AA("p07NamePlural", "Množné číslo", gdc2);
            AA("p07NameInflection", "Koho čeho");
            AppendTimestamp();

            this.EntityName = "p29ContactType";
            AA("p29Name", "Typ klienta", gdc1, null, "string", false, true);
            AFNUM0("p29Ordinary", "#");
            AppendTimestamp();

            this.EntityName = "p42ProjectType";
            AA( "p42Name", "Typ", gdc1, null, "string", false, true);
            AA("p42Code", "Kód");
            AFNUM0("p42Ordinary", "#");
            AppendTimestamp();

            this.EntityName = "p51PriceList";
            AA("p51TypeFlag", "Typ ceníku", gdc2, "case a.p51TypeFlag when 1 then 'Fakturační sazby' when 2 then 'Nákladové sazby' when 3 then 'Režijní sazby' when 5 then 'Kořenový (ROOT) ceník' when 4 then 'Efektivní sazby' end", "string", false, true);
            AA("p51Name", "Pojmenovaný ceník", gdc1, null, "string", false, true);
            AA("p51DefaultRateT", "Výchozí hod.sazba", gdc0, null, "num");
            AFNUM0("p51Ordinary", "#");
            oc=AFBOOL("p51IsCustomTailor", "Sazby na míru");oc.DefaultColumnFlag = gdc2;
            AppendTimestamp();

            this.EntityName = "p34ActivityGroup";
            AA("p34Name", "Sešit", gdc1, null, "string", false, true);
            AA("p34Code", "Kód", gdc2);
            AA("p33ID", "Vstupí data", gdc2, "case a.p33ID when 1 then 'Čas' when 2 then 'Peníze bez DPH' when 3 then 'Kusovník' when 5 then 'Peníze+DPH' end");
            AppendTimestamp();

            this.EntityName = "p32Activity";
            AA("p32Name", "Aktivita", gdc1, null, "string", false, true);
            AA("p32Code", "Kód", gdc2);
            oc=AFBOOL("p32IsBillable", "Fakturovatelný úkon");oc.DefaultColumnFlag = gdc2;
           oc= AFNUM0("p32Ordinary", "#");oc.DefaultColumnFlag = gdc2;
            oc=AFBOOL("p32IsTextRequired", "Povinný text úkonu");oc.DefaultColumnFlag = gdc2;
            AA("p32Value_Default", "Výchozí hodnota úkonu", gdc0, null, "num");
            AA("p32Value_Minimum", "MIN", gdc0, null, "num");
            AA("p32Value_Maximum", "MAX", gdc0, null, "num");
            AA("p32DefaultWorksheetText", "Výchozí text úkonu");
            AA("p32Name_BillingLang1", "Aktivita €1");
            AA("p32Name_BillingLang2", "Aktivita €2");
            AA("p32Name_BillingLang3", "Aktivita €3");
            AA("p32Name_BillingLang4", "Aktivita €4");
            AppendTimestamp();

            this.EntityName = "p38ActivityTag";
            AA("p38Name", "Odvětví aktivity", gdc1, null, "string", false, true);
            AFNUM0("p38Ordinary", "#").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            this.EntityName = "p36LockPeriod";
            AFDATE("p36DateFrom", "Od").DefaultColumnFlag = gdc1;
            AFDATE("p36DateUntil", "Do").DefaultColumnFlag = gdc1;
            AFBOOL("p36IsAllSheets", "Všechny sešity").DefaultColumnFlag = gdc2;
            AFBOOL("p36IsAllPersons", "Všechny osoby").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            this.EntityName = "p53VatRate";
            AA("p53Value", "Sazba DPH", gdc1, null, "num", false, true);
            AFDATE("p53ValidFrom", "Platí od").DefaultColumnFlag = gdc1;
            AFDATE("p53ValidUntil", "Platí do").DefaultColumnFlag = gdc1;
            AppendTimestamp();

            this.EntityName = "p61ActivityCluster";
            AA("p61Name", "Klast aktivit", gdc1, null, "string", false, true);
            AppendTimestamp();

            this.EntityName = "p63Overhead";
            AA("p63Name", "Režijní přirážka", gdc1, null, "string", false, true);
            AppendTimestamp();

            this.EntityName = "p80InvoiceAmountStructure";
            AA("p80Name", "Název rozpisu", gdc1, null, "string", false, true);
            AFBOOL("p80IsTimeSeparate", "Čas 1:1").DefaultColumnFlag = gdc2;
            AFBOOL("p80IsExpenseSeparate", "Výdaje 1:1").DefaultColumnFlag = gdc2;
            AFBOOL("p80IsFeeSeparate", "Pevné odměny 1:1").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            this.EntityName = "p87BillingLanguage";
            AA("p87Name", "Fakturační jazyk", gdc1, null, "string", false, true);
            AFNUM0("p87LangIndex", "#").DefaultColumnFlag = gdc2;

            this.EntityName = "p92InvoiceType";
            AA("p92Name", "Typ faktury", BO.TheGridDefColFlag.GridAndCombo, null, "string", false, true);
            AA("p92ReportConstantPreText1", "Preambule hlavního textu faktury");
            AA("p92InvoiceDefaultText1", "Výchozí hlavní text faktury");
            AA("p92ReportConstantText", "Preambule technického textu");
            AA("p92InvoiceDefaultText2", "Výchozí technický text");
            AA("p92AccountingIDS", "Předkontace v účetním IS");
            AA("p92ClassificationVATIDS", "Klasifikace DPH v účetním IS");
            AFNUM0("p92Ordinary", "#").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            this.EntityName = "p98Invoice_Round_Setting_Template";
            AA("p98Name", "Zaokrouhlovací pravidlo", gdc1, null, "string", false, true);
            AFBOOL("p98IsDefault", "Výchozí pravidlo").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            this.EntityName = "p89ProformaType";
            AA("p89Name", "Typ zálohy", gdc1, null, "string", false, true);
            AppendTimestamp();

            this.EntityName = "p93InvoiceHeader";
            AA("p93Name", "Vystavovatel faktury", gdc1, null, "string", false, true);
            AA("p93Company", "Firma", gdc2);
            AA("p93RegID", "IČ", gdc2);
            AA("p93VatID", "DIČ", gdc2);
            AA("p93City", "Město");
            AA("p93Street", "Ulice");
            AA("p93Zip", "PSČ");
            AppendTimestamp();

            this.EntityName = "p86BankAccount";
            AA( "p86Name", "Bankovní účet", gdc2, null, "string", false, true);
            AA("p86BankAccount", "Číslo účtu", gdc1);
            AA("p86BankCode", "Kód banky", gdc1);
            AA("p93Names", "Vazba na vystavovatele faktur", gdc2, "dbo.p86_get_p93names(a.p86ID)");
            AA("p86BankName", "Banka");
            AA("p86SWIFT", "SWIFT");
            AA("p86IBAN", "IBAN");
            AA("p86BankAddress", "Adresa banky");
            AppendTimestamp();

            this.EntityName = "p95InvoiceRow";
            AA("p95Name", "Fakturační oddíl", gdc1, null, "string", false, true);
            AA("p95Code", "Kód");
            AFNUM0("p95Ordinary", "#").DefaultColumnFlag = gdc2;
            AA("p95Name_BillingLang1", "Název €1");
            AA("p95Name_BillingLang2", "Název €2");
            AA("p95Name_BillingLang3", "Název €3");
            AA("p95Name_BillingLang4", "Název €4");
            AppendTimestamp();

            this.EntityName = "m62ExchangeRate";
            AFDATE("m62Date", "Datum kurzu").DefaultColumnFlag= gdc1;
            AA("m62Rate", "Kurz", gdc1, null, "num3");
            AA("Veta", "", gdc2, "CONVERT(varchar(10),a.m62Units)+' '+(select j27Code from j27Currency where j27ID=a.j27ID_Slave)+' = '+CONVERT(varchar(10),a.m62Rate)+' '+(select j27Code FROM j27Currency where j27ID=a.j27ID_Master)");
            AA("m62RateType", "Typ kurzu", gdc2, "case when a.m62RateType=1 then 'Fakturační kurz' else 'Fixní kurz' end");
            AppendTimestamp();

            this.EntityName = "p35Unit";
            AA("p35Name", "Kusovníková jednotka", gdc1, null, "string", false, true);
            AA("p35Code", "Kód");
            AppendTimestamp();

            //o40 = smtp poštovní účty   
            this.EntityName = "o40SmtpAccount";
            AA("o40Name", "Jméno odesílatele", gdc1, null, "string", false, true);
            AA("o40Server", "Smtp server", gdc2);
            AA("o40EmailAddress", "Adresa odesílatele", gdc1);
            AFNUM0("o40Port", "Smtp Port");
            AFBOOL("o40IsGlobalDefault", "Globální účet").DefaultColumnFlag = gdc1;

            //o53 = štítky
            this.EntityName = "o53TagGroup";
            AA("o53Name", "Název štítku", gdc1, null, "string", false, true);
            AFNUM0("o53Ordinary", "#").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            //o51 = Položky štítku
            this.EntityName = "o51Tag";
            AA("o51Name", "Položka štítku", gdc1, null, "string", false, true);            
            AFBOOL("o51IsColor", "Je barva?");
            AA("o51BackColor", "Barva pozadí", gdc1, "'<div style=\"background-color:'+a.o51BackColor+';\">...</div>'").FixedWidth = 110;
            AA("o51ForeColor", "Barva písma", gdc1, "'<div style=\"background-color:'+a.o51ForeColor+';\">...</div>'").FixedWidth = 110;
            AFNUM0("o51Ordinary", "#").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            //x18 = typy dokumentů
            this.EntityName = "x18EntityCategory";
            AA("x18Name", "Název", gdc1, null, "string", false, true);
            AFNUM0("x18Ordinary", "#").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            //x40 = OUTBOX     
            this.EntityName = "x40MailQueue";
            AA("MessageTime", "Čas", gdc1, "case when a.x40WhenProceeded is not null then a.x40WhenProceeded else a.x40DateInsert end", "datetime", false, true);
            AA("x40SenderName", "Odesílatel", gdc1);
            AA("x40SenderAddress", "Odesílatel (adresa)");
            AA("x40Recipient", "Komu", gdc1);
            AA("x40CC", "Cc");
            AA("x40BCC", "Bcc");
            AA("x40State", "Stav", gdc1, "case a.x40State when 1 then 'Čeká na odeslání' when 2 then 'Chyba' when 3 then 'Odesláno' when 4 then 'Zastaveno' when 5 then 'Čeká na schválení' end");
            AA("x40Subject", "Předmět zprávy", gdc1);
            //AF("x40MailQueue", "x40Body", "Text zprávy", BO.TheGridDefColFlag.GridAndCombo, "convert(varchar(150),a.x40Body)+'...'");
            AA("x40Attachments", "Přílohy", gdc1);
            AA("x40ErrorMessage", "Chyba", gdc1);



            //x31 = tisková sestava      
            this.EntityName = "x31Report";
            AA("x31Name", "Tisková sestava", gdc1, null, "string", false, true);
            AA("RepFormat", "Formát", gdc1, "case a.x31FormatFlag when 1 then 'REPORT' when 2 then 'DOCX' when 3 then 'PLUGIN' when 4 then 'XLS' end");
            AA("x31Code", "Kód sestavy");
            AFBOOL("x31IsPeriodRequired", "Filtr čaového období");

            AA("x31FileName", "Soubor šablony", gdc1);
            AFNUM0("x31Ordinary", "#").DefaultColumnFlag = gdc1;

            AA("x31ExportFileNameMask", "Maska export souboru");
            AA("x31IsScheduling", "Pravidelné odesílání");
            AA("x31Description", "Poznámka");
            AppendTimestamp();

            //uživatelská pole
            this.EntityName = "x28EntityField";
            AA("x28Name", "Uživatelské pole", gdc1, null, "string", false, true);
            AA("x28Field", "Fyzický název", gdc2);
            AFBOOL("x28IsRequired", "Povinné").DefaultColumnFlag = gdc1;
            AFNUM0("x28Ordinary", "#").DefaultColumnFlag = gdc1;
            AppendTimestamp();

            //skupina uživatelských polí
            this.EntityName = "x27EntityFieldGroup";
            AA("x27Name", "Skupina polí", gdc1, null, "string", false, true);
            AFNUM0("x27Ordinary", "#").DefaultColumnFlag = gdc2;

            //číselné řady
            this.EntityName = "x38CodeLogic";
            AA("x38Name", "Číselná řada", gdc1, null, "string", false, true);
            AA("x38ConstantBeforeValue", "Konstanta před", gdc2);
            AA("x38ConstantAfterValue", "Konstanta za", gdc2);
            AFNUM0("x38Scale", "Rozsah nul").DefaultColumnFlag = gdc2;
            AA("Maska", "Min-Max", 0, "case when a.x38MaskSyntax IS NULL then ISNULL(a.x38ConstantBeforeValue,'')+RIGHT('000000001',a.x38Scale)+' - '+ISNULL(a.x38ConstantBeforeValue,'')+RIGHT('99999999999',a.x38Scale) else a.x38MaskSyntax end");
            AppendTimestamp();

            //x51 = nápověda
            this.EntityName = "x51HelpCore";
            AA("x51Name", "Nápověda", gdc1, null, "string", false, true);
            AA("x51ViewUrl", "View Url", gdc2);
            AA("x51NearUrls", "Související Urls", gdc2);
            AA("x51ExternalUrl", "Externí Url");
            AppendTimestamp();

            //x55 = dashboard widget
            this.EntityName = "x55Widget";
            AA("x55Name", "Widget", gdc1, null, "string", false, true);
            AA("x55Code", "Kód widgetu", gdc2, null, "string", false, true);
            AA("x55Description", "Poznámka", gdc2);
            AA("x55Skin", "Cílový dashboard");
            AA("x55DataTablesLimit", "Minimum záznamů pro [DataTables]", gdc2);
            AFNUM0("x55Ordinal", "#").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            this.EntityName = "x67EntityRole";
            AA("x67Name", "Název role", gdc1, null, "string", false, true);
            AFNUM0("x67Ordinary", "#").DefaultColumnFlag = gdc2;
            AppendTimestamp();

            //x29 = entita
            this.EntityName = "x29Entity";
            AA("x29Name", "Entita", gdc1, null, "string", false, true);

            //x97 = překlad
            this.EntityName = "x97Translate";
            AA("x97Code", "Originál", gdc1, null, "string", false, true);
            AA("x97Lang1", "English", gdc1);
            AA("x97Lang2", "Deutsch", gdc1);
            AA("x97Lang4", "Slovenčina", gdc1);
            AA("x97OrigSource", "Zdroj");
            AppendTimestamp(false);

            this.EntityName = "x15VatRateType";
            AA("x15Name", "Druh DPH", gdc1, null, "string", false, true);
        }




        private BO.TheGridColumn AA(string strField, string strHeader, BO.TheGridDefColFlag dcf = BO.TheGridDefColFlag._none, string strSqlSyntax = null, string strFieldType = "string", bool bolIsShowTotals = false, bool bolNotShowRelInHeader = false)
        {
            oc=AF(strField, strHeader, strSqlSyntax, strFieldType, bolIsShowTotals);
            oc.DefaultColumnFlag = dcf;
            oc.NotShowRelInHeader = bolNotShowRelInHeader;

            return oc;
        }
    }
}
