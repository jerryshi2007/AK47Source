using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 复制主线活动点信息节点
    /// </summary>
    internal class WfCopyMainStreamActivityNode
    {
        private IWfActivityDescriptor _TemplateActivityDescriptor = null;
        private IWfActivityDescriptor _OriginalActivityDescriptor = null;
        private bool _NeedCopy = false;
        private bool _IsEndActivity = false;
        private WfCopyMainStreamSubActivityNodeCollection _ToActivities = new WfCopyMainStreamSubActivityNodeCollection();

        public WfCopyMainStreamActivityNode(IWfActivityDescriptor templateActDesp, IWfActivityDescriptor originalActDesp, bool needCopy, bool isEndActivity)
        {
            this._TemplateActivityDescriptor = templateActDesp;
            this._OriginalActivityDescriptor = originalActDesp;
            this._NeedCopy = needCopy;
            this._IsEndActivity = isEndActivity;
        }

        #region 属性集合
        public WfCopyMainStreamSubActivityNodeCollection ToActivities
        {
            get
            {
                return this._ToActivities;
            }
        }

        /// <summary>
        /// 是否需要复制
        /// </summary>
        public bool NeedCopy
        {
            get
            {
                return this._NeedCopy;
            }
        }

        /// <summary>
        /// 是否是最后一个需要复制的点
        /// </summary>
        public bool IsEndActivity
        {
            get
            {
                return this._IsEndActivity;
            }
        }

        /// <summary>
        /// 原始的活动描述
        /// </summary>
        public IWfActivityDescriptor TemplateActivityDescriptor
        {
            get
            {
                return this._TemplateActivityDescriptor;
            }
        }

        public IWfActivityDescriptor OriginalActivityDescriptor
        {
            get
            {
                return this._OriginalActivityDescriptor;
            }
        }

        internal string ActivityKey
        {
            get
            {
                return this._TemplateActivityDescriptor != null ? this._TemplateActivityDescriptor.Key : string.Empty;
            }
        }
        #endregion

        public void OutputNodeInfo()
        {
            Console.WriteLine("Key:{0}, NeedCopy: {1}, IsEnd: {2}", this.ActivityKey, this.NeedCopy, this.IsEndActivity);

            InnerOutputNodeInfo(1);
        }

        private void InnerOutputNodeInfo(int level)
        {
            foreach (WfCopyMainStreamSubActivityNode subNode in this.ToActivities)
            {
                Console.Write(new string('\t', level));
                Console.WriteLine("Key:{0}, NeedCopy: {1}, IsEnd: {2}",
                    subNode.ActivityNode.ActivityKey, subNode.ActivityNode.NeedCopy, subNode.ActivityNode.IsEndActivity);

                subNode.ActivityNode.InnerOutputNodeInfo(level + 1);
            }
        }

        /// <summary>
        /// 将MainStreamContext中的活动，复制到某个活动后
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="activityToAppend"></param>
        /// <param name="context"></param>
        /// <param name="operationType"></param>
        /// <returns>返回的第一个Clone活动的出线</returns>
        public IWfTransitionDescriptor GenerateCopiedActivitiesDescriptors(IWfProcess processInstance, IWfActivityDescriptor activityToAppend, WfCopyMainStreamContext context, WfControlOperationType operationType)
        {
            IWfTransitionDescriptor resultTransition = null;

            CloneRelativeActivities(activityToAppend.ProcessInstance, context, operationType);

            IWfActivityDescriptor clonedActDesp = null;

            if (context.ClonedActivities.TryGetValue(this._TemplateActivityDescriptor.Key, out clonedActDesp))
            {
                //如果本活动是模板生成的动态活动，且退件的目标不是模板，则删除后续的动态活动
                if (activityToAppend.GeneratedByTemplate && clonedActDesp.GeneratedByTemplate == false)
                {
                    IWfTransitionDescriptor templateTransition = activityToAppend.ToTransitions.Find(t => t.GeneratedByTemplate);

                    while (templateTransition != null)
                    {
                        IWfActivityDescriptor actDesp = templateTransition.ToActivity;

                        //如果线的目标活动是动态活动，则删除，否则忽略
                        if (actDesp.GeneratedByTemplate)
                            actDesp.Instance.Delete();

                        templateTransition = templateTransition.ToActivity.ToTransitions.Find(t => t.GeneratedByTemplate);
                    }
                }

                IWfTransitionDescriptor lastTransition = context.EntryTransition;

                if (lastTransition == null)
                    lastTransition = activityToAppend.ToTransitions.GetAllCanTransitForwardTransitions().FindDefaultSelectTransition();

                IWfActivityDescriptor lastActDesp = null;

                if (lastTransition != null)
                    lastActDesp = lastTransition.ToActivity;

                string entryTransitionKey = (lastTransition != null) ? lastTransition.Key : string.Empty;

                //将第一个克隆的活动与原流程的某一个活动关联。
                //保留退回线
                IList<IWfTransitionDescriptor> originalBackwardTransitions = activityToAppend.ToTransitions.FindAll(t => t.IsBackward);

                activityToAppend.ToTransitions.Clear();

                foreach (WfTransitionDescriptor t in originalBackwardTransitions)
                {
                    WfTransitionDescriptor newBackwardTransition = (WfTransitionDescriptor)t.Clone();

                    newBackwardTransition.Key = activityToAppend.Process.FindNotUsedTransitionKey();
                    activityToAppend.ToTransitions.AddTransition(t.ToActivity, newBackwardTransition);
                }

                //activityToAppend.ToTransitions.CopyFrom(originalBackwardTransitions);

                IWfTransitionDescriptor newTransition = JoinOriginalActivityToClonedActivity(activityToAppend, clonedActDesp, lastTransition);

                resultTransition = newTransition;

                //将自动流转设置为False
                if (clonedActDesp.Properties.GetValue("AutoMoveTo", false))
                    clonedActDesp.Properties.TrySetValue("AutoMoveTo", false);

                GenerateSubActivitiesDescriptors(processInstance, clonedActDesp, lastActDesp, context, operationType);
            }

            return resultTransition;
        }

        private static IWfTransitionDescriptor JoinOriginalActivityToClonedActivity(IWfActivityDescriptor originalActDesp, IWfActivityDescriptor clonedActDesp, IWfTransitionDescriptor transitionTemplate)
        {
            WfTransitionDescriptor newTransition = (WfTransitionDescriptor)originalActDesp.ToTransitions.AddForwardTransition(clonedActDesp);

            //将新添加的线的属性设置为原来线的属性
            //string newKey = newTransition.Key;
            newTransition.Properties.ReplaceExistedPropertyValues(transitionTemplate.Properties);
            //newTransition.Key = newKey;
            newTransition.IsBackward = false;
            newTransition.DefaultSelect = true;

            return newTransition;
        }

        /// <summary>
        /// 复制所有的后续活动（没有连线）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="operationType"></param>
        internal void CloneRelativeActivities(IWfProcess processInstance, WfCopyMainStreamContext context, WfControlOperationType operationType)
        {
            if (this.NeedCopy)
            {
                if (context.ClonedActivities.ContainsKey(this.TemplateActivityDescriptor.Key) == false)
                {
                    IWfActivityDescriptor clonedActDesp = CreateClonedActivityDescriptor(processInstance, context, operationType);

                    context.ClonedActivities.Add(this.TemplateActivityDescriptor.Key, clonedActDesp);

                    this.ToActivities.ForEach(sub => sub.ActivityNode.CloneRelativeActivities(processInstance, context, operationType));
                }
            }
        }

        /// <summary>
        /// 克隆后续的活动并且连线
        /// </summary>
        /// <param name="endActivity"></param>
        /// <param name="context"></param>
        /// <param name="operationType"></param>
        private void GenerateSubActivitiesDescriptors(IWfProcess processInstance, IWfActivityDescriptor endActivity, IWfActivityDescriptor lastActDesp, WfCopyMainStreamContext context, WfControlOperationType operationType)
        {
            foreach (WfCopyMainStreamSubActivityNode subNode in this.ToActivities)
            {
                IWfActivityDescriptor targetActDesp = null;

                if (subNode.ActivityNode.NeedCopy)
                {
                    if (context.ClonedActivities.TryGetValue(subNode.ActivityNode.TemplateActivityDescriptor.Key, out targetActDesp) == false)
                    {
                        targetActDesp = subNode.ActivityNode.CreateClonedActivityDescriptor(processInstance, context, operationType);
                        context.ClonedActivities.Add(subNode.ActivityNode.TemplateActivityDescriptor.Key, targetActDesp);
                    }
                }
                else
                {
                    targetActDesp = subNode.ActivityNode.OriginalActivityDescriptor;
                    //targetActDesp = subNode.ActivityNode.TemplateActivityDescriptor;
                }

                //本身是复制点，则必须复制线
                if (this.NeedCopy)
                {
                    WfTransitionDescriptor clonedTransition = (WfTransitionDescriptor)((WfTransitionDescriptor)subNode.FromTransition).Clone();

                    //沈峥注释，2015-6-25，FindNotUsedTransitionKey应该从实例流程查找
                    //if (clonedTransition.Key.IsNullOrEmpty())
                    //    clonedTransition.Key = subNode.ActivityNode.TemplateActivityDescriptor.Process.FindNotUsedTransitionKey();
                    if (clonedTransition.Key.IsNullOrEmpty())
                        clonedTransition.Key = endActivity.Process.FindNotUsedTransitionKey();

                    if (this.IsEndActivity)
                    {
                        string targetAssociatedKey = null;

                        //沈峥调整，2015-1-24，目标点非Clone点且是加签点
                        if (targetActDesp.ClonedKey.IsNullOrEmpty() && targetActDesp.GetAssociatedActivity() != null)
                            targetAssociatedKey = targetActDesp.GetAssociatedActivity().Key;

                        if (lastActDesp != null && targetAssociatedKey != null && lastActDesp.GetAssociatedActivity().Key == targetAssociatedKey)
                            targetActDesp = lastActDesp;
                    }

                    endActivity.ToTransitions.AddTransition(targetActDesp, clonedTransition);
                }

                subNode.ActivityNode.GenerateSubActivitiesDescriptors(processInstance, targetActDesp, lastActDesp, context, operationType);
            }
        }

        private IWfActivityDescriptor CreateClonedActivityDescriptor(IWfProcess processInstance, WfCopyMainStreamContext context, WfControlOperationType operationType)
        {
            WfActivityDescriptor result = this.TemplateActivityDescriptor.Clone() as WfActivityDescriptor;
            result.Key = processInstance.Descriptor.FindNotUsedActivityKey();

            //2012/12/03 修改克隆时向开始环节转换时需要设置的属性
            if (this.TemplateActivityDescriptor.ActivityType == WfActivityType.InitialActivity && operationType == WfControlOperationType.Return)
                ResetPropertiesByDefinedName(result, "DefaultReturnToInitialActivityTemplate");

            //如果模板活动不是主线活动，则使用该活动的关联Key，否则去实例上查找主线活动对应活动的关联Key。
            if (this.TemplateActivityDescriptor.IsMainStreamActivity == false)
            {
                result.AssociatedActivityKey = this.TemplateActivityDescriptor.GetAssociatedActivity().Key;

                result.ClonedKey = this.TemplateActivityDescriptor.Key;
            }
            else
            {
                IWfActivityDescriptor matchActDesp = FindActivityDescriptorByMainStreamKey(processInstance, this.TemplateActivityDescriptor.Key);

                if (matchActDesp != null)
                {
                    result.AssociatedActivityKey = matchActDesp.Key;

                    result.ClonedKey = matchActDesp.Key;
                }
            }

            //如果原来活动上没有配置资源，则使用原来活动实例的指派人作为资源
            if (result.Resources.Count == 0)
                result.Resources.CopyFrom(this.TemplateActivityDescriptor.Instance.Assignees.ToResources());

            WfActivityBase newActivity = WfActivityBase.CreateActivityInstance(result, processInstance);

            if (context.IsMainStream)
                newActivity.MainStreamActivityKey = this.TemplateActivityDescriptor.Key;

            //沈峥调整，生成资源
            if (result.Resources.Count == 0)
            {
                newActivity.Candidates.CopyFrom(this.TemplateActivityDescriptor.Instance.Candidates);

                if (newActivity.Candidates.Count == 0)
                {
                    IWfActivityDescriptor matchActDesp = FindActivityDescriptorByMainStreamKey(processInstance, this.TemplateActivityDescriptor.Key);

                    if (matchActDesp != null && OguBase.IsNotNullOrEmpty(matchActDesp.Instance.Operator))
                        newActivity.Candidates.Add(matchActDesp.Instance.Operator);
                }

                newActivity.Assignees.CopyFrom(newActivity.Candidates);
            }
            else
            {
                if (newActivity.Descriptor.Properties.GetValue("AutoGenerateCadidates", true))
                {
                    newActivity.GenerateCandidatesFromResources();
                    newActivity.Assignees.Clear();
                    newActivity.Assignees.CopyFrom(newActivity.Candidates);
                }
                else
                {
                    newActivity.Candidates.Clear();
                    newActivity.Assignees.Clear();

                    //仅复制能够和直接转换为用户的角色
                    foreach (IUser user in result.Resources.ToUsers())
                    {
                        newActivity.Candidates.Add(user);
                        newActivity.Assignees.Add(user);
                    }
                }
            }

            processInstance.Descriptor.Activities.Add(result);

            return result;
        }

        private static void ResetPropertiesByDefinedName(WfActivityDescriptor actDesp, string definedName)
        {
            PropertyDefineCollection definedProperties = new PropertyDefineCollection();
            definedProperties.LoadPropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups[definedName]);

            actDesp.Properties.ReplaceDefinedProperties(definedProperties);
        }

        private static IWfActivityDescriptor FindFirstNotRunningAssociateActivity(IWfActivityDescriptor startActDesp, string actKey)
        {
            Dictionary<string, IWfTransitionDescriptor> elapsedTransitions = new Dictionary<string, IWfTransitionDescriptor>();

            IWfActivityDescriptor result = null;

            foreach (IWfTransitionDescriptor toTransition in startActDesp.ToTransitions)
            {
                result = InnerFindFirstAssociateActivity(toTransition, actKey, elapsedTransitions);

                if (result != null)
                    break;
            }

            return result;
        }

        private static IWfActivityDescriptor InnerFindFirstAssociateActivity(IWfTransitionDescriptor transition, string actKey, Dictionary<string, IWfTransitionDescriptor> elapsedTransitions)
        {
            IWfActivityDescriptor result = null;

            if (elapsedTransitions.ContainsKey(transition.Key) || transition.IsBackward == true)
                return result;

            elapsedTransitions.Add(transition.Key, transition);

            if (transition.ToActivity.GetAssociatedActivity().Key == actKey)
            {
                if (transition.ToActivity.Instance != null && transition.ToActivity.Instance.Status == WfActivityStatus.NotRunning)
                    result = transition.ToActivity;
            }

            if (result == null)
            {
                foreach (IWfTransitionDescriptor toTransition in transition.ToActivity.ToTransitions)
                {
                    result = InnerFindFirstAssociateActivity(toTransition, actKey, elapsedTransitions);

                    if (result != null)
                        break;
                }
            }

            return result;
        }

        private static IWfActivityDescriptor FindLastAssociateActivity(IWfActivityDescriptor startActDesp, string actKey)
        {
            Dictionary<string, IWfTransitionDescriptor> elapsedTransitions = new Dictionary<string, IWfTransitionDescriptor>();

            IWfActivityDescriptor result = null;

            foreach (IWfTransitionDescriptor fromTransition in startActDesp.FromTransitions)
            {
                result = InnerFindLastAssociateActivity(fromTransition, actKey, elapsedTransitions);

                if (result != null)
                    break;
            }

            return result;
        }

        private static IWfActivityDescriptor InnerFindLastAssociateActivity(IWfTransitionDescriptor transition, string actKey, Dictionary<string, IWfTransitionDescriptor> elapsedTransitions)
        {
            IWfActivityDescriptor result = null;

            if (elapsedTransitions.ContainsKey(transition.Key) || transition.IsBackward == true)
                return result;

            elapsedTransitions.Add(transition.Key, transition);

            if (transition.FromActivity.GetAssociatedActivity().Key == actKey)
                result = transition.FromActivity;

            foreach (IWfTransitionDescriptor fromTransition in transition.FromActivity.FromTransitions)
            {
                result = InnerFindLastAssociateActivity(fromTransition, actKey, elapsedTransitions);

                if (result != null)
                    break;
            }

            return result;
        }

        /// <summary>
        /// 查找在流程实例中匹配到某个主线活动，且不是关联活动的活动描述
        /// </summary>
        /// <param name="process"></param>
        /// <param name="msActKey"></param>
        /// <returns></returns>
        private static IWfActivityDescriptor FindActivityDescriptorByMainStreamKey(IWfProcess process, string msActKey)
        {
            IWfActivityDescriptor matchedActivity = null;

            process.Descriptor.ProbeAllActivities(actArgs =>
            {
                bool matched = actArgs.ActivityDescriptor.Instance.MainStreamActivityKey == msActKey
                    && actArgs.ActivityDescriptor.AssociatedActivityKey.IsNullOrEmpty();

                if (matched)
                    matchedActivity = actArgs.ActivityDescriptor;

                return matched == false;
            }, t => true);

            return matchedActivity;
        }
    }
}
