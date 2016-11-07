using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace EM.Logging.Config
{
    public class LoggersCollection : ConfigurationElementCollection
    {
        public void Add(LoggerElement logger)
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
            return new LoggerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoggerElement)element).id;
        }

        public LoggerElement this[int Index]
        {
            get
            {
                return (LoggerElement)BaseGet(Index);
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

        new public LoggerElement this[string id]
        {
            get
            {
                return (LoggerElement)BaseGet(id);
            }
        }

        public int indexof(LoggerElement element)
        {
            return BaseIndexOf(element);
        }

        public void Remove(LoggerElement url)
        {
            if (BaseIndexOf(url) >= 0)
                BaseRemove(url.id);
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
