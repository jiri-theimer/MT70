
using System.Collections.Generic;

namespace BO
{
    public class myQueryJ90:baseQuery
    {
        public int j02id { get; set; }
        public int j03id { get; set; }


        public myQueryJ90()
        {
            this.Prefix = "j90";
        }

        public override List<QRow> GetRows()
        {

            if (this.j02id > 0)
            {
                AQ("a.j03ID IN (select j03ID FROM j03User WHERE j02ID=@j02id)", "j02id", this.j02id);
            }
            if (this.j03id > 0)
            {
                AQ("a.j03ID=@j03id)", "j03id", this.j03id);
            }



            return this.InhaleRows();

        }
    }
}
