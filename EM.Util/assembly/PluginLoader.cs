using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using EM.Collections;
using System.Threading;

namespace EM.Util
{
    /// <summary>
    /// use it to do something like this Invoke(new PluginAdder(a_method));
    /// from the assembly loader event handler of your main form in order to 
    /// add GUI controls from the thread of the main form 
    /// and not from the tread of the assembly loader that calls the loading handler
    /// </summary>
    public delegate void PluginAdder(Assembly plugin);

    /// <summary>
    /// usage:
    ///     PluginLoader pl;
    ///     public Form_Load() {
    ///         pl = new PluginLoader();
    ///         pl.OnLoadPlugin += ....
    ///         pl.startLoadingLoop();  
    ///           OR
    ///         pl.load_once(); //do one load and be done  
    ///     }
    ///     public Form_Close() {
    ///         pl.stopLoadingLoop();
    ///     }
    /// </summary>
    public class PluginLoader
    {
        public OrderedDictionary<string, Assembly> loadedPugins { get; private set;}

        public delegate void LoadHandler(Assembly plugin);
        public event LoadHandler OnLoadPlugin;
        
        protected Thread loadingThread;

        private string _loadingFolder;
        /// <summary>
        /// - Gets/Sets the loading folder 
        /// - defaults to the folder where the program was deployed
        /// </summary>
        public string loadingFolder
        {
            get { return _loadingFolder; }
            set { _loadingFolder = value; }
        }

        public PluginLoader()
        {
            this.loadedPugins = new OrderedDictionary<string, Assembly>();
            this.loadingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public bool isLoading 
        { 
            get 
            { 
                return this.loadingThread != null && this.loadingThread.IsAlive;
            } 
        }

        public void startLoadingLoop() 
        {
            //let people call the start more then one time 
            if ( !this.isLoading )  
            {
                if (this.OnLoadPlugin == null)
                {
                    throw new InvalidOperationException("OnLoadPlugin event has no registered handlers, please register at list one event handler;");
                }
                this.loadingThread = new Thread(loadLoop);
                this.loadingThread.IsBackground = true;
                this.loadingThread.Start();
            }
        }

        public void stopLoadingLoop()
        {
            if (loadingThread != null)
            {
                loadingThread.Abort();
                while (loadingThread.IsAlive)
                {
                    Thread.Sleep(100);
                }
                loadingThread = null;
            }
        }


        protected void loadLoop()
        {
            try
            {
                while (true)
                {
                    load_once();
                    Thread.Sleep(1000 * 60);
                }
            }
            catch (ThreadAbortException ex)
            {
                //okidoki
            }
        }


        private object _locker = new object();

        /// <summary>
        ///  - Perform one Plugins Load 
        ///  - you should probably use the loading loop via startLoadingLoop instead, if you want to be able to 
        ///    load plugins as they apear without having to do a program restart
        /// </summary>
        public void load_once()
        {
            if (OnLoadPlugin == null)
            {
                throw new InvalidOperationException("OnLoadPlugin event has no registered handlers, please register at list one event handler;");
            }
            lock (_locker)
            {
                string[] files = Directory.GetFiles(this.loadingFolder);
                foreach (string file in files)
                {
                    if (file.ToLower().EndsWith("plugin.dll") || file.ToLower().EndsWith("plugin.exe"))
                    {
                        if (!loadedPugins.ContainsKey(file))
                        {
                            Assembly plugin = Assembly.LoadFile(file);
                            OnLoadPlugin(plugin);
                            loadedPugins.Add(file, plugin);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   - pseudocode: foreach( p as instantiate<IPlugin>(assembly) ) p.install(host)
        ///   - if the classes in the assembly don't implement IPlugin you will have to use instantiate manualy 
        ///     on whatever type and do the instalation into the host program manually 
        /// </summary>
        public void installPlugin(Assembly plugin, IPluginHost host) 
        {
            EList<IPlugin> clsList = AssemblyLoader.instantiate<IPlugin>(plugin);
            foreach (IPlugin p in clsList)
            {
                p.installPlugin(host);
            }
        }


    }
}
