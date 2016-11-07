using System;
using System.Collections.Generic;
using System.Text;
using EM.Util;
using System.IO;
using EM.Collections;

namespace ProjectDeployPackage
{
    public class DatabaseObjects
    {
        public string server { get; set; }
        public string database { get; set; }
        public ESet<string> tables = new ESet<string>();
        public ESet<string> functions = new ESet<string>();
        public ESet<string> storedProcedures = new ESet<string>();
        public OrderedDictionary<string, ESet<string>> data = new OrderedDictionary<string, ESet<string>>();
        public ESet<string> jobs = new ESet<string>();
        public ESet<string> views = new ESet<string>();
        public OrderedDictionary<string, ESet<string>> versions = new OrderedDictionary<string, ESet<string>>();

        //public DatabaseObjects(string database): this("SQL", database) { }
        public DatabaseObjects(string server, string database)
        {
            this.database = database;
            this.server = server;
        }

        //public bool contains(string dbObjName, string fileName)
        //{
        //    string obj = dbObjName.Trim().ToLower();
        //    if ( obj.IndexOf("table") >= 0 ) { return contains(tables, fileName); }
        //    if (obj.IndexOf("func") >= 0) { return contains(functions, fileName); }
        //    if ( obj.IndexOf("view") >= 0 ) { return contains(views, fileName); }
        //    if ( obj.IndexOf("proc") >= 0 ) { return contains(storedProcedures, fileName); }
        //    if ( obj.IndexOf("job") >= 0 ) { return contains(jobs, fileName); }
        //    if ( obj.IndexOf("data") >= 0 ) { return contains(data, fileName); }
        //    return false;
        //}
        public bool contains(ESet<string> where, string fileName)
        {
            return where.Contains(fileName);
        }
        public bool contains(string version, string table)
        {
            return this.versions[version].Contains(table);
        }
        public string getDictKey()
        {
            return getDictKey(this.server, this.database);
        }
        public static string getDictKey(string server, string database)
        {
            return server + ":" + database;
        }
        public static DatabaseObjects createInstanceFromDictKey(string key)
        {
            string[] parts = key.Split(new char[] { ':'});
            return new DatabaseObjects(parts[0], parts[1]);
        }
    }

}
