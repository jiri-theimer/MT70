using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryP56:baseQuery
    {
        public int p57id { get; set; }
        public int p41id { get; set; }
        public int j02id_owner { get; set; }
        public int p91id { get; set; }
        public int j02id { get; set; }
        public int b02id { get; set; }


        public myQueryP56()
        {
            this.Prefix = "p56";
        }

        public override List<QRow> GetRows()
        {
            if (this.global_d1 != null && this.global_d2 != null)
            {
                DateTime d1 = Convert.ToDateTime(this.global_d1); DateTime d2 = Convert.ToDateTime(this.global_d2);
                if (d1.Year > 1900 || d2.Year < 3000)
                {
                    switch (this.period_field)
                    {
                        case "p56DateInsert":
                            AQ("a.p56DateInsert BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", this.global_d2_235959);
                            break;
                        case "p56PlanFrom":
                            AQ("a.p56PlanFrom BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", this.global_d2_235959);
                            break;
                        case "p56PlanUntil":
                            AQ("a.p56PlanUntil BETWEEN @d1 AND @d2", "d1", d1, "AND", null, null, "d2", this.global_d2_235959);
                            break;
                        case "p91Date":
                            AQ("a.p56ID IN (select xa.p56ID FROM p31Worksheet xa INNER JOIN p91Invoice xb ON xa.p91ID=xb.p91ID WHERE xa.p56ID IS NOT NULL AND xb.p91Date BETWEEN @d1 AND @d2)", "d1", d1, "AND", null, null, "d2", d2);
                            break;
                        case "p91DateSupply":
                            AQ("a.p56ID IN (select xa.p56ID FROM p31Worksheet xa INNER JOIN p91Invoice xb ON xa.p91ID=xb.p91ID WHERE xa.p56ID IS NOT NULL AND xb.p91DateSupply BETWEEN @d1 AND @d2)", "d1", d1, "AND", null, null, "d2", d2);
                            break;
                        case "p31Date":
                        default:
                            AQ("a.p56ID IN (select p56ID FROM p31Worksheet WHERE p56ID IS NOT NULL AND p31Date BETWEEN @d1 AND @d2)", "d1", d1, "AND", null, null, "d2", d2);
                            break;
                    }
                }
            }
            if (this.p57id > 0)
            {
                AQ("a.p57ID=@p57id)", "p57id", this.p57id);
            }
            if (this.p41id > 0)
            {
                AQ("(a.p41ID=@p41id)", "p41id", this.p41id);
            }
            if (this.j02id_owner > 0)
            {
                AQ("a.j02ID_Owner=@ownerid)", "ownerid", this.j02id_owner);
            }
           
            
            if (this.b02id > 0)
            {
                AQ("a.b02ID=@b02id", "b02id", this.b02id);
            }
            
            if (this.p91id > 0)
            {
                AQ("a.p56ID IN (select p56ID FROM p31Worksheet WHERE p56ID IS NOT NULL AND p91ID=@p91id)", "p91id", this.p91id);
            }
            if (this.j02id > 0)
            {
                //obdržel jakoukoliv roli v úkolu
                AQ("a.p56ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=356 AND (x69.j02ID=@j02id OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID = @j02id)))", "j02id", this.j02id);
            }
            return this.InhaleRows();

        }
    }
}
