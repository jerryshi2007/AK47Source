using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Expression;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.Builders
{
    /// <summary>
    /// 创建一个流程活动的参数
    /// </summary>
    [Serializable]
    public class WfCreateActivityParam
    {
        public const string DefaultNextActivityDescription = "DefaultNextActivity";

        private WfActivityDescriptor _Template = null;
        private WfCreateTransitionParamCollection _TransitionTemplates = null;

        public WfCreateActivityParam()
        {
        }

        public static WfCreateActivityParam FromRowUsers(SOARolePropertyRowUsers rowUsers, SOARolePropertyDefinitionCollection definitions, PropertyDefineCollection definedProperties)
        {
            rowUsers.NullCheck("rowUsers");
            definitions.NullCheck("definitions");
            definedProperties.NullCheck("definedProperties");

            WfCreateActivityParam cap = new WfCreateActivityParam();

            cap.Source = rowUsers.Row;

            string strActivitySN = rowUsers.Row.Values.GetValue(SOARolePropertyDefinition.ActivitySNColumn, "0");

            int activitySN = 0;

            int.TryParse(strActivitySN, out activitySN).FalseThrow("不能将值为{0}的ActivitySN转换为整数", strActivitySN);

            cap.ActivitySN = activitySN;
            cap.Template.Properties.MergeDefinedProperties(definedProperties);

            InitActivityTemplateProperties(cap, definitions, rowUsers.Row);

            rowUsers.Users.ForEach(u => cap.Template.Resources.Add(new WfUserResourceDescriptor(u)));

            return cap;
        }

        [NonSerialized]
        private SOARolePropertyRow _Source = null;

        /// <summary>
        /// 创建活动后，活动对象的实例
        /// </summary>
        public IWfActivityDescriptor CreatedDescriptor
        {
            get;
            internal set;
        }

        /// <summary>
        /// 创建此活动的源
        /// </summary>
        internal SOARolePropertyRow Source
        {
            get
            {
                return this._Source;
            }
            set
            {
                this._Source = value;
            }
        }

        /// <summary>
        /// 创建活动后，默认的下一个活动对象（按顺序创建时的活动排列，如果是最后一个动态活动，则是下一个非动态活动）
        /// </summary>
        public IWfActivityDescriptor DefaultNextDescriptor
        {
            get;
            internal set;
        }

        /// <summary>
        /// 排序号
        /// </summary>
        public int ActivitySN
        {
            get;
            set;
        }

        /// <summary>
        /// 活动的模板
        /// </summary>
        public WfActivityDescriptor Template
        {
            get
            {
                if (this._Template == null)
                    this._Template = new WfActivityDescriptor(string.Empty, WfActivityType.NormalActivity);

                return this._Template;
            }
        }

        /// <summary>
        /// 出线的模板
        /// </summary>
        public WfCreateTransitionParamCollection TransitionTemplates
        {
            get
            {
                if (this._TransitionTemplates == null)
                    this._TransitionTemplates = new WfCreateTransitionParamCollection();

                return this._TransitionTemplates;
            }
        }

        private List<string> _RoleDefineActivityPropertyNames = null;
        //ydz 2013-2-18 解决活动矩阵定义的属性优先于活动模板定义的属性
        /// <summary>
        /// 活动矩阵中定义了当前环节的名称
        /// </summary>
        public List<string> RoleDefineActivityPropertyNames
        {
            get
            {
                if (this._RoleDefineActivityPropertyNames == null)
                    this._RoleDefineActivityPropertyNames = new List<string>();

                return this._RoleDefineActivityPropertyNames;
            }
        }

        private static void InitActivityTemplateProperties(WfCreateActivityParam cap, SOARolePropertyDefinitionCollection definitions, SOARolePropertyRow row)
        {
            foreach (SOARolePropertyDefinition pd in definitions)
            {
                if (string.Compare(pd.Name, SOARolePropertyDefinition.ActivityPropertiesColumn, true) == 0)
                    ProcessActivityPropertyByRoleJsonProperty(cap, row, pd);
                else
                    ProcessActivityPropertyByRoleProperty(cap, row, pd);
            }

            if (cap.Template.Variables.ContainsKey(WfProcessBuilderBase.AutoBuiltActivityVariableName))
                cap.Template.Variables[WfProcessBuilderBase.AutoBuiltActivityVariableName].OriginalValue = "True";
            else
                cap.Template.Variables.Add(new WfVariableDescriptor(WfProcessBuilderBase.AutoBuiltActivityVariableName, "True", DataType.Boolean));
        }

        private static void ProcessActivityPropertyByRoleJsonProperty(WfCreateActivityParam cap, SOARolePropertyRow row, SOARolePropertyDefinition pd)
        {
            string json = row.Values.GetValue(pd.Name, row.GetPropertyDefinitions().GetColumnDefaultValue(pd.Name, string.Empty));

            if (json.IsNotEmpty())
            {
                Dictionary<string, object> data = JSONSerializerExecute.Deserialize<Dictionary<string, object>>(json);

                foreach (KeyValuePair<string, object> kp in data)
                {
                    cap.Template.Properties.TrySetValue(kp.Key, kp.Value);
                    //ydz 2013-2-18 解决活动矩阵定义的属性优先于活动模板定义的属性
                    cap.RoleDefineActivityPropertyNames.Add(kp.Key);
                }
            }
        }

        /// <summary>
        /// 根据带Activity前缀的Role的属性名称，初始化Activity的属性值
        /// </summary>
        /// <param name="cap"></param>
        /// <param name="row"></param>
        /// <param name="pd"></param>
        private static void ProcessActivityPropertyByRoleProperty(WfCreateActivityParam cap, SOARolePropertyRow row, SOARolePropertyDefinition pd)
        {
            const string prefixName = "Activity";

            if (pd.Name.IndexOf(prefixName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                string actPropertyName = pd.Name.Substring(prefixName.Length);

                string propertyValue = row.Values.GetValue(pd.Name, row.GetPropertyDefinitions().GetColumnDefaultValue(pd.Name, string.Empty));

                cap.Template.Properties.TrySetValue(actPropertyName, propertyValue);

                cap.RoleDefineActivityPropertyNames.Add(actPropertyName);
            }
        }
    }

    /// <summary>
    /// 创建流程的参数集合，可以根据此参数动态创建流程
    /// </summary>
    [Serializable]
    public class WfCreateActivityParamCollection : EditableDataObjectCollectionBase<WfCreateActivityParam>
    {
        public WfCreateActivityParam FindByActivitySN(int sn)
        {
            return this.Find(cap => cap.ActivitySN == sn);
        }

        /// <summary>
        /// 根据Activity来合并创造活动点的参数
        /// </summary>
        public void MergeSameActivityParamBySN()
        {
            int i = 0;
            Dictionary<int, WfCreateActivityParam> capDict = new Dictionary<int, WfCreateActivityParam>();

            while (i < this.Count)
            {
                WfCreateActivityParam currentParam = this[i];
                WfCreateActivityParam capInDict = null;

                if (capDict.TryGetValue(currentParam.ActivitySN, out capInDict))
                {
                    capInDict.Template.Resources.CopyFrom(currentParam.Template.Resources);
                    this.RemoveAt(i);
                }
                else
                {
                    capDict.Add(currentParam.ActivitySN, currentParam);
                    i++;
                }
            }
        }

        /// <summary>
        /// 根据活动点的描述获取活动描述
        /// </summary>
        /// <param name="processDesp">流程描述</param>
        /// <param name="currentCap">当前活动</param>
        /// <param name="toActivityDesp"></param>
        /// <returns></returns>
        public IWfActivityDescriptor FindActivityByActivityDescription(IWfProcessDescriptor processDesp, WfCreateActivityParam currentCap, string toActivityDesp)
        {
            IWfActivityDescriptor result = null;

            string foundActKey = FindActivityKeyByActivityDescription(processDesp, currentCap, toActivityDesp);

            if (foundActKey.IsNotEmpty())
                result = processDesp.Activities[foundActKey];

            return result;
        }

        /// <summary>
        /// 根据WfCreateActivityParamCollection的值创建节点，覆盖当前的流程
        /// </summary>
        /// <param name="overrideInitActivity"></param>
        public void CreateActivities(IWfProcessDescriptor processDesp, bool overrideInitActivity)
        {
            WfActivityDescriptor currentActDesp = (WfActivityDescriptor)processDesp.InitialActivity;

            CreateAndAppendActivities(currentActDesp, overrideInitActivity);
        }

        private WfActivityDescriptorCollection CreateAndAppendActivities(WfActivityDescriptor currentActDesp, bool overrideInitActivity)
        {
            IWfProcessDescriptor processDesp = currentActDesp.Process;
            WfCreateActivityParam lastCreatedActivityParam = null;

            WfActivityDescriptorCollection result = new WfActivityDescriptorCollection(processDesp);

            if (currentActDesp != null)
            {
                int i = 0;

                foreach (WfCreateActivityParam cap in this)
                {
                    if (overrideInitActivity && i == 0)
                    {
                        string key = currentActDesp.Key;
                        cap.Template.CopyPropertiesTo(currentActDesp);

                        currentActDesp.Key = key;
                    }
                    else
                    {
                        WfActivityDescriptor actDesp = (WfActivityDescriptor)cap.Template.Clone();

                        actDesp.Process = processDesp;
                        actDesp.Key = processDesp.FindNotUsedActivityKey();

                        currentActDesp.Append(actDesp);

                        currentActDesp = actDesp;
                    }

                    cap.CreatedDescriptor = currentActDesp;
                    result.Add(currentActDesp);

                    if (lastCreatedActivityParam != null)
                        lastCreatedActivityParam.DefaultNextDescriptor = currentActDesp;

                    lastCreatedActivityParam = cap;

                    i++;
                }

                if (lastCreatedActivityParam != null)
                {
                    IWfTransitionDescriptor defaultTransition =
                        lastCreatedActivityParam.CreatedDescriptor.ToTransitions.GetAllCanTransitForwardTransitions().FindDefaultSelectTransition();

                    if (defaultTransition != null)
                        lastCreatedActivityParam.DefaultNextDescriptor = defaultTransition.ToActivity;
                }

                AdjustTransitionsByTemplate();
            }

            return result;
        }

        /// <summary>
        /// 根据线定义进行
        /// </summary>
        internal void AdjustTransitionsByTemplate(string templateKey = "")
        {
            for (int i = 0; i < this.Count; i++)
            {
                WfCreateActivityParam cap = this[i];

                if (cap.CreatedDescriptor != null & cap.TransitionTemplates.Count > 0)
                {
                    IEnumerable<IWfTransitionDescriptor> createdTransitions = AdjustOneActivityTransitionsByTemplate(cap);

                    if (templateKey.IsNotEmpty())
                    {
                        foreach (WfTransitionDescriptor newTransition in createdTransitions)
                        {
                            newTransition.GeneratedByTemplate = true;
                            newTransition.TemplateKey = templateKey;
                        }
                    }
                }
            }
        }

        private List<IWfTransitionDescriptor> AdjustOneActivityTransitionsByTemplate(WfCreateActivityParam cap)
        {
            List<IWfTransitionDescriptor> result = new List<IWfTransitionDescriptor>();

            cap.CreatedDescriptor.ToTransitions.Clear();
            IWfProcessDescriptor processDesp = cap.CreatedDescriptor.Process;

            foreach (WfCreateTransitionParam transitionParam in cap.TransitionTemplates)
            {
                string toActivityDescription = DictionaryHelper.GetValue(
                    transitionParam.Parameters,
                    "ToActivityKey",
                    WfCreateActivityParam.DefaultNextActivityDescription);

                IWfActivityDescriptor toActDesp = FindActivityByActivityDescription(processDesp, cap, toActivityDescription);

                if (toActDesp != null)
                    result.Add(transitionParam.CreateTransitionAndConnectActivities(cap.CreatedDescriptor, toActDesp));
            }

            return result;
        }

        private string FindActivityKeyByActivityDescription(IWfProcessDescriptor processDesp, WfCreateActivityParam currentCap, string toActivityDesp)
        {
            WfCreateActivityFunctionContext context = new WfCreateActivityFunctionContext(processDesp, this, currentCap);

            int sn = 0;
            if (int.TryParse(toActivityDesp, out sn))
                toActivityDesp = string.Format("SN({0})", sn);

            return (string)ExpressionParser.Calculate(toActivityDesp, new CalculateUserFunction(WfCreateActivityBuiltInFunctions.Instance.Calculate), context);
        }
    }
}
