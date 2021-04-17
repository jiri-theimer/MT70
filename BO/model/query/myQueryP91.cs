using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class myQueryP91:baseQuery
    {
        public int p28id { get; set; }
        public int p41id { get; set; }
        public int p56id { get; set; }
        public int j02id { get; set; }
        public int p92id { get; set; }
        public int p93id { get; set; }
        public int b02id { get; set; }
        public int j27id { get; set; }
        public int o38id { get; set; }

        public myQueryP91()
        {
            this.Prefix = "p91";
        }

        public override List<QRow> GetRows()
        {

            if (this.b02id > 0)
            {
                AQ("a.b02ID=@b02id", "b02id", this.b02id);
            }
            if (this.p28id > 0)
            {
                AQ("a.p28ID=@p28id", "p28id", this.p28id);
            }
            if (this.p41id > 0)
            {
                AQ("a.p91ID IN (SELECT p91ID FROM p31Worksheet WHERE p91ID IS NOT NULL AND p41ID=@p41id)", "p41id", this.p41id);
            }
            if (this.p56id > 0)
            {
                AQ("a.p91ID IN (SELECT p91ID FROM p31Worksheet WHERE p91ID IS NOT NULL AND p56ID=@p56id)", "p56id", this.p56id);
            }
            if (this.j27id > 0)
            {
                AQ("a.j27ID=@j27id)", "j27id", this.j27id);
            }
            if (this.p93id > 0)
            {
                AQ("a.p92ID IN (SELECT p92ID FROM p92InvoiceType WHERE p93ID=@p93id)", "p93id", this.p93id);
            }
            if (this.j02id > 0)
            {
                AQ("a.p91ID IN (SELECT p91ID FROM p31Worksheet WHERE p91ID IS NOT NULL AND j02ID=@j02id)", "j02id", this.j02id);
            }
            if (this.o38id > 0)
            {
                AQ("(a.o38ID_Primary=@o38id OR a.o38ID_Delivery=@o38id)", "o38id", this.o38id);
            }

            if (_searchstring != null && _searchstring.Length > 2)
            {
                AQ("(a.p91Code Like '%'+@expr+'%' OR a.p91Text1 LIKE '%'+@expr+'%' OR a.p91Client_RegID LIKE '%'+@expr+'%' OR a.p91Client_VatID LIKE @expr+'%' OR a.p41ID_First IN (select xa.p41ID FROM p41Project xa LEFT OUTER JOIN p28Contact xb ON xa.p28ID_Client=xb.p28ID WHERE xa.p41Name LIKE '%'+@expr+'%' OR xa.p41Code LIKE '%'+@expr+'%' OR xb.p28Name LIKE '%'+@expr+'%') OR a.p91Client LIKE '%'+@expr+'%' OR a.p28ID IN (select p28ID FROM p28Contact WHERE p28CompanyShortName LIKE '%'+@expr+'%' OR p28Name LIKE '%'+@expr+'%'))", "expr", _searchstring);

            }

            return this.InhaleRows();

        }
    }
}
