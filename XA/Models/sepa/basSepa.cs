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
            var doc = new SepaInkaso.Document();
            var xx = new SepaInkaso.CstmrDrctDbtInitn();
            xx.GrpHdr = new SepaInkaso.GrpHdr() { MsgId = BO.BAS.GetGuid(), CreDtTm = DateTime.Now.ToString("dd.MM.yyyy"), NbOfTxs = "325", CtrlSum = "8999.44" };

            doc.CstmrDrctDbtInitn = xx;

            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(SepaInkaso.Document));
            System.IO.TextWriter tw = new System.IO.StreamWriter(@"c:\temp\garage.xml");
            xs.Serialize(tw, doc);
        }
    }
}
