using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using MCS.Library.SOA.DataObjects.Schemas.Client;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	[Serializable]
	public class ClientSCBase : ClientSchemaObjectBase
	{
		protected ClientSCBase()
		{
		}

		public ClientSCBase(string schemaType)
			: base(schemaType)
		{
		}

		/// <summary>
		/// 获取或设置名称
		/// </summary>
		[XmlIgnore]
		[ScriptIgnore]
		public string Name
		{
			get
			{
				return this.Properties.GetValue("Name", string.Empty); ;
			}

			set
			{
				this.Properties.AddOrSetValue("Name", ClientPropertyDataType.String, value ?? string.Empty);
			}
		}

		/// <summary>
		/// 获取或设置显示名称
		/// </summary>
		[XmlIgnore]
		[ScriptIgnore]
		public string DisplayName
		{
			get
			{
				return this.Properties.GetValue("DisplayName", string.Empty);
			}

			set
			{
				this.Properties.AddOrSetValue("DisplayName", ClientPropertyDataType.String, value ?? string.Empty);
			}
		}

		/// <summary>
		/// 获取或设置代码名称
		/// </summary>
		[XmlIgnore]
		[ScriptIgnore]
		public string CodeName
		{
			get
			{
				return this.Properties.GetValue("CodeName", string.Empty); ;
			}

			set
			{
				this.Properties.AddOrSetValue("CodeName", ClientPropertyDataType.String, value ?? string.Empty);
			}
		}
	}

	[Serializable]
	public class ClientSCBaseCollection : EditableDataObjectCollectionBase<ClientSCBase>
	{
		/// <summary>
		/// 初始化<see cref="ClientSCBaseCollection"/>的新实例。
		/// </summary>
		public ClientSCBaseCollection()
		{
		}

		/// <summary>
		/// 使用一组<see cref="ClientSCBase"/>初始化<see cref="ClientSCBaseCollection"/>的新实例。
		/// </summary>
		/// <param name="clientObjects"></param>
		public ClientSCBaseCollection(IEnumerable<ClientSCBase> clientObjects)
		{
			clientObjects.NullCheck("clientObjects");

			clientObjects.ForEach(obj => this.Add(obj));
		}
	}

	[Serializable]
	public class ClientSCBaseKeyedCollection : SerializableEditableKeyedDataObjectCollectionBase<string, ClientSCBase>
	{
		/// <summary>
		/// 初始化<see cref="SchemaPropertyValueCollection"/>的新实例
		/// </summary>
		public ClientSCBaseKeyedCollection()
		{
		}

		public ClientSCBaseKeyedCollection(IEnumerable<ClientSCBase> clientObjects)
		{
			clientObjects.NullCheck("clientObjects");

			clientObjects.ForEach(obj => this.Add(obj));
		}

		/// <summary>
		/// 使用指定的<see cref="SerializationInfo"/>和<see cref="StreamingContext"/>初始化<see cref="SchemaPropertyValueCollection"/>的新实例
		/// </summary>
		/// <param name="info">存储将对象序列化或反序列化所需的全部数据</param>
		/// <param name="context">序列化描述的上下文</param>
		protected ClientSCBaseKeyedCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		protected override string GetKeyForItem(ClientSCBase item)
		{
			return item.ID;
		}
	}
}
