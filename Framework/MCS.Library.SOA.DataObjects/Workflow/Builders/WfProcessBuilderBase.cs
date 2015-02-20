using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Builders
{
	/// <summary>
	/// 流程构造器的虚基类
	/// </summary>
	public abstract class WfProcessBuilderBase
	{
		internal static string AutoBuiltProcessVariableName = "AutoBuiltProcess";
		internal static string AutoBuiltActivityVariableName = "AutoBuiltActivity";

		private string applicationName = string.Empty;
		private string programName = string.Empty;

		protected WfProcessBuilderBase(string appName, string progName)
		{
			this.applicationName = appName;
			this.programName = progName;
		}

		/// <summary>
		/// 基类中构造开始点和结束点，不包含两个点的连线
		/// </summary>
		/// <param name="processKey"></param>
		/// <param name="processName"></param>
		/// <returns></returns>
		public virtual IWfProcessDescriptor Build(string processKey, string processName)
		{
			processKey.CheckStringIsNullOrEmpty("processKey");
			processName.CheckStringIsNullOrEmpty("processName");

			WfProcessDescriptor processDesp = new WfProcessDescriptor(processKey);

			processDesp.ApplicationName = this.applicationName;
			processDesp.ProgramName = this.programName;
			processDesp.Name = processName;

			processDesp.Variables.Add(new WfVariableDescriptor(AutoBuiltProcessVariableName, "True", DataType.Boolean));

			processDesp.Activities.Add(new WfActivityDescriptor("Start", WfActivityType.InitialActivity) { Name = "起草" });
			processDesp.Activities.Add(new WfActivityDescriptor("End", WfActivityType.CompletedActivity) { Name = "办结" });

			processDesp.InitialActivity.Variables.Add(new WfVariableDescriptor(AutoBuiltActivityVariableName, "True", DataType.Boolean));
			processDesp.CompletedActivity.Variables.Add(new WfVariableDescriptor(AutoBuiltActivityVariableName, "True", DataType.Boolean));

			if (processDesp.Properties.ContainsKey("ProbeParentProcessParams"))
				processDesp.Properties.SetValue("ProbeParentProcessParams", true);

			return processDesp;
		}

		public virtual PropertyDefineCollection GetDefinedProperties()
		{
			return new PropertyDefineCollection();
		}
	}
}
