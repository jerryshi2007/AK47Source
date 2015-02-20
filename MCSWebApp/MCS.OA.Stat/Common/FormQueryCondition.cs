using System;
using System.Collections.Generic;
using System.Web;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Validation;

namespace SinoOcean.OA.Stat
{
    [Serializable]
    public class FormQueryCondition
    {
        private string applicationName;
        private string programName;
        private string _programName_SinoOcean;
        private string subject;
        private string currentUsersName;
        private string draftDepartmentName;
        private DateTime createTimeBegin;
        private DateTime createTimeEnd;

        /// <summary>
        /// 表单大类
        /// </summary>
        [ConditionMapping("APPLICATION_NAME", "=")]
        public string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        /// <summary>
        /// 表单小类
        /// </summary>
        //[ConditionMapping("PROGRAM_NAME", "=")]
		[ConditionMapping("AI.NAME", "=")]
        public string ProgramName
        {
            get { return programName; }
            set { programName = value; }
        }

        /// <summary>
        /// 表单小类-远洋地产
        /// </summary>
        [ConditionMapping("ACI.[PROGRAM_NAME]", "=")]
        public string ProgramName_SinoOcean {
            get { return _programName_SinoOcean; }
            set { _programName_SinoOcean = value; }
        }
        
        /// <summary>
        /// 标题
        /// </summary>
        [ConditionMapping("SUBJECT", "like")]
        [StringLengthValidator(0, 100, MessageTemplate = "查询标题必须少于100字")]
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        ///// <summary>
        ///// 处理人
        ///// </summary>
        //[ConditionMapping("CURRENT_USERS_NAME", "like")]
        //[StringLengthValidator(0, 20, MessageTemplate = "查询申请人必须少于20字")]
        //public string CurrentUsersName
        //{
        //    get { return currentUsersName; }
        //    set { currentUsersName = value; }
        //}

        /// <summary>
        /// 申请人
        /// </summary>
        [ConditionMapping("CREATOR_NAME", "like")]
        [StringLengthValidator(0, 20, MessageTemplate = "查询申请人必须少于20字")]
        public string CurrentUsersName
        {
            get { return currentUsersName; }
            set { currentUsersName = value; }
        }

        /// <summary>
        /// 申请人部门
        /// </summary>
        [ConditionMapping("DRAFT_DEPARTMENT_NAME", "like")]
        public string DraftDepartmentName
        {
            get { return draftDepartmentName; }
            set { draftDepartmentName = value; }
        }

        /// <summary>
        /// 起草时间
        /// </summary>
        [ConditionMapping("CREATE_TIME", ">=")]
        public DateTime CreateTimeBegin
        {
            get { return this.createTimeBegin; }
            set { this.createTimeBegin = value; }
        }

        [ConditionMapping("CREATE_TIME", "<")]
        public DateTime CreateTimeEnd
        {
            get { return this.createTimeEnd; }
            set { this.createTimeEnd = value; }
        }

    }
}
