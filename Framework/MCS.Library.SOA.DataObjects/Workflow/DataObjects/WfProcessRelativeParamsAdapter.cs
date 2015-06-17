using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程相关参数的保存
	/// </summary>
	public class WfProcessRelativeParamsAdapter : UpdatableAndLoadableAdapterBase<WfProcessRelativeParam, WfProcessRelativeParamCollection>
	{
		public readonly static WfProcessRelativeParamsAdapter Instance = new WfProcessRelativeParamsAdapter();

		private WfProcessRelativeParamsAdapter()
		{
		}

		public WfProcessRelativeParamCollection Load(string processID)
		{
			return LoadByInBuilder(b =>
			{
				b.DataField = "PROCESS_ID";
				b.AppendItem(processID);
			});
		}

		public void Update(IWfProcess process)
		{
			process.NullCheck("process");

			Update(process.ID, ProcessToRelativeParams(process));
		}

		public void Update(string processID, WfProcessRelativeParamCollection relativeParams)
		{
			processID.CheckStringIsNullOrEmpty("processID");

            WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

            wBuilder.AppendItem("PROCESS_ID", processID);
            wBuilder.AppendTenantCodeSqlClause(typeof(WfProcessCurrentActivity));

			StringBuilder strB = new StringBuilder();

			strB.AppendFormat("DELETE WF.PROCESS_RELATIVE_PARAMS WHERE {0}",
                wBuilder.ToSqlString(TSqlBuilder.Instance));

			foreach (WfProcessRelativeParam rp in relativeParams)
			{
				if (strB.Length > 0)
					strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

				strB.Append(ORMapping.GetInsertSql(rp, TSqlBuilder.Instance));
			}

			DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName());
		}

		private static WfProcessRelativeParamCollection ProcessToRelativeParams(IWfProcess process)
		{
			WfProcessRelativeParamCollection relativeParams = new WfProcessRelativeParamCollection();

			foreach (string key in process.RelativeParams.AllKeys)
			{
				WfProcessRelativeParam rp = new WfProcessRelativeParam();

				rp.ProcessID = process.ID;
				rp.ParamKey = key;
				rp.ParamValue = process.RelativeParams[key];

				relativeParams.Add(rp);
			}

			return relativeParams;
		}

		protected override string GetConnectionName()
		{
			return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
		}
	}
}
