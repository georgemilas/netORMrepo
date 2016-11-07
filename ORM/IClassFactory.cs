using System;
using System.Collections.Generic;
using System.Text;
using ORM.db_store;

namespace ORM
{
    public interface IClassFactory
    {
        TableRow getInstance(Type type, ORMContext context);
        TableRow getInstance(TableName table, ORMContext context);
    }
}
