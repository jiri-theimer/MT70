using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryP32 : baseQuery
    {
        public int p34id { get; set; }
        public int p38id { get; set; }
        public int p95id { get; set; }
        public int p33id {get;set;}
        public int p61id { get; set; }
        public int x15id { get; set; }
        public bool? ismoneyinput { get; set; }
        public bool? isbillable { get; set; }
        public myQueryP32()
        {
            this.Prefix = "p32";
        }

        public override List<QRow> GetRows()
        {
            if (this.p34id > 0)
            {
                AQ("a.p34ID=@p34id)", "p34id", this.p34id);
            }
            if (this.p38id > 0)
            {
                AQ("a.p38ID=@p38id)", "p38id", this.p38id);
            }
            if (this.p95id > 0)
            {
                AQ("a.p95ID=@p95id)", "p95id", this.p95id);
            }
            if (this.p33id > 0)
            {
                AQ("a.p34ID IN (select p34ID FROM p34ActivityGroup WHERE p33ID=@p33id)", "p33id", this.p33id);
            }
            if (this.x15id > 0)
            {
                AQ("a.x15ID=@x15id)", "x15id", this.x15id);
            }
            if (this.p61id > 0)
            {
                AQ("a.p32ID IN (SELECT p32ID FROM p62ActivityCluster_Item WHERE p61ID=@p61id)", "p61id", this.p61id);
            }
            if (this.ismoneyinput != null)
            {
                if (this.ismoneyinput==true)
                {
                    AQ("p34.p33ID IN (2,5)", null, null);
                }
                else
                {
                    AQ("p34.p33ID NOT IN (2,5)", null, null);
                }
            }
            if (this.isbillable != null)
            {
                AQ("a.p32IsBillable=@billable)", "billable", this.isbillable);
            }

            return this.InhaleRows();

        }
    }
}
