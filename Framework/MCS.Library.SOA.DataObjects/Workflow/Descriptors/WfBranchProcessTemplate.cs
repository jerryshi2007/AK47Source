using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Expression;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow.Builders;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 分支流程的模板定义
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class WfBranchProcessTemplateDescriptor : WfKeyedDescriptorBase, IWfBranchProcessTemplateDescriptor, ISimpleXmlSerializer
    {
        private WfResourceDescriptorCollection _Resources = null;
        private IWfProcessDescriptor _PredefinedProcessDescriptor = null;

        private WfResourceDescriptorCollection _CancelSubProcessNotifier = null;
        private WfConditionDescriptor _Condition = null;
        private WfServiceOperationDefinition _OperationDefinition = null;
        private WfRelativeLinkDescriptorCollection _RelativeLinks = null;

        public WfBranchProcessTemplateDescriptor()
        {
        }

        public WfBranchProcessTemplateDescriptor(string key)
            : base(key)
        {
        }

        #region IWfBranchProcessTemplate Members

        public string BranchProcessKey
        {
            get { return Properties.GetValue("BranchProcessKey", string.Empty); }
            set { Properties.SetValue("BranchProcessKey", value); }
        }

        public IWfProcessDescriptor PredefinedProcessDescriptor
        {
            get { return this._PredefinedProcessDescriptor; }
            set { this._PredefinedProcessDescriptor = value; }
        }

        public WfBranchProcessExecuteSequence ExecuteSequence
        {
            get { return Properties.GetValue("ExecuteSequence", WfBranchProcessExecuteSequence.Parallel); }
            set { Properties.SetValue("ExecuteSequence", value); }
        }

        public WfBranchProcessBlockingType BlockingType
        {
            get { return Properties.GetValue("BlockingType", WfBranchProcessBlockingType.WaitAllBranchProcessesComplete); }
            set { Properties.SetValue("BlockingType", value); }
        }

        public WfSubProcessApprovalMode SubProcessApprovalMode
        {
            get { return Properties.GetValue("SubProcessApprovalMode", WfSubProcessApprovalMode.NoActivityDecide); }
            set { Properties.SetValue("SubProcessApprovalMode", value); }
        }

        public string DefaultProcessName
        {
            get { return Properties.GetValue("DefaultProcessName", string.Empty); }
            set { Properties.SetValue("DefaultProcessName", value); }
        }

        public string DefaultUrl
        {
            get { return Properties.GetValue("DefaultUrl", string.Empty); }
            set { Properties.SetValue("DefaultUrl", value); }
        }

        public string DefaultTaskTitle
        {
            get { return Properties.GetValue("DefaultTaskTitle", string.Empty); }
            set { Properties.SetValue("DefaultTaskTitle", value); }
        }

        public WfResourceDescriptorCollection Resources
        {
            get
            {
                if (this._Resources == null)
                    this._Resources = new WfResourceDescriptorCollection(this);

                return this._Resources;
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

        public WfResourceDescriptorCollection CancelSubProcessNotifier
        {
            get
            {
                if (this._CancelSubProcessNotifier == null)
                    this._CancelSubProcessNotifier = new WfResourceDescriptorCollection(this);

                return this._CancelSubProcessNotifier;
            }
        }

        public WfServiceOperationDefinition OperationDefinition
        {
            get
            {
                return _OperationDefinition;
            }
            set
            {
                _OperationDefinition = value;
            }
        }

        public IWfBranchProcessTemplateDescriptor Clone()
        {
            WfBranchProcessTemplateDescriptor template = new WfBranchProcessTemplateDescriptor();

            this.CloneProperties(template);

            return template;
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

            result += this.Resources.ReplaceAllUserResourceDescriptors(originalUser, replaceUsers);
            result += this.CancelSubProcessNotifier.ReplaceAllUserResourceDescriptors(originalUser, replaceUsers);

            return result;
        }

        public IWfProcessDescriptor GetBranchProcessDescriptor()
        {
            IWfProcessDescriptor result = this.PredefinedProcessDescriptor;

            if (result == null)
            {
                if (this.BranchProcessKey.IsNotEmpty())
                    result = WfProcessDescriptorManager.GetDescriptor(this.BranchProcessKey);
                else
                {
                    this.Key.CheckStringIsNullOrEmpty<WfRuntimeException>("加载分支子流程时，Template的BranchProcessKey不能和Key不能为空");
                    result = WfProcessDescriptorManager.GetDescriptor(this.Key);
                }

                if (this.ExecuteSequence == WfBranchProcessExecuteSequence.SerialInSameProcess)
                {
                    //Activity Matrix
                    WfProcessBuilderInfo builderInfo = WfProcessDescriptorManager.GetBuilderInfo(this.BranchProcessKey);

                    PropertyDefineCollection definedProperties = null;

                    if (builderInfo == null)
                        definedProperties = new PropertyDefineCollection();
                    else
                        definedProperties = builderInfo.Builder.GetDefinedProperties();

                    //生成活动
                    result.CreateActivities(this.Resources.ToCreateActivityParams(definedProperties), true);
                }

                AutoAdjustAgreeTransitions(result);

                //todo:ydz 2012-07-21
                this.SetBranchProcessActivityEditMode(result);
                //todo:ydz 2012-07-21
                this.CancelSubProcessNotifier.ForEach(wfr =>
                {
                    result.CancelEventReceivers.Add(wfr);
                });
            }

            return result;
        }

        private void SetBranchProcessActivityEditMode(IWfProcessDescriptor subProcessDesp)
        {
            WfSubProcessActivityEditMode currentEditMode = this.Properties.GetValue("AllowEditActivities", WfSubProcessActivityEditMode.Default);

            if (currentEditMode != WfSubProcessActivityEditMode.Default)
            {
                bool changeValue = false;

                if (currentEditMode == WfSubProcessActivityEditMode.AllowEdit)
                    changeValue = true;

                subProcessDesp.Activities.ForEach(ac =>
                {
                    ac.Properties.SetValue("AllowToBeAppended", changeValue);
                    ac.Properties.SetValue("AllowToBeModified", changeValue);
                    ac.Properties.SetValue("AllowToBeDeleted", changeValue);
                });
            }
        }

        public WfConditionDescriptor Condition
        {
            get
            {
                if (this._Condition == null)
                    this._Condition = new WfConditionDescriptor(this);

                return this._Condition;
            }
            set
            {
                this._Condition = value;
                this._Condition.Owner = this;
            }
        }

        public bool CanStart()
        {
            try
            {
                return this.Enabled && (this.Condition != null && this.Condition.Evaluate(new CalculateUserFunction(WfRuntime.ProcessContext.FireEvaluateBranchTemplateCondition)));
            }
            catch (System.Exception ex)
            {
                throw new WfConditionEvaluationException(string.Format("判断分支流程{0}:{1}的启动条件的条件，{2}", this.Key, this.Name, ex.Message), this.Condition);
            }
        }

        /// <summary>
        /// 调整同意和不同意的线
        /// </summary>
        /// <param name="result"></param>
        private void AutoAdjustAgreeTransitions(IWfProcessDescriptor processDesp)
        {
            IWfActivityDescriptor currentActDesp = processDesp.InitialActivity;
            IWfTransitionDescriptor transition = currentActDesp.ToTransitions.GetAllCanTransitForwardTransitions().FindDefaultSelectTransition();

            while (transition != null && currentActDesp.ActivityType != WfActivityType.CompletedActivity)
            {
                IWfActivityDescriptor nextActDesp = transition.ToActivity;

                if (currentActDesp.Variables.GetValue(WfProcessBuilderBase.AutoBuiltActivityVariableName, false))
                {
                    if (this.SubProcessApprovalMode == WfSubProcessApprovalMode.AnyActivityDecide ||
                        (this.SubProcessApprovalMode == WfSubProcessApprovalMode.LastActivityDecide && nextActDesp.ActivityType == WfActivityType.CompletedActivity))
                    {
                        currentActDesp.ToTransitions.RemoveByToActivity(nextActDesp);
                        //这句可以不要，RemoveByToActivity清除了相关的线
                        //nextActDesp.FromTransitions.Remove(t => string.Compare(t.Key, transition.Key) == 0);

                        AddAgreeAndDisagreeLine(currentActDesp, nextActDesp, this);
                    }
                }

                currentActDesp = nextActDesp;

                transition = currentActDesp.ToTransitions.GetAllCanTransitForwardTransitions().FindDefaultSelectTransition();
            }
        }

        /// <summary>
        /// 添加同意和不同意的出线
        /// </summary>
        /// <param name="currentActDesp"></param>
        private static void AddAgreeAndDisagreeLine(IWfActivityDescriptor currentActDesp, IWfActivityDescriptor nextActDesp, IWfBranchProcessTemplateDescriptor template)
        {
            IWfActivityDescriptor completedActDesp = currentActDesp.Process.CompletedActivity;

            WfTransitionDescriptor agreeTransition = (WfTransitionDescriptor)currentActDesp.ToTransitions.AddForwardTransition(nextActDesp);
            agreeTransition.Name = template.Properties.GetValue("AgreeLineName", "同意");
            agreeTransition.DefaultSelect = true;
            agreeTransition.AffectProcessReturnValue = true;
            agreeTransition.AffectedProcessReturnValue = true;
            agreeTransition.Priority = 0;

            WfTransitionDescriptor disagreeTransition = (WfTransitionDescriptor)currentActDesp.ToTransitions.AddForwardTransition(completedActDesp);
            disagreeTransition.Name = template.Properties.GetValue("DisagreeLineName", "不同意");
            disagreeTransition.Priority = 1;
            disagreeTransition.AffectProcessReturnValue = true;
            disagreeTransition.AffectedProcessReturnValue = false;
        }

        internal protected override void CloneProperties(WfKeyedDescriptorBase destObject)
        {
            base.CloneProperties(destObject);

            WfBranchProcessTemplateDescriptor template = (WfBranchProcessTemplateDescriptor)destObject;

            template.Resources.Clear();
            template.Resources.CopyFrom(this.Resources);

            template.CancelSubProcessNotifier.Clear();
            template.CancelSubProcessNotifier.CopyFrom(this.CancelSubProcessNotifier);

            template.Condition = this.Condition.Clone();

            if (this._OperationDefinition != null)
                template._OperationDefinition = this._OperationDefinition.Clone();

            template.RelativeLinks.Clear();
            template.RelativeLinks.CopyFrom(this.RelativeLinks);
        }

        protected override PropertyDefineCollection GetPropertyDefineCollection()
        {
            return GetCachedPropertyDefineCollection("WfBranchProcessTemplateDescriptor",
                () => PropertyDefineCollection.CreatePropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["BasicBranchProcessTemplateProperties"]));
        }
        #endregion

        #region ISimpleXmlSerializer Members

        void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
        {
            if (refNodeName.IsNotEmpty())
                element = element.AddChildElement(refNodeName);

            ((ISimpleXmlSerializer)this.Properties).ToXElement(element, string.Empty);

            if (this.Resources.Count > 0)
                ((ISimpleXmlSerializer)this.Resources).ToXElement(element, "Resources");

            if (this.CancelSubProcessNotifier.Count > 0)
                ((ISimpleXmlSerializer)this.CancelSubProcessNotifier).ToXElement(element, "CancelSubProcessNotifier");

            if (this.Condition.IsEmpty == false)
                ((ISimpleXmlSerializer)this.Condition).ToXElement(element, "Condition");

            if (this.OperationDefinition != null)
                ((ISimpleXmlSerializer)this.OperationDefinition).ToXElement(element, "OperationDefinition");

            if (this.RelativeLinks.Count > 0)
                ((ISimpleXmlSerializer)this.RelativeLinks).ToXElement(element, "RelativeLinks");
        }

        #endregion
    }

    [Serializable]
    [XElementSerializable]
    public class WfBranchProcessTemplateCollection : WfKeyedDescriptorCollectionBase<IWfBranchProcessTemplateDescriptor>, ISimpleXmlSerializer
    {
        internal WfBranchProcessTemplateCollection(IWfDescriptor owner)
            : base(owner)
        {
        }

        internal WfBranchProcessTemplateCollection()
            : base(null)
        {
        }

        protected WfBranchProcessTemplateCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        /// <summary>
        /// 查找没有使用的模板的Key，默认以前缀"T"开始
        /// </summary>
        /// <returns></returns>
        public string FindNotUsedTemplateKey()
        {
            return FindNotUsedTemplateKey("T");
        }

        /// <summary>
        /// 查找没有使用的模板的Key
        /// </summary>
        /// <param name="prefix">Key的前缀</param>
        /// <returns></returns>
        public string FindNotUsedTemplateKey(string prefix)
        {
            prefix.CheckStringIsNullOrEmpty("prefix");

            int i = 0;

            string result = string.Empty;

            while (true)
            {
                result = prefix + i;

                if (this.ContainsKey(result) == false)
                    break;

                i++;
            }

            return result;
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

            foreach (IWfBranchProcessTemplateDescriptor template in this)
                result += template.ReplaceAllUserResourceDescriptors(originalUser, replaceUsers);

            return result;
        }

        /// <summary>
        /// 查找空资源的默认审批流
        /// </summary>
        /// <returns></returns>
        internal IWfBranchProcessTemplateDescriptor FindEmptyResourceDefaultApprovalProcessTemplate()
        {
            IWfBranchProcessTemplateDescriptor result = null;

            foreach (IWfBranchProcessTemplateDescriptor t in this)
            {
                if (t.BranchProcessKey == WfProcessDescriptorManager.DefaultApprovalProcessKey)
                {
                    result = t;
                    break;
                }
            }

            return result;
        }

        #region ISimpleXmlSerializer Members

        void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
        {
            if (refNodeName.IsNotEmpty())
                element = element.AddChildElement(refNodeName);

            foreach (WfBranchProcessTemplateDescriptor bptd in this)
            {
                XElement subRoot = element.AddChildElement("Template");

                ((ISimpleXmlSerializer)bptd).ToXElement(subRoot, string.Empty);
            }
        }

        #endregion
    }
}
