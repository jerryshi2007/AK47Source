using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using System.Data;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters
{
	/// <summary>
	/// 表示模式对象适配器的基类
	/// </summary>
	/// <typeparam name="T"><see cref="AUObjectBase"/>的派生类型。</typeparam>
	public abstract class AUObjectAdapterBase<T> : PC.Adapters.SchemaObjectAdapterBase<T> where T : SchemaObjectBase
	{
		protected override PC.Actions.SchemaObjectUpdateActionCollection GetActions(string actionName)
		{
			return Configuration.AUUpdateActionConfigurationSection.GetConfig().GetActions(actionName);
		}

		/// <summary>
		/// 获取连接的名称
		/// </summary>
		/// <returns>表示连接名称的<see cref="string"/>。</returns>
		protected override string GetConnectionName()
		{
			return AUCommon.PCConnectionName;
		}
	}
}
