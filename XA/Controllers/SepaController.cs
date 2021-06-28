using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using ClosedXML.Excel;
using System.Xml;
using Microsoft.AspNetCore.Http;
using System.IO;

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
        public IActionResult Index(IFormFile file4import, XA.Models.sepa.SepaViewModel v)
        {
           if (file4import !=null)
            {
                v.GuidImport = BO.BAS.GetGuid();
                v.ImportOrigFileName = file4import.FileName;
                string strTempFullPath = _app.LogFolder + "\\" + v.GuidImport + "_" + file4import.FileName;
                using (var stream = new FileStream(strTempFullPath, FileMode.Create))
                {
                    
                    file4import.CopyTo(stream);
                }
            }

            Handle_Import(v);
            if (v.lisRecords != null)
            {
                Handle_Generovat(v);
            }
            
            return View(v);
        }

        public ActionResult Download(string guid)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(_app.LogFolder + "\\" + guid+".xml");
            Response.Headers["Content-Type"] = "text/xml";
            Response.Headers["Content-Length"] = fileBytes.Length.ToString();
            return File(fileBytes, "text/xml", "sepa.xml");
        }

        private void Handle_Import(XA.Models.sepa.SepaViewModel v)
        {
            if (string.IsNullOrEmpty(v.GuidImport))
            {
                return;
            }
            string strTempFullPath = _app.LogFolder + "\\" + v.GuidImport + "_" + v.ImportOrigFileName;
            v.lisRecords = new List<XA.Models.sepa.XlsRecord>();

            using (var workbook = new XLWorkbook(strTempFullPath))
            {
                var worksheet = workbook.Worksheets.First();
                for (int row = 2; row < 10000; row++)
                {
                    if (worksheet.Cell(row, 1).Value != null && worksheet.Cell(row, 1).Value.ToString() != "")
                    {
                        var c = new XA.Models.sepa.XlsRecord();
                        c.ClientID = worksheet.Cell(row, 1).Value.ToString();
                        c.Jmeno = worksheet.Cell(row, 2).Value.ToString();                        
                        c.IBAN= worksheet.Cell(row, 3).Value.ToString();
                        c.BIC = worksheet.Cell(row, 4).Value.ToString();
                        if (worksheet.Cell(row, 5).Value != null && !string.IsNullOrEmpty(worksheet.Cell(row, 5).Value.ToString()))
                        {
                            c.PodepsaniMandatu = BO.BAS.String2Date(worksheet.Cell(row, 5).Value.ToString());
                        }
                        c.FakturaVS= worksheet.Cell(row, 6).Value.ToString();
                        c.FakturaCastka = Convert.ToDouble(worksheet.Cell(row, 7).Value);
                        if (worksheet.Cell(row, 8).Value !=null && worksheet.Cell(row, 8).Value.ToString() != "")
                        {
                            c.FakturaMena = worksheet.Cell(row, 8).Value.ToString();
                        }
                        else
                        {
                            c.FakturaMena = "EUR";
                        }
                        
                        v.lisRecords.Add(c);
                    }
                }
            }
        }
        private void Handle_Generovat(XA.Models.sepa.SepaViewModel v)
        {
            
            double decSoucetCastek = v.lisRecords.Sum(p=>p.FakturaCastka);
            int intPocetInkas = v.lisRecords.Count();            

            var c = new clsXmlSupport(_app.LogFolder + "\\" + v.Guid + ".xml");

            
            c.wstart("Document", "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02");
            
            c.wstart("CstmrDrctDbtInitn");
            c.wstart("GrpHdr");
            c.wss("MsgId", v.Guid);
            c.wsdatetime("CreDtTm", DateTime.Now);
            c.wsint("NbOfTxs", intPocetInkas);
            c.wsnum("CtrlSum", decSoucetCastek);
            c.wstart("InitgPty");
            c.wss("Nm", v.Vystavovatel);
            c.wend(); //InitgPty
            c.wend(); //GrpHdr

            c.wstart("PmtInf");
            c.wss("PmtInfId", v.Guid);
            c.wss("PmtMtd", "DD");
            c.wsbool("BtchBookg", true);
            c.wsint("NbOfTxs", intPocetInkas);
            c.wsnum("CtrlSum", decSoucetCastek);

            c.wstart("PmtTpInf");
            c.wstart("SvcLvl"); c.wss("Cd", "SEPA"); c.wend();
            c.wstart("LclInstrm"); c.wss("Cd", "CORE"); c.wend();
            c.wss("SeqTp", "RCUR");
            c.wend(); //PmtTpInf

            c.wsdate("ReqdColltnDt", v.DatumSplatnosti);
            c.wstart("Cdtr"); c.wss("Nm", v.Vystavovatel); c.wend();
            c.wstart("CdtrAcct"); c.wstart("Id"); c.wss("IBAN", v.PrijemceIBAN); c.wend(); c.wend(); //CdtrAcct
            c.wstart("CdtrAgt"); c.wstart("FinInstnId"); c.wss("BIC", v.PrijemceBIC); c.wend(); c.wend();//CdtrAgt
            c.wss("ChrgBr", "SLEV");

            foreach(var rec in v.lisRecords)
            {
                c.wstart("DrctDbtTxInf");
                c.wstart("PmtId"); c.wss("EndToEndId", "NOTPROVIDED"); c.wend();

                c.wstart("InstdAmt");
                c.oneattribute("Ccy", rec.FakturaMena);
                c.purestring(BO.BAS.GN(rec.FakturaCastka));
                c.wend(); //InstdAmt

                c.wstart("DrctDbtTx");
                c.wstart("MndtRltdInf"); c.wss("MndtId", rec.ClientID); c.wsdate("DtOfSgntr", rec.PodepsaniMandatu); c.wend();

                c.wstart("CdtrSchmeId");
                c.wstart("Id");
                c.wstart("PrvtId");
                c.wstart("Othr"); c.wss("Id", v.PrijemceCID);
                c.wstart("SchmeNm"); c.wss("Prtry", "SEPA"); c.wend();
                c.wend(4); //CdtrSchmeId+Id+PrvtId+Othr

                c.wend(); //DrctDbtTx


                c.wstart("DbtrAgt");
                c.wstart("FinInstnId"); c.wss("BIC", rec.BIC); c.wend();
                c.wend(); //DbtrAgt

                c.wstart("Dbtr"); c.wss("Nm", rec.Jmeno); c.wend();
                c.wstart("DbtrAcct");
                c.wstart("Id"); c.wss("IBAN", rec.IBAN); c.wend();
                c.wend(); //DbtrAcct

                c.wstart("RmtInf"); c.wss("Ustrd", rec.FakturaVS); c.wend();


                c.wend(); //DrctDbtTxInf
            }
            

            c.flushandclose();

            v.GeneratedFileName = v.Guid + ".xml";


        }



        
        
    }
}
