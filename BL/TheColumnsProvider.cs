using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace BL
{
    public class TheColumnsProvider
    {
        private readonly BL.RunningApp _app;
        private readonly BL.TheEntitiesProvider _ep;
        private readonly BL.TheTranslator _tt;
        private List<BO.TheGridColumn> _lis;        
        private string _lastEntity;
        private string _curEntityAlias;
        
        public TheColumnsProvider(BL.RunningApp runningapp,BL.TheEntitiesProvider ep,BL.TheTranslator tt)
        {
            _app = runningapp;
            _ep = ep;
            _tt = tt;
            _lis = new List<BO.TheGridColumn>();
            SetupPallete();
            Handle_DbOpers();

        }

        public void Refresh()
        {
            _lis = new List<BO.TheGridColumn>();
            SetupPallete();
            Handle_DbOpers();
        }

        private int SetDefaultColWidth(string strFieldType)
        {
            switch (strFieldType)
            {
                case "date":
                    return 90;                    
                case "datetime":
                    return 120;                   
                case "num":
                case "num4":
                case "num5":
                case "num3":
                    return 100;                    
                case "num0":
                    return 75;
                case "bool":
                    return 75;
                default:
                    return 0;
            }
            
        }

        private BO.TheGridColumn AF(string strEntity, string strField, string strHeader, int intDefaultFlag = 0, string strSqlSyntax = null, string strFieldType = "string", bool bolIsShowTotals = false,bool bolNotShowRelInHeader=false)
        {
            if (strEntity != _lastEntity)
            {
                _curEntityAlias = _ep.ByTable(strEntity).AliasSingular;
            }
           
            _lis.Add(new BO.TheGridColumn() { Field = strField, Entity = strEntity, EntityAlias = _curEntityAlias, Header = strHeader, DefaultColumnFlag = intDefaultFlag, SqlSyntax = strSqlSyntax, FieldType = strFieldType, IsShowTotals = bolIsShowTotals,NotShowRelInHeader= bolNotShowRelInHeader,FixedWidth= SetDefaultColWidth(strFieldType),TranslateLang1=strHeader,TranslateLang2=strHeader,TranslateLang3=strHeader });
            _lastEntity = strEntity;
            return _lis[_lis.Count - 1];
        }

        


        private void AF_TIMESTAMP(string strEntity, string strField, string strHeader, string strSqlSyntax, string strFieldType)
        {
            if (strEntity != _lastEntity)
            {
                _curEntityAlias = _ep.ByTable(strEntity).AliasSingular;
            }
            _lis.Add(new BO.TheGridColumn() { IsTimestamp = true, Field = strField, Entity = strEntity, EntityAlias = _curEntityAlias, Header = strHeader, SqlSyntax = strSqlSyntax, FieldType = strFieldType, FixedWidth = SetDefaultColWidth(strFieldType) });
            _lastEntity = strEntity;
        }

        private void AppendTimestamp(string strEntity,bool include_validity =true)
        {
            string prefix = strEntity.Substring(0, 3);
            AF_TIMESTAMP(strEntity, "DateInsert_" + strEntity, "Založeno", "a."+ prefix+"DateInsert", "datetime");
            AF_TIMESTAMP(strEntity, "UserInsert_" + strEntity, "Založil", "a."+ prefix+"UserInsert", "string");
            AF_TIMESTAMP(strEntity, "DateUpdate_" + strEntity, "Aktualizace", "a."+ prefix+"DateUpdate", "datetime");
            AF_TIMESTAMP(strEntity, "UserUpdate_" + strEntity, "Aktualizoval", "a."+ prefix+"UserUpdate", "string");
            if (include_validity == true)
            {
                AF_TIMESTAMP(strEntity, "ValidFrom_" + strEntity, "Platné od", "a." + prefix + "ValidFrom", "datetime");
                AF_TIMESTAMP(strEntity, "ValidUntil_" + strEntity, "Platné do", "a." + prefix + "ValidUntil", "datetime");

                AF_TIMESTAMP(strEntity, "IsValid_" + strEntity, "Časově platné", string.Format("convert(bit,case when GETDATE() between a.{0}ValidFrom AND a.{0}ValidUntil then 1 else 0 end)", prefix), "bool");
            }
            
        }

        private void Handle_DbOpers()
        {
            //Překlad do ostatních jazyků: Musí být před načtením kategorií, aby se názvy kategorií nepřekládali!
            foreach (var col in _lis)
            {
                bool b = true;
                if (col.Header.Length > 3 && col.Header.Substring(0, 3) == "Col")
                {
                    b = false;
                }
                if (b)
                {
                    col.TranslateLang1 = _tt.DoTranslate(col.Header, 1);
                    col.TranslateLang2 = _tt.DoTranslate(col.Header, 2);
                }

            }

            BO.TheGridColumn onecol;
            DL.DbHandler db = new DL.DbHandler(_app.ConnectString, new BO.RunningUser(), _app.LogFolder);
            var dt = db.GetDataTable("select * from o53TagGroup WHERE o53Field IS NOT NULL AND o53Entities IS NOT NULL ORDER BY o53Ordinary");
            foreach (System.Data.DataRow dbrow in dt.Rows)
            {
               
                onecol = AF("o54TagBindingInline", dbrow["o53Field"].ToString(), dbrow["o53Name"].ToString(), 0, null, "string", false, true);
                onecol.VisibleWithinEntityOnly = dbrow["o53Entities"].ToString();
            }
            
            
        }
        private void SetupPallete()
        {
            BO.TheGridColumn onecol;

            //j02 = osoby
            AF("j02Person", "fullname_desc", "Příjmení+Jméno", 1, "a.j02LastName+' '+a.j02FirstName+isnull(' '+a.j02TitleBeforeName,'')", "string",false,true);
            AF("j02Person", "fullname_asc", "Jméno+Příjmení", 0, "isnull(a.j02TitleBeforeName+' ','')+a.j02FirstName+' '+a.j02LastName+isnull(' '+a.j02TitleAfterName,'')", "string", false, true);
            
                              
            AF("j02Person", "j02Email", "E-mail", 1);
            AF("j02Person", "j02FirstName", "Jméno");
            AF("j02Person", "j02LastName", "Příjmení");
            AF("j02Person", "j02TitleBeforeName", "Titul před");
            AF("j02Person", "j02TitleAfterName", "Titul za");
            AF("j02Person", "j02Phone", "TEL");
            AF("j02Person", "j02Mobile", "Mobil");
            AF("j02Person", "j02Code", "Kód");
            AF("j02Person", "j02JobTitle", "Pozice na vizitce");
            AF("j02Person", "j02Office", "Adresa");
            AF("j02Person", "j02IsIntraPerson", "Interní osoba", 0, null, "bool");
            AF("j02Person", "j02InvoiceSignatureFile", "Grafický podpis");
            AF("j02Person", "j02Salutation", "Oslovení");
            AF("j02Person", "j02EmailSignature", "E-mail podpis");
            AppendTimestamp("j02Person");

            //j03 = uživatelé
            AF("j03User", "j03Login", "Login", 1,null,"string",false,true);
            AF("j03User", "j04Name", "Role", 1, "j03_j04.j04Name","string",false,true);
            AF("j03User", "Lang", "Jazyk", 1, "case isnull(a.j03LangIndex,0) when 0 then 'Česky' when 1 then 'English' when 4 then 'Slovenčina' end");
            AF("j03User", "j03Ping_TimeStamp", "Last ping", 0, "a.j03PingTimestamp", "datetime");
            AF("j03User", "j03IsDebugLog", "Debug log", 0, null, "bool");
            AppendTimestamp("j03User");
            
            AF("j04UserRole", "j04Name", "Aplikační role", 1,null,"string",false,true);
            AppendTimestamp("j04UserRole");

            AF("j07PersonPosition", "j07Name", "Pozice", 1, null, "string", false, true);
            AF("j07PersonPosition", "j07Ordinary", "#", 2, null, "num0");
            AppendTimestamp("j07PersonPosition");

            AF("j18Region", "j18Name", "Středisko", 1, null, "string", false, true);
            AF("j18Region", "j18Ordinary", "#", 2, null, "num0");
            AF("j18Region", "j18Code", "Kód");
            AppendTimestamp("j18Region");

            AF("j17Country", "j17Name", "Region", 1, null, "string", false, true);
            AF("j17Country", "j17Ordinary", "#", 2, null, "num0");
            AF("j17Country", "j17Code", "Kód");
            AppendTimestamp("j17Country");

            AF("c21FondCalendar", "c21Name", "Název fondu", 1, null, "string", false, true);
            AF("c21FondCalendar", "c21Ordinary", "#", 2, null, "num0");
            AppendTimestamp("c21FondCalendar");

            AF("c26Holiday", "c26Date", "Datum", 1, null, "date");
            AF("c26Holiday", "c26Name", "Název svátku", 1, null, "string", false, true);            
            AppendTimestamp("c26Holiday");

            AF("j11Team", "j11Name", "Tým osob", 1, null, "string", false, true);           
            AppendTimestamp("j11Team");

            AF("j27Currency", "j27Code", "Měna", 1, null, "string", false, true);
            AF("j27Currency", "j27Name", "Název měny");


            //b01 = workflow šablona            
            AF("b01WorkflowTemplate", "b01Name", "Workflow šablona", 1, null, "string", false, true);
            onecol=AF("b01WorkflowTemplate", "b01Code", "Kód", 2);
            onecol.FixedWidth = 70;
            AppendTimestamp("b01WorkflowTemplate");

            //b02 = workflow stav            
            //AF("b02WorkflowStatus", "b02Name", "Stav", 1, null, "string", false, true);
            AF("b02WorkflowStatus", "b02Name", "Stav", 1);
            onecol =AF("b02WorkflowStatus", "b02Ident", "Kód stavu", 1);
            onecol.FixedWidth = 70;
            AF("b02WorkflowStatus", "b02IsDefaultStatus", "Výchozí stav", 2, null, "bool");
            AF("b02WorkflowStatus", "b02IsHoldStatus", "Záchytný stav", 2, null, "bool");
            AF("b02WorkflowStatus", "b02Order", "#", 2, null, "num0");           
            AppendTimestamp("b02WorkflowStatus");




            //b05 = workflow historie          
            AF("b05Workflow_History", "Kdy", "Čas", 1, "a.b05DateInsert", "datetime");
            AF("b05Workflow_History", "b05Comment", "Text", 1, null, "string", false, true);
            
            AF("b05Workflow_History", "b05IsCommentOnly", "Pouze komentář", 2, null, "bool");
            AF("b05Workflow_History", "b05IsManualStep", "Ruční krok", 0, null, "bool");
            AF("b05Workflow_History", "b05IsNominee", "Nominace řešitele", 2, null, "bool");
            AF("b05Workflow_History", "b05IsCommentRestriction", "Interní komentář", 2, null, "bool");            
            AppendTimestamp("b05Workflow_History",false);

            //b06 = workflow krok
            AF("b06WorkflowStep", "b06Name", "Workflow krok", 1, null, "string", false, true);
            AF("b06WorkflowStep", "b06Order", "#", 2, null, "num0");
            AppendTimestamp("b06WorkflowStep");

            //b65 = notifikační šablona
            AF("b65WorkflowMessage", "b65Name", "Notifikační šablona", 1, null, "string", false, true);
            AF("b65WorkflowMessage", "b65MessageSubject", "Předmět zprávy", 2);
            AF("b65WorkflowMessage", "SystemFlag", "🚩", 1, "case when isnull(a.b65SystemFlag,0)>0 then '<div style='+char(34)+'background-color:red;'+char(34)+'>&nbsp;</div>' end");
            AppendTimestamp("b65WorkflowMessage");
            
            
            //j90 = access log uživatelů
            AF("j90LoginAccessLog", "j90Date", "Čas", 1, null, "datetime");
            AF("j90LoginAccessLog", "j90BrowserFamily", "Prohlížeč", 1);
            AF("j90LoginAccessLog", "j90Platform", "OS", 1);
            AF("j90LoginAccessLog", "j90BrowserDeviceType", "Device");
            AF("j90LoginAccessLog", "j90ScreenPixelsWidth", "Šířka (px)", 1);
            AF("j90LoginAccessLog", "j90ScreenPixelsHeight", "Výška (px)", 1);
            AF("j90LoginAccessLog", "j90UserHostAddress", "Host", 1);
            AF("j90LoginAccessLog", "j90LoginMessage", "Chyba", 1);
            AF("j90LoginAccessLog", "j90CookieExpiresInHours", "Expirace přihlášení", 1, null, "num0");
            AF("j90LoginAccessLog", "j90LoginName", "Login", 1);

            //j92 = ping log uživatelů
            AF("j92PingLog", "j92Date", "Čas", 1, null, "datetime");
            AF("j92PingLog", "j92BrowserFamily", "Prohlížeč", 1);
            AF("j92PingLog", "j92BrowserOS", "OS", 1);
            AF("j92PingLog", "j92BrowserDeviceType", "Device", 1);
            AF("j92PingLog", "j92BrowserAvailWidth", "Šířka (px)", 1);
            AF("j92PingLog", "j92BrowserAvailHeight", "Výška (px)", 1);
            AF("j92PingLog", "j92RequestURL", "Url", 1);


            AF("o15AutoComplete", "o15Value", "Hodnota", 1);
            AF("o15AutoComplete", "o15Flag", "Typ dat", 1, "case a.o15Flag when 1 then 'Titul před' when 2 then 'Titul za' when 3 then 'Pracovní funkce' when 328 then 'Stát' when 427 then 'URL adresa' end");
            AF("o15AutoComplete", "o15Ordinary", "#", 2, null, "num0");

            AF("p42ProjectType", "p42Name", "Název", 1, null, "string", false, true);
            AppendTimestamp("p42ProjectType");

            AF("p51PriceList", "p51TypeFlag", "Typ ceníku", 1, "case a.p51TypeFlag when 1 then 'Fakturační sazby' when 2 then 'Nákladové sazby' when 3 then 'Režijní sazby' when 5 then 'Kořenový (ROOT) ceník' when 4 then 'Efektivní sazby' end", "string", false, true);
            AF("p51PriceList", "p51Name", "Pojmenovaný ceník", 1, null, "string", false, true);
            AF("p51PriceList", "p51DefaultRateT", "Výchozí hod.sazba",0,null,"num");
            AF("p51PriceList", "p51Ordinary", "#", 0, null, "num0");
            AF("p51PriceList", "p51IsCustomTailor", "Sazby na míru", 0, null, "bool");
            AppendTimestamp("p51PriceList");

            AF("p34ActivityGroup", "p34Name", "Sešit", 1, null, "string", false, true);
            AF("p34ActivityGroup", "p34Code", "Kód", 2);
            AF("p34ActivityGroup", "p33ID", "Vstupí data", 1, "case a.p33ID when 1 then 'Čas' when 2 then 'Peníze bez DPH' when 3 then 'Kusovník' when 5 then 'Peníze+DPH' end");
            AppendTimestamp("p34ActivityGroup");

            AF("p32Activity", "p32Name", "Aktivita", 1, null, "string", false, true);
            AF("p32Activity", "p32Code", "Kód", 2);
            AF("p32Activity", "p32IsBillable", "Fakturovatelný úkon", 1, null, "bool");
            AF("p32Activity", "p32Ordinary", "#", 2, null, "num0");
            AF("p32Activity", "p32IsTextRequired", "Povinný text úkonu", 2, null, "bool");            
            AF("p32Activity", "p32Value_Default", "Výchozí hodnota úkonu", 0, null, "num");
            AF("p32Activity", "p32Value_Minimum", "MIN", 0, null, "num");
            AF("p32Activity", "p32Value_Maximum", "MAX", 0, null, "num");            
            AF("p32Activity", "p32DefaultWorksheetText", "Výchozí text úkonu");
            AF("p32Activity", "p32Name_BillingLang1", "Aktivita €1");
            AF("p32Activity", "p32Name_BillingLang2", "Aktivita €2");
            AF("p32Activity", "p32Name_BillingLang3", "Aktivita €3");
            AF("p32Activity", "p32Name_BillingLang4", "Aktivita €4");
            AppendTimestamp("p32Activity");

            AF("p38ActivityTag", "p38Name", "Odvětví aktivity", 1, null, "string", false, true);
            AF("p38ActivityTag", "p38Ordinary", "#", 2, null, "num0");
            AppendTimestamp("p38ActivityTag");

            AF("p36LockPeriod", "p36DateFrom", "Od", 1, null, "date");
            AF("p36LockPeriod", "p36DateUntil", "Do", 1, null, "date");
            AF("p36LockPeriod", "p36IsAllSheets", "Všechny sešity", 2, null,"bool");
            AF("p36LockPeriod", "p36IsAllPersons", "Všechny osoby", 2, null, "bool");            
            AppendTimestamp("p36LockPeriod");

            AF("p61ActivityCluster", "p61Name", "Klast aktivit", 1, null, "string", false, true);            
            AppendTimestamp("p61ActivityCluster");

            AF("p92InvoiceType", "p92Name", "Typ faktury", 1, null, "string", false, true);
            AF("p92InvoiceType", "p92Ordinary", "#", 2, null, "num0");
            AppendTimestamp("p92InvoiceType");

            AF("p93InvoiceHeader", "p93Name", "Vystavovatel faktury", 1, null, "string", false, true);
            AF("p93InvoiceHeader", "p93Company", "Firma",1);
            AF("p93InvoiceHeader", "p93RegID", "IČ",2);
            AF("p93InvoiceHeader", "p93VatID", "DIČ",2);
            AF("p93InvoiceHeader", "p93City", "Město");
            AF("p93InvoiceHeader", "p93Street", "Ulice");
            AF("p93InvoiceHeader", "p93Zip", "PSČ");            
            AppendTimestamp("p93InvoiceHeader");

            AF("p86BankAccount", "p86Name", "Bankovní účet", 1, null, "string", false, true);
            AF("p86BankAccount", "p86BankAccount", "Číslo účtu",1);
            AF("p86BankAccount", "p86BankCode", "Kód banky", 1);
            AF("p86BankAccount", "p93Names", "Vazba na vystavovatele faktur", 2, "dbo.p86_get_p93names(a.p86ID)");
            AF("p86BankAccount", "p86BankName", "Banka");
            AF("p86BankAccount", "p86SWIFT", "SWIFT");
            AF("p86BankAccount", "p86IBAN", "IBAN");
            AF("p86BankAccount", "p86BankAddress", "Adresa banky");
            AppendTimestamp("p86BankAccount");

            AF("p95InvoiceRow", "p95Name", "Fakturační oddíl", 1, null, "string", false, true);
            AF("p95InvoiceRow", "p95Code", "Kód");
            AF("p95InvoiceRow", "p95Ordinary", "#", 2, null, "num0");
            AF("p95InvoiceRow", "p95Name_BillingLang1", "Název €1");
            AF("p95InvoiceRow", "p95Name_BillingLang2", "Název €2");
            AF("p95InvoiceRow", "p95Name_BillingLang3", "Název €3");
            AF("p95InvoiceRow", "p95Name_BillingLang4", "Název €4");
            AppendTimestamp("p95InvoiceRow");

            AF("p35Unit", "p35Name", "Kusovníková jednotka", 1, null, "string", false, true);                        

            //x40 = OUTBOX            
            AF("x40MailQueue", "MessageTime", "Čas", 1, "case when a.x40DatetimeProcessed is not null then a.x40DatetimeProcessed else a.x40DateInsert end", "datetime",false,true);            
            AF("x40MailQueue", "x40SenderName", "Odesílatel", 0);
            AF("x40MailQueue", "x40SenderAddress", "Odesílatel (adresa)");
            AF("x40MailQueue", "x40Recipient", "Komu", 1);
            AF("x40MailQueue", "x40CC", "Cc");
            AF("x40MailQueue", "x40BCC", "Bcc");
            AF("x40MailQueue", "x40Status", "Stav", 1, "case a.x40Status when 1 then 'Čeká na odeslání' when 2 then 'Chyba' when 3 then 'Odesláno' when 4 then 'Zastaveno' when 5 then 'Čeká na schválení' end");
            AF("x40MailQueue", "x40Subject", "Předmět zprávy", 1);
            AF("x40MailQueue", "x40Body", "Text zprávy", 1, "convert(varchar(150),a.x40Body)+'...'");
            AF("x40MailQueue", "x40Attachments", "Přílohy");
            AF("x40MailQueue", "x40EmlFileSize_KB", "Velikost (kB)", 0, "a.x40EmlFileSize/1024", "num0", true);
            AF("x40MailQueue", "x40EmlFileSize_MB", "Velikost (MB)", 0, "convert(float,a.x40EmlFileSize)/1048576", "num", true);
            AF("x40MailQueue", "x40ErrorMessage", "Chyba", 1);



            //x31 = tisková sestava
            AF("x31Report", "x31Name", "Tisková sestava", 1, null, "string", false, true);
            AF("x31Report", "x31PID", "Kód sestavy",2);
            AF("x31Report", "x31Is4SingleRecord", "Kontextová sestava", 2, null, "bool");
            
            AF("x31Report", "Roles", "Oprávnění", 2, "dbo._core_x31_get_role_inline(a.x31ID)");
            AF("x31Report", "Categories", "Kategorie", 1, "dbo._core_x31_get_category_inline(a.x31ID)");
            AF("x31Report", "a10Names", "Typy akcí", 2, "dbo._core_x31_get_a10name_inline(a.x31ID)");
            AF("x31Report", "a08Names", "Témata akcí", 2, "dbo._core_x31_get_a08name_inline(a.x31ID)");
            AF("x31Report", "RepFormat", "Formát", 1, "case a.x31ReportFormat when 1 then 'REP' when 2 then 'DOCX' when 3 then 'XLSX' when 4 then 'MSREP' end");
            AF("x31Report", "x31Description", "Popis");
            AppendTimestamp("x31Report");


            //x55 = dashboard widget
            AF("x55Widget", "x55Name", "Widget", 1, null, "string", false, true);
            AF("x55Widget", "x55Code", "Kód widgetu", 0, null, "string", false, true);
            AF("x55Widget", "x55Description", "Poznámka", 1);
            AF("x55Widget", "x55Skin", "Cílový dashboard");
            AF("x55Widget", "x55DataTablesLimit", "Minimum záznamů pro [DataTables]", 2);
            AF("x55Widget", "x55Ordinal", "#", 2, null, "num0");
            AppendTimestamp("x55Widget");

            AF("x67EntityRole", "x67Name", "Název role", 1, null, "string", false, true);
            AF("x67EntityRole", "x67Ordinary", "#", 2, null, "num0");
            AppendTimestamp("x67EntityRole");

            //x29 = entita
            AF("x29Entity", "x29Name", "Entita", 1, null, "string", false, true);
            AF("x29Entity", "x29NamePlural", "Plurál", 2);
            AF("x29Entity", "x29IsAttachment", "Přílohy", 2,null,"bool");

            //x91 = entita
            AF("x97Translate", "x97Code", "Originál", 1, null, "string", false, true);
            AF("x97Translate", "x97Lang1", "English", 1);
            AF("x97Translate", "x97Lang4", "Slovenčina", 1);

            AF("x15VatRateType", "x15Name", "Druh DPH", 1, null, "string", false, true);


            SetupP28();
        }

        private void SetupP28()
        {
            AF("p28Contact", "p28Name", "Klient", 1, null, "string", false, true);
            AF("p28Contact", "p28Code", "Kód");
            AF("p28Contact", "p28CompanyShortName", "Zkrácený název");
            
            AF("p28Contact", "p28RegID", "IČ");
            AF("p28Contact", "p28VatID", "DIČ", 2);
            AF("p28Contact", "p28BillingMemo", "Fakturační poznámka");

            AF("p28Contact", "p28Round2Minutes", "Zaokrouhlování času", 0, null, "num0");
            
            AF("p28Contact", "p28ICDPH_SK", "IČ DPH (SK)");
            AF("p28Contact", "p28SupplierID", "Kód dodavatele");
            
            
        }
        
        public List<BO.TheGridColumn> getDefaultPallete(bool bolComboColumns, BO.baseQuery mq)
        {
            int intDefaultFlag1 = 1; int intDefaultFlag2 = 2;
            if (bolComboColumns == true)
            {
                intDefaultFlag2 = 3;
            }

            List<BO.TheGridColumn> ret = new List<BO.TheGridColumn>();
            foreach (BO.TheGridColumn c in _lis.Where(p => p.Prefix == mq.Prefix && (p.DefaultColumnFlag == intDefaultFlag1 || p.DefaultColumnFlag == intDefaultFlag2)))
            {
                ret.Add(Clone2NewInstance(c));
            }

            //List<BO.TheGridColumn> ret = _lis.Where(p => p.Prefix == mq.Prefix && (p.DefaultColumnFlag == intDefaultFlag1 || p.DefaultColumnFlag == intDefaultFlag2)).ToList();

            List<BO.EntityRelation> rels = _ep.getApplicableRelations(mq.Prefix);

            switch (mq.Prefix)
            {
                case "j02":
                    ret.Add(InhaleColumn4Relation("j02_j03", "j03User", "j03Login", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("j02_j03", "j03User", "j04Name", rels, bolComboColumns));
                    

                    break;
                
               
                case "b02":
                    ret.Add(InhaleColumn4Relation("b02_b01", "b01WorkflowTemplate", "b01Name", rels, bolComboColumns));
                   
                    break;
                case "b05":
                    ret.Add(InhaleColumn4Relation("b05_j03", "j03User", "j03Login", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("b05_b06", "b06WorkflowStep", "b06Name", rels, bolComboColumns));
                    break;
                case "b65":
                    ret.Add(InhaleColumn4Relation("b65_x29", "x29Entity", "x29Name", rels, bolComboColumns));
                    break;
                case "p51":
                    ret.Add(InhaleColumn4Relation("p51_j27", "j27Currency", "j27Code", rels, bolComboColumns));
                    break;
                case "p32":
                    ret.Add(InhaleColumn4Relation("p32_p34", "p34ActivityGroup", "p34Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p32_p95", "p95InvoiceRow", "p95Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p32_p38", "p38ActivityTag", "p38Name", rels, bolComboColumns));
                    break;
                case "p92":
                    ret.Add(InhaleColumn4Relation("p92_j27", "j27Currency", "j27Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p92_p93", "p93InvoiceHeader", "p93Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p92_x15", "x15VatRateType", "x15Name", rels, bolComboColumns));
                    break;

            }

            return ret;


        }
        public IEnumerable<BO.TheGridColumn> AllColumns()
        {

            return _lis;


        }
        private BO.TheGridColumn InhaleColumn4Relation(string strRelName, string strFieldEntity, string strFieldName, List<BO.EntityRelation> applicable_rels, bool bolComboColumns)
        {
            BO.TheGridColumn c0 = ByUniqueName("a__" + strFieldEntity + "__" + strFieldName);
            BO.TheGridColumn c = Clone2NewInstance(c0);


            BO.EntityRelation rel = applicable_rels.Where(p => p.RelName == strRelName).First();
            c.RelName = strRelName;
            c.RelSql = rel.SqlFrom;
            if (rel.RelNameDependOn != null)
            {
                c.RelSqlDependOn = applicable_rels.Where(p => p.RelName == rel.RelNameDependOn).First().SqlFrom;    //relace závisí na jiné relaci
            }

            if (c.NotShowRelInHeader == true)
            {
                return c;   //nezobrazovat u sloupce název relace
            }

            if (bolComboColumns == true)
            {
                c.Header = rel.AliasSingular;
            }
            else
            {
                c.Header = c.Header + " [" + rel.AliasSingular + "]";

            }


            return c;
        }
        public BO.TheGridColumn ByUniqueName(string strUniqueName)
        {
            if (_lis.Where(p => p.UniqueName == strUniqueName).Count() > 0)
            {
                return _lis.Where(p => p.UniqueName == strUniqueName).First();
            }
            else
            {
                return null;
            }
        }
        private BO.TheGridColumn Clone2NewInstance(BO.TheGridColumn c)
        {
            return new BO.TheGridColumn() { Entity = c.Entity, EntityAlias = c.EntityAlias, Field = c.Field, FieldType = c.FieldType, FixedWidth = c.FixedWidth, Header = c.Header, SqlSyntax = c.SqlSyntax, IsFilterable = c.IsFilterable, IsShowTotals = c.IsShowTotals, IsTimestamp = c.IsTimestamp, RelName = c.RelName, RelSql = c.RelSql, RelSqlDependOn = c.RelSqlDependOn,NotShowRelInHeader=c.NotShowRelInHeader, TranslateLang1= c.TranslateLang1, TranslateLang2=c.TranslateLang2, TranslateLang3= c.TranslateLang3 };

        }



        public List<BO.TheGridColumn> ParseTheGridColumns(string strPrimaryPrefix, string strJ72Columns,int intLangIndex)
        {
            //v strJ72Columns je čárkou oddělený seznam sloupců z pole j72Columns: název relace+__+entita+__+field


            List<BO.EntityRelation> applicable_rels = _ep.getApplicableRelations(strPrimaryPrefix);
            List<string> sels = BO.BAS.ConvertString2List(strJ72Columns, ",");
            List<BO.TheGridColumn> ret = new List<BO.TheGridColumn>();

            string[] arr;
            BO.EntityRelation rel;

            for (var i = 0; i < sels.Count; i++)
            {
                arr = sels[i].Split("__");

                if (_lis.Exists(p => p.Entity == arr[1] && p.Field == arr[2]))
                {
                    //var c0 = _lis.Where(p => p.Entity == arr[1] && p.Field == arr[2]).First();
                    BO.TheGridColumn c = Clone2NewInstance(_lis.Where(p => p.Entity == arr[1] && p.Field == arr[2]).First());
                    switch (intLangIndex)
                    {
                        case 1:
                            c.Header = c.TranslateLang1;
                            break;
                        case 2:
                            c.Header = c.TranslateLang2;
                            break;
                        case 3:
                            c.Header = c.TranslateLang3;
                            break;
                        default:
                            c.Header = c.Header;
                            break;
                    }
                    if (arr[0] == "a")
                    {
                        c.RelName = null;
                    }
                    else
                    {
                        c.RelName = arr[0]; //název relace v sql dotazu
                        rel = applicable_rels.Where(p => p.RelName == c.RelName).First();
                        c.RelSql = rel.SqlFrom;    //sql klauzule relace    
                        if (c.NotShowRelInHeader == false)
                        {
                            c.Header = c.Header + " [" + rel.AliasSingular + "]";   //zobrazovat název entity v záhlaví sloupce                           
                        }
                        

                        if (rel.RelNameDependOn != null)
                        {
                            c.RelSqlDependOn = applicable_rels.Where(p => p.RelName == rel.RelNameDependOn).First().SqlFrom;    //relace závisí na jiné relaci
                        }
                    }



                    if ((i == sels.Count - 1) && (c.FieldType == "num" || c.FieldType == "num0" || c.FieldType == "num3"))
                    {
                        c.CssClass = "tdn_lastcol";
                    }
                    ret.Add(c);
                }


            }

            return ret;


        }

        public List<BO.TheGridColumnFilter> ParseAdhocFilterFromString(string strJ72Filter, IEnumerable<BO.TheGridColumn> explicit_cols)
        {
            var ret = new List<BO.TheGridColumnFilter>();
            if (String.IsNullOrEmpty(strJ72Filter) == true) return ret;


            List<string> lis = BO.BAS.ConvertString2List(strJ72Filter, "$$$");
            foreach (var s in lis)
            {
                List<string> arr = BO.BAS.ConvertString2List(s, "###");
                if (explicit_cols.Where(p => p.UniqueName == arr[0]).Count() > 0)
                {
                    var c = new BO.TheGridColumnFilter() { field = arr[0], oper = arr[1], value = arr[2] };
                    c.BoundColumn = explicit_cols.Where(p => p.UniqueName == arr[0]).First();
                    ParseFilterValue(ref c);
                    ret.Add(c);
                }


            }
            return ret;
        }

        private void ParseFilterValue(ref BO.TheGridColumnFilter col)
        {

            {
                if (col.value.Contains("|"))
                {
                    var a = col.value.Split("|");
                    col.c1value = a[0];
                    col.c2value = a[1];
                }
                else
                {
                    col.c1value = col.value;
                    col.c2value = "";
                }
                switch (col.oper)
                {
                    case "1":
                        {
                            col.value_alias = "Je prázdné";
                            break;
                        }

                    case "2":
                        {
                            col.value_alias = "Není prázdné";
                            break;
                        }

                    case "3":  // obsahuje
                        {
                            col.value_alias = col.c1value;
                            break;
                        }

                    case "5":  // začíná na
                        {

                            col.value_alias = "[*=] " + col.c1value;
                            break;
                        }

                    case "6":  // je rovno
                        {
                            col.value_alias = "[=] " + col.c1value;
                            break;
                        }

                    case "7":  // není rovno
                        {
                            col.value_alias = "[<>] " + col.c1value;
                            break;
                        }

                    case "8":
                        {
                            col.value_alias = "ANO";
                            break;
                        }

                    case "9":
                        {
                            col.value_alias = "NE";
                            break;
                        }

                    case "10": // je větší než nula
                        {
                            col.value_alias = "větší než 0";
                            break;
                        }

                    case "11":
                        {
                            col.value_alias = "0 nebo prázdné";
                            break;
                        }

                    case "4":  // interval
                        {

                            if (col.BoundColumn.FieldType == "date" | col.BoundColumn.FieldType == "datetime")
                            {
                                col.value_alias = col.c1value + " - " + col.c2value;   // datum
                            }
                            else
                            {
                                col.value_alias = col.c1value + " - " + col.c2value;
                            }    // číslo

                            break;
                        }
                }





            }


        }


    }
}
