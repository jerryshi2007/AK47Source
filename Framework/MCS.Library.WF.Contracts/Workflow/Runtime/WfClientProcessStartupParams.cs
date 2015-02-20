using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.DataObjects;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
    [DataContract]
    [Serializable]
    public class WfClientProcessStartupParams
    {
        private bool _AutoCommit = false;
        private bool _AutoPersist = true;
        private bool _AutoStartInitialActivity = true;
        private bool _CheckStartProcessUserPermission = true;
        private Dictionary<string, object> _ApplicationRuntimeParameters = null;
        private Dictionary<string, object> _ProcessContext = null;
        private WfClientAssigneeCollection _Assignees = null;

        public WfClientProcessStartupParams()
        {
        }

        /// <summary>
        /// 需要启动的流程模板
        /// </summary>
        public string ProcessDescriptorKey
        {
            get;
            set;
        }

        /// <summary>
        /// 流程第一个活动的执行人
        /// </summary>
        public WfClientAssigneeCollection Assignees
        {
            get
            {
                return this._Assignees = this._Assignees ?? new WfClientAssigneeCollection();
            }
        }

        /// <summary>
        /// 流程中的上下文参数
        /// </summary>
        public Dictionary<string, object> ApplicationRuntimeParameters
        {
            get
            {
                return this._ApplicationRuntimeParameters =
                    this._ApplicationRuntimeParameters ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// 流程中的上下文参数
        /// </summary>
        public Dictionary<string, object> ProcessContext
        {
            get
            {
                return this._ProcessContext =
                    this._ProcessContext ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// 流程启动时设置流程Committed的属性
        /// </summary>
        public bool AutoCommit
        {
            get
            {
                return this._AutoCommit;
            }
            set
            {
                this._AutoCommit = value;
            }
        }

        /// <summary>
        /// 流程的创建者
        /// </summary>
        public WfClientUser Creator
        {
            get;
            set;
        }

        /// <summary>
        /// 流程的创建部门
        /// </summary>
        public WfClientOrganization Department
        {
            get;
            set;
        }

        /// <summary>
        /// 是否自动启动第一个活动
        /// </summary>
        public bool AutoStartInitialActivity
        {
            get
            {
                return this._AutoStartInitialActivity;
            }
            set
            {
                this._AutoStartInitialActivity = value;
            }
        }

        /// <summary>
        /// 流程关联的资源ID
        /// </summary>
        public string ResourceID
        {
            get;
            set;
        }

        /// <summary>
        /// 默认待办标题
        /// </summary>
        public string DefaultTaskTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 默认的Url
        /// </summary>
        public string DefaultUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 运行时的流程名称
        /// </summary>
        public string RuntimeProcessName
        {
            get;
            set;
        }

        /// <summary>
        /// 流程关联的其它流程ID
        /// </summary>
        public string RelativeID
        {
            get;
            set;
        }

        /// <summary>
        /// 流程关联的其它流程的Url
        /// </summary>
        public string RelativeURL
        {
            get;
            set;
        }

        /// <summary>
        /// 流程是否持久化（默认是True）
        /// </summary>
        public bool AutoPersist
        {
            get
            {
                return this._AutoPersist;
            }
            set
            {
                this._AutoPersist = value;
            }
        }

        /// <summary>
        /// 当前需要保存的意见
        /// </summary>
        public WfClientOpinion Opinion
        {
            get;
            set;
        }

        /// <summary>
        /// 是否检查启动流程的人的权限
        /// </summary>
        public bool CheckStartProcessUserPermission
        {
            get
            {
                return this._CheckStartProcessUserPermission;
            }
            set
            {
                this._CheckStartProcessUserPermission = value;
            }
        }
    }
}
