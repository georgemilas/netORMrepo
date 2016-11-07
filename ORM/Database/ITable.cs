using System;
using System.Data;
using ORM.db_store.persitence;
using ORM.exceptions;
using EM.Collections;
using System.Collections.Generic;

namespace ORM
{
    public interface ITable<T> : IList<T> 
        where T : ITableRow
    {
        DataTable adoDataTable { get; set; }
        GenericDatabase db { get; set; }
        string name { get; set; }
        ESet<ValidationException> validationExceptions { get; set; }
    }
}
