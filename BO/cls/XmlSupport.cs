﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BO.CLS
{
    public class XmlSupport
    {
        private XmlWriter _wr { get; set; }
        private string _XmlFullPath { get; set; }
        public XmlSupport(string strXmlFullPath,string encoding=null)
        {
            _XmlFullPath = strXmlFullPath;
            XmlWriterSettings settings = new XmlWriterSettings() { CloseOutput = true, Indent = true };
            if (encoding == null)
            {
                settings.Encoding = System.Text.Encoding.UTF8;
            }
            else
            {
                settings.Encoding = System.Text.Encoding.GetEncoding(encoding);
            }

            _wr = XmlWriter.Create(_XmlFullPath, settings);
        }


        public void wstart(string strStartElementName)
        {
            _wr.WriteStartElement(strStartElementName);
        }
        public void wstart(string strStartElementName, string ns)
        {
            _wr.WriteStartElement(strStartElementName, ns);
        }
        public void wend(int krat = 1)
        {
            for (int i = 1; i <= krat; i++)
            {
                _wr.WriteEndElement();
            }

        }

        public void oneattribute(string strName, string strValue)
        {
            _wr.WriteAttributeString(strName, strValue);

                       
        }
        public void oneattribute(string strPrefix,string strName,string strNs,string strValue)
        {
            _wr.WriteAttributeString(strPrefix, strName, null, strValue);
        }
        public void purestring(string s)
        {
            _wr.WriteString(s);
        }
        public void wss(string strElement, string s)
        {
            _wr.WriteElementString(strElement, s);
        }

        public void wsdate(string strElement, DateTime d)
        {
            _wr.WriteElementString(strElement, DAT(d));
        }
        public void wsdatetime(string strElement, DateTime d)
        {
            _wr.WriteElementString(strElement, DATISO(d));
        }
        public void wsnum(string strElement, double n)
        {
            _wr.WriteElementString(strElement, NUM(n));
        }
        public void wsint(string strElement, int n)
        {
            _wr.WriteElementString(strElement, n.ToString());
        }
        public void wsbool(string strElement, bool b)
        {
            if (b)
            {
                _wr.WriteElementString(strElement, "true");
            }
            else
            {
                _wr.WriteElementString(strElement, "false");
            }

        }

        public void flushandclose()
        {
            _wr.Flush();
            _wr.Close();
            _wr.Dispose();
            
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
