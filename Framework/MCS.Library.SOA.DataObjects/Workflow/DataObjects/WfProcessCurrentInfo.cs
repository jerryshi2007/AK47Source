using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using System.Data;
using MCS.Library.OGUPermission;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程数据简体版。即不包含WfProcessDescriptorInfo流程数据的DATA
    /// </summary>
    [Serializable]
    [XElementSerializable]
    [ORTableMapping("WF.PROCESS_INSTANCES")]
    public class WfProcessCurrentInfo
    {
        private bool _Committed = true;

        [ORFieldMapping("INSTANCE_ID", PrimaryKey = true)]
        public string InstanceID
        {
            get;
            set;
        }

        [ORFieldMapping("CURRENT_ACTIVITY_ID")]
        public string CurrentActivityID
        {
            get;
            set;
        }

        [ORFieldMapping("OWNER_ACTIVITY_ID")]
        public string OwnerActivityID
        {
            get;
            set;
        }

        [ORFieldMapping("OWNER_TEMPLATE_KEY")]
        public string OwnerTemplateKey
        {
            get;
            set;
        }

        [ORFieldMapping("SEQUENCE")]
        public int Sequence
        {
            get;
            set;
        }

        [ORFieldMapping("RESOURCE_ID")]
        public string ResourceID
        {
            get;
            set;
        }

        [ORFieldMapping("STATUS")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumString)]
        public WfProcessStatus Status
        {
            get;
            set;
        }

        [ORFieldMapping("DESCRIPTOR_KEY")]
        public string DescriptorKey
        {
            get;
            set;
        }

        [ORFieldMapping("PROCESS_NAME")]
        public string ProcessName
        {
            get;
            set;
        }

        [ORFieldMapping("APPLICATION_NAME")]
        public string ApplicationName
        {
            get;
            set;
        }

        [ORFieldMapping("PROGRAM_NAME")]
        public string ProgramName
        {
            get;
            set;
        }

        [ORFieldMapping("START_TIME")]
        public DateTime StartTime
        {
            get;
            set;
        }

        [ORFieldMapping("END_TIME")]
        public DateTime EndTime
        {
            get;
            set;
        }

        [ORFieldMapping("CREATE_TIME")]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
        public DateTime CreateTime
        {
            get;
            set;
        }

        private IUser _Creator = null;

        [SubClassORFieldMapping("ID", "CREATOR_ID")]
        [SubClassSqlBehavior("ID", ClauseBindingFlags.Insert | ClauseBindingFlags.Select)]
        [SubClassORFieldMapping("DisplayName", "CREATOR_NAME")]
        [SubClassSqlBehavior("DisplayName", ClauseBindingFlags.Insert | ClauseBindingFlags.Select)]
        [SubClassORFieldMapping("FullPath", "CREATOR_PATH")]
        [SubClassSqlBehavior("FullPath", ClauseBindingFlags.Insert | ClauseBindingFlags.Select)]
        [SubClassType(typeof(OguUser))]
        public IUser Creator
        {
            get
            {
                return this._Creator;
            }
            set
            {
                this._Creator = (IUser)OguUser.CreateWrapperObject(value);
            }
        }

        private IOrganization _Department = null;

        [SubClassORFieldMapping("ID", "DEPARTMENT_ID")]
        [SubClassSqlBehavior("ID", ClauseBindingFlags.Insert | ClauseBindingFlags.Select)]
        [SubClassORFieldMapping("DisplayName", "DEPARTMENT_NAME")]
        [SubClassSqlBehavior("DisplayName", ClauseBindingFlags.Insert | ClauseBindingFlags.Select)]
        [SubClassORFieldMapping("FullPath", "DEPARTMENT_PATH")]
        [SubClassSqlBehavior("FullPath", ClauseBindingFlags.Insert | ClauseBindingFlags.Select)]
        [SubClassType(typeof(OguOrganization))]
        public IOrganization Department
        {
            get
            {
                return this._Department;
            }
            set
            {
                this._Department = value;
            }
        }

        [ORFieldMapping("UPDATE_TAG")]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
        public int UpdateTag
        {
            get;
            set;
        }

        /// <summary>
        /// 流程是否是提交的。如果为False，则表示是用户打开表单，启动了流程，但是没有保存和流转
        /// </summary>
        [ORFieldMapping("COMMITTED")]
        public bool Committed
        {
            get
            {
                return this._Committed;
            }
            set
            {
                this._Committed = value;
            }
        }

        private WfAssigneeCollection _Assignees;

        [NoMapping]
        public WfAssigneeCollection Assignees
        {
            get
            {
                if (this._Assignees == null)
                    this._Assignees = new WfAssigneeCollection();

                return this._Assignees;
            }
        }
    }

    /// <summary>
    /// 流程数据信息列表
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class WfProcessCurrentInfoCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfProcessCurrentInfo>
    {
        public WfProcessCurrentInfoCollection()
        {
        }

        protected WfProcessCurrentInfoCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        internal void LoadFromDataView(DataView dv)
        {
            this.Clear();

            ORMapping.DataViewToCollection(this, dv);
        }

        protected override string GetKeyForItem(WfProcessCurrentInfo item)
        {
            return item.InstanceID;
        }
    }
}
