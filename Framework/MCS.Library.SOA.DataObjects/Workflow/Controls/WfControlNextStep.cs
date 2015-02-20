using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 在流程相关的控件中，流转环节的下一步
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public sealed class WfControlNextStep
	{
		private WfAssigneeCollection _Candidates = null;

		public WfControlNextStep()
		{
		}

		public WfControlNextStep(IWfActivity toActivity)
		{
			toActivity.NullCheck("toActivity");

			this.ActivityDescriptor = toActivity.Descriptor;
			this._Candidates = toActivity.Candidates.GetSelectedAssignees();
		}

		public WfControlNextStep(IWfTransitionDescriptor transition, IWfActivity toActivity)
		{
			transition.NullCheck("transition");
			toActivity.NullCheck("toActivity");

			this.ActivityDescriptor = toActivity.Descriptor;
			this._Candidates = toActivity.Candidates.GetSelectedAssignees();;
			this.TransitionDescriptor = transition;
		}

		public IWfActivityDescriptor ActivityDescriptor
		{
			get;
			internal set;
		}

		public WfAssigneeCollection Candidates
		{
			get
			{
				if (this._Candidates == null)
					this._Candidates = new WfAssigneeCollection();

				return this._Candidates;
			}
			internal set
			{
				this._Candidates = value;
			}
		}

		public IWfTransitionDescriptor TransitionDescriptor
		{
			get;
			internal set;
		}

		/// <summary>
		/// 转换为Xml节点
		/// </summary>
		/// <param name="element"></param>
		public void ToXElement(XElement element)
		{
			element.NullCheck("element");

			if (this.TransitionDescriptor != null)
			{
				element.SetAttributeValue("transitionKey", this.TransitionDescriptor.Key);
				element.SetAttributeValue("transitionName", this.TransitionDescriptor.Name);
				element.SetAttributeValue("transitionDescription", this.TransitionDescriptor.Description);
			}

			if (this.ActivityDescriptor != null)
			{
				element.SetAttributeValue("activityKey", this.ActivityDescriptor.Key);
				element.SetAttributeValue("activityName", this.ActivityDescriptor.Name);
				element.SetAttributeValue("activityDescription", this.ActivityDescriptor.Description);
			}
		}
	}
}
