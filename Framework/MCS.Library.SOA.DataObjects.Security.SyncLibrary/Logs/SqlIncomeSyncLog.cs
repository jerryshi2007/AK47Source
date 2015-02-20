using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using System.Diagnostics;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    [Serializable]
    [ORTableMapping("SC.IncomeSynchronizeLog")]
    public class SqlIncomeSyncLog
    {
        [ORFieldMapping("LogID", PrimaryKey = true)]
        public string LogID { get; set; }
        public string SourceName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string OperatorID { get; set; }
        public string OperatorName { get; set; }
        public int NumberOfExceptions { get; set; }
        [SqlBehavior(ClauseBindingFlags.Select)]
        public DateTime CreateTime { get; set; }
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public IncomeSyncStatus Status { get; set; }
        public int NumberOfModifiedItems { get; set; }

        /// <summary>
        /// 根据环境信息初始化Log。
        /// 初始化的属性包括：CreateTime , OperatorID, OperatorName
        /// </summary>
        /// <returns></returns>
        public static SqlIncomeSyncLog CreateLogFromEnvironment()
        {
            SqlIncomeSyncLog log = new SqlIncomeSyncLog();

            log.CreateTime = SCActionContext.Current.TimePoint;

            if (DeluxePrincipal.IsAuthenticated)
            {
                log.OperatorID =  DeluxeIdentity.CurrentUser.ID;
                log.OperatorName = DeluxeIdentity.CurrentUser.Name;
            }

            return log;
        }
    }


    public enum IncomeSyncStatus
    {
        [EnumItemDescription("未启动")]
        None = 0,
        [EnumItemDescription("运行中未终止")]
        Running = 1,
        [EnumItemDescription("操作结束")]
        Completed = 2,
        [EnumItemDescription("致命错误")]
        FaultError = 3,
    }
}
