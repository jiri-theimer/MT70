using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.bas
{
    public class pohoda_faktura_Setting
    {
        public string Implementation { get; set; }
        public string GlobalICO { get; set; }
        public bool IsProjects { get; set; }
        public bool isCentersCode { get; set; }
        public bool isCentersName { get; set; }
        public bool PenezniUkony11 { get; set; }
    }
    public class pohoda_faktura_Support
    {
        private System.Data.DataRow _dbrow { get; set; }
        private BL.Factory _f;
        private pohoda_faktura_Setting _setting { get; set; }

        public string Generate(List<int> p91ids, BL.Factory f, DL.DbHandler _db, pohoda_faktura_Setting setting)
        {
            _f = f;
            _setting = setting;

            string strGUID = "MT" + BO.BAS.ObjectDateTime2String(DateTime.Now, "dd-MM-yyyy-HH-mm-ss-fff");
            string strFileName = "pohoda-" + _f.CurrentUser.j03Login + "-" + strGUID + ".xml";
            string strTempPath = _f.x35GlobalParamBL.TempFolder() + "\\" + strFileName;

            var c = new BO.CLS.XmlSupport(strTempPath, "WINDOWS-1250");
            Handle_DataPack_Header(c, strGUID, _setting.GlobalICO);


            var lisP91 = _f.p91InvoiceBL.GetList(new BO.myQueryP91() { pids = p91ids });
            int x = 1;
            foreach (var rec in lisP91)
            {
                Handle_DataPackItem(c, _db, rec, strGUID + BO.BAS.RightString("0000" + x.ToString(), 4));
                x += 1;
            }
            c.flushandclose();

            return strFileName;
        }

        private void Handle_DataPack_Header(BO.CLS.XmlSupport c, string strGUID, string ico)
        {
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
        }

        private void Handle_DataPackItem(BO.CLS.XmlSupport c, DL.DbHandler _db, BO.p91Invoice rec, string strPackItemID)
        {
            bool bolForeignInvoice = false; double dblExchangeRate = 0;
            System.Data.DataTable dt = getDT(rec.pid, _db);
            _dbrow = dt.Rows[0];
            if (IN("j27ID_Domestic") != IN("j27ID"))
            {
                bolForeignInvoice = true; dblExchangeRate = Convert.ToDouble(_dbrow["p91ExchangeRate"]);

            }
            else
            {
                bolForeignInvoice = false; dblExchangeRate = 1;


            }

            c.wstart("dataPackItem"); c.oneattribute("version", "2.0"); c.oneattribute("id", strPackItemID);
            c.wstart("invoice", "http://www.stormware.cz/schema/version_2/invoice.xsd"); c.oneattribute("version", "2.0");
            c.wstart("invoiceHeader");
            if (IN("p92InvoiceType") == "2")
            {
                c.wss("invoiceType", "issuedCorrectiveTax");
            }
            else
            {
                c.wss("invoiceType", "issuedInvoice");
            }
            c.wstart("number"); c.wss("numberRequested", IN("p91Code")); c.wend();
            c.wss("symVar", IN("p91Code"));

            c.wsdate("date", Convert.ToDateTime(_dbrow["p91Date"]));
            c.wsdate("dateTax", Convert.ToDateTime(_dbrow["p91DateSupply"]));
            c.wsdate("dateAccounting", Convert.ToDateTime(_dbrow["p91DateSupply"]));
            if (_dbrow["p91DateMaturity"] != null)
            {
                c.wsdate("dateDue", Convert.ToDateTime(_dbrow["p91DateMaturity"]));
            }
            if (!string.IsNullOrEmpty(IN("p92AccountingIDS")))
            {
                c.wstart("accounting");c.wss("ids", IN("p92AccountingIDS"));c.wend();
            }
            if (!string.IsNullOrEmpty(IN("p92ClassificationVATIDS")))
            {
                c.wstart("classificationVAT"); c.wss("ids", IN("p92ClassificationVATIDS")); c.wend();
            }

            c.wss("text", IN("p91Text1", 240));

            c.wstart("partnerIdentity");
            c.wstart("address");
            c.wss("company", IN("p91Client"));
            c.wss("city", IN("p91ClientAddress1_City", 35));
            c.wss("street", IN("p91ClientAddress1_Street", 64));
            c.wss("zip", IN("p91ClientAddress1_ZIP", 15));
            c.wss("ico", IN("p91Client_RegID"));
            c.wss("dic", IN("p91Client_VatID"));
            c.wend();   //address
            c.wend();   //partnerIdentity

            c.wstart("myIdentity");
            c.wstart("address");
            c.wss("company", IN("p93Company"));
            c.wss("city", IN("p93City", 35));
            c.wss("street", IN("p93Street", 64));
            c.wss("zip", IN("p93Zip", 15));
            c.wss("ico", IN("p93RegID"));
            c.wss("dic", IN("p93VatID"));
            c.wss("surname", IN("p93Referent"));

            c.wend();   //address
            c.wend();   //myIdentity

            c.wstart("account");
            c.wss("accountNo", IN("p86BankAccount"));
            c.wss("bankCode", IN("p86BankCode"));
            c.wend();   //account

            c.wss("symConst", "0308");
            
            if (_setting.IsProjects && !string.IsNullOrEmpty(IN("contract")))
            {
                c.wstart("contract");c.wss("ids", IN("contract")); c.wend();    //projekt
            }
            if ((_setting.isCentersCode || _setting.isCentersName) && _dbrow["p41ID_First"] != System.DBNull.Value && _dbrow["p41ID_First"] != null)
            {
                var recP41 = _f.p41ProjectBL.Load(Convert.ToInt32(_dbrow["p41ID_First"]));
                var recJ18 = _f.j18RegionBL.Load(recP41.j18ID);
                c.wstart("centre");
                if (_setting.isCentersCode) c.wss("ids", recJ18.j18Code);
                if (_setting.isCentersName) c.wss("ids", recJ18.j18Name);
                c.wend();    //centre

            }
            c.wsbool("markRecord", false);

            c.wend();   //invoiceHeader

            Handle_InvoiceItems(rec, c, bolForeignInvoice, dblExchangeRate);

            c.wstart("invoiceSummary");
            if (bolForeignInvoice)
            {
                c.wstart("foreignCurrency");
                c.wstart("currency");c.wss("ids", IN("j27Code"));c.wend();
                c.wsnum("rate", dblExchangeRate);
                c.wss("amount", "1");
                c.wsnum("priceSum", NUM("p91Amount_TotalDue"));
                c.wend();   //foreignCurrency
            }
            c.wend();   //invoiceSummary

        }




        private void Handle_InvoiceItems(BO.p91Invoice recP91, BO.CLS.XmlSupport c, bool bolForeignInvoice, double dblExchangeRate)
        {

            c.wstart("invoiceDetail");

            var lis = _f.p91InvoiceBL.GetList_CenovyRozpis(recP91.pid, false, false, 0);
            foreach (var rec in lis)
            {
                string strVatType = "high";
                if (rec.DPHSazba < 20) strVatType = "low";
                if (rec.DPHSazba == 0) strVatType = "none";

                c.wstart("invoiceItem");

                c.wss("text", rec.Oddil.Substring(0, 89));
                c.wss("quantity", "1");
                c.wss("unit", "ks");
                c.wss("coefficient", "1");
                c.wsbool("payVAT", false);
                c.wss("rateVAT", strVatType);
                c.wss("discountPercentage", "0");

                c.wstart("homeCurrency");
                if (!bolForeignInvoice)
                {
                    c.wsnum("unitPrice", rec.BezDPH);
                    c.wsnum("price", rec.BezDPH);
                    c.wsnum("priceVAT", rec.DPH);
                    c.wsnum("priceSum", rec.VcDPH);
                }
                else
                {
                    c.wsnum("unitPrice", rec.BezDPH* dblExchangeRate);
                    c.wsnum("price", rec.BezDPH* dblExchangeRate);
                    c.wsnum("priceVAT", rec.DPH* dblExchangeRate);
                    c.wsnum("priceSum", rec.VcDPH* dblExchangeRate);
                }
                c.wend();  //homeCurrency
                if (bolForeignInvoice)
                {
                    c.wstart("foreignCurrency");
                    c.wsnum("unitPrice", rec.BezDPH);
                    c.wsnum("price", rec.BezDPH);
                    c.wsnum("priceVAT", rec.DPH);
                    c.wsnum("priceSum", rec.VcDPH);
                    c.wend();   //foreignCurrency

                }
                c.wsbool("PDP", false);

                Handle_InvoiceItem_Predkontace(recP91, c, rec);

                c.wend();   //invoiceItem

            }

            c.wend();   //invoiceDetail

        }

        private void Handle_InvoiceItem_Predkontace(BO.p91Invoice recP91, BO.CLS.XmlSupport c,BO.p91_CenovyRozpis recRozpis)
        {
            string strAccountingIDS = null;string strActivityIDS = null;
            if (recRozpis.p31ID > 0)
            {
                var recP31 = _f.p31WorksheetBL.Load(recRozpis.p31ID);
                var recP32 = _f.p32ActivityBL.Load(recP31.p32ID);
                strAccountingIDS = recP32.p32AccountingIDS;
                strActivityIDS = recP32.p32ActivityIDS;

                if (_setting.IsProjects || (_setting.Implementation=="zch" && _setting.PenezniUkony11))
                {
                    var recP41 = _f.p41ProjectBL.Load(recP31.p41ID);
                    c.wstart("contract");
                    if (_setting.Implementation == "zch")
                    {
                        if (recP31.p31Code != null)
                        {
                            c.wss("ids", recP31.p31Code);   //do pohoda zakázky exportovat kód dokladu - zch specifikum
                        }
                    }
                    else
                    {
                        c.wss("ids", recP41.p41Code);
                    }
                    c.wend();   //contract

                    if ((_setting.isCentersCode || _setting.isCentersName) && recP41.j18ID>0)
                    {
                        var recJ18 = _f.j18RegionBL.Load(recP41.j18ID);
                        
                        if (_setting.Implementation == "zch")
                        {
                            //v ZCH se středisko projektu exportuje do činnosti -> prasárna největší
                            c.wstart("activity");                           
                            strAccountingIDS = null;
                        }
                        else
                        {
                            c.wstart("centre");                                                        
                        }
                        if (_setting.isCentersCode) c.wss("ids", recJ18.j18Code);
                        if (_setting.isCentersName) c.wss("ids", recJ18.j18Name);
                        c.wend();
                    }
                }
                if (!string.IsNullOrEmpty(recP31.p31Code))
                {
                    c.wss("code", recP31.p31Code);
                }
            }


        }
        private string IN(string fieldname, int maxlength = 0)
        {
            if (_dbrow[fieldname] == System.DBNull.Value || _dbrow[fieldname] == null)
            {
                return "";
            }
            if (maxlength == 0)
            {
                return _dbrow[fieldname].ToString();
            }
            else
            {
                return _dbrow[fieldname].ToString().Substring(0, maxlength - 1);
            }

        }
        private double NUM(string fieldname)
        {
            if (_dbrow[fieldname] == System.DBNull.Value || _dbrow[fieldname] == null)
            {
                return 0;
            }
            return Convert.ToDouble(_dbrow[fieldname]);
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
