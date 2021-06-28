using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XA.Models.sepa
{
    public class basSepa
    {
        public void Priklad()
        {

            var c=new SepaPokus.Document();
            var doc = new SepaPokus.Document();
            var xx = new SepaPokus.CstmrDrctDbtInitn();
            xx.GrpHdr = new SepaPokus.GrpHdr() { MsgId = BO.BAS.GetGuid(), CreDtTm = DateTime.Now.ToString("dd.MM.yyyy"), NbOfTxs = "325", CtrlSum = "8999.44" };

            doc.CstmrDrctDbtInitn = xx;



            var paymentinstruction = new SepaPokus.PmtInf();
            paymentinstruction.PmtInfId = BO.BAS.GetGuid();
            paymentinstruction.PmtMtd = "DD";
            paymentinstruction.BtchBookg = "true";
            paymentinstruction.NbOfTxs = "325";
            paymentinstruction.CtrlSum = "3333";


           

            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(SepaInkaso.Document));
            System.IO.TextWriter tw = new System.IO.StreamWriter(@"c:\temp\garage.xml");
            xs.Serialize(tw, doc);
        }
    }
}
