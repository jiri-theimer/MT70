using System.Collections.Generic;


namespace BO
{
    public class myQueryP30:baseQuery
    {
        public int p28id { get; set; }
        public int p41id { get; set; }
        public int j02id { get; set; }
        public myQueryP30()
        {
            this.Prefix = "p30";
        }

        public override List<QRow> GetRows()
        {

            if (this.p28id > 0)
            {
                AQ("a.p28ID=@p28id", "p28id", this.p28id);
            }
            if (this.p41id > 0)
            {
                AQ("a.p41ID=@p41id", "p41id", this.p41id);
            }
            if (this.j02id > 0)
            {
                AQ("a.j02ID=@j02id", "j02id", this.j02id);
            }

            return this.InhaleRows();

        }
    }
}
