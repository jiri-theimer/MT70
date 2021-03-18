using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryO40: baseQuery
    {
        public int j02id { get; set; }

        public myQueryO40()
        {
            this.Prefix = "o40";
        }

        public override List<QRow> GetRows()
        {

            if (this.j02id > 0)
            {
                AQ("a.j02ID_Owner=@j02id", "j02id", this.j02id);
            }

            return this.InhaleRows();

        }
    }
}
