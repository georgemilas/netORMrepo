using System;
using EM.Collections;
using ORM.db_store;

namespace ORM.generator
{
    public class DataSetSP : GenTable
    {
        public EDictionary<int, TableName> tables { get; set; }
        public DataSetSP(string dotString)
            : base(dotString)
        {
            tables = new EDictionary<int, TableName>();
        }
    }
}
