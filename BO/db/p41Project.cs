using System;

namespace BO
{
    public enum p41WorksheetOperFlagEnum
    {
        _NotSpecified = 0,                   // 0 - do projektu je povoleno zapisovat i fakturovat worksheet
        NoEntryData = 1,                     // 1 - V projektu není povoleno zapisování úkonů
        OnlyEntryData = 2,                   // 2 - V projektu je povoleno pouze zapisování úkonů
        WithTaskOnly = 3,                      // 3 - V projektu je povoleno zapisovat úkony pouze přes úkol
        WithTaskHoursOnly = 4,                      // 4 - V projektu je povoleno vykazovat hodiny pouze přes úkol
        NoLimit = 9
    }

    public enum p41BillingFlagEnum
    {
        _NotSpecified = 0,
        FixedPrice = 6,                  // projekt s pevnou cenou bez fakturace hodin    
        WithoutBilling = 99                // interní projekt - se nebude fakturovat
    }

    public enum p41ReportingFlagEnum
    {
        p28Name_p41Name = 0,
        p41Name = 1,
        p41Code_p41name = 2
    }


    public class p41Project:BaseBO
    {
        public int p42ID { get; set; }
        public int p28ID_Client { get; set; }
        public int p28ID_Billing { get; set; }
        public int b02ID { get; set; }
        public int p87ID { get; set; }
        public int p51ID_Billing { get; set; }
        public int p51ID_Internal { get; set; }
        public int p92ID { get; set; }
        public int j18ID { get; set; }
        public int p61ID { get; set; }
        public int o25ID_Calendar { get; set; }
        public int j02ID_Owner { get; set; }

        public string p41Name { get; set; }
        public bool p41IsDraft { get; set; }
        public string p41NameShort { get; set; }
        public string p41RobotAddress { get; set; }
        public string p41ExternalPID { get; set; }
        public int p41ParentID { get; set; }
        public string p41BillingMemo { get; set; }
        public BO.p72IdENUM p72ID_NonBillable { get; set; }
        public BO.p72IdENUM p72ID_BillableHours { get; set; }

        public int p65ID { get; set; }
        public string p41RecurNameMask { get; set; }
        public DateTime? p41RecurBaseDate { get; set; }
        public int p41RecurMotherID { get; set; }
        public bool p41IsStopRecurrence { get; set; }

        public string TagsInlineHtml { get; }
        public int p41TreeOrdinary { get; set; }

        public p41BillingFlagEnum p41BillingFlag { get; set; } = p41BillingFlagEnum._NotSpecified;
        public p41ReportingFlagEnum p41ReportingFlag { get; set; } = p41ReportingFlagEnum.p28Name_p41Name;

      
        public string p41Code { get; set; }
       

        public string p41InvoiceDefaultText1 { get; set; }
        public string p41InvoiceDefaultText2 { get; set; }
        public int p41InvoiceMaturityDays { get; set; }

        public p41WorksheetOperFlagEnum p41WorksheetOperFlag { get; set; } = p41WorksheetOperFlagEnum.NoLimit;

        public DateTime? p41PlanFrom { get; set; }
        public DateTime? p41PlanUntil { get; set; }

        public int p41LimitWipFlag { get; set; }
        public double p41LimitHours_Notification { get; set; }
        public double p41LimitFee_Notification { get; set; }
        public bool p41IsNoNotify { get; set; }
        public int j02ID_ContactPerson_DefaultInWorksheet { get; set; }

        public int j02ID_ContactPerson_DefaultInInvoice { get; set; }

        public string Owner { get; }
        public string p42name { get; }
        public string j18Name { get; }

       
        public string b02Name { get; }
        public int b01ID { get; }
        
        public string p92Name { get; }
       
        public string Client { get; }
       public int x38ID { get; }

        public int p07Level { get; }
        public string p07Name { get; set; }
        public int p41TreeLevel { get; set; }
       
        public int p41TreeIndex { get; set; }
       
        public int p41TreePrev { get; set; }        
        public int p41TreeNext { get; set; }
        public string p41TreePath { get; }


        public string p51Name_Billing { get; }
        
        public int p87ID_Client { get; }
       
       
        public string FullName
        {
            get
            {
                string s = this.p41NameShort;
                if (s == null)
                    s = this.p41Name;
                if (this.p41TreePath != null)
                    s = this.p41TreePath;
                
                s += " (" + this.p41Code + ")";
                if (this.p28ID_Client > 0)
                {
                    if (Client.Length > 25)
                        return BO.BAS.LeftString(this.Client, 25) + "... - " + s;
                    else
                        return this.Client + " - " + s;
                }
                return s;
            }
        }
        public string ProjectWithMask(int maskindex)
        {
            string s = this.p41NameShort;
            if (s == null)
                s = this.p41Name;

            switch (maskindex)
            {
                case 1:
                    {
                        return s;  // pouze název
                    }

                case 2:
                    {
                        return s + " [" + this.p41Code + "]";  // název projektu + kód
                    }

                case 3:
                    {
                        return s + " [" + this.Client + "]";    // název+klient
                    }

                case 4:
                    {
                        return this.p41Code;                // pouze kód projektu
                    }

                case 5:
                    {
                        if (this.p41TreePath != null)
                            return this.p41TreePath;
                        else
                            return this.PrefferedName; // nadřízený+podřízený projekt

                       
                    }

                default:
                    {
                        return FullName + " [" + this.p41Code + "]";
                    }
            }
        }
        public string PrefferedName
        {
            get
            {
                if (this.p41NameShort != null)
                    return this.p41NameShort;


                return this.p41Name;
            }
        }
        
        
    }
}
