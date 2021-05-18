/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace SepaInkaso
{
	[XmlRoot(ElementName = "Authstn", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Authstn
	{
		[XmlElement(ElementName = "Cd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Cd { get; set; }
		[XmlElement(ElementName = "Titl", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Titl { get; set; }
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
	}

	[XmlRoot(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class PstlAdr
	{
		[XmlElement(ElementName = "AdrTp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string AdrTp { get; set; }
		[XmlElement(ElementName = "Dept", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Dept { get; set; }
		[XmlElement(ElementName = "SubDept", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string SubDept { get; set; }
		[XmlElement(ElementName = "StrtNm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string StrtNm { get; set; }
		[XmlElement(ElementName = "BldgNb", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string BldgNb { get; set; }
		[XmlElement(ElementName = "PstCd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string PstCd { get; set; }
		[XmlElement(ElementName = "TwnNm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string TwnNm { get; set; }
		[XmlElement(ElementName = "CtrySubDvsn", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtrySubDvsn { get; set; }
		[XmlElement(ElementName = "Ctry", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ctry { get; set; }
		[XmlElement(ElementName = "AdrLine", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<string> AdrLine { get; set; }
	}

	[XmlRoot(ElementName = "SchmeNm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class SchmeNm
	{
		[XmlElement(ElementName = "Cd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Cd { get; set; }
	}

	[XmlRoot(ElementName = "Othr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Othr
	{
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Id { get; set; }
		[XmlElement(ElementName = "SchmeNm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public SchmeNm SchmeNm { get; set; }
		[XmlElement(ElementName = "Issr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Issr { get; set; }
	}

	[XmlRoot(ElementName = "OrgId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class OrgId
	{
		[XmlElement(ElementName = "BICOrBEI", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string BICOrBEI { get; set; }
		[XmlElement(ElementName = "Othr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<Othr> Othr { get; set; }
	}

	[XmlRoot(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Id
	{
		[XmlElement(ElementName = "OrgId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public OrgId OrgId { get; set; }
		[XmlElement(ElementName = "IBAN", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string IBAN { get; set; }
	}

	[XmlRoot(ElementName = "CtctDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class CtctDtls
	{
		[XmlElement(ElementName = "NmPrfx", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string NmPrfx { get; set; }
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PhneNb", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string PhneNb { get; set; }
		[XmlElement(ElementName = "MobNb", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string MobNb { get; set; }
		[XmlElement(ElementName = "FaxNb", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string FaxNb { get; set; }
		[XmlElement(ElementName = "EmailAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string EmailAdr { get; set; }
		[XmlElement(ElementName = "Othr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Othr { get; set; }
	}

	[XmlRoot(ElementName = "InitgPty", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class InitgPty
	{
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PstlAdr PstlAdr { get; set; }
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "CtryOfRes", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtryOfRes { get; set; }
		[XmlElement(ElementName = "CtctDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CtctDtls CtctDtls { get; set; }
	}

	[XmlRoot(ElementName = "ClrSysId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class ClrSysId
	{
		[XmlElement(ElementName = "Cd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Cd { get; set; }
	}

	[XmlRoot(ElementName = "ClrSysMmbId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class ClrSysMmbId
	{
		[XmlElement(ElementName = "ClrSysId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public ClrSysId ClrSysId { get; set; }
		[XmlElement(ElementName = "MmbId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string MmbId { get; set; }
	}

	[XmlRoot(ElementName = "FinInstnId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class FinInstnId
	{
		[XmlElement(ElementName = "BIC", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string BIC { get; set; }
		[XmlElement(ElementName = "ClrSysMmbId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public ClrSysMmbId ClrSysMmbId { get; set; }
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PstlAdr PstlAdr { get; set; }
		[XmlElement(ElementName = "Othr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Othr Othr { get; set; }
	}

	[XmlRoot(ElementName = "BrnchId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class BrnchId
	{
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Id { get; set; }
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PstlAdr PstlAdr { get; set; }
	}

	[XmlRoot(ElementName = "FwdgAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class FwdgAgt
	{
		[XmlElement(ElementName = "FinInstnId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public FinInstnId FinInstnId { get; set; }
		[XmlElement(ElementName = "BrnchId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public BrnchId BrnchId { get; set; }
	}

	[XmlRoot(ElementName = "GrpHdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class GrpHdr
	{
		[XmlElement(ElementName = "MsgId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string MsgId { get; set; }
		[XmlElement(ElementName = "CreDtTm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CreDtTm { get; set; }
		[XmlElement(ElementName = "Authstn", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<Authstn> Authstn { get; set; }
		[XmlElement(ElementName = "NbOfTxs", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string NbOfTxs { get; set; }
		[XmlElement(ElementName = "CtrlSum", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtrlSum { get; set; }
		[XmlElement(ElementName = "InitgPty", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public InitgPty InitgPty { get; set; }
		[XmlElement(ElementName = "FwdgAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public FwdgAgt FwdgAgt { get; set; }
	}

	[XmlRoot(ElementName = "SvcLvl", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class SvcLvl
	{
		[XmlElement(ElementName = "Cd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Cd { get; set; }
	}

	[XmlRoot(ElementName = "LclInstrm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class LclInstrm
	{
		[XmlElement(ElementName = "Cd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Cd { get; set; }
	}

	[XmlRoot(ElementName = "CtgyPurp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class CtgyPurp
	{
		[XmlElement(ElementName = "Cd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Cd { get; set; }
	}

	[XmlRoot(ElementName = "PmtTpInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class PmtTpInf
	{
		[XmlElement(ElementName = "InstrPrty", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string InstrPrty { get; set; }
		[XmlElement(ElementName = "SvcLvl", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public SvcLvl SvcLvl { get; set; }
		[XmlElement(ElementName = "LclInstrm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public LclInstrm LclInstrm { get; set; }
		[XmlElement(ElementName = "SeqTp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string SeqTp { get; set; }
		[XmlElement(ElementName = "CtgyPurp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CtgyPurp CtgyPurp { get; set; }
	}

	[XmlRoot(ElementName = "Cdtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Cdtr
	{
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PstlAdr PstlAdr { get; set; }
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "CtryOfRes", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtryOfRes { get; set; }
		[XmlElement(ElementName = "CtctDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CtctDtls CtctDtls { get; set; }
		[XmlElement(ElementName = "TaxId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string TaxId { get; set; }
		[XmlElement(ElementName = "RegnId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string RegnId { get; set; }
		[XmlElement(ElementName = "TaxTp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string TaxTp { get; set; }
	}

	[XmlRoot(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Tp
	{
		[XmlElement(ElementName = "Cd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Cd { get; set; }
		[XmlElement(ElementName = "CdOrPrtry", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CdOrPrtry CdOrPrtry { get; set; }
		[XmlElement(ElementName = "Issr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Issr { get; set; }
	}

	[XmlRoot(ElementName = "CdtrAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class CdtrAcct
	{
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Tp Tp { get; set; }
		[XmlElement(ElementName = "Ccy", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ccy { get; set; }
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
	}

	[XmlRoot(ElementName = "CdtrAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class CdtrAgt
	{
		[XmlElement(ElementName = "FinInstnId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public FinInstnId FinInstnId { get; set; }
		[XmlElement(ElementName = "BrnchId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public BrnchId BrnchId { get; set; }
	}

	[XmlRoot(ElementName = "CdtrAgtAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class CdtrAgtAcct
	{
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Tp Tp { get; set; }
		[XmlElement(ElementName = "Ccy", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ccy { get; set; }
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
	}

	[XmlRoot(ElementName = "UltmtCdtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class UltmtCdtr
	{
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PstlAdr PstlAdr { get; set; }
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "CtryOfRes", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtryOfRes { get; set; }
		[XmlElement(ElementName = "CtctDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CtctDtls CtctDtls { get; set; }
	}

	[XmlRoot(ElementName = "ChrgsAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class ChrgsAcct
	{
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Tp Tp { get; set; }
		[XmlElement(ElementName = "Ccy", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ccy { get; set; }
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
	}

	[XmlRoot(ElementName = "ChrgsAcctAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class ChrgsAcctAgt
	{
		[XmlElement(ElementName = "FinInstnId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public FinInstnId FinInstnId { get; set; }
		[XmlElement(ElementName = "BrnchId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public BrnchId BrnchId { get; set; }
	}

	[XmlRoot(ElementName = "CdtrSchmeId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class CdtrSchmeId
	{
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PstlAdr PstlAdr { get; set; }
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "CtryOfRes", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtryOfRes { get; set; }
		[XmlElement(ElementName = "CtctDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CtctDtls CtctDtls { get; set; }
	}

	[XmlRoot(ElementName = "PmtId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class PmtId
	{
		[XmlElement(ElementName = "InstrId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string InstrId { get; set; }
		[XmlElement(ElementName = "EndToEndId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string EndToEndId { get; set; }
	}

	[XmlRoot(ElementName = "InstdAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class InstdAmt
	{
		[XmlAttribute(AttributeName = "Ccy")]
		public string Ccy { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "OrgnlCdtrSchmeId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class OrgnlCdtrSchmeId
	{
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PstlAdr PstlAdr { get; set; }
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "CtryOfRes", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtryOfRes { get; set; }
		[XmlElement(ElementName = "CtctDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CtctDtls CtctDtls { get; set; }
	}

	[XmlRoot(ElementName = "OrgnlCdtrAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class OrgnlCdtrAgt
	{
		[XmlElement(ElementName = "FinInstnId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public FinInstnId FinInstnId { get; set; }
		[XmlElement(ElementName = "BrnchId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public BrnchId BrnchId { get; set; }
	}

	[XmlRoot(ElementName = "OrgnlCdtrAgtAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class OrgnlCdtrAgtAcct
	{
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Tp Tp { get; set; }
		[XmlElement(ElementName = "Ccy", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ccy { get; set; }
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
	}

	[XmlRoot(ElementName = "OrgnlDbtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class OrgnlDbtr
	{
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PstlAdr PstlAdr { get; set; }
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "CtryOfRes", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtryOfRes { get; set; }
		[XmlElement(ElementName = "CtctDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CtctDtls CtctDtls { get; set; }
	}

	[XmlRoot(ElementName = "OrgnlDbtrAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class OrgnlDbtrAcct
	{
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Tp Tp { get; set; }
		[XmlElement(ElementName = "Ccy", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ccy { get; set; }
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
	}

	[XmlRoot(ElementName = "OrgnlDbtrAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class OrgnlDbtrAgt
	{
		[XmlElement(ElementName = "FinInstnId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public FinInstnId FinInstnId { get; set; }
		[XmlElement(ElementName = "BrnchId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public BrnchId BrnchId { get; set; }
	}

	[XmlRoot(ElementName = "OrgnlDbtrAgtAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class OrgnlDbtrAgtAcct
	{
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Tp Tp { get; set; }
		[XmlElement(ElementName = "Ccy", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ccy { get; set; }
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
	}

	[XmlRoot(ElementName = "AmdmntInfDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class AmdmntInfDtls
	{
		[XmlElement(ElementName = "OrgnlMndtId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string OrgnlMndtId { get; set; }
		[XmlElement(ElementName = "OrgnlCdtrSchmeId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public OrgnlCdtrSchmeId OrgnlCdtrSchmeId { get; set; }
		[XmlElement(ElementName = "OrgnlCdtrAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public OrgnlCdtrAgt OrgnlCdtrAgt { get; set; }
		[XmlElement(ElementName = "OrgnlCdtrAgtAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public OrgnlCdtrAgtAcct OrgnlCdtrAgtAcct { get; set; }
		[XmlElement(ElementName = "OrgnlDbtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public OrgnlDbtr OrgnlDbtr { get; set; }
		[XmlElement(ElementName = "OrgnlDbtrAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public OrgnlDbtrAcct OrgnlDbtrAcct { get; set; }
		[XmlElement(ElementName = "OrgnlDbtrAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public OrgnlDbtrAgt OrgnlDbtrAgt { get; set; }
		[XmlElement(ElementName = "OrgnlDbtrAgtAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public OrgnlDbtrAgtAcct OrgnlDbtrAgtAcct { get; set; }
		[XmlElement(ElementName = "OrgnlFnlColltnDt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string OrgnlFnlColltnDt { get; set; }
		[XmlElement(ElementName = "OrgnlFrqcy", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string OrgnlFrqcy { get; set; }
	}

	[XmlRoot(ElementName = "MndtRltdInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class MndtRltdInf
	{
		[XmlElement(ElementName = "MndtId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string MndtId { get; set; }
		[XmlElement(ElementName = "DtOfSgntr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string DtOfSgntr { get; set; }
		[XmlElement(ElementName = "AmdmntInd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string AmdmntInd { get; set; }
		[XmlElement(ElementName = "AmdmntInfDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public AmdmntInfDtls AmdmntInfDtls { get; set; }
		[XmlElement(ElementName = "ElctrncSgntr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string ElctrncSgntr { get; set; }
		[XmlElement(ElementName = "FrstColltnDt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string FrstColltnDt { get; set; }
		[XmlElement(ElementName = "FnlColltnDt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string FnlColltnDt { get; set; }
		[XmlElement(ElementName = "Frqcy", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Frqcy { get; set; }
	}

	[XmlRoot(ElementName = "DrctDbtTx", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class DrctDbtTx
	{
		[XmlElement(ElementName = "MndtRltdInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public MndtRltdInf MndtRltdInf { get; set; }
		[XmlElement(ElementName = "CdtrSchmeId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CdtrSchmeId CdtrSchmeId { get; set; }
		[XmlElement(ElementName = "PreNtfctnId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string PreNtfctnId { get; set; }
		[XmlElement(ElementName = "PreNtfctnDt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string PreNtfctnDt { get; set; }
	}

	[XmlRoot(ElementName = "DbtrAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class DbtrAgt
	{
		[XmlElement(ElementName = "FinInstnId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public FinInstnId FinInstnId { get; set; }
		[XmlElement(ElementName = "BrnchId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public BrnchId BrnchId { get; set; }
	}

	[XmlRoot(ElementName = "DbtrAgtAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class DbtrAgtAcct
	{
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Tp Tp { get; set; }
		[XmlElement(ElementName = "Ccy", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ccy { get; set; }
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
	}

	[XmlRoot(ElementName = "Dbtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Dbtr
	{
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PstlAdr PstlAdr { get; set; }
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "CtryOfRes", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtryOfRes { get; set; }
		[XmlElement(ElementName = "CtctDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CtctDtls CtctDtls { get; set; }
		[XmlElement(ElementName = "TaxId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string TaxId { get; set; }
		[XmlElement(ElementName = "RegnId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string RegnId { get; set; }
		[XmlElement(ElementName = "TaxTp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string TaxTp { get; set; }
		[XmlElement(ElementName = "Authstn", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Authstn Authstn { get; set; }
	}

	[XmlRoot(ElementName = "DbtrAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class DbtrAcct
	{
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Tp Tp { get; set; }
		[XmlElement(ElementName = "Ccy", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ccy { get; set; }
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
	}

	[XmlRoot(ElementName = "UltmtDbtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class UltmtDbtr
	{
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PstlAdr PstlAdr { get; set; }
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "CtryOfRes", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtryOfRes { get; set; }
		[XmlElement(ElementName = "CtctDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CtctDtls CtctDtls { get; set; }
	}

	[XmlRoot(ElementName = "Purp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Purp
	{
		[XmlElement(ElementName = "Cd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Cd { get; set; }
	}

	[XmlRoot(ElementName = "Authrty", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Authrty
	{
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "Ctry", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ctry { get; set; }
	}

	[XmlRoot(ElementName = "Amt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Amt
	{
		[XmlAttribute(AttributeName = "Ccy")]
		public string Ccy { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Dtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Dtls
	{
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Tp { get; set; }
		[XmlElement(ElementName = "Dt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Dt { get; set; }
		[XmlElement(ElementName = "Ctry", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ctry { get; set; }
		[XmlElement(ElementName = "Cd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Cd { get; set; }
		[XmlElement(ElementName = "Amt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Amt Amt { get; set; }
		[XmlElement(ElementName = "Inf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<string> Inf { get; set; }
		[XmlElement(ElementName = "Prd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Prd Prd { get; set; }
	}

	[XmlRoot(ElementName = "RgltryRptg", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class RgltryRptg
	{
		[XmlElement(ElementName = "DbtCdtRptgInd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string DbtCdtRptgInd { get; set; }
		[XmlElement(ElementName = "Authrty", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Authrty Authrty { get; set; }
		[XmlElement(ElementName = "Dtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<Dtls> Dtls { get; set; }
	}

	[XmlRoot(ElementName = "TtlTaxblBaseAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class TtlTaxblBaseAmt
	{
		[XmlAttribute(AttributeName = "Ccy")]
		public string Ccy { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "TtlTaxAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class TtlTaxAmt
	{
		[XmlAttribute(AttributeName = "Ccy")]
		public string Ccy { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "FrToDt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class FrToDt
	{
		[XmlElement(ElementName = "FrDt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string FrDt { get; set; }
		[XmlElement(ElementName = "ToDt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string ToDt { get; set; }
	}

	[XmlRoot(ElementName = "Prd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Prd
	{
		[XmlElement(ElementName = "Yr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Yr { get; set; }
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Tp { get; set; }
		[XmlElement(ElementName = "FrToDt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public FrToDt FrToDt { get; set; }
	}

	[XmlRoot(ElementName = "TaxblBaseAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class TaxblBaseAmt
	{
		[XmlAttribute(AttributeName = "Ccy")]
		public string Ccy { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "TtlAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class TtlAmt
	{
		[XmlAttribute(AttributeName = "Ccy")]
		public string Ccy { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "TaxAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class TaxAmt
	{
		[XmlElement(ElementName = "Rate", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Rate { get; set; }
		[XmlElement(ElementName = "TaxblBaseAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public TaxblBaseAmt TaxblBaseAmt { get; set; }
		[XmlElement(ElementName = "TtlAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public TtlAmt TtlAmt { get; set; }
		[XmlElement(ElementName = "Dtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<Dtls> Dtls { get; set; }
		[XmlAttribute(AttributeName = "Ccy")]
		public string Ccy { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Rcrd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Rcrd
	{
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Tp { get; set; }
		[XmlElement(ElementName = "Ctgy", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ctgy { get; set; }
		[XmlElement(ElementName = "CtgyDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtgyDtls { get; set; }
		[XmlElement(ElementName = "DbtrSts", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string DbtrSts { get; set; }
		[XmlElement(ElementName = "CertId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CertId { get; set; }
		[XmlElement(ElementName = "FrmsCd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string FrmsCd { get; set; }
		[XmlElement(ElementName = "Prd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Prd Prd { get; set; }
		[XmlElement(ElementName = "TaxAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public TaxAmt TaxAmt { get; set; }
		[XmlElement(ElementName = "AddtlInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string AddtlInf { get; set; }
	}

	[XmlRoot(ElementName = "Tax", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Tax
	{
		[XmlElement(ElementName = "Cdtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Cdtr Cdtr { get; set; }
		[XmlElement(ElementName = "Dbtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Dbtr Dbtr { get; set; }
		[XmlElement(ElementName = "AdmstnZn", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string AdmstnZn { get; set; }
		[XmlElement(ElementName = "RefNb", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string RefNb { get; set; }
		[XmlElement(ElementName = "Mtd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Mtd { get; set; }
		[XmlElement(ElementName = "TtlTaxblBaseAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public TtlTaxblBaseAmt TtlTaxblBaseAmt { get; set; }
		[XmlElement(ElementName = "TtlTaxAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public TtlTaxAmt TtlTaxAmt { get; set; }
		[XmlElement(ElementName = "Dt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Dt { get; set; }
		[XmlElement(ElementName = "SeqNb", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string SeqNb { get; set; }
		[XmlElement(ElementName = "Rcrd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<Rcrd> Rcrd { get; set; }
	}

	[XmlRoot(ElementName = "Adr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Adr
	{
		[XmlElement(ElementName = "AdrTp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string AdrTp { get; set; }
		[XmlElement(ElementName = "Dept", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Dept { get; set; }
		[XmlElement(ElementName = "SubDept", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string SubDept { get; set; }
		[XmlElement(ElementName = "StrtNm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string StrtNm { get; set; }
		[XmlElement(ElementName = "BldgNb", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string BldgNb { get; set; }
		[XmlElement(ElementName = "PstCd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string PstCd { get; set; }
		[XmlElement(ElementName = "TwnNm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string TwnNm { get; set; }
		[XmlElement(ElementName = "CtrySubDvsn", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtrySubDvsn { get; set; }
		[XmlElement(ElementName = "Ctry", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ctry { get; set; }
		[XmlElement(ElementName = "AdrLine", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<string> AdrLine { get; set; }
	}

	[XmlRoot(ElementName = "RmtLctnPstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class RmtLctnPstlAdr
	{
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "Adr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Adr Adr { get; set; }
	}

	[XmlRoot(ElementName = "RltdRmtInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class RltdRmtInf
	{
		[XmlElement(ElementName = "RmtId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string RmtId { get; set; }
		[XmlElement(ElementName = "RmtLctnMtd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string RmtLctnMtd { get; set; }
		[XmlElement(ElementName = "RmtLctnElctrncAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string RmtLctnElctrncAdr { get; set; }
		[XmlElement(ElementName = "RmtLctnPstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public RmtLctnPstlAdr RmtLctnPstlAdr { get; set; }
	}

	[XmlRoot(ElementName = "CdOrPrtry", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class CdOrPrtry
	{
		[XmlElement(ElementName = "Cd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Cd { get; set; }
	}

	[XmlRoot(ElementName = "RfrdDocInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class RfrdDocInf
	{
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Tp Tp { get; set; }
		[XmlElement(ElementName = "Nb", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nb { get; set; }
		[XmlElement(ElementName = "RltdDt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string RltdDt { get; set; }
	}

	[XmlRoot(ElementName = "DuePyblAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class DuePyblAmt
	{
		[XmlAttribute(AttributeName = "Ccy")]
		public string Ccy { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "DscntApldAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class DscntApldAmt
	{
		[XmlAttribute(AttributeName = "Ccy")]
		public string Ccy { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "CdtNoteAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class CdtNoteAmt
	{
		[XmlAttribute(AttributeName = "Ccy")]
		public string Ccy { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "AdjstmntAmtAndRsn", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class AdjstmntAmtAndRsn
	{
		[XmlElement(ElementName = "Amt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Amt Amt { get; set; }
		[XmlElement(ElementName = "CdtDbtInd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CdtDbtInd { get; set; }
		[XmlElement(ElementName = "Rsn", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Rsn { get; set; }
		[XmlElement(ElementName = "AddtlInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string AddtlInf { get; set; }
	}

	[XmlRoot(ElementName = "RmtdAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class RmtdAmt
	{
		[XmlAttribute(AttributeName = "Ccy")]
		public string Ccy { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "RfrdDocAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class RfrdDocAmt
	{
		[XmlElement(ElementName = "DuePyblAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public DuePyblAmt DuePyblAmt { get; set; }
		[XmlElement(ElementName = "DscntApldAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public DscntApldAmt DscntApldAmt { get; set; }
		[XmlElement(ElementName = "CdtNoteAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CdtNoteAmt CdtNoteAmt { get; set; }
		[XmlElement(ElementName = "TaxAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public TaxAmt TaxAmt { get; set; }
		[XmlElement(ElementName = "AdjstmntAmtAndRsn", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<AdjstmntAmtAndRsn> AdjstmntAmtAndRsn { get; set; }
		[XmlElement(ElementName = "RmtdAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public RmtdAmt RmtdAmt { get; set; }
	}

	[XmlRoot(ElementName = "CdtrRefInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class CdtrRefInf
	{
		[XmlElement(ElementName = "Tp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Tp Tp { get; set; }
		[XmlElement(ElementName = "Ref", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Ref { get; set; }
	}

	[XmlRoot(ElementName = "Invcr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Invcr
	{
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PstlAdr PstlAdr { get; set; }
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "CtryOfRes", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtryOfRes { get; set; }
		[XmlElement(ElementName = "CtctDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CtctDtls CtctDtls { get; set; }
	}

	[XmlRoot(ElementName = "Invcee", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Invcee
	{
		[XmlElement(ElementName = "Nm", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string Nm { get; set; }
		[XmlElement(ElementName = "PstlAdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PstlAdr PstlAdr { get; set; }
		[XmlElement(ElementName = "Id", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Id Id { get; set; }
		[XmlElement(ElementName = "CtryOfRes", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtryOfRes { get; set; }
		[XmlElement(ElementName = "CtctDtls", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CtctDtls CtctDtls { get; set; }
	}

	[XmlRoot(ElementName = "Strd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Strd
	{
		[XmlElement(ElementName = "RfrdDocInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<RfrdDocInf> RfrdDocInf { get; set; }
		[XmlElement(ElementName = "RfrdDocAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public RfrdDocAmt RfrdDocAmt { get; set; }
		[XmlElement(ElementName = "CdtrRefInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CdtrRefInf CdtrRefInf { get; set; }
		[XmlElement(ElementName = "Invcr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Invcr Invcr { get; set; }
		[XmlElement(ElementName = "Invcee", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Invcee Invcee { get; set; }
		[XmlElement(ElementName = "AddtlRmtInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<string> AddtlRmtInf { get; set; }
	}

	[XmlRoot(ElementName = "RmtInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class RmtInf
	{
		[XmlElement(ElementName = "Ustrd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<string> Ustrd { get; set; }
		[XmlElement(ElementName = "Strd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<Strd> Strd { get; set; }
	}

	[XmlRoot(ElementName = "DrctDbtTxInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class DrctDbtTxInf
	{
		[XmlElement(ElementName = "PmtId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PmtId PmtId { get; set; }
		[XmlElement(ElementName = "PmtTpInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PmtTpInf PmtTpInf { get; set; }
		[XmlElement(ElementName = "InstdAmt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public InstdAmt InstdAmt { get; set; }
		[XmlElement(ElementName = "ChrgBr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string ChrgBr { get; set; }
		[XmlElement(ElementName = "DrctDbtTx", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public DrctDbtTx DrctDbtTx { get; set; }
		[XmlElement(ElementName = "UltmtCdtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public UltmtCdtr UltmtCdtr { get; set; }
		[XmlElement(ElementName = "DbtrAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public DbtrAgt DbtrAgt { get; set; }
		[XmlElement(ElementName = "DbtrAgtAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public DbtrAgtAcct DbtrAgtAcct { get; set; }
		[XmlElement(ElementName = "Dbtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Dbtr Dbtr { get; set; }
		[XmlElement(ElementName = "DbtrAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public DbtrAcct DbtrAcct { get; set; }
		[XmlElement(ElementName = "UltmtDbtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public UltmtDbtr UltmtDbtr { get; set; }
		[XmlElement(ElementName = "InstrForCdtrAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string InstrForCdtrAgt { get; set; }
		[XmlElement(ElementName = "Purp", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Purp Purp { get; set; }
		[XmlElement(ElementName = "RgltryRptg", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<RgltryRptg> RgltryRptg { get; set; }
		[XmlElement(ElementName = "Tax", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Tax Tax { get; set; }
		[XmlElement(ElementName = "RltdRmtInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<RltdRmtInf> RltdRmtInf { get; set; }
		[XmlElement(ElementName = "RmtInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public RmtInf RmtInf { get; set; }
	}

	[XmlRoot(ElementName = "PmtInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class PmtInf
	{
		[XmlElement(ElementName = "PmtInfId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string PmtInfId { get; set; }
		[XmlElement(ElementName = "PmtMtd", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string PmtMtd { get; set; }
		[XmlElement(ElementName = "BtchBookg", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string BtchBookg { get; set; }
		[XmlElement(ElementName = "NbOfTxs", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string NbOfTxs { get; set; }
		[XmlElement(ElementName = "CtrlSum", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string CtrlSum { get; set; }
		[XmlElement(ElementName = "PmtTpInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public PmtTpInf PmtTpInf { get; set; }
		[XmlElement(ElementName = "ReqdColltnDt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string ReqdColltnDt { get; set; }
		[XmlElement(ElementName = "Cdtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public Cdtr Cdtr { get; set; }
		[XmlElement(ElementName = "CdtrAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CdtrAcct CdtrAcct { get; set; }
		[XmlElement(ElementName = "CdtrAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CdtrAgt CdtrAgt { get; set; }
		[XmlElement(ElementName = "CdtrAgtAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CdtrAgtAcct CdtrAgtAcct { get; set; }
		[XmlElement(ElementName = "UltmtCdtr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public UltmtCdtr UltmtCdtr { get; set; }
		[XmlElement(ElementName = "ChrgBr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public string ChrgBr { get; set; }
		[XmlElement(ElementName = "ChrgsAcct", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public ChrgsAcct ChrgsAcct { get; set; }
		[XmlElement(ElementName = "ChrgsAcctAgt", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public ChrgsAcctAgt ChrgsAcctAgt { get; set; }
		[XmlElement(ElementName = "CdtrSchmeId", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CdtrSchmeId CdtrSchmeId { get; set; }
		[XmlElement(ElementName = "DrctDbtTxInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<DrctDbtTxInf> DrctDbtTxInf { get; set; }
	}

	[XmlRoot(ElementName = "CstmrDrctDbtInitn", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class CstmrDrctDbtInitn
	{
		[XmlElement(ElementName = "GrpHdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public GrpHdr GrpHdr { get; set; }
		[XmlElement(ElementName = "PmtInf", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public List<PmtInf> PmtInf { get; set; }
	}

	[XmlRoot(ElementName = "Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
	public class Document
	{
		[XmlElement(ElementName = "CstmrDrctDbtInitn", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02")]
		public CstmrDrctDbtInitn CstmrDrctDbtInitn { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

}