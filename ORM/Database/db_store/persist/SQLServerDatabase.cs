using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using EM.Collections;
using EM.DB;
using System.Data;
using ORM.db_store;
using System.Data.Common;
using System.Data.SqlClient;
using EM.DB.Index;
using ORM.DBFields;

namespace ORM.db_store.persitence
{
    [Serializable]
    /// <summary>
    /// Backend for MS SQL Server 
    /// </summary>
    public class SQLServerDatabase: GenericDatabase
    {
        //TODO: add suport for db owner other then dbo so that "select ... from blabla" can be written as "select ... from gmilas.blabla" in order to succed
        //select obj.name, usr.name from dbo.sysobjects as obj
        //left join sysusers as usr on obj.uid = usr.uid
        //where OBJECTPROPERTY(obj.id, N'IsView') = 1 and obj.status>0

        public SQLServerDatabase() { }
        public SQLServerDatabase(SqlServerDBWorker db): base(db) { }
        public SQLServerDatabase(SqlServerDBWorkerOneConnection db) : base(db) { }

        protected EList<TableName> _tables;
        protected EList<TableName> _views;
        protected EList<TableName> _storedProcedures;
        
        public override string DBName 
        {
            get 
            {
                this.db.restoreConnection();
                return this.db.connection.Database; 
            } 
        }

        public override string tb_autofield() {
            return "int IDENTITY (1, 1)";
        }

        public override string escape(string identifier) {
            return "["+ identifier +"]";  //I just like it better then ""
        }

        public override ILastInsertIDProvider getLastInsertID(TableRow tb)
        {
            return new SQLServerLastInserIDProvider();
            //return "SELECT IDENT_CURRENT('" + tb.name.sqlFromName +"');";
        }

        public override SQLStatement limitAndOffset(PKInfo pk, SQLStatement atr)
        {
            if (atr.limit > 0) //select TOP ...
            {
                if (!atr.limitOffsetHelper.limitApplied)
                {
                    atr.fields = String.Format("TOP {0} {1}", atr.limit, atr.fields);
                    atr.limitOffsetHelper.limitApplied = true;
                }
                else
                {
                    string[] atrflds = atr.fields.Split(' ');
                    EList<string> atrfldsList = new EList<string>();
                    for (int i = 2; i < atrflds.Length; i++)
                    {
                        atrfldsList.Add(atrflds[i]);
                    }
                    atr.fields = String.Format("TOP {0} {1}", atr.limit, atrfldsList.join(" "));
                }
            }

            if (atr.offset > 0)     // select ... WHERE PK not in (select TOP ... order by PK)
            {
                if (atr.limitOffsetHelper.offsetApplied)
                {
                    atr.where = atr.limitOffsetHelper["where"];  // get the "where" as it was before applying offset
                }

                string inwhere = (atr.where != null && atr.where.Trim() != "") ? " AND " : " WHERE ";
                if (pk.Count == 0) pk = this.pk(pk.table);
                if (pk.Count == 0)
                    throw new NotSupportedException("this ORM only suports offset if a PK is set. No pk was found for " + pk.table);
                string pksql = "";
                foreach (string f in pk)
                {
                    string val = string.Format("cast({0}.[{1}] as varchar)", pk.table.sqlFromName, f);
                    if (pksql != "") { pksql += " + ' ' + " + val; }
                    else pksql = val;
                }

                if (!atr.from.ToLower().Contains("from ")) atr.from = "FROM " + atr.from;
                string subwhere = atr.where;
                if (subwhere != null && subwhere.Trim() != "")
                {
                    if (!subwhere.ToLower().Trim().StartsWith("where"))
                    {
                        subwhere = " WHERE (" + subwhere + ")";
                    }
                }

                string subfrom = atr.from;
                string suborder = atr.order;
                string subgroup = atr.group;
                string subhaving = atr.having;
                if (atr.from != null && atr.from.Trim() != "" && !atr.from.Trim().ToLower().StartsWith("from")) subfrom = "FROM " + atr.from;
                if (atr.order != null && atr.order.Trim() != "" && !atr.order.Trim().ToLower().StartsWith("order by")) suborder = "ORDER BY " + atr.order;
                if (atr.group != null && atr.group.Trim() != "" && !atr.group.Trim().ToLower().StartsWith("group by")) subgroup = "GROUP BY " + atr.group;
                if (atr.having != null && atr.having.Trim() != "" && !atr.having.Trim().ToLower().StartsWith("having")) subhaving = "HAVING " + atr.having;
                inwhere += string.Format("{0} not in (select top {1} {0} {2} {3} {4} {5} {6})", pksql, atr.offset.ToString(), subfrom, subwhere, subgroup, subhaving, suborder);
                atr.limitOffsetHelper.offsetApplied = true;
                atr.limitOffsetHelper["where"] = atr.where;
                if (atr.where != null && atr.where.Trim() != "")
                {
                    if (subwhere.ToLower().Trim().StartsWith("where"))
                    {
                        atr.where = " WHERE (" + atr.where + ") " + inwhere;
                    }
                    else
                    {
                        atr.where = "(" + atr.where + ") " + inwhere;
                    }
                }
                else
                {
                    atr.where = inwhere;
                }
            }
            return atr;
        }



