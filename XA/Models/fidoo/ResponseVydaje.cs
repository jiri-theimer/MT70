using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XA.Models.fidoo
{
    public class ResponseVydaje
    {
        public bool complete { get; set; }
        public string nextOffsetToken { get; set; }
        public List<Expenselist> expenseList { get; set; }
        
    }

    public class Expenselist
    {
        public string expenseId { get; set; }
        public string ownerUserId { get; set; }
        public DateTime dateTime { get; set; }
        public DateTime lastEditDateTime { get; set; }
        public string name { get; set; }
        public string classState { get; set; }
        public string type { get; set; }
        public float amount { get; set; }
        public string currency { get; set; }
        public string shortId { get; set; }
        public List<string> receiptIds { get; set; }
        public float privateAmount { get; set; }
        public string cardId { get; set; }
        public string travelReportId { get; set; }
        public float? vatAmount { get; set; }
        public float? vatRate { get; set; }
        public string accountCode { get; set; }
        public string accountCredit { get; set; }
        public string accountDebit { get; set; }
        public string vatAccountCode { get; set; }
        public string vatAccountCredit { get; set; }
        public string vatAccountDebit { get; set; }
        public List<string> costCenterIds { get; set; }
        public List<string> projectIds { get; set; }
        public string vatBreakDownId { get; set; }
        public string taxableDate { get; set; }
        public int? merchantIdentificationNumber { get; set; }
        public string merchantVatId { get; set; }
        public string merchantName { get; set; }
        public string merchantAddress { get; set; }
        public string cardTransactionId { get; set; }
        public float exchangeRate { get; set; }
        public string description { get; set; }
        public bool closed { get; set; }
        public List<string> receiptUrls { get; set; }
    }
}
