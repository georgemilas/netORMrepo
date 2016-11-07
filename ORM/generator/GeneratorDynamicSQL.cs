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
    public class GeneratorDynamicSQL : GeneratorBase
    {
        
        public GeneratorDynamicSQL(GenericDatabase db): base(db)
        {

        }
       
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //// BASIC GENERATOR TEMPLATE
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region basic generaor template

        protected override void addStartClassFilePersist(TableName tn, StringBuilder s)
        {
            s.Append(CRLF + "    public partial class " + tn.className + " : TableRowDynamicSQL, " + tn.interfaceName + " {" + CRLF);
        }

        protected void addFieldsConstructor(TableName table, TableColumnsWrap cw, List<string> fields, bool isPK, StringBuilder s)
        {
            EList<string> params_ = new EList<string>();
            EList<string> params2_ = new EList<string>();
            EList<string> params3_ = new EList<string>();

            foreach (string f in fields)
            {
                params_.Add(cw.fieldDotNetType(f) + " " + fixName(f));
                params2_.Add(fixName(f));
                params3_.Add(string.Format("{0} = @{1}", this.db.escape(f), fixName(f)));
            }
            string name = GetMethodName_ParamsPart(fields);
            if (isPK)
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
            s.AppendFormat("            SQLStatement stm = new SQLStatement(this);" + CRLF);
            s.AppendFormat("            stm.where = \"{0}\";" + CRLF, params3_.join(" and "));
            s.AppendFormat("            setFromDB(stm, p);" + CRLF);
            s.Append("        }" + CRLF);

            s.AppendFormat("        public static {2} loadBy{0}(ORMContext cx, {1}) {{" + CRLF, name, params_.join(", "), table.className);
            s.AppendFormat("            {0} tb = new {0}(cx);" + CRLF, table.className);
            s.AppendFormat("            tb.loadBy{0}({1});" + CRLF, name, params2_.join(", "));
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

                addFieldsConstructor(table, cw, pk, true, s);
            }
        }


        

        protected override void addUniqueConstraintConstructors(TableName table, EList<DBConstraint> constraints, TableColumnsWrap cw, StringBuilder s)
        {
            s.Append(CRLF);
            s.Append("        /////////////////////////////////////////////////////////////////////////////////////" + CRLF);
            s.Append("        //////////// Constructors - Unique Constraints and Unique Indexes " + CRLF + CRLF);

            foreach (DBConstraint c in constraints)
            {
                addFieldsConstructor(table, cw, c, false, s);                
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
                        
                        addFieldsConstructor(table, cw, rel.fieldsThere, false, s);
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
                        
                        EList<string> params_ = new EList<string>();
                        EList<string> params2_ = new EList<string>();
                        EList<string> params3_ = new EList<string>();
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
                            s.AppendFormat("            p.Add(new DBParam(\"@{0}\", {1}));" + CRLF, rel.tableHere.getCustomFieldPropertyName(rel.fieldsHere[i]), params2_[i]);
                            params3_.Add(string.Format("{0} = @{1}", this.db.escape(rel.fieldsHere[i]), fixName(rel.fieldsHere[i])));
                        }
                        s.AppendFormat("            {0} c = new {0}(cx);" + CRLF, rel.tableHere.className);
                        s.AppendFormat("            SQLStatement stm = new SQLStatement(c);" + CRLF);
                        s.AppendFormat("            stm.where = \"{0}\";" + CRLF, params3_.join(" and "));
                        s.AppendFormat("            return c.selectDataTable(stm, p);" + CRLF);                        
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
                            EList<string> params_ = new EList<string>();
                            EList<string> params2_ = new EList<string>();
                            EList<string> params3_ = new EList<string>();
                            foreach (string f in c)
                            {
                                params_.Add(cw.fieldDotNetType(f) + " " + fixName(f));
                                params2_.Add(fixName(f));
                            }
                            string name = GetMethodName_ParamsPart(c);
                            string retType = isView ? "Table" : "TablePersist"; 
                            s.AppendFormat("        public static {3}<{0}> selectBy{1}(ORMContext cx, {2}) {{" + CRLF, table.className, name, params_.join(", "), retType);
                            s.AppendFormat("            DataTable tb = {2}.selectDataTableBy{0}(cx, {1});" + CRLF, name, params2_.join(", "), table.className);
                            s.AppendFormat("            return {0}.getInstancesFromDataTable<{0}>(cx, tb);" + CRLF, table.className);
                            s.Append("        }" + CRLF);

                            s.AppendFormat("        public static DataTable selectDataTableBy{0}(ORMContext cx, {1}) {{" + CRLF, name, params_.join(", "));
                            s.AppendFormat("            DBParams p = new DBParams();" + CRLF);
                            for (int i = 0; i < c.Count; i++)
                            {
                                s.AppendFormat("            p.Add(new DBParam(\"@{0}\", {1}));" + CRLF, fixName(c[i]), params2_[i]);
                                params3_.Add(string.Format("{0} = @{1}", this.db.escape(c[i]), fixName(c[i])));
                            }
                            s.AppendFormat("            {0} c = new {0}(cx);" + CRLF, table.className);
                            s.AppendFormat("            SQLStatement stm = new SQLStatement(c);" + CRLF);
                            s.AppendFormat("            stm.where = \"{0}\";" + CRLF, params3_.join(" and "));
                            s.AppendFormat("            return c.selectDataTable(stm, p);" + CRLF);
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
                            EList<string> params_ = new EList<string>();
                            EList<string> params2_ = new EList<string>();
                            EList<string> params3_ = new EList<string>();
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
                                params3_.Add(string.Format("{0} = @{1}", this.db.escape(c[i]), fixName(c[i])));
                            }
                            s.AppendFormat("            {0} c = new {0}(cx);" + CRLF, table.className);
                            s.AppendFormat("            SQLStatement stm = new SQLStatement(c);" + CRLF);
                            s.AppendFormat("            stm.where = \"{0}\";" + CRLF, params3_.join(" and "));
                            throw new NotImplementedException("Not yet implemented c.delete(stm, p)");
                            //s.AppendFormat("            return c.selectDataTable(stm, p);" + CRLF);
                            //s.Append("        }" + CRLF);
                            //s.Append(CRLF);
                            
                        }
                    }
                }

            }

        }


        #endregion basic generaor template


    }
}
