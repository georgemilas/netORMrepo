using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace EM.Util
{

    /// <summary>
    /// A class with an XmlDocument as the State and abstractions for stringifying and other helpers
    /// </summary>
    public class XMLBuilder
    {
        protected XmlDocument doc;

        public XMLBuilder()
        {
            this.doc = new XmlDocument();            
        }
        
        public XmlElement CreateElement(string name)
        {
            return this.doc.CreateElement(name);
        }
        public XmlElement CreateElement(string name, string value)
        {
            XmlElement el = this.doc.CreateElement(name);
            el.InnerText = value;
            return el;
        }

        public XmlDocument GetXMLDocument()
        {
            return doc;
        }

        /// <summary>
        /// write the XML string indented and in the encoding specifiend in the xml tag: &lt;?xml encoding="XXX"> or if none then Use UTF8
        /// </summary>
        public string GetXMLString()
        {
            return GetXMLString(true);
        }
        /// <summary>
        /// write the XML string in the encoding specifiend in the xml tag: &lt;?xml encoding="XXX"> or if none then Use UTF8
        /// </summary>
        public string GetXMLString(bool indent)
        {
            XmlWriterSettings cfg = new XmlWriterSettings();
            cfg.Indent = indent;
            return GetXMLString(cfg);
        }
        /// <summary>
        /// write the XML string in the encoding specifiend in the xml tag: &lt;?xml encoding="XXX"> or if none then Use UTF8
        /// </summary>
        public string GetXMLString(XmlWriterSettings cfg)
        {
            Encoding enc = Encoding.UTF8;
            try
            {
                if (doc.FirstChild != null && doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                {
                    XmlDeclaration decl = (XmlDeclaration)doc.FirstChild;                
                    enc = Encoding.GetEncoding(decl.Encoding);                
                }
            }
            catch (Exception) { }

            return GetXMLString(cfg, enc);            
        }

        public string GetXMLString(XmlWriterSettings cfg, Encoding enc)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriter xw = XmlWriter.Create(ms, cfg);
            this.doc.WriteTo(xw);
            xw.Flush();

            return enc.GetString(ms.ToArray());
        }
    }
}
