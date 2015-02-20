using System;
using MCS.Library.Core;
using System.Xml.Serialization;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	[Serializable]
	public abstract class ClientSchemaObjectBase : ClientObjectBase
	{
		private string _SchemaType = string.Empty;

		/// <summary>
		/// 初始化<see cref="SchemaObjectBase"/>成员
		/// </summary>
		protected ClientSchemaObjectBase()
		{
		}

		protected virtual void OnSchemaTypeDirty()
		{
		}

		/// <summary>
		/// 使用指定的模式名初始化<see cref="SchemaObjectBase"/>成员
		/// </summary>
		/// <param name="schemaType">模式的名称</param>
		public ClientSchemaObjectBase(string schemaType)
		{
			schemaType.CheckStringIsNullOrEmpty("schemaType");

			this._SchemaType = schemaType;
		}

		/// <summary>
		/// 在派生类中重写时，获取或设置模式的类型
		/// </summary>
		/// <remarks>当替调用setter时，会引发OnSchemaTypeDirty。</remarks>
		public virtual string SchemaType
		{
			get
			{
				return this._SchemaType;
			}

			set
			{
				this._SchemaType = value;
				this.OnSchemaTypeDirty();
			}
		}

		/// <summary>
		/// 在派生类中重写时，获取或设置ID
		/// </summary>
		[XmlAttribute]
		public virtual string ID
		{
			get;
			set;
		}

		/// <summary>
		/// 在派生类中重写时，获取创建日期
		[XmlAttribute]
		public virtual DateTime CreateDate
		{
			get;
			set;
		}

		private ClientPropertyValueCollection _Properties = new ClientPropertyValueCollection();

		[XmlElement]
		public ClientPropertyValueCollection Properties
		{
			get
			{
				return this._Properties;
			}
		}

		public ClientOguUser Creator
		{
			get;
			set;
		}
	}
}
