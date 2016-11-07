using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EM.Collections;
using EM.Util;
using System.IO;
using EM.Logging;
using TreeSync;
using System.Drawing;

namespace ProjectDeployPackage
{
    public class PackageDeployUtils
    {
        public string deployFileDatabaseAndScopeSeparator = ":";
        public OrderedDictionary<string, DatabaseObjects> databases;

        private Func<string, string, string> cb = (s1, s2) => Path.Combine(s1, s2);

        public PackageDeployUtils(string deployFileDatabaseAndScopeSeparator)
        {
            this.deployFileDatabaseAndScopeSeparator = deployFileDatabaseAndScopeSeparator;
            databases = new OrderedDictionary<string, DatabaseObjects>();
        }

        private static void addObjects(IEnumerable<string> src, ESet<string> dest)
        {
            foreach (var obj in src)
            {
                dest.Add(obj);
            }
        }
        private static void addObjects(OrderedDictionary<string, ESet<string>> src, OrderedDictionary<string, ESet<string>> dest)
        {
            foreach (string key in src.Keys)
            {
                dest.setdefault(key, new ESet<string>()).AddRange(src[key]);
            }
        }


        public void addDatabases(IDictionary<string, DatabaseObjects> additionalDatabases)
        {
            foreach(string db in additionalDatabases.Keys)
            {
                DatabaseObjects srcdb = additionalDatabases[db];
                string key = srcdb.getDictKey();
                if (this.databases.ContainsKey(key))
                {
                    DatabaseObjects resdb = this.databases[key];
                    addObjects(srcdb.tables, resdb.tables);
                    addObjects(srcdb.views, resdb.views);
                    addObjects(srcdb.storedProcedures, resdb.storedProcedures);
                    addObjects(srcdb.jobs, resdb.jobs);
                    addObjects(srcdb.data, resdb.data);
                    addObjects(srcdb.functions, resdb.functions);
                    if (srcdb.versions.Keys.Count > 0)
                    {
                        foreach (string version in srcdb.versions.Keys)
                        {
                            addObjects(srcdb.versions[version], resdb.versions.setdefault(version, new ESet<string>()));
                        }
                    }
                }
                else
                {
                    this.databases.Add(key, srcdb);
                }
            }
        }

        

        public void addDeployFileToDatabases(string src)
        {

            DeployScriptConfig srcCfg = new DeployScriptConfig(src, this.deployFileDatabaseAndScopeSeparator);
            foreach (var dbObjKey in srcCfg.databases.Keys)
            {
                DatabaseObjects srcdb = srcCfg.databases[dbObjKey];
                DatabaseObjects resdb = databases.setdefault(dbObjKey, DatabaseObjects.createInstanceFromDictKey(dbObjKey));
                addObjects(srcdb.tables, resdb.tables);
                addObjects(srcdb.views, resdb.views);
                addObjects(srcdb.storedProcedures, resdb.storedProcedures);
                addObjects(srcdb.jobs, resdb.jobs);
                addObjects(srcdb.data, resdb.data);
                addObjects(srcdb.functions, resdb.functions);
                if (srcdb.versions.Keys.Count > 0)
                {
                    foreach (string version in srcdb.versions.Keys)
                    {
                        addObjects(srcdb.versions[version], resdb.versions.setdefault(version, new ESet<string>()));
                    }
                }
            }
        }