        /// <summary>
        /// "Returns a list of table names in the current database."
        /// </summary>
        public override EList<TableName> tables
        {
            get 
            {
                if (_tables==null || _tables.Count==0) {
                    //DataTable tb = this.db.getDataTable("select name from dbo.sysobjects where OBJECTPROPERTY(id, N'IsUserTable') = 1 and name not like 'sys%'");
                    DataTable tb = this.db.getDataTable("SELECT * FROM INFORMATION_SCHEMA.Tables T where table_type = 'BASE TABLE'");
                    _tables = new EList<TableName>();
                    foreach(DataRow row in tb.Rows) 
                    {
                        TableName tn = new TableName(row[0].ToString(), row[1].ToString(), row[2].ToString());
                        _tables.Add(tn);    //row[0].ToString()
                    }
                }
                return _tables;
            }
        }


        
        public override EList<TableName> views
        {
            get 
            {
                if (_views==null || _views.Count==0) {
                    //DataTable tb = this.db.getDataTable("select name from dbo.sysobjects where OBJECTPROPERTY(id, N'IsView') = 1 and name not like 'sys%'");
                    DataTable tb = this.db.getDataTable("SELECT * FROM INFORMATION_SCHEMA.Tables T where table_type = 'VIEW'");
                    _views = new EList<TableName>();
                    foreach(DataRow row in tb.Rows) 
                    {
                        TableName tn = new TableName(row[0].ToString(), row[1].ToString(), row[2].ToString());
                        _views.Add(tn);
                    }
                }
                return _views;
            }
        }

        public override EList<TableName> storedProcedures
        {
            get
            {
                if (_storedProcedures == null || _storedProcedures.Count == 0)
                {
                    if (_storedProcedures == null) { _storedProcedures = new EList<TableName>(); }
                    DataTable tb = this.db.getDataTable("SELECT ROUTINE_CATALOG, ROUTINE_SCHEMA, ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES T where routine_type = 'PROCEDURE'");
                    _views = new EList<TableName>();
                    foreach (DataRow row in tb.Rows)
                    {
                        TableName tn = new TableName(row[0].ToString(), row[1].ToString(), row[2].ToString());
                        _storedProcedures.Add(tn);
                    }
                }
                return _storedProcedures;
            }
        }

