using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryJ61:baseQuery
    {
        public int x29id { get; set; }
        public myQueryJ61()
        {
            this.Prefix = "j61";
        }

        public override List<QRow> GetRows()
        {
            if (this.x29id > 0)
            {
                AQ("a.x29ID=@x29id", "x29id", this.x29id);
            }

            if (this.MyRecordsDisponible)
            {

                AQ("(a.j02ID_Owner=@j02id_query OR a.j61ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=161 AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query))))", "j02id_query", this.CurrentUser.j02ID);
            }

            return this.InhaleRows();

        }
    }
}
