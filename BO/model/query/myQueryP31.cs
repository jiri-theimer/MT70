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
    public class myQueryP31 : baseQuery
    {
        public int j02id { get; set; }
        public int p28id { get; set; }
        public int p41id { get; set; }
        public int p32id { get; set; }
        public int p91id { get; set; }
        public int p70id { get; set; }
        public bool? iswip { get;set; }
        public bool? isinvoiced { get; set; }
        public bool? isapproved_and_wait4invoice { get; set; }
        public bool? p32isbillable { get; set; }
        public int p71id { get; set; }
        public string tabquery { get; set; }
        public myQueryP31_Period periodfield { get; set; }
        

        public myQueryP31()
        {
            this.Prefix = "p31";
        }

        public override List<QRow> GetRows()
        {

            if (this.global_d1 != null && this.global_d2 != null)
            {
                DateTime d1 = Convert.ToDateTime(this.global_d1); DateTime d2 = Convert.ToDateTime(this.global_d2);
                switch (this.periodfield)
                {
                    case myQueryP31_Period.p31DateInsert:
                        AQ("a.p31DateInsert BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", BO.BAS.ConvertDateTo235959(d2));
                        break;
                    case myQueryP31_Period.p91Date:
                        AQ("p91x.p91Date BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", d2);
                        break;
                    case myQueryP31_Period.p91DateSupply:
                        if (d2 > d1)
                        {
                            AQ("p91x.p91DateSupply BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", d2);
                        }
                        else
                        {
                            AQ("p91x.p91DateSupply = @d1", "d1", d1);
                        }
                        break;
                    case myQueryP31_Period.p31Date:
                    default:
                        if (d2 > d1)
                        {
                            AQ("a.p31Date BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", d2);
                        }
                        else
                        {
                            AQ("a.p31Date = @d1", "d1", d1);
                        }
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
            if (this.p32id > 0)
            {
                AQ("a.p32ID=@p32id", "p32id", this.p32id);
            }
            if (this.p70id > 0)
            {
                if (this.p70id == 23)
                {
                    AQ("a.p70ID IN (2,3)",null,null);
                }
                else
                {
                    AQ("a.p70ID=@p70id", "p70id", this.p70id);
                }
                
            }
            if (this.p71id > 0)
            {
                AQ("a.p71ID=@p71id", "p71id", this.p71id);
            }
            if (this.p32isbillable != null)
            {
                AQ("p32x.p32IsBillable=@billable","billable",this.p32isbillable);
            }
            if (this.iswip != null)
            {
                if (this.iswip == true)
                {
                    AQ("a.p71ID IS NULL AND a.p91ID IS NULL",null,null);
                }
                else
                {
                    AQ("a.p71ID IS NOT NULL", null, null);
                }                
            }
            if (this.isinvoiced != null)
            {
                if (this.isinvoiced == true)
                {
                    AQ("a.p91ID IS NOT NULL", null, null);
                }
                else
                {
                    AQ("a.p91ID IS NULL", null, null);
                }
            }
            if (this.isapproved_and_wait4invoice != null)
            {
                if (this.isapproved_and_wait4invoice == true)
                {
                    AQ("a.p71ID=1 AND a.p91ID IS NULL", null, null);
                }
                else
                {
                    AQ("a.p91ID IS NULL", null, null);
                }
            }

            if (this.tabquery != null)
            {
                switch (this.tabquery)
                {
                    case "time":
                        AQ("p34x.p33ID=1", null, null); break;
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
