using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Serialization;
using System.Net.Http.Headers;
using System.Text;
using System.Linq;

namespace EM.Api.Core.Emit.Serialize
{
    public class DataContractXmlSerializerMediaFormatter : MediaFormatter
    {
        //private XmlMediaTypeFormatter Actual { get; set; }
        public HashSet<Type> ExtraTypes { get; private set; }    //order of types doesn't matter so we don't need ESet which is slower

        public DataContractXmlSerializerMediaFormatter()
        {
            //ExtraTypes = new ESet<ExtraTypeKey>();
            ExtraTypes = new HashSet<Type>();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
            SupportedEncodings.Add(new UTF8Encoding(false, true));
            SupportedEncodings.Add(new UnicodeEncoding(false, true, true));
        }
        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            var extraTypes = ExtraTypes;
            
            DataContractSerializer s = new DataContractSerializer(value != null ? value.GetType() : type, extraTypes.ToArray());
            s.WriteObject(writeStream, value);
        }

        public override object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var extraTypes = ExtraTypes;
            DataContractSerializer s = new DataContractSerializer(type, extraTypes.ToArray());
            return s.ReadObject(readStream);
        }

        public override void ResolveType(Type tp)
        {
            ExtraTypes.Add(tp);
        }

        public override MediaFormatter GetNewFormatterInstance()
        {
            return new DataContractXmlSerializerMediaFormatter();
        }        

    }



    

}