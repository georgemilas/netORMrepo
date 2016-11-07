using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace EM.Collections
{
    //where T : class       -> ref type
    //where T : struct      -> value type
    //where T : new()       -> supports no param constructor
    //where T : IEnumerable -> implements IEnumerable

    //initialy I had it "where T : class" but later I needed EList<int> 
    [Serializable]
    public class EList<T> : List<T>      //where T : class
    {
        public static EList<T> fromAray(T[] arr) { return EList<T>.fromEnumarable(arr); }
        public static EList<T> fromObjectEnumarable(IEnumerable arr)
        {
            EList<T> res = new EList<T>();
            foreach (object t in arr)
            {
                res.Add((T)t);
            }
            return res;
        }
        public static EList<T> fromEnumarable(IEnumerable<T> arr)
        {
            EList<T> res = new EList<T>();
            foreach (T t in arr)
            { 
                res.Add(t);
            }
            return res;
        }
        public static EList<T> fromObjectAray(object[] arr) 
        {
            EList<T> res = new EList<T>();
            foreach (object t in arr)
            {
                res.Add((T)t);
            }
            return res;
        }

        private static string litteralFixup(string litteral)
        {
            litteral = litteral.Trim();
            if (!(litteral.StartsWith("[") && litteral.EndsWith("]")))
            {
                throw new NotSupportedException("A EList litteral must start and end with [ respective ]");
            }
            litteral = litteral.Substring(1, litteral.Length - 2);   //strip out []
            litteral = litteral.Trim();
            if (!litteral.EndsWith(",")) litteral = litteral + ",";  //every list item must be followed by a comma 
            return litteral;

        }
        
        /// <summary>
        /// ['a value', 'other "value"'  ,   'george's value',
        /// 'one more', 'and more on other line'] ==> EList&lt;string>
        ///  -surround with [] and every item is in sigle quotes with anything inside (even other quotes or)
        /// </summary>
        public static EList<string> fromStr(string litteral)
        {
            litteral = EList<string>.litteralFixup(litteral);

            EList<string> ds = new EList<string>();
            Regex dStrStr = new Regex("\\s*(?<item>'.*?')\\s*,");

            MatchCollection matches = dStrStr.Matches(litteral);
            for (int i = 0; i != matches.Count; ++i)
            {
                ds.Add(matches[i].Groups["item"].ToString().Substring(1, matches[i].Groups["item"].ToString().Length - 2));
            }

            return ds;
        }



        public static EList<object> fromStrWithSubLists(string litteral)
        {
            int idx = -1;
            string curItem="";
            EList<EList<object>> stack = new EList<EList<object>>();
            EList<object> curList = new EList<object>();
            string state = "start";
            
            foreach (char c in litteral)
            {   
                idx += 1; 
                
                if (c == '[')
                {   //start a list
                    state = "in list";
                    stack.Add(curList);
                    curList = new EList<object>();
                    //Console.WriteLine("new list started");
                    continue;
                }
                if (state == "in list" && c == ']')
                {   //finish a list
                    EList<object> doneList = curList;
                    try 
                    { 
                        curList = stack.pop();
                        curList.Add(doneList);
                        //Console.WriteLine("list finished " + doneList.ToString());
                    }
                    catch (IndexOutOfRangeException) 
                    {
                        //we are in the root list wich we finished so we are done
                        //Console.WriteLine("DONE");
                        break;
                    }                    
                    continue;
                }
                if (state != "in item" && c == '\'')
                {   //start a new string item
                    state = "in item";
                    curItem = "";
                    //Console.WriteLine("new item started");
                    continue;
                }
                
                if (state == "in item" && c == '\'' &&   //look ahed if is finished
                    (litteral.Substring(idx + 1).TrimStart().Substring(0, 1) == "," || 
                     litteral.Substring(idx + 1).TrimStart().Substring(0, 1) == "]"))
                {   //finish a string item
                    state = "in list";
                    curList.Add(curItem);
                    //Console.WriteLine("item finished " + curItem);
                    continue;
                }
                if (state == "in list" && c == ',')
                {
                    //just move ahed
                    continue;
                }
                if (state == "in item")
                {
                    curItem += c.ToString();
                    continue;
                }

            }
            return curList;
        }

        
        public EList<Object> toObjectList()
        {
            EList<Object> res = new EList<object>();
            foreach (T itm in this)
            {
                res.Add((Object)itm);
            }
            return res;
        }

        /// <summary>
        /// - returns a representaion something like: (itm1, itm2, itm3)  
        ///     with the parantheses and comma and space after comma
        /// - use ToString(false) to return without paranteses: itm1, itm2, itm3
        /// - for more flexibility use CSV.toCsvLine(this)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString(true);    
        }
        
        public string ToString(bool paranteses, string separator = ",")
        {
            string res = CSV.toCsvLine(this, separator);

            if (paranteses)
            {
                res = "(" + res + ")";
            }
            return res;
        }

        public EList<T> intersection(T[] arr)
        {
            return this.intersection(fromAray(arr));
        }
        public EList<T> intersection(List<T> lst2)
        {
            EList<T> res = new EList<T>();
            
            foreach (T itm in lst2)
            {
                if (this.Contains(itm))
                {
                    res.Add(itm);
                }
            }
            return res;
        }

        /// <summary>
        /// - return what in this and not in that AND ALSO what in that and not in this
        /// - this.union(other).left_diference(this.intersection(other))
        /// </summary>
        public EList<T> diference(T[] arr)
        {
            return this.diference(fromAray(arr));
        }
        public EList<T> diference(List<T> lst2)
        {
            EList<T> inters = this.intersection(lst2);
            EList<T> uni = this.union(lst2);
            return uni.left_diference(inters);
        }
        /// <summary>
        /// return what in this and not in that    (but not what in that and not in this)
        /// </summary>
        public EList<T> left_diference(List<T> lst2)
        {
            EList<T> res = new EList<T>();
            foreach (T itm in this)
            {
                if (!lst2.Contains(itm))
                {
                    res.Add(itm);
                }
            }
            return res;
        }

        public EList<T> union(T[] arr)
        {
            return this.union(fromAray(arr));
        }
        
        public EList<T> union(List<T> lst2)
        {
            EList<T> res = this.copy();

            foreach (T itm in lst2)
            {
                if (!res.Contains(itm))
                {
                    res.Add(itm);
                }
            }
            return res;
            
        }

        public new EList<T> GetRange(int idx, int count)
        {
            EList<T> res = new EList<T>();
            res.AddRange(base.GetRange(idx, count));
            return res;

        }

        //filter => FindAll, map => ForEach
        public delegate T ReduceAction(T t1, T acum);
        public T reduce(ReduceAction reducer)
        {
            return reduce(reducer, default(T));
        }
        public T reduce(ReduceAction reducer, T acum)
        {
            if (this.Count == 0)
            {
                return default(T); // null;
            }

            if (this.Count == 1)
            {
                return this[0];
            }

            T res = acum;

            lock (this)  //prevent add, delete for the time of this loop
            {
                for (int i = 0; i <= this.Count - 1; i++)
                {
                    res = reducer(this[i], res);
                }
            }
            return res;
        }

        /// <summary>
        /// - like Clone but we don't implement ICloanable because then all items in the list must implement it
        ///   and we really don't need that (We want to be able to say EList&lt;FileInfo&gt; without subclassing FileInfo)
        /// - it's 1/2 of clone (not a deep copy) because we copy references into a new list but they are pointing to 
        ///   the same old items
        /// - it's implemented as GetRange(0, len)
        /// </summary>
        public EList<T> copy()
        {   
            lock (this)
            {
                if (this.Count > 0)
                {
                    return GetRange(0, this.Count);
                }
                return new EList<T>();
            }
        }

        /// <summary>
        /// - [1,2,3,4,5,6,7,8,9,10].unzip(3) -> [(1,2,3),(4,5,6),(7,8,9), (10,)]
        /// </summary>
        public EList<EList<T>> unzip(int nr)
        {
            EList<T> lst = new EList<T>(); 
            EList<EList<T>> mainlst = new EList<EList<T>>();
            foreach(T itm in this)
            {
                if (lst.Count < nr) 
                {
                    lst.Add(itm);
                }
                else 
                {
                    mainlst.Add(lst);
                    lst = new EList<T>();
                    lst.Add(itm);
                }
            }
            if (lst.Count > 0)
            {
                mainlst.Add(lst);
            }
            return mainlst;
        }

        /// <summary>
        /// - [1,2,3,4,5,6,7,8,9,10].split(3) -> [(1,2,3),(4,5,6),(7,8,9,10)]
        /// </summary>
        public EList<EList<T>> split(int nr)
        {
            int lists = this.Count / nr;
            EList<T> lst = new EList<T>();
            EList<EList<T>> mainlst = new EList<EList<T>>();
            foreach (T itm in this)
            {
                if (lst.Count < lists || 
                    (lst.Count >= lists && mainlst.Count >= lists)
                   )
                {
                    lst.Add(itm);
                }
                else
                {
                    mainlst.Add(lst);
                    lst = new EList<T>();
                    lst.Add(itm);
                }
            }
            if (lst.Count > 0)
            {
                mainlst.Add(lst);
            }
            return mainlst;
        }

        /// <summary>
        /// [1,2,3,4,5].join(" | ") -> "1 | 2 | 3 | 4 | 5"
        /// </summary>
        public string join(string separator)
        {
            StringBuilder res = new StringBuilder();
            for (int i=0; i<this.Count;i++)
            {
                if (i != this.Count - 1) res.Append(this[i].ToString() + separator);
                else res.Append(this[i].ToString());
            }
            return res.ToString();
        }

        
        public EList<T> extractRange(int idx, int count)
        {
            EList<T> res;
            lock (this)
            {
                res = this.GetRange(idx, count);
                this.RemoveRange(idx, count);
            }
            return res;
        }
        
        /// <summary>
        ///     Adds the item to the list and return the list itself.
        ///     => this.chainAdd(itm1).chainAdd(itm2).chainAdd(itm3);
        /// </summary>
        /// <param name="itm"></param>
        /// <returns>this</returns>
        public EList<T> chainAdd(T itm) 
        {
            this.Add(itm);
            return this;
        }

        /// <summary>
        /// use the list like a stack this.Add & this.pop
        /// </summary>
        public T pop()
        {
            if (this.Count > 0)
            {
                T itm = this[this.Count - 1];
                this.Remove(itm);
                return itm;
            }
            else
            {
                throw new IndexOutOfRangeException("The list is empty");
            }
        }


        /// <summary>
        /// - return the idx's element from the list or if no such index exists then return the defValue supplied
        /// - like this[idx] but instead of IndexOutOfBounds Exception, return a default value
        /// </summary>
        public T get(int idx, T defValue)
        {
            if (this.Count > idx && idx >=0 )
            {
                return this[idx];
            }
            else
            {
                return defValue;
            }
        }


        /// <summary>
        /// - see slice(ixFrom, null)
        /// </summary>
        public EList<T> slice(int idxFrom) 
        {
            return slice(idxFrom, null);
        }
        
        /// <summary>
        /// - like the slice operator in python
        /// - lst = [1,2,3,4,5,6,7]
        ///     lst.slice(3, null) => [4,5,6,7]
        ///     lst.slice(-3, null) => [5,6,7]
        ///     lst.slice(20, null) => []
        ///     lst.slice(-20, null) => [1,2,3,4,5,6,7]
        ///  
        ///     lst.slice(null, 3) => [1,2,3]
        ///     lst.slice(null, -3) => [1,2,3,4]
        ///     lst.slice(null, 20) => [1,2,3,4,5,6,7]
        ///     lst.slice(null, -20) => []
        /// 
        ///     lst.slice(3, 4) => [4]
        ///     lst.slice(-3, -1) => [5,6]
        ///     lst.slice(-3, -4) => []
        ///     lst.slice(3, 20) => [4,5,6,7]
        /// </summary>
        public EList<T> slice(int? idxFrom, int? idxTo)
        {
            if (idxFrom == null) { idxFrom = 0; }
            if (idxTo == null) { idxTo = this.Count; }

            if (idxFrom < 0)
            {
                idxFrom = this.Count + idxFrom;
                if (idxFrom < 0) idxFrom = 0;
            }
            if (idxTo < 0)
            {
                idxTo = this.Count + idxTo;
                if (idxTo < 0) idxTo = 0;
            }

            int cnt = (int)idxTo;
            if (cnt > this.Count) cnt = this.Count;
            cnt = cnt - (int)idxFrom;
            if (cnt < 0) cnt = 0;                
            
            if ((int)idxFrom >= this.Count) return new EList<T>();

            return this.GetRange((int)idxFrom, cnt);    //all to the end
        }
        

    }

}
