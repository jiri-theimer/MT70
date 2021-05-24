using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQuery:baseQuery
    {

        public int x29id { get; set; }

        public myQuery(string prefix)
        {
            this.Prefix = prefix.Substring(0,3);
            
        }

       

        public override List<QRow> GetRows()
        {

            if (this.x29id > 0)
            {
                AQ("a.x29ID=@x29id", "x29id", this.x29id);
            }

            if (this.Prefix == "x67")
            {
                AQ("a.x67ParentID IS NULL", null, null);
            }

            return this.InhaleRows();

        }

        
    }
}
