using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.Builders
{
	/// <summary>
	/// 默认的审批流程
	/// </summary>
	public class WfApprovalProcessBuilder : WfProcessBuilderBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="appName"></param>
		/// <param name="progName"></param>
		public WfApprovalProcessBuilder(string appName, string progName)
			: base(appName, progName)
		{
		}

		/// <summary>
		/// 构造流程的起始点和终止点，并且连线
		/// </summary>
		/// <param name="processKey"></param>
		/// <param name="processName"></param>
		/// <returns></returns>
		public override IWfProcessDescriptor Build(string processKey, string processName)
		{
			IWfProcessDescriptor processDesp = base.Build(processKey, processName);

			processDesp.InitialActivity.ToTransitions.AddForwardTransition(processDesp.CompletedActivity);
			((WfActivityDescriptor)(processDesp.InitialActivity)).Name = "";

			processDesp.Properties.SetValue("UseMatrix", false);
			processDesp.Properties.SetValue("Independent", false);
			processDesp.DefaultReturnValue = true;

			//设置一些会签活动的属性设置
			PropertyDefineCollection definedProperties = GetDefinedProperties();

			processDesp.InitialActivity.Properties.MergeDefinedProperties(definedProperties,true);

			return processDesp;
		}

		public override PropertyDefineCollection GetDefinedProperties()
		{
			PropertyDefineCollection definedProperties = new PropertyDefineCollection();

			definedProperties.LoadPropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["DefaultApprovalActivityTemplate"]);

			return definedProperties;
		}
	}
}
