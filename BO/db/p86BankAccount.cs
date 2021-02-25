using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class p86BankAcc: BaseBO  //kvůli poli p86BankAccount se třída jmenuje jinak než db tabulka!
    {
        public string p86Name { get; set; }
        public string p86BankName { get; set; }
        public string p86BankAccount { get; set; }
        public string p86BankCode { get; set; }
        public string p86SWIFT { get; set; }
        public string p86IBAN { get; set; }
        public string p86BankAddress { get; set; }
    }
}
