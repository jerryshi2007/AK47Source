using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow.Builders
{
	[Serializable]
	public class WfCreateProcessParam
	{
		private WfCreateActivityParamCollection _ActivityTemplates = null;
		private PropertyValueCollection _Properties = null;

		public WfCreateProcessParam()
		{
			WfProcessDescriptor processDesp = new WfProcessDescriptor();

			this.Properties.CopyFrom(processDesp.Properties);
		}

		public string Key
		{
			get
			{
				return this.Properties.GetValue("Key", string.Empty);
			}
			set
			{
				this.Properties.TrySetValue("Key", value);
			}
		}

		public string Name
		{
			get
			{
				return this.Properties.GetValue("Name", string.Empty);
			}
			set
			{
				this.Properties.TrySetValue("Name", value);
			}
		}

		public string Description
		{
			get
			{
				return this.Properties.GetValue("Description", string.Empty);
			}
			set
			{
				this.Properties.SetValue("Description", value);
			}
		}

		public WfCreateActivityParamCollection ActivityTemplates
		{
			get
			{
				if (this._ActivityTemplates == null)
					this._ActivityTemplates = new WfCreateActivityParamCollection();

				return this._ActivityTemplates;
			}
		}

		public PropertyValueCollection Properties
		{
			get
			{
				if (this._Properties == null)
					this._Properties = new PropertyValueCollection();

				return this._Properties;
			}
		}

		/// <summary>
		/// 创建流程定义
		/// </summary>
		/// <param name="overrideInitActivity"></param>
		/// <returns></returns>
		public WfProcessDescriptor CreateProcess(bool overrideInitActivity)
		{
			WfProcessDescriptor processDesp = new WfProcessDescriptor();

			processDesp.Properties.ReplaceExistedPropertyValues(this.Properties);

			CreateInitAndCompletedActivities(processDesp);

			processDesp.CreateActivities(this.ActivityTemplates, overrideInitActivity);

			return processDesp;
		}

		private static void CreateInitAndCompletedActivities(WfProcessDescriptor processDesp)
		{
			WfActivityDescriptor initAct = new WfActivityDescriptor("Initial", WfActivityType.InitialActivity);
			initAct.Name = "开始";
			initAct.CodeName = "Initial Activity";

			processDesp.Activities.Add(initAct);

			WfActivityDescriptor completedAct = new WfActivityDescriptor("Completed", WfActivityType.CompletedActivity);
			completedAct.Name = "结束";
			completedAct.CodeName = "Completed Activity";

			processDesp.Activities.Add(completedAct);

			initAct.ToTransitions.AddForwardTransition(completedAct);
		}
	}
}
