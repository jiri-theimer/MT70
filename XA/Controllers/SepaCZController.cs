using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SepaInkasoCZ;

namespace XA.Controllers
{
    public class SepaCZController : Controller
    {
        public IActionResult Index()
        {
            string strGUID = BO.BAS.GetGuid();
            DateTime datDatumSplatnosti = DateTime.Today.AddDays(1);
            string strVystavovatel = "SCHAFFER & PARTNER";
            string strVystavovatelUlice = "Vodičkova 710/31";
            string strVystavovatelMesto = "Praha 1";
            string strIBAN = "DE30760200700003660788";
            string strBIC = "HYVEDEMM460";
            //string strIBAN_Poplatky = null;
            string strClientID = "1298 ALECUR GmbH";
            DateTime datPodepsaniMandatu = DateTime.Today.AddDays(-20);
            string strPrijemceCID = "CZ58ZZZ10272";

            var doc = new Document();
            var cd1 = new CustomerDirectDebitInitiationV02();

            cd1.GrpHdr = new GroupHeader39CZ() { MsgId = strGUID, CreDtTm = DateTime.Now, NbOfTxs = "325", CtrlSum = DN(8999.44) };



            cd1.GrpHdr.InitgPty = new PartyIdentification32CZ1() { Nm = strVystavovatel };

            string[] adresaVystavovatel = new string[2];
            adresaVystavovatel[0] = strVystavovatelUlice;
            adresaVystavovatel[1] = strVystavovatelMesto;
            cd1.GrpHdr.InitgPty.PstlAdr = new PostalAddress6CZ() { Ctry = "CZ", AdrLine = adresaVystavovatel };



            PaymentInstructionInformation4CZ[] pmis = new PaymentInstructionInformation4CZ[1];
            pmis[0] = new PaymentInstructionInformation4CZ() { PmtInfId = strGUID, BtchBookg = true, NbOfTxs = "1", ReqdColltnDt = datDatumSplatnosti };

            pmis[0].Cdtr = new PartyIdentification32CZ2() { Nm = strVystavovatel };  //jméno příjemce
            pmis[0].Cdtr.PstlAdr = new PostalAddress6CZ() { AdrLine = adresaVystavovatel };  //adresa příjemce
            pmis[0].CdtrAcct = new CashAccount16CZ();
            pmis[0].CdtrAcct.Id = new AccountIdentification4CZ() { IBAN = strIBAN }; //číslo účtu příjemce ve formátu IBAN
            pmis[0].CdtrAgt = new BranchAndFinancialInstitutionIdentification4CZ();
            pmis[0].CdtrAgt.FinInstnId = new FinancialInstitutionIdentification7CZ() { BIC = strBIC };   //kód banky příjemce ve formátu BIC / SWIFT kódu
            //pmis[0].ChrgBr = new ChargeBearerType1CodeCZ();  //typ poplatku SLEV – plátce hradí poplatky své banky, příjemce hradí poplatky své banky
            pmis[0].ChrgBr = ChargeBearerType1CodeCZ.SLEV;

            

            //if (strIBAN_Poplatky != null)
            //{
            //    //účet pro poplatky
            //    pmis[0].ChrgsAcct = new CashAccount16CZ();
            //    pmis[0].ChrgsAcct.Id = new AccountIdentification4CZ() { IBAN = strIBAN_Poplatky };
            //}



            pmis[0].PmtTpInf = new PaymentTypeInformation20CZ();
            pmis[0].PmtTpInf.SvcLvl = new ServiceLevel8CZ(); //typ služby, konstanta: SEPA
            pmis[0].PmtTpInf.SvcLvl.Cd = ExternalServiceLevel1CodeCZ.SEPA;  //typ služby, konstanta: SEPA
            pmis[0].PmtTpInf.LclInstrm = new LocalInstrument2CZ();   //platební schema, konstanta: CORE
            pmis[0].PmtTpInf.LclInstrm.Cd = ExternalLocalInstrument1CodeCZ.CORE;
            pmis[0].PmtTpInf.SeqTp = SequenceType1Code.RCUR; //pořadí SEPA inkasa



            DirectDebitTransactionInformation9CZ[] items = new DirectDebitTransactionInformation9CZ[10];  //jednotlivé pohledávky
            items[0] = new DirectDebitTransactionInformation9CZ();
            items[0].PmtId = new PaymentIdentification1() { EndToEndId = "NOTPROVIDED" };

            items[0].InstdAmt = new ActiveOrHistoricCurrencyAndAmount();
            items[0].InstdAmt.Value = DN(9988.66);
            items[0].InstdAmt.Ccy = ActiveOrHistoricCurrencyCodeCZ.EUR; //new ActiveOrHistoricCurrencyCodeCZ();

            items[0].DrctDbtTx = new DirectDebitTransaction6CZ();
            items[0].DrctDbtTx.MndtRltdInf = new MandateRelatedInformation6() { MndtId = strClientID, DtOfSgntr = datPodepsaniMandatu }; //číslo mandátu (dohody o inkasování mezi příjemcem a plátcem
            items[0].DrctDbtTx.CdtrSchmeId = new PartyIdentification32CZ3();
            items[0].DrctDbtTx.CdtrSchmeId.Id = new Party6CZ();
            items[0].DrctDbtTx.CdtrSchmeId.Id.Item = new PersonIdentification5CZ2();
            items[0].DrctDbtTx.CdtrSchmeId.Id.Item.Othr = new GenericPersonIdentification1CZ() { Id = strPrijemceCID };  //CID příjemce

            pmis[0].DrctDbtTxInf = items;




            cd1.PmtInf = pmis;




            doc.CstmrDrctDbtInitn = cd1;


            //var paymentinstruction = new CustomerDirectDebitInitiationV02();


            //paymentinstruction.PmtInfId = BO.BAS.GetGuid();
            //paymentinstruction.PmtMtd = "DD";



            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Document));
            System.IO.TextWriter tw = new System.IO.StreamWriter(@"c:\temp\sepa5.xml");
            xs.Serialize(tw, doc);


            return View();
        }






        private Decimal DN(double n)
        {
            return Convert.ToDecimal(n);
        }
    }
}
