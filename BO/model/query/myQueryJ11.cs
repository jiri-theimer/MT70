using System;
using System.Collections.Generic;


namespace BO
{
    public class myQueryJ11:baseQuery
    {
        public int j02id { get; set; }
        public bool? notall { get; set; }

        public myQueryJ11()
        {
            this.Prefix = "j11";
        }

        public override List<QRow> GetRows()
        {
            if (this.j02id > 0)
            {
                AQ("a.j11ID IN (select j11ID FROM j12Team_Person where j02ID=@j02id)", "j02id", this.j02id);
            }
            if (this.notall == true)
            {
                AQ("a.j11IsAllPersons=0", null, null);
            }

            return this.InhaleRows();

        }
    }
}
