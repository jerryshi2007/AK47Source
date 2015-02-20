using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace MCSResponsiveOAPortal
{
    public sealed class TaskQuery
    {
        public static readonly TaskQuery Instance = new TaskQuery();

        public TaskQuery()
        {

        }

        public UserTask GetLatestUserTask(string userID)
        {
            string sql =
                string.Format(
                    "SELECT TOP 1 TASK_GUID, TASK_TITLE, DELIVER_TIME,APPLICATION_NAME, URL, STATUS FROM WF.USER_TASK (NOLOCK) WHERE SEND_TO_USER = {0} ORDER BY DELIVER_TIME DESC",
                    TSqlBuilder.Instance.CheckQuotationMark(userID, true));

            UserTask task = null;

            using (var context = DbContext.GetContext("HB2008"))
            {
                using (var dr = DbHelper.RunSqlReturnDR(sql))
                {
                    if (dr.Read())
                    {
                        task = new UserTask();
                        ORMapping.DataReaderToObject(dr, GetMapptingInfo(), task);
                    }
                }
            }

            return task;

        }

        private static ORMappingItemCollection GetMapptingInfo()
        {
            return ORMapping.GetMappingInfo<UserTask>();
        }
    }
}