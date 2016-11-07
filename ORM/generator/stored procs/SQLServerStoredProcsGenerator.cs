using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ORM.db_store;
using EM.Collections;


namespace ORM.generator
{
    public class SQLServerStoredProcsGenerator: Generator, IStoredProcsGenerator
    {
        private StringBuilder spdrop;
        public SQLServerStoredProcsGenerator() :base() { }

        public void customizeFromOtherGenerator(GeneratorBase g)
        {
            this.db = g.db;
            this.versionNumber = g.versionNumber;
            this.filePathBuilder = g.filePathBuilder;
            this.outputFolder = g.outputFolder;
            this.rootNameSpace = g.rootNameSpace;
            this.rootNameSpaceFolder = g.rootNameSpaceFolder;
            this.isSkipDatabaseObject = g.isSkipDatabaseObject;
            this.IsDoComputeAllNeededTables = g.IsDoComputeAllNeededTables;
        }
       
        public virtual void addStoreProcInit(TableName table, StringBuilder s)
        {
            CRUDBasedStoredProcs crudsp = new CRUDBasedStoredProcs(table, this.versionNumber);

            s.AppendFormat("            this.storedProcedures = new TableRowStoredProcedures(this.dbObjectName, \"CRUD\");" + CRLF);
            s.AppendFormat("            this.storedProcedures.loadByPK.table = \"{0}\";" + CRLF, crudsp.procs.loadByPK.table);            
            s.AppendFormat("            this.storedProcedures.updateByPK.table = \"{0}\";" + CRLF, crudsp.procs.updateByPK.table);            
            s.AppendFormat("            this.storedProcedures.deleteByPK.table = \"{0}\";" + CRLF, crudsp.procs.deleteByPK.table);
            s.AppendFormat("            this.storedProcedures.insert.table = \"{0}\";" + CRLF, crudsp.procs.insert.table);
            s.AppendFormat("            this.storedProcedures.countAll.table = \"{0}\";" + CRLF, crudsp.procs.countAll.table);
        }

        protected virtual void addSPProcStart(TableName proc, StringBuilder s)
        {
            s.AppendFormat("USE [{0}]" + CRLF, proc.catalog);
            s.Append("GO" + CRLF);
            s.AppendFormat("IF OBJECT_ID ( '{0}', 'P' ) IS NOT NULL" + CRLF, proc.sqlFromName);
            s.AppendFormat("DROP PROCEDURE {0};" + CRLF, proc.sqlFromName);
            s.Append("GO" + CRLF);
        }

        protected virtual void addSPExecPermission(TableName proc, StringBuilder s)
        {
            s.Append(CRLF);
            s.Append("GO" + CRLF);
            s.AppendFormat("GRANT  EXECUTE  ON {0}  TO [SPExecUsers]" + CRLF, proc.sqlFromName);
            s.Append("GO" + CRLF);            
        }

        protected virtual void addSPDrop(TableName proc)
        {
            if (this.spdrop == null)
            {
                this.spdrop = new StringBuilder();                
            }
            this.spdrop.AppendFormat("USE [{0}]" + CRLF, proc.catalog);
            this.spdrop.Append("GO" + CRLF);
            this.spdrop.AppendFormat("DROP PROCEDURE {0}" + CRLF, proc.sqlFromName);
            this.spdrop.Append("GO" + CRLF);
        }

        public virtual void buildSPDropAllProcs() 
        {
            if (this.spdrop != null)     //might be null if we ony generate views
            {
                writeFile("01. data - drop all stored procedures", "Other Scripts", "CRUD", PartialType.DB_CRUD,
                          this.spdrop.ToString(), "Data");
            }
        } 

