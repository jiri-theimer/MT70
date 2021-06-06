using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryO23:baseQuery
    {
        public int x29id { get; set; }
        public int recpid { get; set; }
        public int p28id { get; set; }
        public int j02id { get; set; }
        public int p41id { get; set; }
        public int p91id { get; set; }
        public int leindex { get; set; }   //nadřízená vertikální úrověň projektu #1 - #4
        public int lepid { get; set; }     //nadřízená vertikální úrověň projektu, hodnota p41id

        public myQueryO23()
        {
            this.Prefix = "o23";
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
            if (this.p91id > 0)
            {
                this.x29id = 391; this.recpid = this.p91id;
            }


            if (this.x29id > 0 && this.recpid>0)
            {
                AQ("a.o23ID IN (select xa.o23ID FROM x19EntityCategory_Binding xa INNER JOIN x20EntiyToCategory xb ON xa.x20ID=xb.x20ID WHERE xb.x29ID=@x29id AND xa.x19RecordPID=@recpid)", "x29id", this.x29id,"AND",null,null,"recpid",this.recpid);
            }


            if (this.leindex > 0 && this.lepid > 0)
            {
                AQ($"a.o23ID IN (select xa.o23ID FROM x19EntityCategory_Binding xa INNER JOIN x20EntiyToCategory xb ON xa.x20ID=xb.x20ID AND xb.x29ID=141 INNER JOIN p41Project xc ON xa.x19RecordPID=xc.p41ID WHERE xc.p41ID=@lepid OR xc.p41ID_P07Level{this.leindex}=@lepid)", "lepid", this.lepid);
            }

            return this.InhaleRows();

        }
    }
}
