using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow.Builders;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程活动描述的接口定义
    /// </summary>
    public interface IWfActivityDescriptor : IWfKeyedDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        string CodeName
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        WfActivityType ActivityType
        {
            get;
        }

        /// <summary>
        /// 活动的Url
        /// </summary>
        string Url
        {
            get;
        }

        /// <summary>
        /// 环节的名称
        /// </summary>
        string LevelName
        {
            get;
        }

        /// <summary>
        /// 场景的名称
        /// </summary>
        string Scene
        {
            get;
        }

        /// <summary>
        /// 只读场景的名称
        /// </summary>
        string ReadOnlyScene
        {
            get;
        }

        /// <summary>
        /// 继承下来的场景。如果当前流程场景为空，则检查父流程
        /// </summary>
        string InheritedScene
        {
            get;
        }

        /// <summary>
        /// 继承下来的只读场景。如果当前流程场景为空，则检查父流程
        /// </summary>
        string InheritedReadOnlyScene
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IWfProcessDescriptor Process
        {
            get;
        }

        /// <summary>
        /// 预计开始时间
        /// </summary>
        DateTime EstimateStartTime
        {
            get;
        }

        /// <summary>
        /// 预计完成时间
        /// </summary>
        DateTime EstimateEndTime
        {
            get;
        }

        /// <summary>
        /// 预计进行时间（小时）
        /// </summary>
        Decimal EstimateDuration
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        FromTransitionsDescriptorCollection FromTransitions
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        ToTransitionsDescriptorCollection ToTransitions
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
        /// 
        /// </summary>
        /// <param name="activityDespKey"></param>
        /// <returns></returns>
        bool CanReachTo(IWfActivityDescriptor targetAct);

        /// <summary>
        /// 
        /// </summary>
        WfVariableDescriptorCollection Variables
        {
            get;
        }

        WfResourceDescriptorCollection Resources
        {
            get;
        }

        /// <summary>
        /// 活动能够继续进行的条件
        /// </summary>
        WfConditionDescriptor Condition
        {
            get;
        }

        /// <summary>
        /// 分支流程的模板
        /// </summary>
        WfBranchProcessTemplateCollection BranchProcessTemplates
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
        /// 进入活动时的被通知人
        /// </summary>
        WfResourceDescriptorCollection EnterEventReceivers
        {
            get;
        }

        /// <summary>
        /// 离开活动时的被通知人
        /// </summary>
        WfResourceDescriptorCollection LeaveEventReceivers
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
        /// 外部人员
        /// </summary>
        WfExternalUserCollection ExternalUsers
        {
            get;
        }

        /// <summary>
        /// 进入活动时调用的WebService
        /// </summary>
        WfServiceOperationDefinitionCollection EnterEventExecuteServices
        {
            get;
        }

        /// <summary>
        /// 离开活动时调用的WebService
        /// </summary>
        WfServiceOperationDefinitionCollection LeaveEventExecuteServices
        {
            get;
        }

        /// <summary>
        /// 进入活动时调用的WebService的Key
        /// </summary>
        string EnterEventExecuteServiceKeys
        {
            get;
        }

        /// <summary>
        /// 离开活动时调用的WebService的Key
        /// </summary>
        string LeaveEventExecuteServiceKeys
        {
            get;
        }

        /// <summary>
        /// 撤回到此活动时调用的Service
        /// </summary>
        WfServiceOperationDefinitionCollection WithdrawExecuteServices
        {
            get;
        }

        /// <summary>
        /// 被撤回活动调用的Service
        /// </summary>
        WfServiceOperationDefinitionCollection BeWithdrawnExecuteServices
        {
            get;
        }

        /// <summary>
        /// 撤回到此活动时调用的Service的Key
        /// </summary>
        string WithdrawExecuteServiceKeys
        {
            get;
        }

        /// <summary>
        /// 被撤回活动调用的Service的Key
        /// </summary>
        string BeWithdrawnExecuteServiceKeys
        {
            get;
        }

        /// <summary>
        /// 关联活动的Key，如果有此节点，表示此节点是被某个活动创建的。
        /// 例如加签操作，某个环节A加签了B，B的AssociatedActivityKey就是A。
        /// </summary>
        string AssociatedActivityKey
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        string ClonedKey
        {
            set;
            get;
        }

        /// <summary>
        /// 得到关联的活动定义
        /// </summary>
        /// <returns></returns>
        IWfActivityDescriptor GetAssociatedActivity();

        /// <summary>
        /// 在当前节点之后添加节点。当前节点不能是结束点。
        /// 自动创建一条当前点和新节点的连线，而当前点的出线，或作为新点的出线。
        /// </summary>
        /// <param name="newActDesp"></param>
        void Append(IWfActivityDescriptor newActDesp);

        /// <summary>
        /// 在当前活动之后添加活动。当前活动不能是结束点。
        /// 自动创建一条当前点和新节点的连线，而当前点的向前的出线，将作为新点的出线。
        /// </summary>
        /// <param name="newActDesp">需要添加的新活动</param>
        /// <param name="moveReturnLine">是否将当前点的退回线也复制到新的活动之后</param>
        void Append(IWfActivityDescriptor newActDesp, bool moveReturnLine);

        /// <summary>
        /// 在当前活动之前插入活动。当前活动不能是起始点。
        /// </summary>
        /// <param name="newActDesp"></param>
        void InsertBefore(IWfActivityDescriptor newActDesp);

        /// <summary>
        /// 克隆当前节点
        /// </summary>
        /// <param name="actDesp"></param>
        /// <returns></returns>
        IWfActivityDescriptor Clone();

        /// <summary>
        /// 删除活动描述本身，前后点的所有线都被删除
        /// </summary>
        void Remove();

        /// <summary>
        /// 删除活动描述本身
        /// </summary>
        void Delete();

        /// <summary>
        /// 描述所对应的实例对象
        /// </summary>
        IWfActivity Instance
        {
            get;
        }

        /// <summary>
        /// 得到所有的出线所包含的活动（没有重复的）
        /// </summary>
        /// <returns></returns>
        WfActivityDescriptorCollection GetToActivities();

        /// <summary>
        /// 得到所有的进线所包含的活动（没有重复的）
        /// </summary>
        /// <returns></returns>
        WfActivityDescriptorCollection GetFromActivities();

        /// <summary>
        /// 默认的待办标题    NotifyTaskTitle
        /// </summary>
        string TaskTitle
        {
            get;
            set;
        }

        string NotifyTaskTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 流程活动导航上的显示方式
        /// </summary>
        WfNavigatorDisplayMode NavigatorDisplayMode
        {
            get;
        }

        /// <summary>
        /// 是否是条件节点（有条件，且有开始时间）
        /// </summary>
        bool IsConditionActivity
        {
            get;
        }

        /// <summary>
        /// 是否是流程实例主线流程中的活动点
        /// </summary>
        bool IsMainStreamActivity
        {
            get;
        }

        /// <summary>
        /// 是否是退件时需要跳过的活动
        /// </summary>
        bool IsReturnSkipped
        {
            get;
        }

        /// <summary>
        /// 是否由模板活动生成的活动
        /// </summary>
        bool GeneratedByTemplate
        {
            get;
        }

        /// <summary>
        /// 生成此活动的动态模板活动的Key
        /// </summary>
        string TemplateKey
        {
            get;
        }

        /// <summary>
        /// 相同组的活动。当Varialbes中的“ActivityGroupName”变量值相同的活动。如果ActivityGroupName为空，则以Key值代替(组里只有自己)
        /// </summary>
        IEnumerable<IWfActivityDescriptor> GetSameGroupActivities();

        /// <summary>
        /// 根据活动的出线查找后续第一个符合条件的活动
        /// </summary>
        /// <param name="predicate">条件回调</param>
        /// <returns></returns>
        IWfActivityDescriptor FindSubsequentActivity(Func<IWfTransitionDescriptor, IWfActivityDescriptor, bool> predicate);

        /// <summary>
        /// 根据模板生成动态活动
        /// </summary>
        /// <returns>返回生成的活动，如果活动已经在执行，则返回null。返回null与返回0记录是不同的</returns>
        IList<IWfActivityDescriptor> GenerateDynamicActivities();

        /// <summary>
        /// 根据活动创建参数生成动态活动
        /// </summary>
        /// <param name="createActivitiesParams"></param>
        /// <returns>返回生成的活动，如果活动已经在执行，则返回null。返回null与返回0记录是不同的</returns>
        IList<IWfActivityDescriptor> GenerateDynamicActivities(WfCreateActivityParamCollection createActivitiesParams);

        /// <summary>
        /// Clone传入的线，然后与现有的线合并。
        /// 合并的原则是，目标点如果在现有的集合中已经存在，则不合并。
        /// </summary>
        /// <returns>最后添加的线</returns>
        /// <param name="sourceTransitions"></param>
        IList<IWfTransitionDescriptor> CloneAndMergeToTransitions(IEnumerable<IWfTransitionDescriptor> sourceTransitions);

        /// <summary>
        /// 将所有包含originalUser的WfUserResourceDescriptor的资源替换为包含replaceUsers的一系列资源。
        /// 如果replaceUsers为null或者空集合，则相当于删除原始用户
        /// </summary>
        /// <param name="originalUser"></param>
        /// <param name="replaceUsers"></param>
        /// <returns>被替换的个数</returns>
        int ReplaceAllUserResourceDescriptors(IUser originalUser, IEnumerable<IUser> replaceUsers);
    }

    /// <summary>
    /// 主流活动点的描述
    /// </summary>
    public interface IWfMainStreamActivityDescriptor
    {
        /// <summary>
        /// 对应的活动点描述
        /// </summary>
        IWfActivityDescriptor Activity
        {
            get;
        }

        /// <summary>
        /// 相关的活动点
        /// </summary>
        WfAssociatedActivitiesDescriptorCollection AssociatedActivities
        {
            get;
        }

        /// <summary>
        /// 根据连线遍历时的活动的层级
        /// </summary>
        int Level
        {
            get;
        }

        /// <summary>
        /// 这个活动是否经过
        /// </summary>
        bool Elapsed
        {
            get;
        }
    }
}
