using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 用户个人设置的类别
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class UserSettingsCategory
	{
		private PropertyValueCollection _Properties = null;

		internal UserSettingsCategory()
		{
		}

		internal UserSettingsCategory(PropertyGroupConfigurationElement elem)
		{
			InitFromConfiguration(elem);
		}

		/// <summary>
		/// 类别的名称
		/// </summary>
		public string Name
		{
			get;
			internal set;
		}

		/// <summary>
		/// 类别的描述
		/// </summary>
		public string Description
		{
			get;
			internal set;
		}

		/// <summary>
		/// 类别所对应的属性
		/// </summary>
		public PropertyValueCollection Properties
		{
			get
			{
				if (this._Properties == null)
				{
					this._Properties = new PropertyValueCollection();
				}

				return this._Properties;
			}
		}

		/// <summary>
		/// 从配置信息初始化
		/// </summary>
		/// <param name="categoryName"></param>
		internal void InitFromConfiguration(PropertyGroupConfigurationElement elem)
		{
			this.Name = elem.Name;
			this.Description = elem.Description;

			this._Properties = new PropertyValueCollection();

			PropertyDefineCollection definedProperties = new PropertyDefineCollection();
			definedProperties.LoadPropertiesFromConfiguration(elem);

			this._Properties.InitFromPropertyDefineCollection(definedProperties);
		}

		internal void ImportProperties(UserSettingsCategory srcCategory)
		{
			foreach (PropertyValue property in this.Properties)
			{
				PropertyValue srcProperty = srcCategory.Properties[property.Definition.Name];

				if (srcProperty != null)
					property.StringValue = srcProperty.StringValue;
			}
		}
	}

	/// <summary>
	/// 用户个人设置的类别集合
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class UserSettingsCategoryCollection : SerializableEditableKeyedDataObjectCollectionBase<string, UserSettingsCategory>
	{
		/// <summary>
		/// 得到Key
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override string GetKeyForItem(UserSettingsCategory item)
		{
			return item.Name;
		}

	}
}
