using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.bas
{
    public class IsdocSupport
    {
        private System.Data.DataRow _dbrow { get; set; }
        public string GenerateOne(int p91id, BL.Factory _f, DL.DbHandler _db)
        {
            _db.RunSql("UPDATE p91Invoice set p91Guid=NEWID() WHERE p91Guid IS NULL AND p91ID=@pid", new { pid = p91id });
            System.Data.DataTable dt = getDT(p91id, _db);
            _dbrow = dt.Rows[0];
            bool bolForeignInvoice = false; double dblExchangeRate = 0; string strFileName = _dbrow["p91Code"] + ".ISDOC";


            var c = new BO.CLS.XmlSupport(_f.x35GlobalParamBL.TempFolder() + "\\" + strFileName);
            c.wstart("Invoice", "http://isdoc.cz/namespace/2013");
            c.wss("DocumentType", "1");
            c.wss("ID", IN("p91Code"));
            c.wss("UUID", IN("p91Guid"));
            c.wsbool("EgovFlag", false);
            c.wss("IssuingSystem", "MARKTIME 7.0");
            c.wsdate("IssueDate", Convert.ToDateTime(_dbrow["p91Date"]));
            c.wsdate("TaxPointDate", Convert.ToDateTime(_dbrow["p91DateSupply"]));
            c.wsbool("VATApplicable", true);
            c.wss("ElectronicPossibilityAgreementReference", "");
            if (_dbrow["p91Text1"] != System.DBNull.Value)
            {
                c.wss("Note", IN("p91Text1"));
            }

            c.wss("LocalCurrencyCode", IN("j27Code_Domestic"));
            if (_dbrow["j27ID_Domestic"] == _dbrow["j27ID"])
            {
                c.wsnum("CurrRate", 1);
            }
            else
            {
                bolForeignInvoice = true;
                dblExchangeRate = Convert.ToDouble(_dbrow["p91ExchangeRate"]);
                c.wss("ForeignCurrencyCode", IN("j27Code"));
                c.wsnum("CurrRate", NUM("p91ExchangeRate"));
            }
            c.wss("RefCurrRate", "1");

            handle_supplier_or_seller("AccountingSupplierParty", c);
            handle_supplier_or_seller("SellerSupplierParty", c);

            var recP28 = _f.p28ContactBL.Load(Convert.ToInt32(_dbrow["p28ID"]));
            handle_customer_or_buyer("AccountingCustomerParty", c, recP28);
            handle_customer_or_buyer("BuyerCustomerParty", c, recP28);

            Handle_InvoiceLines(p91id, _f, c, bolForeignInvoice, dblExchangeRate);

            Handle_TaxTotal(c, bolForeignInvoice, dblExchangeRate);


            c.flushandclose();

            return strFileName;
        }

        private void Handle_TaxTotal(BO.CLS.XmlSupport c, bool bolForeignInvoice, double dblExchangeRate)
        {
            c.wstart("TaxTotal");

            c.wstart("TaxSubTotal");
            c.wsnum("TaxableAmount", NUM("p91Amount_WithoutVat_Standard"));
            
            c.wsnum("TaxAmount", NUM("p91Amount_Vat_Standard"));
            c.wsnum("TaxInclusiveAmount", NUM("p91Amount_WithVat_Standard"));

            

            if (bolForeignInvoice)
            {
                c.wsnum("AlreadyClaimedTaxableAmountCurr", 0);  //na záloze již upl., základ v sazbě v C.M.
                c.wsnum("AlreadyClaimedTaxAmountCurr", 0);  //na záloze již upl., základ v sazbě v C.M.                
                c.wsnum("AlreadyClaimedTaxInclusiveAmountCurr", 0);   //na záloze již upl., s daní v sazbě v C.M.
                


            }
            
            c.wsnum("AlreadyClaimedTaxableAmount", NUM("p91ProformaAmount_WithoutVat_Standard"));  //na záloze již uplatněno, základ v sazbě v T.M.
            c.wsnum("AlreadyClaimedTaxAmount", NUM("p91ProformaAmount_Vat_Standard"));
            c.wsnum("AlreadyClaimedTaxInclusiveAmount", NUM("p91ProformaAmount_WithoutVat_Standard")+NUM("p91ProformaAmount_Vat_Standard"));

            c.wsnum("DifferenceTaxableAmount", NUM("p91Amount_WithoutVat_Standard"));
            c.wsnum("DifferenceTaxAmount", NUM("p91Amount_Vat_Standard"));
            c.wsnum("DifferenceTaxInclusiveAmount", NUM("p91Amount_WithVat_Standard"));
            c.wstart("TaxCategory");
            c.wsnum("Percent", NUM("p91VatRate_Standard"));
            c.wsbool("VATApplicable", true);
            c.wsbool("LocalReverseChargeFlag", true);
            c.wend();   //TaxCategory

        }



        private void Handle_InvoiceLines(int p91id, BL.Factory _f, BO.CLS.XmlSupport c, bool bolForeignInvoice, double dblExchangeRate)
        {
            c.wstart("InvoiceLines");

            var lis = _f.p91InvoiceBL.GetList_CenovyRozpis(p91id, false, false, 0);
            foreach (var rec in lis)
            {
                c.wstart("InvoiceLine");
                c.wss("ID", rec.RowPID.ToString());
                c.wss("InvoicedQuantity", "1");

                c.wsnum("LineExtensionAmountBeforeDiscount", 0);

                c.wsnum("LineExtensionAmountTaxInclusiveBeforeDiscount", 0);
                c.wsnum("LineExtensionTaxAmount", rec.DPH);


                if (bolForeignInvoice)
                {
                    c.wsnum("UnitPrice", rec.BezDPH * dblExchangeRate);
                    c.wsnum("UnitPriceTaxInclusive", rec.VcDPH * dblExchangeRate);

                    c.wsnum("LineExtensionAmountCurr", rec.BezDPH);
                    c.wsnum("LineExtensionAmount", rec.BezDPH * dblExchangeRate);
                    c.wsnum("LineExtensionAmountTaxInclusiveCurr", rec.VcDPH * dblExchangeRate);
                    c.wsnum("LineExtensionAmountTaxInclusive", rec.VcDPH * dblExchangeRate);
                }
                else
                {
                    c.wsnum("UnitPrice", rec.BezDPH);
                    c.wsnum("UnitPriceTaxInclusive", rec.VcDPH);

                    c.wsnum("LineExtensionAmount", rec.BezDPH);
                    c.wsnum("LineExtensionAmountTaxInclusive", rec.VcDPH);
                }

                c.wstart("ClassifiedTaxCategory");
                c.wss("Percent", rec.DPHSazba.ToString());
                c.wss("VATCalculationMethod", "0");
                c.wsbool("VATApplicable", true);
                c.wend();

                c.wstart("Item");c.wss("Description", rec.Oddil);c.wend();

                c.wend();   //InvoiceLine


            }

            c.wend();   //InvoiceLines
        }


        private void handle_supplier_or_seller(string partyelementname, BO.CLS.XmlSupport c)
        {
            c.wstart(partyelementname);
            c.wstart("Party");
            c.wstart("PartyIdentification");
            c.wss("UserID", IN("p93ID"));
            c.wss("ID", IN("p93RegID"));
            c.wend();   //PartyIdentification
            c.wstart("PartyName"); c.wss("Name", IN("p93Company")); c.wend();

            c.wstart("PostalAddress");
            c.wss("StreetName", IN("p93Street"));
            c.wss("CityName", IN("p93City"));
            c.wss("PostalZone", IN("p93Zip"));
            c.wstart("Country");
            c.wss("IdentificationCode", IN("p93CountryCode"));
            if (_dbrow["p93Country"] == System.DBNull.Value)
            {
                c.wss("Name", "Česká republika");
            }
            else
            {
                c.wss("Name", IN("p93Country"));
            }
            c.wend();   //Country
            c.wend();   //PostalAddress
            c.wstart("PartyTaxScheme");
            c.wss("CompanyID", IN("p93VatID"));
            c.wss("TaxScheme", "VAT");
            c.wend();   //PartyTaxScheme
            c.wstart("Contact");
            c.wss("Name", IN("p93Referent"));
            c.wss("Telephone", IN("p93Contact"));
            c.wss("ElectronicMail", IN("p93Email"));
            c.wend();   //Contact

            c.wend();    //Party
            c.wend();   //AccountingSupplierParty
        }
        private void handle_customer_or_buyer(string partyelementname, BO.CLS.XmlSupport c, BO.p28Contact recP28)
        {
            c.wstart(partyelementname);
            c.wstart("Party");
            c.wstart("PartyIdentification");
            c.wss("UserID", IN("p28ID"));
            if (_dbrow["p91Client_RegID"] == System.DBNull.Value)
            {
                c.wss("ID", IN("p91Client_VatID"));
            }
            else
            {
                c.wss("ID", IN("p91Client_RegID"));
            }

            c.wend();   //PartyIdentification
            c.wstart("PartyName"); c.wss("Name", IN("p91Client")); c.wend();

            c.wstart("PostalAddress");
            c.wss("StreetName", IN("p91ClientAddress1_Street"));
            c.wss("BuildingNumber", "");
            c.wss("CityName", IN("p91ClientAddress1_City"));
            c.wss("PostalZone", IN("p91ClientAddress1_ZIP"));
            c.wstart("Country");
            if (!string.IsNullOrEmpty(recP28.p28Pohoda_VatCode))
            {
                c.wss("IdentificationCode", recP28.p28Pohoda_VatCode);
            }
            else
            {
                c.wss("IdentificationCode", "CZ");
            }

            c.wss("Name", IN("p91ClientAddress1_Country"));

            c.wend();   //Country
            c.wend();   //PostalAddress
            c.wstart("PartyTaxScheme");
            if (_dbrow["p91Client_ICDPH_SK"] != System.DBNull.Value)
            {
                c.wss("CompanyID", IN("p91Client_ICDPH_SK"));
                c.wss("TaxScheme", "VAT");
            }
            else
            {
                c.wss("CompanyID", IN("p91Client_VatID"));
                c.wss("TaxScheme", "VAT");
            }

            c.wend();   //PartyTaxScheme            

            c.wend();    //Party
            c.wend();   //AccountingSupplierParty
        }


        private string IN(string fieldname)
        {
            if (_dbrow[fieldname] == System.DBNull.Value)
            {
                return "";
            }
            return _dbrow[fieldname].ToString();
        }
        private double NUM(string fieldname)
        {
            if (_dbrow[fieldname] == System.DBNull.Value)
            {
                return 0;
            }
            return Convert.ToDouble(_dbrow[fieldname]);
        }


        private System.Data.DataTable getDT(int p91id, DL.DbHandler _db)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("select p91.*,p93.*,'' as stredisko,p41.projekt as contract,j27.j27Code,p28.*,o38prim.*,p86.*,j27dom.j27Code as j27Code_Domestic");
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
