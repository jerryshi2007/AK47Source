using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class UrlElementCollection : ConfigurationElementCollection 
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new UrlElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((UrlElement)element).Name;
        }

        protected override string ElementName
        {
            get { return "url"; }
        }

        public new int Count
        {
            get { return base.Count; }
        }


        public UrlElement this[int index]
        {
            get
            {
                return (UrlElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public UrlElement this[string Name]
        {
            get
            {
                return (UrlElement)BaseGet(Name);
            }
        }

        public int IndexOf(UrlElement element)
        {
            return BaseIndexOf(element);
        }

        public void Add(UrlElement element)
        {
            BaseAdd(element);
        }

        public void Remove(UrlElement element)
        {
            if (BaseIndexOf(element) >= 0)
                BaseRemove(element.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string client)
        {
            BaseRemove(client);
        }

        public void Clear()
        {
            BaseClear();
        }
        /// <summary>
        /// 根据名称获取Url对象
        /// </summary>
        /// <returns></returns>
        public UrlElement GetUrlElement(string pageUrlName)
        {
            UrlElement result = null;

            foreach (UrlElement item in this)
            {
                if (item != null && item.Name == pageUrlName)
                {
                    result = item;
                    break;
                }
            }

            return result;
        }
    }
}
