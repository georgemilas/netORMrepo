using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EM.Collections
{
    public static class StringExtensions
    {
        /// <summary>
        /// see EList.slice (python like slice)
        /// </summary>
        public static string slice(this string txt, int idxFrom)
        {
            return StringUtil.slice(txt, idxFrom);
        }

        /// <summary>
        /// see EList.slice (python like slice)
        /// </summary>
        public static string slice(this string txt, int? idxFrom, int? idxTo)
        {
            return StringUtil.slice(txt, idxFrom, idxTo);
        }

        /// <summary>
        /// see EList.slice (python like slice)
        /// </summary>
        public static string slice(this string txt, int idxFrom, int idxTo)
        {
            return StringUtil.slice(txt, idxFrom, idxTo);
        }
    }
}
