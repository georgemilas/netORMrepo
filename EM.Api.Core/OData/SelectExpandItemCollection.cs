using System.Collections.Generic;
using System.Linq;

namespace EM.Api.Core.OData
{
    public class SelectExpandItemCollection : List<SelectExpandItem>
    {
        public string GetHashString()
        {
            var str = this.OrderBy(i => i.Name).Aggregate("", (acum, itm) => acum + itm.Name + itm.DataType + itm.ItemType);
            return str;
        }
        public override int GetHashCode()
        {
            //var hash = this.Aggregate(0, (acum, item) => acum | item.GetHashCode());  
            //not reliable with the integers binary OR (neither ^ aka XOR for that matter) so we are using string concatenation method instead
            var str = GetHashString();
            var hash = str.GetHashCode();
            return hash;
        }
        public override bool Equals(object obj)
        {
            if (obj is SelectExpandItemCollection)
            {
                var other = obj as SelectExpandItemCollection;
                if (this.Count == other.Count)
                {
                    if (this.Count > 0)
                    {
                        var meOrdered = this.OrderBy(i => i.Name).ToList();
                        var otherOrdered = other.OrderBy(i => i.Name).ToList();

                        for (var i = 0; i <= this.Count - 1; i++)
                        {
                            if (!meOrdered[i].Equals(otherOrdered[i]))
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}