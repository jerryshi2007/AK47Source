using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class PropertyDefineCollection : SerializableEditableKeyedDataObjectCollectionBase<string, PropertyDefine>
	{
		public static PropertyDefineCollection CreatePropertiesFromConfiguration(string groupName)
		{
			PropertyDefineCollection pds = new PropertyDefineCollection();

			pds.LoadPropertiesFromConfiguration(groupName);
	
			return pds;
		}

		public static PropertyDefineCollection CreatePropertiesFromConfiguration(PropertyGroupConfigurationElement group)
		{
			PropertyDefineCollection pds = new PropertyDefineCollection();

			pds.LoadPropertiesFromConfiguration(group);

			return pds;
		}

		public void LoadPropertiesFromConfiguration(string groupName)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(groupName, "groupName");

			PropertyGroupConfigurationElement group = PropertyGroupSettings.GetConfig().Groups[groupName];

			ExceptionHelper.FalseThrow<KeyNotFoundException>(group != null, "不能根据'{0}'找到对应的属性组定义", groupName);

			LoadPropertiesFromConfiguration(group);
		}

		public void AppendPropertiesFromConfiguration(string groupName)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(groupName, "groupName");

			PropertyGroupConfigurationElement group = PropertyGroupSettings.GetConfig().Groups[groupName];

			ExceptionHelper.FalseThrow<KeyNotFoundException>(group != null, "不能根据'{0}'找到对应的属性组定义", groupName);

			AppendPropertiesFromConfiguration(group);
		}

		public void LoadPropertiesFromConfiguration(PropertyGroupConfigurationElement group)
		{
			this.Clear();

			AppendPropertiesFromConfiguration(group);
		}

		public void AppendPropertiesFromConfiguration(PropertyGroupConfigurationElement group)
		{
			if (group != null)
			{
				foreach (PropertyDefineConfigurationElement propDefineElement in group.AllProperties)
				{
					if (this.ContainsKey(propDefineElement.Name))
					{
						//徐磊注释   2012/6/14
						//if (propDefineElement.AllowOverride)
						//{ 
						//    this.Remove(pd => pd.Name == propDefineElement.Name);
						//    this.Add(new PropertyDefine(propDefineElement));
						//}
					}
					else
						this.Add(new PropertyDefine(propDefineElement));
				}
			}
		}

		protected override string GetKeyForItem(PropertyDefine item)
		{
			return item.Name;
		}
	}
}
