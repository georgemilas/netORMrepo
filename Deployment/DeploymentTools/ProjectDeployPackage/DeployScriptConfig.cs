using System;
using System.Collections.Generic;
using System.Text;
using EM.Util;
using System.IO;
using EM.Collections;

namespace ProjectDeployPackage
{

    public class DeployScriptConfig
    {
        private SimpleConfigParser cfg { get; set; }
        public string deployFileDatabaseAndScopeSeparator = ":";

        public EDictionary<string, DatabaseObjects> databases = new EDictionary<string, DatabaseObjects>();

        private void addValue(string key, string version, OrderedDictionary<string, ESet<string>> collection)
        {
            addValue(key, collection.setdefault(version, new ESet<string>()));
        }
        private void addValue(string key, ESet<string> collection)
        {
            string value = cfg[key];
            string[] lst = value.Split('\n');
            foreach (string el in lst)            
            {
                if (el.Trim() != "")
                {
                    collection.Add(el.Trim());
                }
            }
        }

        private void addVersionScripts(string key, DatabaseObjects obj)
        {
            string value = cfg[key];
            string[] lst = value.Split('\n');
            string version = key.Replace(obj.database + deployFileDatabaseAndScopeSeparator, "");
            version = version.Replace(obj.server + deployFileDatabaseAndScopeSeparator, "");
            ESet<string> collection = obj.versions.setdefault(version, new ESet<string>());
            foreach (string el in lst)
            {
                if (el.Trim() != "")
                {
                    collection.Add(el.Trim());
                }
            }
        }

        public DeployScriptConfig(string path, string deployFileDatabaseAndScopeSeparator)
        {
            this.deployFileDatabaseAndScopeSeparator = deployFileDatabaseAndScopeSeparator;

            if (path != null && path.Trim() != "")
            {
                try
                {
                    FileInfo cfgFile = new FileInfo(path);
                    this.cfg = SimpleConfigParser.parse(cfgFile.FullName, true, true);

                    foreach (string key in cfg.Keys)
                    {
                        string[] parts = key.Split(new string[] { deployFileDatabaseAndScopeSeparator }, StringSplitOptions.None);
                        string server = "SQL";
                        string database;
                        string dbObject;
                        string dataScriptsVersion = "";
                        if (parts.Length > 2)
                        {
                            server = parts[0].Trim();
                            database = parts[1].Trim();
                            dbObject = parts[2].Trim().ToLower();
                            if (parts.Length > 3)
                            {
                                dataScriptsVersion = parts[3].Trim().ToLower();
                            }
                        }
                        else
                        {
                            database = parts[0].Trim();
                            dbObject = parts[1].Trim().ToLower();
                        }
                        DatabaseObjects defObj = new DatabaseObjects(server, database);
                        DatabaseObjects objs = databases.setdefault(defObj.getDictKey(), defObj);

                        switch (dbObject)
                        {
                            case "table":
                                addValue(key, objs.tables);
                                break;
                            case "sp":
                                addValue(key, objs.storedProcedures);
                                break;
                            case "view":
                                addValue(key, objs.views);
                                break;
                            case "job":
                                addValue(key, objs.jobs);
                                break;
                            case "data":
                                addValue(key, dataScriptsVersion, objs.data);
                                break;
                            case "func":
                                addValue(key, objs.functions);
                                break;
                            default:
                                addVersionScripts(key, objs);
                                break;
                        }
                    }
                }
                catch { }
            }
        }    

    }
}
