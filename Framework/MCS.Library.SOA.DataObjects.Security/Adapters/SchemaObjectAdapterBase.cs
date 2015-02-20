using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 表示模式对象适配器的基类
	/// </summary>
	/// <typeparam name="T"><see cref="SchemaObjectBase"/>的派生类型。</typeparam>
	public abstract class SchemaObjectAdapterBase<T> : VersionedSchemaObjectAdapterBase<T> where T : SchemaObjectBase
	{
		protected override SchemaObjectUpdateActionCollection GetActions(string actionName)
		{
			return SchemaObjectUpdateActionSettings.GetConfig().GetActions(actionName);
		}

		/// <summary>
		/// 在派生类中重写时， 获取映射信息的集合
		/// </summary>
		/// <returns><see cref="ORMappingItemCollection"/>，表示映射信息</returns>
		protected override ORMappingItemCollection GetMappingInfo()
		{
			return ORMapping.GetMappingInfo(typeof(T));
		}

		/// <summary>
		/// 获取连接的名称
		/// </summary>
		/// <returns>表示连接名称的<see cref="string"/>。</returns>
		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
