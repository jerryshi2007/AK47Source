using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// 节点的状态
    /// </summary>
    public enum WfActivityStatus
    {
        /// <summary>
        /// 运行中
        /// </summary>
		[EnumItemDescription("运行中", Define.DefaultCulture)]
        Running,

        /// <summary>
        /// 被删除（用于撤回操作）
        /// </summary>
		[EnumItemDescription("被删除（用于撤回操作）", Define.DefaultCulture)]
        Deleted,

        /// <summary>
        /// 已完成
        /// </summary>
		[EnumItemDescription("已完成", Define.DefaultCulture)]
        Completed,

		/// <summary>
        /// 被终止
        /// </summary>
		[EnumItemDescription("被终止", Define.DefaultCulture)]
        Aborted,
    }

    /// <summary>
    /// 节点实例的接口定义
    /// </summary>
    public interface IWfActivity
    {
        #region properties

        /// <summary>
        /// 节点的ID
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// 节点的描述信息
        /// </summary>
        IWfActivityDescriptor Descriptor
        {
            get;
        }

        /// <summary>
        /// 节点的流程信息
        /// </summary>
        IWfProcess Process
        {
            get;
        }

        /// <summary>
        /// 节点状态
        /// </summary>
        WfActivityStatus Status
        {
            get;
        }

        /// <summary>
        /// 节点起始时间
        /// </summary>
        DateTime StartTime
        {
            get;
        }

        /// <summary>
        /// 节点结束时间
        /// </summary>
        DateTime EndTime
        {
            get;
        }

        bool IsAborted
        {
            get;
        }

        /// <summary>
        /// 进线
        /// </summary>
        IWfTransition FromTransition
        {
            get;
        }

        /// <summary>
        /// 出线
        /// </summary>
        IWfTransition ToTransition
        {
            get;
        }

        /// <summary>
        /// 节点的上下文
        /// </summary>
        WfActivityContext Context
        {
            get;
        }


		WfAssigneeCollection Assignees
        {
            get;
        }

		IUser Operator
		{
			get;
			set;
		}

		/// <summary>
		/// 是否是主流程的第一个点
		/// </summary>
		bool IsFirstActivity
		{
			get;
		}

		/// <summary>
		/// 是否是当前流程的第一个点
		/// </summary>
		bool IsCurrentProcessFirstActivity
		{
			get;
		}

		/// <summary>
		/// 是否是当前流程的最后一个点
		/// </summary>
		bool IsLastActivity
		{
			get;
		}

		/// <summary>
		/// 根Activity。如果是分支流程，则是它对应的根AnchorActivity，否则是它自己
		/// </summary>
		IWfActivity RootActivity
		{
			get;
		}

		/// <summary>
		/// 从哪条线定义产生的活动点。有可能为null，例如起始点。
		/// </summary>
		IWfTransitionDescriptor FromTransitionDescriptor
		{
			get;
		}

		DataLoadingType LoadingType
		{
			get;
		}
        #endregion

        #region methods

        bool AbleToMoveTo();

        #endregion
    }
}
