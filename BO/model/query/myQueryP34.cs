using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryP34:baseQuery
    {
        public int p42id { get; set; }
        public int p36id { get; set; }  //sešity z uzamčeného období
        public bool? ismoneyinput { get; set; }
        public myQueryP34()
        {
            this.Prefix = "p34";
        }

        public override List<QRow> GetRows()
        {
            if (this.p42id > 0)
            {
                AQ("a.p34ID IN (select p34ID FROM p43ProjectType_Workload WHERE p42ID=@p42id)", "p42id", this.p42id);
            }
            if (this.p36id > 0)
            {
                AQ("a.p34ID IN (select p34ID FROM p37LockPeriod_Sheet WHERE p36ID=@p36id)", "p36id", this.p36id);
            }
            if (this.ismoneyinput != null)
            {
                if (this.ismoneyinput == true)
                {
                    AQ("a.p33ID IN (2,5)", null, null);
                }
                else
                {
                    AQ("a.p33ID IN (1,3)", null, null);                    
                }
            }


            return this.InhaleRows();

        }
    }
}
