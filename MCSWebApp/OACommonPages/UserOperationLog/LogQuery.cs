using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.Builder;
using MCS.Library.Data;
using System.Data;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.DataObjects;

namespace MCS.OA.CommonPages.UserOperationLog
{
	public class LogQuery : ObjectDataSourceQueryAdapterBase<MCS.Library.SOA.DataObjects.UserOperationLog, UserOperationLogCollection>
	{
		protected override string GetConnectionName()
		{
			return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(AppLogSettings.GetConfig().ConnectionName);
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.OrderByClause = "OPERATE_DATETIME Desc";
			qc.SelectFields = "*";
			qc.FromClause = "WF.USER_OPERATION_LOG (NOLOCK)";
			base.OnBuildQueryCondition(qc);
		}

		protected override void OnDataRowToObject(UserOperationLogCollection dataCollection, System.Data.DataRow row)
		{
			base.OnDataRowToObject(dataCollection, row);
		}

		protected override void OnAfterQuery(UserOperationLogCollection result)
		{
			UserOperationTasksLogCollection userTasklogs = this.GetOperationTasksLogs(result);

			Dictionary<int, UserOperationTasksLogCollection> dic = new Dictionary<int, UserOperationTasksLogCollection>();
			Dictionary<int, int> logIDCount = new Dictionary<int, int>();
			List<string> userIDs = new List<string>();

			foreach (UserOperationTasksLog item in userTasklogs)
			{
				if (dic.ContainsKey(item.ID) == false)
				{
					dic[item.ID] = new UserOperationTasksLogCollection();
					dic[item.ID].Add(item);

					userIDs.Add(item.SendToUserID);
					logIDCount.Add(item.ID, 0);
				}
				else if (dic[item.ID].Count < 5 && userIDs.Contains(item.SendToUserID) == false)
				{
					dic[item.ID].Add(item);
					userIDs.Add(item.SendToUserID);
				}

				logIDCount[item.ID]++;
			}
			userIDs = null;
			if (dic.Count > 0)
				this.SetTargetDescription(result, dic, logIDCount);
		}

		private void SetTargetDescription(UserOperationLogCollection result, Dictionary<int, UserOperationTasksLogCollection> dic, Dictionary<int, int> logIDCount)
		{
			foreach (MCS.Library.SOA.DataObjects.UserOperationLog setitem in result)
			{
				if (dic.ContainsKey(setitem.ID) == true)
				{
					StringBuilder strB = new StringBuilder();
					bool flag = true;
					foreach (UserOperationTasksLog item in dic[setitem.ID])
					{
						if (flag)
						{
							strB.Append(item.SendToUserName);
							flag = false;
						}
						else
							strB.AppendFormat(", {0}", item.SendToUserName);
					}

					if (logIDCount[setitem.ID] > 5)
						setitem.TargetDescription = string.Format("{0}......(共{1}个)", strB.ToString(), logIDCount[setitem.ID]);
					else
						setitem.TargetDescription = strB.ToString();
				}
			}
		}

		private UserOperationTasksLogCollection GetOperationTasksLogs(UserOperationLogCollection logs)
		{
			UserOperationTasksLogCollection taskLogs = null;

			if (logs.Count > 0)
			{
				taskLogs = UserOperationTasksLogAdapter.Instance.Load(builder =>
				{
					builder.LogicOperator = LogicOperatorDefine.Or;

					foreach (MCS.Library.SOA.DataObjects.UserOperationLog item in logs)
					{
						builder.AppendItem("LOG_ID", item.ID);
					}
				});
			}
			else
				taskLogs = new UserOperationTasksLogCollection();

			return taskLogs;
		}
	}
}