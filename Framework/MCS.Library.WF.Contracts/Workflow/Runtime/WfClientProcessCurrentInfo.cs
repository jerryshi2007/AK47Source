using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
    /// <summary>
    /// 流程的简要描述信息
    /// </summary>
    [DataContract]
    [Serializable]
    public class WfClientProcessCurrentInfo
    {
        private bool _Committed = true;

        public string InstanceID
        {
            get;
            set;
        }

        public string CurrentActivityID
        {
            get;
            set;
        }

        public string OwnerActivityID
        {
            get;
            set;
        }

        public string OwnerTemplateKey
        {
            get;
            set;
        }

        public int Sequence
        {
            get;
            set;
        }

        public string ResourceID
        {
            get;
            set;
        }

        public WfClientProcessStatus Status
        {
            get;
            set;
        }

        public string DescriptorKey
        {
            get;
            set;
        }

        public string ProcessName
        {
            get;
            set;
        }

        public string ApplicationName
        {
            get;
            set;
        }

        public string ProgramName
        {
            get;
            set;
        }

        public DateTime StartTime
        {
            get;
            set;
        }

        public DateTime EndTime
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public WfClientUser Creator
        {
            get;
            set;
        }

        public WfClientOrganization Department
        {
            get;
            set;
        }

        public int UpdateTag
        {
            get;
            set;
        }

        /// <summary>
        /// 流程是否是提交的。如果为False，则表示是用户打开表单，启动了流程，但是没有保存和流转
        /// </summary>
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
    }

    /// <summary>
    /// 流程数据信息列表
    /// </summary>
    [DataContract]
    [Serializable]
    public class WfClientProcessCurrentInfoCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfClientProcessCurrentInfo>
    {
        public WfClientProcessCurrentInfoCollection()
        {
        }

        protected WfClientProcessCurrentInfoCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected override string GetKeyForItem(WfClientProcessCurrentInfo item)
        {
            return item.InstanceID;
        }
    }
}
