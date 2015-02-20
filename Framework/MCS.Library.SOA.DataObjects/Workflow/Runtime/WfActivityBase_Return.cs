using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public abstract partial class WfActivityBase
    {
        /// <summary>
        /// 在当前活动实例后，添加一些被克隆的活动。
        /// </summary>
        /// <param name="startActivity">需要复制的活动的起始点</param>
        /// <param name="entryTransition"></param>
        /// <param name="operationType"></param>
        /// <returns>返回的第一个Clone活动的出线</returns>
        public IWfTransitionDescriptor CopyMainStreamActivities(IWfActivity startActivity, IWfTransitionDescriptor entryTransition, WfControlOperationType operationType)
        {
            return CopyMainStreamActivities(this, startActivity, this, entryTransition, operationType);
        }

        /// <summary>
        /// 在指定的活动点之后，添加从startActivity与endActivity之间复制出来的
        /// </summary>
        /// <param name="activityToAppend">复制的活动需要添加到哪一个活动之后</param>
        /// <param name="startActivity">需要复制的活动的起始点</param>
        /// <param name="endActivity">需要复制的活动的终止点</param>
        /// <param name="entryTransition"></param>
        /// <param name="operationType">操作类型</param>
        /// <returns>返回的第一个Clone活动的出线</returns>
        public IWfTransitionDescriptor CopyMainStreamActivities(IWfActivity activityToAppend, IWfActivity startActivity, IWfActivity endActivity, IWfTransitionDescriptor entryTransition, WfControlOperationType operationType)
        {
            activityToAppend.NullCheck("activityToAppend");

            //沈峥注释，和WfMoveToExecutor中，是否按退回线进行的判断进行匹配
            ExceptionHelper.TrueThrow(startActivity.Status == WfActivityStatus.NotRunning, "退件时所指定活动必须是已执行过的活动");

            //沈峥重构后的实现
            return CreateMainStreamActivities(activityToAppend, startActivity, endActivity, entryTransition, operationType);
        }

        #region 新版实现
        private static IWfTransitionDescriptor CreateMainStreamActivities(IWfActivity activityToAppend, IWfActivity startActivity, IWfActivity endActivity, IWfTransitionDescriptor entryTransition, WfControlOperationType operationType)
        {
            bool useOriginal = true;
            IWfTransitionDescriptor resultTransition = null;

            if (activityToAppend.Process.MainStream != null)
            {
                IWfActivityDescriptor startActDesp = startActivity.GetMainStreamActivityDescriptor();
                IWfActivityDescriptor endActDesp = endActivity.GetMainStreamActivityDescriptor();

                if (startActDesp != null && endActDesp != null)
                {
                    WfCopyMainStreamContext context = new WfCopyMainStreamContext(startActDesp, endActivity.Descriptor, entryTransition, true);

                    FillCopyContext(context.StartActivityDescriptor, endActDesp, context, operationType);

                    resultTransition = context.StartActivityDescriptor.GenerateCopiedActivitiesDescriptors(activityToAppend.Process, activityToAppend.Descriptor, context, operationType);
                    useOriginal = false;
                }
            }

            if (useOriginal)
                OriginalCreateMainStreamActivities(activityToAppend, startActivity, endActivity, entryTransition, operationType);

            return resultTransition;
        }

        /// <summary>
        /// 历史实现规则。当流程不存在MainStream属性时，采用从当前流程活动中爬出来
        /// </summary>
        /// <param name="activityToAppend"></param>
        /// <param name="startActivity"></param>
        /// <param name="endActivity"></param>
        /// <param name="operationType"></param>
        private static void OriginalCreateMainStreamActivities(IWfActivity activityToAppend, IWfActivity startActivity, IWfActivity endActivity, IWfTransitionDescriptor entryTransition, WfControlOperationType operationType)
        {
            WfCopyMainStreamContext context = new WfCopyMainStreamContext(startActivity.Descriptor, endActivity.Descriptor, entryTransition, false);

            FillCopyContext(context.StartActivityDescriptor, endActivity.Descriptor, context, operationType);

            context.StartActivityDescriptor.GenerateCopiedActivitiesDescriptors(activityToAppend.Process, activityToAppend.Descriptor, context, operationType);
        }

        private static void FillCopyContext(WfCopyMainStreamActivityNode currentActivity, IWfActivityDescriptor endActivity, WfCopyMainStreamContext context, WfControlOperationType operationType)
        {
            foreach (IWfTransitionDescriptor transition in currentActivity.TemplateActivityDescriptor.ToTransitions.GetNotDynamicActivityTransitions())
            {
                if (context.IsElapsedTransition(transition) == false)
                {
                    IWfActivityDescriptor nextActivityDesp = transition.ToActivity;

                    if (operationType == WfControlOperationType.Return)
                        nextActivityDesp = FindNotReturnSkippedActivity(transition.ToActivity);

                    bool nextIsEndActivity = nextActivityDesp.Key == endActivity.Key;

                    IWfActivityDescriptor originalActDesp = nextActivityDesp;

                    if (context.IsMainStream)
                    {
                        originalActDesp = context.EndActivityDescriptor.FindSubsequentActivity((t, actDesp) =>
                            t.IsBackward == false && actDesp.Instance.MainStreamActivityKey == nextActivityDesp.Key);
                    }

                    WfCopyMainStreamActivityNode toActivity = new WfCopyMainStreamActivityNode(
                            nextActivityDesp,
                            originalActDesp,
                            IsSubsequentActivity(nextActivityDesp, endActivity) == false,
                            nextIsEndActivity);

                    currentActivity.ToActivities.Add(new WfCopyMainStreamSubActivityNode(transition, toActivity));

                    context.AddElapsedTransition(transition);

                    if (currentActivity.TemplateActivityDescriptor.Key != endActivity.Key)
                        FillCopyContext(toActivity, endActivity, context, operationType);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentActivityDesp"></param>
        /// <returns></returns>
        private static IWfActivityDescriptor FindNotReturnSkippedActivity(IWfActivityDescriptor currentActivityDesp)
        {
            Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor> elapsedTransitions = new Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor>();

            return InnerFindNotReturnSkippedActivity(currentActivityDesp, elapsedTransitions);
        }

        private static IWfActivityDescriptor InnerFindNotReturnSkippedActivity(IWfActivityDescriptor currentActivityDesp, Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor> elapsedTransitions)
        {
            IWfActivityDescriptor result = null;

            if (currentActivityDesp.IsReturnSkipped == false)
                result = currentActivityDesp;
            else
            {
                foreach (IWfTransitionDescriptor transition in currentActivityDesp.ToTransitions)
                {
                    if (elapsedTransitions.ContainsKey(transition) == false)
                    {
                        elapsedTransitions.Add(transition, transition);

                        result = InnerFindNotReturnSkippedActivity(transition.ToActivity, elapsedTransitions);

                        if (result != null)
                            break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// targetActivity是否在currentActivity的后续的线上
        /// </summary>
        /// <param name="targetActivity"></param>
        /// <param name="currentActivity"></param>
        /// <returns></returns>
        private static bool IsSubsequentActivity(IWfActivityDescriptor targetActivity, IWfActivityDescriptor currentActivity)
        {
            Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor> elapsedTransitions = new Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor>();

            return InnerIsSubsequentActivity(targetActivity, currentActivity, elapsedTransitions);
        }

        private static bool InnerIsSubsequentActivity(IWfActivityDescriptor targetActivity, IWfActivityDescriptor currentActivity, Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor> elapsedTransitions)
        {
            bool result = false;

            if (targetActivity.Key != currentActivity.Key)
            {
                foreach (IWfTransitionDescriptor transition in currentActivity.ToTransitions)
                {
                    if (elapsedTransitions.ContainsKey(transition) == false)
                    {
                        elapsedTransitions.Add(transition, transition);

                        if (transition.IsBackward == false)
                        {
                            if (transition.ToActivity.GetAssociatedActivity().Key == targetActivity.Key)
                                result = true;

                            if (result == false)
                                result = InnerIsSubsequentActivity(targetActivity, transition.ToActivity, elapsedTransitions);

                            if (result)
                                break;
                        }
                    }
                }
            }

            return result;
        }

        internal static void ResetPropertiesByDefinedName(WfActivityDescriptor actDesp, string definedName)
        {
            PropertyDefineCollection definedProperties = new PropertyDefineCollection();
            definedProperties.LoadPropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups[definedName]);

            actDesp.Properties.ReplaceDefinedProperties(definedProperties);
        }
        #endregion 新版实现
    }
}
