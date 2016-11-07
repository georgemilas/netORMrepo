using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;
using ORM.DBFields;
using ORM.db_store;

namespace ORM
{
    [Serializable]
    public class DBRelation : IComparable, IEquatable<DBRelation>
    {
        //TODO: add TableName as table entity in DBRelation
        public string relationName { get; set; }      //db constraint name, ex:  FK_tblHere_tblThere_keyHere 
        public TableName tableHere { get; set; }
        public TableName tableThere { get; set; }
        public int keysCount { get; set; }
        public EList<string> fieldsHere { get; set; }
        public EList<string> fieldsThere { get; set; }

        public delegate void AdditionalSQLAttributesBuilder(DBRelation rel, GenericField field);
        /// <summary>
        /// see additionalSQLAttributes property 
        /// </summary>
        public AdditionalSQLAttributesBuilder additionalSQLAttributesBuilder;

        protected SQLStatement _additionalSQLAttributes;
        protected string _fieldThere_name;

        public DBRelation()
        {
            this.fieldsHere = new EList<string>();
            this.fieldsThere = new EList<string>();
            this.tableHere = null;
            this.tableThere = null;
            this.additionalSQLAttributesBuilder = null;
        }

        /// <summary>
        /// a field from tableThere that can be shown in a combobox to the user as FK but when selecting the 
        /// actualy using fieldThere
        /// ex: html &lt;option value="fieldThere">fieldThere_name&lt;/option>
        /// </summary>
        public string fieldThere_name
        {
            get { return _fieldThere_name; }
            set { _fieldThere_name = value; }
        }

        /// <summary>
        /// usualy for a relation that you define manualy (for example is not a foreign key) 
        /// and need something more then just fieldHere, fieldsThere
        /// ex:
        ///     rel.tableHere = "tbl_rate"
        ///     rel.fieldsHere = "fld_type_id"      //the type rate from a more general statuses table not only for rates
        ///     
        ///     rel.tableThere = "tbl_status"
        ///     rel.fieldThere = "fld_id"
        ///     rel.additionalSQLAttributes = new SQLStatement();
        ///     rel.additionalSQLAttributes.where = "fld_status_group='RATE_TYPE'"
        ///     ==>  select fld_id as fld_type_id from tbl_status where fld_status_group='RATE_TYPE'
        /// 
        ///     or even
        ///     rel.additionalSQLAttributes.from = "join tbl_status_group T2 on tbl_status.fld_status_group = t2.fld_id"
        ///     -- additional from must not include the main from clause (aka from tableThere)
        ///     rel.additionalSQLAttributes.where = "T2.fld_id='RATE_TYPE'"
        ///     rel.additionalSQLAttributes.order = "fld_id DESC"
        ///     ==>  select fld_id as fld_type_id from tbl_status join tbl_status_group T2 on tbl_status.fld_status_group = t2.fld_id where T2.fld_id='RATE_TYPE' ORDER BY fld_id DESC
        /// 
        ///     ADITIONALY if you want to set the rel.additionalSQLAttributes later when you have values in the fields
        ///     you can do: 
        ///     rel.additionalSQLAttributesBuilder = a_function_of(DBRelation thisRel, GenericField relFieldHere) 
        ///         which you may use to populate thisRel.additionalSQLAttributes 
        ///     
        /// </summary>
        public SQLStatement additionalSQLAttributes 
        {
            get { return _additionalSQLAttributes; }
            set { _additionalSQLAttributes = value; }
        }

        public override string ToString()
        {
            return string.Format("DBRelation {0}:{2} to {1}:{3}", tableHere.sqlFromName , tableThere.sqlFromName , fieldsHere.ToString(), fieldsThere.ToString()) ;
        }

        
        #region IComparable Members

        public int CompareTo(object obj)
        {
            DBRelation t = (DBRelation)obj;
            if ( this.tableHere.CompareTo(t.tableHere) != 0) return 1;
            if (this.tableThere.CompareTo(t.tableThere) != 0) return 1;
            if ( this.fieldsHere.diference(t.fieldsHere).Count > 0 ) return 1;
            if ( this.fieldsThere.diference(t.fieldsThere).Count > 0 ) return 1;
            return 0;
        }
        /*
        public bool Equals(DBRelation obj)
        {
            return this.CompareTo(obj) == 0;
        }
        public override int GetHashCode()
        {
            //return (this.catalog + this.schema + this.table + this.className).GetHashCode();
            return this.tableHere.GetHashCode() | this.tableThere.GetHashCode() | this.fieldsHere.GetHashCode() | this.fieldsThere.GetHashCode();    //faster then above
        }
        */
        #endregion




        #region IEquatable<DBRelation> Members

        bool IEquatable<DBRelation>.Equals(DBRelation other)
        {
            return this.CompareTo(other) == 0;
        }

        #endregion
    }

}
