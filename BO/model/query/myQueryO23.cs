using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryO23:baseQuery
    {
        public int recpid { get; set; }
        

        public myQueryO23()
        {
            this.Prefix = "o23";
        }

        public override List<QRow> GetRows()
        {

            if (this.x29id > 0 && this.recpid>0)
            {
                AQ("a.o23ID IN (select xa.o23ID FROM x19EntityCategory_Binding xa INNER JOIN x20EntiyToCategory xb ON xa.x20ID=xb.x20ID WHERE xb.x29ID=@x29id AND xa.x19RecordPID=@recpid)", "x29id", this.x29id,"AND",null,null,"recpid",this.recpid);
            }
            
            


            return this.InhaleRows();

        }
    }
}
