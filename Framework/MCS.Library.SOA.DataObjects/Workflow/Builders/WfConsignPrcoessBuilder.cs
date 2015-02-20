using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.Builders
{
	/// <summary>
	/// 默认的会签子流程构造器，仅仅构造一个起始点和终止点
	/// </summary>
	public class WfConsignProcessBuilder : WfProcessBuilderBase
	{
		public WfConsignProcessBuilder(string appName, string progName) : base(appName, progName)
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

			processDesp.Properties.SetValue("UseMatrix", false);
			processDesp.Properties.SetValue("Independent", false);

			processDesp.InitialActivity.ToTransitions.AddForwardTransition(processDesp.CompletedActivity);
			
			//设置一些会签活动的属性设置
			PropertyDefineCollection definedProperties = GetDefinedProperties();

			processDesp.InitialActivity.Properties.MergeDefinedProperties(definedProperties,true);

			return processDesp;
		}

		public override PropertyDefineCollection GetDefinedProperties()
		{
			PropertyDefineCollection definedProperties = new PropertyDefineCollection();

			definedProperties.LoadPropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["DefaultConsignActivityTemplate"]);

			return definedProperties;
		}
	}
}
