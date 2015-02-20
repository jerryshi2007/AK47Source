using System;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.Permissions
{
	[Serializable]
	public class SCAclPermissionItem
	{
		public SCAclPermissionItem()
		{
		}

		public SCAclPermissionItem(ObjectSchemaPermissionConfigurationElement element)
		{
			element.NullCheck("element");

			this.Name = element.Name;
			this.DisplayName = element.DisplayName;
			this.Description = element.Description;
		}

		/// <summary>
		/// Key
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// 显示名称
		/// </summary>
		public string DisplayName
		{
			get;
			set;
		}

		/// <summary>
		/// 描述信息
		/// </summary>
		public string Description
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 权限中心被授权对象的权限描述信息集合
	/// </summary>
	public class SCAclPermissionItemCollection : SerializableEditableKeyedDataObjectCollectionBase<string, SCAclPermissionItem>
	{
		public SCAclPermissionItemCollection()
		{
		}

		public SCAclPermissionItemCollection(ObjectSchemaPermissionConfigurationElementCollection elements)
		{
			this.LoadFromConfiguration(elements);
		}

		protected SCAclPermissionItemCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		public void LoadFromConfiguration(ObjectSchemaPermissionConfigurationElementCollection elements)
		{
			this.Clear();

			if (elements != null)
			{
				foreach (ObjectSchemaPermissionConfigurationElement element in elements)
					this.Add(new SCAclPermissionItem(element));
			}
		}

		protected override string GetKeyForItem(SCAclPermissionItem item)
		{
			return item.Name;
		}
	}
}
