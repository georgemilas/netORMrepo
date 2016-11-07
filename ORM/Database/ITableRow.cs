using System.Data;
using EM.Collections;
using EM.DB;
using ORM.db_store;
using ORM.DBFields;
using ORM.exceptions;
using System.Data.Common;
using System.Collections.Generic;

namespace ORM
{
    public interface ITableRow
    {
        TableName dbObjectName { get; set; }
        Dictionary<string, GenericField> fields { get; set; }
        PKInfo pk { get; set; }
        FKInfo fk { get; set; }
        OneToManyInfo oneToMany { get; set; }     //meta-data
        EList<TableRow.ValidatorFunc> validators { get; set; }
        bool isReadOnly { get; }
        ORMContext context { get; set; }

        /// <summary>
        /// a list of all validation errors from both table level and all the fields 
        /// </summary>
        ESet<ValidationException> validationExceptions { get; }

        /// <summary>
        /// object encapsulating the list of all validation errors from both table level and all the fields 
        /// </summary>
        TableValidationExceptions tableValidationExceptions { get; }

        bool validate();
        Table<T> getInstancesFromDataTable<T>(DataTable tb) where T : TableRow;
        void setFromOneRowDataTable(DBParams param, DataTable tb);

        /// <summary>
        /// set instance fields values to DataRow values
        /// </summary>
        void setFromDataRow(DataRow row);

        void setFromDataRow(DataRow row, DataColumnCollection columns);
        void setFromDataReader(DbDataReader reader);
    }
}