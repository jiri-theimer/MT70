using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.bas
{
    public class pohoda_faktura_Support
    {
        private System.Data.DataRow _dbrow { get; set; }

        public string GenerateOne(int p91id, BL.Factory _f, DL.DbHandler _db,string ico)
        {
            System.Data.DataTable dt = getDT(p91id, _db);
            _dbrow = dt.Rows[0];
            bool bolForeignInvoice = false; double dblExchangeRate = 0;
            string strGUID = "MT"+BO.BAS.ObjectDateTime2String(DateTime.Now, "dd-MM-yyyy-HH-mm-ss-fff");
            string strFileName = "pohoda-" + _f.CurrentUser.j03Login + "-" + strGUID + ".xml";
            string strTempPath = _f.x35GlobalParamBL.TempFolder() + "\\" + strFileName;

            var c = new BO.CLS.XmlSupport(strTempPath, "WINDOWS-1250");
            c.wstart("dataPack", "http://www.stormware.cz/schema/version_2/data.xsd");
            c.oneattribute("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
            c.oneattribute("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            c.oneattribute("version", "2.0");
            c.oneattribute("id", strGUID);
            c.oneattribute("ico", ico);
            c.oneattribute("key", BO.BAS.GetGuid());
            c.oneattribute("programVersion", "6x");
            c.oneattribute("application", "MARKTIME");
            c.oneattribute("note", "Export MARKTIME vyúčtování");


            c.flushandclose();

            return strFileName;
        }



        private System.Data.DataTable getDT(int p91id, DL.DbHandler _db)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("select p91.*,p93.*,'' as stredisko,p41.projekt as contract,j27.j27Code,p28.*,o38prim.*,p86.*,j27dom.j27Code as j27Code_Domestic,p92.p92AccountingIDS,p92.p92ClassificationVATIDS,p92.p92InvoiceType");
            sb.Append(" from p91invoice p91");
            sb.Append(" left outer join p28Contact p28 on p91.p28id=p28.p28id");
            sb.Append(" LEFT OUTER JOIN p92InvoiceType p92 on p91.p92id=p92.p92id");
            sb.Append(" left outer join p93InvoiceHeader p93 on p92.p93id=p93.p93id");
            sb.Append(" LEFT OUTER JOIN o38Address o38prim ON p91.o38ID_Primary=o38prim.o38ID");
            sb.Append($" LEFT OUTER JOIN (select {p91id} as p91id,* FROM p86BankAccount WHERE p86ID=dbo.p91_get_p86id({p91id})) p86 ON p91.p91ID=p86.p91ID");
            sb.Append($" left outer join (select min(p41Code) as projekt,min(a.p91ID) as p91ID FROM p31worksheet a INNER JOIN p41Project b ON a.p41ID=b.p41ID WHERE a.p91ID={p91id}) p41 ON p91.p91ID=p41.p91ID");
            sb.Append(" left outer join j27Currency j27 ON p91.j27ID=j27.j27ID");
            sb.Append(" LEFT OUTER JOIN j27Currency j27dom ON p91.j27ID_Domestic=j27dom.j27ID");
            sb.Append($" where p91.p91id = {p91id}");

            return _db.GetDataTable(sb.ToString());

        }
    }
}
