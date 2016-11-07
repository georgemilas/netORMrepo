using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;
using ORM.db_store;

namespace ORM
{   
    [Serializable]
    public class OneToManyInfo : RelationsCollection
    {
        public OneToManyInfo(): base()
        {
            this.tableLookup = "tableHere";
        }

    }
}
