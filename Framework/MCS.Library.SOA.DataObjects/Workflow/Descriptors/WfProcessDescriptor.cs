using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    [Serializable]
    [XElementSerializable]
    public partial class WfProcessDescriptor : WfKeyedDescriptorBase, IWfProcessDescriptor, ISimpleXmlSerializer
    {
        private static readonly string[] ReservedVariableNames = new string[] { "MainStream" };

        private WfVariableDescriptorCollection _Variables = null;
        private WfActivityDescriptorCollection _Activities = null;
        private WfRelativeLinkDescriptorCollection _RelativeLinks = null;
        private WfResourceDescriptorCollection _CancelEventReceivers = null;
        private WfResourceDescriptorCollection _CompleteEventReceivers = null;
        private WfResourceDescriptorCollection _InternalRelativeUsers = null;
        private WfExternalUserCollection _ExternalUsers = null;

        private WfServiceOperationDefinitionCollection _CancelBeforeExecuteServices = null;
        private WfServiceOperationDefinitionCollection _CancelAfterExecuteServices = null;

        [XElementFieldSerialize(IgnoreDeserializeError = true)]
        private WfParameterNeedToBeCollected _ParametersNeedToBeCollected = null;

        #region IWfProcessDescriptor Members
        public WfProcessDescriptor()
        {
        }

        public WfProcessDescriptor(string key)
            : base(key)
        {
        }

        [XElementFieldSerialize(AlternateFieldName = "_GD")]
        public string GraphDescription
        {
            get;
            set;
        }

        public WfVariableDescriptorCollection Variables
        {
            get
            {
                if (this._Variables == null)
                    this._Variables = new WfVariableDescriptorCollection(this);

                return this._Variables;
            }
        }

        public WfActivityDescriptorCollection Activities
        {
            get
            {
                if (this._Activities == null)
                    this._Activities = new WfActivityDescriptorCollection(this);

                return this._Activities;
            }
        }

        public WfRelativeLinkDescriptorCollection RelativeLinks
        {
            get
            {
                if (this._RelativeLinks == null)
                    this._RelativeLinks = new WfRelativeLinkDescriptorCollection(this);

                return this._RelativeLinks;
            }
        }

        public string Version
        {
            get;
            set;
        }

        public bool AutoGenerateResourceUsers
        {
            get { return Properties.GetValue("AutoGenerateResourceUsers", true); }
            set { Properties.SetValue("AutoGenerateResourceUsers", value); }
        }

        public WfProcessType ProcessType
        {
            get { return Properties.GetValue("ProcessType", WfProcessType.Approval); }
            set { Properties.SetValue("ProcessType", value); }
        }

        public string DefaultTaskTitle
        {
            get { return Properties.GetValue("DefaultTaskTitle", string.Empty); }
            set { Properties.SetValue("DefaultTaskTitle", value); }
        }

        public string DefaultNotifyTaskTitle
        {
            get { return Properties.GetValue("DefaultNotifyTaskTitle", string.Empty); }
            set { Properties.SetValue("DefaultNotifyTaskTitle", value); }
        }

        public bool DefaultReturnValue
        {
            get { return Properties.GetValue("DefaultReturnValue", true); }
            set { Properties.SetValue("DefaultReturnValue", value); }
        }

        public IWfActivityDescriptor InitialActivity
        {
            get { return Activities.InitialActivity; }
        }

        public IWfActivityDescriptor CompletedActivity
        {
            get { return Activities.CompletedActivity; }
        }

        /// <summary>
        /// 作废流程时的事件通知人
        /// </summary>
        public WfResourceDescriptorCollection CancelEventReceivers
        {
            get
            {
                if (this._CancelEventReceivers == null)
                    this._CancelEventReceivers = new WfResourceDescriptorCollection(this);

                return this._CancelEventReceivers;
            }
        }

        /// <summary>
        /// 办结流程时的事件通知人
        /// </summary>
        public WfResourceDescriptorCollection CompleteEventReceivers
        {
            get
            {
                if (this._CompleteEventReceivers == null)
                    this._CompleteEventReceivers = new WfResourceDescriptorCollection(this);

                return this._CompleteEventReceivers;
            }
        }

        /// <summary>
        /// 能够启动流程的人
        /// </summary>
        public WfResourceDescriptorCollection ProcessStarters
        {
            get
            {
                string json = this.Properties.GetValue("ProcessStarters", string.Empty);

                WfResourceDescriptorCollection result = null;

                if (json.IsNotEmpty())
                    result = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(json);
                else
                    result = new WfResourceDescriptorCollection();

                return result;
            }
        }

        public string FindNotUsedActivityKey()
        {
            return FindNotUsedActivityKey("N");
        }

        public string FindNotUsedActivityKey(string Prefix)
        {
            int keyNum = 0;

            for (int i = 0; i < this.Activities.Count; i++)
            {
                var activityKey = this.Activities[i].Key.Replace(Prefix, "");
                var parseResult = 0;
                if (!int.TryParse(activityKey, out parseResult))
                {
                    continue;
                }
                if (keyNum < parseResult) keyNum = parseResult;
            }

            return Prefix + ++keyNum;
        }

        public string FindNotUsedTransitionKey()
        {
            int keyNum = 0;
            var parseResult = 0;

            for (int i = 0; i < this.Activities.Count; i++)
            {
                foreach (var tran in this.Activities[i].FromTransitions)
                {
                    var tranKey = tran.Key.Replace("L", "");
                    if (!int.TryParse(tranKey, out parseResult)) continue;

                    if (keyNum < parseResult) keyNum = parseResult;
                }

                foreach (var tran in this.Activities[i].ToTransitions)
                {
                    var tranKey = tran.Key.Replace("L", "");
                    if (!int.TryParse(tranKey, out parseResult)) continue;

                    if (keyNum < parseResult) keyNum = parseResult;
                }
            }

            return "L" + ++keyNum;
        }

        public IWfTransitionDescriptor FindTransitionByKey(string transitionKey)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(transitionKey, "transitionKey");

            IWfTransitionDescriptor result = null;

            foreach (IWfActivityDescriptor actDesp in this.Activities)
            {
                result = actDesp.ToTransitions[transitionKey];

                if (result != null)
                    break;
            }

            return result;
        }

        public string ApplicationName
        {
            get { return Properties.GetValue("ApplicationName", string.Empty); }
            set { Properties.SetValue("ApplicationName", value); }
        }

        public string ProgramName
        {
            get { return Properties.GetValue("ProgramName", string.Empty); }
            set { Properties.SetValue("ProgramName", value); }
        }

        public string Url
        {
            get { return Properties.GetValue("Url", string.Empty); }
            set { Properties.SetValue("Url", value); }
        }

        /// <summary>
        /// 广度遍历所有的活动
        /// </summary>
        /// <param name="activityFunc">每一个活动的回调，如果返回False，则中止遍历</param>
        /// <param name="transitionFunc">每一条线的回调，如果返回False，则忽略该线</param>
        public void ProbeAllActivities(Func<WfProbeActivityArgs, bool> activityFunc, Func<IWfTransitionDescriptor, bool> transitionFunc)
        {
            activityFunc.NullCheck("activityFunc");
            transitionFunc.NullCheck("transitionFunc");

            Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor> elapsedTransitions = new Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor>();

            InnerProbeAllActivities(this.InitialActivity, activityFunc, transitionFunc, 0, elapsedTransitions);
        }

        private static bool InnerProbeAllActivities(IWfActivityDescriptor activityDesp, Func<WfProbeActivityArgs, bool> activityFunc, Func<IWfTransitionDescriptor, bool> transitionFunc, int level, Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor> elapsedTransitions)
        {
            WfProbeActivityArgs activityArgs = new WfProbeActivityArgs() { ActivityDescriptor = activityDesp, Level = level };

            bool result = activityFunc(activityArgs);

            if (result)
            {
                List<IWfTransitionDescriptor> transitionsNeedToProbed = new List<IWfTransitionDescriptor>();

                //广度遍历，先处理所有的线
                foreach (IWfTransitionDescriptor transition in activityDesp.ToTransitions)
                {
                    if (elapsedTransitions.ContainsKey(transition) == false)
                    {
                        elapsedTransitions.Add(transition, transition);

                        if (transitionFunc(transition))
                            transitionsNeedToProbed.Add(transition);
                    }
                }

                foreach (IWfTransitionDescriptor transition in transitionsNeedToProbed)
                {
                    result = InnerProbeAllActivities(transition.ToActivity, activityFunc, transitionFunc, level + 1, elapsedTransitions);

                    if (result == false)
                        break;
                }
            }

            return result;
        }
        #endregion

        protected override PropertyDefineCollection GetPropertyDefineCollection()
        {
            return GetCachedPropertyDefineCollection("WfProcessDescriptor",
                () => PropertyDefineCollection.CreatePropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["BasicProcessProperties"]));
        }

        internal void SetProcessInstance(IWfProcess process)
        {
            this.ProcessInstance = process;

            Activities.ForEach(activity => ((WfActivityDescriptor)activity).SetProcessInstance(process));

            //2011-8-2 徐磊修改  ..流程撤销时,撤销资源没有流程实例.
            this.CancelEventReceivers.ForEach(r => r.SetProcessInstance(process));
            this.CompleteEventReceivers.ForEach(r => r.SetProcessInstance(process));
        }

        /// <summary>
        /// 内部相关人员
        /// </summary>
        public WfResourceDescriptorCollection InternalRelativeUsers
        {
            get
            {
                if (this._InternalRelativeUsers == null)
                    this._InternalRelativeUsers = new WfResourceDescriptorCollection(this);

                return _InternalRelativeUsers;
            }
        }

        /// <summary>
        /// 外部相关人员
        /// </summary>
        public WfExternalUserCollection ExternalUsers
        {
            get
            {
                if (this._ExternalUsers == null)
                    this._ExternalUsers = new WfExternalUserCollection();

                return this._ExternalUsers;
            }
        }

        /// <summary>
        /// 得到主流活动点
        /// </summary>
        /// <returns></returns>
        public WfMainStreamActivityDescriptorCollection GetMainStreamActivities()
        {
            WfMainStreamActivityDescriptorCollection result = new WfMainStreamActivityDescriptorCollection();

            List<WfMainStreamActivityDescriptor> msActivities = new List<WfMainStreamActivityDescriptor>();
            FillMainStreamActivities(this.InitialActivity, msActivities);
            result.CopyFrom(msActivities);

            return result;
        }

        /// <summary>
        /// 是否是流程实例中的主线流程定义
        /// </summary>
        public bool IsMainStream
        {
            get
            {
                return this.Variables.GetValue("MainStream", false);
            }
            set
            {
                this.Variables.SetValue("MainStream", value.ToString(), DataType.Boolean);
            }
        }

        /// <summary>
        /// 得到主流活动点
        /// </summary>
        /// <returns></returns>
        public WfMainStreamActivityDescriptorCollection GetMainStreamForDisplayActivities()
        {
            WfMainStreamActivityDescriptorCollection result = new WfMainStreamActivityDescriptorCollection();

            List<WfMainStreamActivityDescriptor> msActivities = new List<WfMainStreamActivityDescriptor>();
            FillMainStreamForDisplayActivities(this.InitialActivity, msActivities);
            result.CopyFrom(msActivities);

            return result;
        }

        /// <summary>
        /// 流和自动收集参数集合
        /// </summary>
        public WfParameterNeedToBeCollected ParametersNeedToBeCollected
        {
            get
            {
                if (this._ParametersNeedToBeCollected == null)
                    this._ParametersNeedToBeCollected = new WfParameterNeedToBeCollected();

                return this._ParametersNeedToBeCollected;
            }
        }

        /// <summary>
        /// 将所有包含originalUser的WfUserResourceDescriptor的资源替换为包含replaceUsers的一系列资源。
        /// 如果replaceUsers为null或者空集合，则相当于删除原始用户
        /// </summary>
        /// <param name="originalUser"></param>
        /// <param name="replaceUsers"></param>
        /// <returns>被替换的个数</returns>
        public int ReplaceAllUserResourceDescriptors(IUser originalUser, IEnumerable<IUser> replaceUsers)
        {
            int result = 0;

            result += this.CancelEventReceivers.ReplaceAllUserResourceDescriptors(originalUser, replaceUsers);
            result += this.CompleteEventReceivers.ReplaceAllUserResourceDescriptors(originalUser, replaceUsers);
            result += this.InternalRelativeUsers.ReplaceAllUserResourceDescriptors(originalUser, replaceUsers);

            result += this.Activities.ReplaceAllUserResourceDescriptors(originalUser, replaceUsers);

            return result;
        }

        /// <summary>
        /// 撤销通知人撤销前执行服务
        /// </summary>
        public WfServiceOperationDefinitionCollection CancelBeforeExecuteServices
        {
            get
            {
                if (this._CancelBeforeExecuteServices == null)
                    this._CancelBeforeExecuteServices = new WfServiceOperationDefinitionCollection();

                return this._CancelBeforeExecuteServices;
            }
        }

        /// <summary>
        /// 撤销通知人撤销后执行服务
        /// </summary>
        public WfServiceOperationDefinitionCollection CancelAfterExecuteServices
        {
            get
            {
                if (this._CancelAfterExecuteServices == null)
                    this._CancelAfterExecuteServices = new WfServiceOperationDefinitionCollection();

                return this._CancelAfterExecuteServices;
            }
        }

        /// <summary>
        /// 撤销流程时调用服务的Key，逗号分隔
        /// </summary>
        public string CancelExecuteServiceKeys
        {
            get
            {
                return this.Properties.GetValue("CancelExecuteServiceKeys", string.Empty);
            }
            set
            {
                this.Properties.SetValue("CancelExecuteServiceKeys", value);
            }
        }

        public IWfProcessDescriptor Clone()
        {
            XElementFormatter formatter = new XElementFormatter();

            formatter.OutputShortType = WorkflowSettings.GetConfig().OutputShortType;
            formatter.FieldCanXElementSerialize += new FieldCanXElementSerializeHandler(formatter_FieldCanXElementSerialize);

            XElement root = formatter.Serialize(this);

            return (IWfProcessDescriptor)formatter.Deserialize(root);
        }

        /// <summary>
        /// 根据WfCreateActivityParamCollection的值创建节点，覆盖当前的流程
        /// </summary>
        /// <param name="capc"></param>
        /// <param name="overrideInitActivity"></param>
        public void CreateActivities(WfCreateActivityParamCollection capc, bool overrideInitActivity)
        {
            WfActivityDescriptor currentActDesp = (WfActivityDescriptor)this.InitialActivity;

            capc.CreateActivities(this, overrideInitActivity);
        }

        internal void CopyPropertiesTo(WfKeyedDescriptorBase destObject)
        {
            WfProcessDescriptor actDesp = (WfProcessDescriptor)destObject;

            base.CloneProperties(destObject);

            this.CloneCollectionProperties(actDesp);
        }

        private void CloneCollectionProperties(WfProcessDescriptor targetProcessDesp)
        {
            targetProcessDesp.CancelBeforeExecuteServices.Clear();
            targetProcessDesp.CancelBeforeExecuteServices.CopyFrom(this.CancelBeforeExecuteServices, s => s.Clone());
            targetProcessDesp.CancelAfterExecuteServices.Clear();
            targetProcessDesp.CancelAfterExecuteServices.CopyFrom(this.CancelAfterExecuteServices, s => s.Clone());

            targetProcessDesp.CancelEventReceivers.Clear();
            targetProcessDesp.CancelEventReceivers.CopyFrom(this.CancelEventReceivers);

            targetProcessDesp.CompleteEventReceivers.Clear();
            targetProcessDesp.CompleteEventReceivers.CopyFrom(this.CompleteEventReceivers);

            targetProcessDesp.ExternalUsers.Clear();
            targetProcessDesp.ExternalUsers.CopyFrom(this.ExternalUsers);

            targetProcessDesp.InternalRelativeUsers.Clear();
            targetProcessDesp.InternalRelativeUsers.CopyFrom(this.InternalRelativeUsers);

            if (this.ProcessInstance != null)
                targetProcessDesp.ProcessInstance = this.ProcessInstance;

            targetProcessDesp.RelativeLinks.Clear();
            targetProcessDesp.RelativeLinks.CopyFrom(this.RelativeLinks);

            targetProcessDesp.Variables.Clear();
            targetProcessDesp.Variables.CopyFrom(this.Variables, v => v.Clone());

            targetProcessDesp.ParametersNeedToBeCollected.Clear();
            targetProcessDesp.ParametersNeedToBeCollected.CopyFrom(this.ParametersNeedToBeCollected);
        }

        private static bool formatter_FieldCanXElementSerialize(ExtendedFieldInfo efi)
        {
            //Clone时，不序列化实例
            return (efi.FieldInfo.Name != "_ProcessInstance");
        }

        private void FillMainStreamActivities(IWfActivityDescriptor actDesp, List<WfMainStreamActivityDescriptor> msActivities)
        {
            Dictionary<string, IWfActivityDescriptor> elapsedActivities = new Dictionary<string, IWfActivityDescriptor>();

            this.InnerFillMainStreamActivities(actDesp, msActivities, elapsedActivities);
        }

        private void InnerFillMainStreamActivities(IWfActivityDescriptor actDesp, List<WfMainStreamActivityDescriptor> msActivities, Dictionary<string, IWfActivityDescriptor> elapsedActivities)
        {
            if (elapsedActivities.ContainsKey(actDesp.Key))
                return;

            elapsedActivities.Add(actDesp.Key, actDesp);

            string actKey = actDesp.GetAssociatedActivity() != null ? actDesp.GetAssociatedActivity().Key : actDesp.Key;

            int position = -1;

            for (int i = msActivities.Count - 1; i >= 0; i--)
            {
                if (msActivities[i].Activity.Key == actKey || msActivities[i].Activity.AssociatedActivityKey == actKey)
                {
                    position = i;
                    break;
                }
            }

            if (position >= 0)	//替换为最新的活动
                msActivities[position] = new WfMainStreamActivityDescriptor(actDesp);
            else
                msActivities.Add(new WfMainStreamActivityDescriptor(actDesp));

            //2012/5/2，也许需要调整一下次序，符合条件的优先，然后是经过的线
            //查找经过的线
            IWfTransitionDescriptor transition = null;

            //如果流程描述，不是实例中保存的主线流程描述，则从经过的线查找
            if (this.Variables.GetValue("MainStream", false) == false)
                transition = actDesp.ToTransitions.FindElapsedTransition();

            if (transition != null && transition.IsBackward == true)
                transition = null;

            if (transition == null)
            {
                //如果经过的线不存在，则查找缺省的线或优先级最高的线
                WfTransitionDescriptorCollection transitions = actDesp.ToTransitions.GetAllCanTransitForwardTransitions();
                transition = transitions.FindDefaultSelectTransition();
            }

            if (transition == null)
                transition = actDesp.ToTransitions.FindDefaultSelectTransition();

            if (transition != null && transition.IsBackward == true)
            {
                transition = null;
                transition = actDesp.ToTransitions.Find(t => t.IsBackward == false);
            }

            if (transition != null)
                InnerFillMainStreamActivities(transition.ToActivity, msActivities, elapsedActivities);
        }

        private void FillMainStreamForDisplayActivities(IWfActivityDescriptor actDesp, List<WfMainStreamActivityDescriptor> msActivities)
        {
            string actKey = string.Empty;
            if (!string.IsNullOrEmpty(actDesp.AssociatedActivityKey))
                actKey = actDesp.AssociatedActivityKey;
            else
                actKey = actDesp.Key;

            List<WfMainStreamActivityDescriptor> cloneActivities = new List<WfMainStreamActivityDescriptor>();
            foreach (var innerAct in msActivities)
            {
                if (innerAct.Activity.Key == actKey || (innerAct.Activity.AssociatedActivityKey != null && innerAct.Activity.AssociatedActivityKey == actKey))
                {
                    cloneActivities.Add(new WfMainStreamActivityDescriptor(actDesp));
                    break;
                }
                cloneActivities.Add(innerAct);
            }
            msActivities.Clear();
            cloneActivities.ForEach(c => msActivities.Add(c));
            if (msActivities.Find(act => act.Activity.Key == actKey || (act.Activity.AssociatedActivityKey != null && act.Activity.AssociatedActivityKey == actKey)) == null)
                msActivities.Add(new WfMainStreamActivityDescriptor(actDesp));

            //查找经过的线
            IWfTransitionDescriptor transition = actDesp.ToTransitions.FindElapsedTransition();
            if (transition != null && transition.IsBackward == true)
                transition = null;
            if (transition == null)
            {
                //如果经过的线不存在，则查找缺省的线或优先级最高的线
                WfTransitionDescriptorCollection transitions = actDesp.ToTransitions.GetAllCanTransitForwardTransitions();
                transition = transitions.FindDefaultSelectTransition();
            }

            if (transition == null)
            {
                transition = actDesp.ToTransitions.FindDefaultSelectTransition();
            }
            if (transition != null && transition.IsBackward == true)
            {
                transition = null;
                transition = actDesp.ToTransitions.Find(t => t.IsBackward == false);
            }

            if (transition != null)
            {
                bool flag = msActivities.Find(a => a.Activity.Key == transition.ToActivity.Key) == null;

                if (flag)
                    FillMainStreamForDisplayActivities(transition.ToActivity, msActivities);
            }
        }

        public override void MergeDefinedProperties()
        {
            base.MergeDefinedProperties();

            foreach (WfActivityDescriptor actDesp in this.Activities)
                actDesp.MergeDefinedProperties();
        }

        public override void SyncPropertiesToFields()
        {
            this.Variables.SyncPropertiesToFields(this.Properties["Variables"], ReservedVariableNames);
            this.CancelBeforeExecuteServices.SyncPropertiesToFields(this.Properties["CancelBeforeExecuteServices"]);
            this.CancelAfterExecuteServices.SyncPropertiesToFields(this.Properties["CancelAfterExecuteServices"]);

            this.CancelEventReceivers.SyncPropertiesToFields(this.Properties["CancelEventReceivers"]);
            this.CompleteEventReceivers.SyncPropertiesToFields(this.Properties["CompleteEventReceivers"]);

            this.InternalRelativeUsers.SyncPropertiesToFields(this.Properties["InternalRelativeUsers"]);
            this.ExternalUsers.SyncPropertiesToFields(this.Properties["ExternalUsers"]);

            this.RelativeLinks.SyncPropertiesToFields(this.Properties["RelativeLinks"]);
            this.ParametersNeedToBeCollected.SyncPropertiesToFields(this.Properties["ParametersNeedToBeCollected"]);
        }

        #region ISimpleXmlSerializer Members

        void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
        {
            element.NullCheck("element");

            ((ISimpleXmlSerializer)this.Properties).ToXElement(element, refNodeName);

            ((ISimpleXmlSerializer)this.Activities).ToXElement(element, refNodeName);

            if (this.CancelEventReceivers.Count > 0)
                ((ISimpleXmlSerializer)this.CancelEventReceivers).ToXElement(element, "CancelEventReceivers");

            if (this.CompleteEventReceivers.Count > 0)
                ((ISimpleXmlSerializer)this.CompleteEventReceivers).ToXElement(element, "CompleteEventReceivers");

            if (this.CancelBeforeExecuteServices.Count > 0)
                ((ISimpleXmlSerializer)this.CancelBeforeExecuteServices).ToXElement(element, "CancelBeforeExecuteServices");

            if (this.CancelAfterExecuteServices.Count > 0)
                ((ISimpleXmlSerializer)this.CancelAfterExecuteServices).ToXElement(element, "CancelAfterExecuteServices");

            if (this.RelativeLinks.Count > 0)
                ((ISimpleXmlSerializer)this.RelativeLinks).ToXElement(element, "RelativeLinks");
        }

        #endregion
    }
}
