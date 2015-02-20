using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using System.Data.SqlClient;
using System.Data;
using MCS.Library.Data.Mapping;

namespace MCS.OA.Portal.Common
{
    public class GetLastUserTaskQuery
    {
        public static UserTask GetLatestUserTask(string userID)
        {
            string sql =
                string.Format(
                    "SELECT TOP 1 TASK_GUID, TASK_TITLE, DELIVER_TIME,APPLICATION_NAME, URL, STATUS FROM WF.USER_TASK (NOLOCK) WHERE SEND_TO_USER = {0} ORDER BY DELIVER_TIME DESC",
                    TSqlBuilder.Instance.CheckQuotationMark(userID, true));
            DataTable dataTable = null;
            DbHelper.RunSql(db => dataTable = db.ExecuteDataSet(CommandType.Text, sql).Tables[0], "HB2008");
            UserTask task = new UserTask();
            ORMappingItemCollection mapping = GetMappingInfo();
            foreach (DataRow row in dataTable.Rows)
            {
                ORMapping.DataRowToObject(row, mapping, task);
            }

            return task;
        }


        private static ORMappingItemCollection GetMappingInfo()
        {
            return ORMapping.GetMappingInfo<UserTask>();
        }

    }
}