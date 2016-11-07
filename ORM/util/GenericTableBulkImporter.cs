using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;
using ORM.generator;
using System.Data;
using ORM.exceptions;
using System.IO;
using ORM.db_store;

namespace ORM.Util
{
    public class GenericTableBulkImporter : CSVBulkImporter
    {
        /// <summary>
        /// file must contain all columns and in the same order as the table
        /// </summary>
        /// <param name="genericTableRow"></param>
        public GenericTableBulkImporter(TableRow genericTableRow) : this(null, genericTableRow) { }
        /// <summary>
        /// file may contain less columns and in any order, fileMap will specify the columnName=indexInTheFile
        /// </summary>
        public GenericTableBulkImporter(IEnumerable<string> columns, TableRow genericTableRow)
        {
            this.fileMap = new EDictionary<string, int>();
            IEnumerable<string> cols;

            if (columns == null)
            {
                cols = (IEnumerable<string>)genericTableRow.fields.Keys;
            }
            else
            {
                cols = columns;
            }

            int i = 0;
            foreach (string col in cols)
            {
                this.fileMap.Add(col, i);
                i++;
            }

            this.databaseTable = genericTableRow.dbObjectName;
            this.db = genericTableRow.db.db;

            tableDef = new TableColumnsWrap(genericTableRow);
        }

        public GenericTableBulkImporter(TableRow genericTableRow, EDictionary<string, int> fileMap)
        {
            this.fileMap = new EDictionary<string, int>();
            if (fileMap == null)
            {
                int i = 0;
                foreach (string col in genericTableRow.fields.Keys)
                {
                    this.fileMap.Add(col, i);
                    i++;
                }
            }
            else
            {
                this.fileMap = fileMap;
            }

            this.databaseTable = genericTableRow.dbObjectName;
            this.db = genericTableRow.db.db;

            tableDef = new TableColumnsWrap(genericTableRow);
        }


        public virtual void importBulk(Table<TableRow> table)
        {
            if (table.Count >= 1)
            {
                importBulk((IEnumerable<TableRow>)table);               
            }
            else
            {
                errMessage = "Table was empty, nothing to import";
            }
        }

        public virtual void importBulk(IEnumerable<TableRow> table)
        {            
            DataAdder adder = delegate(DataTable dt)
            {
                addGenericTableToImport(table, dt);
            };
            this.importBulkTemplate(adder);        
        }

        private void addGenericTableToImport(IEnumerable<TableRow> table, DataTable dt)
        {
            foreach (TableRow tr in table)
            {
                if (!tr.validate())
                {
                    string validationMessages = "";
                    foreach (ValidationException tve in tr.validationExceptions)
                    {
                        validationMessages += StringUtil.CRLF + tve.Message;
                    }
                    throw new InvalidDataException(validationMessages);
                }

                DataRow dtr = dt.NewRow();

                foreach (string column in this.fileMap.Keys)
                {
                    object val = tr.fields[column].value;
                    if (columnValidateAndReturnDBValue != null)
                    {
                        val = columnValidateAndReturnDBValue(column, val, null);
                    }
                    dtr[column] = val != null ? val : DBNull.Value;
                }

                dt.Rows.Add(dtr);
            }

            dt.AcceptChanges();
        }


    }


}
