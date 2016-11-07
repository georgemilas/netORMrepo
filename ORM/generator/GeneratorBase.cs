using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using EM.Collections;
using EM.DB;
using ORM.db_store;
using ORM.db_store.persitence;
using ORM.exceptions;

namespace ORM.generator
{

    /// <summary>
    /// - Generates DAL for specified database objects and their related objects under a folder named DB_PARTIAL
    /// - files generated under CLS_PARTIAL folder (built only ones) could be used to implement business logic 
    /// - if inheriting (maybe in a diferent DLL) for business logic, an ORM.generator.ORMClassFactory should be specified, 
    ///   so that it will instantiate polimorficaly from the inheritated classes in case of select statements
    /// </summary>
    public abstract class GeneratorBase
    {
        public enum PartialType { CLS_PARTIAL, DB_PARTIAL, DB_INTERFACE, DB_CRUD };
        protected EDictionary<PartialType, string> partial;

        public const string CRLF = StringUtil.CRLF;

        public bool IsDoComputeAllNeededTables { get; set; }

        protected ORMContext context;
        private GenericDatabase _db;
        public virtual GenericDatabase db
        {
            get { return _db; }
            set 
            { 
                _db = value;
                context = new ORMContext();
                context.db = value;
            }
        }
        public delegate string FilePathBuilder(string className, string catalogName, string schemaName, PartialType partialType, string classType, string rootOutputFolder, string rootNameSpaceFolder, EDictionary<PartialType, string> partialDict);
        
        private FilePathBuilder _filePathBuilder;
        /// <summary>
        /// Use it to customize the way the files it creates are organized on the hard disk
        /// </summary>
        public virtual FilePathBuilder filePathBuilder
        {
            get { return _filePathBuilder; }
            set { _filePathBuilder = value; }
        }


        public GeneratorBase()
        {
            this.IsDoComputeAllNeededTables = true;

            this.partial = new EDictionary<PartialType, string>();
            this.partial.Add(PartialType.DB_PARTIAL, "DB_PARTIAL");
            this.partial.Add(PartialType.CLS_PARTIAL, "CLS_PARTIAL");
            this.partial.Add(PartialType.DB_INTERFACE, "DB_INTERFACE");
            this.partial.Add(PartialType.DB_CRUD, "DB_CRUD");
            
            this.filePathBuilder = this.internalFilePathBuilder;
        }
        public GeneratorBase(GenericDatabase db) : this()
        {
            this.db = db;            
        }

        /// <summary>
        /// when assigned the generator may provide version numbers to some of it's generated content
        /// </summary>
        public virtual string versionNumber { get; set; } 

        protected string fixName(string name)
        {
            return ORMContext.fixName(name);
        }

        protected string getPropName(string fldName, TableName tbl)
        {
            if (tbl != null)
            {
                return tbl.getCustomFieldPropertyName(fldName);
            }
            return fixName(fldName);
        }
        
        #region write file

        protected string _outputFolder;
        /// <summary>
        /// the folder where it writes the classes it creates (by default if not given is C:\Test)
        /// </summary>
        public virtual string outputFolder
        {
            get 
            {
                if (this._outputFolder == null)
                {
                    this.outputFolder = "C:\\TEST";
                }
                return this._outputFolder;
            }
            set 
            { 
                this._outputFolder = value; 
                if (Directory.Exists(this._outputFolder)) 
                {
                    Directory.CreateDirectory(this._outputFolder);
                }
            }
        }

        private string _rootNameSpace;
        /// <summary>
        ///  - All classes generated will belong to  rootNamespace.Database.Schema
        ///  - ex:  DAL or MyCompany.Data.GenDAL
        /// </summary>
        public virtual string rootNameSpace
        {
            get { return _rootNameSpace; }
            set { _rootNameSpace = value; }
        }

        private string _rootNameSpaceFolder = null;
        /// <summary>
        /// - what folder to create for the given rootNamespace
        /// - by default creates a folder for each namespace and subnamespace
        ///     MyCompany.Data.GenDAL -> MyCompany\Data\GenDAL
        /// - use it to make less folders for example
        /// </summary>
        public virtual string rootNameSpaceFolder
        {
            get 
            {
                if (_rootNameSpaceFolder == null)
                {
                    return this.rootNameSpace.Replace('.', '\\');
                }
                else
                {
                    return _rootNameSpaceFolder;
                }
            }
            set { _rootNameSpaceFolder = value; }
        }


        protected string internalFilePathBuilder(string className, string catalogName, string schemaName, PartialType partialType, string classType, string rootOutputFolder, string nameSpaceFolder, EDictionary<PartialType, string> partial)
        {
            string outFolder = rootOutputFolder + "\\" + nameSpaceFolder;
            string folder = outFolder;
            if ( partialType == PartialType.DB_CRUD )
            {
                // ...\DB_CRUD\SurePayroll\Stored Procedures
                folder += "\\" + partial[partialType];
                if (catalogName != null)
                {
                    folder += "\\" + catalogName;
                }
                if (classType != null)
                {
                    folder += "\\" + classType;
                }    
            }
            else
            {
                // ...\SurePayroll\DB_PARTIAL\dbo\Tables
                if (catalogName != null)
                {
                    folder += "\\" + catalogName;
                }
                folder += "\\" + partial[partialType];
                if (schemaName != null)
                {
                    folder += "\\" + schemaName;
                }
                if (classType != null)
                {
                    folder += "\\" + classType;
                }
            }
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            if (partialType == PartialType.DB_CRUD)
            {
                return folder + "\\" + className + ".sql";
            }
            else
            {
                return folder + "\\" + className + ".cs";
            }

        }

        protected void writeFile(TableName t, PartialType partialType, string content, string classType) { writeFile(t.className, t.catalog, t.schema, partialType, content, classType); }
        protected void writeFile(string className, PartialType partialType, string content, string classType) { writeFile(className, null, null, partialType, content, classType); }
        protected void writeFile(string className, string catalogName, string schemaName, PartialType partialType, string content, string classType)
        {
            string filePath = this.filePathBuilder(className, catalogName, schemaName, partialType, classType, this.outputFolder, this.rootNameSpaceFolder, this.partial);

            if (partialType == PartialType.CLS_PARTIAL && File.Exists(filePath))
            {
                Console.WriteLine("SKIP existing CLS file :" + filePath);
            }
            else
            {
                CheckFileAttributes(filePath);
                StreamWriter sw = new StreamWriter(filePath, false); //don't append, overwrite
                sw.Write(content);
                sw.Close();
                Console.WriteLine("wrote file :" + filePath);
            }
        }

