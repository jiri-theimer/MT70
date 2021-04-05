using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryP31:baseQuery
    {
        public int j02id { get; set; }
        public string tabquery { get; set; }

        public myQueryP31()
        {
            this.Prefix = "p31";
        }

        public override List<QRow> GetRows()
        {
           
            if (this.j02id > 0)
            {
                AQ("a.j02ID=@j02id", "j02id", this.j02id);
            }

            if (this.tabquery != null)
            {
                switch (this.tabquery)
                {
                    case "time":
                        AQ("p34x.p33ID=1", null, null);break;
                    case "expense":
                        AQ("p34x.p33ID IN (2,5) AND p34x.p34IncomeStatementFlag=1", null, null); break;
                    case "fee":
                        AQ("p34x.p33ID IN (2,5) AND p34x.p34IncomeStatementFlag=2", null, null); break;
                    case "kusovnik":
                        AQ("p34x.p33ID=3", null, null); break;
                }
            }


            return this.InhaleRows();

        }
    }
}
