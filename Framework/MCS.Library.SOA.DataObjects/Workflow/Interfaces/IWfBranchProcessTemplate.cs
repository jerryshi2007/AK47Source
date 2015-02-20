using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 分支流程的模板定义
    /// </summary>
    public interface IWfBranchProcessTemplateDescriptor : IWfKeyedDescriptor
    {
        /// <summary>
        /// 分支流程起始点的操作人
        /// </summary>
        WfResourceDescriptorCollection Resources
        {
            get;
        }

        /// <summary>
        /// 分支流程描述的Key
        /// </summary>
        string BranchProcessKey
        {
            get;
        }

        /// <summary>
        /// 预定义好的流程描述。加载分支流程时，如果有预定义好的流程，则使用此流程描述，否则使用BranchProcessKey临时加载
        /// </summary>
        IWfProcessDescriptor PredefinedProcessDescriptor
        {
            get;
            set;
        }

        /// <summary>
        /// 分支流程的执行次序
        /// </summary>
        WfBranchProcessExecuteSequence ExecuteSequence
        {
            get;
        }

        /// <summary>
        /// 分支流程和主流程之间的阻塞关系
        /// </summary>
        WfBranchProcessBlockingType BlockingType
        {
            get;
        }

        /// <summary>
        /// 子流程审批流的确认模式
        /// </summary>
        WfSubProcessApprovalMode SubProcessApprovalMode
        {
            get;
            set;
        }

        /// <summary>
        /// 缺省的流程名称
        /// </summary>
        string DefaultProcessName
        {
            get;
        }

        /// <summary>
        /// 缺省的表单Url
        /// </summary>
        string DefaultUrl
        {
            get;
        }

        /// <summary>
        /// 缺省的待办标题
        /// </summary>
        string DefaultTaskTitle
        {
            get;
        }

        /// <summary>
        /// Clone一个Template
        /// </summary>
        /// <returns></returns>
        IWfBranchProcessTemplateDescriptor Clone();

        /// <summary>
        /// 得到分支流程的流程描述。如果有PredefinedProcessDescriptor，则返回它，否则根据BranchProcessKey加载流程，
        /// 如果BranchProcessKey为空，则根据Key加载流程
        /// </summary>
        /// <returns></returns>
        IWfProcessDescriptor GetBranchProcessDescriptor();

        /// <summary>
        /// 分支流程的启动条件
        /// </summary>
        WfConditionDescriptor Condition
        {
            get;
        }

        /// <summary>
        /// 结束子流程时的通知人
        /// </summary>
        WfResourceDescriptorCollection CancelSubProcessNotifier
        {
            get;
        }

        WfServiceOperationDefinition OperationDefinition
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
        /// 是否可以根据模板启动分支流程
        /// </summary>
        bool CanStart();

        /// <summary>
        /// 将所有包含originalUser的WfUserResourceDescriptor的资源替换为包含replaceUsers的一系列资源。
        /// 如果replaceUsers为null或者空集合，则相当于删除原始用户
        /// </summary>
        /// <param name="originalUser"></param>
        /// <param name="replaceUsers"></param>
        int ReplaceAllUserResourceDescriptors(IUser originalUser, IEnumerable<IUser> replaceUsers);
    }
}
