using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;
using ORM.db_store;
using System.Collections;

namespace ORM
{
    [Serializable]
    public class RelationsCollection : IList<DBRelation>
    {
        public TableName table { get; set; }
        public string tableLookup = "tableThere";

        public RelationsCollection()
        {
            this.tableLookup = "tableThere";
        }

        private EList<DBRelation> _collection = new EList<DBRelation>();
        protected virtual IList<DBRelation> collection
        {
            get { return _collection; }        
        }

        public EList<DBRelation> getAllRelationsByTable(string table)
        {
            EList<DBRelation> res = new EList<DBRelation>();
            foreach (DBRelation r in this)
            {
                if (tableLookup == "tableThere" && r.tableThere.table == table)
                {
                    res.Add(r);
                }
                if (tableLookup == "tableHere" && r.tableHere.table == table)
                {
                    res.Add(r);
                }
            }
            return res;
        }

        /// <summary>
        /// return first relation that matches tableName
        /// </summary>
        public DBRelation getRelationByTable(string table)
        {
            foreach (DBRelation r in this)
            {
                if (tableLookup == "tableThere" && r.tableThere.table == table)
                {
                    return r;
                }
                if (tableLookup == "tableHere" && r.tableHere.table == table)
                {
                    return r;
                }
            }
            throw new IndexOutOfRangeException("No such relation was found rel.<tableLookup>.table == " + table);        
        }



        public DBRelation getRelationByName(string relationName)
        {
            foreach (DBRelation r in this)
            {
                if (r.relationName == relationName)
                {
                    return r;
                }
            }
            throw new IndexOutOfRangeException("No such relation was found rel.relationName == " + relationName);
        }


        #region IList<DBRelation> Members

        public int IndexOf(DBRelation item)
        {
            return this.collection.IndexOf(item);
        }

        public void Insert(int index, DBRelation item)
        {
            this.collection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.collection.RemoveAt(index);
        }

        public DBRelation this[int index]
        {
            get
            {
                return this.collection[index];
            }
            set
            {
                this.collection[index] = value;
            }
        }

        #endregion

        #region ICollection<DBRelation> Members

        public void Add(DBRelation item)
        {
            this.collection.Add(item);
        }

        public void Clear()
        {
            this.collection.Clear();
        }

        public bool Contains(DBRelation item)
        {
            return this.collection.Contains(item);
        }

        public void CopyTo(DBRelation[] array, int arrayIndex)
        {
            this.collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return this.collection.IsReadOnly; }
        }

        public bool Remove(DBRelation item)
        {
            return this.collection.Remove(item);
        }

        #endregion

        #region IEnumerable<DBRelation> Members

        public IEnumerator<DBRelation> GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.collection).GetEnumerator();
        }

        #endregion
    }
}
