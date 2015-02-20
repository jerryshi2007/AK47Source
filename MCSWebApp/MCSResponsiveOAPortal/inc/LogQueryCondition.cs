using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Mapping;

namespace MCSResponsiveOAPortal
{
    [Serializable]
    public class LogQueryCondition
    {
        [ConditionMapping("OPERATOR_ID", "IN", IsExpression = true)]
        public string Operator { get; set; }

        [ConditionMapping("APPLICATION_NAME")]
        public string ApplicationName { get; set; }

        [ConditionMapping("PROGRAM_NAME")]
        public string ProgramName { get; set; }

        [ConditionMapping("SUBJECT", "Like")]
        public string Title { get; set; }

        [ConditionMapping("OPERATE_DATETIME", ">=")]
        public DateTime StartDate { get; set; }

        [ConditionMapping("OPERATE_DATETIME", "<")]
        public DateTime EndDate { get; set; }

        [ConditionMapping("ACTIVITY_NAME", "Like")]
        public string ActivityName { get; set; }

        [ConditionMapping("RESOURCE_ID", "=")]
        public string ResourceID { get; set; }

        [ConditionMapping("PROCESS_ID", "=")]
        public string ProcessID { get; set; }

        private int _TagNull = 1;

        [ConditionMapping("1", "=")]
        public int TagNull
        {
            get { return this._TagNull; }
            set { this._TagNull = value; }
        }
    }
}