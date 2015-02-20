using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class TenantElementCollection : ConfigurationElementCollection 
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new TenantElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((TenantElement)element).Name;
        }

        protected override string ElementName
        {
            get { return "tenant"; }
        }

        public new int Count
        {
            get { return base.Count; }
        }


        public TenantElement this[int index]
        {
            get
            {
                return (TenantElement)BaseGet(index);
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

        new public TenantElement this[string Name]
        {
            get
            {
                return (TenantElement)BaseGet(Name);
            }
        }

        public int IndexOf(TenantElement element)
        {
            return BaseIndexOf(element);
        }

        public void Add(TenantElement element)
        {
            BaseAdd(element);
        }

        public void Remove(TenantElement element)
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
        /// 获取当前生效的租户处理接口
        /// </summary>
        /// <returns></returns>
        public TenantElement GetEffectiveElement()
        {
            TenantElement result = null;

            foreach (TenantElement item in this)
            {
                if (item.Enabled)
                {
                    result= item;
                    break;
                }
            }

            return result;
        }
    }
}
