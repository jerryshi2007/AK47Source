using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
	[DataContract]
	[Serializable]
	public abstract class WfClientProcessDescriptorBase : WfClientKeyedDescriptorBase
	{
		public string ApplicationName
		{
			get { return this.Properties.GetValue("ApplicationName", string.Empty); }
			set { this.Properties.AddOrSetValue("ApplicationName", value); }
		}

		public string ProgramName
		{
			get { return this.Properties.GetValue("ProgramName", string.Empty); }
			set { this.Properties.AddOrSetValue("ProgramName", value); }
		}

		public string Url
		{
			get { return this.Properties.GetValue("Url", string.Empty); }
			set { this.Properties.AddOrSetValue("Url", value); }
		}

		public bool AutoGenerateResourceUsers
		{
			get { return this.Properties.GetValue("AutoGenerateResourceUsers", true); }
			set { this.Properties.AddOrSetValue("AutoGenerateResourceUsers", value); }
		}

		public WfClientProcessType ProcessType
		{
			get { return this.Properties.GetValue("ProcessType", WfClientProcessType.Approval); }
			set { this.Properties.AddOrSetValue("ProcessType", value); }
		}

		public string DefaultTaskTitle
		{
			get { return this.Properties.GetValue("DefaultTaskTitle", string.Empty); }
			set { this.Properties.AddOrSetValue("DefaultTaskTitle", value); }
		}

		public string DefaultNotifyTaskTitle
		{
			get { return this.Properties.GetValue("DefaultNotifyTaskTitle", string.Empty); }
			set { this.Properties.AddOrSetValue("DefaultNotifyTaskTitle", value); }
		}

		public bool DefaultReturnValue
		{
			get { return this.Properties.GetValue("DefaultReturnValue", false); }
			set { this.Properties.AddOrSetValue("DefaultReturnValue", value); }
		}
	}
}
