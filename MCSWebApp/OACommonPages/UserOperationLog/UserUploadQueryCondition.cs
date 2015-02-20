using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Mapping;

namespace MCS.OA.CommonPages.UserOperationLog
{
    [Serializable]
    public class UserUploadQueryCondition
    {
        [ConditionMapping("OPERATOR_ID")]
        public string Operator { get; set; }

        [ConditionMapping("APPLICATION_NAME", EscapeLikeString = true, Prefix = "%", Postfix = "%", Operation = "LIKE")]
        public string ApplicationName { get; set; }

        [ConditionMapping("PROGRAM_NAME", EscapeLikeString = true, Prefix = "%", Postfix = "%", Operation = "LIKE")]
        public string ProgramName { get; set; }

        [ConditionMapping("STATUS")]
        public int Status { get; set; }

        [ConditionMapping("CREATE_TIME", ">=")]
        public DateTime StartDate { get; set; }

        [ConditionMapping("CREATE_TIME", "<", AdjustDays = 1)]
        public DateTime EndDate { get; set; }
    }
}