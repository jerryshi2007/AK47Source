using System;
using System.Collections.Generic;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow.Builders;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程定义的接口定义
    /// </summary>
    public interface IWfProcessDescriptor : IWfKeyedDescriptor
    {
        /// <summary>
        /// 应用的名称
        /// </summary>
        string ApplicationName
        {
            get;
            set;
        }

        /// <summary>
        /// 应用的模块名称
        /// </summary>
        string ProgramName
        {
            get;
            set;
        }

        /// <summary>
        /// 流程启动或表单缺省的Url
        /// </summary>
        string Url
        {
            get;
            set;
        }

        /// <summary>
        /// 和流程图相关的描述信息，例如坐标等
        /// </summary>
        string GraphDescription
        {
            get;
        }

        /// <summary>
        /// 版本信息
        /// </summary>
        string Version
        {
            get;
            set;
        }

        /// <summary>
        /// 默认的待办标题
        /// </summary>
        string DefaultTaskTitle
        {
            get;
            set;
        }

        string DefaultNotifyTaskTitle
        {
            get;
            set;
        }

        bool DefaultReturnValue
        {
            get;
            set;
        }

        /// <summary>
        /// 流程的类型
        /// </summary>
        WfProcessType ProcessType
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        WfVariableDescriptorCollection Variables
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        WfActivityDescriptorCollection Activities
        {
            get;
        }

        /// <summary>
        /// 相关链接
        /// </summary>
        WfRelativeLinkDescriptorCollection RelativeLinks
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IWfActivityDescriptor InitialActivity
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IWfActivityDescriptor CompletedActivity
        {
            get;
        }

        /// <summary>
        /// 内部相关人员
        /// </summary>
        WfResourceDescriptorCollection InternalRelativeUsers
        {
            get;
        }

        /// <summary>
        /// 流和自动收集参数集合
        /// </summary>
        WfParameterNeedToBeCollected ParametersNeedToBeCollected
        {
            get;
        }

        /// <summary>
        /// 撤销前调用服务
        /// </summary>
        WfServiceOperationDefinitionCollection CancelBeforeExecuteServices
        {
            get;
        }

        /// <summary>
        /// 撤销后调用服务
        /// </summary>
        WfServiceOperationDefinitionCollection CancelAfterExecuteServices
        {
            get;
        }

        /// <summary>
        /// 撤销流程时调用服务的Key，逗号分隔
        /// </summary>
        string CancelExecuteServiceKeys
        {
            get;
        }

        /// <summary>
        /// 外部人员
        /// </summary>
        WfExternalUserCollection ExternalUsers
        {
            get;
        }

        /// <summary>
        /// 流程启动时是否自动生成资源中的人员
        /// </summary>
        bool AutoGenerateResourceUsers
        {
            get;
        }

        /// <summary>
        /// 自动计算出一个没有用过的ActivityKey
        /// </summary>
        /// <returns></returns>
        string FindNotUsedActivityKey();

        /// <summary>
        /// 自动计算出一个没有用过的ActivityKey
        /// </summary>
        /// <returns></returns>
        string FindNotUsedActivityKey(string Prefix);

        /// <summary>
        /// 自动计算出一个没有用过的TransitionKey
        /// </summary>
        /// <returns></returns>
        string FindNotUsedTransitionKey();

        /// <summary>
        /// 根据Key查找Transition，如果没有找到，返回null;
        /// </summary>
        /// <param name="transitionKey"></param>
        /// <returns></returns>
        IWfTransitionDescriptor FindTransitionByKey(string transitionKey);

        /// <summary>
        /// 作废流程时需要通知的人
        /// </summary>
        WfResourceDescriptorCollection CancelEventReceivers
        {
            get;
        }

        /// <summary>
        /// 办结流程时需要通知的人
        /// </summary>
        WfResourceDescriptorCollection CompleteEventReceivers
        {
            get;
        }

		/// <summary>
		/// 允许启动流程的人
		/// </summary>
		WfResourceDescriptorCollection ProcessStarters
		{
			get;
		}

        /// <summary>
        /// 得到主流活动点
        /// </summary>
        /// <returns></returns>
        WfMainStreamActivityDescriptorCollection GetMainStreamActivities();

        /// <summary>
        /// 是否是流程实例中的主线流程定义
        /// </summary>
        bool IsMainStream
        {
            get;
        }

        /// <summary>
        /// 克隆一个当前流程描述
        /// </summary>
        /// <returns></returns>
        IWfProcessDescriptor Clone();

        /// <summary>
        /// 是否根据创建活动的参数来创建活动
        /// </summary>
        /// <param name="capc"></param>
        /// <param name="overrideInitActivity">参数中的第一个点是否与起始点合并</param>
        void CreateActivities(WfCreateActivityParamCollection capc, bool overrideInitActivity);

        /// <summary>
        /// 遍历所有的活动
        /// </summary>
        /// <param name="activityFunc">每一个活动的回调，如果返回False，则中止遍历</param>
        /// <param name="transitionFunc">每一条线的回调，如果返回False，则忽略该线</param>
		void ProbeAllActivities(Func<WfProbeActivityArgs, bool> activityFunc, Func<IWfTransitionDescriptor, bool> transitionFunc);

		/// <summary>
		/// 将所有包含originalUser的WfUserResourceDescriptor的资源替换为包含replaceUsers的一系列资源。
		/// 如果replaceUsers为null或者空集合，则相当于删除原始用户
		/// </summary>
		/// <param name="originalUser"></param>
		/// <param name="replaceUsers"></param>
		/// <returns>被替换的个数</returns>
		int ReplaceAllUserResourceDescriptors(IUser originalUser, IEnumerable<IUser> replaceUsers);
    }
}
