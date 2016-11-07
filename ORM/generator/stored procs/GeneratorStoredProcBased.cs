using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using EM.DB;
using EM.Collections;
using ORM;
using ORM.DBFields;
using System.Data;
using System.IO;
using ORM.db_store;
using System.Reflection;
using ORM.db_store.persitence;

namespace ORM.generator
{
    /// <summary>
    /// - Generates DAL for specified database objects and their related objects under a folder named DB_PARTIAL
    /// - files generated under CLS_PARTIAL folder (built only ones) could be used to implement business logic 
    /// - if inheriting (maybe in a diferent DLL) for business logic, an ORM.generator.ORMClassFactory should be specified, 
    ///   so that it will instantiate polimorficaly from the inheritated classes in case of select statements
    /// </summary>
    public abstract class GeneratorStoredProcBased : GeneratorBase
    {
        public abstract IStoredProcsGenerator gsp { get; }

        public GeneratorStoredProcBased(GenericDatabase db)
            : base(db)
        {
            
        }

        /// <summary>
        /// you should not need this but either buildTables or buildDatabaseObjects instead   (which uses this one internaly)
        /// </summary>
        public override void buildTable(TableName table, bool isView, OrderedDictionary<TableName, ESet<DBRelation>> oneToMany, EList<TableName> allNeededTables)
        {
            base.buildTable(table, isView, oneToMany, allNeededTables);

            if (!isView)
            {
                PKInfo pk = this.db.pk(table);
                TableColumnsInfo tci = this.db.columns(table);
                TableColumnsWrap cw = new TableColumnsWrap(this.db, tci);

                this.gsp.buildSPInsert(table, cw, pk);
                this.gsp.buildSPUpdateByPK(table, cw, pk);
                this.gsp.buildSPDeleteByPK(table, cw, pk);
                this.gsp.buildSPLoadByPK(table, cw, pk);
                this.gsp.buildSPCountAll(table);
            }
        }



