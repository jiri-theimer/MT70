using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p93Record:BaseRecordViewModel
    {
        public BO.p93InvoiceHeader Rec { get; set; }
        public string LogoFile { get; set; }
        public string SignatureFile { get; set; }

        public string UploadGuidLogo { get; set; }
        public string UploadGuidSignature { get; set; }

        
    }
}
