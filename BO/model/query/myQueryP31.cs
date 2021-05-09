using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public enum myQueryP31_Period
    {
        p31Date = 1,
        p31DateInsert = 2,
        p91Date = 3,
        p91DateSupply = 4,
        p31TimerTimestamp = 5
    }
    public class myQueryP31:baseQuery
    {
        public int j02id { get; set; }
        public int p28id { get; set; }
        public int p41id { get; set; }
        public int p91id { get; set; }
        public string tabquery { get; set; }
        public myQueryP31_Period periodfield { get; set; }

        public myQueryP31()
        {
            this.Prefix = "p31";
        }

        public override List<QRow> GetRows()
        {
            if (this.global_d1 !=null && this.global_d2 != null)
            {                
                switch (this.periodfield)
                {
                    case myQueryP31_Period.p31Date:
                    default:
                        AQ("a.p31Date BETWEEN @d1 AND @d2", "d1", this.global_d1, "AND", null, null, "d2");
                        break;
                }
                
            }
            if (this.j02id > 0)
            {
                AQ("a.j02ID=@j02id", "j02id", this.j02id);
            }
            if (this.p41id > 0)
            {
                AQ("a.p41ID=@p41id", "p41id", this.p41id);
            }
            if (this.p28id > 0)
            {
                AQ("a.p41ID IN (select p41ID FROM p41Project WHERE p28ID_Client=@p28id)", "p28id", this.p28id);
            }
            if (this.p91id > 0)
            {
                AQ("a.p91ID=@p91id", "p91id", this.p91id);
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
