using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EM.Collections
{
    
    /// <summary>
    ///  - an ESet is a EList where we can't have the same item twice
    ///  - uses twice as much space compared to a list because internaly uses a dictionary to asure that 
    ///    an item can not be in twice
    /// </summary>     
    [Serializable]
    public class ESet<T> : EList<T>   //where T : class
    {
        //using a dict to speed up Add-ing items to the list since we must only add if not already there
        //and so we need a lookup before adding so instead of this.Contains(item) which is time consuming 
        //for large lists we will do this.content.ContainsKey which should be much faster
        public EDictionary<T, bool> _content { get; set; }  

        public ESet() 
            : base()
        {
            this._content = new EDictionary<T, bool>();
        }

        public new bool Contains(T itm)
        {
            return this._content.ContainsKey(itm);
        }

        //------------------------------------------------------------------------------------------------------
        //     ADD
        //------------------------------------------------------------------------------------------------------
        public new void Add(T itm)
        {
            if (!this._content.ContainsKey(itm))
            //if (!this.Contains(itm))
            {
                base.Add(itm);
                this._content.Add(itm, true);
            }
        }

        public new void AddRange(IEnumerable<T> coll)
        {
            foreach(T itm in coll)
            {
                this.Add(itm);
            }
        }
        
        //------------------------------------------------------------------------------------------------------
        //     INSERT
        //------------------------------------------------------------------------------------------------------
        public new void Insert(int idx, T itm)
        {
            if (!this._content.ContainsKey(itm))
            {
                base.Insert(idx, itm);
                this._content.Add(itm, true);
            }
        }
        
        public new void InsertRange(int idx, IEnumerable<T> coll)
        {
            int cnt = 0;
            foreach (T itm in coll)
            {
                if (!this._content.ContainsKey(itm))
                {
                    base.Insert(idx + cnt, itm);
                    this._content.Add(itm, true);
                    cnt++;
                }
            }
        }

        //------------------------------------------------------------------------------------------------------
        //     REMOVE
        //------------------------------------------------------------------------------------------------------
        
        public new void Clear()
        {
            base.Clear();
            this._content.Clear();
        }
        
        public new bool Remove(T itm)
        {
            bool res = base.Remove(itm);
            this._content.Remove(itm);
            return res;
        }
        
        public new int RemoveAll(System.Predicate<T> pred)
        {
            int res = base.RemoveAll(pred);
            this._content = new EDictionary<T,bool>();
            foreach (T itm in this)
            {
                this._content.Add(itm, true);
            }
            return res;
        }
        
        public new void RemoveAt(int idx)
        {
            T itm = this[idx];
            this._content.Remove(itm);
            base.RemoveAt(idx);
        }

        public new void RemoveRange(int idx, int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.RemoveAt(idx);
            }
        }


        public static ESet<T> fromList(IEnumerable<T> lst)
        {
            ESet<T> set = new ESet<T>();
            set.AddRange(lst);
            return set;
        }

        public static new ESet<T> fromStr(string litteral)
        {
            ESet<T> set = new ESet<T>();
            set.AddRange((IEnumerable<T>)EList<T>.fromStr(litteral));
            return set;
        }


        


        //------------------------------------------------------------------------------------------------------
        //     LIKE EList but faster
        //------------------------------------------------------------------------------------------------------




        public ESet<T> intersection(ESet<T> lst2)
        {
            ESet<T> res = new ESet<T>();

            foreach (T itm in lst2)
            {
                if (this._content.ContainsKey(itm))
                {
                    res.Add(itm);
                }
            }
            return res;
        }

        public ESet<T> diference(ESet<T> lst2)
        {
            ESet<T> inters = this.intersection(lst2);
            ESet<T> uni = this.union(lst2);
            return uni.left_diference(inters);
        }
        /// <summary>
        /// return what in this and not in that    (but not what in that and not in this)
        /// </summary>
        public ESet<T> left_diference(ESet<T> lst2)
        {
            ESet<T> res = new ESet<T>();
            foreach (T itm in this)
            {
                if (!lst2._content.ContainsKey(itm))
                {
                    res.Add(itm);
                }
            }
            return res;
        }

       
        public ESet<T> union(ESet<T> lst2)
        {
            ESet<T> res = this.copy();

            foreach (T itm in lst2)
            {
                if (!res._content.ContainsKey(itm))
                {
                    res.Add(itm);
                }
            }
            return res;
        }


        /// <summary>
        /// - like Clone but we don't implement ICloanable because then all items in the list must implement it
        ///   and we really don't need that (We want to be able to say ESet&lt;FileInfo&gt; without subclassing FileInfo)
        /// - it's 1/2 of clone (not a deep copy) because we copy references into a new list but they are pointing to 
        ///   the same old items
        /// </summary>
        public new ESet<T> copy()
        {
            return ESet<T>.fromList(this);
        }






    }

}
