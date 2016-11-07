using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using EM.Collections;
using System.Linq;

namespace EM.DB.Index.Generic
{

    public class GenericDictIndex<TD> : GenericLookup<string, TD>
    {
        public GenericDictIndex(IEnumerable<TD> tbl, string fieldName) : this(tbl, fieldName, false) { }
        public GenericDictIndex(IEnumerable<TD> tbl, string fieldName, bool toUpper) : base(tbl, new DictKeyProvider<TD>(fieldName, toUpper, new GenericPropertyGetter<TD>())) { }
    }

    public class GenericDictIndexUnique<TD> : GenericLookup<string, TD>
    {
        public GenericDictIndexUnique(IEnumerable<TD> tbl, string fieldName) : this(tbl, fieldName, false) { }
        public GenericDictIndexUnique(IEnumerable<TD> tbl, string fieldName, bool toUpper) : base(tbl, new UniqueDictKeyProvider<TD>(fieldName, toUpper, new GenericPropertyGetter<TD>())) { }

        /// <summary>
        /// returns null if item is not found and throws exception if more than one item exists
        /// </summary>
        public TD Find(object value)
        {
            return this.Select(new[] { value }).SingleOrDefault();
        }
    }



    public class GenericDictIndexMultiField<TD> : GenericLookup<MultiFieldDictKey, TD>
    {
        public GenericDictIndexMultiField(IEnumerable<TD> tbl, IEnumerable<string> fieldNames) : this(tbl, fieldNames, false) { }
        public GenericDictIndexMultiField(IEnumerable<TD> tbl, IEnumerable<string> fieldNames, bool toUpper) : base(tbl, new MultiFieldDictKeyProvider<TD>(fieldNames, toUpper, new GenericPropertyGetter<TD>())) { }
    }

    public class GenericDictIndexMultiFieldUnique<TD> : GenericLookup<MultiFieldDictKey, TD>
    {
        public GenericDictIndexMultiFieldUnique(IEnumerable<TD> tbl, IEnumerable<string> fieldNames) : this(tbl, fieldNames, false) { }
        public GenericDictIndexMultiFieldUnique(IEnumerable<TD> tbl, IEnumerable<string> fieldNames, bool toUpper) : base(tbl, new UniqueMultiFieldDictKeyProvider<TD>(fieldNames, toUpper, new GenericPropertyGetter<TD>())) { }

        /// <summary>
        /// returns null if item is not found and throws exception if more than one item exists
        /// </summary>
        public TD Find(IEnumerable<object> values)
        {
            return this.Select(values).SingleOrDefault();
        }
    }



}
