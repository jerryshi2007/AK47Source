using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;


namespace MCS.OA.Portal.Common
{
	public class UnCompleteListQuery : ObjectDataSourceQueryAdapterBase<ExtendedUserTask, ExtendedUserTaskCollection>
	{
		protected override string GetConnectionName()
		{
			return ConnectionDefine.DBConnectionName;
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.SelectFields = "U.*, P.PARAM_VALUE AS PROJECT_NAME";
			qc.FromClause = "WF.USER_TASK(NOLOCK) U LEFT JOIN WF.PROCESS_RELATIVE_PARAMS(NOLOCK) P ON U.PROCESS_ID = P.PROCESS_ID AND P.PARAM_KEY = 'ProjectName'";
			qc.OrderByClause = GetOrderByString(qc);

			base.OnBuildQueryCondition(qc);
		}

		protected string GetOrderByString(QueryCondition qc)
		{

			//排序规则为“置顶”>“用户指定”>“时间”
			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = "TOP_FLAG DESC,DELIVER_TIME DESC";
			else if (qc.OrderByClause.Contains("DELIVER_TIME"))
				qc.OrderByClause = string.Format("TOP_FLAG DESC,{0}", qc.OrderByClause);
			//09-01-12新需求
			//当点击按缓急程度排序后，优先显示缓急程度高的文件，同样缓急程度的文件，办理时限早的文件排在上面，同一办理时限的文件，发送时间早的文件排在上面。
			//else if (inputOrderBy.Contains("EMERGENCY"))
			//    inputOrderBy = string.Format("TOP_FLAG DESC,{0},ISNULL(EXPIRE_TIME, '2999-12-31') ASC,DELIVER_TIME ASC", inputOrderBy);
			else
				qc.OrderByClause = string.Format("TOP_FLAG DESC,{0},DELIVER_TIME DESC", qc.OrderByClause);

			return qc.OrderByClause;
		}
	}
}