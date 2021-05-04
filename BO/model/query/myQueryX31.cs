using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryX31:baseQuery
    {
        public int x29id { get; set; }
        public myQueryX31()
        {
            this.Prefix = "x31";
        }

        public override List<QRow> GetRows()
        {
            if (this.x29id > 0)
            {
                AQ("a.x29ID=@x29id", "x29id", this.x29id);
            }

            if (this.MyRecordsDisponible)
            {
                
                if (!this.CurrentUser.IsAdmin)
                {
                    bool b = this.CurrentUser.TestPermission(x53PermValEnum.GR_P31_Reader);
                    if (b) b = this.CurrentUser.IsRatesAccess;  //oprávnění ke všem úkonům v db + k sazbám
                    if (!b)
                    {
                        AQ("a.X31ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=931 AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query)))", "j02id_query", this.CurrentUser.j02ID);
                    }                    
                }
            }

            return this.InhaleRows();

        }
    }
}
