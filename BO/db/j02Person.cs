using System;

namespace BO
{
    public class j02Person : BaseBO
    {
        public int j07ID { get; set; }
        public int j17ID { get; set; }
        public int j18ID { get; set; }
        public int c21ID { get; set; }
        public int j03ID { get; }
        public string j03Login { get; }

        public string j02FirstName { get; set; }
        public string j02LastName { get; set; }
        public string j02TitleBeforeName { get; set; }
        public string j02TitleAfterName { get; set; }
        public string j02Code { get; set; }
        public string j02Email { get; set; }
        public string j02EmailSignature { get; set; }
        public string j02Phone { get; set; }
        public string j02Mobile { get; set; }
        public string j02JobTitle { get; set; }
        public string j02Office { get; set; }
        public string j02AvatarImage { get; set; }
        public string j02Description { get; set; }
        public bool j02IsIntraPerson { get; set; }
        public string j02RobotAddress { get; set; }
        public string j02ExternalPID { get; set; }

        public int j02TimesheetEntryDaysBackLimit { get; set; }
        public string j02TimesheetEntryDaysBackLimit_p34IDs { get; set; }
        public string j02Salutation { get; set; }
        public int j02WorksheetAccessFlag { get; set; }   // 1= nemá přístup k již vyfakturovaným úkonům
        public p72IdENUM p72ID_NonBillable { get; set; }
        public string j02DomainAccount { get; set; }
        public bool j02IsInvoiceEmail { get; set; }
        public string TagsInlineHtml { get; set; }

        public int j02ParentID { get; set; }  // nové pole ve verzi 6
        public string j02TreePath { get; set; }   // nové pole ve verzi 6
        public int j02TreeLevel { get; set; } // nové pole ve verzi 6
        public int j02TreeIndex { get; set; } // nové pole ve verzi 6
        public int j02TreePrev { get; set; }  // nové pole ve verzi 6
        public int j02TreeNext { get; set; }  // nové pole ve verzi 6
        public string j02TreeRelName { get; set; }    // nové pole ve verzi 6
        public int j02VirtualParentID { get; set; }   // nové pole verze 6
        public string j02InvoiceSignatureFile { get; set; }   // nové pole verze 6
        public int c21ScopeFlag { get; }
        public int j02NotifySubscriberFlag { get; set; }  // 1-nepřeje si zasílat x46 notifikace

        public string FullNameAsc
        {
            get
            {
                return (j02TitleBeforeName + " " + j02FirstName + " " + j02LastName + " " + j02TitleAfterName).Trim();
            }
        }
        public string FullNameDesc
        {
            get
            {
                return (j02LastName + " " + j02FirstName + " " + j02TitleBeforeName).Trim();
            }
        }
        public string FullNameDescWithEmail
        {
            get
            {
                return this.FullNameDesc + " [" + this.j02Email + "]";
            }
        }
        public string FullNameDescWithJobTitle
        {
            get
            {
                if (this.j02JobTitle != "")
                    return this.FullNameDesc + " [" + this.j02JobTitle + "]";
                else
                    return this.FullNameDesc;
            }
        }

        public string j07Name { get; }       
        public string _c21Name { get; }
        public string c21Name { get; }       
        public string j18Name { get; }
        

    }
}