        public virtual void buildSPInsert(TableName table, TableColumnsWrap cw, PKInfo pk)
        {
            CRUDBasedStoredProcs crudsp = new CRUDBasedStoredProcs(table, this.versionNumber);
            
            TableName proc = crudsp.getProc(SPtype.insert);
            EList<string> parmsDeclare = new EList<string>();
            EList<string> parmsInsertName = new EList<string>();
            EList<string> parmsInsertValue = new EList<string>();
            string identityColumn = null;

            foreach (string c in cw.getColumns())
            {
                ColumnAttributes ca = cw.columnAttributes(c);
                if (cw.isComputed(c)) continue;
                if (ca.dbType.ToLower() == "timestamp") continue;    
                
                if ( ca.autoIncrement ) //&& pk.Contains(c))
                {
                    identityColumn = c;
                    continue;
                }
                //if (ca.autoIncrement)  //not part of PK
                //{
                //    continue;
                //}
                string p = getParamDeclarationType(c, ca);
                parmsDeclare.Add(p);
                parmsInsertName.Add("[" + c + "]");
                parmsInsertValue.Add("@" + fixName(c));
            }

            StringBuilder s = new StringBuilder();
            addSPProcStart(proc, s);
            s.AppendFormat(@"
CREATE PROCEDURE {0}
(
   {1}
)
AS
BEGIN
INSERT INTO {2}
           ({3})
     VALUES
           ({4})
", proc.sqlFromName, parmsDeclare.join("," + CRLF), table.sqlFromName, parmsInsertName.join("," + CRLF), parmsInsertValue.join("," + CRLF));
            
            s.Append(CRLF);
            if ( identityColumn!= null )
            {
                //we need the declare trick because SCOPE_IDENTITY() is always decimal and we need to CAST that to actual field type
                ColumnAttributes ca = cw.columnAttributes(identityColumn);
                s.AppendFormat("DECLARE @{0} {1}" + CRLF, fixName(identityColumn), ca.dbType);
                s.AppendFormat("SET @{0} = SCOPE_IDENTITY()" + CRLF, fixName(identityColumn));
                s.AppendFormat("SELECT @{0}" + CRLF, fixName(identityColumn));
            }
            s.Append("END" + CRLF);
            addSPExecPermission(proc, s);
            addSPDrop(proc);

            writeFile(proc, PartialType.DB_CRUD, s.ToString(), "Stored Procedure");
        }

        private string getParamDeclarationType(string c, ColumnAttributes ca)
        {
            string p = "@" + fixName(c) + " " + ca.dbType;
            if (ca.dbType.ToLower().EndsWith("char"))
            {
                if (ca.maxLength == -1) { p += "(MAX)"; }
                else { p += "(" + ca.maxLength + ")"; }
            }
            if (ca.dbType.ToLower().EndsWith("numeric"))
            {
                p += "(" + ca.numericPrecision + "," + ca.numericScale + ")";
            }
            if (ca.dbType.ToLower().EndsWith("decimal"))
            {
                p += "(" + ca.numericPrecision + "," + ca.numericScale + ")";
            }
            if (ca.dbType.ToLower().EndsWith("varbinary"))
            {
                if (ca.maxLength == -1) { p += "(MAX)"; }
                else { p += "(" + ca.maxLength + ")"; }
            }
            return p;
        }

        public virtual void buildSPUpdateByPK(TableName table, TableColumnsWrap cw, PKInfo pk)
        {
            if (pk.Count <= 0 || cw.getColumns().Count() == pk.Count)
            {
                //no PK, nothing to do
                return;
            }

            CRUDBasedStoredProcs crudsp = new CRUDBasedStoredProcs(table, this.versionNumber);
            TableName proc = crudsp.getProc(SPtype.updateByPK);

            EList<string> parmsDeclare = new EList<string>();
            EList<string> parmsUpdate = new EList<string>();

            EList<string> parmsWhere = new EList<string>();
            
            foreach (string c in cw.getColumns())
            {
                ColumnAttributes ca = cw.columnAttributes(c);
                if (cw.isComputed(c)) continue;
                if (ca.dbType.ToLower() == "timestamp") continue;    
                
                string p = getParamDeclarationType(c, ca);

                if (pk.Contains(c))
                {
                    parmsDeclare.Add(p);
                    parmsWhere.Add("[" + c + "] = @" + fixName(c));
                }
                else
                {
                    if (!ca.autoIncrement)  
                    {
                        parmsDeclare.Add(p);
                        parmsUpdate.Add("[" + c + "] = @" + fixName(c));
                    }                    
                }
            }
            
            StringBuilder s = new StringBuilder();
            addSPProcStart(proc, s);
            s.AppendFormat(@"
CREATE PROCEDURE {0} 
(
    {1}
)
AS
BEGIN
UPDATE {2}
SET  {3}
WHERE {4}
", proc.sqlFromName, parmsDeclare.join("," + CRLF), table.sqlFromName, parmsUpdate.join("," + CRLF), parmsWhere.join(" AND " + CRLF));

            s.Append("END" + CRLF);
            addSPExecPermission(proc, s);
            addSPDrop(proc);

            writeFile(proc, PartialType.DB_CRUD, s.ToString(), "Stored Procedure");
        }

        public virtual void buildSPDeleteByPK(TableName table, TableColumnsWrap cw, PKInfo pk)
        {
            CRUDBasedStoredProcs crudsp = new CRUDBasedStoredProcs(table, this.versionNumber);
            TableName proc = crudsp.getProc(SPtype.deleteByPK);
            _buildSPDeleteProcByCustomFields(proc, table, cw, pk);
        }

        public TableName buildSPDeleteByCustomFields(TableName table, TableColumnsWrap cw, DBConstraint constraint)
        {
            CRUDBasedStoredProcs crudsp = new CRUDBasedStoredProcs(table, this.versionNumber);
            TableName proc = crudsp.getRelationDeleteProc(table, constraint);
            _buildSPDeleteProcByCustomFields(proc, table, cw, constraint);
            return proc;
        }

        private void _buildSPDeleteProcByCustomFields(TableName proc, TableName table, TableColumnsWrap cw, List<string> customWhereFields)
        {
            if (customWhereFields.Count <= 0)
            {
                //no fields, nothing to do
                return;
            }


            EList<string> parmsDeclare = new EList<string>();
            EList<string> parmsWhere = new EList<string>();

            foreach (string c in cw.getColumns())
            {
                ColumnAttributes ca = cw.columnAttributes(c);
                if (cw.isComputed(c)) continue;

                string p = getParamDeclarationType(c, ca);

                if (customWhereFields.Contains(c))
                {
                    parmsDeclare.Add(p);
                    parmsWhere.Add("[" + c + "] = @" + fixName(c));
                }
            }

            StringBuilder s = new StringBuilder();
            addSPProcStart(proc, s);
            s.AppendFormat(@"
CREATE PROCEDURE {0}
(
   {1}
)
AS
BEGIN
DELETE FROM {2}
WHERE {3}
", proc.sqlFromName, parmsDeclare.join("," + CRLF), table.sqlFromName, parmsWhere.join(" AND " + CRLF));

            s.Append("END" + CRLF);
            addSPExecPermission(proc, s);
            addSPDrop(proc);

            writeFile(proc, PartialType.DB_CRUD, s.ToString(), "Stored Procedure");
        }


        public virtual void buildSPLoadByPK(TableName table, TableColumnsWrap cw, PKInfo pk)
        {
            if (pk.Count <= 0)
            {
                //no PK, nothing to do
                return;
            }


            CRUDBasedStoredProcs crudsp = new CRUDBasedStoredProcs(table, this.versionNumber);
            TableName proc = crudsp.getProc(SPtype.loadByPK);

            EList<string> parmsDeclare = new EList<string>();

            EList<string> parmsWhere = new EList<string>();

            foreach (string c in cw.getColumns())
            {
                ColumnAttributes ca = cw.columnAttributes(c);
                if (cw.isComputed(c)) continue;

                string p = getParamDeclarationType(c, ca);

                if (pk.Contains(c))
                {
                    parmsDeclare.Add(p);
                    parmsWhere.Add("[" + c + "] = @" + fixName(c));
                }
            }

            StringBuilder s = new StringBuilder();
            addSPProcStart(proc, s);
            s.AppendFormat(@"
CREATE PROCEDURE {0}
(
   {1}
)
AS
BEGIN
SELECT * FROM {2}
WHERE {3}
", proc.sqlFromName, parmsDeclare.join("," + CRLF), table.sqlFromName, parmsWhere.join(" AND " + CRLF));

            s.Append("END" + CRLF);
            addSPExecPermission(proc, s);
            addSPDrop(proc);

            writeFile(proc, PartialType.DB_CRUD, s.ToString(), "Stored Procedure");
        }

        public virtual void buildSPCountAll(TableName table)
        {
            CRUDBasedStoredProcs crudsp = new CRUDBasedStoredProcs(table, this.versionNumber);
            TableName proc = crudsp.getProc(SPtype.countAll);

            StringBuilder s = new StringBuilder();
            addSPProcStart(proc, s);
            s.AppendFormat(@"
CREATE PROCEDURE {0} ()
AS
BEGIN
SELECT count(*) FROM {1}
", proc.sqlFromName, table.sqlFromName);

            s.Append("END" + CRLF);
            addSPExecPermission(proc, s);
            addSPDrop(proc);

            writeFile(proc, PartialType.DB_CRUD, s.ToString(), "Stored Procedure");
        }

        public virtual TableName buildSPLoadDBRelation(TableName table, List<string> loadFields, bool isSelect)
        {
            TableColumnsInfo tci = this.db.columns(table);
            TableColumnsWrap cw = new TableColumnsWrap(this.db, tci);
            CRUDBasedStoredProcs crudsp = new CRUDBasedStoredProcs(table, this.versionNumber);
            TableName proc = null;
            if (isSelect)
            {
                proc = crudsp.getRelationSelectProc(loadFields);
            }
            else
            {
                proc = crudsp.getRelationLoadProc(table, loadFields);
            }

            EList<string> parmsDeclare = new EList<string>();

            EList<string> parmsWhere = new EList<string>();

            foreach (string c in cw.getColumns())
            {
                ColumnAttributes ca = cw.columnAttributes(c);
                if (cw.isComputed(c)) continue;

                string p = getParamDeclarationType(c, ca);

                if (loadFields.Contains(c))
                {
                    parmsDeclare.Add(p);
                    parmsWhere.Add("[" + c + "] = @" + fixName(c));
                }
            }

            StringBuilder s = new StringBuilder();
            addSPProcStart(proc, s);
            string paramsDeclareStr = "";
            if (parmsDeclare.Count > 0)
            {
                paramsDeclareStr = string.Format("{1}({1}   {0}{1})", parmsDeclare.join("," + CRLF), CRLF);
            }
            s.AppendFormat(@"
CREATE PROCEDURE {0}{1}
AS
BEGIN
SELECT * FROM {2}
{3} {4}
", proc.sqlFromName, paramsDeclareStr, table.sqlFromName, parmsWhere.Count >0 ? "WHERE " : "", parmsWhere.join(" AND " + CRLF));

            s.Append("END" + CRLF);
            addSPExecPermission(proc, s);
            addSPDrop(proc);

            writeFile(proc, PartialType.DB_CRUD, s.ToString(), "Stored Procedure");

            return proc;

        }

    }
}


