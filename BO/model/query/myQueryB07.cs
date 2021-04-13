using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryB07:baseQuery
    {
        public int recpid { get; set; }
        
        public myQueryB07()
        {
            this.Prefix = "b07";
        }

        public override List<QRow> GetRows()
        {
            if (this.x29id > 0)
            {
                AQ("a.x29ID=@x29id", "x29id", this.x29id);
            }



            if (this.recpid > 0)
            {
                AQ("a.b07RecordPID=@recpid", "recpid", this.recpid);
            }
            

            return this.InhaleRows();

        }
    }
}
