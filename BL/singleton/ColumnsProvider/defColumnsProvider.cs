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
            AA("j27Code", "Měna", gdc1, null, "string", false, true);
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