        #region File Attributes Utilities
        private static void CheckFileAttributes(string filePath)
        {
            if (!File.Exists(filePath)) return;
            
            var attributes = File.GetAttributes(filePath);
            if ((attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly) return;
            
            // Make the file RW
            attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
            File.SetAttributes(filePath, attributes);
            Console.WriteLine("Removed ReadOnly Attribute file :" + filePath);
        }

        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        //private static void CreateNodes(ItemCollection nodes)
        //{
        //    using (var tfs = TeamFoundationServerFactory.GetServer("http://devsource:8080/tfs"))
        //    {
        //        var versionControlServer = tfs.GetService(typeof(VersionControlServer)) as VersionControlServer;
        //        versionControlServer.NonFatalError += OnNonFatalError;

        //        // Create a new workspace for the currently authenticated user.             
        //        var workspace = versionControlServer.CreateWorkspace("Temporary Workspace", versionControlServer.AuthenticatedUser);

        //        try
        //        {
        //            // Check if a mapping already exists.
        //            var workingFolder = new WorkingFolder("$/testagile", @"c:\tempFolder");

        //            // Create the mapping (if it exists already, it just overides it, that is fine).
        //            workspace.CreateMapping(workingFolder);

        //            // Go through the folder structure defined and create it locally, then check in the changes.
        //            CreateFolderStructure(workspace, nodes, workingFolder.LocalItem);

        //            // Check in the changes made.
        //            workspace.CheckIn(workspace.GetPendingChanges(), "This is my comment");
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //        }
        //        finally
        //        {
        //            // Cleanup the workspace.
        //            workspace.Delete();

        //            // Remove the temp folder used.
        //            Directory.Delete("tempFolder", true);
        //        }
        //    }
        //}

        //private static void CreateFolderStructure(Workspace workspace, ItemCollection nodes, string initialPath)
        //{
        //    foreach (RadTreeViewItem node in nodes)
        //    {
        //        var newFolderPath = initialPath + @"\" + node.Header;
        //        Directory.CreateDirectory(newFolderPath);
        //        workspace.PendAdd(newFolderPath);
        //        if (node.HasItems)
        //        {
        //            CreateFolderStructure(workspace, node.Items, newFolderPath);
        //        }
        //    }
        //}
        #endregion File Attributes Utilities
        
        #endregion write file


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //// compute nedded tables
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region compute nedded tables

        public ESet<TableName> stringToTableName(ESet<string> tables, bool areViews)
        {
            ESet<TableName> res = new ESet<TableName>();

            EList<TableName> alltables;
            if (areViews) alltables = db.views;
            else alltables = db.tables;

            foreach (TableName t in alltables)
            {
                if (tables.Contains(t.table))
                {
                    res.Add(t);
                }
            }
            return res;
        }

        protected string getClassName(TableName table, EList<TableName> allNeddedTables)
        {
            return this.db.getClassName(table, allNeddedTables);
        }

        /// <summary>
        /// return all specified tables plus all needed tables found via relationships like foreign key constraints
        /// </summary>
        public ESet<TableName> computeAllNeededTables(ESet<TableName> tables, bool areViews)
        {
            if (!IsDoComputeAllNeededTables)
            {
                return tables;
            }

            ESet<TableName> workTables = new ESet<TableName>();
            workTables.AddRange(tables);

            EList<TableName> alltables;
            if (areViews) alltables = db.views;
            else alltables = db.tables;

            ESet<TableName> allNeeded = new ESet<TableName>();
            ESet<TableName> related = new ESet<TableName>();

            while (workTables.Count > 0)
            {
                foreach (TableName t in alltables)
                {
                    if (workTables.Contains(t))
                    {
                        allNeeded.Add(workTables[workTables.IndexOf(t)]);
                        FKInfo fk = this.db.fk(t);  //related tables
                        foreach (DBRelation r in fk)
                        {
                            if (!workTables.Contains(r.tableThere) && !allNeeded.Contains(r.tableThere))
                            {   //add related if not already there
                                related.Add(r.tableThere);    
                            }
                        }
                    }
                }

                workTables.Clear();
                if (related.Count > 0)
                {
                    workTables.AddRange(related.GetRange(0, related.Count));
                }
                related.Clear();

            }

            //allNeeded.ForEach(delegate(TableName t) {
            //    //set correct class name in case of dupliacte table names from multiple schemas
            //    t.className = getClassName(t, allNeeded);   
            //});
            return allNeeded;
        }

        public delegate bool DatabaseObjectSkipRule(TableName table, bool isView);
        private DatabaseObjectSkipRule _isSkipDatabaseObject;
        /// <summary>
        /// example:
        /// delegate(TableName table, isView)
        /// {
        ///     string tname = table.table.Trim().ToLower();
        ///     if ( !isView && tname.StartsWith("tbl_tmb") return true;           
        /// 
        ///     if ( !isView && tname.StartsWith("tbl_") return false;
        ///     if ( isView && tname.StartsWith("vw_") return false;
        ///     
        ///     return true;    //skip all trash
        /// }
        /// </summary>
        public virtual DatabaseObjectSkipRule isSkipDatabaseObject
        {
            get { return _isSkipDatabaseObject; }
            set { _isSkipDatabaseObject = value; }
        }

        /// <summary>
        /// - return a dictionary of  { tableName :  oneToMany related DBRelation }
        /// </summary>
        public OrderedDictionary<TableName, ESet<DBRelation>> computeOneToManyRalationships(EList<TableName> allNeededTables)
        {
            //first compute oneToMany relationships on given allNeededTables,
            //FK we already known but we need the inverce of it aka oneToMany
            OrderedDictionary<TableName, ESet<DBRelation>> oneToMany = new OrderedDictionary<TableName, ESet<DBRelation>>();
            //Console.WriteLine("Compute oneToMany: ");

            if (IsDoComputeAllNeededTables)
            {
                for (int i = 0; i < allNeededTables.Count; i++)
                {
                    if (isSkipDatabaseObject != null && isSkipDatabaseObject(allNeededTables[i], false)) continue;

                    //Console.WriteLine(string.Format("table {0} of {1}: {2} FK", i, allNeededTables.Count, allNeededTables[i].table));
                    FKInfo fk = this.db.fk(allNeededTables[i]);
                    foreach (DBRelation rel in fk)
                    {
                        remapTablesInRelationFromAllNeddetables(allNeededTables, rel);
                        rel.tableThere.className = getClassName(rel.tableThere, allNeededTables);
                        rel.tableHere.className = getClassName(rel.tableHere, allNeededTables);
                        oneToMany.setdefault(rel.tableThere, new ESet<DBRelation>()).Add(rel);
                    }
                }
            }
            return oneToMany;
        }

        private static void remapTablesInRelationFromAllNeddetables(EList<TableName> allNeededTables, DBRelation rel)
        {
            if (allNeededTables != null)
            {
                int ixThere = allNeededTables.FindIndex(tt => rel.tableThere.Equals(tt));
                int ixHere = allNeededTables.FindIndex(th => rel.tableHere.Equals(th));

                if (ixThere >= 0) { rel.tableThere = allNeededTables[ixThere]; }
                if (ixHere >= 0) { rel.tableHere = allNeededTables[ixHere]; }
            }
        }
        #endregion compute nedded tables


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //// ensure unique relation name
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region ensure unique relation name

        protected bool sameFields(EList<string> fields, EList<string> fields2)
        {
            foreach (string f in fields)
            {
                if (!fields2.Contains(f)) { return false; }
            }
            foreach (string f in fields2)
            {
                if (!fields.Contains(f)) { return false; }
            }
            return true;
        }

        protected string GetMethodName_ParamsPart(List<string> fields)
        {
            return ORMContext.GetMethodName_ParamsPart(fields);            
        }

        /// <summary>
        /// ensure unique relation name if 2 or more relations with one table exists
        /// </summary>
        protected string getDBRelationName(string name, DBRelation rel, OrderedDictionary<DBRelation, string> namesCache, EDictionary<string, EList<DBRelation>> existingNames)
        {
            if (namesCache.ContainsKey(rel))
            {
                return namesCache[rel];
            }
            else
            {
                EList<DBRelation> existing = existingNames.setdefault(name, new EList<DBRelation>());
                if (existing.Count >= 1)
                {
                    EList<string> difFields = new EList<string>();
                    difFields.AddRange(rel.fieldsHere);
                    foreach (DBRelation r in existing)
                    {
                        difFields = difFields.left_diference(r.fieldsHere);
                    }
                    if (difFields.Count == 0)
                    {
                        difFields.AddRange(rel.fieldsThere);
                        foreach (DBRelation r in existing)
                        {
                            difFields = difFields.left_diference(r.fieldsThere);
                        }
                    }
                    if (difFields.Count == 0)
                    {
                        throw new ORM.exceptions.ORMException("can not make a new name for DBRelation " + rel.ToString());
                    }
                    else
                    {
                        existing.Add(rel);
                        //name = name + "_" + difFields.join("_");
                        name = name + "_" + GetMethodName_ParamsPart(difFields);
                    }
                }
                else
                {
                    existing.Add(rel);
                }

                namesCache[rel] = name;
                return name;
            }
        }

        protected string getFKDBRelationName(DBRelation rel, OrderedDictionary<DBRelation, string> namesCache, EDictionary<string, EList<DBRelation>> existingNames)
        {
            string name = fixName(rel.tableThere.className);
            return getDBRelationName(name, rel, namesCache, existingNames);
        }
        protected string getOtoMDBRelationName(DBRelation rel, OrderedDictionary<DBRelation, string> namesCache, EDictionary<string, EList<DBRelation>> existingNames)
        {
            string name = fixName(rel.tableHere.className);
            return getDBRelationName(name, rel, namesCache, existingNames);
        }

        protected EList<DBConstraint> DoUniqueConstraintList(TableName table, EList<DBConstraint> constraints, PKInfo pk, FKInfo fk, OrderedDictionary<TableName, ESet<DBRelation>> oneToMany, ESet<DBConstraint> customLoad)
        {
            EList<DBConstraint> res = new EList<DBConstraint>();
            EList<DBConstraint> seen = new EList<DBConstraint>();

            EList<DBConstraint> allUnique = constraints.union(customLoad);

            foreach (DBConstraint c in allUnique)
            {
                if (sameFields(c, pk)) continue;

                bool doit = true;
                foreach (DBRelation rel in fk)
                {
                    if ( sameFields(c, rel.fieldsHere) )
                    {
                        doit = false;
                        break;
                    }
                }
                if (oneToMany.get(table, null) != null)
                {
                    foreach (DBRelation rel in oneToMany[table])
                    {
                        if (sameFields(c, rel.fieldsThere))
                        {
                            doit = false;
                            break;
                        }
                    }
                }
                foreach (DBConstraint c2 in seen)
                {
                    if ( sameFields(c, c2) )
                    {
                        doit = false;
                        break;
                    }
                }
                seen.Add(c);
                if (doit)
                {
                    res.Add(c);
                }
            }

            return res;

        }

        protected EList<DBConstraint> DoCustomSelectList(TableName table, ESet<DBConstraint> customSelect, FKInfo fk )
        {
            EList<DBConstraint> res = new EList<DBConstraint>();
            EList<DBConstraint> seen = new EList<DBConstraint>();

            foreach (DBConstraint c in customSelect)
            {
                bool doit = true;
                foreach (DBRelation rel in fk)
                {
                    if (sameFields(c, rel.fieldsHere))
                    {
                        doit = false;
                        break;
                    }
                }
                foreach (DBConstraint c2 in seen)
                {
                    if (sameFields(c, c2))
                    {
                        doit = false;
                        break;
                    }
                }
                seen.Add(c);
                if (doit)
                {
                    res.Add(c);
                }
            }

            return res;

        }

        #endregion ensure unique relation name


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //// main public API - DB level
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region main public API - DB level
        /// <summary>
        /// build specified tables and all the other ones that are needed for the relationships
        /// </summary>
        public ESet<TableName> buildTables(ESet<string> tables) { return _buildTables(tables, false); }
        public ESet<TableName> buildTables(ESet<TableName> tables) { return _buildTables(tables, false); }

        /// <summary>
        /// build specified views 
        /// </summary>
        public ESet<TableName> buildViews(ESet<string> views) { return _buildTables(views, true); }
        public ESet<TableName> buildViews(ESet<TableName> views) { return _buildTables(views, true); }


        private ESet<TableName> _buildTables(ESet<string> tables, bool areViews) { return _buildTables(stringToTableName(tables, areViews), areViews); }
        private ESet<TableName> _buildTables(ESet<TableName> tables, bool areViews)
        {
            ESet<TableName> allNedded = computeAllNeededTables(tables, areViews);
            ESet<TableName> processed = this.buildDatabaseObjects(allNedded, areViews);

            foreach (TableName t in processed)
            {
                string s = this.buildExtensionPartialClass(t);
                Console.WriteLine("built " + t.className);
                writeFile(t, PartialType.CLS_PARTIAL, s, areViews ? "VIEW" : "TABLE");
            }
            return processed;
        }

        /// <summary>
        /// build all tables and views in the database
        /// </summary>
        public void buildDatabaseTablesAndViews()
        {
            EList<TableName> tables = this.db.tables;
            EList<TableName> views = this.db.views;
            tables.ForEach(delegate(TableName t) { t.className = getClassName(t, tables.union(views)); });
            views.ForEach(delegate(TableName t) { t.className = getClassName(t, tables.union(views)); });
            buildDatabaseObjects(tables, false);
            buildDatabaseObjects(views, true);

            Console.WriteLine("DONE, press enter to exit");
            Console.ReadLine();
        }

 
        /// <summary>
        /// - build a list of TableName's
        /// </summary>
        protected ESet<TableName> buildDatabaseObjects(EList<TableName> allNeededTables, bool views)
        {
            OrderedDictionary<TableName, ESet<DBRelation>> oneToMany = computeOneToManyRalationships(allNeededTables);
            ESet<TableName> processed = new ESet<TableName>();
            for (int i = 0; i < allNeededTables.Count; i++)
            {   
                //don't do temporary allNeededTables
                if (!views && isSkipDatabaseObject != null && isSkipDatabaseObject(allNeededTables[i], false)) continue;
                if (views && isSkipDatabaseObject != null && isSkipDatabaseObject(allNeededTables[i], true)) continue;

                //write the backed class: 
                string tableType = views ? "view" : "table";
                Console.Write(string.Format ("creating {3} {0} of {1}: {2} ",i,allNeededTables.Count, allNeededTables[i].className, tableType));
                buildTable(allNeededTables[i], views, oneToMany, allNeededTables);
                processed.Add(allNeededTables[i]);
                Console.WriteLine(" DONE");
            }
            return processed;

        }

        #endregion main public API - DB level


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //// main generaor API - individual object level
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region main generaor API - individual object level

        protected void _buildTable(TableName table, bool isView, OrderedDictionary<TableName, ESet<DBRelation>> oneToMany, EList<TableName> allNeededTables)
        {
            ESet<DBConstraint> customLoad = null;
            ESet<DBConstraint> customSelect = null;
            ESet<DBConstraint> customDelete = null;

            if (table is GenTable)
            {
                GenTable gt = (GenTable)table;
                customLoad = gt.customLoad;
                customSelect = gt.customSelect;
                customDelete = gt.customDelete;
            }

            if (customLoad == null) { customLoad = new ESet<DBConstraint>(); }
            if (customSelect == null) { customSelect = new ESet<DBConstraint>(); }
            if (customDelete == null) { customDelete = new ESet<DBConstraint>(); }

            table.context = this.context;

            string tableName = table.table;
            PKInfo pk = this.db.pk(table);

            FKInfo fk = new FKInfo(table);
            if (IsDoComputeAllNeededTables)
            {
                fk = this.db.fk(table);
            }


            TableColumnsInfo tci = this.db.columns(table);
            TableColumnsWrap cw = new TableColumnsWrap(this.db, tci);

            ESet<string> schemas = new ESet<string>();
            foreach (DBRelation rel in fk)
            {
                remapTablesInRelationFromAllNeddetables(allNeededTables, rel);
                rel.tableHere.context = table.context;
                rel.tableThere.context = table.context;
                string s1 = table.catalog + "." + table.schema;
                string s2 = rel.tableThere.catalog + "." + rel.tableThere.schema;
                if (s1 != s2) schemas.Add(s2);
            }
            foreach (DBRelation rel in oneToMany.get(table, new ESet<DBRelation>()))
            {
                rel.tableHere.context = table.context;
                rel.tableThere.context = table.context;
                string s1 = table.catalog + "." + table.schema;
                string s2 = rel.tableHere.catalog + "." + rel.tableHere.schema;
                if (s1 != s2) schemas.Add(s2);
            }

            //cache helpers for name clashes in fk relations and one to many relation
            OrderedDictionary<DBRelation, string> fkNamesCache = new OrderedDictionary<DBRelation, string>();
            OrderedDictionary<DBRelation, string> otmNamesCache = new OrderedDictionary<DBRelation, string>();
            EDictionary<string, EList<DBRelation>> fkExisting = new EDictionary<string, EList<DBRelation>>();
            EDictionary<string, EList<DBRelation>> otmExisting = new EDictionary<string, EList<DBRelation>>();

            StringBuilder s = new StringBuilder();
            string namesp = addFileNamespace(table.catalog, table.schema, s, schemas);
            if (isView) { addStartClassFile(table, s); }
            else { addStartClassFilePersist(table, s); }            
            addDBRelationsDeclarations(table, oneToMany, pk, fk, s, fkNamesCache, otmNamesCache, fkExisting, otmExisting);
            addDBConstructorStart(table, cw, s, isView, namesp);
            addDBConstructorDBRelationsPart(table, oneToMany, pk, fk, s, isView);
            addDBConstructorFinish(s);
            addConstructorPK(table, pk, cw, s);
            EList<DBConstraint> constraints = DoUniqueConstraintList(table, this.db.uniqueConstraints(table), pk, fk, oneToMany, customLoad);
            addUniqueConstraintConstructors(table, constraints, cw, s);
            addDBOneToManyConstructors(table, pk, oneToMany, cw, s);
            addDBForeignKeysConstructors(table, fk, cw, s);
            addCustomSelectConstructors(table, fk, cw, s, customSelect, isView);
            addCustomDeleteMethods(table, fk, cw, s, customDelete);
            addFieldsProperties(cw, s, table);
            addDBForeignKeysProprieties(table, pk, fk, s, fkNamesCache, fkExisting, cw);
            addDBOneToManyProprieties(table, oneToMany, pk, s, otmNamesCache, otmExisting, cw);
            addFinishClassFile(s);

            writeFile(table, PartialType.DB_PARTIAL, s.ToString(), isView ? "VIEW" : "TABLE");

            buildInterface(table, cw, isView ? "VIEW" : "TABLE");

        }

        /// <summary>
        /// you should not need this but either buildTables or buildDatabaseObjects instead   (which uses this one internaly)
        /// </summary>
        public virtual void buildTable(TableName table, bool isView, OrderedDictionary<TableName, ESet<DBRelation>> oneToMany, EList<TableName> allNeededTables)
        {
            this._buildTable(table, isView, oneToMany, allNeededTables);
        }
        
        

        public void buildFromColumnsInfo(TableName table, bool isView, TableColumnsInfo tci) { buildFromColumnsInfo(table, isView, tci, false); }
        public void buildFromColumnsInfo(TableName table, bool isView, TableColumnsInfo tci, bool generateExtensionPartialClass)
        {
            StringBuilder s = new StringBuilder();
            string namesp = addFileNamespace(table.catalog, table.schema, s);
            addStartClassFile(table, s);
            TableColumnsWrap cw = new TableColumnsWrap(this.db, tci);
            addConstructor(table, cw, s, true, namesp, true);
            addFieldsProperties(cw, s, table);
            s.Append(CRLF);
            addFinishClassFile(s);

            writeFile(table, PartialType.DB_PARTIAL, s.ToString(), null);
            
            if (generateExtensionPartialClass)
            {
                string icls = this.buildExtensionPartialClass(table);
                writeFile(table, PartialType.CLS_PARTIAL, icls, null);
            }

            buildInterface(table, cw, null);
        }


        public void buildSelectDataSetStoredProc(TableName procName) { buildSelectDataSetStoredProc(procName, null, false); }
        public void buildSelectDataSetStoredProc(TableName procName, bool useOutputParamAsInputOutput) { buildSelectDataSetStoredProc(procName, null, useOutputParamAsInputOutput); }
        public void buildSelectDataSetStoredProc(TableName mainProcName, DBParams param, bool useOutputParamAsInputOutput)
        {
            StoredProcDataSetDef sp = this.db.storedProcedure<StoredProcDataSetDef>(mainProcName);
            sp.defaultParameters = param;
            ORMContext context = new ORMContext();
            context.db = this.db;
            context.classFactory = new ORMClassFactory(Assembly.GetExecutingAssembly());

            //write main file
            StringBuilder s = new StringBuilder();
            string namesp = addFileNamespace(mainProcName.catalog, mainProcName.schema, s);


            Func<int, TableName> getInnerTableName = (tix) =>
            {
                var nm = String.Format("{0}_Table{1}", mainProcName.table, tix);
                TableName procName = new TableName(mainProcName.catalog, mainProcName.schema, nm, nm, mainProcName.classNamespace, "I" + nm);
                if (mainProcName is DataSetSP)
                {
                    procName = ((DataSetSP)mainProcName).tables.get(tix - 1, procName);
                }
                return procName;
            };

            int ix = 0;
            EList<string> reTypes = new EList<string>();
            foreach (var ci in sp.resultsetColumnsDefinition)
            {
                ix++;
                TableName tn = getInnerTableName(ix);
                var nm = String.Format("Table<{0}>", tn.className);
                reTypes.Add(nm);
            }
            string retType = mainProcName.className + "Result";

            //Result Class
            s.Append(String.Format("{0}    public class {1}  {{{0}", CRLF, retType));
            ix = 1;
            foreach(var rt in reTypes) 
            {
                s.Append(String.Format("        public {1} Item{2} {{ get; set; }}{0} ", CRLF, rt, ix));
                ix++;
            }
            s.Append(String.Format("    }}{0}", CRLF));
            

            //Main Stored Proc Class
            s.Append(String.Format("{0}    public partial class {1}  {{{0}", CRLF, mainProcName.className));
            s.Append(String.Format("{0}        public ORMContext context;{0}", CRLF));
            s.Append(String.Format("{0}        public TableName dbObjectName;{0}", CRLF));

            s.AppendFormat("        public {0}(ORMContext context) {{" + CRLF, mainProcName.className);
            s.Append("            this.context = context;" + CRLF);
            //s.AppendFormat("            this.dbObjectName = new TableName({0},{1},{2},{3},\"{4}\",{5});\n", 
            s.AppendFormat("            this.dbObjectName = new TableName({0},{1},{2},{3},\"{4}\");" + CRLF,
                        mainProcName.catalog != null ? '"' + mainProcName.catalog + '"' : "null",
                        mainProcName.schema != null ? '"' + mainProcName.schema + '"' : "null",
                        mainProcName.table != null ? '"' + mainProcName.table + '"' : "null",
                        mainProcName.className != null ? '"' + mainProcName.className + '"' : "null",
                        computeNamespace(this.rootNameSpace, mainProcName)//,
                //procName.interfaceName != null ? '"' + procName.interfaceName + '"' : "null"
                        );
            s.Append("            this.dbObjectName.context = this.context;" + CRLF);
            s.Append("        }" + CRLF);

            s.Append(CRLF);
            EList<string> spCall = new EList<string>();
            EList<string> spCall2 = new EList<string>();
            foreach (StoredProcParam p in sp.parameters)
            {
                spCall.Add((p.isOutputParam ? "ref " : "") + this.db.fieldDotNetType(p.dbType) + " " + this.fixName(p.name));
                spCall2.Add((p.isOutputParam ? "ref " : "") + this.fixName(p.name));
            }

            

            

            s.AppendFormat("        public static {2} execute(ORMContext ctx{1} {0}) {{" + CRLF, spCall.ToString(false), spCall.Count > 0 ? "," : "", retType);
            s.AppendFormat("                {0} c = new {0}(ctx);" + CRLF, mainProcName.className);
            s.AppendFormat("                return c.execute({0});" + CRLF, spCall2.ToString(false));
            s.Append("        }" + CRLF);
            s.AppendFormat("        public static DataSet executeDataSet(ORMContext ctx{1} {0}) {{" + CRLF, spCall.ToString(false), spCall.Count > 0 ? "," : "");
            s.AppendFormat("                {0} c = new {0}(ctx);" + CRLF, mainProcName.className);
            s.AppendFormat("                return c.executeDataSet({0});" + CRLF, spCall2.ToString(false));
            s.Append("        }" + CRLF);
            s.AppendFormat("        public DataSet executeDataSet({0}) {{" + CRLF, spCall.ToString(false));
            s.Append("                DBParams p = new DBParams();" + CRLF);

            string direction = useOutputParamAsInputOutput ? "ParameterDirection.InputOutput" : "ParameterDirection.Output";
            foreach (StoredProcParam p in sp.parameters)
            {
                if (p.isOutputParam)
                {
                    s.AppendFormat("                object {0}_obj = {0}; if ({0} == null) {{ {0}_obj = System.DBNull.Value; }}" + CRLF, this.fixName(p.name));
                    if (p.dbType.ToLower().Contains("char"))
                    {
                        s.AppendFormat("                p.Add(\"{0}\", {0}_obj, {3}, {1}, {2});" + CRLF, this.fixName(p.name), this.db.GetSqlDbType(p.dbType), p.characterMaxLength, direction);
                    }
                    else
                    {
                        s.AppendFormat("                p.Add(\"{0}\", {0}_obj, {2}, {1});" + CRLF, this.fixName(p.name), this.db.GetSqlDbType(p.dbType), direction);
                    }
                }
                else
                {
                    if (p.isNullable)
                    {
                        s.AppendFormat("                object {0}_obj = {0}; if ({0} == null) {{ {0}_obj = System.DBNull.Value; }}" + CRLF, this.fixName(p.name));
                        s.AppendFormat("                p.Add(\"{0}\", {0}_obj);" + CRLF, this.fixName(p.name));
                    }
                    else
                    {
                        s.AppendFormat("                p.Add(\"{0}\", {0});" + CRLF, this.fixName(p.name));
                    }
                }
            }
            s.AppendFormat("                DataSet ret = this.context.db.selectDataSetStoredProcedure(this.dbObjectName, p);" + CRLF);
            
            foreach (StoredProcParam p in sp.parameters)
            {
                if (p.isOutputParam)
                {
                    string nettp = this.db.fieldDotNetType(p.dbType).ToLower();
                    if (nettp.Contains("?") || nettp == "string")
                    {
                        s.AppendFormat("                {0} = p[\"{0}\"] != System.DBNull.Value ? ({1})p[\"{0}\"] : ({1})null;" + CRLF, this.fixName(p.name), this.db.fieldDotNetType(p.dbType));
                    }
                    else
                    {
                        s.AppendFormat("                {0} = ({1})p[\"{0}\"];" + CRLF, this.fixName(p.name), this.db.fieldDotNetType(p.dbType));
                    }
                }
            }
            s.Append("                return ret;" + CRLF);
            s.Append("        }" + CRLF);



            s.AppendFormat("        public {1} execute({0}) {{" + CRLF, spCall.ToString(false), retType);
            s.AppendFormat("                DataSet ret = this.executeDataSet({0});" + CRLF, spCall2.ToString(false));            
            ix = 0;
            EList<string> args = new EList<string>();
            foreach (var ci in sp.resultsetColumnsDefinition)
            {
                ix++;
                TableName tn = getInnerTableName(ix);
                var nm = String.Format("{0}", tn.className);
                s.AppendFormat("                var item{0} = {1}.getInstancesFromDataTable<{1}>(this.context, ret.Tables[{2}]);" + CRLF, ix, nm, ix-1);
                args.Add(String.Format(" Item{0} = item{0}", ix));                
            }
            s.AppendFormat("                var res = new {0}() {{ {1} }};" + CRLF, retType, args.ToString(false));
            s.Append("                return res;" + CRLF);
            s.Append("        }" + CRLF);

            addFinishClassFile(s);

            writeFile(mainProcName, PartialType.DB_PARTIAL, s.ToString(), "SP");


            //write individual table files
            ix = 0;
            foreach (var ci in sp.resultsetColumnsDefinition)
            {
                ix++;
                TableColumnsWrap cw = new TableColumnsWrap(this.db, ci);
                TableName procName = getInnerTableName(ix);
                ci.table = procName;
                if (procName.sqlFromName.Contains("_ET"))
                {
                    //string stophere = "stop here";
                }
                
                s = new StringBuilder();
                namesp = addFileNamespace(procName.catalog, procName.schema, s);
                addStartClassFile(procName, s);
                
                addConstructor(procName, cw, s, true, namesp, true);
                addFieldsProperties(cw, s, procName);

                s.Append(CRLF);
                s.Append(CRLF);
                
                addFinishClassFile(s);

                writeFile(procName, PartialType.DB_PARTIAL, s.ToString(), "SP");

                buildInterface(procName, cw, "SP");

                string icls = this.buildExtensionPartialClass(procName);
                writeFile(procName, PartialType.CLS_PARTIAL, icls, "SP");

            }
        }

        /// <summary>
        /// writes a class similar to the one by buildTable for a Stored Procedure that return a DataTable
        /// </summary>
        public void buildSelectStoredProc(TableName procName) { buildSelectStoredProc(procName, null, false); }
        public void buildSelectStoredProc(TableName procName, bool useOutputParamAsInputOutput) { buildSelectStoredProc(procName, null, useOutputParamAsInputOutput); }
        public void buildSelectStoredProc(TableName procName, DBParams param, bool useOutputParamAsInputOutput)
        {
            StoredProcDef sp = this.db.storedProcedure<StoredProcDef>(procName);
            sp.defaultParameters = param;
            ORMContext context = new ORMContext();
            context.db = this.db;
            context.classFactory = new ORMClassFactory(Assembly.GetExecutingAssembly());

            //StoredProcTableRow tr = new StoredProcTableRow(context, sp);
            //TableColumnsWrap cw = new TableColumnsWrap(this, tr.defTable);

            TableColumnsInfo tci = sp.resultsetColumnsDefinition;
            tci.table = procName;
            TableColumnsWrap cw = new TableColumnsWrap(this.db, tci);
                        
            StringBuilder s = new StringBuilder();
            string namesp = addFileNamespace(procName.catalog, procName.schema, s);
            addStartClassFile(procName, s);
            s.Append(CRLF+CRLF+"        public SQLStatement sqlStatement;"+CRLF+CRLF);

            addConstructor(procName, cw, s, true, namesp, true);
            addFieldsProperties(cw, s, procName);

            s.Append(CRLF);
            s.Append(CRLF);
            EList<string> spCall = new EList<string>();
            EList<string> spCall2 = new EList<string>();
            foreach (StoredProcParam p in sp.parameters)
            {
                spCall.Add((p.isOutputParam ? "ref " : "") + this.db.fieldDotNetType(p.dbType) + " " + this.fixName(p.name));
                spCall2.Add((p.isOutputParam ? "ref " : "") + this.fixName(p.name));
            }
            s.AppendFormat("        public static Table<{1}> execute(ORMContext ctx{2} {0}) {{"+CRLF, spCall.ToString(false), procName.className, spCall.Count>0 ? "," : "");
            s.AppendFormat("                {0} c = new {0}(ctx);"+CRLF, procName.className);
            s.AppendFormat("                return c.execute({0});" + CRLF, spCall2.ToString(false));
            s.Append("        }" + CRLF);
            s.AppendFormat("        public static DataTable executeDataTable(ORMContext ctx{1} {0}) {{" + CRLF, spCall.ToString(false), spCall.Count > 0 ? "," : "");
            s.AppendFormat("                {0} c = new {0}(ctx);" + CRLF, procName.className);
            s.AppendFormat("                return c.executeDataTable({0});" + CRLF, spCall2.ToString(false));
            s.Append("        }" + CRLF);
            s.AppendFormat("        public Table<{1}> execute({0}) {{" + CRLF, spCall.ToString(false), procName.className);
            s.AppendFormat("                DataTable tb = this.executeDataTable({0});" + CRLF, spCall2.ToString(false));
            s.AppendFormat("                return this.getInstancesFromDataTable<{0}>(tb);" + CRLF, procName.className);
            s.Append("        }" + CRLF);
            s.AppendFormat("        public DataTable executeDataTable({0}) {{" + CRLF, spCall.ToString(false));
            s.Append("                DBParams p = new DBParams();" + CRLF);

            string direction = useOutputParamAsInputOutput ? "ParameterDirection.InputOutput" : "ParameterDirection.Output";
            foreach (StoredProcParam p in sp.parameters)
            {
                if (p.isOutputParam)
                {                    
                    s.AppendFormat("                object {0}_obj = {0}; if ({0} == null) {{ {0}_obj = System.DBNull.Value; }}" + CRLF, this.fixName(p.name));
                    if (p.dbType.ToLower().Contains("char"))
                    {
                        s.AppendFormat("                p.Add(\"{0}\", {0}_obj, {3}, {1}, {2});" + CRLF, this.fixName(p.name), this.db.GetSqlDbType(p.dbType), p.characterMaxLength, direction);
                    }
                    else
                    {
                        s.AppendFormat("                p.Add(\"{0}\", {0}_obj, {2}, {1});" + CRLF, this.fixName(p.name), this.db.GetSqlDbType(p.dbType), direction);
                    }

                }
                else
                {
                    if (p.isNullable)
                    {
                        s.AppendFormat("                object {0}_obj = {0}; if ({0} == null) {{ {0}_obj = System.DBNull.Value; }}" + CRLF, this.fixName(p.name));
                        //s.AppendFormat("                p.Add(\"{0}\", {0}_obj);" + CRLF, this.fixName(p.name));
                        s.AppendFormat("                p.Add(\"{0}\", {0}_obj, ParameterDirection.Input, {1});" + CRLF, this.fixName(p.name), this.db.GetSqlDbType(p.dbType));
                    }
                    else
                    {
                        //s.AppendFormat("                p.Add(\"{0}\", {0});" + CRLF, this.fixName(p.name));
                        s.AppendFormat("                p.Add(\"{0}\", {0}, ParameterDirection.Input, {1});" + CRLF, this.fixName(p.name), this.db.GetSqlDbType(p.dbType));
                    }
                }
            }
            s.Append("                DataTable ret = this.db.selectStoredProcedure(this.dbObjectName, p);" + CRLF);
            foreach (StoredProcParam p in sp.parameters)
            {
                if (p.isOutputParam)
                {
                    string nettp = this.db.fieldDotNetType(p.dbType).ToLower();
                    if (nettp.Contains("?") || nettp == "string")
                    {
                        s.AppendFormat("                {0} = p[\"{0}\"] != System.DBNull.Value ? ({1})p[\"{0}\"] : ({1})null;" + CRLF, this.fixName(p.name), this.db.fieldDotNetType(p.dbType));
                    }
                    else
                    {
                        s.AppendFormat("                {0} = ({1})p[\"{0}\"];" + CRLF, this.fixName(p.name), this.db.fieldDotNetType(p.dbType));
                    }                        
                }
            }
            s.Append("                return ret;" + CRLF);
            s.Append("        }" + CRLF);      


            addFinishClassFile(s);

            
            writeFile(procName, PartialType.DB_PARTIAL, s.ToString(), "SP");
            
            buildInterface(procName, cw, "SP");

            string icls = this.buildExtensionPartialClass(procName);
            writeFile(procName, PartialType.CLS_PARTIAL, icls, "SP");

        }

        protected EList<string> interfacesBuilt = new EList<string>();
        protected EDictionary<string, EList<string>> interfacesBuiltClassType = new EDictionary<string, EList<string>>();
        protected EDictionary<string, TableColumnsWrap> interfacesBuiltLastTableColumnsWrap = new EDictionary<string,TableColumnsWrap>(); 
        protected void buildInterface(TableName table, TableColumnsWrap cw, string classType)
        {
            StringBuilder s = new StringBuilder();
            string namesp = addFileNamespace(table.catalog, table.schema, s);
            addStartInterfaceFile(table.interfaceName, s);
            s.AppendFormat(CRLF+"        //table: {0}.{1}.{2}{4}        //class: {3}{4}", table.catalog, table.schema, table.table, table.className, CRLF);

            if (!interfacesBuilt.Contains(table.interfaceName))
            {
                interfacesBuilt.Add(table.interfaceName);
                interfacesBuiltClassType.setdefault(table.interfaceName, new EList<string>()).Add(classType);
                interfacesBuiltLastTableColumnsWrap[table.interfaceName] = cw;
                addInterfaceFields(cw, s, table);
            }
            else
            {
                TableColumnsWrap cw2 = interfacesBuiltLastTableColumnsWrap[table.interfaceName];
                EList<string> dif = EList<string>.fromEnumarable(cw.getColumns()).diference(EList<string>.fromEnumarable(cw2.getColumns()));
                if (dif.Count > 0)
                {
                    throw new ORMException("Can not build interface " + table.interfaceName + " because it was already built in " + interfacesBuiltClassType[table.interfaceName].ToString() + " and there are fields that don't match in one interface and another " + dif.ToString());
                }
                
                if ( interfacesBuiltClassType[table.interfaceName].Contains(classType) )
                {
                    addInterfaceFields(cw, s, table);
                }
                else
                {
                    s.AppendFormat(CRLF + CRLF + "        //See the other partial classes for the interface in: {0}{1}", interfacesBuiltClassType[table.interfaceName].ToString(), CRLF);
                    interfacesBuiltClassType[table.interfaceName].Add(classType);
                }
            }

            addFinishClassFile(s);

            writeFile(table.interfaceName, table.catalog, table.schema, PartialType.DB_INTERFACE, s.ToString(), classType);
        }

        /// <summary>
        /// writes a class similar to execute a Stored Procedure that does not return anything
        /// </summary>
        public void buildExecStoredProc(TableName procName) { buildExecStoredProc(procName, null, false); }
        public void buildExecStoredProc(TableName procName, bool useOutputParamAsInputOutput) { buildExecStoredProc(procName, null, useOutputParamAsInputOutput); }
        public void buildExecStoredProc(TableName procName, DBParams param, bool useOutputParamAsInputOutput)
        {
            _buildExecStoredProcMain(procName, param, useOutputParamAsInputOutput, false);
        }


        /// <summary>
        /// writes a class similar to execute a Stored Procedure that does not return anything
        /// </summary>
        public void buildExecStoredProcWithReturn(TableName procName) { buildExecStoredProcWithReturn(procName, null, false); }
        public void buildExecStoredProcWithReturn(TableName procName, bool useOutputParamAsInputOutput) { buildExecStoredProcWithReturn(procName, null, useOutputParamAsInputOutput); }
        public void buildExecStoredProcWithReturn(TableName procName, DBParams param, bool useOutputParamAsInputOutput)
        {
            _buildExecStoredProcMain(procName, param, useOutputParamAsInputOutput, true);
        }
        
        private void _buildExecStoredProcMain(TableName procName, DBParams param, bool useOutputParamAsInputOutput, bool useExecuteScalar)
        {
            StoredProcDef sp = this.db.storedProcedure<StoredProcDef>(procName);
            sp.defaultParameters = param;
            ORMContext context = new ORMContext();
            context.db = this.db;
            context.classFactory = new ORMClassFactory(Assembly.GetExecutingAssembly());

            StringBuilder s = new StringBuilder();
            string namesp = addFileNamespace(procName.catalog, procName.schema, s);
            s.Append(CRLF + "    public partial class " + procName.className + "  {" + CRLF);
            s.Append(CRLF + "        public ORMContext context;" + CRLF);
            s.Append(CRLF + "        public TableName dbObjectName;" + CRLF);

            s.AppendFormat("        public {0}(ORMContext context) {{" + CRLF, procName.className);
            s.Append("            this.context = context;" + CRLF);
            //s.AppendFormat("            this.dbObjectName = new TableName({0},{1},{2},{3},\"{4}\",{5});\n", 
            s.AppendFormat("            this.dbObjectName = new TableName({0},{1},{2},{3},\"{4}\");" + CRLF,
                        procName.catalog != null ? '"' + procName.catalog + '"' : "null",
                        procName.schema != null ? '"' + procName.schema + '"' : "null",
                        procName.table != null ? '"' + procName.table + '"' : "null",
                        procName.className != null ? '"' + procName.className + '"' : "null",
                        computeNamespace(this.rootNameSpace, procName)//,
                //procName.interfaceName != null ? '"' + procName.interfaceName + '"' : "null"
                        );
            s.Append("            this.dbObjectName.context = this.context;" + CRLF);
            s.Append("        }" + CRLF);

            s.Append(CRLF);
            EList<string> spCall = new EList<string>();
            EList<string> spCall2 = new EList<string>();
            foreach (StoredProcParam p in sp.parameters)
            {
                spCall.Add((p.isOutputParam ? "ref " : "") + this.db.fieldDotNetType(p.dbType) + " " + this.fixName(p.name));
                spCall2.Add((p.isOutputParam ? "ref " : "") + this.fixName(p.name));
            }
            s.AppendFormat("        public static {2} execute(ORMContext ctx{1} {0}) {{" + CRLF, spCall.ToString(false), spCall.Count > 0 ? "," : "", useExecuteScalar ? "object" : "bool");
            s.AppendFormat("                {0} c = new {0}(ctx);" + CRLF, procName.className);
            s.AppendFormat("                return c.execute({0});" + CRLF, spCall2.ToString(false));
            s.Append("        }" + CRLF);
            s.AppendFormat("        public {1} execute({0}) {{" + CRLF, spCall.ToString(false), useExecuteScalar ? "object" : "bool");
            s.Append("                DBParams p = new DBParams();" + CRLF);

            string direction = useOutputParamAsInputOutput ? "ParameterDirection.InputOutput" : "ParameterDirection.Output";
            foreach (StoredProcParam p in sp.parameters)
            {
                if (p.isOutputParam)
                {
                    s.AppendFormat("                object {0}_obj = {0}; if ({0} == null) {{ {0}_obj = System.DBNull.Value; }}" + CRLF, this.fixName(p.name));
                    if (p.dbType.ToLower().Contains("char"))
                    {
                        s.AppendFormat("                p.Add(\"{0}\", {0}_obj, {3}, {1}, {2});" + CRLF, this.fixName(p.name), this.db.GetSqlDbType(p.dbType), p.characterMaxLength, direction);
                    }
                    else
                    {
                        s.AppendFormat("                p.Add(\"{0}\", {0}_obj, {2}, {1});" + CRLF, this.fixName(p.name), this.db.GetSqlDbType(p.dbType), direction);
                    }
                }
                else
                {
                    if (p.isNullable)
                    {
                        s.AppendFormat("                object {0}_obj = {0}; if ({0} == null) {{ {0}_obj = System.DBNull.Value; }}" + CRLF, this.fixName(p.name));
                        //s.AppendFormat("                p.Add(\"{0}\", {0}_obj);" + CRLF, this.fixName(p.name));
                        s.AppendFormat("                p.Add(\"{0}\", {0}_obj, ParameterDirection.Input, {1});" + CRLF, this.fixName(p.name), this.db.GetSqlDbType(p.dbType));
                    }
                    else
                    {
                        //s.AppendFormat("                p.Add(\"{0}\", {0});" + CRLF, this.fixName(p.name));
                        s.AppendFormat("                p.Add(\"{0}\", {0}, ParameterDirection.Input, {1});" + CRLF, this.fixName(p.name), this.db.GetSqlDbType(p.dbType));
                    }
                }
            }
            s.AppendFormat("                {0} ret = this.context.db.{1}(this.dbObjectName, p);" + CRLF, useExecuteScalar ? "object" : "bool", useExecuteScalar ? "executeScalarStoredProcedure" : "executeStoredProcedure");
            foreach (StoredProcParam p in sp.parameters)
            {
                if (p.isOutputParam)
                {
                    string nettp = this.db.fieldDotNetType(p.dbType).ToLower();
                    if (nettp.Contains("?") || nettp == "string")
                    {
                        s.AppendFormat("                {0} = p[\"{0}\"] != System.DBNull.Value ? ({1})p[\"{0}\"] : ({1})null;" + CRLF, this.fixName(p.name), this.db.fieldDotNetType(p.dbType));
                    }
                    else
                    {
                        s.AppendFormat("                {0} = ({1})p[\"{0}\"];" + CRLF, this.fixName(p.name), this.db.fieldDotNetType(p.dbType));
                    }
                }
            }
            s.Append("                return ret;" + CRLF);
            s.Append("        }" + CRLF);

            addFinishClassFile(s);

            writeFile(procName, PartialType.DB_PARTIAL, s.ToString(), "SP");

        }

        /// <summary>
        /// writes a class similar to the one by buildTable for an arbitrary sql select statement
        /// </summary>
        public void buildSQLSelect(string className, string catalogName, string sql) { buildSQLSelect(className, catalogName, sql, false); }
        public void buildSQLSelect(string className, string catalogName, string sql, bool generateExtensionPartialClass) { buildSQLSelect(className, catalogName, sql, generateExtensionPartialClass, "I" + className); }
        /// <summary>
        /// Writes a class similar to the one by buildTable for an arbitrary sql select statement. 
        /// Chose if you want an extension mockup class in CLS_PARTIAL to allow for customizations
        /// </summary>
        public void buildSQLSelect(string className, string catalogName, string sql, bool generateExtensionPartialClass, string interfaceName)
        {
            ORMContext context = new ORMContext();
            context.db = this.db;
            context.classFactory = new ORMClassFactory(Assembly.GetExecutingAssembly());
            QueryStatementTableRow tr = new QueryStatementTableRow(context, sql);
            tr.dbObjectName.catalog = catalogName;
            tr.dbObjectName.className = className;
            tr.dbObjectName.interfaceName = interfaceName;

            StringBuilder s = new StringBuilder();
            string namesp = addFileNamespace(catalogName, null, s);
            addStartClassFileDynamicSQL(tr.dbObjectName, s);
            s.Append(CRLF + CRLF + "        public SQLStatement sqlStatement;" + CRLF + CRLF);
            TableColumnsWrap cw = new TableColumnsWrap(this.db, tr.defTable);
            addDBConstructorStart(tr.dbObjectName, cw, s, true, namesp);
            s.Append("            this.setSQLDef();" + CRLF);
            addDBConstructorFinish(s);
            addFieldsProperties(cw, s, null);
            s.Append(CRLF);
            s.Append(CRLF);
            
            OrderedDictionary<string, string> d = new OrderedDictionary<string, string>();
            d.Add("fields", tr.querySQL.fields);     //strip out the select word
            d.Add("from", tr.querySQL.from);
            d.Add("where", tr.querySQL.where);
            d.Add("group", tr.querySQL.group);
            d.Add("having", tr.querySQL.having);
            d.Add("order", tr.querySQL.order);

            s.Append("        protected void setSQLDef()  {" + CRLF);
            s.Append("            sqlStatement = new SQLStatement(this);" + CRLF);
            foreach (string k in d.Keys)
            {
                if (d[k] != "")
                {
                    s.AppendFormat("            this.sqlStatement.{0} = @\"{1}\";" + CRLF + CRLF, k, d[k]);
                }
            }
            s.Append("        }" + CRLF + CRLF);

            s.Append("        protected override void setSQLAttributes(ref SQLStatement stm)  {" + CRLF);
            foreach (string k in d.Keys)
            {
                s.AppendFormat("            if (stm.{0}==null) {{ stm.{0} = this.sqlStatement.{0}; }}" + CRLF, k);
            }
            s.Append("        }" + CRLF + CRLF);

            s.Append("    }" + CRLF);  //class
            s.Append("}" + CRLF);      //namespace

            writeFile(className, catalogName, null, PartialType.DB_PARTIAL, s.ToString(), "QR");
            
            if (generateExtensionPartialClass)
            {
                string icls = this.buildExtensionPartialClass(className, catalogName, null);
                writeFile(className, catalogName, null, PartialType.CLS_PARTIAL, icls, "QR");
            }

            buildInterface(tr.dbObjectName, cw, "QR");
                    
        }


        /// <summary>
        ///     writes a class similar to the one by buildTable for an arbitrary DataTable / Middle Layer 
        ///         (later when you pass GenericDatabase to it, it should be a custome middle layer
        ///             for select, save, insert, update, delete to work
        ///         )
        /// </summary>
        public void buildFromDataTable(DataTable table, TableName name) { buildFromDataTable(table, name, false); }
        /// <summary>
        ///     writes a class similar to the one by buildTable for an arbitrary DataTable / Middle Layer 
        ///         (later when you pass GenericDatabase to it, it should be a custome middle layer
        ///             for select, save, insert, update, delete to work
        ///         )
        /// Chose if you want an extension mockup class in CLS_PARTIAL to allow for customizations
        /// </summary>
        public void buildFromDataTable(DataTable table, TableName name, bool generateExtensionPartialClass)
        {
            StringBuilder s = new StringBuilder();
            string namesp = addFileNamespace(name.catalog, name.schema, s);
            addStartClassFile(name, s);
            s.Append(CRLF + CRLF + "        public SQLStatement sqlStatement;" + CRLF + CRLF);
            TableColumnsWrap cw = new TableColumnsWrap(this.db, table);
            addConstructor(name, cw, s, false, namesp, true);
            addFieldsProperties(cw, s, name);
            addFinishClassFile(s);
            
            writeFile(name, PartialType.DB_PARTIAL, s.ToString(), "QR");
            
            if (generateExtensionPartialClass)
            {
                string icls = this.buildExtensionPartialClass(name.className, name.catalog, null);
                writeFile(name, PartialType.CLS_PARTIAL, icls, "QR");
            }

            buildInterface(name, cw, "QR");

        }




        /// <summary>
        /// usualy this is used internaly by the other build methods so you should not need to call it manualy yourself
        /// </summary>
        public string buildExtensionPartialClass(TableName table) { return buildExtensionPartialClass(table.className, table.catalog, table.schema); }
        public string buildExtensionPartialClass(string className, string catalogName, string schemaName)
        {
            StringBuilder s = new StringBuilder();
            string namesp = addFileNamespace(catalogName, schemaName, s);
            s.AppendFormat("    [Serializable]" + CRLF);
            s.AppendFormat("    public partial class {0}{1}    {{" + CRLF + CRLF, className, CRLF);
            //s.AppendFormat("        public {0}() : base() {{ }}" + CRLF, className);
            //constructor init:
            s.Append(CRLF);

            s.Append(CRLF);
            s.Append("        /////////////////////////////////////////////////////////////////////////////////////" + CRLF);
            s.Append("        //////////// INIT " + CRLF + CRLF);
            s.AppendFormat("{0}        protected override void init(){0}        {{{0}            //write your customizations here{0}        }}{0}{0}", CRLF);
            s.Append("    }" + CRLF);  //class
            s.Append("}" + CRLF);      //namespace

            return s.ToString();

        }

        #endregion main generaor API - individual object level



        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //// BASIC GENERATOR TEMPLATE
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region basic generaor template

        protected virtual string computeNamespace(string startNameSpace, TableName table) { return computeNamespace(startNameSpace, table.catalog, table.schema); }
        protected virtual string computeNamespace(string startNameSpace, string catalogName, string schemaName)
        {
            string namesp = startNameSpace;
            if (catalogName != null)
            {
                namesp += "." + catalogName;
            }
            if (schemaName != null)
            {
                namesp += "." + schemaName;
            }
            return namesp;
        }

        protected virtual string addFileNamespace(string catalogName, string schemaName, StringBuilder s) { return addFileNamespace(catalogName, schemaName, s, null); }
        protected virtual string addFileNamespace(string catalogName, string schemaName, StringBuilder s, ESet<string> aditionalCatalogAndSchemas)
        {
            s.Append("using System;" + CRLF +
                     "using System.Data;" + CRLF +
                     "using System.Text;" + CRLF +
                     "using System.Text.RegularExpressions;" + CRLF +
                     "using ORM;" + CRLF +
                     "using ORM.exceptions;" + CRLF +
                     "using ORM.DBFields;" + CRLF +
                     "using ORM.db_store;" + CRLF +
                     "using EM.DB;" + CRLF +
                     "using EM.Collections;" + CRLF + CRLF);
            if (aditionalCatalogAndSchemas != null)
            {
                foreach (string aditionalSchema in aditionalCatalogAndSchemas)
                {
                    s.Append("using " + this.rootNameSpace + "." + aditionalSchema + ";" + CRLF);
                }
                s.Append(CRLF + CRLF);
            }
            string namesp = computeNamespace(this.rootNameSpace, catalogName, schemaName);

            s.Append("namespace " + namesp + " {" + CRLF + CRLF);
            
            return namesp;

        }
        protected virtual void addStartClassFile(TableName tn, StringBuilder s)
        {
            s.Append(CRLF + "    public partial class " + tn.className + " : TableRow, " + tn.interfaceName + " {" + CRLF);                      
        }
        protected virtual void addStartClassFileDynamicSQL(TableName tn, StringBuilder s)
        {
            s.Append(CRLF + "    public partial class " + tn.className + " : TableRowDynamicSQL, " + tn.interfaceName + " {" + CRLF);
        }
        protected virtual void addStartClassFileStoredProcBased(TableName tn, StringBuilder s)
        {
            s.Append(CRLF+"    public partial class " + tn.className + " : TableRowStoredProcBased, " + tn.interfaceName + " {" + CRLF);
        }
        protected virtual void addStartInterfaceFile(string interfaceName, StringBuilder s)
        {
            s.Append(CRLF + "    public partial interface " + interfaceName + " {" + CRLF);
        }
        protected virtual void addDBRelationsDeclarations(TableName table, OrderedDictionary<TableName, ESet<DBRelation>> oneToMany, PKInfo pk, FKInfo fk, StringBuilder s, OrderedDictionary<DBRelation, string> fkNamesCache, OrderedDictionary<DBRelation, string> otmNamesCache, EDictionary<string, EList<DBRelation>> fkExisting, EDictionary<string, EList<DBRelation>> otmExisting)
        {
            if (fk.Count > 0 || oneToMany.Count > 0)
            {
                s.Append("        //FK and oneToMany Cache variables (property helpers)" + CRLF);
                foreach (DBRelation rel in fk)
                {
                    if (rel.tableThere.Equals(table) && sameFields(rel.fieldsThere, pk)) { continue; }
                    string relName = getFKDBRelationName(rel, fkNamesCache, fkExisting);
                    s.AppendFormat("        protected {0} _{1}_1to1;  //FK - one to one" + CRLF, fixName(rel.tableThere.className), relName);
                }
                foreach (DBRelation rel in oneToMany.get(table, new ESet<DBRelation>()))
                {
                    if (rel.tableHere.Equals(table) && sameFields(rel.fieldsHere, pk)) { continue; }
                    string relName = getOtoMDBRelationName(rel, otmNamesCache, otmExisting);
                    s.AppendFormat("        protected TablePersist<{0}> _{1}_1toM;  //one to many" + CRLF, fixName(rel.tableHere.className), relName);
                    s.AppendFormat("        protected DataTable _{0}_1toMdt;  //one to many" + CRLF, relName);
                }
            }
        }
        protected virtual void addConstructor(TableName name, TableColumnsWrap cw, StringBuilder s, bool isReadOnly, string namesp) { addConstructor(name, cw, s, isReadOnly, namesp, false); }
        protected virtual void addConstructor(TableName name, TableColumnsWrap cw, StringBuilder s, bool isReadOnly, string namesp, bool includeNoDBConstructor)
        {
            addConstructorHeaderComments(s);
            if (includeNoDBConstructor)
            {
                addNoDBConstructor(name.className, s);
            }
            addDBConstructorStart(name, cw, s, isReadOnly, namesp);
            addDBConstructorFinish(s);
        }
        protected virtual void addConstructorHeaderComments(StringBuilder s)
        {
            s.Append(CRLF);

            s.Append(CRLF);
            s.Append("        /////////////////////////////////////////////////////////////////////////////////////" + CRLF);
            s.Append("        //////////// Constructors" + CRLF + CRLF);
        }
        protected virtual void addNoDBConstructor(string className, StringBuilder s)
        {
                  s.Append("        /// <summary>" + CRLF);
                  s.Append("        /// Empty constructor uses ORMContextNoDB() context " + CRLF);
                  s.Append("        /// </summary>" + CRLF);
            s.AppendFormat("        public {0}() : this( new ORMContextNoDB()) {{ }}" + CRLF + CRLF, className);
                  s.Append("        /// <summary>" + CRLF);
                  s.Append("        /// DataRow constructor uses ORMContextNoDB() context " + CRLF);
                  s.Append("        /// </summary>" + CRLF);
            s.AppendFormat("        public {0}(DataRow row) : this( new ORMContextNoDB()) {{ this.setFromDataRow(row); }}" + CRLF + CRLF, className);            
        }
        protected virtual void addDBConstructorStart(TableName name, TableColumnsWrap cw, StringBuilder s, bool isReadOnly, string namesp)
        {
            s.AppendFormat("        public {0}(ORMContext context, DataRow row) : this(context) {{ this.setFromDataRow(row); }}" + CRLF, name.className);
            //s.AppendFormat("        public {0}() : this(ORMContext.instance.db) {{ }}"+CRLF, className);
            s.AppendFormat("        public {0}(ORMContext context) : base(context) {{" + CRLF, name.className);
            s.AppendFormat("            this._isReadOnly = {0};" + CRLF, isReadOnly.ToString().ToLower());
            
            
            foreach (string c in cw.getColumns())
            {
                //s.AppendFormat("            this.fields[\"{0}\"] = new {1}{2}; this.fields[\"{0}\"].table = this; this.fields[\"{0}\"].isComputed = {3}; this.fields[\"{0}\"].valueTypeName = \"{4}\";" + CRLF,
                //using object initializer to optimize away the dictionary get_item lookup time for this.fields[...]
                s.AppendFormat("            this.fields[\"{0}\"] = new {1}{2} {{ table = this, isComputed = {3}, valueTypeName = \"{4}\" }};" + CRLF,
                    c,
                    cw.fieldClassType(c),
                    cw.classParams(c, cw.fieldClassType(c)), 
                    cw.isComputed(c).ToString().ToLower(),
                    cw.fieldDotNetType(c));
            }
            if (name != null)
            {
                s.AppendFormat("            this.dbObjectName = new TableName({0},{1},{2},{3},\"{4}\",{5});" + CRLF, 
                            name.catalog != null ? '"' + name.catalog + '"' : "null", 
                            name.schema != null ? '"' + name.schema + '"' : "null", 
                            name.table != null ? '"' + name.table + '"' : "null", 
                            name.className != null ? '"' + name.className + '"' : "null", 
                            namesp,
                            name.interfaceName != null ? '"' + name.interfaceName + '"' : "null");
                s.Append("            this.dbObjectName.context = this.context;" + CRLF);
            }
            s.Append("            this.pk = new PKInfo();" + CRLF);
            s.Append("            this.pk.table = this.dbObjectName;" + CRLF);
            s.Append("            this.fk = new FKInfo();" + CRLF);
            s.Append("            this.fk.table = this.dbObjectName;" + CRLF);
            
        }
        protected virtual void addDBConstructorDBRelationsPart(TableName table, OrderedDictionary<TableName, ESet<DBRelation>> oneToMany, PKInfo pk, FKInfo fk, StringBuilder s, bool isView)
        {
            //add PK
            foreach (string pkf in pk)
            {
                s.AppendFormat("            this.pk.Add(\"{0}\");" + CRLF, pkf);
            }

            //add FK
            if (fk.Count > 0 || oneToMany.get(table, null) != null)
            {
                s.Append("            DBRelation rel;" + CRLF);
            }
            if (fk.Count > 0)
            {
                s.Append("            //FK relations:" + CRLF);
            }
            foreach (DBRelation rel in fk)
            {
                if (rel.tableThere.Equals(table) && sameFields(rel.fieldsThere, pk)) { continue; }
                s.Append("            rel = new DBRelation();" + CRLF);
                s.AppendFormat("            rel.relationName = \"{0}\";" + CRLF, rel.relationName);
                s.Append("            rel.tableHere = this.dbObjectName;" + CRLF);
                s.AppendFormat("            rel.tableThere = new TableName(\"{0}\",\"{1}\",\"{2}\", \"{3}\", \"{4}\", \"{5}\");" + CRLF,
                            rel.tableThere.catalog,
                            rel.tableThere.schema,
                            rel.tableThere.table,
                            rel.tableThere.className,
                            computeNamespace(this.rootNameSpace, rel.tableThere),
                            rel.tableThere.interfaceName);
                s.Append("            rel.tableThere.context = this.context;" + CRLF);
                foreach (string key in rel.fieldsHere)
                {
                    s.AppendFormat("            rel.fieldsHere.Add(\"{0}\");" + CRLF, key);
                }
                foreach (string key in rel.fieldsThere)
                {
                    s.AppendFormat("            rel.fieldsThere.Add(\"{0}\");" + CRLF, key);
                }
                s.Append("            this.fk.Add(rel);" + CRLF);
            }

            //add oneToMany List as metadata
            
            if (oneToMany.get(table, null) != null)
            {
                s.Append("            //one to many relations:" + CRLF);
                s.Append("            this.oneToMany = new OneToManyInfo();" + CRLF);
                foreach (DBRelation rel in oneToMany[table])
                {
                    if (rel.tableHere.Equals(table) && sameFields(rel.fieldsHere, pk)) { continue; }
                    s.Append("            rel = new DBRelation();" + CRLF);
                    s.AppendFormat("            rel.relationName = \"{0}\";" + CRLF, rel.relationName);
                    s.Append("            rel.tableHere = this.dbObjectName;" + CRLF);
                    s.AppendFormat("            rel.tableThere = new TableName(\"{0}\",\"{1}\",\"{2}\", \"{3}\", \"{4}\", \"{5}\");" + CRLF,
                                    rel.tableHere.catalog,
                                    rel.tableHere.schema,
                                    rel.tableHere.table,
                                    rel.tableHere.className,
                                    computeNamespace(this.rootNameSpace, rel.tableHere),
                                    rel.tableHere.interfaceName);
                    s.Append("            rel.tableThere.context = this.context;" + CRLF);
                    foreach (string key in rel.fieldsThere)
                    {
                        s.AppendFormat("            rel.fieldsHere.Add(\"{0}\");" + CRLF, key);
                    }
                    foreach (string key in rel.fieldsHere)
                    {
                        s.AppendFormat("            rel.fieldsThere.Add(\"{0}\");" + CRLF, key);
                    }
                    s.Append("            this.oneToMany.Add(rel);" + CRLF);
                }
            }
        }
        protected virtual void addDBConstructorFinish(StringBuilder s)
        {
            s.Append("            init();" + CRLF);
            s.Append("        }" + CRLF);
        }               
        protected virtual void addFieldsProperties(TableColumnsWrap cw, StringBuilder s, TableName tbl)
        {
            Func<string, string> getPropName = fldName => this.getPropName(fldName, tbl);

            //fields properties
            s.Append(CRLF);
            s.Append("        /////////////////////////////////////////////////////////////////////////////////////" + CRLF);
            s.Append("        //////////// FIELDS" + CRLF + CRLF);
            foreach (string c in cw.getColumns())
            {
                s.AppendFormat("        public {0} {1}_object {{ get {{ return ({0})this.fields[\"{2}\"]; }} }}" + CRLF, cw.fieldClassType(c), getPropName(c), c);
                s.Append("        ///<summary>" + CRLF);
                s.AppendFormat("        ///db: {0}" + CRLF, cw.columnAttributes(c).ToString());
                s.Append("        ///</summary>" + CRLF);
                if (cw.isRequired(c) || tbl.customFieldPropertyType.ContainsKey(c))   //it could never have null as value
                {
                    s.AppendFormat("        public {0} {1} {{" + CRLF, cw.fieldDotNetType(c).Replace("?", ""), getPropName(c));
                    s.Append("            get {" + CRLF);
                    s.AppendFormat("                object v = this.fields[\"{0}\"].value;" + CRLF, c);
                    s.AppendFormat("                return ({0})v;" + CRLF, cw.fieldDotNetType(c).Replace("?", ""));
                    s.Append("            }" + CRLF);
                }
                else
                {
                    s.AppendFormat("        public {0} {1} {{" + CRLF, cw.fieldDotNetType(c), getPropName(c));
                    s.Append("            get {" + CRLF);
                    s.AppendFormat("                object v = this.fields[\"{0}\"].value;" + CRLF, c);
                    s.AppendFormat("                return v != System.DBNull.Value ? ({0})v : ({0})null;" + CRLF, cw.fieldDotNetType(c));
                    s.Append("            }" + CRLF);
                }
                s.AppendFormat("            set {{ this.fields[\"{0}\"].value = value; }}" + CRLF, c);
                s.Append("        }" + CRLF + CRLF);                
            }
            s.Append(CRLF);
        }
        protected virtual void addInterfaceFields(TableColumnsWrap cw, StringBuilder s, TableName tbl)
        {
            Func<string, string> getPropName = fldName => this.getPropName(fldName, tbl);

            s.Append(CRLF);
            foreach (string c in cw.getColumns())
            {
                if (cw.isRequired(c))   //it could never have null as value
                {
                    s.AppendFormat("        {0} {1} {{ get; set; }}" + CRLF, cw.fieldDotNetType(c).Replace("?", ""), getPropName(c));                    
                }
                else
                {
                    s.AppendFormat("        {0} {1} {{ get; set; }}"+CRLF, cw.fieldDotNetType(c), getPropName(c));
                }
            }
            s.Append(CRLF);
        }

        /// <summary>
        /// assumes that builtable will create proprieties like TablePersist selectBy{1}({2})"
        /// and "DataTable selectDataTableBy{1}({2})"
        /// </summary>
        protected virtual void addDBOneToManyProprieties(TableName table, OrderedDictionary<TableName, ESet<DBRelation>> oneToMany, PKInfo pk, StringBuilder s, OrderedDictionary<DBRelation, string> otmNamesCache, EDictionary<string, EList<DBRelation>> otmExisting, TableColumnsWrap cw)
        {
            //one to Many   (the inverse of a foreign key)
            if (oneToMany.get(table, null) != null)
            {
                s.Append(CRLF);
                s.Append("        /////////////////////////////////////////////////////////////////////////////////////" + CRLF);
                s.Append("        //////////// One To Many Relations" + CRLF + CRLF);

                Console.Write(" has oneToMany ");

                foreach (DBRelation rel in oneToMany[table])
                {
                    //the DBRelation if from the FK of the related table so rel.tableHere is actualy tableThere that we need
                    if (rel.tableHere.Equals(table) && sameFields(rel.fieldsHere, pk)) { continue; }
                    string relName = getOtoMDBRelationName(rel, otmNamesCache, otmExisting);

                    TableColumnsInfo tciTableHere = this.db.columns(rel.tableHere);
                    TableColumnsWrap cwTableHere = new TableColumnsWrap(this.db, tciTableHere);
                    Func<int, bool> isNullableThereButNotHere = (i) =>
                    {
                        //if field there is not nullable but field here is nullable make sure to say fieldHere.Value instead of just fieldHere
                        return
                            (cwTableHere.isRequired(rel.fieldsHere[i]) ||
                              (rel.tableHere.customFieldPropertyType.ContainsKey(rel.fieldsHere[i]) &&
                               !cwTableHere.fieldDotNetType(rel.fieldsHere[i]).EndsWith("?"))
                            ) &&
                            cw.fieldDotNetType(rel.fieldsThere[i]).EndsWith("?");
                    };


                    EList<string> params_ = new EList<string>();
                    for (int i = 0; i < rel.fieldsHere.Count; i++)
                    {
                        var fieldNameThere = rel.tableThere.getCustomFieldPropertyName(rel.fieldsThere[i]);
                        if (isNullableThereButNotHere(i))
                        {
                            params_.Add(string.Format("this.{0}.Value", fieldNameThere));
                            s.AppendFormat("        //{0} is nullable in {1} but {2} is not nullable in {3}" + CRLF, rel.fieldsThere[i], rel.tableThere.table, rel.fieldsHere[i], rel.tableHere.table);
                        }
                        else
                        {
                            params_.Add(string.Format("this.{0}", fieldNameThere));
                        }
                        
                    }
                    string name = GetMethodName_ParamsPart(rel.fieldsHere);

                    s.Append(CRLF);
                    //TablePersist ////////////////////////////////////////////////////
                    s.AppendFormat("        public TablePersist<{1}> {0}_list {{  //FK from {1} - one to many" + CRLF, relName, rel.tableHere.className);
                    s.AppendFormat("            get {{" + CRLF);
                    s.AppendFormat("                if (_{0}_1toM == null) " + CRLF, relName);
                    s.AppendFormat("                {{" + CRLF);
                    //s.AppendFormat("                    {0} c = new {0}(this.context);" + CRLF, rel.tableHere.className);
                    //s.AppendFormat("                    {0} c = ({0}){0}.getInstance(typeof({0}), this.context);" + CRLF, rel.tableHere.className);     //allow instantiating inherited business entity class
                    s.AppendFormat("                    _{0}_1toM = {3}.selectBy{1}(this.context, {2});" + CRLF, relName, name, params_.join(", "), rel.tableHere.className);
                    s.AppendFormat("                }}" + CRLF + CRLF);
                    s.AppendFormat("                return _{0}_1toM;" + CRLF, relName);
                    s.AppendFormat("            }}" + CRLF);
                    s.AppendFormat("            set {{" + CRLF);
                    s.AppendFormat("                _{0}_1toM = value;" + CRLF, relName);
                    s.AppendFormat("            }}" + CRLF);
                    s.AppendFormat("        }}" + CRLF);
                    //TablePersist Generics ////////////////////////////////////////////////////
                    s.Append("        /// <summary>" + CRLF);
                    s.AppendFormat("        /// get list of {0} returning list of generic type T, TablePersist&lt;T&gt; " + CRLF, relName);
                    s.Append("        /// </summary>" + CRLF);
                    s.AppendFormat("        public TablePersist<T> {0}_listOf<T>() where T : {1} {{" + CRLF, relName, rel.tableHere.className);
                    s.AppendFormat("            if ( !this.cashList.ContainsKey(typeof(T)) )  {{" + CRLF);
                    //s.AppendFormat("                {0} c = new {0}(this.context);" + CRLF, rel.tableHere.className);
                    s.AppendFormat("                DataTable tb = {2}.selectDataTableBy{0}(this.context, {1});" + CRLF, name, params_.join(", "), rel.tableHere.className);
                    s.AppendFormat("                this.cashList[typeof(T)] = {0}.getInstancesFromDataTable<T>(this.context, tb);" + CRLF, rel.tableHere.className);
                    s.AppendFormat("            }}" + CRLF);
                    s.AppendFormat("            return (TablePersist<T>)cashList[typeof(T)];" + CRLF);
                    s.AppendFormat("        }}" + CRLF);
                    //DataTable  ////////////////////////////////////////////////////
                    s.AppendFormat("        public DataTable {0}_DataTable {{  //FK from {1} - one to many" + CRLF, relName, rel.tableHere.className);
                    s.AppendFormat("            get {{" + CRLF);
                    s.AppendFormat("                if (_{0}_1toMdt == null) " + CRLF, relName);
                    s.AppendFormat("                {{" + CRLF);
                    //s.AppendFormat("                    {0} c = new {0}(this.context);" + CRLF, rel.tableHere.className);
                    s.AppendFormat("                    _{0}_1toMdt = {3}.selectDataTableBy{1}(this.context, {2});" + CRLF, relName, name, params_.join(", "), rel.tableHere.className);
                    s.AppendFormat("                }}" + CRLF + CRLF);
                    s.AppendFormat("                return _{0}_1toMdt;" + CRLF, relName);
                    s.AppendFormat("            }}" + CRLF);
                    s.AppendFormat("        }}" + CRLF);
                }
            }
        }

        /// <summary>
        /// assumes that builtable will create methods like   loadBy{1}({2}) or loadByPK"
        /// </summary>
        protected virtual void addDBForeignKeysProprieties(TableName table, PKInfo pk, FKInfo fk, StringBuilder s, OrderedDictionary<DBRelation, string> fkNamesCache, EDictionary<string, EList<DBRelation>> fkExisting, TableColumnsWrap cw)
        {
            //Foreign Keys:     one to one
            if (fk.Count > 0)
            {
                s.Append(CRLF);
                s.Append("        /////////////////////////////////////////////////////////////////////////////////////" + CRLF);
                s.Append("        //////////// Foreign Key Relations" + CRLF + CRLF);

                Console.Write(" has FK ");
                foreach (DBRelation rel in fk)
                {
                    if (rel.tableThere.Equals(table) && sameFields(rel.fieldsThere, pk)) { continue; }
                    string relName = getFKDBRelationName(rel, fkNamesCache, fkExisting);

                    TableColumnsInfo tciTableThere = this.db.columns(rel.tableThere);
                    TableColumnsWrap cwTableThere = new TableColumnsWrap(this.db, tciTableThere);
                    Func<int, bool> isNullableHereNotThere = (i) =>
                    {
                        //if field there is not nullable but field here is nullable make sure to say fieldHere.Value instead of just fieldHere
                        return 
                            (cwTableThere.isRequired(rel.fieldsThere[i]) ||
                              (rel.tableThere.customFieldPropertyType.ContainsKey(rel.fieldsThere[i]) && 
                               !cwTableThere.fieldDotNetType(rel.fieldsThere[i]).EndsWith("?"))
                            ) &&
                            cw.fieldDotNetType(rel.fieldsHere[i]).EndsWith("?");
                    };

                    EList<string> prms = new EList<string>();
                    for (int i = 0; i < rel.fieldsThere.Count; i++)
                    {
                        var fieldNameHere = rel.tableHere.getCustomFieldPropertyName(rel.fieldsHere[i]);                                            
                        if (isNullableHereNotThere(i))
                        {
                            prms.Add(string.Format("this.{0}.Value",fieldNameHere));
                            s.AppendFormat("        //{0} is nullable in {1} but {2} is not nullable in {3}" + CRLF, rel.fieldsHere[i], table.table, rel.fieldsThere[i], rel.tableThere.table);
                        }
                        else
                        {
                            prms.Add(string.Format("this.{0}", fieldNameHere));                            
                        }
                    }
                    string nm = GetMethodName_ParamsPart(rel.fieldsThere);
                    PKInfo pkr = this.db.pk(rel.tableThere);

                    //regular proprety
                    s.Append(CRLF);
                    s.Append("        /// <summary>" + CRLF);
                    s.AppendFormat("        /// get and set the {0} FK instance. After setting to a new instance data is not saved to the database but you must save it yourself using save/update" + CRLF, rel.tableThere.className);
                    s.Append("        /// </summary>" + CRLF);
                    s.AppendFormat("        public {0} {1} {{  //FK - one to one" + CRLF, rel.tableThere.className, relName);
                    s.AppendFormat("            get {{" + CRLF);
                    s.AppendFormat("                if (_{0}_1to1 == null) " + CRLF, relName);
                    s.AppendFormat("                {{" + CRLF);
                    s.AppendFormat("                    {0} c = ({0}){0}.getInstance(typeof({0}), this.context);" + CRLF, rel.tableThere.className);  //allow instantiating inherited business entity class
                    if (sameFields(rel.fieldsThere, pkr))
                    {
                        s.AppendFormat("                    c.loadByPK({0});" + CRLF, prms.join(", "));
                    }
                    else
                    {
                        s.AppendFormat("                    c.loadBy{0}({1});" + CRLF, nm, prms.join(", "));
                    }
                    s.AppendFormat("                    _{0}_1to1 = c;" + CRLF, relName);
                    s.AppendFormat("                }}" + CRLF);
                    s.AppendFormat("                return _{0}_1to1;" + CRLF, relName);
                    s.AppendFormat("            }}" + CRLF);
                    s.AppendFormat("            set {{" + CRLF);
                    s.AppendFormat("                _{0}_1to1 = value;" + CRLF, relName);
                    for (int i = 0; i < rel.fieldsHere.Count; i++)
                    {
                        s.AppendFormat("                this.{0} = value.{1};" + CRLF, rel.tableHere.getCustomFieldPropertyName(rel.fieldsHere[i]), rel.tableThere.getCustomFieldPropertyName(rel.fieldsThere[i]));
                    }
                    s.AppendFormat("            }}" + CRLF);
                    s.AppendFormat("        }}" + CRLF + CRLF);

                    //Generics getter
                    s.Append("        /// <summary>" + CRLF);
                    s.AppendFormat("        /// get {0} FK returning generic type T" + CRLF, rel.tableThere.className);
                    s.Append("        /// </summary>" + CRLF);
                    s.AppendFormat("        public T {0}_of<T>() where T : {1} {{ " + CRLF, relName, rel.tableThere.className);
                    s.AppendFormat("            if (!this.cashObject.ContainsKey(typeof(T))) {{" + CRLF);
                    s.AppendFormat("                T t = (T)Activator.CreateInstance(typeof(T), this.context);" + CRLF);
                    if (sameFields(rel.fieldsThere, pkr))
                    {
                        s.AppendFormat("                t.loadByPK({0});" + CRLF, prms.join(", "));
                    }
                    else
                    {
                        s.AppendFormat("                t.loadBy{0}({1});" + CRLF, nm, prms.join(", "));
                    }
                    s.AppendFormat("                this.cashObject[typeof(T)] = t;" + CRLF);
                    s.AppendFormat("            }}" + CRLF);
                    s.AppendFormat("            return (T)cashObject[typeof(T)];" + CRLF);
                    s.AppendFormat("        }}" + CRLF);

        



                }
            }
        }

        protected static void addFinishClassFile(StringBuilder s)
        {
            s.Append(CRLF);
            s.Append("    }" + CRLF);  //class
            s.Append("}");        //namespace
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////  ABSTRACT  //////////////////////////////////////////////////////////////////////////
        protected abstract void addStartClassFilePersist(TableName tn, StringBuilder s);
        protected abstract void addConstructorPK(TableName table, PKInfo pk, TableColumnsWrap cw, StringBuilder s);
        /// <summary>
        /// will create methods like   loadBy{1}({2})  
        /// </summary>
        protected abstract void addDBOneToManyConstructors(TableName table, PKInfo pk, OrderedDictionary<TableName, ESet<DBRelation>> oneToMany, TableColumnsWrap cw, StringBuilder s);
        /// <summary>
        /// will create methods like "TablePersist selectBy{1}({2})" 
        /// and "DataTable selectDataTableBy{1}({2})"  
        /// </summary>
        protected abstract void addDBForeignKeysConstructors(TableName table, FKInfo fk, TableColumnsWrap cw, StringBuilder s);
        protected abstract void addCustomSelectConstructors(TableName table, FKInfo fk, TableColumnsWrap cw, StringBuilder s, ESet<DBConstraint> customSelect, bool isView);
        protected abstract void addUniqueConstraintConstructors(TableName table, EList<DBConstraint> constraints, TableColumnsWrap cw, StringBuilder s);
        protected abstract void addCustomDeleteMethods(TableName table, FKInfo fk, TableColumnsWrap cw, StringBuilder s, ESet<DBConstraint> customDelete);
        #endregion basic generaor template


    }
}
