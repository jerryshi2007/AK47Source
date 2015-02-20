using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    [Serializable]
    [ORTableMapping("SC.IncomeSynchronizeLogDetail")]
    public class SqlIncomeSyncLogDetail
    {
        [ORFieldMapping("LogDetailID", PrimaryKey = true)]
        public string LogDetailID { get; set; }
        public string LogID { get; set; }
        public string SCObjectID { get; set; }
        [SqlBehavior(ClauseBindingFlags.Select)]
        public DateTime CreateTime { get; set; }
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public SqlIncomeSyncLogDetailStatus ActionType { get; set; }
        public string Summary { get; set; }
        public string Detail { get; set; }
    }

    public enum SqlIncomeSyncLogDetailStatus
    {
        [EnumItemDescription("未定义")]
        Undefined = 0,
        [EnumItemDescription("新增")]
        Create = 1,
        [EnumItemDescription("修改")]
        Update = 2,
        [EnumItemDescription("删除")]
        Delete = 3
    }
}
