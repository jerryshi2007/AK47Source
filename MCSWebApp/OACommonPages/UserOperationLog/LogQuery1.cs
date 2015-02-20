using System;
using System.Collections.Generic;
using System.Web;
using MCS.Library.SOA.DataObjects;
using System.Data;
using MCS.Library.Data.Mapping;

namespace MCS.OA.CommonPages.UserOperationLog
{
    public class LogQuery1
    {
        public UserOperationLogCollection GetUserOperationLogsWithCount(int startRowIndex, int maximumRows, string where, string orderBy, ref int totalCount)
        {
            if (string.IsNullOrEmpty(orderBy))
                orderBy = "OPERATE_DATETIME Desc";

            QueryCondition qc = new QueryCondition(
                       startRowIndex,
                       maximumRows,
                       "*",
                       "WF.USER_OPERATION_LOG (NOLOCK)",
                       orderBy,
                       where
                       );

            DataSet ds = CommonAdapter.Instance.SplitPageQuery(qc, totalCount < 0);

            UserOperationLogCollection Logs = new UserOperationLogCollection();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                MCS.Library.SOA.DataObjects.UserOperationLog log = new MCS.Library.SOA.DataObjects.UserOperationLog();

                ORMapping.DataRowToObject(row, log);

                Logs.Add(log);
            }

            if (ds.Tables.Count > 1)
                totalCount = (int)ds.Tables[1].Rows[0][0];

            HttpContext.Current.Items["UserOperationLogsCount"] = totalCount;

            //当页码超出索引的，返回最大页
            if (Logs.Count == 0 && totalCount > 0)
            {
                int newStartRowIndex = (totalCount - 1) / maximumRows * maximumRows;

                totalCount = -1;

                Logs = GetUserOperationLogsWithCount(newStartRowIndex, maximumRows, where, orderBy, ref totalCount);
            }

            return Logs;
        }

        public int GetLogsCount(string where, ref int totalCount)
        {
            return (int)HttpContext.Current.Items["UserOperationLogsCount"];
        }





    }
}
