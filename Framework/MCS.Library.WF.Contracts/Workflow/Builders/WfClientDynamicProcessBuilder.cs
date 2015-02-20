using MCS.Library.WF.Contracts.PropertyDefine;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Builders
{
    /// <summary>
    /// 构造带动态活动的客户端流程
    /// </summary>
    public class WfClientDynamicProcessBuilder : WfClientProcessBuilderBase
    {
        private WfCreateClientDynamicProcessParams _CreateParams;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="progName"></param>
        public WfClientDynamicProcessBuilder(WfCreateClientDynamicProcessParams createParams)
            : base(createParams.ApplicationName, createParams.ProgramName)
        {
            this._CreateParams = createParams;
        }

        public WfCreateClientDynamicProcessParams CreateParams
        {
            get
            {
                return _CreateParams;
            }
        }

        public override WfClientProcessDescriptor Build(string processKey, string processName)
        {
            this.CreateParams.Key = processKey;
            this.CreateParams.Name = processName;

            WfClientProcessDescriptor processDesp = base.Build(processKey, processName);

            processDesp.Variables.AddOrSetValue("ClientDynamicProcess", "True", WfClientVariableDataType.Boolean);

            MergeProperties(processDesp.Properties, this.CreateParams.Properties);

            WfClientActivityDescriptor dynamicActivityDesp = CreateDynamicActivityDescriptor(this.CreateParams.ActivityMatrix);

            processDesp.Activities.Add(dynamicActivityDesp);

            WfClientTransitionDescriptor transitionSN = new WfClientTransitionDescriptor(processDesp.InitialActivity.Key, dynamicActivityDesp.Key) { Key = "L1" };

            processDesp.InitialActivity.ToTransitions.Add(transitionSN);

            WfClientTransitionDescriptor transitionNC = new WfClientTransitionDescriptor(dynamicActivityDesp.Key, processDesp.CompletedActivity.Key) { Key = "L2" };

            dynamicActivityDesp.ToTransitions.Add(transitionNC);

            return processDesp;
        }

        private static WfClientActivityDescriptor CreateDynamicActivityDescriptor(WfClientActivityMatrixResourceDescriptor activityMatrix)
        {
            WfClientActivityDescriptor dynamicActivityDesp = new WfClientActivityDescriptor(WfClientActivityType.NormalActivity) { Key = "N1", Name = "审批" };

            dynamicActivityDesp.Properties.AddOrSetValue("IsDynamic", "True");

            dynamicActivityDesp.Resources.Add(activityMatrix);

            return dynamicActivityDesp;
        }

        private static void MergeProperties(ClientPropertyValueCollection target, ClientPropertyValueCollection source)
        {
            foreach (ClientPropertyValue pv in source)
                target.AddOrSetValue(pv.Key, pv.StringValue);
        }
    }
}
