using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Executors;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 表示模式操作的定义
	/// </summary>
	[Serializable]
	public class SchemaOperationDefine
	{
		/// <summary>
		/// 初始化<see cref="SchemaOperationDefine"/>的新实例
		/// </summary>
		public SchemaOperationDefine()
		{
		}

		/// <summary>
		/// 根据指定的配置元素 初始化<see cref="SchemaOperationDefine"/>的新实例
		/// </summary>
		/// <param name="element"><see cref="ObjectSchemaOperationElement"/>对象</param>
		public SchemaOperationDefine(ObjectSchemaOperationElement element)
		{
			element.NullCheck("element");

			this.OperationMode = (SCObjectOperationMode)Enum.Parse(typeof(SCObjectOperationMode), element.Name, true);
			this.MethodName = element.Method;
			this.HasParentParemeter = element.HasParentParemeter;
		}

		/// <summary>
		/// 获取或设置<see cref="SCObjectOperationMode"/>值之一，表示操作的类型
		/// </summary>
		public SCObjectOperationMode OperationMode
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置方法名称
		/// </summary>
		public string MethodName
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置一个<see cref="bool"/>值，表示是否具有父参数
		/// </summary>
		public bool HasParentParemeter
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 表示模式操作定义的集合
	/// </summary>
	[Serializable]
	public class SchemaOperationDefineCollection : SerializableEditableKeyedDataObjectCollectionBase<SCObjectOperationMode, SchemaOperationDefine>
	{
		/// <summary>
		/// 初始化<see cref="SchemaOperationDefineCollection"/>的新实例
		/// </summary>
		public SchemaOperationDefineCollection()
		{
		}

		/// <summary>
		/// 从配置信息中初始化
		/// </summary>
		/// <param name="elements"></param>
		public SchemaOperationDefineCollection(ObjectSchemaOperationElementCollection elements)
		{
			this.LoadFromConfiguration(elements);
		}

		/// <summary>
		/// 反序列化方式专用
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected SchemaOperationDefineCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		public void LoadFromConfiguration(ObjectSchemaOperationElementCollection elements)
		{
			this.Clear();

			if (elements != null)
			{
				foreach (ObjectSchemaOperationElement opElem in elements)
				{
					this.Add(new SchemaOperationDefine(opElem));
				}
			}
		}

		/// <summary>
		/// 获取指定对象的键
		/// </summary>
		/// <param name="item">要在集合中查找键的<see cref="SchemaOperationDefine"/>对象</param>
		/// <returns><see cref="SCObjectOperationMode"/>值</returns>
		protected override SCObjectOperationMode GetKeyForItem(SchemaOperationDefine item)
		{
			return item.OperationMode;
		}
	}
}
