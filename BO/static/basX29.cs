using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public static class BASX29
    {
        public static string GetPrefixDb(string prefix)
        {
            if (prefix == "le5" || prefix == "le4" || prefix == "le3" || prefix == "le2" || prefix == "le1") return "p41";

            return prefix;
        }
        public static string GetPrefix(BO.x29IdEnum x29id)
        {
            //return x29id.ToString().Substring(0, 3);
            return Enum.GetName(x29id.GetType(), x29id).Substring(0, 3);

        }
        public static string GetEntity(BO.x29IdEnum x29id)
        {            
            return Enum.GetName(x29id.GetType(), x29id);
        }
        public static string GetEntity(string prefix)
        {
            var x29id = GetEnum(prefix);
            return Enum.GetName(x29id.GetType(), x29id);
        }
        public static int GetInt(string prefix)
        {
            return (int)GetEnum(prefix);
        }
        public static BO.x29IdEnum GetEnum(string prefix)
        {
            switch (prefix.Substring(0, 3))
            {                
                case "le1":
                    return BO.x29IdEnum.le1;
                case "le2":
                    return BO.x29IdEnum.le2;
                case "le3":
                    return BO.x29IdEnum.le3;
                case "le4":
                    return BO.x29IdEnum.le4;
                case "le5":
                    return BO.x29IdEnum.le5;
                case "p41":
                    return BO.x29IdEnum.p41Project;
                case "p91": return BO.x29IdEnum.p91Invoice;
                case "p90": return BO.x29IdEnum.p90Proforma;
                case "p82": return BO.x29IdEnum.p82Proforma_Payment;
                case "p28": return BO.x29IdEnum.p28Contact;
                case "p29": return BO.x29IdEnum.p29ContactType;
                case "b02": return BO.x29IdEnum.b02WorkflowStatus;
                case "b01": return BO.x29IdEnum.b01WorkflowTemplate;
                case "p42": return BO.x29IdEnum.p42ProjectType;
                case "p31": return BO.x29IdEnum.p31Worksheet;
                case "j03": return BO.x29IdEnum.j03User;
                case "j02": return BO.x29IdEnum.j02Person;
                case "p56": return BO.x29IdEnum.p56Task;
                case "p57": return BO.x29IdEnum.p57TaskType;
                case "o23": return BO.x29IdEnum.o23Doc;
                case "o25": return BO.x29IdEnum.o25App;
                case "o22": return BO.x29IdEnum.o22Milestone;
                case "o43": return BO.x29IdEnum.o43ImapRobotHistory;
                case "j23": return BO.x29IdEnum.j23NonPerson;
                case "j24": return BO.x29IdEnum.j24NonPersonType;
                case "x31": return BO.x29IdEnum.x31Report;
                case "b07": return BO.x29IdEnum.b07Comment;
                case "app": return BO.x29IdEnum.Approving;
                case "j18": return BO.x29IdEnum.j18Region;
                case "p45": return BO.x29IdEnum.p45Budget;
                case "p49": return BO.x29IdEnum.p49FinancialPlan;
                case "p47": return BO.x29IdEnum.p47CapacityPlan;
                case "x18": return BO.x29IdEnum.x18EntityCategory;
                case "p32": return BO.x29IdEnum.p32Activity;
                case "p34": return BO.x29IdEnum.p34ActivityGroup;
                case "p51": return BO.x29IdEnum.p51PriceList;
                case "p40": return BO.x29IdEnum.p40WorkSheet_Recurrence;
                case "j61": return BO.x29IdEnum.j61TextTemplate;
                case "p63": return BO.x29IdEnum.p63Overhead;
                case "p80": return BO.x29IdEnum.p80InvoiceAmountStructure;
                case "p92": return BO.x29IdEnum.p92InvoiceType;
                case "j07": return BO.x29IdEnum.j07PersonPosition;
                case "c21": return BO.x29IdEnum.c21FondCalendar;
                case "p98": return BO.x29IdEnum.p98Invoice_Round_Setting_Template;
                case "x67": return BO.x29IdEnum.x67EntityRole;
                case "x97":return BO.x29IdEnum.x97Translate;
                default:
                    return BO.x29IdEnum._NotSpecified;
            }
        }

        public static string GetAlias(int x29id)
        {
            switch (x29id)
            {
                case 141:
                    return "Projekt";
                case 328:
                    return "Klient";
                case 391:
                    return "Vyúčtování";
                case 390:
                    return "Záloha";                
                case 331:
                    return "Úkon";
                case 102:
                    return "Osoba";
                case 103:
                    return "Uživatel";
                case 223:
                    return "Dokument";
                case 356:
                    return "Úkol";
                case 607:
                    return "Poznámka";
                default:
                    BO.x29IdEnum c = (BO.x29IdEnum)x29id;
                    return GetEntity(c);
            }
        }
    }
}
