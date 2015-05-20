using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.DataObjects;

namespace MCS.Library.WF.Contracts.Operations
{
    [ServiceContract]
    public interface IWfClientProcessRuntimeService
    {
        /// <summary>
        /// 启动一个流程实例
        /// </summary>
        /// <param name="startupParams"></param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo StartWorkflow(WfClientProcessStartupParams clientStartupParams);

        /// <summary>
        /// 启动一个流程实例，并且流转到下一个环节。
        /// </summary>
        /// <param name="clientStartupParams">流程启动参数</param>
        /// <param name="clientTransferParams">流程下一步的流转参数，如果这个参数为空，则流转到默认环节</param>
        /// <param name="runtimeContext">流转上下文信息</param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo StartWorkflowAndMoveTo(WfClientProcessStartupParams clientStartupParams, WfClientTransferParams clientTransferParams, WfClientRuntimeContext runtimeContext);

        /// <summary>
        /// 流转到流程默认的下一个活动，活动的候选人就是待办人
        /// </summary>
        /// <param name="processID">流程的ID</param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo MoveToNextDefaultActivity(string processID, WfClientRuntimeContext runtimeContext);

        /// <summary>
        /// 流转到指定的流程活动
        /// </summary>
        /// <param name="processID">流程的ID</param>
        /// <param name="clientTransferParams">流转参数</param>
        /// <param name="runtimeContext">流转上下文信息</param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo MoveTo(string processID, WfClientTransferParams clientTransferParams, WfClientRuntimeContext runtimeContext);

        /// <summary>
        /// 更新流程的运行时参数，并且根据runtimeContext参数中的AutoCalculate决定是否自动计算
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="runtimeContext"></param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo UpdateRuntimeParameters(string processID, WfClientRuntimeContext runtimeContext);

        /// <summary>
        /// 保存流程的状态(包括附加的意见、参数等内容)
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="runtimeContext"></param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo SaveProcess(string processID, WfClientRuntimeContext runtimeContext);

        /// <summary>
        /// 流程撤回一步
        /// </summary>
        /// <param name="processID">流程的ID</param>
        /// <param name="runtimeContext"></param>
        /// <returns>流转上下文信息</returns>
        [OperationContract]
        WfClientProcessInfo Withdraw(string processID, WfClientRuntimeContext runtimeContext);

        /// <summary>
        /// 流程撤回一步，然后作废
        /// </summary>
        /// <param name="processID">流程的ID</param>
        /// <param name="runtimeContext"></param>
        /// <returns>流转上下文信息</returns>
        [OperationContract]
        WfClientProcessInfo WithdrawAndCancel(string processID, WfClientRuntimeContext runtimeContext);

        /// <summary>
        /// 作废流程
        /// </summary>
        /// <param name="processID">流程的ID</param>
        /// <param name="runtimeContext">流转上下文信息</param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo Cancel(string processID, WfClientRuntimeContext runtimeContext);

        /// <summary>
        /// 暂停流程
        /// </summary>
        /// <param name="processID">流程的ID</param>
        /// <param name="runtimeContext">流转上下文信息</param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo Pause(string processID, WfClientRuntimeContext runtimeContext);

        /// <summary>
        /// 恢复暂停的流程
        /// </summary>
        /// <param name="processID">流程的ID</param>
        /// <param name="runtimeContext">流转上下文信息</param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo Resume(string processID, WfClientRuntimeContext runtimeContext);

        /// <summary>
        /// 还原被作废的流程
        /// </summary>
        /// <param name="processID">流程的ID</param>
        /// <param name="runtimeContext">流转上下文信息</param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo Restore(string processID, WfClientRuntimeContext runtimeContext);

        /// <summary>
        /// 替换某个活动中的办理人，无论该活动的状态。如果这个人有待办，待办也会被替换。
        /// </summary>
        /// <param name="activityID">需要替换的活动的ID</param>
        /// <param name="originalUser">被替换的人。如果这个属性为null，则替换掉这个活动中所有的指派人和候选人</param>
        /// <param name="targetUsers">替换成的人，如果为null，则不完成替换</param>
        /// <param name="runtimeContext">流转上下文信息</param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo ReplaceAssignees(string activityID, WfClientUser originalUser, List<WfClientUser> targetUsers, WfClientRuntimeContext runtimeContext);

