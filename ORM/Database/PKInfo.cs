using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;
using ORM.db_store;

namespace ORM
{
    [Serializable]
    /// <summary>
    /// a list of field names making up the primary key in the table
    /// </summary>
    public class PKInfo : EList<string>
    {
        public TableName table { get; set; }
    }
}
