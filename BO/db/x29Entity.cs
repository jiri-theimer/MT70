using System;


namespace BO
{
    public enum x29IdEnum
    {
        System = 1,
        j02Person = 102,
        j03User = 103,
        j04UserRole = 104,
        j07PersonPosition = 107,
        j11Team = 111,
        j18Region = 118,
        j19PaymentType = 119,
        j23NonPerson = 123,
        j24NonPersonType = 124,
        j27Currency = 127,
        j61TextTemplate = 161,
        j62MenuHome = 162,
        p41Project = 141,
        p45Budget = 345,
        o22Milestone = 222,
        o23Doc = 223,
        o25App = 225,
        o43ImapRobotHistory = 243,
        o27Attachment = 227,
        p28Contact = 328,
        p31Worksheet = 331,
        p40WorkSheet_Recurrence = 340,
        p47CapacityPlan = 347,
        p48ProjectGroup = 348,
        p49FinancialPlan = 349,
        p51PriceList = 351,
        p56Task = 356,
        p57TaskType = 357,
        p63Overhead = 363,
        p90Proforma = 390,
        p82Proforma_Payment = 382,
        p91Invoice = 391,
        b01WorkflowTemplate = 601,
        b02WorkflowStatus = 602,
        b05Workflow_History = 605,
        b06WorkflowStep = 606,
        b07Comment = 607,
        x67EntityRole = 967,
        x69EntityRole_Assign = 969,
        x31Report = 931,
        x40MailQueue = 940,
        x50Help = 950,
        p36LockPeriod = 336,
        p42ProjectType = 342,
        p92InvoiceType = 392,
        p89ProformaType = 389,
        p87BillingLanguage = 387,
        p29ContactType = 329,
        c21FondCalendar = 421,
        p34ActivityGroup = 334,
        p32Activity = 332,
        p80InvoiceAmountStructure = 380,
        p95InvoiceRow = 395,
        p98Invoice_Round_Setting_Template = 398,
        p71ApproveStatus = 371,
        p72PreBillingStatus = 372,
        p70BillingStatus = 370,
        j70QueryTemplate = 170,
        j77WorksheetStatTemplate = 177,
        x18EntityCategory = 918,
        x48SqlTask = 948,
        x97Translate=997,
        Approving = 999,
        _NotSpecified = 0
    }

    public class x29Entity
    {
        private int _x29ID { get; set; }
        public string x29NameSingle { get; set; }
        public string x29TableName { get; set; }
        public string x29Description { get; set; }
        public bool x29IsAttachment { get; set; }
        public bool x29IsReport { get; set; }

        public x29IdEnum X29ID
        {
            get
            {
                try
                {
                    return (x29IdEnum)_x29ID;
                }
                catch
                {
                    return x29IdEnum._NotSpecified;
                }
            }
        }
    }
}
