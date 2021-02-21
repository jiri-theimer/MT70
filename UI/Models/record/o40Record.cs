using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class o40Record:BaseRecordViewModel
    {
        public BO.o40SmtpAccount Rec { get; set; }
        public string ComboPerson { get; set; }
        public int UsageFlag { get; set; }
        public bool IsUseSSL { get; set; }
    }
}
