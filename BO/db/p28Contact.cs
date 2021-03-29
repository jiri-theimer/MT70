using System;

namespace BO
{
    public enum p28SupplierFlagENUM
    {
        _NotSpecified = 0,
        All = 1,
        SupplierOnly = 2,
        ClientAndSupplier = 3,
        NotClientNotSupplier = 4,
        ProjectClientOnly = 5,
        InvoiceReceiverOnly = 6
    }
    public class p28Contact:BaseBO
    {
        public int p29ID { get; set; }
        public int p92ID { get; set; }
        public int p87ID { get; set; }
        public int p51ID_Billing { get; set; }
        public int j02ID_Owner { get; set; }
        public int p51ID_Internal { get; set; }
        public int b02ID { get; set; }
        public int p63ID { get; set; }
        public int o25ID_Calendar { get; set; }
        public int j61ID_Invoice { get; set; }
        public bool p28IsDraft { get; set; }
        public bool p28IsCompany { get; set; }
        public string p28Code { get; set; }
        public string p28FirstName { get; set; }
        public string p28LastName { get; set; }
        public string p28TitleBeforeName { get; set; }
        public string p28TitleAfterName { get; set; }
        public string p28RegID { get; set; }
        public string p28VatID { get; set; }
        public string p28Person_BirthRegID { get; set; }
        public string p28CompanyName { get; set; }
        public string p28CompanyShortName { get; set; }
        public int p28ParentID { get; set; }
        public string p28InvoiceDefaultText1 { get; set; }
        public string p28InvoiceDefaultText2 { get; set; }
        public int p28InvoiceMaturityDays { get; set; }
        public string p28AvatarImage { get; set; }

        public double p28LimitHours_Notification { get; set; }
        public double p28LimitFee_Notification { get; set; }
        public string p28RobotAddress { get; set; }
        public p28SupplierFlagENUM p28SupplierFlag { get; set; }
        public string p28SupplierID { get; set; }
        public string p28ExternalPID { get; set; }
        public string p28BillingMemo { get; set; }
        public string p28Pohoda_VatCode { get; set; }
        public int j02ID_ContactPerson_DefaultInWorksheet { get; set; }
        public int j02ID_ContactPerson_DefaultInInvoice { get; set; }
        
        private string _Owner { get; set; }
        public string TagsInlineHtml { get; set; }
        public int p28Round2Minutes { get; set; }
        public string p28ICDPH_SK { get; set; }


        public string p28name { get; }

        public string p29Name { get; }
        

        public string p92Name { get; }
       
        public string b02Name { get; }
       
        public string p51Name_Billing { get; }
       
        
        public string p87Name { get; }
        
        public string Owner { get; }
       
        public string p28TreePath { get; }        
        public int p28TreeLevel { get; }        
        public int p28TreeIndex { get; }       
        public int p28TreePrev { get; }     
        public int p28TreeNext { get; }
    
    }
}
