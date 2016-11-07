using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace EM.Util.Config
{
    public class ServiceLoaderCollection : ConfigurationElementCollection
    {
        public void Add(ServiceLoaderElement logger)
        {
            base.BaseAdd(logger, false);
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceLoaderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceLoaderElement)element).name;
        }

        public ServiceLoaderElement this[int Index]
        {
            get
            {
                return (ServiceLoaderElement)BaseGet(Index);
            }
            set
            {
                if (BaseGet(Index) != null)
                {
                    BaseRemoveAt(Index);
                }
                BaseAdd(Index, value);
            }
        }

        new public ServiceLoaderElement this[string id]
        {
            get
            {
                return (ServiceLoaderElement)BaseGet(id);
            }
        }

        public int indexof(ServiceLoaderElement element)
        {
            return BaseIndexOf(element);
        }

        public void Remove(ServiceLoaderElement url)
        {
            if (BaseIndexOf(url) >= 0)
                BaseRemove(url.name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string id)
        {
            BaseRemove(id);
        }

        public void Clear()
        {
            BaseClear();
        }

    }


}
