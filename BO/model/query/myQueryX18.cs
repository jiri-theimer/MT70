using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryX18:baseQuery
    {
        public int x29id { get; set; }
        public myQueryX18()
        {
            this.Prefix = "x18";
        }

        public override List<QRow> GetRows()
        {
            if (this.x29id > 0)
            {
                AQ("a.x18ID IN (select x18ID FROM x20EntiyToCategory WHERE x29ID=@x29id)", "x29id", this.x29id);
            }


            return this.InhaleRows();

        }
    }
}
