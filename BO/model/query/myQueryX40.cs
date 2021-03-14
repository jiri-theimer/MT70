using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryX40: baseQuery
    {
        public int j02id { get; set; }
        
        public myQueryX40()
        {
            this.Prefix = "x40";
        }

        public override List<QRow> GetRows()
        {
            
            if (this.j02id > 0)
            {
                AQ("a.j03ID_Sys IN (select j03ID FROM j03User WHERE j02ID=@j02id)", "j02id", this.j02id);
            }
        
            return this.InhaleRows();

        }
    }
}
