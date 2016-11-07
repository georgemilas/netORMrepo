using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;
using EM.DB;

using ORM.exceptions;
using System.Text.RegularExpressions;
using ORM.db_store.persitence;

namespace ORM
{
    [Serializable]
    public class SQLLimitOffsetHelper: EDictionary<string,string>
    {
        public bool limitApplied;
        public bool offsetApplied;
    }
    [Serializable]
    public class SQLStatement
    {
        public string fields;
        public string from;
        public string where;
        public string group;
        public string having;
        public string order;
        public bool distinct;
        public string customSql;
        
        public int limit;
        public SQLLimitOffsetHelper limitOffsetHelper;
        public int offset;
        

        protected GenericDatabase _db;
        protected TableRow tb;

        protected SQLStatement() 
        {
            this.fields = null;
            this.from = null;
            this.where = null;
            this.group = null;
            this.having = null;
            this.order = null;
            this.distinct = false;
            this.limit = -1;
            this.offset = -1;
            this.tb = null;
            this.limitOffsetHelper = new SQLLimitOffsetHelper();
            this.customSql = null;
        }
        public SQLStatement(TableRow tb) : this(tb.db) 
        {
            this.tb = tb;
        }
        public SQLStatement(GenericDatabase db) : this()
        {
            this.db = db;
        }

        public static SQLStatement getFromSQLQuery(GenericDatabase db, string sql)
        {
            EList<Regex> rl = new EList<Regex>();
            OrderedDictionary<string, Match> md = new OrderedDictionary<string, Match>();
            OrderedDictionary<string, string> sd = new OrderedDictionary<string, string>();
            rl.Add(new Regex(@"\s*select\s+", RegexOptions.Singleline | RegexOptions.IgnoreCase));
            rl.Add(new Regex(@"\s+from\s+", RegexOptions.Singleline | RegexOptions.IgnoreCase));
            rl.Add(new Regex(@"\s+where\s+", RegexOptions.Singleline | RegexOptions.IgnoreCase));
            rl.Add(new Regex(@"\s+group by\s+", RegexOptions.Singleline | RegexOptions.IgnoreCase));
            rl.Add(new Regex(@"\s+having\s+", RegexOptions.Singleline | RegexOptions.IgnoreCase));
            rl.Add(new Regex(@"\s+order by\s+", RegexOptions.Singleline | RegexOptions.IgnoreCase));
            rl.ForEach(delegate(Regex ex) { md.Add(ex.ToString(), ex.Match(sql)); });
            rl.ForEach(delegate(Regex ex) { sd.Add(ex.ToString(), md[ex.ToString()].Success ? sql.Substring(md[ex.ToString()].Index) : ""); });
            for (int i = 4; i >= 0; i--)
            {
                string sql_fld = sd[rl[i].ToString()];
                for (int j = i + 1; j <= 5; j++)
                {
                    if (sd[rl[j].ToString()] != "")
                    {
                        sql_fld = sql_fld.Replace(sd[rl[j].ToString()], "");
                    }
                }
                sd[rl[i].ToString()] = sql_fld;
            }

            SQLStatement stm = new SQLStatement(db);
            stm.fields = sd[rl[0].ToString()].Substring(md[rl[0].ToString()].Length);       //strip out the "select" word
            stm.from = sd[rl[1].ToString()];
            stm.where = sd[rl[2].ToString()];
            stm.group = sd[rl[3].ToString()];
            stm.having = sd[rl[4].ToString()];
            stm.order = sd[rl[5].ToString()];

            if (stm.fields.ToUpper().Contains(" DISTINCT "))
            {
                stm.distinct = true;
            }

            return stm;
        }

        public GenericDatabase db
        {
            get
            {
                if (_db == null) { throw new NullReferenceException("Database was not initialized"); }
                return _db;
            }
            set { this._db = value; }
        }

        public virtual string selectSql()
        {
            SQLStatement stm = this;

            if (stm.customSql != null)
            {
                //maybe the user set's this from outside 
                return stm.customSql;
            }

            if (stm.limit > 0 || stm.offset > 0)
            {
                //may be null reference if this.tb was not set
                stm = db.limitAndOffset(this.tb.pk, stm);    //may set stm.customSql;
            }
            if (stm.customSql != null)
            {
                //db.limitAndOffset may set stm.customSql
                return stm.customSql;
            }
            else
            {
                if (stm.from != null && stm.from.Trim() != "" && !stm.from.Trim().ToLower().StartsWith("from")) stm.from = "FROM " + stm.from;
                if (stm.where != null && stm.where.Trim() != "" && !stm.where.Trim().ToLower().StartsWith("where")) stm.where = "WHERE " + stm.where;
                if (stm.order != null && stm.order.Trim() != "" && !stm.order.Trim().ToLower().StartsWith("order by")) stm.order = "ORDER BY " + stm.order;
                if (stm.group != null && stm.group.Trim() != "" && !stm.group.Trim().ToLower().StartsWith("group by")) stm.group = "GROUP BY " + stm.group;
                if (stm.having != null && stm.having.Trim() != "" && !stm.having.Trim().ToLower().StartsWith("having")) stm.having = "HAVING " + stm.having;
                return string.Format("SELECT{0} {1} {2} {3} {4} {5} {6}", (stm.distinct && !stm.fields.ToUpper().Contains(" DISTINCT ")) ? " DISTINCT" : "", stm.fields, stm.from, stm.where, stm.group, stm.having, stm.order);
            }
        }

        public virtual string deleteSql()
        {
            if (this.from != null && this.from.Trim() != "" && !this.from.Trim().ToLower().StartsWith("from")) this.from = "FROM " + this.from;
            if (this.where != null && this.where.Trim() != "" && !this.where.Trim().ToLower().StartsWith("where")) this.where = "WHERE " + this.where;
            return string.Format("DELETE {0} {1}", this.from, this.where);
        }

        public virtual string updateSql()
        {
            if (this.from != null && this.from.Trim() != "" && this.from.Trim().ToLower().StartsWith("from")) this.from = this.from.Substring(4);   //delete the from keyword if there
            if (this.where != null && this.where.Trim() != "" && !this.where.Trim().ToLower().StartsWith("where")) this.where = "WHERE " + this.where;
            return string.Format("UPDATE {0} SET {1} {2}", this.from, this.fields, this.where);

        }

        public virtual string insertSql()
        {
            if (this.from != null && this.from.Trim() != "" && this.from.Trim().ToLower().StartsWith("from")) this.from = this.from.Substring(4);   //delete the from keyword if there
            if (this.fields == null || this.fields.Trim() == "" && !this.fields.Trim().ToLower().Contains(" values ")) throw new SQLSintaxException("fields atributes where not in corect format: (f1, f2,...) values (@p1, @p2,...) ");
            return string.Format("INSERT INTO {0} {1}", this.from, this.fields);
        }


    }
}
