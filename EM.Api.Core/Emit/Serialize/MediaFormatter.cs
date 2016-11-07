using System;
using System.Net.Http.Formatting;

namespace EM.Api.Core.Emit.Serialize
{
    public abstract class MediaFormatter : BufferedMediaTypeFormatter, ICustomSerializerTypeResolver
    {
        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public static string ToCLSCompliantIdentifier(string name)
        {
            //we replace uncompliant chars with unique compliat chars to preserve identifier name uniqueness 
            var clsCompiant =
                name.Replace("`", "a")   //a postrofe
                    .Replace("[", "o")   //o pen bracket
                    .Replace("]", "c")   //c lose bracket
                    .Replace("(", "l")   //l eft paranteses
                    .Replace(")", "r")   //r ight paranteses
                    .Replace(",", "m")   //co mma
                    .Replace("=", "e")   //e qual
                    .Replace(".", "d")   //d ot
                    .Replace("_", "u")   //u nderscore   
                    .Replace("-", "n")   //n egative
                    .Replace("/", "f")   //f orward slash
                    .Replace("\\", "b")  //b ack slash
                    .Replace(":", "t")   //collon (double do t)
                    .Replace(" ", "s");  //s pace
            return clsCompiant;
        }

        public abstract void ResolveType(Type tp);
        public abstract MediaFormatter GetNewFormatterInstance();        

    }
}

