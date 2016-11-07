using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;
using ORM.db_store;

namespace ORM
{
    [Serializable]
    public class FKInfo : RelationsCollection
    {
        public FKInfo(): base()
        {
            this.tableLookup = "tableThere";
        }

        public FKInfo(TableName table) : this()
        {
            this.table  = table;
        }


        private ESet<DBRelation> _collection = new ESet<DBRelation>();
        protected override IList<DBRelation> collection
        {
            get { return _collection; }
        }        
    }
}
