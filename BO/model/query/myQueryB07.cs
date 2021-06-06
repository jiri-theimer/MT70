using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryB07:baseQuery
    {
        public int x29id { get; set; }
        public int recpid { get; set; }        
        public int j02id { get; set; }
        public int p28id { get; set; }
        public int p41id { get; set; }
        public int o23id { get; set; }
        public int leindex { get; set; }   //nadřízená vertikální úrověň projektu #1 - #4
        public int lepid { get; set; }     //nadřízená vertikální úrověň projektu, hodnota p41id

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
            if (this.o23id > 0)
            {
                this.x29id = 223; this.recpid = this.o23id;                
            }
            if (this.x29id > 0)
            {
                AQ("a.x29ID=@x29id", "x29id", this.x29id);
                if (this.x29id == 223)
                {
                    AQ("(a.b07Value NOT LIKE 'upload' OR a.b07Value IS NULL)", null, null);   //vyloučit technický upload záznam dokumentu
                }
            }

            if (this.recpid > 0)
            {
                AQ("a.b07RecordPID=@recpid", "recpid", this.recpid);
            }

            if (this.leindex > 0 && this.lepid > 0)
            {
                AQ($"a.x29ID=141 AND (a.b07Value NOT LIKE 'upload' OR a.b07Value IS NULL) AND a.b07RecordPID IN (select p41ID FROM p41Project WHERE p41ID=@lepid OR p41ID_P07Level{this.leindex}=@lepid)", "lepid", this.lepid);
            }

            return this.InhaleRows();

        }
    }
}
