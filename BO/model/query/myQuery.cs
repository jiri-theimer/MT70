using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQuery:baseQuery
    {
        
        
        
        public myQuery(string prefix)
        {
            this.Prefix = prefix.Substring(0,3);
        }

       

        public override List<QRow> GetRows()
        {
         
         
            
            

            return this.InhaleRows();

        }
    }
}
