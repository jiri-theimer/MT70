using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryJ02:baseQuery
    {
        public bool? j02isintraperson { get; set; }
        public int j04id { get; set; }
        public int j11id { get; set; }
        public int j07id { get; set; }
        public int j18id { get; set; }
        public int p28id { get; set; }
        public int p41id { get; set; }
        public int p91id { get; set; }
        

        public myQueryJ02()
        {
            this.Prefix = "j02";
        }

        public override List<QRow> GetRows()
        {
            if (this.j02isintraperson != null)
            {
                AQ("a.j02IsIntraPerson=@isintra", "isintra", this.j02isintraperson);
            }
            if (this.j04id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM j03User WHERE j04ID=@j04id)", "j04id", this.j04id);
            }            
            if (this.j11id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM j12Team_Person WHERE j11ID=@j11id)", "j11id", this.j11id);
            }
            if (this.j07id > 0)
            {
                AQ("a.j07ID=@j07id", "j07id", this.j07id);
            }
            if (this.j18id > 0)
            {
                AQ("a.j18ID=@j18id", "j18id", this.j18id);
            }
            if (this.p41id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM p30Contact_Person WHERE p41ID=@p41id OR p28ID IN (SELECT p28ID_Client FROM p41Project WHERE p41ID=@p41id AND p28ID_Client IS NOT NULL))", "p41id", this.p41id);
            }
            if (this.p28id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM p30Contact_Person WHERE p28ID=@p28id OR p41ID IN (SELECT p41ID FROM p41Project WHERE p28ID_Client=@p28id))", "p28id", this.p28id);
            }
            if (this.p91id > 0)
            {
                AQ("a.j02ID IN (select a1.j02ID FROM p30Contact_Person a1 INNER JOIN p91Invoice a2 ON a1.p28ID=a2.p28ID WHERE a2.p91ID=@p91id)", "p91id", this.p91id);
            }
           

            if (_searchstring != null && _searchstring.Length > 2)
            {                
                AQ("(a.j02firstname like @expr+'%' OR a.j02LastName LIKE '%'+@expr+'%' OR a.j02Email LIKE '%'+@expr+'%' OR a.j02ID IN (select j02ID FROM j03User where j03Login LIKE '%'+@expr+'%'))", "expr", _searchstring);

            }

            return this.InhaleRows();

        }
    }
}
