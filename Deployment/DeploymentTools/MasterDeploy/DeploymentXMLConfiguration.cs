using System.Linq;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System;

namespace MasterDeploy
{
    // <?xml version="1.0" encoding="utf-8" ?>
    // <deployment rootFolder="C:\Test\DeployTest">
    //        <deploy deployType="SQL" folder="DatabaseScriptsDeployment\SQL" >
    //              [sql_source_folder]= {rootFolder}\{folder}
    //              [sql_exceptions]=
    //              [sql_use_win_auth]= true
    //              ...  the CFG config elemets for the original tab in the deployment tools app
    //        </deploy>
    //        
    // ...    
    // </deployment>      
    
    public class DeploymentXMLConfiguration
    {
        public string xmlFile; 
        
        public DeploymentXMLConfiguration(string xmlFile)
        {
            this.xmlFile = xmlFile;
        }

        public string rootFolder;
        private List<DeployAction> _deployActions = new List<DeployAction>();
        public IEnumerable<DeployAction> DeployActions { get { return _deployActions; } }
        
        public static void test()
        {

            DeploymentXMLConfiguration xml = new DeploymentXMLConfiguration(@"c:\Source\SurePayroll\Dev\SurePayroll10\Tools\MasterBuild\MasterBuild\GroupPatterns.xml");
            xml.parse();            
        }

        
        private string replaceConfigValues(string str, string folder)
        {
            return str.Replace("{rootFolder}", this.rootFolder)
                      .Replace("{folder}", folder);
        }

        /// <summary>
        /// reload configuration on the existing objects created during first parse 
        /// </summary>
        public List<DeployAction> reload()
        {
            return this.parse();
        }



        private object _lockObj = new object();
        public List<DeployAction> parse()
        {
            List<DeployAction> actions = new List<DeployAction>();
            var doc = new XmlDocument();
            doc.Load(xmlFile);
            var root = doc.ChildNodes[1]; //the <groups> node, not the <xml> node
            var rootEl = (XmlElement)root;
            lock (_lockObj)
            {
                if (this.rootFolder == null) { this.rootFolder = rootEl.GetAttribute("rootFolder"); }                
            }

            foreach (XmlNode p in root.ChildNodes)
            {
                if (p is XmlComment) { continue; }
                var el = (XmlElement)p;

                string deployType = el.GetAttribute("deployType");
                string folder = el.GetAttribute("folder");
                string rawConfig = p.InnerText.Trim();
                rawConfig = replaceConfigValues(rawConfig, folder);
                bool disable = el.GetAttribute("disable").ToLower() == "true" ? true : false;
                Tuple<DeployAction, bool> ac = null;
                switch (deployType)
                {
                    case "SQL":
                        ac = bindAction(rawConfig, folder, disable, () => new SQLDeployAction(rawConfig, folder, disable));
                        break;
                    case "COM":
                        ac = bindAction(rawConfig, folder, disable, () => new COMDeployAction(rawConfig, folder, disable));                        
                        break;
                    case "PROD_FILE_DEPLOY":
                        ac = bindAction(rawConfig, folder, disable, () => new ProdFilesDeployAction(rawConfig, folder, disable));                        
                        break;
                    case "SERVICE":
                        ac = bindAction(rawConfig, folder, disable, () => new WinServiceDeployAction(rawConfig, folder, disable));                        
                        break;
                    default:
                        continue;
                }

                if (ac.Item2)  //a new instance was created 
                {
                    actions.Add(ac.Item1);                    
                }
            }


            lock (_lockObj)
            {
                this._deployActions.AddRange(actions); //add the new found actions
            }

            return actions;

        }

        /// <summary>
        /// return the action and whether it is a new instance (true) or an existing instace that was reconfigured (false)
        /// </summary>        
        public Tuple<DeployAction, bool> bindAction<T>(string rawConfig, string name, bool disable, Func<T> instanceFunc) where T : DeployAction
        {
            var tp = from a in this.DeployActions where a.GetType() == typeof(T) && a.name == name select a;
            T ac = null;
            if (tp.Count() == 1)
            {
                ac = (T)tp.First();
            }
            if (ac == null)
            {
                ac = instanceFunc();
                return new Tuple<DeployAction, bool>(ac, true);
            }
            else
            {
                ac.PrepareConfig(rawConfig);
                ac.disable = disable; 
                return new Tuple<DeployAction, bool>(ac, false);
            }            
        }


    }
}
