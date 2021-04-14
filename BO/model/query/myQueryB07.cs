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
        public int j02id { get; set; }
        public int p28id { get; set; }
        public int p41id { get; set; }
        

        public myQueryB07()
        {
            this.Prefix = "b07";
        }

        public override List<QRow> GetRows()
        {
            if (this.j02id > 0)
            {
                this.x29id = 102;this.recpid = this.j02id;
            }
            if (this.p28id > 0)
            {
                this.x29id = 328; this.recpid = this.p28id;
            }
            if (this.p41id > 0)
            {
                this.x29id = 141; this.recpid = this.p41id;
            }

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
