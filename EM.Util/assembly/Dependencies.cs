using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using EM.Collections;

namespace EM.Util
{
    [Serializable]
    /// <summary>
    /// - Finds all Assemblies that the given entry Assembly depends on 
    /// - recursive
    /// - excludes basic .NET assemblies (mscorlib, System etc.)
    /// </summary>
    public class Dependencies: MarshalByRefObject
    {
        public Dependencies() { }
        public Dependencies(string assemblyPath)
        {
            this.entryAssembly = Assembly.LoadFile(assemblyPath);
        }
        public Dependencies(Assembly entryAssembly)
        {
            this.entryAssembly = entryAssembly;
        }

        private List<string> _excludeList;
        /// <summary>
        /// - get/set list of Assemblies to exclude like basic .NET assemblies 
        /// - Defaults to (mscorlib, System, Microsoft.VisualStudio, Microsoft.VisualC, Microsoft.VisualBasic, Microsoft.Windows, Microsoft.Office, Microsoft.CompactFramework)
        /// </summary>
        public List<string> ExcludeList
        {
            get 
            {
                if (_excludeList == null)
                {
                    _excludeList = new List<string>();
                    _excludeList.Add("mscorlib");   //.NET loader
                    _excludeList.Add("System");     //main .NET stuff
                    _excludeList.Add("Microsoft.VisualStudio");  
                    _excludeList.Add("Microsoft.VisualC");
                    _excludeList.Add("Microsoft.VisualBasic");
                    _excludeList.Add("Microsoft.CompactFramework");
                    _excludeList.Add("Microsoft.Windows");
                    _excludeList.Add("Microsoft.Office");
                }
                return _excludeList;
            }
            set 
            { 
                _excludeList = value; 
            }
        }


        private string _entryAssemblyPath;
        public string entryAssemblyPath
        {
            get { return _entryAssemblyPath; }
            set { 
                _entryAssemblyPath = value;
                entryAssembly = Assembly.LoadFile(value);
            }
        }

        private Assembly _entryAssembly;
        /// <summary>
        /// Gets/Sets the assembly for which to search for dependencies
        /// </summary>
        public Assembly entryAssembly
        {
            get { return this._entryAssembly; }
            set 
            { 
                _entryAssembly = value;
                _PDBs = null;
                _assemblies = null;
                _entryAssemblyPath = value.Location;
            }
        }

        private bool _excludeGACAssembly = false;
        /// <summary>
        /// Should Assemblies installed in GAC be excluded?
        /// </summary>
        public bool excludeGACAssembly
        {
            get { return _excludeGACAssembly; }
            set { _excludeGACAssembly = value; }
        }


        private bool _recursive = true;
        /// <summary>
        /// - If the search for dependencies should be recursive or not? 
        /// - Defaults to recursive 
        /// </summary>
        public bool isRecursive
        {
            get { return _recursive; }
            set { _recursive = value; }
        }

        private bool isExcluded(AssemblyName an)
        {
            string anlower = an.FullName.ToLower();
            foreach (string s in this.ExcludeList)
            {
                if (anlower.StartsWith(s.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Copies the list of dependencies to a given destination folder
        /// </summary>
        public void copy(DirectoryInfo destination)
        {
            this.copy(destination, false);
        }
        /// <summary>
        /// Copies the list of dependencies and the PDB files to a given destination folder
        /// </summary>
        public void copy(DirectoryInfo destination, bool includePDBs)
        {
            foreach (Assembly a in this.assemblies)
            {
                string dest = Path.Combine(destination.FullName, Path.GetFileName(a.Location));
                File.Copy(a.Location, dest, true);
                if (includePDBs)
                {
                    string srcPDBPath = Path.Combine(Path.GetDirectoryName(a.Location), Path.GetFileNameWithoutExtension(a.Location) + ".pdb");
                    if (File.Exists(srcPDBPath))
                    {
                        string destPDBPath = Path.Combine(destination.FullName, Path.GetFileName(srcPDBPath));
                        File.Copy(srcPDBPath, destPDBPath, true);
                    }
                }
            }
        }

        public object createInstance(string fullTypeName)
        {
            if (this.assemblies != null)
            {
                return this.entryAssembly.CreateInstance(fullTypeName);
            }
            return null;
        }


        private ESet<Assembly> _assemblies;
        /// <summary>
        /// get the list of dependencies
        /// </summary>
        public ESet<Assembly> assemblies
        {
            get
            {
                if (_assemblies == null)
                {
                    if (this.entryAssembly == null)
                    {
                        throw new InvalidOperationException("entryAssembly must be specified (must not be null)");
                    }
                    this.useThisDependenciesForInstanceLoad = false;
                    _assemblies = new ESet<Assembly>();
                    _assemblies.Add(this.entryAssembly);
                    this.find(this.entryAssembly, _assemblies);
                    //if (this.useThisDependenciesForInstanceLoad)
                    //{
                        AppDomain.CurrentDomain.AssemblyResolve += this.AssemblyResolver;
                    //}
                }
                return _assemblies;
            }
        }

        public Assembly AssemblyResolver(object sender, ResolveEventArgs args)
        {
            return this.assemblies.Find(delegate(Assembly cur) { return cur.FullName == args.Name; });
        }

        private List<string> _PDBs;
        /// <summary>
        /// Get the list of PDB files associated with the dependencies
        /// </summary>
        public List<string> PDBs
        {
            get 
            {
                if (_PDBs == null)
                {
                    _PDBs = new List<string>();
                    foreach (Assembly a in this.assemblies)
                    {
                        string srcPDBPath = Path.Combine(Path.GetDirectoryName(a.Location), Path.GetFileNameWithoutExtension(a.Location) + ".pdb");
                        if (File.Exists(srcPDBPath))
                        {
                            _PDBs.Add(srcPDBPath);
                        }                        
                    }
                }
                return _PDBs;
            }
        }

        private bool useThisDependenciesForInstanceLoad = false;
        private void find(Assembly main, ESet<Assembly> stack)
        {
            foreach (AssemblyName dependencyName in main.GetReferencedAssemblies())
            {
                // skip assemblies that are in the exclusion list
                if (this.isExcluded(dependencyName)) { continue; }
                Assembly dependency = null;
                try
                {
                    dependency = Assembly.Load(dependencyName);
                }
                catch
                {
                    this.useThisDependenciesForInstanceLoad = true;

                    FileInfo fi = new FileInfo(main.Location);
                    string fname = Path.Combine(fi.DirectoryName, dependencyName.Name + ".dll");
                    try
                    {
                        dependency = Assembly.LoadFrom(fname);
                    }
                    catch
                    {
                        fname = Path.Combine(fi.DirectoryName, dependencyName.Name + ".exe");
                        try
                        {
                            dependency = Assembly.LoadFrom(fname);
                        }
                        catch { }
                    }
                }

                if (dependency != null)
                {
                    // skip assemblies that are in the GAC
                    if (this.excludeGACAssembly && dependency.GlobalAssemblyCache) { continue; }

                    if (!stack.Contains(dependency))
                    {
                        stack.Add(dependency);
                        if (this.isRecursive)
                        {
                            this.find(dependency, stack);
                        }
                    }
                }
            }
        }




    }

}
