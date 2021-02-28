using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p88Repeater:BO.p88InvoiceHeader_BankAccount
    {
        public string ComboJ27 { get; set; }
        public string ComboP86 { get; set; }
        public bool IsTempDeleted { get; set; }
        public string TempGuid { get; set; }
        public string CssTempDisplay
        {
            get
            {
                if (this.IsTempDeleted == true)
                {
                    return "display:none;";
                }
                else
                {
                    return "display:table-row;";
                }
            }
        }
    }
    public class p93Record:BaseRecordViewModel
    {
        public BO.p93InvoiceHeader Rec { get; set; }
        public string LogoFile { get; set; }
        public string SignatureFile { get; set; }

        public string UploadGuidLogo { get; set; }
        public string UploadGuidSignature { get; set; }

        public bool IsDeleteLogo { get; set; }
        public bool IsDeleteSignature { get; set; }

        public List<p88Repeater> lisP88 { get; set; }

    }
}
