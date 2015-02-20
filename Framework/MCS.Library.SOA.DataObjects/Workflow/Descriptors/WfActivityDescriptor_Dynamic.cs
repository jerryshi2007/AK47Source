using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow.Builders;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public partial class WfActivityDescriptor
    {
        /// <summary>
        /// 根据模板生成动态活动
        /// </summary>
        /// <returns>返回生成的活动，如果活动已经在执行，则返回null。返回null与返回0记录是不同的</returns>
        public IList<IWfActivityDescriptor> GenerateDynamicActivities()
        {
            return InternalGenerateDynamicActivities(() => this.Resources.ToCreateActivityParams());
        }

        /// <summary>
        /// 根据活动创建参数生成动态活动
        /// </summary>
        /// <param name="createActivitiesParams"></param>
        /// <returns>返回生成的活动，如果活动已经在执行，则返回null。返回null与返回0记录是不同的</returns>
        public IList<IWfActivityDescriptor> GenerateDynamicActivities(WfCreateActivityParamCollection createActivitiesParams)
        {
            return InternalGenerateDynamicActivities(() => createActivitiesParams);
        }

        /// <summary>
        /// 创建动态活动
        /// </summary>
        /// <param name="createActivitiesParamsFetcher"></param>
        /// <returns></returns>
        private IList<IWfActivityDescriptor> InternalGenerateDynamicActivities(Func<WfCreateActivityParamCollection> createActivitiesParamsFetcher)
        {
            createActivitiesParamsFetcher.NullCheck("createActivitiesParamsFetcher");

            List<IWfActivityDescriptor> result = null;

            //随便找一条进线和活动，构造动态活动的起始点。
            IWfTransitionDescriptor firstTemplateTransition = this.FromTransitions.FirstOrDefault();

            if (firstTemplateTransition != null)
            {
                //寻找动态活动的前一个点，如果有多个点，随机选取一个
                WfActivityDescriptor startActDesp = (WfActivityDescriptor)firstTemplateTransition.FromActivity;

                IWfTransitionDescriptor dynInTransition = startActDesp.FindExsitedDynamicToTransition(this);

                ////没有生成过，或者已经生成过，且生成的活动还没有执行
                if (dynInTransition == null || InstanceIsRunning(dynInTransition.ToActivity) == false)
                {
                    //如果已经生成过
                    if (dynInTransition != null)
                        DeleteExistedDynamicActivities(dynInTransition, this);

                    WfCreateActivityParamCollection createActivityParams = createActivitiesParamsFetcher();

                    //活动矩阵定义的属性优先于活动模板定义的属性，矩阵中没有设置的属性，则使用活动模版的属性
                    MergeDynamicActivitiesProperties(createActivityParams, this);

                    WfActivityDescriptorCollection createdActivities = CreateActivities(this.FromTransitions, createActivityParams, this.ToTransitions);

                    ChangeNewDynActivitiesProperties(createdActivities, this);

                    result = new List<IWfActivityDescriptor>();

                    createdActivities.CopyTo(result);
                }
            }

            return result;
        }

        /// <summary>
        /// 活动实例是否运行过或正在运行
        /// </summary>
        /// <returns></returns>
        private static bool InstanceIsRunning(IWfActivityDescriptor actDesp)
        {
            bool result = false;

            if (actDesp.IsMainStreamActivity == false && actDesp.Instance != null)
                result = actDesp.Instance.Status != WfActivityStatus.NotRunning;

            return result;
        }

        //活动矩阵定义的属性优先于活动模板定义的属性，矩阵中没有设置的属性，则使用活动模版的属性
        private static void MergeDynamicActivitiesProperties(WfCreateActivityParamCollection createActivityParams, IWfActivityDescriptor templateActDesp)
        {
            foreach (WfCreateActivityParam cap in createActivityParams)
            {
                PropertyValueCollection templateProperties = new PropertyValueCollection();

                foreach (PropertyValue pv in templateActDesp.Properties)
                {
                    if (cap.RoleDefineActivityPropertyNames.Exists(name => pv.Definition.Name == name))
                    {
                        if (cap.Template.Properties.ContainsKey(pv.Definition.Name))
                            templateProperties.Add(cap.Template.Properties[pv.Definition.Name]);
                    }
                    else
                        templateProperties.Add(pv);
                }

                cap.Template.Properties.ReplaceExistedPropertyValues(templateProperties);

                cap.Template.RelativeLinks.Clear();
                cap.Template.RelativeLinks.CopyFrom(templateActDesp.RelativeLinks);

                cap.Template.Variables.Clear();
                cap.Template.Variables.CopyFrom(templateActDesp.Variables);

                cap.Template.EnterEventReceivers.Clear();
                cap.Template.EnterEventReceivers.CopyFrom(templateActDesp.EnterEventReceivers);

                cap.Template.LeaveEventReceivers.Clear();
                cap.Template.LeaveEventReceivers.CopyFrom(templateActDesp.LeaveEventReceivers);

                cap.Template.EnterEventExecuteServices.Clear();
                cap.Template.EnterEventExecuteServices.CopyFrom(templateActDesp.EnterEventExecuteServices);

                cap.Template.LeaveEventExecuteServices.Clear();
                cap.Template.LeaveEventExecuteServices.CopyFrom(templateActDesp.LeaveEventExecuteServices);
            }
        }

        /// <summary>
        ///  删除已经生成的动态活动
        /// </summary>
        /// <param name="dynInTransition"></param>
        /// <param name="templateActivity"></param>
        internal static void DeleteExistedDynamicActivities(IWfTransitionDescriptor dynInTransition, IWfActivityDescriptor templateActivity)
        {
            IWfActivityDescriptor currentActDesp = dynInTransition.ToActivity;

            WfActivityDescriptorCollection endActivities = templateActivity.GetToActivities();

            //沈峥修改，2015-1-25，原来删除已经存在的动态活动时没有考虑多条出线的情况，现在改为递归删除
            RemoveExistedDynamicActivityRecursively(currentActDesp, endActivities);
            ////判断当前活动不在结束的活动中
            //while (currentActDesp != null && ((WfActivityDescriptor)currentActDesp).GeneratedByTemplate && endActivities.ContainsKey(currentActDesp.Key) == false)
            //{
            //    IWfActivityDescriptor nextActDesp = currentActDesp.GetToActivities().FirstOrDefault();

            //    if (currentActDesp.IsMainStreamActivity == false && currentActDesp.Instance != null)
            //        currentActDesp.Instance.Remove();
            //    else
            //        currentActDesp.Remove();

            //    currentActDesp = nextActDesp;
            //}

            foreach (IWfActivityDescriptor actDesp in templateActivity.GetFromActivities())
            {
                IList<IWfTransitionDescriptor> transitions = actDesp.ToTransitions.FindAll(t => t.Properties.GetValue("DynamicSource", string.Empty) == templateActivity.Key);

                transitions.ForEach(t => actDesp.ToTransitions.Remove(t));
            }
        }

        private static void RemoveExistedDynamicActivityRecursively(IWfActivityDescriptor currentActDesp, WfActivityDescriptorCollection endActivities)
        {
            //判断当前活动不在结束的活动中
            if (currentActDesp != null && ((WfActivityDescriptor)currentActDesp).GeneratedByTemplate && endActivities.ContainsKey(currentActDesp.Key) == false)
            {
                //判断当前活动是不已经被删除
                if (currentActDesp.Process.Activities.ContainsKey(currentActDesp.Key))
                {
                    WfActivityDescriptorCollection nextActivities = currentActDesp.GetToActivities();

                    if (currentActDesp.IsMainStreamActivity == false && currentActDesp.Instance != null)
                        currentActDesp.Instance.Remove();
                    else
                        currentActDesp.Remove();

                    foreach (IWfActivityDescriptor nextActivity in nextActivities)
                        RemoveExistedDynamicActivityRecursively(nextActivity, endActivities);
                }
            }
        }

        private WfActivityDescriptorCollection CreateActivities(WfTransitionDescriptorCollection fromTransitions, WfCreateActivityParamCollection capc, WfTransitionDescriptorCollection toTransitions)
        {
            WfActivityDescriptorCollection result = new WfActivityDescriptorCollection(this.Process);

            IWfActivityDescriptor firstDynamicActivity = null;
            IWfActivityDescriptor lastDynamicActivity = null;
            WfCreateActivityParam lastCreatedActivityParam = null;

            foreach (WfCreateActivityParam cap in capc)
            {
                WfActivityDescriptor actDesp = (WfActivityDescriptor)cap.Template.Clone();

                actDesp.Process = this.Process;

                string newActKey = "ND" + cap.ActivitySN;

                if (this.Process.Activities.ContainsKey(newActKey))
                    newActKey = this.Process.FindNotUsedActivityKey();

                actDesp.Key = newActKey;
                cap.CreatedDescriptor = actDesp;

                this.Process.Activities.Add(actDesp);

                if (lastCreatedActivityParam != null)
                    lastCreatedActivityParam.DefaultNextDescriptor = actDesp;

                lastCreatedActivityParam = cap;

                if (this.IsMainStreamActivity == false && this.ProcessInstance != null)
                    WfActivityBase.CreateActivityInstance(actDesp, this.ProcessInstance);

                if (firstDynamicActivity == null)
                    firstDynamicActivity = actDesp;

                if (lastDynamicActivity != null)
                {
                    WfTransitionDescriptor newTransition = (WfTransitionDescriptor)lastDynamicActivity.ToTransitions.AddForwardTransition(actDesp);

                    //如果不是第一个生成的活动
                    if (firstDynamicActivity != actDesp)
                    {
                        newTransition.GeneratedByTemplate = true;
                        newTransition.TemplateKey = this.Key;
                    }
                }

                lastDynamicActivity = actDesp;

                result.Add(actDesp);
            }

            if (firstDynamicActivity != null && lastDynamicActivity != null)
            {
                SetEntryTransitionsProperties(firstDynamicActivity, fromTransitions);

                if (lastCreatedActivityParam != null)
                {
                    IWfTransitionDescriptor defaultTransition =
                        toTransitions.GetAllConditionMatchedTransitions().FindDefaultSelectTransition(true);

                    if (defaultTransition != null)
                        lastCreatedActivityParam.DefaultNextDescriptor = defaultTransition.ToActivity;
                }

                capc.AdjustTransitionsByTemplate(this.Key);

                //如果在活动模板参数的最后一个活动没有配置出线，则使用默认的出线；否则使用配置的出线
                if (lastDynamicActivity.ToTransitions.Count == 0)
                    SetExitTransitionsProperties(lastDynamicActivity, toTransitions);
                else
                    lastDynamicActivity.ToTransitions.ForEach(t => SetDynamicTransitionProperties(null, t, this, false));
            }
            else
            {
                //当没有动态活动生成时，前后的活动直接串联到一起
                JoinOriginalActivitiesAndSetProperties(fromTransitions, toTransitions);
            }

            //将模版点的进出线的Enabled都设置为False
            fromTransitions.ForEach(t => ((WfTransitionDescriptor)t).Enabled = false);
            toTransitions.ForEach(t => ((WfTransitionDescriptor)t).Enabled = false);

            return result;
        }

        /// <summary>
        /// 设置原来的进线和第一个动态活动点之间的关系和属性
        /// </summary>
        /// <param name="firstDynamicActivity"></param>
        /// <param name="fromTransitions"></param>
        private static void SetEntryTransitionsProperties(IWfActivityDescriptor firstDynamicActivity, WfTransitionDescriptorCollection fromTransitions)
        {
            foreach (IWfTransitionDescriptor transition in fromTransitions)
            {
                IWfTransitionDescriptor dynTransition = transition.FromActivity.ToTransitions.AddForwardTransition(firstDynamicActivity);

                SetDynamicTransitionProperties(transition, dynTransition, transition.ToActivity, true);
            }
        }

        /// <summary>
        /// 设置原来的出线和最后一个动态活动点之间的关系和属性
        /// </summary>
        /// <param name="lastDynamicActivity"></param>
        /// <param name="toTransitions"></param>
        private static void SetExitTransitionsProperties(IWfActivityDescriptor lastDynamicActivity, WfTransitionDescriptorCollection toTransitions)
        {
            foreach (IWfTransitionDescriptor transition in toTransitions)
            {
                IWfTransitionDescriptor dynTransition = transition.ToActivity.FromTransitions.AddForwardTransition(lastDynamicActivity);

                SetDynamicTransitionProperties(transition, dynTransition, transition.FromActivity, false);
            }
        }

        /// <summary>
        /// 当没有动态活动生成时，前后的活动直接串联到一起
        /// </summary>
        /// <param name="fromTransitions"></param>
        /// <param name="toTransitions"></param>
        private static void JoinOriginalActivitiesAndSetProperties(WfTransitionDescriptorCollection fromTransitions, WfTransitionDescriptorCollection toTransitions)
        {
            foreach (IWfTransitionDescriptor fromTransition in fromTransitions)
            {
                foreach (IWfTransitionDescriptor toTransition in toTransitions)
                {
                    IWfTransitionDescriptor dynTransition = fromTransition.FromActivity.ToTransitions.AddForwardTransition(toTransition.ToActivity);

                    SetDynamicTransitionProperties(fromTransition, dynTransition, fromTransition.ToActivity, true);
                }
            }
        }

        /// <summary>
        /// 设置动态活动进出线的属性
        /// </summary>
        /// <param name="sourceTransition"></param>
        /// <param name="dynTransition"></param>
        /// <param name="templateActDesp"></param>
        /// <param name="isEntryTransition"></param>
        private static void SetDynamicTransitionProperties(IWfTransitionDescriptor sourceTransition, IWfTransitionDescriptor dynTransition, IWfActivityDescriptor templateActDesp, bool isEntryTransition)
        {
            if (sourceTransition != null)
                ((WfTransitionDescriptor)sourceTransition).CloneProperties((WfTransitionDescriptor)dynTransition);

            ((WfTransitionDescriptor)dynTransition).Enabled = true;

            if (isEntryTransition)
                ((WfTransitionDescriptor)dynTransition).IsDynamicActivityTransition = true;

            dynTransition.Properties.TrySetValue("DynamicSource", templateActDesp.Key);
        }

        /// <summary>
        /// 动态活动生成后整理新生成的动态活动的属性
        /// </summary>
        /// <param name="firstDynActDesp"></param>
        /// <param name="wfActivityBase"></param>
        private static void ChangeNewDynActivitiesProperties(WfActivityDescriptorCollection createdActivities, IWfActivityDescriptor templateActDesp)
        {
            foreach (IWfActivityDescriptor actDesp in createdActivities)
            {
                ((WfActivityDescriptor)actDesp).SetDynamicActivityProperties(templateActDesp.Key);

                if (actDesp.IsMainStreamActivity == false && actDesp.Instance != null)
                    actDesp.Instance.GenerateCandidatesFromResources();
            }
        }

        internal void SetDynamicActivityProperties(string templateActkey)
        {
            this.Properties.TrySetValue("AutoGenerateCadidates", false);
            this.Properties.TrySetValue("IsDynamic", false);

            //由模板活动生成的活动标志
            this.GeneratedByTemplate = true;
            this.TemplateKey = templateActkey;
        }
    }
}
