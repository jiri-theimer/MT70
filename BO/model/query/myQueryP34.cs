using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryP34:baseQuery
    {
        public int p42id { get; set; }
        public myQueryP34()
        {
            this.Prefix = "p34";
        }

        public override List<QRow> GetRows()
        {
            if (this.p42id > 0)
            {
                AQ("a.p34ID IN (select p34ID FROM p43ProjectType_Workload WHERE p42ID=@p42id)", "p42id", this.p42id);
            }


            return this.InhaleRows();

        }
    }
}
