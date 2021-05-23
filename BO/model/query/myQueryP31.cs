using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
   
    public class myQueryP31 : baseQuery
    {
        public int j02id { get; set; }
        public int p28id { get; set; }
        public int p41id { get; set; }
        public int p32id { get; set; }
        public int p91id { get; set; }
        public int p56id { get; set; }
        public int p70id { get; set; }
        
        public bool? p32isbillable { get; set; }
        public int p71id { get; set; }
        public string tabquery { get; set; }
        
        

        public myQueryP31()
        {
            this.Prefix = "p31";
        }

        public override List<QRow> GetRows()
        {
            if (this.p31statequery > 0) Handle_p31StateQuery_p31();

            if (this.global_d1 != null && this.global_d2 != null)
            {
                DateTime d1 = Convert.ToDateTime(this.global_d1); DateTime d2 = Convert.ToDateTime(this.global_d2);
                if (d1.Year>1900 || d2.Year < 3000)
                {
                    switch (this.period_field)
                    {
                        case "p31DateInsert":
                            AQ("a.p31DateInsert BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", this.global_d2_235959);
                            break;
                        case "p91Date":
                            AQ("p91x.p91Date BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", d2);
                            break;
                        case "p91DateSupply":
                            AQ("p91x.p91DateSupply BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", d2);
                            break;
                        case "p31Date":
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
            if (this.p56id > 0)
            {
                AQ("a.p56ID=@p56id", "p56id", this.p56id);
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

        private void Handle_p31StateQuery_p31()
        {
            switch (this.p31statequery)
            {
                case 1: //Rozpracované
                    this.iswip = true; break;
                case 2://rozpracované s korekcí
                    AQ("a.p71ID IS NULL AND a.p91ID IS NULL AND a.p72ID_AfterTrimming IS NOT NULL", null, null); break;
                case 3://nevyúčtované
                    AQ("a.p91ID IS NULL AND GETDATE()<a.p31ValidUntil", null, null); break;
                case 4://schválené
                    this.isapproved_and_wait4invoice = true; break;  //AQ("a.p71ID=1 AND a.p91ID IS NULL", null, null); break;
                case 5://schválené jako fakturovat
                    AQ("a.p71ID=1 AND a.p72ID_AfterApprove=4 AND a.p91ID IS NULL", null, null); break;
                case 6://schválené jako paušál
                    AQ("a.p71ID=1 AND a.p72ID_AfterApprove=6 AND a.p91ID IS NULL", null, null); break;
                case 7://schválené jako odpis
                    AQ("a.p71ID=1 AND a.p72ID_AfterApprove IN (2,3) AND a.p91ID IS NULL", null, null); break;
                case 8://schválené jako fakturovat později
                    AQ("a.p71ID=1 AND a.p72ID_AfterApprove=7 AND a.p91ID IS NULL", null, null); break;
                case 9://neschválené
                    AQ("a.p71ID=2 AND a.p91ID IS NULL", null, null); break;
                case 10://vyúčtované
                    this.isinvoiced = true; break;
                case 11://DRAFT vyúčtování
                    AQ("a.p91ID IS NOT NULL AND p91x.p91IsDraft=1", null, null); break;
                case 12://vyúčtované jako fakturovat
                    AQ("a.p70ID=4 AND a.p91ID IS NOT NULL", null, null); break;
                case 13://vyúčtované jako paušál
                    AQ("a.p70ID=6 AND a.p91ID IS NOT NULL", null, null); break;
                case 14://vyúčtované jako odpis
                    AQ("a.p70ID IN (2,3) AND a.p91ID IS NOT NULL", null, null); break;
                case 15: //v archivu
                    AQ("a.p31ValidUntil<GETDATE()", null, null); break;
                case 16://rozpracované Fa aktivita
                    AQ("a.p71ID IS NULL AND a.p91ID IS NULL AND p32x.p32IsBillable=1", null, null); break;
                case 17://rozpracované Fa aktivita
                    AQ("a.p71ID IS NULL AND a.p91ID IS NULL AND p32x.p32IsBillable=0", null, null); break;
            }
        }
    }
}
