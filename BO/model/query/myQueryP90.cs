using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryP90:baseQuery
    {
        public int p28id { get; set; }
        public int p91id { get; set; }
        public int j27id { get; set; }

        public myQueryP90()
        {
            this.Prefix = "p90";
        }

        public override List<QRow> GetRows()
        {

            if (this.p91id > 0)
            {
                AQ("a.p90ID IN (SELECT zb.p90ID FROM p99Invoice_Proforma za INNER JOIN p82Proforma_Payment zb ON za.p82ID=zb.p82ID WHERE za.p91ID=@p91id)", "p91id", this.p91id);
            }
            if (this.p28id > 0)
            {
                AQ("a.p28ID=@p28id", "p28id", this.p28id);
            }
            if (this.j27id > 0)
            {
                AQ("a.j27ID=@j27id)", "j27id", this.j27id);
            }

            if (_searchstring != null && _searchstring.Length > 2)
            {
                AQ("(a.p90Code like '%'+@expr+'%' OR a.p90Text1 LIKE '%'+@expr+'%' OR a.p28ID IN (select p28ID FROM p28Contact WHERE p28Name like '%'+@expr+'%'))", "expr", _searchstring);

            }

            return this.InhaleRows();

        }
    }
}