        private void setKeyValue(SimpleConfigParser cfg, Action<string, string> loggerFunc, DatabaseObjects db, string name, ESet<string> collection, string version = null)
        {
            string key = db.server + this.deployFileDatabaseAndScopeSeparator +
                             db.database + this.deployFileDatabaseAndScopeSeparator +
                             name;
            if (version != null)
            {
                key = key + this.deployFileDatabaseAndScopeSeparator + version;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");
            foreach (string v in collection)
            {
                sb.AppendLine("\t\t" + v);
            }
            string value = sb.ToString();
            if (value.Trim() != "")
            {
                string msg = "Setting " + key + " as ";
                if (loggerFunc != null)
                {
                    loggerFunc(key, value);
                }
                cfg.Add(key, value);
            }
        }

        public void saveDeployFile(string destinationPath, Action<string, string> loggerFunc)
        {
            SimpleConfigParser cfg = SimpleConfigParser.empty();
            
            foreach (DatabaseObjects db in this.databases.Values)
            {
                setKeyValue(cfg, loggerFunc, db, "Table", db.tables);
                setKeyValue(cfg, loggerFunc, db, "View", db.views);
                setKeyValue(cfg, loggerFunc, db, "Func", db.functions);
                setKeyValue(cfg, loggerFunc, db, "SP", db.storedProcedures);
                setKeyValue(cfg, loggerFunc, db, "Job", db.jobs);
                foreach (string version in db.data.Keys)
                {
                    setKeyValue(cfg, loggerFunc, db, "Data", db.data[version], version);
                }
                foreach (string version in db.versions.Keys)
                {
                    setKeyValue(cfg, loggerFunc, db, version, db.versions[version]);
                }
            }

            if (File.Exists(destinationPath)) { File.Delete(destinationPath); }
            cfg.saveAs(destinationPath);            
        }

        public bool copyDatabaseScriptsFromConfig(string deployFile, string schema, string data, string crud, string rootDest)
        {
            return copyDatabaseScriptsFromConfig(deployFile, false, schema, data, crud, rootDest, null);
        }
        public bool copyDatabaseScriptsFromConfig(string deployFile, bool isPreview, string schema, string data, string crud, string rootDest, ILogger logger)
        {
            DeployScriptConfig cfg = new DeployScriptConfig(deployFile, this.deployFileDatabaseAndScopeSeparator);
            return copyDatabaseScriptsFromConfig(cfg, isPreview, schema, data, crud, rootDest, logger);
        }
        public bool copyDatabaseScriptsFromConfig(DeployScriptConfig cfg, bool isPreview, string schema, string data, string crud, string rootDest, ILogger logger)
        {
            if (logger == null) { logger = new Logger(); }

            int cnt = 1;
            
            Action<ESet<string>, DatabaseObjects, string, string> copy = (from, dbo, obj, srcRoot) =>
            {
                foreach (var file in from)
                {
                    string src = cb(cb(cb(srcRoot, dbo.database), obj), file);
                    string fdest = cnt.ToString().PadLeft(4, '0') + " " + file;
                    string destFolder = cb(cb(cb(rootDest, dbo.server), dbo.database), obj);
                    if (!Directory.Exists(destFolder)) { Directory.CreateDirectory(destFolder); }
                    string dest = cb(destFolder, fdest);
                    string msg = "extract " + dbo.server + "/" + dbo.database + "/" + obj + "/" + fdest;
                    if (isPreview)
                    {
                        logger.debug("SQL Deploy", msg);
                    }
                    else
                    {
                        try
                        {
                            Copy_OSToOS.copy(src, dest, msg, logger);
                        }
                        catch (Exception ex)
                        {
                            logger.error("SQL Deploy", "Error copying file. Source: " + src + ", Destination: " + dest, ex);
                        }
                    }
                    cnt++;
                }
            };

            Action<DatabaseObjects, string> copyCRUD = (dbo, version) =>
            {

                string srcRoot = cb(cb(cb(crud, version), dbo.database), "Stored Procedure");
                foreach (var crudTable in dbo.versions[version])
                {
                    //need underscores like in  *_tableName_* to make sure we don't get to much
                    foreach (var src in Directory.GetFiles(srcRoot, "*_" + crudTable + "_*"))
                    {
                        string file = Path.GetFileName(src);
                        string destFolder = cb(cb(cb(rootDest, dbo.server), dbo.database), "4. Stored Procedure CRUD");
                        if (!Directory.Exists(destFolder)) { Directory.CreateDirectory(destFolder); }
                        string dest = cb(destFolder, file);
                        string msg = "extract " + dbo.server + "/" + dbo.database + "/4. Stored Procedure CRUD/" + file;
                        if (isPreview)
                        {
                            logger.debug("SQL Deploy", msg);
                        }
                        else
                        {                            
                            try
                            {
                                Copy_OSToOS.copy(src, dest, msg, logger);
                            }
                            catch (Exception ex)
                            {
                                logger.error("SQL Deploy", "Error copying file. Source: " + src + ", Destination: " + dest, ex);
                            }
                        }
                    }
                }
            };

            try
            {
                if (Directory.Exists(rootDest)) { Directory.Delete(rootDest, true); }
                foreach (var dbObjsKey in cfg.databases.Keys)
                {
                    cnt = 1;
                    DatabaseObjects dbo = cfg.databases[dbObjsKey];
                    copy(dbo.tables, dbo, "1. Table", schema);
                    copy(dbo.functions, dbo, "2. Function", schema);
                    copy(dbo.views, dbo, "3. View", schema);
                    copy(dbo.storedProcedures, dbo, "4. Stored Procedure", schema);                    
                    foreach (string version in dbo.data.Keys)
                    {
                        string dataPath = data;
                        if (!String.IsNullOrWhiteSpace(version))
                        {
                            dataPath = cb(data, version);
                        }
                        copy(dbo.data[version], dbo, "5. Data", cb(data, dataPath));
                    }
                    copy(dbo.jobs, dbo, "6. SQL Jobs", schema);
                    if (dbo.versions.Keys.Count > 0)
                    {
                        foreach (string version in dbo.versions.Keys)
                        {
                            copyCRUD(dbo, version);
                        }
                    }
                }
                logger.info("SQL Deploy", "DONE - Succesfully created database scripts deployment package to " + rootDest);
                return true;
            }
            catch (Exception err)
            {
                logger.error("SQL Deploy", "Could not finish SQL deploy, an error occured", err);
                return false;
            }
        }

    }
}
