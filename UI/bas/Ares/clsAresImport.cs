using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace UI.Ares
{
    public class clsAresImport
    {
        public AresRecord LoadByIco(string ico)
        {
            var url = "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_bas.cgi?ico="+ico;

            System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
            xmldoc.Load(url);

            UI.Ares.Ares_odpovedi response = null;
            XmlSerializer serializer = new XmlSerializer(typeof(UI.Ares.Ares_odpovedi));
            using (XmlReader reader = new XmlNodeReader(xmldoc))
            {
                response = (UI.Ares.Ares_odpovedi)serializer.Deserialize(reader);

                
            }

            var ret = new AresRecord();

            if (response.Odpoved.VBAS == null)
            {
                ret.Error = ico+": záznam v rejstříku nebyl nalezen.";
                return ret;
            }


            var odp = response.Odpoved.VBAS;

            ret.Company = odp.OF.Value;
            ret.DIC = odp.DIC.Value;
            ret.City = odp.AA.N;
            if (!string.IsNullOrEmpty(odp.AA.NMC))
            {
                ret.City = odp.AA.NMC;
            }
            ret.Street = response.Odpoved.VBAS.AD.UC;
            ret.Country = response.Odpoved.VBAS.AA.NS;
            ret.PostCode = response.Odpoved.VBAS.AA.PSC.ToString();


            return ret;
        }
    }
}
