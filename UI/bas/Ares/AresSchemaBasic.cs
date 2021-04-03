using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Ares
{


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_answer_basic/v_1.0.3")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_answer_basic/v_1.0.3", IsNullable = false)]
    public partial class Ares_odpovedi
    {

        private Ares_odpovediOdpoved odpovedField;

        private System.DateTime odpoved_datum_casField;

        private byte odpoved_pocetField;

        private string odpoved_typField;

        private string vystup_formatField;

        private string xsltField;

        private string validation_XSLTField;

        private string idField;

        /// <remarks/>
        public Ares_odpovediOdpoved Odpoved
        {
            get
            {
                return this.odpovedField;
            }
            set
            {
                this.odpovedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime odpoved_datum_cas
        {
            get
            {
                return this.odpoved_datum_casField;
            }
            set
            {
                this.odpoved_datum_casField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte odpoved_pocet
        {
            get
            {
                return this.odpoved_pocetField;
            }
            set
            {
                this.odpoved_pocetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string odpoved_typ
        {
            get
            {
                return this.odpoved_typField;
            }
            set
            {
                this.odpoved_typField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string vystup_format
        {
            get
            {
                return this.vystup_formatField;
            }
            set
            {
                this.vystup_formatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string xslt
        {
            get
            {
                return this.xsltField;
            }
            set
            {
                this.xsltField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string validation_XSLT
        {
            get
            {
                return this.validation_XSLTField;
            }
            set
            {
                this.validation_XSLTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_answer_basic/v_1.0.3")]
    public partial class Ares_odpovediOdpoved
    {

        private byte pIDField;

        private VH vhField;

        private byte pZAField;

        private UVOD uVODField;

        private VBAS vBASField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
        public byte PID
        {
            get
            {
                return this.pIDField;
            }
            set
            {
                this.pIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
        public VH VH
        {
            get
            {
                return this.vhField;
            }
            set
            {
                this.vhField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
        public byte PZA
        {
            get
            {
                return this.pZAField;
            }
            set
            {
                this.pZAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
        public UVOD UVOD
        {
            get
            {
                return this.uVODField;
            }
            set
            {
                this.uVODField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
        public VBAS VBAS
        {
            get
            {
                return this.vBASField;
            }
            set
            {
                this.vBASField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3", IsNullable = false)]
    public partial class VH
    {

        private byte kField;

        /// <remarks/>
        public byte K
        {
            get
            {
                return this.kField;
            }
            set
            {
                this.kField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3", IsNullable = false)]
    public partial class UVOD
    {

        private string ndField;

        private System.DateTime aDBField;

        private System.DateTime dVYField;

        private System.DateTime cASField;

        private byte typ_odkazuField;

        /// <remarks/>
        public string ND
        {
            get
            {
                return this.ndField;
            }
            set
            {
                this.ndField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime ADB
        {
            get
            {
                return this.aDBField;
            }
            set
            {
                this.aDBField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime DVY
        {
            get
            {
                return this.dVYField;
            }
            set
            {
                this.dVYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "time")]
        public System.DateTime CAS
        {
            get
            {
                return this.cASField;
            }
            set
            {
                this.cASField = value;
            }
        }

        /// <remarks/>
        public byte Typ_odkazu
        {
            get
            {
                return this.typ_odkazuField;
            }
            set
            {
                this.typ_odkazuField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3", IsNullable = false)]
    public partial class VBAS
    {

        private VBASICO iCOField;

        private VBASDIC dICField;

        private VBASOF ofField;

        private System.DateTime dvField;

        private VBASPF pfField;

        private VBASAD adField;

        private VBASAA aaField;

        private string pSUField;

        private VBASROR rORField;

        private VBASRRZ rRZField;

        private VBASKPP kPPField;

        private VBASNACE[] naceField;

        private VBASPP[] pPIField;

        private VBASObor_cinnosti[] obory_cinnostiField;

        /// <remarks/>
        public VBASICO ICO
        {
            get
            {
                return this.iCOField;
            }
            set
            {
                this.iCOField = value;
            }
        }

        /// <remarks/>
        public VBASDIC DIC
        {
            get
            {
                return this.dICField;
            }
            set
            {
                this.dICField = value;
            }
        }

        /// <remarks/>
        public VBASOF OF
        {
            get
            {
                return this.ofField;
            }
            set
            {
                this.ofField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime DV
        {
            get
            {
                return this.dvField;
            }
            set
            {
                this.dvField = value;
            }
        }

        /// <remarks/>
        public VBASPF PF
        {
            get
            {
                return this.pfField;
            }
            set
            {
                this.pfField = value;
            }
        }

        /// <remarks/>
        public VBASAD AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }

        /// <remarks/>
        public VBASAA AA
        {
            get
            {
                return this.aaField;
            }
            set
            {
                this.aaField = value;
            }
        }

        /// <remarks/>
        public string PSU
        {
            get
            {
                return this.pSUField;
            }
            set
            {
                this.pSUField = value;
            }
        }

        /// <remarks/>
        public VBASROR ROR
        {
            get
            {
                return this.rORField;
            }
            set
            {
                this.rORField = value;
            }
        }

        /// <remarks/>
        public VBASRRZ RRZ
        {
            get
            {
                return this.rRZField;
            }
            set
            {
                this.rRZField = value;
            }
        }

        /// <remarks/>
        public VBASKPP KPP
        {
            get
            {
                return this.kPPField;
            }
            set
            {
                this.kPPField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("NACE", IsNullable = false)]
        public VBASNACE[] Nace
        {
            get
            {
                return this.naceField;
            }
            set
            {
                this.naceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("PP", IsNullable = false)]
        public VBASPP[] PPI
        {
            get
            {
                return this.pPIField;
            }
            set
            {
                this.pPIField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Obor_cinnosti", IsNullable = false)]
        public VBASObor_cinnosti[] Obory_cinnosti
        {
            get
            {
                return this.obory_cinnostiField;
            }
            set
            {
                this.obory_cinnostiField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASICO
    {

        private string zdrojField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zdroj
        {
            get
            {
                return this.zdrojField;
            }
            set
            {
                this.zdrojField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASDIC
    {

        private string zdrojField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zdroj
        {
            get
            {
                return this.zdrojField;
            }
            set
            {
                this.zdrojField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASOF
    {

        private string zdrojField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zdroj
        {
            get
            {
                return this.zdrojField;
            }
            set
            {
                this.zdrojField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASPF
    {

        private byte kPFField;

        private string nPFField;

        private string zdrojField;

        /// <remarks/>
        public byte KPF
        {
            get
            {
                return this.kPFField;
            }
            set
            {
                this.kPFField = value;
            }
        }

        /// <remarks/>
        public string NPF
        {
            get
            {
                return this.nPFField;
            }
            set
            {
                this.nPFField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zdroj
        {
            get
            {
                return this.zdrojField;
            }
            set
            {
                this.zdrojField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASAD
    {

        private string ucField;

        private string pbField;

        private string zdrojField;

        /// <remarks/>
        public string UC
        {
            get
            {
                return this.ucField;
            }
            set
            {
                this.ucField = value;
            }
        }

        /// <remarks/>
        public string PB
        {
            get
            {
                return this.pbField;
            }
            set
            {
                this.pbField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zdroj
        {
            get
            {
                return this.zdrojField;
            }
            set
            {
                this.zdrojField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASAA
    {

        private uint iDAField;

        private byte ksField;

        private string nsField;

        private string nField;

        private string nCOField;

        private string nMCField;

        private string nuField;

        private ushort cdField;

        private byte tCDField;

        private byte coField;

        private ushort pSCField;

        private VBASAAAU auField;

        private string zdrojField;

        /// <remarks/>
        public uint IDA
        {
            get
            {
                return this.iDAField;
            }
            set
            {
                this.iDAField = value;
            }
        }

        /// <remarks/>
        public byte KS
        {
            get
            {
                return this.ksField;
            }
            set
            {
                this.ksField = value;
            }
        }

        /// <remarks/>
        public string NS
        {
            get
            {
                return this.nsField;
            }
            set
            {
                this.nsField = value;
            }
        }

        /// <remarks/>
        public string N
        {
            get
            {
                return this.nField;
            }
            set
            {
                this.nField = value;
            }
        }

        /// <remarks/>
        public string NCO
        {
            get
            {
                return this.nCOField;
            }
            set
            {
                this.nCOField = value;
            }
        }

        /// <remarks/>
        public string NMC
        {
            get
            {
                return this.nMCField;
            }
            set
            {
                this.nMCField = value;
            }
        }

        /// <remarks/>
        public string NU
        {
            get
            {
                return this.nuField;
            }
            set
            {
                this.nuField = value;
            }
        }

        /// <remarks/>
        public ushort CD
        {
            get
            {
                return this.cdField;
            }
            set
            {
                this.cdField = value;
            }
        }

        /// <remarks/>
        public byte TCD
        {
            get
            {
                return this.tCDField;
            }
            set
            {
                this.tCDField = value;
            }
        }

        /// <remarks/>
        public byte CO
        {
            get
            {
                return this.coField;
            }
            set
            {
                this.coField = value;
            }
        }

        /// <remarks/>
        public ushort PSC
        {
            get
            {
                return this.pSCField;
            }
            set
            {
                this.pSCField = value;
            }
        }

        /// <remarks/>
        public VBASAAAU AU
        {
            get
            {
                return this.auField;
            }
            set
            {
                this.auField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zdroj
        {
            get
            {
                return this.zdrojField;
            }
            set
            {
                this.zdrojField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASAAAU
    {

        private byte kOLField;

        private byte kkField;

        private ushort kOKField;

        private uint koField;

        private byte kPOField;

        private byte knField;

        private uint kCOField;

        private uint kMCField;

        private ushort pSCField;

        private uint kULField;

        private ushort cdField;

        private byte tCDField;

        private byte coField;

        private uint kaField;

        private uint kOBField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public byte KOL
        {
            get
            {
                return this.kOLField;
            }
            set
            {
                this.kOLField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public byte KK
        {
            get
            {
                return this.kkField;
            }
            set
            {
                this.kkField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public ushort KOK
        {
            get
            {
                return this.kOKField;
            }
            set
            {
                this.kOKField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public uint KO
        {
            get
            {
                return this.koField;
            }
            set
            {
                this.koField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public byte KPO
        {
            get
            {
                return this.kPOField;
            }
            set
            {
                this.kPOField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public byte KN
        {
            get
            {
                return this.knField;
            }
            set
            {
                this.knField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public uint KCO
        {
            get
            {
                return this.kCOField;
            }
            set
            {
                this.kCOField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public uint KMC
        {
            get
            {
                return this.kMCField;
            }
            set
            {
                this.kMCField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public ushort PSC
        {
            get
            {
                return this.pSCField;
            }
            set
            {
                this.pSCField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public uint KUL
        {
            get
            {
                return this.kULField;
            }
            set
            {
                this.kULField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public ushort CD
        {
            get
            {
                return this.cdField;
            }
            set
            {
                this.cdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public byte TCD
        {
            get
            {
                return this.tCDField;
            }
            set
            {
                this.tCDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public byte CO
        {
            get
            {
                return this.coField;
            }
            set
            {
                this.coField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public uint KA
        {
            get
            {
                return this.kaField;
            }
            set
            {
                this.kaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/uvis_datatypes/v_1.0.3")]
        public uint KOB
        {
            get
            {
                return this.kOBField;
            }
            set
            {
                this.kOBField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASROR
    {

        private VBASRORSZ szField;

        private VBASRORSOR sORField;

        /// <remarks/>
        public VBASRORSZ SZ
        {
            get
            {
                return this.szField;
            }
            set
            {
                this.szField = value;
            }
        }

        /// <remarks/>
        public VBASRORSOR SOR
        {
            get
            {
                return this.sORField;
            }
            set
            {
                this.sORField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASRORSZ
    {

        private VBASRORSZSD sdField;

        private string ovField;

        /// <remarks/>
        public VBASRORSZSD SD
        {
            get
            {
                return this.sdField;
            }
            set
            {
                this.sdField = value;
            }
        }

        /// <remarks/>
        public string OV
        {
            get
            {
                return this.ovField;
            }
            set
            {
                this.ovField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASRORSZSD
    {

        private byte kField;

        private string tField;

        /// <remarks/>
        public byte K
        {
            get
            {
                return this.kField;
            }
            set
            {
                this.kField = value;
            }
        }

        /// <remarks/>
        public string T
        {
            get
            {
                return this.tField;
            }
            set
            {
                this.tField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASRORSOR
    {

        private string sSUField;

        private VBASRORSORKKZ kKZField;

        private VBASRORSORVY vyField;

        private VBASRORSORZAM zAMField;

        private VBASRORSORLI liField;

        /// <remarks/>
        public string SSU
        {
            get
            {
                return this.sSUField;
            }
            set
            {
                this.sSUField = value;
            }
        }

        /// <remarks/>
        public VBASRORSORKKZ KKZ
        {
            get
            {
                return this.kKZField;
            }
            set
            {
                this.kKZField = value;
            }
        }

        /// <remarks/>
        public VBASRORSORVY VY
        {
            get
            {
                return this.vyField;
            }
            set
            {
                this.vyField = value;
            }
        }

        /// <remarks/>
        public VBASRORSORZAM ZAM
        {
            get
            {
                return this.zAMField;
            }
            set
            {
                this.zAMField = value;
            }
        }

        /// <remarks/>
        public VBASRORSORLI LI
        {
            get
            {
                return this.liField;
            }
            set
            {
                this.liField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASRORSORKKZ
    {

        private byte kField;

        /// <remarks/>
        public byte K
        {
            get
            {
                return this.kField;
            }
            set
            {
                this.kField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASRORSORVY
    {

        private byte kField;

        /// <remarks/>
        public byte K
        {
            get
            {
                return this.kField;
            }
            set
            {
                this.kField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASRORSORZAM
    {

        private byte kField;

        /// <remarks/>
        public byte K
        {
            get
            {
                return this.kField;
            }
            set
            {
                this.kField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASRORSORLI
    {

        private byte kField;

        /// <remarks/>
        public byte K
        {
            get
            {
                return this.kField;
            }
            set
            {
                this.kField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASRRZ
    {

        private VBASRRZZU zuField;

        private VBASRRZFU fuField;

        /// <remarks/>
        public VBASRRZZU ZU
        {
            get
            {
                return this.zuField;
            }
            set
            {
                this.zuField = value;
            }
        }

        /// <remarks/>
        public VBASRRZFU FU
        {
            get
            {
                return this.fuField;
            }
            set
            {
                this.fuField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASRRZZU
    {

        private uint kZUField;

        private string nZUField;

        /// <remarks/>
        public uint KZU
        {
            get
            {
                return this.kZUField;
            }
            set
            {
                this.kZUField = value;
            }
        }

        /// <remarks/>
        public string NZU
        {
            get
            {
                return this.nZUField;
            }
            set
            {
                this.nZUField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASRRZFU
    {

        private byte kFUField;

        private string nFUField;

        /// <remarks/>
        public byte KFU
        {
            get
            {
                return this.kFUField;
            }
            set
            {
                this.kFUField = value;
            }
        }

        /// <remarks/>
        public string NFU
        {
            get
            {
                return this.nFUField;
            }
            set
            {
                this.nFUField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASKPP
    {

        private string zdrojField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zdroj
        {
            get
            {
                return this.zdrojField;
            }
            set
            {
                this.zdrojField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASNACE
    {

        private string zdrojField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zdroj
        {
            get
            {
                return this.zdrojField;
            }
            set
            {
                this.zdrojField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASPP
    {

        private string[] tField;

        private string zdrojField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("T")]
        public string[] T
        {
            get
            {
                return this.tField;
            }
            set
            {
                this.tField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zdroj
        {
            get
            {
                return this.zdrojField;
            }
            set
            {
                this.zdrojField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3")]
    public partial class VBASObor_cinnosti
    {

        private string kField;

        private string tField;

        /// <remarks/>
        public string K
        {
            get
            {
                return this.kField;
            }
            set
            {
                this.kField = value;
            }
        }

        /// <remarks/>
        public string T
        {
            get
            {
                return this.tField;
            }
            set
            {
                this.tField = value;
            }
        }
    }




}