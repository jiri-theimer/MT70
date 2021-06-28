using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Xml;

namespace XA.Controllers
{
    public class SepaController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly BL.RunningApp _app;
        public SepaController(BL.RunningApp app,IWebHostEnvironment env)
        {
            _app = app;
            _env = env;
        }
        private XmlWriter _wr { get; set; }
        public IActionResult Index()
        {
            var v = new XA.Models.sepa.SepaViewModel() { 
                Guid = BO.BAS.GetGuid()
                , Vystavovatel = "SCHAFFER PARTNER"
                , VystavovatelMesto = "Praha 1"
                , VystavovatelUlice = "Vodičkova 710/31"
                ,PrijemceIBAN= "DE30760200700003660788"
                ,PrijemceBIC= "HYVEDEMM460"
                ,PrijemceCID= "CZ58ZZZ10272"                
                ,DatumSplatnosti= DateTime.Today.AddDays(10)
            };

            


            return View(v);
        }

        [HttpPost]
        public IActionResult Index(XA.Models.sepa.SepaViewModel v)
        {
            Generovat(v);
            return View(v);
        }

        public ActionResult Download(string guid)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(_app.LogFolder + "\\" + guid+".xml");
            Response.Headers["Content-Type"] = "text/xml";
            Response.Headers["Content-Length"] = fileBytes.Length.ToString();
            return File(fileBytes, "text/xml", "sepa.xml");
        }

        private void Generovat(XA.Models.sepa.SepaViewModel v)
        {
            string strClientID = "1298";
            DateTime datPodepsaniMandatu = DateTime.Today.AddDays(-20);
            string strClientBankaPlatceBIC = "GENODED1MRB";
            string strClientJmenoPlatce = "BARI Mönchengladbach OHG";
            string strClientIBAN = "DE29310605177202767014";
            string strFakturaVS = "1520211269";
            double decFakturaCastka = 8949.50;
            double decSoucetCastek = 8949.50;
            int intPocetInkas = 1;

            XmlWriterSettings settings = new XmlWriterSettings() { CloseOutput = true, Indent = true, Encoding = System.Text.Encoding.UTF8 };


            //_wr = XmlWriter.Create(@"c:\temp\sepa10.xml", settings);
            string strPath = _app.LogFolder + "\\" + v.Guid + ".xml";
            _wr = XmlWriter.Create(strPath, settings);

            _wr.WriteStartElement("Document", "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02");
            wstart("CstmrDrctDbtInitn");
            wstart("GrpHdr");
            wss("MsgId", v.Guid);
            wsdatetime("CreDtTm", DateTime.Now);
            wsint("NbOfTxs", 1);
            wsnum("CtrlSum", decSoucetCastek);
            wstart("InitgPty");
            wss("Nm", v.Vystavovatel);
            wend(); //InitgPty
            wend(); //GrpHdr

            wstart("PmtInf");
            wss("PmtInfId", v.Guid);
            wss("PmtMtd", "DD");
            wsbool("BtchBookg", true);
            wsint("NbOfTxs", intPocetInkas);
            wsnum("CtrlSum", decSoucetCastek);

            wstart("PmtTpInf");
            wstart("SvcLvl"); wss("Cd", "SEPA"); wend();
            wstart("LclInstrm"); wss("Cd", "CORE"); wend();
            wss("SeqTp", "RCUR");
            wend(); //PmtTpInf

            wsdate("ReqdColltnDt", v.DatumSplatnosti);
            wstart("Cdtr"); wss("Nm", v.Vystavovatel); wend();
            wstart("CdtrAcct"); wstart("Id"); wss("IBAN", strClientIBAN); wend(); wend(); //CdtrAcct
            wstart("CdtrAgt"); wstart("FinInstnId"); wss("BIC", v.PrijemceBIC); wend(); wend();//CdtrAgt
            wss("ChrgBr", "SLEV");


            wstart("DrctDbtTxInf");
            wstart("PmtId"); wss("EndToEndId", "NOTPROVIDED"); wend();

            wstart("InstdAmt");
            oneattribute("Ccy", "EUR");
            purestring(BO.BAS.GN(decFakturaCastka));
            wend(); //InstdAmt

            wstart("DrctDbtTx");
            wstart("MndtRltdInf"); wss("MndtId", strClientID); wsdate("DtOfSgntr", datPodepsaniMandatu); wend();

            wstart("CdtrSchmeId");
            wstart("Id");
            wstart("PrvtId");
            wstart("Othr"); wss("Id", v.PrijemceCID);
            wstart("SchmeNm"); wss("Prtry", "SEPA"); wend();
            wend(4); //CdtrSchmeId+Id+PrvtId+Othr

            wend(); //DrctDbtTx


            wstart("DbtrAgt");
            wstart("FinInstnId"); wss("BIC", strClientBankaPlatceBIC); wend();
            wend(); //DbtrAgt

            wstart("Dbtr"); wss("Nm", strClientJmenoPlatce); wend();
            wstart("DbtrAcct");
            wstart("Id"); wss("IBAN", strClientIBAN); wend();
            wend(); //DbtrAcct

            wstart("RmtInf"); wss("Ustrd", strFakturaVS); wend();


            wend(); //DrctDbtTxInf

            _wr.Flush();
            _wr.Close();

            v.GeneratedFileName = v.Guid + ".xml";


        }


        private void wstart(string strStartElementName)
        {
            _wr.WriteStartElement(strStartElementName);
        }
        private void wend(int krat = 1)
        {
            for (int i = 1; i <= krat; i++)
            {
                _wr.WriteEndElement();
            }

        }

        private void oneattribute(string strName,string strValue)
        {
            _wr.WriteAttributeString(strName, strValue);
        }
        private void purestring(string s)
        {
            _wr.WriteString(s);
        }
        private void wss(string strElement, string s)
        {
            _wr.WriteElementString(strElement, s);
        }
        
        private void wsdate(string strElement, DateTime d)
        {
            _wr.WriteElementString(strElement, DAT(d));
        }
        private void wsdatetime(string strElement, DateTime d)
        {
            _wr.WriteElementString(strElement, DATISO(d));
        }
        private void wsnum(string strElement, double n)
        {
            _wr.WriteElementString(strElement, NUM(n));
        }
        private void wsint(string strElement, int n)
        {
            _wr.WriteElementString(strElement, n.ToString());
        }
        private void wsbool(string strElement,bool b)
        {
            if (b)
            {
                _wr.WriteElementString(strElement,"true");
            }
            else
            {
                _wr.WriteElementString(strElement, "false");
            }
            
        }
        private string NUM(double n)
        {
            return BO.BAS.GN(n);
        }
        private string DAT(DateTime d)
        {
            return BO.BAS.ObjectDate2String(d, "yyyy-MM-dd");
        }
        private string DATISO(DateTime d)
        {
            return BO.BAS.ObjectDateTime2String(d, "yyyy-MM-ddTHH:mm:ssZ");
        }
    }
}
