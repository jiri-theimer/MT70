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
        public bool? is4sendmail { get; set; }

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

            if (this.is4sendmail == true)
            {
                AQ("(a.o40IsGlobalDefault=1 OR a.j02ID_Owner=@j02id) AND (GETDATE() BETWEEN a.o40ValidFrom AND a.o40ValidUntil)","j02id",CurrentUser.j02ID);
            }

            return this.InhaleRows();

        }
    }
}
