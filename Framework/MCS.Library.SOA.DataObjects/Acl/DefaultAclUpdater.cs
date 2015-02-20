using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects
{
	internal class DefaultAclUpdater : UpdatableAndLoadableAdapterBase<WfAclItem, WfAclItemCollection>, IAclUpdater
	{
		public static readonly DefaultAclUpdater Instance = new DefaultAclUpdater();

		private DefaultAclUpdater()
		{
		}

		public void Update(WfAclItemCollection aclItems)
		{
			aclItems.NullCheck("aclItems");

			WfAclItemCollection existedAcl = GetAclFromAclResourceIDs(aclItems);

			StringBuilder strB = new StringBuilder();

			foreach (WfAclItem item in aclItems)
			{
				if (existedAcl.Exists(a => string.Compare(a.ResourceID, item.ResourceID, true) == 0 && string.Compare(a.ObjectID, item.ObjectID, true) == 0) == false)
				{
					if (strB.Length > 0)
						strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

					strB.Append(ORMapping.GetInsertSql(item, TSqlBuilder.Instance));
				}
			}

			if (strB.Length > 0)
				DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName());
		}

		/// <summary>
		/// 根据已经存在的acl中的resourceID，得到相关acl
		/// </summary>
		/// <param name="acl"></param>
		/// <returns></returns>
		private WfAclItemCollection GetAclFromAclResourceIDs(WfAclItemCollection acl)
		{
			Dictionary<string, string> resourceIDDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			foreach (WfAclItem item in acl)
				resourceIDDict[item.ResourceID] = item.ResourceID;

			WfAclItemCollection result = null;

			if (resourceIDDict.Count > 0)
			{
				result = Load(builder =>
				{
					builder.LogicOperator = LogicOperatorDefine.Or;

					foreach (KeyValuePair<string, string> kp in resourceIDDict)
						builder.AppendItem("RESOURCE_ID", kp.Key);
				});
			}
			else
				result = new WfAclItemCollection();

			return result;
		}

		protected override string GetConnectionName()
		{
			return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
		}
	}
}
