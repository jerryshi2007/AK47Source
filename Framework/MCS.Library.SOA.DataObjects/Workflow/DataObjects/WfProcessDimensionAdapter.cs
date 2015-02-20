using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfProcessDimensionAdapter : UpdatableAndLoadableAdapterBase<WfProcessDimension, WfProcessDimensionCollection>
	{
		public static readonly WfProcessDimensionAdapter Instance = new WfProcessDimensionAdapter();

		private WfProcessDimensionAdapter()
		{
		}

		public WfProcessDimension Load(string processID)
		{
			processID.CheckStringIsNullOrEmpty("processID");

			return LoadByInBuilder(builder =>
				{
					builder.DataField = "PROCESS_ID";
					builder.AppendItem(processID);
				}).FirstOrDefault();
		}

		protected override void BeforeInnerUpdate(WfProcessDimension data, Dictionary<string, object> context)
		{
			data.UpdateTime = DateTime.MinValue;

			base.BeforeInnerUpdate(data, context);
		}

		/// <summary>
		/// 删除流程运行时活动节点的Assignees
		/// </summary>
		/// <param name="activityID"></param>
		public void Delete(WfProcessCurrentInfoCollection processesInfo)
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder("PROCESS_ID");

			processesInfo.ForEach(p => builder.AppendItem(p.CurrentActivityID));

			if (builder.Count > 0)
			{
				string sql = string.Format("DELETE {0} WHERE {1}",
					GetMappingInfo(new Dictionary<string, object>()).TableName,
					builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DbHelper.RunSql(sql, GetConnectionName());
			}
		}
	}
}
