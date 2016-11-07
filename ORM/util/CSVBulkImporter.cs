using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using EM.Collections;
using ORM.generator;
using ORM.db_store;
using ORM.db_store.persitence;

namespace ORM.Util
{

    //public class test
    //{
    //    public void tt()
    //    {
    //        ORMContext cx = new SurePayrollORMContext();
    //        TableName table = TableName.fromDotString("dbo.tblFrozenQuarterEA");
    //        EDictionary<string, int> fileMap = new EDictionary<string, int>{
    //            {"BCL_CODE", -1},       //missing from file
    //            {"YEAR_NUM", -1},       //missing from file
    //            {"QUARTER_NUM", -1},    //missing from file    
    //            {"E1_ID", 0},
    //            {"REC_TYPE", 1},
    //            {"CODE_TYPE", 2},
    //            {"CODE", 3},
    //            {"CUR_ACCUM", 4},
    //            {"MTD_ACCUM", 5},
    //            {"QTD_ACCUM", 6},
    //            {"YTD_ACCUM", 7},
    //            {"PRI_ACCUM", 8}
    //        };
    //        CSVBulkImporter importer = new CSVBulkImporter(fileMap, table, cx.db);
    //        importer.columnValueCallback = delegate(string col, ValueParser csvLineParser)
    //         {
    //             if (col == "BCL_CODE") { return "D2MZ"; }   
    //             if (col == "YEAR_NUM") { return 2009; }
    //             if (col == "QUARTER_NUM") { return 3; }
    //             return null;
    //         } ;
    //        importer.importBulk(@"C:\TEST\D2MZQ3EA.TXT");
    //    }
    //}


    public class CSVBulkImporter : DataTableBulkImporter
    {

        protected CSVBulkImporter() { }

        /// <summary>
        /// file may contain less columns and in any order, fileMap will specify the columnName=indexInTheFile
        /// if you specify columnName=-1 then you must provide a value for those columns via columnValueCallback  delegate
        /// </summary>
        public CSVBulkImporter(EDictionary<string, int> fileMap, TableName databaseTable, GenericDatabase db)
        { 
            this.fileMap = fileMap;
            this.databaseTable = databaseTable;
            
            TableColumnsInfo tci = db.columns(this.databaseTable);
            tableDef = new TableColumnsWrap(db, tci);

            this.db = db.db;
        }


        /// <summary>
        /// first line in considered to be a header line so is ignored
        /// </summary>
        public virtual void importBulk(string filePath) { importBulk(CSV.parse(File.OpenText(filePath))); }
        /// <summary>
        /// first line in considered to be a header line so is ignored
        /// </summary>
        public virtual void importBulk(Stream stream) { importBulk(CSV.parse(new StreamReader(stream))); }
        /// <summary>
        /// first line in considered to be a header line so is ignored
        /// </summary>
        public virtual void importBulk(EList<EList<string>> csv)
        {
            if (csv.Count > 1)
            {
                DataAdder adder = delegate(DataTable dt)
                {
                    addCSVToImport(csv, dt);
                };
                importBulkTemplate(adder);
            }
            else
            {
                errMessage = "File was empty, nothing to import";
            }

        }

        private void addCSVToImport(EList<EList<string>> csv, DataTable dt)
        {
            //loop to get values in file
            for (int i = 1; i < csv.Count; i++)     //first line is header
            {
                if (csv[i].ToString(false).Replace(",", "").Trim() == "") continue;     //empty line
                ValueParser p = new ValueParser(i, csv[i], fileMap);

                DataRow dtr = dt.NewRow();

                foreach (string column in this.fileMap.Keys)
                {
                    if (this.fileMap[column] == -1)  //value for column you specify via callback
                    {
                        if (columnValueCallback != null)
                        {
                            dtr[column] = columnValueCallback(column, p);
                        }
                        else
                        {
                            throw new InvalidOperationException("There is no data in the file for column " + column + " and columnValueCallback was not specified");
                        }
                    }
                    else   //value for column is in the file
                    {
                        object val;
                        if (columnValidateAndReturnDBValue != null)
                        {
                            val = p.parse(false, column, dt.Columns[column].DataType);
                            val = columnValidateAndReturnDBValue(column, val, p);
                        }
                        else
                        {
                            val = p.parse(tableDef.isRequired(column), column, dt.Columns[column].DataType);
                        }
                        dtr[column] = val ?? System.DBNull.Value;
                    }
                }

                dt.Rows.Add(dtr);
            }

            dt.AcceptChanges();

        }
    }

}
