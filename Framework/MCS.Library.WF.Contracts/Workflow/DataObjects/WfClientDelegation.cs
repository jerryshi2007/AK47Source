using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.WF.Contracts.Workflow.DataObjects
{
    [DataContract]
    [Serializable]
    public class WfClientDelegation
    {

        #region 服务端属性
        /// <summary>
        /// 是否启用
        /// </summary>         
        public bool Enabled
        {
            set;
            get;
        }

        /// <summary>
        /// 委托人的用户ID
        /// </summary>        
        public string SourceUserID
        {
            get;
            set;
        }

        /// <summary>
        /// 被委托人的用户ID
        /// </summary>        
        public string DestinationUserID
        {
            get;
            set;
        }

        /// <summary>
        /// 委托人的用户名称
        /// </summary>       
        public string SourceUserName
        {
            get;
            set;
        }

        /// <summary>
        /// 被委托人的用户名称
        /// </summary>       
        public string DestinationUserName
        {
            get;
            set;
        }

        /// <summary>
        /// 开始时间
        /// </summary>        
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 结束时间
        /// </summary>       
        public DateTime EndTime
        {
            get;
            set;
        }
        #endregion

        #region 客户端扩展
        /// <summary>
        /// 流程的应用名称
        /// </summary>
        public string ApplicationName
        {
            get;
            set;
        }

        /// <summary>
        /// 流程的模块名称
        /// </summary>
        public string ProgramName
        {
            get;
            set;
        }

        /// <summary>
        /// 租户的ID
        /// </summary>
        public string TenantCode
        {
            get;
            set;
        }
        #endregion
    }

    [DataContract]
    [Serializable]
    public class WfClientDelegationCollection : EditableDataObjectCollectionBase<WfClientDelegation>
    {
        public WfClientDelegationCollection()
        {
        }

        public WfClientDelegationCollection(IEnumerable<WfClientDelegation> source)
        {
            this.CopyFrom(source);
        }
    }
}
