using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Mapping;
using MCS.Library.Validation;

namespace MCS.OA.CommonPages.UserInfoExtend
{
    [Serializable]
    public class SearchUserQueryCondition
    {
        [ConditionMapping("Name", "like")]
        [StringLengthValidator(0, 100, MessageTemplate = "查询用户名称必须少于100字")]
        public string Name { get; set; }

        [ConditionMapping("DepartmentID", "=")]
        public string DepartmentId { get; set; }

    }
}