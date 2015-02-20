using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Schemas.Configuration
{
	/// <summary>
	/// Schema的配置项
	/// </summary>
	public class ObjectSchemaConfigurationElement : ObjectSchemaConfigurationElementBase
	{
		/// <summary>
		/// 类型描述信息
		/// </summary>
		/// <remarks>一般采用QualifiedName （QuanlifiedTypeName, AssemblyName）方式</remarks>
		[ConfigurationProperty("type", IsRequired = false, DefaultValue = "MCS.Library.SOA.DataObjects.SCGenericObject, MCS.Library.SOA.DataObjects.Security")]
		public override string Type
		{
			get
			{
				return base.Type;
			}
		}

		/// <summary>
		/// 是不是群组或角色之类的人员的容器
		/// </summary>
		[ConfigurationProperty("isUsersContainer", DefaultValue = false, IsRequired = false)]
		public bool IsUsersContainer
		{
			get
			{
				return (bool)this["isUsersContainer"];
			}
		}

		/// <summary>
		/// 是不是群组或角色之类的人员的容器的成员。通常人员会设置为true
		/// </summary>
		[ConfigurationProperty("isUsersContainerMember", DefaultValue = false, IsRequired = false)]
		public bool IsUsersContainerMember
		{
			get
			{
				return (bool)this["isUsersContainerMember"];
			}
		}

		[ConfigurationProperty("logoImage", DefaultValue = "ou.gif", IsRequired = false)]
		public string LogoImage
		{
			get
			{
				return (string)this["logoImage"];
			}
		}

		/// <summary>
		/// CodeName的分组的Key，同一Key的对象CodeName才是唯一的
		/// </summary>
		[ConfigurationProperty("codeNameKey", DefaultValue = "", IsRequired = false)]
		public string CodeNameKey
		{
			get
			{
				string result = (string)this["codeNameKey"];

				if (result.IsNullOrEmpty())
					result = this.Name;

				return result;
			}
		}

		/// <summary>
		/// 对象的CodeName唯一性校验逻辑
		/// </summary>
		[ConfigurationProperty("codeNameValidationMethod", DefaultValue = SchemaObjectCodeNameValidationMethod.ByCodeNameKey, IsRequired = false)]
		public SchemaObjectCodeNameValidationMethod CodeNameValidationMethod
		{
			get
			{
				return (SchemaObjectCodeNameValidationMethod)this["codeNameValidationMethod"];
			}
		}

		/// <summary>
		/// 对象的FullPath唯一性校验逻辑
		/// </summary>
		[ConfigurationProperty("fullPathValidationMethod", DefaultValue = SCRelationFullPathValidationMethod.None, IsRequired = false)]
		public SCRelationFullPathValidationMethod FullPathValidationMethod
		{
			get
			{
				return (SCRelationFullPathValidationMethod)this["fullPathValidationMethod"];
			}
		}

		/// <summary>
		/// Schema对应增、删、改的操作方法定义
		/// </summary>
		[ConfigurationProperty("operations", IsRequired = false)]
		public ObjectSchemaOperationElementCollection Operations
		{
			get
			{
				return (ObjectSchemaOperationElementCollection)this["operations"];
			}
		}

		/// <summary>
		/// 所需要的权限集合
		/// </summary>
		[ConfigurationProperty("permissionSet", IsRequired = false)]
		public ObjectSchemaPermissionConfigurationElementCollection PermissionSet
		{
			get
			{
				return (ObjectSchemaPermissionConfigurationElementCollection)this["permissionSet"];
			}
		}
	}

	/// <summary>
	/// Schema的配置项集合
	/// </summary>
	public class ObjectSchemaConfigurationElementCollection : NamedConfigurationElementCollection<ObjectSchemaConfigurationElement>
	{
	}
}
