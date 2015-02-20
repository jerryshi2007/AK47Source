using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Builders
{
    public abstract class WfClientProcessBuilderBase
    {
        private string _ApplicationName = string.Empty;
        private string _ProgramName = string.Empty;

        protected WfClientProcessBuilderBase(string appName, string progName)
        {
            this._ApplicationName = appName;
            this._ProgramName = progName;
        }

        /// <summary>
        /// 基类中构造开始点和结束点，不包含两个点的连线
        /// </summary>
        /// <param name="processKey"></param>
        /// <param name="processName"></param>
        /// <returns></returns>
        public virtual WfClientProcessDescriptor Build(string processKey, string processName)
        {
            processKey.CheckStringIsNullOrEmpty("processKey");
            processName.CheckStringIsNullOrEmpty("processName");

            WfClientProcessDescriptor processDesp = new WfClientProcessDescriptor();

            processDesp.Key = processKey;
            processDesp.ApplicationName = this._ApplicationName;
            processDesp.ProgramName = this._ProgramName;
            processDesp.Name = processName;

            processDesp.Activities.Add(new WfClientActivityDescriptor(WfClientActivityType.InitialActivity) { Key = "Start", Name = "起草" });
            processDesp.Activities.Add(new WfClientActivityDescriptor(WfClientActivityType.CompletedActivity) { Key = "Complete", Name = "办结" });

            processDesp.Properties.AddOrSetValue("ProbeParentProcessParams", "True");

            return processDesp;
        }
    }
}
