using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryX38:baseQuery
    {
        public int x29id { get; set; }
        public myQueryX38()
        {
            this.Prefix = "x38";
        }

        public override List<QRow> GetRows()
        {
            if (this.x29id > 0)
            {
                AQ("a.x29ID=@x29id", "x29id", this.x29id);
            }


            return this.InhaleRows();

        }
    }
}
