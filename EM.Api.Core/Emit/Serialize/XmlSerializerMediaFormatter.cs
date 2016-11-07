using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace EM.Api.Core.Emit.Serialize
{
    public class XmlSerializerMediaFormatter : MediaFormatter
    {
        //private ESet<ExtraTypeKey> ExtraTypes { get; set; }   
        private HashSet<Type> ExtraTypes { get; set; }    //order of types doesn't matter so we don't need ESet which is slower
        
        public XmlSerializerMediaFormatter()
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
            var xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);
            //var extraTypes = from et in ExtraTypes select et.TypeEmitter.Type;
            var extraTypes = ExtraTypes;

            var serializer = new XmlSerializer(value != null ? value.GetType() : type, extraTypes.ToArray());
            using (var streamWriter = new StreamWriter(writeStream, SupportedEncodings[0]))
            {
                serializer.Serialize(streamWriter, value, xns);
            }
            
        }

        public override object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);
            //var extraTypes = from et in ExtraTypes select et.TypeEmitter.Type;
            var extraTypes = ExtraTypes;
            var serializer = new XmlSerializer(type, extraTypes.ToArray());
            using (var streamReader = new StreamReader(readStream, SupportedEncodings[0]))
            {
                return serializer.Deserialize(streamReader);
            }
            
        }

        public override void ResolveType(Type tp)
        {
            ExtraTypes.Add(tp);
        }


        public override MediaFormatter GetNewFormatterInstance()
        {
            return new XmlSerializerMediaFormatter();
        }
        

    }


  

}