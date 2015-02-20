using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using System.ComponentModel;
using MCS.Library.Data.DataObjects;
using System.Data;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 用户自定义搜索条件
    /// </summary>
    [Serializable]
    [ORTableMapping("USER_CUSTOM_SEARCH_CONDITION")]
    public class UserCustomSearchCondition
    {
        private string _ID = string.Empty;
        private string _UserID = string.Empty;
        private string _ConditionName = string.Empty;
        private string _ConditionContent = string.Empty;
        private string _ConditionType = string.Empty;
        private DateTime _CreateTime;
        private string _ResouceID = string.Empty;

        /// <summary>
        /// ID
        /// </summary>
        [Description("ID")]
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Description("用户ID")]
        [ORFieldMapping("USER_ID")]
        public string UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        /// <summary>
        /// 搜索条件名称
        /// </summary>
        [Description("搜索条件名称")]
        [ORFieldMapping("CONDITION_NAME")]
        public string ConditionName
        {
            get { return _ConditionName; }
            set { _ConditionName = value; }
        }

        /// <summary>
        /// 搜索条件内容
        /// </summary>
        [Description("搜索条件内容")]
        [ORFieldMapping("CONDITION_CONTENT")]
        public string ConditionContent
        {
            get { return _ConditionContent; }
            set { _ConditionContent = value; }
        }

        /// <summary>
        /// 搜索条件类型
        /// </summary>
        [Description("搜索条件类型")]
        [ORFieldMapping("CONDITION_Type")]
        public string ConditiontType
        {
            get { return _ConditionType; }
            set { _ConditionType = value; }
        }

        /// <summary>
        /// 搜索条件ResourceID
        /// </summary>
        [Description("搜索条件ResourceID")]
        [ORFieldMapping("RESOURCE_ID")]
        public string ResourceID
        {
            get { return _ResouceID; }
            set { _ResouceID = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        [ORFieldMapping("CREATE_TIME")]
        public DateTime CreateTime
        {
            get { return _CreateTime; }
            set { _CreateTime = value; }
        }

    }

    [Serializable]
    public class UserCustomSearchConditionCollection : EditableDataObjectCollectionBase<UserCustomSearchCondition>
    {
    }
}
