using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using System.Diagnostics;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	/// <summary>
	/// Schema中的属性定义
	/// </summary>
	[Serializable]
	[XElementSerializable]
	[ORTableMapping("SC.SchemaPropertyDefine")]
	[DebuggerDisplay("DataType={DataType}")]
	public class SchemaPropertyDefine : PropertyDefine
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public SchemaPropertyDefine()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="propertyDefineElem"></param>
		public SchemaPropertyDefine(SchemaPropertyDefineConfigurationElement propertyDefineElem) :
			base(propertyDefineElem)
		{
			propertyDefineElem.NullCheck("propertyDefineElem");

			this.Tab = propertyDefineElem.Tab;
			this.SnapshotMode = propertyDefineElem.SnapshotMode;
			this.SnapshotFieldName = propertyDefineElem.SnapshotFieldName;
		}

		/// <summary>
		/// 属性所属页签
		/// </summary>
		[XElementFieldSerialize(AlternateFieldName = "_Tab")]
		public string Tab { get; set; }

		/// <summary>
		/// 属性的快照模式
		/// </summary>
		[XElementFieldSerialize(AlternateFieldName = "_SnapshotMode")]
		public SnapshotModeDefinition SnapshotMode { get; set; }

		/// <summary>
		/// 属性生成快照时的字段名称，如果没有进行设置，则为PropertyName
		/// </summary>
		[XElementFieldSerialize(AlternateFieldName = "_SnapshotFieldName")]
		public string SnapshotFieldName { get; set; }
	}
}