        /// <summary>
        /// 根据流程ID得到流程的当前摘要信息
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="user">当前用户，如果该参数不为空，则返回用户对流程的权限信息</param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo GetProcessInfoByID(string processID, WfClientUser user);

        /// <summary>
        /// 根据活动ID得到流程的当前摘要信息
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="user">当前用户，如果该参数不为空，则返回用户对流程的权限信息</param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessInfo GetProcessInfoByActivityID(string activityID, WfClientUser user);

        /// <summary>
        /// 根据ResourceID得到流程的当前摘要信息
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [OperationContract]
        List<WfClientProcessInfo> GetProcessesInfoByResourceID(string resourceID, WfClientUser user);

        /// <summary>
        /// 根据流程的ID获取流程信息
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="filter">获取不同部分的信息</param>
        /// <param name="user">当前用户，如果该参数不为空，则返回用户对流程的权限信息</param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcess GetProcessByID(string processID, WfClientUser user, WfClientProcessInfoFilter filter);

        /// <summary>
        /// 根据流程活动的ID获取流程信息
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="user">当前用户，如果该参数不为空，则返回用户对流程的权限信息</param>
        /// <param name="filter">获取不同部分的信息</param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcess GetProcessByActivityID(string activityID, WfClientUser user, WfClientProcessInfoFilter filter);

        /// <summary>
        /// 根据ResourceID获取相关的流程信息
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="user">当前用户，如果该参数不为空，则返回用户对流程的权限信息</param>
        /// <param name="filter">获取不同部分的信息</param>
        /// <returns></returns>
        [OperationContract]
        List<WfClientProcess> GetProcessesByResourceID(string resourceID, WfClientUser user, WfClientProcessInfoFilter filter);

        /// <summary>
        /// 根据ResourceID读取意见
        /// </summary>
        /// <param name="resourceID"></param>
        /// <returns></returns>
        [OperationContract]
        List<WfClientOpinion> GetOpinionsByResourceID(string resourceID);

        /// <summary>
        /// 根据ProcessID读取意见
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        [OperationContract]
        List<WfClientOpinion> GetOpinionsByProcessID(string processID);

        /// <summary>
        /// 获取流程中的应用上下文参数。返回的结果一定是字符串型，其它复合类型不支持
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="key"></param>
        /// <param name="probeMode"></param>
        /// <returns></returns>
        [OperationContract]
        string GetApplicationRuntimeParameters(string processID, string key, WfClientProbeApplicationRuntimeParameterMode probeMode);

        /// <summary>
        /// 分页查询某个活动下子流程的信息
        /// </summary>
        /// <param name="ownerActivityID">子流程的容器活动</param>
        /// <param name="ownerTemplateKey">容器活动中的分支流程模板，可以为空。为空表示所有子流程，否则是某个模板的子流程</param>
        /// <param name="startRowIndex">从0开始的起始行，相当于分页查询的每一页的起始行</param>
        /// <param name="maximumRows">返回的最大行，相当于分页查询每页的大小</param>
        /// <param name="orderBy">排序字段，允许为空，如果为空，则使用StartTime排序</param>
        /// <param name="totalCount">以前查询的总记录数，如果是第一次，则传入-1</param>
        /// <returns>分页查询结果，里面包含总行数和每一行的结果。其总行数在翻页时需要传入到totalCount参数中</returns>
        [OperationContract]
        WfClientProcessCurrentInfoPageQueryResult QueryBranchProcesses(string ownerActivityID, string ownerTemplateKey, int startRowIndex, int maximumRows, string orderBy, int totalCount);

        /// <summary>
        /// 按照流程的筛选条件，分页查询流程实例
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="startRowIndex">从0开始的起始行，相当于分页查询的每一页的起始行</param>
        /// <param name="maximumRows">返回的最大行，相当于分页查询每页的大小</param>
        /// <param name="orderBy">排序字段，允许为空，如果为空，则使用StartTime排序</param>
        /// <param name="totalCount">以前查询的总记录数，如果是第一次，则传入-1</param>
        /// <returns>分页查询结果，里面包含总行数和每一行的结果。其总行数在翻页时需要传入到totalCount参数中</returns>
        [OperationContract]
        WfClientProcessCurrentInfoPageQueryResult QueryProcesses(WfClientProcessQueryCondition condition, int startRowIndex, int maximumRows, string orderBy, int totalCount);
    }
}
