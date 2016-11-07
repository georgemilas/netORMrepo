using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;
using EM.Batch;
using System.Net;
using System.Xml;

namespace EM.Util.Config
{
    /*
    <configSections>
	    <section name="servicesSection" type="EM.Util.Config.ServiceLoaderSection, EUtil, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"  />
	</configSections>
     
    <servicesSection>
		<services>
			<service name="class1" type="full name for class 1" order="" />  <!-- order is optional -->			
            <service name="class2" type="full name for class 2" />
		</services>
	</servicesSection>
    */
    public class ConfigServiceLoaderFactory
    {
        public static string getServiceNameParsingXMLConfigFromAssemblyPath(Type tp) 
        {
            /*We USE XML because:
            * 
            * 1) new ConfigServiceLoaderFactory() fails during installUtil because ConfigurationManager opens installUtils configuration not our configuration?
            *      ConfigServiceLoaderFactory sl = new ConfigServiceLoaderFactory();
            *      string configServiceName = sl.getSettings("tickerServicesSection").serviceName;
            * 2) ConfigurationManager.OpenExeConfiguration fails to load 
            *                  <section name="tickerServicesSection" type="EM.Util.Config.ServiceLoaderSection ...
            *          therefor this fails:
            *          var service = Assembly.GetAssembly(typeof(PaychexServicesStarter));
            *          Configuration config = ConfigurationManager.OpenExeConfiguration(service.Location);
            *          var section = config.GetSection("tickerServicesSection");  //fails
            *          ServiceLoaderSection logSection = (ServiceLoaderSection)section;
            *          string configServiceName = logSection.settings.serviceName;
            */
             var app = Assembly.GetAssembly(tp);
             XmlDocument doc = new XmlDocument();
             doc.Load(app.Location + ".config");
             var root = doc.ChildNodes[1];
             var s = (XmlElement)root.SelectSingleNode("tickerServicesSection/settings");
             return s.GetAttribute("serviceName");
        }
        public ServiceSettingsElement getSettings()
        {
            return getSettings("servicesSection");
        }
        public ServiceSettingsElement getSettings(string sectionName)
        {
            ServiceLoaderSection logSection = (ServiceLoaderSection)ConfigurationManager.GetSection(sectionName);
            return logSection.settings;
        }

        public virtual IEnumerable<T> getServices<T>() where T: new() 
        {
            return getServices<T>("servicesSection");
        }

        public virtual IEnumerable<T> getServices<T>(string sectionName) where T : new() 
        {
            ServiceLoaderSection logSection = (ServiceLoaderSection)ConfigurationManager.GetSection(sectionName);
            List<T> res = new List<T>();
            Dictionary<T, int> order = new Dictionary<T, int>();
            foreach (ServiceLoaderElement serv in logSection.services)
            {
                Type tp = Type.GetType(serv.type);
                T inst = (T)Activator.CreateInstance(tp);
                res.Add(inst);

                if (serv.order != null)
                {
                    int ix = int.Parse(serv.order);
                    order[inst] = ix;
                }
            }

            if (order.Count > 0)
            {
                foreach (T s in order.Keys)
                {
                    res.Remove(s);
                    res.Insert(order[s], s);
                }
            }
            return res;
        }

        public virtual IEnumerable<Type> getServices(string sectionName) 
        {
            ServiceLoaderSection logSection = (ServiceLoaderSection)ConfigurationManager.GetSection(sectionName);
            List<Type> res = new List<Type>();
            Dictionary<Type, int> order = new Dictionary<Type, int>();
            foreach (ServiceLoaderElement serv in logSection.services)
            {
                Type tp = Type.GetType(serv.type);
                res.Add(tp);

                if (!String.IsNullOrWhiteSpace(serv.order))
                {
                    int ix = int.Parse(serv.order);
                    order[tp] = ix;
                }
            }

            if (order.Count > 0)
            {
                foreach (Type t in order.Keys)
                {
                    res.Remove(t);
                    res.Insert(order[t], t);
                }
            }
            return res;
        }
        


    }
}
