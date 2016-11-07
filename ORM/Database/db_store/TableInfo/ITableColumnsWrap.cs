using System;
using System.Collections.Generic;

namespace ORM.db_store
{
    public interface ITableColumnsWrap
    {
        string classParams(string columnName, string className);
        Type dataTableColumnType(string columnName);
        string fieldClassType(string columnName);
        string fieldDotNetType(string columnName);
        IEnumerable<string> getColumns();
        bool isComputed(string columnName);
        bool isRequired(string columnName);
        ColumnAttributes columnAttributes(string columnName);        
    }
}