        protected override void addDBConstructorDBRelationsPart(TableName table, OrderedDictionary<TableName, ESet<DBRelation>> oneToMany, PKInfo pk, FKInfo fk, StringBuilder s, bool isView)
        {
            base.addDBConstructorDBRelationsPart(table, oneToMany, pk, fk, s, isView);
            if (!isView)
            {
                this.gsp.addStoreProcInit(table, s);
            }
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //// BASIC GENERATOR TEMPLATE
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region basic generaor template

        protected override void addStartClassFilePersist(TableName tn, StringBuilder s)
        {
            s.Append(CRLF + "    public partial class " + tn.className + " : TableRowStoredProcBased, " + tn.interfaceName + " {" + CRLF);
        }

        
        protected void addFieldsConstructor(TableName table, TableColumnsWrap cw, List<string> fields, TableName proc, StringBuilder s)
        {
            EList<string> params_ = new EList<string>();
            EList<string> params2_ = new EList<string>();
            //EList<string> params3_ = new EList<string>();            
            foreach (string f in fields)
            {
                params_.Add(cw.fieldDotNetType(f) + " " + fixName(f));
                params2_.Add(fixName(f));
                //params3_.Add(string.Format("{0} = @{1}", this.db.escape(f), fixName(f)));
            }
            string name = GetMethodName_ParamsPart(fields);
            if (proc == null)
            {
                name = "PK";
            }
            s.AppendFormat("        public void loadBy{0}({1}) {{" + CRLF, name, params_.join(", "));
            s.AppendFormat("            DBParams p = new DBParams();" + CRLF);
            foreach (string f in fields)
            {
                s.AppendFormat("            if ({0} == null) throw new BusinessLogicError(\"{0} was not supplied\");" + CRLF, fixName(f));
                s.AppendFormat("            p.Add(new DBParam(\"@{0}\", {1}));" + CRLF, fixName(f), fixName(f));
            }
            if (proc == null)
            {
                s.AppendFormat("            DataTable tb = this.db.db.getDataTable(this.storedProcedures.loadByPK.sqlFromName, p, CommandType.StoredProcedure);" + CRLF);
            }
            else
            {
                s.AppendFormat("            DataTable tb = this.db.db.getDataTable(\"{0}\", p, CommandType.StoredProcedure);" + CRLF, proc.sqlFromName);
            }
            s.AppendFormat("            this.setFromOneRowDataTable(p, tb);" + CRLF);
            s.Append("        }" + CRLF);

            s.AppendFormat("        public static {2} loadBy{0}(ORMContext cx, {1}) {{" + CRLF, name, params_.join(", "), table.className);
            s.AppendFormat("            {0} tb = new {0}(cx);" + CRLF, table.className);
            s.AppendFormat("            tb.loadBy{0}({1});" + CRLF, name, params2_.join(", "));
            s.Append("            return tb;" + CRLF);
            s.Append("        }" + CRLF);


            s.AppendFormat("        public static {2} loadOrGetEmptyInstanceBy{0}(ORMContext cx, {1}) {{" + CRLF, name, params_.join(", "), table.className);
            s.AppendFormat("            {0} tb = new {0}(cx);" + CRLF, table.className);
            s.AppendFormat("            try {{" + CRLF);
            s.AppendFormat("                tb.loadBy{0}({1});" + CRLF, name, params2_.join(", "));
            s.AppendFormat("            }} catch(BusinessLogicError er) {{" + CRLF);
            foreach (string f in fields)
            {
                
                if ( (cw.isRequired(f) || table.customFieldPropertyType.ContainsKey(f)) && cw.fieldDotNetType(f).EndsWith("?"))
                {
                    //it could never have null as value
                    s.AppendFormat("                tb.{0} = {1}.Value;" + CRLF, fixName(f), fixName(f));
                }
                else
                {
                    s.AppendFormat("                tb.{0} = {1};" + CRLF, fixName(f), fixName(f));
                }
            }
            s.AppendFormat("            }}" + CRLF);
            s.Append("            return tb;" + CRLF);
            s.Append("        }" + CRLF);

            
            


        }

        protected override void addConstructorPK(TableName table, PKInfo pk, TableColumnsWrap cw, StringBuilder s)
        {
            //constructor pk:
            if (pk.Count > 0)
            {
                s.Append(CRLF);
                s.Append("        /////////////////////////////////////////////////////////////////////////////////////" + CRLF);
                s.Append("        //////////// Constructor PK" + CRLF + CRLF);

                addFieldsConstructor(table, cw, pk, null, s);                
            }
        }

        protected override void addUniqueConstraintConstructors(TableName table, EList<DBConstraint> constraints, TableColumnsWrap cw, StringBuilder s)
        {
            s.Append(CRLF);
            s.Append("        /////////////////////////////////////////////////////////////////////////////////////" + CRLF);
            s.Append("        //////////// Constructors - Unique Constraints and Unique Indexes " + CRLF + CRLF);

            foreach (DBConstraint c in constraints)
            {
                TableName proc = this.gsp.buildSPLoadDBRelation(table, c, false);
                addFieldsConstructor(table, cw, c, proc, s);
            }
        }

        protected override void addDBOneToManyConstructors(TableName table, PKInfo pk, OrderedDictionary<TableName, ESet<DBRelation>> oneToMany, TableColumnsWrap cw, StringBuilder s)
        {
            if (oneToMany.get(table, null) != null)
            {
                s.Append(CRLF);
                s.Append("        /////////////////////////////////////////////////////////////////////////////////////" + CRLF);
                s.Append("        //////////// Constructors - One To Many " + CRLF + CRLF);

                EList<EList<string>> seen = new EList<EList<string>>(); 
                foreach (DBRelation rel in oneToMany[table])
                {
                    if (rel.tableThere.Equals(table) && sameFields(rel.fieldsThere, pk)) { continue; }
                    
                    bool isSeen = false;
                    foreach (EList<string> sf in seen)
                    {
                        if (sf.diference(rel.fieldsThere).Count == 0)
                        {
                            isSeen = true;
                            break;
                        }
                    }

                    if (!isSeen)
                    {
                        seen.Add(rel.fieldsThere);
                        TableName proc = this.gsp.buildSPLoadDBRelation(rel.tableThere, rel.fieldsThere, false);
                        
                        addFieldsConstructor(table, cw, rel.fieldsThere, proc, s);
                        s.Append(CRLF);
                    }
                }
            }
        }

        protected override void addDBForeignKeysConstructors(TableName table, FKInfo fk, TableColumnsWrap cw, StringBuilder s)
        {
            //Foreign Keys:     one to one
            if (fk.Count > 0)
            {
                s.Append(CRLF);
                s.Append("        /////////////////////////////////////////////////////////////////////////////////////" + CRLF);
                s.Append("        //////////// Constructors - Foreign Keys " + CRLF + CRLF);

                EList<EList<string>> seen = new EList<EList<string>>(); 
                foreach (DBRelation rel in fk)
                {
                    bool isSeen = false;
                    foreach (EList<string> sf in seen)
                    {
                        if (sf.diference(rel.fieldsHere).Count == 0)
                        {
                            isSeen = true;
                            break;
                        }
                    }

                    if (!isSeen)
                    {
                        seen.Add(rel.fieldsHere);
                        TableName proc = this.gsp.buildSPLoadDBRelation(rel.tableHere, rel.fieldsHere, true);

                        EList<string> params_ = new EList<string>();
                        EList<string> params2_ = new EList<string>();
                        foreach (string f in rel.fieldsHere)
                        {
                            params_.Add(cw.fieldDotNetType(f) + " " + fixName(f));
                            params2_.Add(rel.tableHere.getCustomFieldPropertyName(f));
                        }
                        string name = GetMethodName_ParamsPart(rel.fieldsHere);
                        s.AppendFormat("        public static TablePersist<{0}> selectBy{1}(ORMContext cx, {2}) {{" + CRLF, rel.tableHere.className, name, params_.join(", "));
                        s.AppendFormat("            DataTable tb = {2}.selectDataTableBy{0}(cx, {1});" + CRLF, name, params2_.join(", "), rel.tableHere.className);
                        s.AppendFormat("            return {0}.getInstancesFromDataTable<{0}>(cx, tb);" + CRLF, rel.tableHere.className);
                        s.Append("        }" + CRLF);

                        s.AppendFormat("        public static DataTable selectDataTableBy{0}(ORMContext cx, {1}) {{" + CRLF, name, params_.join(", "));
                        s.AppendFormat("            DBParams p = new DBParams();" + CRLF);
                        for (int i = 0; i < rel.fieldsHere.Count; i++)
                        {
                            s.AppendFormat("            p.Add(new DBParam(\"@{0}\", {1}));" + CRLF, fixName(rel.fieldsHere[i]), params2_[i]);
                        }
                        s.AppendFormat("            return cx.db.db.getDataTable(\"{0}\", p, CommandType.StoredProcedure);" + CRLF, proc.sqlFromName);
                        s.Append("        }" + CRLF);
                        s.Append(CRLF);

                    }
                }

            }
        }

        protected override void addCustomSelectConstructors(TableName table, FKInfo fk, TableColumnsWrap cw, StringBuilder s, ESet<DBConstraint> customSelect, bool isView)
        {
            //Custom Select Methods
            if (customSelect.Count > 0)
            {
                EList<DBConstraint> todo = DoCustomSelectList(table, customSelect, fk);
                if (todo.Count > 0)
                {
                    s.Append(CRLF);
                    s.Append("        /////////////////////////////////////////////////////////////////////////////////////" + CRLF);
                    s.Append("        //////////// Constructors - Custom Select Methods " + CRLF + CRLF);


                    EList<EList<string>> seen = new EList<EList<string>>();
                    foreach (DBConstraint c in todo)
                    {
                        bool isSeen = false;
                        foreach (EList<string> sf in seen)
                        {
                            if (sf.diference(c).Count == 0)
                            {
                                isSeen = true;
                                break;
                            }
                        }

                        if (!isSeen)
                        {
                            seen.Add(c);
                            TableName proc = this.gsp.buildSPLoadDBRelation(table, c, true);

                            EList<string> params_ = new EList<string>();
                            EList<string> params2_ = new EList<string>();
                            foreach (string f in c)
                            {
                                params_.Add(cw.fieldDotNetType(f) + " " + fixName(f));
                                params2_.Add(fixName(f));
                            }
                            string name = GetMethodName_ParamsPart(c);
                            string retType = isView ? "Table" : "TablePersist";
                            string selectName = params_.Count>0 ? "By" : "All";
                            string comma = params_.Count>0 ? ", " : "";
                            s.AppendFormat("        public static {3}<{0}> select{4}{1}(ORMContext cx{5}{2}) {{" + CRLF, table.className, name, params_.join(", "), retType, selectName, comma);
                            s.AppendFormat("            DataTable tb = {2}.selectDataTable{3}{0}(cx{4}{1});" + CRLF, name, params2_.join(", "), table.className, selectName,comma);
                            s.AppendFormat("            return {0}.getInstancesFromDataTable<{0}>(cx, tb);" + CRLF, table.className);
                            s.Append("        }" + CRLF);

                            s.AppendFormat("        public static DataTable selectDataTable{2}{0}(ORMContext cx{3}{1}) {{" + CRLF, name, params_.join(", "), selectName,comma);
                            s.AppendFormat("            DBParams p = new DBParams();" + CRLF);
                            for (int i = 0; i < c.Count; i++)
                            {
                                s.AppendFormat("            p.Add(new DBParam(\"@{0}\", {1}));" + CRLF, fixName(c[i]), params2_[i]);
                            }
                            s.AppendFormat("            return cx.db.db.getDataTable(\"{0}\", p, CommandType.StoredProcedure);" + CRLF, proc.sqlFromName);
                            s.Append("        }" + CRLF);
                            s.Append(CRLF);

                        }
                    }
                }
            }
            
        }


        protected override void addCustomDeleteMethods(TableName table, FKInfo fk, TableColumnsWrap cw, StringBuilder s, ESet<DBConstraint> customDelete)
        {
            //Custom Delete Methods
            if (customDelete.Count > 0)
            {
                
                s.Append(CRLF);
                s.Append("        /////////////////////////////////////////////////////////////////////////////////////" + CRLF);
                s.Append("        //////////// Custom Delete Methods " + CRLF + CRLF);


                EList<EList<string>> seen = new EList<EList<string>>();
                foreach (DBConstraint c in customDelete)
                {
                    if (c.Count > 0)
                    {
                        bool isSeen = false;
                        foreach (EList<string> sf in seen)
                        {
                            if (sf.diference(c).Count == 0)
                            {
                                isSeen = true;
                                break;
                            }
                        }

                        if (!isSeen)
                        {
                            seen.Add(c);
                            TableName proc = this.gsp.buildSPDeleteByCustomFields(table, cw, c);

                            EList<string> params_ = new EList<string>();
                            EList<string> params2_ = new EList<string>();
                            foreach (string f in c)
                            {
                                params_.Add(cw.fieldDotNetType(f) + " " + fixName(f));
                                params2_.Add(fixName(f));
                            }
                            string name = GetMethodName_ParamsPart(c);
                            string comma = params_.Count > 0 ? ", " : "";
                            s.AppendFormat("        public static void deleteBy{0}(ORMContext cx, {1}) {{" + CRLF, name, params_.join(", "));
                            s.AppendFormat("            DBParams p = new DBParams();" + CRLF);
                            for (int i = 0; i < c.Count; i++)
                            {
                                s.AppendFormat("            p.Add(new DBParam(\"@{0}\", {1}));" + CRLF, fixName(c[i]), params2_[i]);
                            }
                            s.AppendFormat("            cx.db.db.executeQuery(\"{0}\", p, CommandType.StoredProcedure);" + CRLF, proc.sqlFromName);
                            s.Append("        }" + CRLF);
                            s.Append(CRLF);                            
                        }
                    }
                }
                
            }

        }

        

        #endregion basic generaor template


    }
}
