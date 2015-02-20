using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// 流程的状态
    /// </summary>
    public enum WfProcessStatus
    {
        /// <summary>
        /// 运行中
        /// </summary>
        [EnumItemDescription("运行中", Define.DefaultCulture)]
        Running,

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

        /// <summary>
        /// 未运行
        /// </summary>
		[EnumItemDescription("未运行", Define.DefaultCulture)]
        NotRunning
    }

	/// <summary>
	/// 对象的加载方式
	/// </summary>
	public enum DataLoadingType
	{
		/// <summary>
		/// 对象的内容从内存中加载（new）
		/// </summary>
		Memory,

		/// <summary>
		/// 对象的内容从外部加载（数据库）
		/// </summary>
		External
	}

    /// <summary>
    /// 流程实例的接口定义
    /// </summary>
    public interface IWfProcess
    {
        /// <summary>
        /// 流程的ID
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        string ResourceID
        {
            get;
            set;
        }
        
        #region 流程状态信息
        /// <summary>
        /// 流程的状态
        /// </summary>
        WfProcessStatus Status
        {
            get;
        }

        /// <summary>
        /// 流程启动时间
        /// </summary>
        DateTime StartTime
        {
            get;
        }

        /// <summary>
        /// 流程结束信息
        /// </summary>
        DateTime EndTime
        {
            get;
        }
        #endregion 流程状态信息

        #region 节点信息
        /// <summary>
        /// 第一个节点实例
        /// </summary>
        IWfActivity FirstActivity
        {
            get;
        }

        /// <summary>
        /// 最后一个节点实例
        /// </summary>
        IWfActivity LastActivity
        {
            get;
        }

        /// <summary>
        /// 当前的活动节点
        /// </summary>
        IWfActivity CurrentActivity
        {
            get;
        }

        /// <summary>
        /// 流程中节点的实例
        /// </summary>
        WfActivityCollection Activities
        {
            get;
        }
        #endregion 节点信息

        #region 其它信息
		/// <summary>
		/// 根流程信息
		/// </summary>
		IWfProcess RootProcess
		{
			get;
		}

		/// <summary>
		/// 创建实例的工厂
		/// </summary>
		IWfFactory Factory
		{
			get;
		}

        /// <summary>
        /// 
        /// </summary>
        WfBranchProcessInfo EntryInfo
        {
            get;
        }

        /// <summary>
        /// 操作人
        /// </summary>
		IUser Creator
		{
			get;
			set;
		}

		/// <summary>
		/// 流程的部门
		/// </summary>
		IOrganization OwnerDepartment
		{
			get;
			set;
		}

        /// <summary>
        /// 流程上下文
        /// </summary>
        WfProcessContext Context
        {
            get;
        }

		/// <summary>
		/// 流程的加载方式
		/// </summary>
		DataLoadingType LoadingType
		{
			get;
		}

        /// <summary>
        /// 节点流转
        /// </summary>
        /// <param name="transferParams"></param>
        /// <returns></returns>
        IWfActivity MoveTo(WfTransferParamsBase transferParams);

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="destinationActivityID"></param>
		void Withdraw(IWfActivity destinationActivity);

		/// <summary>
		/// 取消流程
		/// </summary>
		void CancelProcess();

		/// <summary>
		/// 得到所有环节信息
		/// </summary>
		/// <param name="autoCalcaulatePath"></param>
		/// <returns></returns>
		WfActivityLevelGroupCollection GetAllLevels(bool autoCalcaulatePath);
        #endregion 其它信息
    }
}
