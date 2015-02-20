using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Expression;
using MCS.Library.Globalization;
using MCS.Library.SOA.DataObjects.Workflow.Builders;


namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 线基本属性
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public abstract class WfTransitionDescriptor : WfKeyedDescriptorBase, IWfTransitionDescriptor
    {
        private static readonly string[] ReservedVariableNames = new string[] { "GeneratedByTemplate", "TemplateKey", "IsDynamicActivityTransition" };

        private IWfActivityDescriptor _FromActivity;

        private IWfActivityDescriptor _ToActivity;

        private WfVariableDescriptorCollection _Variables = null;

        private string _FromActivityKey;
        private string _ToActivityKey;

        //流程设计器保存时使用该属性，导致WCF转换后无效属性，暂时打开。
        //徐磊 2011-4-6
        public string FromActivityKey
        {
            get { return _FromActivityKey; }
            set { _FromActivityKey = value; }
        }

        public string ToActivityKey
        {
            get { return _ToActivityKey; }
            set { _ToActivityKey = value; }
        }

        protected WfTransitionDescriptor()
            : base()
        {
        }
        /// <summary>
        /// 给线赋予Key值
        /// </summary>
        /// <param name="key">Key值</param>

        protected WfTransitionDescriptor(string key)
            : base(key)
        {
        }
        /// <summary>
        /// Priority属性访问器
        /// </summary>
        /// <remarks>Priority属性访问器</remarks>

        public int Priority
        {
            get { return Properties.GetValue("Priority", 0); }
            set { Properties.SetValue("Priority", value); }
        }

        /// <summary>
        /// ToActivity属性访问器
        /// </summary>
        /// <remarks>ToActivity属性访问器</remarks>
        public IWfActivityDescriptor ToActivity
        {
            get
            {
                return _ToActivity;
            }
            set
            {
                _ToActivity = value;
            }
        }

        /// <summary>
        /// FromActivity属性访问器
        /// </summary>
        /// <remarks>FromActivity属性访问器</remarks>

        public IWfActivityDescriptor FromActivity
        {
            get
            {
                return _FromActivity;
            }
            set
            {
                _FromActivity = value;
            }
        }

        /// <summary>
        /// 由模板活动生成的活动标志
        /// </summary>
        public bool GeneratedByTemplate
        {
            get
            {
                return this.Variables.GetValue("GeneratedByTemplate", false);
            }
            set
            {
                this.Variables.SetValue("GeneratedByTemplate", value.ToString(), DataType.Boolean);
            }
        }

        /// <summary>
        /// 生成此活动的动态模板活动的Key
        /// </summary>
        public string TemplateKey
        {
            get
            {
                return this.Variables.GetValue("TemplateKey", string.Empty);
            }
            set
            {
                this.Variables.SetValue("TemplateKey", value);
            }
        }

        public WfVariableDescriptorCollection Variables
        {
            get
            {
                if (this._Variables == null)
                    this._Variables = new WfVariableDescriptorCollection(this);

                return _Variables;
            }
        }

        public bool AffectProcessReturnValue
        {
            get { return Properties.GetValue("AffectProcessReturnValue", false); }
            set { Properties.SetValue("AffectProcessReturnValue", value); }
        }

        public bool AffectedProcessReturnValue
        {
            get { return Properties.GetValue("AffectedProcessReturnValue", false); }
            set { Properties.SetValue("AffectedProcessReturnValue", value); }
        }

        public bool DefaultSelect
        {
            get { return Properties.GetValue("DefaultSelect", false); }
            set { Properties.SetValue("DefaultSelect", value); }
        }

        /// <summary>
        /// 是否是退回线
        /// </summary>
        public bool IsBackward
        {
            get { return Properties.GetValue("IsReturn", false); }
            set { Properties.SetValue("IsReturn", value); }
        }

        /// <summary>
        /// 是否是动态活动的进线或出线
        /// </summary>
        public bool IsDynamicActivityTransition
        {
            get
            {
                return this.Variables.GetValue("IsDynamicActivityTransition", false);
            }
            set
            {
                WfVariableDescriptor variable = null;

                if (this.Variables.ContainsKey("IsDynamicActivityTransition"))
                {
                    variable = this.Variables["IsDynamicActivityTransition"];
                    variable.OriginalValue = value.ToString();
                }
                else
                {
                    variable = new WfVariableDescriptor("IsDynamicActivityTransition", value.ToString(), DataType.Boolean);
                    this.Variables.AddNotExistsItem(variable);
                }
            }
        }

        /// <summary>
        /// 能够流转。Enabled为True且ConditionMatched为True
        /// </summary>
        /// <returns></returns>
        public virtual bool CanTransit()
        {
            return this.Enabled && this.ConditionMatched();
        }

        /// <summary>
        /// 符合线上的条件
        /// </summary>
        /// <returns></returns>
        public virtual bool ConditionMatched()
        {
            return true;
        }

        private WfConditionDescriptor _Condition = null;

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 设置线的FromActivity和ToActivity的相关属性，但是不影响fromActivity的ToTransitions以及toActivity的FromTransition的集合
        /// </summary>
        /// <param name="fromActivity"></param>
        /// <param name="toActivity"></param>
        internal protected virtual void JoinActivity(IWfActivityDescriptor fromActivity, IWfActivityDescriptor toActivity)
        {
            this._FromActivity = fromActivity;
            this._ToActivity = toActivity;

            this._FromActivityKey = fromActivity.Key;
            this._ToActivityKey = toActivity.Key;
        }

        /// <summary>
        /// 设置线的FromActivity和ToActivity的相关属性，并且影响fromActivity的ToTransitions以及toActivity的FromTransition的集合
        /// </summary>
        /// <param name="fromActivity"></param>
        /// <param name="toActivity"></param>
        public void ConnectActivities(IWfActivityDescriptor fromActivity, IWfActivityDescriptor toActivity)
        {
            fromActivity.NullCheck("fromActivity");
            toActivity.NullCheck("toActivity");

            this.JoinActivity(fromActivity, toActivity);

            if (fromActivity.ToTransitions.Contains(this) == false)
                fromActivity.ToTransitions.Add(this);

            if (toActivity.FromTransitions.Contains(this) == false)
                toActivity.FromTransitions.Add(this);
        }

        public override void SyncPropertiesToFields()
        {
            this.Condition.SyncPropertiesToFields(this.Properties["Condition"]);
            this.Variables.SyncPropertiesToFields(this.Properties["Variables"], ReservedVariableNames);
        }

        protected override PropertyDefineCollection GetPropertyDefineCollection()
        {
            return GetCachedPropertyDefineCollection("WfTransitionDescriptor",
                () => PropertyDefineCollection.CreatePropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["BasicTransitionProperties"]));
        }

        #region IComparable<IWfTransitionDescriptor> Members

        public int CompareTo(IWfTransitionDescriptor other)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(other != null, "other");

            return this.Priority.CompareTo(other.Priority);
        }
        #endregion

        internal protected override void CloneProperties(WfKeyedDescriptorBase destObject)
        {
            string originalKey = destObject.Key;

            base.CloneProperties(destObject);

            WfForwardTransitionDescriptor transition = (WfForwardTransitionDescriptor)destObject;

            if (originalKey.IsNullOrEmpty() && this.ProcessInstance != null)
                transition.Key = this.ProcessInstance.Descriptor.FindNotUsedTransitionKey();
            else
                transition.Key = originalKey;

            transition.Condition = this.Condition.Clone();
            transition.Variables.CopyFrom(this.Variables, v => v.Clone());
        }

        public virtual IWfTransitionDescriptor Clone()
        {
            WfForwardTransitionDescriptor transition = new WfForwardTransitionDescriptor();

            CloneProperties(transition);

            return transition;
        }
    }
    /// <summary>
    /// 前向线属性
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class WfForwardTransitionDescriptor : WfTransitionDescriptor, IWfForwardTransitionDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        public WfForwardTransitionDescriptor()
            : base()
        {
        }

        /// <summary>
        /// 前向线Key
        /// </summary>
        /// <param name="key">Key</param>
        /// <remarks>得到前向线的Key值</remarks>

        public WfForwardTransitionDescriptor(string key)
            : base(key)
        {
        }

        /// <summary>
        /// 线上条件是否满足
        /// </summary>
        /// <returns></returns>
        public override bool ConditionMatched()
        {
            try
            {
                return this.Condition != null && this.Condition.Evaluate(new CalculateUserFunction(WfRuntime.ProcessContext.FireEvaluateTransitionCondition));
            }
            catch (System.Exception ex)
            {
                throw new WfTransitionEvaluationException(string.Format("判断线{0}:{1}的条件，{2}", this.Key, this.Name, ex.Message), this.Condition);
            }
        }
    }

    /// <summary>
    /// 后向线属性
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class WfBackwardTransitionDescriptor : WfTransitionDescriptor, IWfBackwardTransitionDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        internal WfBackwardTransitionDescriptor()
            : base()
        {
        }
        /// <summary>
        /// 后向线Key
        /// </summary>
        /// <param name="key">Key</param>
        public WfBackwardTransitionDescriptor(string key)
            : base(key)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public abstract class WfTransitionDescriptorCollection : WfKeyedDescriptorCollectionBase<IWfTransitionDescriptor>
    {
        private WfTransitionDescriptorCollection()
            : base(null)
        {
        }

        public WfTransitionDescriptorCollection(IWfDescriptor owner)
            : base(owner)
        {
        }

        protected WfTransitionDescriptorCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        /// <summary>
        /// 创建一个新的集合
        /// </summary>
        /// <returns></returns>
        internal protected abstract WfTransitionDescriptorCollection CreateNewCollection();

        /// <summary>
        /// 得到流转时已经经过的线
        /// </summary>
        /// <returns></returns>
        public IWfTransitionDescriptor FindElapsedTransition()
        {
            IWfTransitionDescriptor result = null;

            List<IWfTransitionDescriptor> runningOrCompletedTransitions = new List<IWfTransitionDescriptor>();

            foreach (IWfTransitionDescriptor t in this)
            {
                if (t.ToActivity.ProcessInstance != null)
                {
                    IWfActivity runtimeToActivity = t.ToActivity.ProcessInstance.Activities.FindActivityByDescriptorKey(t.ToActivity.Key);

                    (runtimeToActivity != null).FalseThrow<WfRuntimeException>("流程活动{0}能够在流程定义中找到，但是在运行态的流程活动中不存在", t.ToActivity.Key);

                    if (runtimeToActivity.Status != WfActivityStatus.NotRunning
                        && t.FromActivity.Instance.Status != WfActivityStatus.NotRunning
                        && t.FromActivity.Instance.StartTime <= runtimeToActivity.StartTime)
                    {
                        runningOrCompletedTransitions.Add(t);
                    }
                }
            }

            DateTime minStartTime = DateTime.MaxValue;

            foreach (IWfTransitionDescriptor t in runningOrCompletedTransitions)
            {
                IWfActivity actInstance = t.ToActivity.Instance;

                if (actInstance.StartTime < minStartTime)
                {
                    minStartTime = actInstance.StartTime;
                    result = t;
                }
            }

            return result;
        }

        /// <summary>
        /// 得到默认选择的线。如果没有默认选择，则使用最高优先级的，此时不包含禁用的线。
        /// </summary>
        /// <returns></returns>
        public IWfTransitionDescriptor FindDefaultSelectTransition()
        {
            return FindDefaultSelectTransition(false);
        }

        /// <summary>
        /// 得到默认选择的线。如果没有默认选择，则使用最高优先级的
        /// </summary>
        /// <param name="includeDisabled">是否包含禁用的线</param>
        /// <returns></returns>
        public IWfTransitionDescriptor FindDefaultSelectTransition(bool includeDisabled)
        {
            IWfTransitionDescriptor transition = null;

            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].DefaultSelect)
                {
                    transition = this[i];
                    break;
                }
            }

            if (transition == null)
                transition = FindHighestPriorityTransition(includeDisabled);

            return transition;
        }

        /// <summary>
        /// 找到优先级最高的线
        /// </summary>
        /// <param name="includeDisabled"></param>
        /// <returns></returns>
        public IWfTransitionDescriptor FindHighestPriorityTransition(bool includeDisabled)
        {
            IWfTransitionDescriptor result = null;

            int maxPriority = int.MaxValue;

            foreach (IWfTransitionDescriptor transition in this)
            {
                bool needCheck = includeDisabled || transition.Enabled;

                if (needCheck)
                {
                    if (transition.Priority < maxPriority)
                    {
                        result = transition;
                        maxPriority = transition.Priority;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actKey"></param>
        public void RemoveByFromActivity(string actKey)
        {
            List<IWfTransitionDescriptor> list = new List<IWfTransitionDescriptor>();

            foreach (IWfTransitionDescriptor transition in base.List)
                if (string.Compare(transition.FromActivity.Key, actKey, true) == 0)
                    list.Add(transition);

            foreach (IWfTransitionDescriptor transition in list)
                Remove(transition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actDesp"></param>
        public void RemoveByFromActivity(IWfActivityDescriptor actDesp)
        {
            if (actDesp != null)
                RemoveByFromActivity(actDesp.Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actKey"></param>
        public void RemoveByToActivity(string actKey)
        {
            List<IWfTransitionDescriptor> list = new List<IWfTransitionDescriptor>();

            foreach (IWfTransitionDescriptor transition in base.List)
                if (string.Compare(transition.ToActivity.Key, actKey, true) == 0)
                    list.Add(transition);

            foreach (IWfTransitionDescriptor transition in list)
                this.Remove(transition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actDesp"></param>
        public void RemoveByToActivity(IWfActivityDescriptor actDesp)
        {
            if (actDesp != null)
                RemoveByToActivity(actDesp.Key);
        }

        public override bool Remove(IWfTransitionDescriptor transition)
        {
            bool result = false;

            if (transition != null)
            {
                Remove(t => t == transition);
                RemoveRelativeTransition(transition);
            }

            return result;
        }

        /// <summary>
        /// 查找没有使用的优先级(从8以后开始)
        /// </summary>
        /// <returns></returns>
        public int FindNotUsedPriority()
        {
            int result = 8;

            while (true)
            {
                bool notUsed = true;

                foreach (IWfTransitionDescriptor transition in base.List)
                {
                    if (transition.Priority == result)
                    {
                        notUsed = false;
                        result++;
                        break;
                    }
                }

                if (notUsed)
                    break;
            }

            return result;
        }

        /// <summary>
        /// 得到可以抵达的线
        /// </summary>
        /// <returns></returns>
        public WfTransitionDescriptorCollection GetAllCanTransitTransitions()
        {
            return GetAllCanTransitTransitions(true);
        }

        /// <summary>
        /// 得到可以抵达的线
        /// </summary>
        /// <param name="sortByDefaultSelect">是否按照DefaultSelect排序</param>
        /// <returns></returns>
        public WfTransitionDescriptorCollection GetAllCanTransitTransitions(bool sortByDefaultSelect)
        {
            WfTransitionDescriptorCollection result = CreateNewCollection();

            foreach (WfTransitionDescriptor transition in this)
            {
                try
                {
                    if (transition.CanTransit())
                        result.Add(transition);
                }
                catch (WfTransitionEvaluationException ex)
                {
                    ex.WriteToLog();
                }
            }

            result.Sort(sortByDefaultSelect);

            return result;
        }

        /// <summary>
        /// 得到所有符合条件的线。无论是否Enabled
        /// </summary>
        /// <returns></returns>
        public WfTransitionDescriptorCollection GetAllConditionMatchedTransitions()
        {
            return GetAllConditionMatchedTransitions(true);
        }

        /// <summary>
        /// 得到所有符合条件的线。无论是否Enabled
        /// </summary>
        /// <param name="sortByDefaultSelect">是否优先按照DefaultSelect排序</param>
        /// <returns></returns>
        public WfTransitionDescriptorCollection GetAllConditionMatchedTransitions(bool sortByDefaultSelect)
        {
            WfTransitionDescriptorCollection result = CreateNewCollection();

            foreach (IWfTransitionDescriptor transition in this)
            {
                try
                {
                    if (transition.ConditionMatched())
                        result.Add(transition);
                }
                catch (WfTransitionEvaluationException ex)
                {
                    ex.WriteToLog();
                }
            }

            result.Sort(sortByDefaultSelect);

            return result;
        }

        /// <summary>
        /// 排序。如果sortByDefaultSelect为True，则优先按照DefaultSelect排序，否则按Priority排序
        /// </summary>
        /// <param name="sortByDefaultSelect"></param>
        public void Sort(bool sortByDefaultSelect)
        {
            if (sortByDefaultSelect)
            {
                this.Sort((t1, t2) =>
                {
                    int compareResult;

                    if (t1.DefaultSelect == t2.DefaultSelect)
                        compareResult = t1.Priority - t2.Priority;
                    else
                        compareResult = (t1.DefaultSelect) ? -1 : 1;

                    return compareResult;
                });
            }
            else
            {
                this.Sort((t1, t2) => t1.Priority - t2.Priority);
            }
        }

        /// <summary>
        /// 得到所有能够流转的前进线，不含退回
        /// </summary>
        /// <returns></returns>
        public WfTransitionDescriptorCollection GetAllCanTransitForwardTransitions()
        {
            return GetAllCanTransitForwardTransitions(true);
        }

        /// <summary>
        /// 得到所有能够流转的前进线，不含退回
        /// </summary>
        /// <param name="sortByDefaultSelect">是否按照默认选择属性排序</param>
        /// <returns></returns>
        public WfTransitionDescriptorCollection GetAllCanTransitForwardTransitions(bool sortByDefaultSelect)
        {
            WfTransitionDescriptorCollection result = CreateNewCollection();

            foreach (IWfTransitionDescriptor transition in this)
            {
                try
                {
                    if (transition.Properties.GetValue("IsReturn", false) == false && transition.CanTransit())
                        result.Add(transition);
                }
                catch (WfTransitionEvaluationException ex)
                {
                    ex.WriteToLog();
                }
            }

            return result;
        }

        /// <summary>
        /// 返回所有的前进线
        /// </summary>
        /// <returns></returns>
        public WfTransitionDescriptorCollection GetAllForwardTransitions()
        {
            return GetAllForwardTransitions(true);
        }

        /// <summary>
        /// 返回所有的前进线
        /// </summary>
        /// <param name="sortByDefaultSelect">是否按照默认选择属性排序</param>
        /// <returns></returns>
        public WfTransitionDescriptorCollection GetAllForwardTransitions(bool sortByDefaultSelect)
        {
            WfTransitionDescriptorCollection result = CreateNewCollection();

            foreach (IWfTransitionDescriptor transition in this)
            {
                if (transition.Properties.GetValue("IsReturn", false) == false)
                    result.Add(transition);
            }

            return result;
        }

        /// <summary>
        /// 得到不是动态活动入口线的集合
        /// </summary>
        /// <returns></returns>
        public WfTransitionDescriptorCollection GetNotDynamicActivityTransitions()
        {
            WfTransitionDescriptorCollection result = CreateNewCollection();

            foreach (IWfTransitionDescriptor transition in this)
            {
                if (transition.IsDynamicActivityTransition == false)
                    result.Add(transition);
            }

            return result;
        }

        public WfTransitionDescriptorCollection GetAllEnabledTransitions()
        {
            WfTransitionDescriptorCollection result = CreateNewCollection();

            foreach (WfTransitionDescriptor transition in this)
            {
                if (transition.Enabled)
                    result.Add(transition);
            }

            result.Sort((t1, t2) =>
            {
                int compareResult;

                if (t1.DefaultSelect == t2.DefaultSelect)
                    compareResult = t1.Priority - t2.Priority;
                else
                    compareResult = (t1.DefaultSelect) ? -1 : 1;

                return compareResult;
            }
            );

            return result;
        }

        /// <summary>
        /// 删除相关的线，From集合删除To的，反之亦然
        /// </summary>
        /// <param name="transition"></param>
        protected abstract void RemoveRelativeTransition(IWfTransitionDescriptor transition);

        /// <summary>
        /// 清空transition，同时删除相关的transition
        /// </summary>
        protected override void OnClear()
        {
            this.ForEach(t => RemoveRelativeTransition(t));

            base.OnClear();
        }
    }

    /// <summary>
    /// 线属性集合
    /// </summary>
    /// <remarks>记录下从每一点所发出的线，提供线的增加、根据ToActivity查找线</remarks>
    [Serializable]
    [XElementSerializable]
    public class ToTransitionsDescriptorCollection : WfTransitionDescriptorCollection
    {
        private IWfActivityDescriptor _FromActivity = null;

        public ToTransitionsDescriptorCollection()
            : base(null)
        {
        }

        internal ToTransitionsDescriptorCollection(IWfActivityDescriptor fromActivity)
            : base(fromActivity)
        {
            _FromActivity = fromActivity;
        }

        protected ToTransitionsDescriptorCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        public IWfForwardTransitionDescriptor AddForwardTransition(IWfActivityDescriptor toActivity)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(toActivity != null, "toActivity");

            WfForwardTransitionDescriptor transition = new WfForwardTransitionDescriptor(toActivity.Process.FindNotUsedTransitionKey());

            AddTransition(toActivity, transition);

            return transition;
        }

        public IWfBackwardTransitionDescriptor AddBackwardTransition(IWfActivityDescriptor toActivity)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(toActivity != null, "toActivity");

            WfBackwardTransitionDescriptor transition = new WfBackwardTransitionDescriptor();

            transition.Key = toActivity.Process.FindNotUsedTransitionKey();
            transition.IsBackward = true;

            AddTransition(toActivity, transition);

            return transition;
        }

        public void AddTransition(IWfActivityDescriptor toActivity, IWfTransitionDescriptor transition)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(toActivity != null, "toActivity");
            ExceptionHelper.FalseThrow<ArgumentNullException>(transition != null, "transition");

            ExceptionHelper.FalseThrow<WfDescriptorException>(
                    _FromActivity.Process.Activities.Contains(toActivity),
                    Translator.Translate(WfHelper.CultureCategory, "流程活动{0}必须在流程定义当中"), toActivity.Key);

            ((WfTransitionDescriptor)transition).JoinActivity(_FromActivity, toActivity);

            base.Add(transition);
            toActivity.FromTransitions.Add(transition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actKey"></param>
        /// <returns></returns>
        private IWfTransitionDescriptor GetTransitionByToActivity(string actKey)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(actKey, "actKey");

            WfTransitionDescriptor result = null;

            foreach (WfTransitionDescriptor transition in base.List)
            {
                if (transition.ToActivity.Key == actKey)
                {
                    result = transition;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actDesp"></param>
        /// <returns></returns>
        public IWfTransitionDescriptor GetTransitionByToActivity(IWfActivityDescriptor actDesp)
        {
            IWfTransitionDescriptor result = null;

            if (actDesp != null)
                result = GetTransitionByToActivity(actDesp.Key);

            return result;
        }

        internal protected override WfTransitionDescriptorCollection CreateNewCollection()
        {
            return new ToTransitionsDescriptorCollection(this._FromActivity);
        }

        protected override void RemoveRelativeTransition(IWfTransitionDescriptor transition)
        {
            transition.ToActivity.FromTransitions.Remove(t => t == transition);
        }

        /// <summary>
        /// 当Key重复时，试图修改Key以适应这个集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        protected override void OnDuplicateKey(string key, IWfTransitionDescriptor data)
        {
            if (data.FromActivity != null)
            {
                if (data.FromActivity.Process != null)
                    ((WfTransitionDescriptor)data).Key = data.FromActivity.Process.FindNotUsedTransitionKey();
            }

            if (data.Key == key)
                base.OnDuplicateKey(key, data);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class FromTransitionsDescriptorCollection : WfTransitionDescriptorCollection
    {
        private IWfActivityDescriptor _ToActivity = null;

        private FromTransitionsDescriptorCollection()
            : base(null)
        {
        }

        internal FromTransitionsDescriptorCollection(IWfActivityDescriptor toActivity)
            : base(toActivity)
        {
            _ToActivity = toActivity;
        }

        protected FromTransitionsDescriptorCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        public IWfTransitionDescriptor AddForwardTransition(IWfActivityDescriptor fromActivity)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(fromActivity != null, "toActivity");

            WfForwardTransitionDescriptor transition = new WfForwardTransitionDescriptor();

            transition.Key = fromActivity.Process.FindNotUsedTransitionKey();

            AddTransition(fromActivity, transition);

            return transition;
        }

        public IWfTransitionDescriptor AddBackwardTransition(IWfActivityDescriptor fromActivity)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(fromActivity != null, "toActivity");

            WfBackwardTransitionDescriptor transition = new WfBackwardTransitionDescriptor();

            transition.Key = fromActivity.Process.FindNotUsedTransitionKey();
            transition.IsBackward = true;

            AddTransition(fromActivity, transition);

            return transition;
        }

        public void AddTransition(IWfActivityDescriptor fromActivity, IWfTransitionDescriptor transition)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(fromActivity != null, "fromActivity");
            ExceptionHelper.FalseThrow<ArgumentNullException>(transition != null, "transition");
            ExceptionHelper.FalseThrow<WfDescriptorException>(
                _ToActivity.Process.Activities.Contains(fromActivity),
                Translator.Translate(WfHelper.CultureCategory, "流程活动{0}必须在流程定义当中"), fromActivity.Key);

            ((WfTransitionDescriptor)transition).JoinActivity(fromActivity, _ToActivity);

            base.Add(transition);
            fromActivity.ToTransitions.Add(transition);
        }

        /// <summary>
        /// 根据FromActivity(线的发出点)得到线
        /// </summary>
        /// <param name="actKey">FromActivity Key值</param>
        /// <returns>result</returns>
        /// <remarks>根据FromActivity(线的发出点)得到线</remarks>

        private IWfTransitionDescriptor GetTransitionByFromActivity(string actKey)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(actKey, "actKey");

            IWfTransitionDescriptor result = null;

            foreach (IWfTransitionDescriptor transition in base.List)
            {
                if (transition.FromActivity.Key == actKey)
                {
                    result = transition;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 根据FromActivity(线的发出点)得到线
        /// </summary>
        /// <param name="actDesp">FromActivity</param>
        /// <returns>GetTransitionByFromActivity</returns>
        /// <remarks>根据FromActivity(线的发出点)得到线</remarks>
        public IWfTransitionDescriptor GetTransitionByFromActivity(IWfActivityDescriptor actDesp)
        {
            IWfTransitionDescriptor result = null;

            if (actDesp != null)
                result = GetTransitionByFromActivity(actDesp.Key);

            return result;
        }

        internal protected override WfTransitionDescriptorCollection CreateNewCollection()
        {
            return new FromTransitionsDescriptorCollection(this._ToActivity);
        }

        protected override void RemoveRelativeTransition(IWfTransitionDescriptor transition)
        {
            transition.FromActivity.ToTransitions.Remove(t => t == transition);
        }
    }

    /// <summary>
    /// 线上的条件计算所产生的异常
    /// </summary>
    [Serializable]
    public class WfTransitionEvaluationException : WfEvaluationExceptionBase
    {
        /// <summary>
        /// 
        /// </summary>
        public WfTransitionEvaluationException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public WfTransitionEvaluationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="condition"></param>
        public WfTransitionEvaluationException(string message, WfConditionDescriptor condition)
            : base(message, condition)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public WfTransitionEvaluationException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="condition"></param>
        public WfTransitionEvaluationException(string message, System.Exception innerException, WfConditionDescriptor condition)
            : base(message, innerException, condition)
        {
        }
    }
}
