using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;
using ORM.db_store;

namespace ORM
{
    public class DBConstraint : EList<string>
    {
        public TableName table;
        public string constraintName;

        /// <summary>
        /// one of  'uniqueue constraint'  or   'uniqueue index'
        /// </summary>
        public string constraintType;  
    }
}
