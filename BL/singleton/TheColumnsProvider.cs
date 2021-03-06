﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace BL
{
    
    public class TheColumnsProvider
    {
        //private readonly BL.RunningApp _app;
        private readonly BL.TheEntitiesProvider _ep;
        private readonly BL.TheTranslator _tt;
        private List<BO.TheGridColumn> _lis;        
        private string _lastEntity;
        private string _curEntityAlias;
        
        public TheColumnsProvider(BL.TheEntitiesProvider ep,BL.TheTranslator tt)
        {
            //_app = runningapp;
            _ep = ep;
            _tt = tt;
            _lis = new List<BO.TheGridColumn>();
            SetupPallete();
            Handle_Translate();

        }

        public void Refresh()
        {
            _lis = new List<BO.TheGridColumn>();
            SetupPallete();
            Handle_Translate();
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

        private BO.TheGridColumn AF(string strEntity, string strField, string strHeader, BO.TheGridDefColFlag dcf=BO.TheGridDefColFlag._none, string strSqlSyntax = null, string strFieldType = "string", bool bolIsShowTotals = false,bool bolNotShowRelInHeader=false,string strGroup=null)
        {
            if (strEntity != _lastEntity)
            {
                _curEntityAlias = _ep.ByTable(strEntity).AliasSingular;
            }
           
            _lis.Add(new BO.TheGridColumn() { Field = strField, Entity = strEntity, EntityAlias = _curEntityAlias, Header = strHeader, DefaultColumnFlag = dcf, SqlSyntax = strSqlSyntax, FieldType = strFieldType, IsShowTotals = bolIsShowTotals,NotShowRelInHeader= bolNotShowRelInHeader,FixedWidth= SetDefaultColWidth(strFieldType),TranslateLang1=strHeader,TranslateLang2=strHeader,TranslateLang3=strHeader,DesignerGroup=strGroup });
            _lastEntity = strEntity;
            return _lis[_lis.Count - 1];
        }

        


        private void AF_TIMESTAMP(string strEntity, string strField, string strHeader, string strSqlSyntax, string strFieldType)
        {
            if (strEntity != _lastEntity)
            {
                _curEntityAlias = _ep.ByTable(strEntity).AliasSingular;
            }
            _lis.Add(new BO.TheGridColumn() { IsTimestamp = true, Field = strField, Entity = strEntity, EntityAlias = _curEntityAlias, Header = strHeader, SqlSyntax = strSqlSyntax, FieldType = strFieldType, FixedWidth = SetDefaultColWidth(strFieldType),DesignerGroup="Časové razítko záznamu" });
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

        private void Handle_Translate()
        {
            //Překlad do ostatních jazyků
            foreach (var col in _lis.Where(p=>p.Header.Length>2))
            {
                bool b = true;
                if (col.Header.Length > 3 && col.Header.Substring(0, 3) == "Col")
                {
                    b = false;
                }
                if (b)
                {
                    col.TranslateLang1 = _tt.DoTranslate(col.Header, 1,"TheColumnsProvider");
                    col.TranslateLang2 = _tt.DoTranslate(col.Header, 2, "TheColumnsProvider");
                }

            }

            
            
            
        }
        private void SetupPallete()
        {
            BO.TheGridColumn onecol;
            BO.TheGridDefColFlag gdc1 = BO.TheGridDefColFlag.GridAndCombo;
            BO.TheGridDefColFlag gdc0 = BO.TheGridDefColFlag._none;
            BO.TheGridDefColFlag gdc2 = BO.TheGridDefColFlag.GridOnly;

            SetupJ02();
            SetupP28();
            SetupP91();
            
            //j03 = uživatelé
            AF("j03User", "j03Login", "Login", gdc1, null,"string",false,true);
            AF("j03User", "j04Name", "Role", gdc1, "j03_j04.j04Name","string",false,true);
            AF("j03User", "Lang", "Jazyk", gdc1, "case isnull(a.j03LangIndex,0) when 0 then 'Česky' when 1 then 'English' when 4 then 'Slovenčina' end");
            AF("j03User", "j03Ping_TimeStamp", "Last ping", gdc0, "a.j03PingTimestamp", "datetime");
            AF("j03User", "j03IsDebugLog", "Debug log", gdc0, null, "bool");
            AppendTimestamp("j03User");
            
            AF("j04UserRole", "j04Name", "Aplikační role", gdc1, null,"string",false,true);
            AppendTimestamp("j04UserRole");

            AF("j05MasterSlave", "MasterPerson", "Nadřízený", gdc1, "j02master.j02LastName+' '+j02master.j02FirstName+isnull(' '+j02master.j02TitleBeforeName,'')", "string", false, true);
            AF("j05MasterSlave", "SlavePerson", "Podřízený (jednotlivec)", gdc1, "j02slave.j02LastName+' '+j02slave.j02FirstName+isnull(' '+j02slave.j02TitleBeforeName,'')", "string");
            AF("j05MasterSlave", "SlaveTeam", "Podřízený tým", gdc1, "j11slave.j11Name", "string");
            AppendTimestamp("j05MasterSlave");

            AF("j07PersonPosition", "j07Name", "Pozice", gdc1, null, "string", false, true);
            AF("j07PersonPosition", "j07Ordinary", "#", gdc2, null, "num0");
            AF("j07PersonPosition", "j07Name_BillingLang1", "Fakturační jazyk #1");
            AF("j07PersonPosition", "j07Name_BillingLang2", "Fakturační jazyk #2");
            AF("j07PersonPosition", "j07Name_BillingLang3", "Fakturační jazyk #3");
            AF("j07PersonPosition", "j07Name_BillingLang4", "Fakturační jazyk #4");
            AF("j07PersonPosition", "j07FreeText01", "Volné pole #1");
            AF("j07PersonPosition", "j07FreeText02", "Volné pole #2");
            AppendTimestamp("j07PersonPosition");

            AF("j18Region", "j18Name", "Středisko", gdc1, null, "string", false, true);
            AF("j18Region", "j18Ordinary", "#", gdc2, null, "num0");
            AF("j18Region", "j18Code", "Kód");
            AppendTimestamp("j18Region");

            AF("j17Country", "j17Name", "Region", gdc1, null, "string", false, true);
            AF("j17Country", "j17Ordinary", "#", gdc0, null, "num0");
            AF("j17Country", "j17Code", "Kód");
            AppendTimestamp("j17Country");

            AF("c21FondCalendar", "c21Name", "Název fondu", gdc1, null, "string", false, true);
            AF("c21FondCalendar", "c21Ordinary", "#", gdc2, null, "num0");
            AppendTimestamp("c21FondCalendar");

            AF("c26Holiday", "c26Date", "Datum", gdc1, null, "date");
            AF("c26Holiday", "c26Name", "Název svátku", gdc1, null, "string", false, true);            
            AppendTimestamp("c26Holiday");

            AF("j11Team", "j11Name", "Tým osob", gdc1, null, "string", false, true);
            AppendTimestamp("j11Team");

            AF("j25ReportCategory", "j25Name", "Kategorie sestav", gdc1, null, "string", false, true);
            AF("j25ReportCategory", "j25Ordinary", "#", gdc2, null, "num0");
            AppendTimestamp("j25ReportCategory");

            AF("j27Currency", "j27Code", "Měna", gdc1, null, "string", false, true);
            AF("j27Currency", "j27Name", "Název měny");

            AF("j61TextTemplate", "j61Name", "Šablona zprávy", gdc1, null, "string", false, true);
            AF("j61TextTemplate", "j61MailSubject", "Předmět zprávy", gdc2);
            AF("j61TextTemplate", "j61MailTO", "TO");
            AF("j61TextTemplate", "j61MailCC", "CC");
            AF("j61TextTemplate", "j61MailBCC", "BCC");
            AppendTimestamp("j61TextTemplate");

            //b01 = workflow šablona            
            AF("b01WorkflowTemplate", "b01Name", "Workflow šablona", gdc1, null, "string", false, true);
            onecol=AF("b01WorkflowTemplate", "b01Code", "Kód", gdc2);
            onecol.FixedWidth = 70;
            AppendTimestamp("b01WorkflowTemplate");

            //b02 = workflow stav            
            //AF("b02WorkflowStatus", "b02Name", "Stav", 1, null, "string", false, true);
            AF("b02WorkflowStatus", "b02Name", "Stav", gdc1);
            onecol =AF("b02WorkflowStatus", "b02Ident", "Kód stavu", gdc1);
            onecol.FixedWidth = 70;
            AF("b02WorkflowStatus", "b02IsDefaultStatus", "Výchozí stav", gdc2, null, "bool");
            //AF("b02WorkflowStatus", "b02IsHoldStatus", "Záchytný stav", 2, null, "bool");
            AF("b02WorkflowStatus", "b02Order", "#", gdc2, null, "num0");           
            AppendTimestamp("b02WorkflowStatus");




            ////b05 = workflow historie          
            //AF("b05Workflow_History", "Kdy", "Čas", gdc1, "a.b05DateInsert", "datetime");
            //AF("b05Workflow_History", "b05Comment", "Text", gdc1, null, "string", false, true);
            
            //AF("b05Workflow_History", "b05IsCommentOnly", "Pouze komentář", gdc2, null, "bool");
            //AF("b05Workflow_History", "b05IsManualStep", "Ruční krok", 0, null, "bool");
            //AF("b05Workflow_History", "b05IsNominee", "Nominace řešitele", gdc2, null, "bool");
            //AF("b05Workflow_History", "b05IsCommentRestriction", "Interní komentář", gdc2, null, "bool");            
            //AppendTimestamp("b05Workflow_History",false);

            ////b06 = workflow krok
            //AF("b06WorkflowStep", "b06Name", "Workflow krok", gdc1, null, "string", false, true);
            //AF("b06WorkflowStep", "b06Order", "#", gdc2, null, "num0");
            //AppendTimestamp("b06WorkflowStep");

            ////b65 = notifikační šablona
            //AF("b65WorkflowMessage", "b65Name", "Notifikační šablona",gdc1, null, "string", false, true);
            //AF("b65WorkflowMessage", "b65MessageSubject", "Předmět zprávy", gdc2);
            //AF("b65WorkflowMessage", "SystemFlag", "🚩", gdc1, "case when isnull(a.b65SystemFlag,0)>0 then '<div style='+char(34)+'background-color:red;'+char(34)+'>&nbsp;</div>' end");
            //AppendTimestamp("b65WorkflowMessage");
            
            
            //j90 = access log uživatelů
            AF("j90LoginAccessLog", "j90Date", "Čas", gdc1, null, "datetime");
            AF("j90LoginAccessLog", "j90BrowserFamily", "Prohlížeč", gdc1);
            AF("j90LoginAccessLog", "j90Platform", "OS", gdc1);
            AF("j90LoginAccessLog", "j90BrowserDeviceType", "Device");
            AF("j90LoginAccessLog", "j90ScreenPixelsWidth", "Šířka (px)", gdc1);
            AF("j90LoginAccessLog", "j90ScreenPixelsHeight", "Výška (px)", gdc1);
            AF("j90LoginAccessLog", "j90UserHostAddress", "Host", gdc1);
            AF("j90LoginAccessLog", "j90LoginMessage", "Chyba", gdc1);
            AF("j90LoginAccessLog", "j90CookieExpiresInHours", "Expirace přihlášení", gdc1, null, "num0");
            AF("j90LoginAccessLog", "j90LoginName", "Login", gdc1);

            //j92 = ping log uživatelů
            AF("j92PingLog", "j92Date", "Čas", gdc1, null, "datetime");
            AF("j92PingLog", "j92BrowserFamily", "Prohlížeč", gdc1);
            AF("j92PingLog", "j92BrowserOS", "OS", gdc1);
            AF("j92PingLog", "j92BrowserDeviceType", "Device", gdc1);
            AF("j92PingLog", "j92BrowserAvailWidth", "Šířka (px)", gdc1);
            AF("j92PingLog", "j92BrowserAvailHeight", "Výška (px)", gdc1);
            AF("j92PingLog", "j92RequestURL", "Url", gdc1);


            AF("o15AutoComplete", "o15Value", "Hodnota", gdc1);
            AF("o15AutoComplete", "o15Flag", "Typ dat", gdc1, "case a.o15Flag when 1 then 'Titul před' when 2 then 'Titul za' when 3 then 'Pracovní funkce' when 328 then 'Stát' when 427 then 'URL adresa' end");
            AF("o15AutoComplete", "o15Ordinary", "#", gdc2, null, "num0");

            AF("p07ProjectLevel", "p07NameSingular", "Úroveň", gdc1, null, "string", false, true);
            AF("p07ProjectLevel", "p07Level", "Index úrovně", gdc1, null, "num0",false,true);
            AF("p07ProjectLevel", "p07NamePlural", "Množné číslo", gdc2);
            AF("p07ProjectLevel", "p07NameInflection", "Koho čeho");
            AppendTimestamp("p07ProjectLevel");

            AF("p29ContactType", "p29Name", "Typ klienta", gdc1, null, "string", false, true);            
            AF("p29ContactType", "p29Ordinary", "#", gdc0, null, "num0");
            AppendTimestamp("p29ContactType");

            AF("p42ProjectType", "p42Name", "Typ", gdc1, null, "string", false, true);
            AF("p42ProjectType", "p42Code", "Kód");
            AF("p42ProjectType", "p42Ordinary", "#", gdc0, null, "num0");
            AppendTimestamp("p42ProjectType");

            AF("p51PriceList", "p51TypeFlag", "Typ ceníku", gdc2, "case a.p51TypeFlag when 1 then 'Fakturační sazby' when 2 then 'Nákladové sazby' when 3 then 'Režijní sazby' when 5 then 'Kořenový (ROOT) ceník' when 4 then 'Efektivní sazby' end", "string", false, true);
            AF("p51PriceList", "p51Name", "Pojmenovaný ceník", gdc1, null, "string", false, true);
            AF("p51PriceList", "p51DefaultRateT", "Výchozí hod.sazba", gdc0, null,"num");
            AF("p51PriceList", "p51Ordinary", "#", gdc0, null, "num0");
            AF("p51PriceList", "p51IsCustomTailor", "Sazby na míru",gdc2, null, "bool");
            AppendTimestamp("p51PriceList");

            AF("p34ActivityGroup", "p34Name", "Sešit", gdc1, null, "string", false, true);
            AF("p34ActivityGroup", "p34Code", "Kód", gdc2);
            AF("p34ActivityGroup", "p33ID", "Vstupí data", gdc2, "case a.p33ID when 1 then 'Čas' when 2 then 'Peníze bez DPH' when 3 then 'Kusovník' when 5 then 'Peníze+DPH' end");
            AppendTimestamp("p34ActivityGroup");

            AF("p32Activity", "p32Name", "Aktivita", gdc1, null, "string", false, true);
            AF("p32Activity", "p32Code", "Kód", gdc2);
            AF("p32Activity", "p32IsBillable", "Fakturovatelný úkon", gdc2, null, "bool");
            AF("p32Activity", "p32Ordinary", "#", gdc2, null, "num0");
            AF("p32Activity", "p32IsTextRequired", "Povinný text úkonu", gdc2, null, "bool");            
            AF("p32Activity", "p32Value_Default", "Výchozí hodnota úkonu", gdc0, null, "num");
            AF("p32Activity", "p32Value_Minimum", "MIN", gdc0, null, "num");
            AF("p32Activity", "p32Value_Maximum", "MAX", gdc0, null, "num");            
            AF("p32Activity", "p32DefaultWorksheetText", "Výchozí text úkonu");
            AF("p32Activity", "p32Name_BillingLang1", "Aktivita €1");
            AF("p32Activity", "p32Name_BillingLang2", "Aktivita €2");
            AF("p32Activity", "p32Name_BillingLang3", "Aktivita €3");
            AF("p32Activity", "p32Name_BillingLang4", "Aktivita €4");
            AppendTimestamp("p32Activity");

            AF("p38ActivityTag", "p38Name", "Odvětví aktivity", gdc1, null, "string", false, true);
            AF("p38ActivityTag", "p38Ordinary", "#", gdc2, null, "num0");
            AppendTimestamp("p38ActivityTag");

            AF("p36LockPeriod", "p36DateFrom", "Od", gdc1, null, "date");
            AF("p36LockPeriod", "p36DateUntil", "Do", gdc1, null, "date");
            AF("p36LockPeriod", "p36IsAllSheets", "Všechny sešity", gdc2, null,"bool");
            AF("p36LockPeriod", "p36IsAllPersons", "Všechny osoby", gdc2, null, "bool");            
            AppendTimestamp("p36LockPeriod");

            AF("p53VatRate", "p53Value", "Sazba DPH", gdc1, null, "num", false,true);
            AppendTimestamp("p53VatRate");

            AF("p61ActivityCluster", "p61Name", "Klast aktivit", gdc1, null, "string", false, true);            
            AppendTimestamp("p61ActivityCluster");

            AF("p63Overhead", "p63Name", "Režijní přirážka", gdc1, null, "string", false, true);
            AppendTimestamp("p63Overhead");

            AF("p80InvoiceAmountStructure", "p80Name", "Název rozpisu", gdc1, null, "string",false,true);
            AF("p80InvoiceAmountStructure", "p80IsTimeSeparate", "Čas 1:1", gdc2, null, "bool");
            AF("p80InvoiceAmountStructure", "p80IsExpenseSeparate", "Výdaje 1:1", gdc2, null, "bool");
            AF("p80InvoiceAmountStructure", "p80IsFeeSeparate", "Pevné odměny 1:1", gdc2, null, "bool");
            AppendTimestamp("p80InvoiceAmountStructure");

            AF("p92InvoiceType", "p92Name", "Typ faktury", BO.TheGridDefColFlag.GridAndCombo, null, "string", false, true);
            AF("p92InvoiceType", "p92Ordinary", "#", gdc2, null, "num0");
            AppendTimestamp("p92InvoiceType");

            AF("p98Invoice_Round_Setting_Template", "p98Name", "Zaokrouhlovací pravidlo", gdc1, null, "string", false, true);
            AF("p98Invoice_Round_Setting_Template", "p98IsDefault", "Výchozí pravidlo", gdc2, null, "bool");
            AppendTimestamp("p98Invoice_Round_Setting_Template");
            

            AF("p89ProformaType", "p89Name", "Typ zálohy", gdc1, null, "string", false, true);
            AppendTimestamp("p89ProformaType");            

            AF("p93InvoiceHeader", "p93Name", "Vystavovatel faktury", gdc1, null, "string", false, true);
            AF("p93InvoiceHeader", "p93Company", "Firma",gdc2);
            AF("p93InvoiceHeader", "p93RegID", "IČ",gdc2);
            AF("p93InvoiceHeader", "p93VatID", "DIČ",gdc2);
            AF("p93InvoiceHeader", "p93City", "Město");
            AF("p93InvoiceHeader", "p93Street", "Ulice");
            AF("p93InvoiceHeader", "p93Zip", "PSČ");            
            AppendTimestamp("p93InvoiceHeader");

            AF("p86BankAccount", "p86Name", "Bankovní účet", gdc2, null, "string", false, true);
            AF("p86BankAccount", "p86BankAccount", "Číslo účtu", gdc1);
            AF("p86BankAccount", "p86BankCode", "Kód banky", gdc1);
            AF("p86BankAccount", "p93Names", "Vazba na vystavovatele faktur", gdc2, "dbo.p86_get_p93names(a.p86ID)");
            AF("p86BankAccount", "p86BankName", "Banka");
            AF("p86BankAccount", "p86SWIFT", "SWIFT");
            AF("p86BankAccount", "p86IBAN", "IBAN");
            AF("p86BankAccount", "p86BankAddress", "Adresa banky");
            AppendTimestamp("p86BankAccount");

            AF("p95InvoiceRow", "p95Name", "Fakturační oddíl", gdc1, null, "string", false, true);
            AF("p95InvoiceRow", "p95Code", "Kód");
            AF("p95InvoiceRow", "p95Ordinary", "#", gdc2, null, "num0");
            AF("p95InvoiceRow", "p95Name_BillingLang1", "Název €1");
            AF("p95InvoiceRow", "p95Name_BillingLang2", "Název €2");
            AF("p95InvoiceRow", "p95Name_BillingLang3", "Název €3");
            AF("p95InvoiceRow", "p95Name_BillingLang4", "Název €4");
            AppendTimestamp("p95InvoiceRow");

            AF("m62ExchangeRate", "m62Date", "Datum kurzu", gdc1, null, "date", false, true);
            AF("m62ExchangeRate", "m62Rate", "Kurz",gdc1,null,"num3");
            AF("m62ExchangeRate", "Veta", "", gdc2, "CONVERT(varchar(10),a.m62Units)+' '+(select j27Code from j27Currency where j27ID=a.j27ID_Slave)+' = '+CONVERT(varchar(10),a.m62Rate)+' '+(select j27Code FROM j27Currency where j27ID=a.j27ID_Master)");
            AF("m62ExchangeRate", "m62RateType", "Typ kurzu",gdc2, "case when a.m62RateType=1 then 'Fakturační kurz' else 'Fixní kurz' end");
            AppendTimestamp("m62ExchangeRate");

            AF("p35Unit", "p35Name", "Kusovníková jednotka", gdc1, null, "string", false, true);
            AF("p35Unit", "p35Code", "Kód");
            AppendTimestamp("p35Unit");

            //o40 = smtp poštovní účty                        
            AF("o40SmtpAccount", "o40Name", "Jméno odesílatele", gdc1, null, "string", false, true);
            AF("o40SmtpAccount", "o40Server", "Smtp server", gdc2);
            AF("o40SmtpAccount", "o40EmailAddress", "Adresa odesílatele", gdc1);
            AF("o40SmtpAccount", "o40Port", "Smtp Port", gdc2, null, "num0");

            //x40 = OUTBOX            
            AF("x40MailQueue", "MessageTime", "Čas", gdc1, "case when a.x40DatetimeProcessed is not null then a.x40DatetimeProcessed else a.x40DateInsert end", "datetime",false,true);            
            AF("x40MailQueue", "x40SenderName", "Odesílatel", 0);
            AF("x40MailQueue", "x40SenderAddress", "Odesílatel (adresa)");
            AF("x40MailQueue", "x40Recipient", "Komu", BO.TheGridDefColFlag.GridAndCombo);
            AF("x40MailQueue", "x40CC", "Cc");
            AF("x40MailQueue", "x40BCC", "Bcc");
            AF("x40MailQueue", "x40Status", "Stav", BO.TheGridDefColFlag.GridAndCombo, "case a.x40Status when 1 then 'Čeká na odeslání' when 2 then 'Chyba' when 3 then 'Odesláno' when 4 then 'Zastaveno' when 5 then 'Čeká na schválení' end");
            AF("x40MailQueue", "x40Subject", "Předmět zprávy", BO.TheGridDefColFlag.GridAndCombo);
            AF("x40MailQueue", "x40Body", "Text zprávy", BO.TheGridDefColFlag.GridAndCombo, "convert(varchar(150),a.x40Body)+'...'");
            AF("x40MailQueue", "x40Attachments", "Přílohy");
            AF("x40MailQueue", "x40EmlFileSize_KB", "Velikost (kB)", 0, "a.x40EmlFileSize/1024", "num0", true);
            AF("x40MailQueue", "x40EmlFileSize_MB", "Velikost (MB)", 0, "convert(float,a.x40EmlFileSize)/1048576", "num", true);
            AF("x40MailQueue", "x40ErrorMessage", "Chyba", BO.TheGridDefColFlag.GridAndCombo);



            //x31 = tisková sestava            
            AF("x31Report", "x31Name", "Tisková sestava",gdc1, null, "string", false, true);
            AF("x31Report", "RepFormat", "Formát",gdc1, "case a.x31FormatFlag when 1 then 'REPORT' when 2 then 'DOCX' when 3 then 'PLUGIN' when 4 then 'XLS' end");
            AF("x31Report", "x31Code", "Kód sestavy");
            AF("x31Report", "x31IsPeriodRequired", "Filtr čaového období", 0, null, "bool");
            
            AF("x31Report", "x31FileName", "Soubor šablony", gdc1);
            AF("x31Report", "x31Ordinary", "#",gdc1, null, "num0");
            
            AF("x31Report", "x31ExportFileNameMask", "Maska export souboru");            
            AF("x31Report", "x31IsScheduling", "Pravidelné odesílání");
            AF("x31Report", "x31Description", "Poznámka");
            AppendTimestamp("x31Report");

            //uživatelská pole
            AF("x28EntityField", "x28Name", "Uživatelské pole",gdc1, null, "string", false, true);
            AF("x28EntityField", "x28Field", "Fyzický název", gdc2);            
            AF("x28EntityField", "x28IsRequired", "Povinné", gdc1, null,"bool");            
            AF("x28EntityField", "x28Ordinary", "#", gdc1, null, "num0");
            AppendTimestamp("x28EntityField");

            //skupina uživatelských polí
            AF("x27EntityFieldGroup", "x27Name", "Skupina polí", gdc1, null, "string", false, true);
            AF("x27EntityFieldGroup", "x27Ordinary", "#", gdc2, null, "num0");

            //číselné řady
            AF("x38CodeLogic", "x38Name", "Číselná řada", gdc1, null, "string", false, true);            
            AF("x38CodeLogic", "x38ConstantBeforeValue", "Konstanta před", gdc2);
            AF("x38CodeLogic", "x38ConstantAfterValue", "Konstanta za", gdc2);
            AF("x38CodeLogic", "x38Scale", "Rozsah nul", gdc2, null, "num0");
            AF("x38CodeLogic", "Maska", "Min-Max", 0, "case when a.x38MaskSyntax IS NULL then ISNULL(a.x38ConstantBeforeValue,'')+RIGHT('000000001',a.x38Scale)+' - '+ISNULL(a.x38ConstantBeforeValue,'')+RIGHT('99999999999',a.x38Scale) else a.x38MaskSyntax end");
            AppendTimestamp("x38CodeLogic");

            //x51 = nápověda
            AF("x51HelpCore", "x51Name", "Nápověda", gdc1, null, "string", false, true);
            AF("x51HelpCore", "x51ViewUrl", "View Url", gdc2);
            AF("x51HelpCore", "x51NearUrls", "Související Urls", gdc2);
            AF("x51HelpCore", "x51ExternalUrl", "Externí Url");
            AppendTimestamp("x51HelpCore");

            //x55 = dashboard widget
            AF("x55Widget", "x55Name", "Widget", gdc1, null, "string", false, true);
            AF("x55Widget", "x55Code", "Kód widgetu", gdc2, null, "string", false, true);
            AF("x55Widget", "x55Description", "Poznámka", gdc2);
            AF("x55Widget", "x55Skin", "Cílový dashboard");
            AF("x55Widget", "x55DataTablesLimit", "Minimum záznamů pro [DataTables]", gdc2);
            AF("x55Widget", "x55Ordinal", "#", gdc2, null, "num0");
            AppendTimestamp("x55Widget");

            AF("x67EntityRole", "x67Name", "Název role", gdc1, null, "string", false, true);
            AF("x67EntityRole", "x67Ordinary", "#", gdc2, null, "num0");
            AppendTimestamp("x67EntityRole");

            //x29 = entita
            AF("x29Entity", "x29Name", "Entita", gdc1, null, "string", false, true);
            //AF("x29Entity", "x29NameSingle", "Jednotné číslo", gdc2);
            
            //x97 = překlad
            AF("x97Translate", "x97Code", "Originál", gdc1, null, "string", false, true);
            AF("x97Translate", "x97Lang1", "English", gdc1);
            AF("x97Translate", "x97Lang2", "Deutsch", gdc1);
            AF("x97Translate", "x97Lang4", "Slovenčina", gdc1);
            AF("x97Translate", "x97OrigSource", "Zdroj");
            AppendTimestamp("x97Translate",false);

            AF("x15VatRateType", "x15Name", "Druh DPH", gdc1, null, "string", false, true);

            
        }


        private void SetupJ02(string stb="j02Person")
        {
            AF(stb, "fullname_desc", "Příjmení+Jméno", BO.TheGridDefColFlag.GridAndCombo, "a.j02LastName+' '+a.j02FirstName+isnull(' '+a.j02TitleBeforeName,'')", "string", false, true);
            AF(stb, "fullname_asc", "Jméno+Příjmení", BO.TheGridDefColFlag._none, "isnull(a.j02TitleBeforeName+' ','')+a.j02FirstName+' '+a.j02LastName+isnull(' '+a.j02TitleAfterName,'')", "string", false, true);

            AF(stb, "j02Email", "E-mail", BO.TheGridDefColFlag.GridAndCombo);
            AF(stb, "j02FirstName", "Jméno");
            AF(stb, "j02LastName", "Příjmení");
            AF(stb, "j02TitleBeforeName", "Titul před");
            AF(stb, "j02TitleAfterName", "Titul za");
            AF(stb, "j02Phone", "TEL");
            AF(stb, "j02Mobile", "Mobil");
            AF(stb, "j02Code", "Kód");
            AF(stb, "j02JobTitle", "Pozice na vizitce");
            AF(stb, "j02Office", "Adresa");
            AF(stb, "j02IsIntraPerson", "Interní osoba", 0, null, "bool");
            AF(stb, "j02InvoiceSignatureFile", "Grafický podpis");
            AF(stb, "j02Salutation", "Oslovení");
            AF(stb, "j02EmailSignature", "E-mail podpis");
            AppendTimestamp(stb);
        }
        private void SetupP28(string stb="p28Contact")
        {
            AF(stb, "p28Name", "Klient", BO.TheGridDefColFlag.GridAndCombo, null, "string", false, true);
            AF(stb, "p28Code", "Kód");
            AF(stb, "p28CompanyShortName", "Zkrácený název");
            
            AF(stb, "p28RegID", "IČ", BO.TheGridDefColFlag.GridOnly);
            AF(stb, "p28VatID", "DIČ", BO.TheGridDefColFlag.GridAndCombo);
            AF(stb, "p28BillingMemo", "Fakturační poznámka");

            AF(stb, "p28Round2Minutes", "Zaokrouhlování času", 0, null, "num0");
            
            AF(stb, "p28ICDPH_SK", "IČ DPH (SK)");
            AF(stb, "p28SupplierID", "Kód dodavatele");
            AppendTimestamp(stb);

            AF("view_PrimaryAddress","FullAddress", "Fakturační adresa", BO.TheGridDefColFlag.GridOnly, "isnull(a.o38City+', ','')+isnull('<code>'+a.o38Street+'</code>','')+isnull(', '+a.o38ZIP,'')+isnull(' <var>'+a.o38Country+'</var>','')","string",false,true);
            AF("view_PrimaryAddress", "o38Street", "Ulice", BO.TheGridDefColFlag.GridOnly);
            AF("view_PrimaryAddress", "o38City", "Město", BO.TheGridDefColFlag.GridAndCombo);
            AF("view_PrimaryAddress", "o38ZIP", "PSČ");
            AF("view_PrimaryAddress", "o38Country", "Stát");
            AF("view_PrimaryAddress", "o38Name", "Název");
            
        }
        private void SetupP91(string stb= "p91Invoice")
        {
            BO.TheGridColumn oc;

            AF(stb, "p91Code", "Číslo", BO.TheGridDefColFlag.GridAndCombo, null, "string", false, true);
            AF(stb, "p91Client", "Klient vyúčtování", BO.TheGridDefColFlag.GridAndCombo);

            AF(stb, "p91IsDraft", "Draft",0,null,"bool",false,false);

            var strG = "Datum";
            AF(stb, "p91Date", "Vystaveno", BO.TheGridDefColFlag.GridOnly, null,"date",false,false,strG);
            AF(stb, "p91DateSupply", "Datum plnění", BO.TheGridDefColFlag.GridAndCombo, null, "date", false, false, strG);
            AF(stb, "p91DateMaturity", "Splatnost", BO.TheGridDefColFlag.GridOnly, null, "date", false, false, strG);
            AF(stb, "DnuPoSplatnosti", "Dnů do splatnosti", 0, "case When a.p91Amount_Debt=0 Then null Else datediff(day, p91DateMaturity, dbo.get_today()) End", "num0", false, false, strG);
            AF(stb, "p91DateBilled", "Datum úhrady", 0, null, "date", false, false, strG);
            AF(stb, "p91DateExchange", "Datum měn.kurzu", 0, null, "date", false, false, strG);

            strG = "Částka";
            AF(stb, "p91Amount_WithoutVat", "Bez dph", BO.TheGridDefColFlag.GridAndCombo, null,"num",true,false,strG);
            AF(stb, "BezDphKratKurz", "Bez dph x Kurz", 0, "case When a.j27ID=a.j27ID_Domestic Then p91Amount_WithoutVat Else p91Amount_WithoutVat*p91ExchangeRate End", "num",true,false,strG);
            AF(stb, "p91Amount_Debt", "Dluh", 0, null, "num",true,false,strG);
            AF(stb, "DluhKratKurz", "Dluh x Kurz", 0, "case When a.j27ID=a.j27ID_Domestic Then p91Amount_Debt Else p91Amount_Debt*p91ExchangeRate End", "num",true,false,strG);
            AF(stb, "p91Amount_TotalDue", "Celkem", BO.TheGridDefColFlag.GridAndCombo, null, "num",true,false,strG);
            AF(stb, "CelkemKratKurz", "Celkem x Kurz", 0, "case When a.j27ID = a.j27ID_Domestic Then p91Amount_TotalDue Else p91Amount_TotalDue*p91ExchangeRate End", "num",true,false,strG);
            AF(stb, "p91Amount_Vat", "Celkem dph", 0, null, "num",true,false,strG);
            AF(stb, "p91Amount_WithVat", "Vč.dph", 0, null, "num",true,false,strG);
            AF(stb, "p91RoundFitAmount", "Haléřové zaokrouhlení", 0, null, "num", true, false, strG);
            AF(stb, "p91Amount_WithoutVat_None", "Základ v nulové DPH", 0, null, "num", true, false, strG);
            AF(stb, "p91Amount_WithoutVat_Standard", "Základ ve standardní sazbě", 0, null, "num", true, false, strG);
            AF(stb, "p91Amount_WithoutVat_Low", "Základ ve snížené sazbě", 0, null, "num", true, false, strG);
            AF(stb, "p91Amount_WithoutVat_Special", "Základ ve speciální sazbě", 0, null, "num", true, false, strG);
            AF(stb, "p91Amount_Vat_Standard", "DPH ve standardní sazbě", 0, null, "num", true, false, strG);
            AF(stb, "p91Amount_Vat_Low", "DPH ve snížené sazbě", 0, null, "num", true, false, strG);
            AF(stb, "p91Amount_Vat_Special", "DPH ve speciální sazbě", 0, null, "num", true, false, strG);
            AF(stb, "p91VatRate_Standard", "DPH sazba standardní", 0, null, "num", true, false, strG);
            AF(stb, "p91VatRate_Low", "DPH sazba snížená", 0, null, "num", true, false, strG);
            AF(stb, "p91VatRate_Special", "DPH sazba speciální", 0, null, "num", true, false, strG);

            AF(stb, "p91ProformaBilledAmount", "Uhrazené zálohy", 0, null, "num");
            AF(stb, "p91ExchangeRate", "Měnový kurz", 0, null, "num");


            AF(stb, "p91Text1", "Text faktury");
            AF(stb, "p91Text2", "Technický text");

            strG = "Klient ve faktuře";
            oc=AF(stb, "p91Client_RegID", "IČ klienta");
            oc.DesignerGroup = strG;

            oc =AF(stb, "p91Client_VatID", "DIČ klienta", BO.TheGridDefColFlag.GridOnly);
            oc.DesignerGroup = strG;
            oc =AF(stb, "p91Client_ICDPH_SK", "IČ DPH (SK)");
            oc.DesignerGroup = strG;

            oc=AF(stb, "p91ClientAddress1_Street", "Ulice klienta");
            oc.DesignerGroup = strG;
            oc=AF(stb, "p91ClientAddress1_City", "Město klienta");
            oc.DesignerGroup = strG;
            oc=AF(stb, "p91ClientAddress1_ZIP", "PSČ klienta");
            oc.DesignerGroup = strG;
            oc=AF(stb, "p91ClientAddress1_Country", "Stát klienta");
            oc.DesignerGroup = strG;


            AF(stb, "ZapojeneOsoby", "Zapojené osoby",0, "dbo.j02_invoiced_persons_inline(a.p91ID)");

            AppendTimestamp(stb);
        }

        public List<BO.TheGridColumn> getDefaultPallete(bool bolComboColumns, BO.baseQuery mq)
        {
            
            List<BO.TheGridColumn> ret = new List<BO.TheGridColumn>();
            IEnumerable<BO.TheGridColumn> qry = null;
            if (bolComboColumns)
            {
                qry = _lis.Where(p => p.Prefix == mq.Prefix && (p.DefaultColumnFlag == BO.TheGridDefColFlag.GridAndCombo || p.DefaultColumnFlag==BO.TheGridDefColFlag.ComboOnly));
                
            }
            else
            {
                qry = _lis.Where(p => p.Prefix == mq.Prefix && (p.DefaultColumnFlag == BO.TheGridDefColFlag.GridAndCombo || p.DefaultColumnFlag == BO.TheGridDefColFlag.GridOnly));
            }

            foreach (BO.TheGridColumn c in qry)
            {
                ret.Add(Clone2NewInstance(c));
            }

            List<BO.EntityRelation> rels = _ep.getApplicableRelations(mq.Prefix);

            switch (mq.Prefix)
            {
                case "j02":
                    if (!bolComboColumns)
                    {
                        ret.Add(InhaleColumn4Relation("j02_j03", "j03User", "j03Login", rels, bolComboColumns));
                        ret.Add(InhaleColumn4Relation("j02_j03", "j03User", "j04Name", rels, bolComboColumns));
                    }                                       
                    ret.Add(InhaleColumn4Relation("j02_j07", "j07PersonPosition", "j07Name", rels, bolComboColumns));


                    break;
                
               
                //case "b02":
                //    ret.Add(InhaleColumn4Relation("b02_b01", "b01WorkflowTemplate", "b01Name", rels, bolComboColumns));
                   
                //    break;
                //case "b05":
                //    ret.Add(InhaleColumn4Relation("b05_j03", "j03User", "j03Login", rels, bolComboColumns));
                //    ret.Add(InhaleColumn4Relation("b05_b06", "b06WorkflowStep", "b06Name", rels, bolComboColumns));
                //    break;
                case "j61":
                    ret.Add(InhaleColumn4Relation("j61_x29", "x29Entity", "x29Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("j61_j02", "j02Person", "fullname_desc", rels, bolComboColumns));
                    
                    break;
                case "p28":
                    if (bolComboColumns)
                    {
                        ret.Add(InhaleColumn4Relation("p28_address_primary", "view_PrimaryAddress", "o38City", rels, bolComboColumns));
                    }
                    else
                    {
                        ret.Add(InhaleColumn4Relation("p28_address_primary", "view_PrimaryAddress", "FullAddress", rels, bolComboColumns));
                    }
                    
                    break;
                case "p36":
                    ret.Add(InhaleColumn4Relation("p36_j02","j02Person", "fullname_desc", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p36_j11", "j11Team", "j11Name", rels, bolComboColumns));
                    break;
                case "p42":
                    ret.Add(InhaleColumn4Relation("p42_p07", "p07ProjectLevel", "p07NameSingular", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p42_p07", "p07ProjectLevel", "p07Level", rels, bolComboColumns));
                    break;
                case "p51":
                    ret.Add(InhaleColumn4Relation("p51_j27", "j27Currency", "j27Code", rels, bolComboColumns));
                    break;
                case "p53":
                    ret.Add(InhaleColumn4Relation("p53_j27", "j27Currency", "j27Code", rels, bolComboColumns));
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
                case "x31":
                    ret.Add(InhaleColumn4Relation("x31_x29", "x29Entity", "x29Name", rels, bolComboColumns));
                    break;
                case "o40":
                    ret.Add(InhaleColumn4Relation("o40_j02", "j02Person", "fullname_desc", rels, bolComboColumns));
                    break;
                case "m62":
                    ret.Add(InhaleColumn4Relation("m62_j27slave", "j27Currency", "j27Code", rels, bolComboColumns));
                    
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

            if (bolComboColumns)
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
