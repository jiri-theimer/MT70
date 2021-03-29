using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryO38: baseQuery
    {
        public int p28id { get; set; }
        public int p41id { get; set; }

        public myQueryO38()
        {
            this.Prefix = "o38";
        }

        public override List<QRow> GetRows()
        {

            if (this.p28id > 0)
            {
                AQ("a.o38ID IN (SELECT o38ID FROM o37Contact_Address WHERE p28ID=@p28id)", "p28id", this.p28id);
            }
            if (this.p41id > 0)
            {
                AQ("a.o38ID IN (SELECT o38ID FROM o37Contact_Address WHERE p41ID=@p41id)", "p41id", this.p41id);
            }


            return this.InhaleRows();

        }
    }
}
