using System;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Validation;

namespace MCS.OA.Portal.Common
{
    [Serializable]
    public class TaskQueryCondition
    {
        private string taskTitle = string.Empty;
        private DateTime taskStartTime = DateTime.MinValue;
        private DateTime deliverTimeBegin = DateTime.MinValue;
        private DateTime deliverTimeEnd = DateTime.MinValue;
        private string applicationName = string.Empty;
        private string sourceID = string.Empty;
        private string purpose = string.Empty;
        private DateTime expireTimeBegin = DateTime.MinValue;
        private DateTime expireTimeEnd = DateTime.MinValue;
        private string categoryID = string.Empty;
        private DateTime completedTimeBegin = DateTime.MinValue;
        private DateTime completedTimeEnd = DateTime.MinValue;
        private string userID = string.Empty;
        private string draftDepartmentName = string.Empty;
        private string programName = string.Empty;
        private string resourceID = string.Empty;
        private string draftUserID = string.Empty;
        private string draftUserName = string.Empty;

        [ConditionMapping("TASK_START_TIME")]
        public DateTime TaskStartTime
        {
            get { return taskStartTime; }
            set { taskStartTime = value; }
        }

        [ConditionMapping("DRAFT_USER_NAME")]
        public string DraftUserName
        {
            get { return draftUserName; }
            set { draftUserName = value; }
        }

        [ConditionMapping("DRAFT_USER_ID")]
        public string DraftUserID
        {
            get { return draftUserID; }
            set { draftUserID = value; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        [ConditionMapping("RESOURCE_ID")]
        public string ResourceID
        {
            get { return resourceID; }
            set { draftUserID = value; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        [ConditionMapping("PROGRAM_NAME")]
        public string ProgramName
        {
            get { return programName; }
            set { programName = value; }
        }


        /// <summary>
        /// ����
        /// </summary>
        [ConditionMapping("TASK_TITLE", "like")]
        [StringLengthValidator(0, 100, MessageTemplate = "��ѯ�����������100��")]
        public string TaskTitle
        {
            get { return taskTitle; }
            set { taskTitle = value; }
        }


        /// <summary>
        /// ��ʼʱ��
        /// </summary>
        [ConditionMapping("DELIVER_TIME", ">=")]
        public DateTime DeliverTimeBegin
        {
            get { return deliverTimeBegin; }
            set { deliverTimeBegin = value; }
        }


        [ConditionMapping("DELIVER_TIME", "<", AdjustDays = 1)]
        public DateTime DeliverTimeEnd
        {
            get { return deliverTimeEnd; }
            set { deliverTimeEnd = value; }
        }


        /// <summary>
        /// �����̶�
        /// </summary>
        //[ConditionMapping("EMERGENCY")]
        //public SinoOceanFileEmergency Emergency
        //{
        //    get { return this.emergency; }
        //    set { this.emergency = value; }
        //}

        [ConditionMapping("DRAFT_DEPARTMENT_NAME", IsExpression = true)]
        public string DraftDepartmentName
        {
            get { return draftDepartmentName; }
            set { draftDepartmentName = value; }
        }


        /// <summary>
        /// Ӧ������
        /// </summary>
        [ConditionMapping("APPLICATION_NAME", "=")]
        public string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }


        /// <summary>
        /// ������ID
        /// </summary>
        [ConditionMapping("SOURCE_ID", "IN", IsExpression = true)]
        public string SourceID
        {
            get { return sourceID; }
            set { sourceID = value; }
        }

        /// <summary>
        /// Ŀ��
        /// </summary>
        [ConditionMapping("PURPOSE", "like")]
        [StringLengthValidator(0, 30, MessageTemplate = "��ѯĿ�ı�������30��")]
        public string Purpose
        {
            get { return purpose; }
            set { purpose = value; }
        }


        /// <summary>
        /// ��������
        /// </summary>
        [ConditionMapping("EXPIRE_TIME", ">=")]
        public DateTime ExpireTimeBegin
        {
            get { return expireTimeBegin; }
            set { expireTimeBegin = value; }
        }


        [ConditionMapping("EXPIRE_TIME", "<")]
        public DateTime ExpireTimeEnd
        {
            get { return expireTimeEnd; }
            set { expireTimeEnd = value; }
        }


        /// <summary>
        /// ���ID
        /// </summary>
        [ConditionMapping("CATEGORY_GUID")]
        public string CategoryID
        {
            get { return categoryID; }
            set { categoryID = value; }
        }


        /// <summary>
        /// ���ʱ��
        /// </summary>
        [ConditionMapping("COMPLETED_TIME", ">=")]
        public DateTime CompletedTimeBegin
        {
            get { return completedTimeBegin; }
            set { completedTimeBegin = value; }
        }


        [ConditionMapping("COMPLETED_TIME", "<")]
        public DateTime CompletedTimeEnd
        {
            get { return completedTimeEnd; }
            set { completedTimeEnd = value; }
        }


        /// <summary>
        /// ������ID
        /// </summary>
        [ConditionMapping("SEND_TO_USER")]
        public string UserID
        {
            get { return this.userID; }
            set { this.userID = value; }
        }
    }
}