        public override PKInfo pk(TableName table) 
        {   PKInfo res = new PKInfo();
            res.table = table;
            DataTable tb = this.db.getDataTable(string.Format(@"
                SELECT c.name
                FROM sys.indexes AS i
                    INNER JOIN sys.index_columns AS ic ON 
                        i.OBJECT_ID = ic.OBJECT_ID
                        AND i.index_id = ic.index_id
                    INNER JOIN sys.columns AS c ON 
                        c.OBJECT_ID = i.OBJECT_ID
                        AND c.column_id = ic.column_id
                WHERE i.is_primary_key = 1 AND i.OBJECT_ID = object_id(N'[{0}].[{1}]')", table.schema, table.table));
            foreach(DataRow r in tb.Rows) 
            {
                res.Add(r[0].ToString());
            }
            return res;
            /*  OLDER VERSION - is bad (colid <= i.keycnt is causing problems)
            select c.name from syscolumns c, sysindexes i
                where c.id = object_id(N'[{0}].[{1}]') AND c.id = i.id
                    AND (i.status & 0x800) = 0x800 AND c.colid <= i.keycnt
                order by c.colid
            */ 
        }

        public override FKInfo fk(TableName table) 
        {
            FKInfo fks = new FKInfo();
            fks.table = table;

            int nrKeys = 16;  //a fk constraint in sql server may be made out of maximum 16 keys
            string knames = "";
            string rnames = "";
            for(int i=1; i<=nrKeys;i++){
                string kn = string.Format("(select name from syscolumns where id=fkeyid and colid=fkey{0}) as fname{0}", i.ToString());
                string rn = string.Format("(select name from syscolumns where id=rkeyid and colid=rkey{0}) as rname{0}", i.ToString());
                if (i<nrKeys) {knames += kn + ", "; rnames += rn + ", ";}
                else { knames += kn; rnames += rn; }                
            }
            string sql = string.Format(@"select schema_name(so.schema_id) schema_name, o2.name, keycnt, o.name AS relation_name, {0}, {1}
                    from sysreferences r 
                         join sysobjects o2 on r.rkeyid=o2.id
                         join sys.objects so on o2.id=so.object_id,
                         sysobjects o
                    where o.parent_obj = object_id(N'[{2}].[{3}]')
                        AND r.constid = o.id
                        AND o.xtype = 'F' ", knames, rnames, table.schema, table.table);
            //Console.WriteLine(sql);
            DataTable tb = this.db.getDataTable(sql);
            foreach(DataRow row in tb.Rows)
            {   
                DBRelation rel = new DBRelation();
                rel.relationName = row["relation_name"] != System.DBNull.Value ? row["relation_name"].ToString() : "";  //constraint name
                rel.tableHere = table;
                //TableName t = this.tablesDict.get(row[0].ToString(), new TableName(table.catalog, table.schema, row[0].ToString()));
                TableName t = new TableName(table.catalog, row["schema_name"].ToString(), row["name"].ToString());
                t.className = getClassName(t, this.tables);
                rel.tableThere = t;  //relation table name
                rel.keysCount = row["keycnt"]!=System.DBNull.Value ? int.Parse(row["keycnt"].ToString()): 0;         //each constraint definition has 'keycnt' keys participating (row[1])
                for (int i=0;i<rel.keysCount;i++)
                {
                    rel.fieldsHere.Add(row[4+i].ToString());
                    rel.fieldsThere.Add(row[4+nrKeys+i].ToString());
                }
                fks.Add(rel);
            }
                
            return fks;    

        }

        public override EList<DBConstraint> uniqueConstraints(TableName table)
        {
            string sql = string.Format(@"SELECT TC.TABLE_name AS TNAME, TC.CONSTRAINT_NAME AS CTNAME , CCU.COLUMN_NAME AS COLNAME, 'uniqueue constraint' AS CTTYPE
                FROM [INFORMATION_SCHEMA].TABLE_CONSTRAINTS TC 
                JOIN [INFORMATION_SCHEMA].CONSTRAINT_COLUMN_USAGE CCU 
                    ON TC.constraint_name = ccu.constraint_name 
                WHERE TC.constraint_type = 'unique'
                AND TC.TABLE_name = '{1}'
                AND TC.TABLE_SCHEMA = '{0}'

                UNION

                SELECT o.name AS TNAME, ix.NAME AS CTNAME, tc.name AS COLNAME, 'uniqueue index' AS CTTYPE 
                FROM sys.indexes ix 
                    JOIN sys.index_columns ixc ON ix.index_id = ixc.index_id 
                                            and ix.OBJECT_ID = ixc.OBJECT_ID
                    JOIN sys.columns tc ON ix.OBJECT_ID = tc.OBJECT_ID 
                                        AND ixc.column_id = tc.column_id
                    JOIN sys.objects o ON ix.OBJECT_ID = o.OBJECT_ID
                WHERE is_unique = 1 AND is_primary_key = 0 AND is_unique_constraint = 0
                --AND o.NAME NOT LIKE 'sys%'
                AND ixc.object_id = OBJECT_ID('[{0}].[{1}]')                
                ", table.schema, table.table);
            
            DataTable tb = this.db.getDataTable(sql);
            EList<DBConstraint> res = new EList<DBConstraint>();
            string cname = null;
            DBConstraint constraint = null;
            foreach (DataRow r in tb.Rows)
            {
                if (r["CTNAME"].ToString() != cname)
                {
                    cname = r["CTNAME"].ToString();
                    constraint = new DBConstraint();
                    constraint.constraintName = cname;
                    constraint.constraintType = r["CTTYPE"].ToString();
                    constraint.table = table;
                    constraint.Add(r["COLNAME"].ToString());
                    res.Add(constraint);
                }
                else
                {
                    constraint.Add(r["COLNAME"].ToString());
                }
            }
            return res;
        }

        public override TableColumnsInfo columns(TableName table)
        {
            string sql = string.Format(@"SELECT T.TABLE_NAME AS [Table], 
                       C.COLUMN_NAME AS [ColumnName], 
                       C.IS_NULLABLE AS [AllowNull], 
                       C.DATA_TYPE AS [Type],
                       C.CHARACTER_MAXIMUM_LENGTH  AS max_length,
                       COLUMNPROPERTY( OBJECT_ID('[{1}].[{0}]'),C.COLUMN_NAME, 'IsIdentity') AS isIdentity,
                       C.ORDINAL_POSITION as  position,
                       0 AS hasDefaultConstraint,
                       COLUMNPROPERTY ( OBJECT_ID('[{1}].[{0}]'),C.COLUMN_NAME, 'IsComputed' ) AS isComputed  
                       , C.NUMERIC_PRECISION AS [NumericPrecision]
                       , C.NUMERIC_SCALE AS [NumericScale]
                       , C.COLUMN_DEFAULT AS [DefaultValue]
                       --CASE WHEN TCC.COLUMN_NAME = C.COLUMN_NAME THEN 1 ELSE 0 END AS isUnique	
                FROM INFORMATION_SCHEMA.Tables T JOIN INFORMATION_SCHEMA.Columns C
                        ON T.TABLE_NAME = C.TABLE_NAME AND T.TABLE_SCHEMA = C.TABLE_SCHEMA
                     --LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC  ON T.TABLE_NAME = TC.TABLE_NAME AND TC.CONSTRAINT_TYPE = 'UNIQUE'
                     --LEFT JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE TCC ON TC.CONSTRAINT_NAME = TCC.CONSTRAINT_NAME
                WHERE T.TABLE_NAME NOT LIKE 'sys%'
                AND T.TABLE_NAME <> 'dtproperties'
                AND T.TABLE_SCHEMA <> 'INFORMATION_SCHEMA'
                AND T.TABLE_NAME = '{0}'
                AND T.TABLE_SCHEMA = '{1}'
                ORDER BY T.TABLE_NAME, C.ORDINAL_POSITION
                ", table.table, table.schema, table.catalog);
            string sql2 = string.Format(@"SELECT so.name, so.id, sc.colid
                FROM sysobjects so JOIN sysconstraints sc ON so.id = sc.constid
                WHERE so.xtype ='D' AND parent_obj = OBJECT_ID('[{0}].[{1}]')", table.schema, table.table);

            DataTable tb = this.db.getDataTable(sql);
            DataTable tbc = this.db.getDataTable(sql2);
            ESet<string> colsDefConstr = new ESet<string>();
            foreach(DataRow r in tbc.Rows) 
            {
                colsDefConstr.Add(r[2].ToString());
            }
            foreach (DataRow r in tb.Rows)
            {
                if (colsDefConstr.Contains(r["position"].ToString()))
                {
                    r["hasDefaultConstraint"] = 1;
                    r.AcceptChanges();
                }
            }
            tb.AcceptChanges();
            return new TableColumnsInfo(tb, table);
        }

        public override T storedProcedure<T>(TableName procName)
        {
            string dbCstr = null;
            db.restoreConnection();
            if (db.connection.Database.ToLower().Trim() != procName.catalog.ToLower().Trim())
            {   
                //must run the sys proc from withing the catalog
                dbCstr = db.CONN_STR;
                Regex r = new Regex(@"Initial\s+Catalog\s*=\s*[a-z|0-9|-|_|\.]+;", RegexOptions.IgnoreCase);
                db.CONN_STR = r.Replace(db.CONN_STR, "Initial Catalog=" + procName.catalog + ";");
                db.Dispose();
                db.init();                
            }

            DBParams p = new DBParams();
            p.Add("@procedure_name", procName.table);
            p.Add("@procedure_schema", procName.schema);
            //p.Add("@patamenetr_name", );  //find info about one param
            DataTable res = this.db.getDataTable("sys.sp_procedure_params_rowset", p, CommandType.StoredProcedure);

            PKIndex idx = new PKIndex(this.db.getDataTable(
                string.Format("select s.name, s.isoutparam from syscolumns s where id = OBJECT_ID('[{0}].[{1}]')",
                    procName.schema,
                    procName.table)), "name");
            
            if (dbCstr != null)
            {
                db.CONN_STR = dbCstr;
                db.Dispose();
                db.init();                
            }

            T ret = new T();
            ret.init(procName, this);

            foreach (DataRow r in res.Rows)
            {
                if (r["PARAMETER_NAME"].ToString() != "@RETURN_VALUE")
                {
                    ret.parameters.Add(
                        new StoredProcParam(
                            r["PARAMETER_NAME"].ToString(), 
                            r["TYPE_NAME"].ToString(),
                            int.Parse(r["PARAMETER_HASDEFAULT"].ToString()) == 1 ? true : false,
                            (bool)r["IS_NULLABLE"],
                            int.Parse(r["ORDINAL_POSITION"].ToString()),
                            int.Parse(idx.Find(r["PARAMETER_NAME"])["isoutparam"].ToString()) == 1 ? true : false,
                            r["CHARACTER_MAXIMUM_LENGTH"] == null || r["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value ? 0 : int.Parse(r["CHARACTER_MAXIMUM_LENGTH"].ToString()), 
                            r["CHARACTER_OCTET_LENGTH"] == null || r["CHARACTER_OCTET_LENGTH"] == DBNull.Value ? 0: int.Parse(r["CHARACTER_OCTET_LENGTH"].ToString()),  
                            r["NUMERIC_PRECISION"] == null || r["NUMERIC_PRECISION"] == DBNull.Value ? 0: int.Parse(r["NUMERIC_PRECISION"].ToString()),
                            r["NUMERIC_SCALE"] == null || r["NUMERIC_SCALE"] == DBNull.Value ? 0 : int.Parse(r["NUMERIC_SCALE"].ToString())
                        ));
                }
            }

            return ret;
        }

        public override string fieldClassType(string column, TableColumnsInfo tci)
        {
            //string data = @"{'System.String':'FString', 'System.Int16':'FInteger', 'System.Int32':'FInteger', 'System.Byte':'FInteger', 'System.Double':'FFloat', 'System.Boolean':'FBoolean'}"; 
            string data = @"{'bigint':'FInteger',
                             'bit':'FBoolean',
                             'char':'FChar',
                             'nchar':'FChar', 
                             'datetime':'FDatetime',
                             'date':'FDatetime',
                             'time':'FTimeSpan',
                             'decimal':'FFloat',
                             'float':'FFloat',
                             'real': 'FFloat',
                             'int':'FInteger',
                             'long':'FInteger',
                             'money':'FFloat',
                             'numeric':'FFloat',
                             'nvarchar':'FVarchar',
                             'smalldatetime':'FDatetime',
                             'timestamp':'FByteArray',
                             'smallint':'FInteger',
                             'smallmoney':'FFloat',
                             'text':'FText',
                             'ntext':'FText',
                             'tinyint':'FInteger',
                             'uniqueidentifier':'FGuid',
                             'image':'FImage',   
                             'varbinary':'FVarBinary',
                             'xml':'FXML',   
                             'varchar':'FVarchar'}";
            EDictionary<string, string> d = EDictionary<string, string>.fromStrStr(data);
            //string res = d[c.DataType];

            if (tci[column].dbType == null && tci[column].dotNetType != null)
            {
                return this.fieldClassType(tci[column].dotNetType);                
            }

            try
            {
                return d[tci[column].dbType];
            }
            catch
            {
                throw;
            }
            
        }

        public override string GetSqlDbType(string dbType)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("bigint", "SqlDbType.BigInt");
            map.Add("bit", "SqlDbType.Bit");
            map.Add("char", "SqlDbType.Char");
            map.Add("nchar", "SqlDbType.NChar");
            map.Add("datetime", "SqlDbType.DateTime");
            map.Add("time", "SqlDbType.Time");
            map.Add("decimal", "SqlDbType.Decimal");
            map.Add("float", "SqlDbType.Float");
            map.Add("real", "SqlDbType.Real");
            map.Add("int", "SqlDbType.Int");
            map.Add("long", "SqlDbType.BigInt");      //???
            map.Add("money", "SqlDbType.Money");
            map.Add("numeric", "SqlDbType.Decimal");  //???
            map.Add("nvarchar", "SqlDbType.NVarChar");
            map.Add("smalldatetime", "SqlDbType.SmallDateTime");
            map.Add("timestamp", "SqlDbType.Timestamp");
            map.Add("smallint", "SqlDbType.SmallInt");
            map.Add("smallmoney", "SqlDbType.SmallMoney");
            map.Add("text", "SqlDbType.Text");
            map.Add("ntext", "SqlDbType.NText");
            map.Add("tinyint", "SqlDbType.TinyInt");
            map.Add("uniqueidentifier", "SqlDbType.UniqueIdentifier");
            map.Add("image", "SqlDbType.Image");
            map.Add("varbinary", "SqlDbType.VarBinary");
            map.Add("xml", "SqlDbType.Xml");
            map.Add("varchar", "SqlDbType.VarChar");
            map.Add("date", "SqlDbType.Date");

            return map[dbType.Trim().ToLower()];
        }
        
        public override string fieldDotNetType(string dbType)
        {
            //http://msdn.microsoft.com/en-us/library/ms131092.aspx
            //return nullable types
            string data = @"{'bigint':'Int64?',
                             'bit':'Boolean?',
                             'char':'String',
                             'nchar':'String',
                             'datetime':'DateTime?',
                             'decimal':'Decimal?',
                             'float':'Double?',
                             'real':'Single?',
                             'int':'Int32?',
                             'image':'Byte[]',
                             'long':'Int64?',
                             'money':'Decimal?',
                             'numeric':'Decimal?',
                             'nvarchar':'String',
                             'smalldatetime':'DateTime?',
                             'date':'DateTime?',
                             'timestamp':'Byte[]',
                             'smallint':'Int16?',
                             'smallmoney':'Decimal?',
                             'text':'String',
                             'ntext':'String',
                             'tinyint':'Byte?',
                             'time': 'TimeSpan?',
                             'uniqueidentifier':'Guid?',
                             'varbinary':'Byte[]',
                             'xml':'String',   
                             'varchar':'String'}";
            EDictionary<string, string> d = EDictionary<string, string>.fromStrStr(data);
            return d[dbType];

        }

        public override object getDbTypeDummyValue(string dbType)
        {
            switch (dbType)
            {
                case "varchar": return "";
                case "int": return 0;
                case "datetime": return DateTime.Now;
                case "char": return "";
                case "bigint": return 0;
                case "bit": return false;
                case "decimal": return 0;
                case "float": return 0;
                case "long": return 0;
                case "money": return 0;
                case "numeric": return 0;
                case "smalldatetime": return DateTime.Now;
                case "date": return DateTime.Now;
                case "smallint": return 0;
                case "smallmoney": return 0;
                case "real": return 0;
                case "text": return "";
                case "ntext": return "";
                case "tinyint": return 0;
                case "timestamp": return null;
                //case "uniqueidentifier":return 0;
                //case "image": return 0;                                
            }
            return null;
        }

        /// <summary>
        /// for value you may pass in fld.value or fld.defaultValue or ...
        /// </summary>
        public override DBParam getDBParam(GenericField fld, object value, string paramName)
        {
            if (fld is FByteArray)
            {
                var byteArr = value == null ? new byte[0] : (byte[])value;
                SqlParameter sp;
                if (fld is FVarBinary)
                {
                    sp = new SqlParameter(paramName, SqlDbType.VarBinary, byteArr.Length);                    
                }
                else if (fld is FImage)
                {
                    sp = new SqlParameter(paramName, SqlDbType.Image, byteArr.Length);
                }
                else 
                {
                    sp = new SqlParameter();
                    sp.ParameterName = paramName;
                }
                if (value == null)
                {
                    sp.Value = DBNull.Value;
                }
                else
                {
                    sp.Value = byteArr;
                }
                return new DBParam(sp);
            }
            return base.getDBParam(fld, value, paramName);            
        }



    }
}

