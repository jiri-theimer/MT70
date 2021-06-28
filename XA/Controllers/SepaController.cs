using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SepaInkaso;

namespace XA.Controllers
{
    public class SepaController : Controller
    {
        public IActionResult Index()
        {
            string strGUID = BO.BAS.GetGuid();
            DateTime datDatumSplatnosti = DateTime.Today.AddDays(1);
            string strVystavovatel = "SCHAFFER PARTNER";
            string strVystavovatelUlice = "Vodičkova 710/31";
            string strVystavovatelMesto = "Praha 1";
            string strPrijemceIBAN = "DE30760200700003660788";
            string strPrijemceBIC = "HYVEDEMM460";
            //string strIBAN_Poplatky = null;
            string strClientID = "1298 ALECUR GmbH";
            DateTime datPodepsaniMandatu = DateTime.Today.AddDays(-20);
            string strPrijemceCID = "CZ58ZZZ10272";
            string strClientBankaPlatceBIC = "GENODED1MRB";
            string strClientJmenoPlatce = "BARI Mönchengladbach OHG";
            string strClientIBAN = "DE29310605177202767014";
            string strFakturaVS = "1520211269";
            decimal decFakturaCastka = DN(8949.50);
            decimal decSoucetCastek = DN(8949.50);
            int intPocetInkas = 1;

            var doc = new Document();
            var cd1 = new CustomerDirectDebitInitiationV02();
            
            cd1.GrpHdr = new GroupHeader39() { MsgId = strGUID, CreDtTm = DateTime.Now, NbOfTxs = intPocetInkas.ToString(), CtrlSum = decSoucetCastek };

            

            cd1.GrpHdr.InitgPty = new PartyIdentification32() { Nm = strVystavovatel };

            string[] adresaVystavovatel = new string[2];
            adresaVystavovatel[0] = strVystavovatelUlice;
            adresaVystavovatel[1] = strVystavovatelMesto;
            cd1.GrpHdr.InitgPty.PstlAdr = new PostalAddress6() { Ctry = "CZ", AdrLine = adresaVystavovatel };



            PaymentInstructionInformation4[] pmis = new PaymentInstructionInformation4[1];
            pmis[0] = new PaymentInstructionInformation4() { 
                PmtInfId = strGUID
                ,BtchBookg=true
                ,NbOfTxs="1"
                ,ReqdColltnDt= datDatumSplatnosti
                ,ChrgBr= ChargeBearerType1Code.SLEV
            };
            
            
            pmis[0].Cdtr = new PartyIdentification32() { Nm = strVystavovatel };  //jméno příjemce
            pmis[0].Cdtr.PstlAdr = new PostalAddress6() { AdrLine = adresaVystavovatel };  //adresa příjemce
            pmis[0].CdtrAcct = new CashAccount16();
            pmis[0].CdtrAcct.Id = new AccountIdentification4Choice(); //číslo účtu příjemce ve formátu IBAN
            pmis[0].CdtrAcct.Id.Item = strPrijemceIBAN;
               
            pmis[0].CdtrAgt = new BranchAndFinancialInstitutionIdentification4();
            pmis[0].CdtrAgt.FinInstnId = new FinancialInstitutionIdentification7() { BIC = strPrijemceBIC };   //kód banky příjemce ve formátu BIC / SWIFT kódu            

            //pmis[0].ChrgBr = ChargeBearerType1Code.SLEV; //typ poplatku SLEV – plátce hradí poplatky své banky, příjemce hradí poplatky své banky


            

            //if (strIBAN_Poplatky != null)
            //{
            //    //účet pro poplatky
            //    pmis[0].ChrgsAcct = new CashAccount16CZ();
            //    pmis[0].ChrgsAcct.Id = new AccountIdentification4CZ() { IBAN = strIBAN_Poplatky };
            //}



            pmis[0].PmtTpInf = new PaymentTypeInformation20();
            pmis[0].PmtTpInf.SvcLvl = new ServiceLevel8Choice(); //typ služby, konstanta: SEPA
            pmis[0].PmtTpInf.SvcLvl.ItemElementName = ItemChoiceType4.Cd;
            pmis[0].PmtTpInf.SvcLvl.Item = "SEPA";
            

            //pmis[0].PmtTpInf.SvcLvl.Cd = ExternalServiceLevel1CodeCZ.SEPA;  //typ služby, konstanta: SEPA
            pmis[0].PmtTpInf.LclInstrm = new LocalInstrument2Choice();   //platební schema, konstanta: CORE
            pmis[0].PmtTpInf.LclInstrm.ItemElementName = ItemChoiceType5.Cd;
            pmis[0].PmtTpInf.LclInstrm.Item = "CORE";
            pmis[0].PmtTpInf.SeqTp = SequenceType1Code.RCUR; //pořadí SEPA inkasa



            DirectDebitTransactionInformation9[] items = new DirectDebitTransactionInformation9[10];  //jednotlivé pohledávky
            items[0] = new DirectDebitTransactionInformation9();
            items[0].PmtId = new PaymentIdentification1() { EndToEndId = "NOTPROVIDED" };

            items[0].InstdAmt = new ActiveOrHistoricCurrencyAndAmount() { Ccy = "EUR", Value = decFakturaCastka };

            items[0].DrctDbtTx = new DirectDebitTransaction6();
            items[0].DrctDbtTx.MndtRltdInf = new MandateRelatedInformation6() { MndtId = strClientID, DtOfSgntr= datPodepsaniMandatu }; //číslo mandátu (dohody o inkasování mezi příjemcem a plátcem
            items[0].DrctDbtTx.CdtrSchmeId = new PartyIdentification32();
                        
            var cprvid = new PersonIdentification5();
            GenericPersonIdentification1[] gifs=new GenericPersonIdentification1[1];
            gifs[0] = new GenericPersonIdentification1();
            gifs[0].Id = strPrijemceCID;            //CID příjemce   
            gifs[0].SchmeNm = new PersonIdentificationSchemeName1Choice();
            gifs[0].SchmeNm.ItemElementName = ItemChoiceType1.Prtry;
            gifs[0].SchmeNm.Item = "SEPA";
            cprvid.Othr = gifs;

            items[0].DrctDbtTx.CdtrSchmeId.Id = new Party6Choice() { Item = cprvid };

            items[0].DbtrAgt = new BranchAndFinancialInstitutionIdentification4();  //banka plátce
            items[0].DbtrAgt.FinInstnId = new FinancialInstitutionIdentification7() { BIC = strClientBankaPlatceBIC };
            items[0].Dbtr = new PartyIdentification32() {Nm=strClientJmenoPlatce };  //jméno plátce
            items[0].DbtrAcct = new CashAccount16() { Id = new AccountIdentification4Choice() };
            items[0].DbtrAcct.Id.Item = strClientIBAN; //číslo účtu příjemce ve formátu IBAN

            string[] strVS = { strFakturaVS };
            items[0].RmtInf = new RemittanceInformation5() { Ustrd = strVS };      //Nestrukturovaná zpráva pro plátce: variabilní symbol


            pmis[0].DrctDbtTxInf = items;


            

            cd1.PmtInf = pmis;

            


            doc.CstmrDrctDbtInitn = cd1;


            //var paymentinstruction = new CustomerDirectDebitInitiationV02();


            //paymentinstruction.PmtInfId = BO.BAS.GetGuid();
            //paymentinstruction.PmtMtd = "DD";

            string strDestPath = @"c:\temp\sepa8.xml";

            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Document));
            
            System.IO.TextWriter tw = new System.IO.StreamWriter(strDestPath);
           
            xs.Serialize(tw, doc);
            tw.Close();           

            var s = System.IO.File.ReadAllText(strDestPath);
            if (!s.Contains("<ChrgBr>SLEV</ChrgBr>"))
            {
                s = s.Replace("</CdtrAgt>", "</CdtrAgt>" + System.Environment.NewLine + "<ChrgBr>SLEV</ChrgBr>");
                System.IO.File.WriteAllText(@"c:\temp\sepa9.xml", s);
            }
            

            return View();
        }






        private Decimal DN(double n)
        {
            return Convert.ToDecimal(n);
        }
    }
}
