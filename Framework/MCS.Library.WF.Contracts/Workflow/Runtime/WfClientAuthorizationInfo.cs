using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
    /// <summary>
    /// 针对于某一位用户，某一个具体流程的授权信息
    /// </summary>
    [DataContract]
    [Serializable]
    public class WfClientAuthorizationInfo
    {
        /// <summary>
        /// 原始的活动ID
        /// </summary>
        public string OriginalActivityID
        {
            get;
            set;
        }

        /// <summary>
        /// 用户的ID
        /// </summary>
        public string UserID
        {
            get;
            set;
        }

        /// <summary>
        /// 是否具有读取流程信息的权限
        /// </summary>
        public bool IsProcessViewer
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是当前流程的管理员
        /// </summary>
        public bool IsProcessAdmin
        {
            get;
            set;
        }

        /// <summary>
        /// 当前人员是否在流转状态
        /// </summary>
        public bool InMoveToMode
        {
            get;
            set;
        }

        /// <summary>
        /// 当前流程是否在可流转的状态（与人无关）
        /// </summary>
        public bool InMoveToStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 当期人员是否在流程的Acl中
        /// </summary>
        public bool IsInAcl
        {
            get;
            set;
        }
    }
}
