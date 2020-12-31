using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class j02PersonSum
    {
        public int p56_Actual_Count { get; set; }
        public int p56_Closed_Count { get; set; }
        public int o22_Actual_Count { get; set; }
        public int p91_Count { get; set; }
        public int p31_Wip_Time_Count { get; set; }
        public int p31_Wip_Expense_Count { get; set; }
        public int p31_Wip_Fee_Count { get; set; }
        public int p31_Wip_Kusovnik_Count { get; set; }
        public int p31_Approved_Time_Count { get; set; }
        public int p31_Approved_Expense_Count { get; set; }
        public int p31_Approved_Fee_Count { get; set; }
        public int p31_Approved_Kusovnik_Count { get; set; }
        public int b07_Count { get; set; }
        public int o23_Count { get; set; }
        public int o43_Count { get; set; }
        public int last_o23ID { get; set; }

        public DateTime? Last_Access { get; set; }
        public string Last_Worksheet { get; set; }
        public int j13StarIndex { get; set; }
    }
}
