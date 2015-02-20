using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public delegate void PrepareNextStepsEventHanlder(WfControlNextStepCollection nextSteps);
	public delegate void AfterGetNextStepResourcesEventHanlder(WfControlNextStep nextStep);

	[Serializable]
	[XElementSerializable]
	public class WfControlNextStepCollection : EditableDataObjectCollectionBase<WfControlNextStep>
	{
		public static WfControlNextStepCollection GetControlNextStepsByProcessDescriptor(
			IWfActivityDescriptor originalActivityDescriptor,
			PrepareNextStepsEventHanlder prepareNextStepsHandler,
			AfterGetNextStepResourcesEventHanlder afterGetNextStepResourcesHandler)
		{
			WfTransitionDescriptorCollection transitions = originalActivityDescriptor.ToTransitions.GetAllCanTransitTransitions();

			WfControlNextStepCollection nextSteps = new WfControlNextStepCollection(transitions);

			if (prepareNextStepsHandler != null)
				prepareNextStepsHandler(nextSteps);

			SetFirstStepProperties(nextSteps, afterGetNextStepResourcesHandler);

			return nextSteps;
		}

		private static void SetFirstStepProperties(WfControlNextStepCollection nextSteps,
			AfterGetNextStepResourcesEventHanlder afterGetNextStepResourcesHandler)
		{
			if (nextSteps.Count > 0)
			{
				WfControlNextStep firstStep = nextSteps[0];

				if (afterGetNextStepResourcesHandler != null)
					afterGetNextStepResourcesHandler(firstStep);
			}
		}

		public WfControlNextStepCollection()
		{
		}

		public WfControlNextStepCollection(WfTransitionDescriptorCollection transitions)
		{
			foreach (IWfTransitionDescriptor transition in transitions)
			{
				IWfActivity toActivity =
					transition.ToActivity.Process.ProcessInstance.Activities.FindActivityByDescriptorKey(transition.ToActivity.Key);

				if (toActivity != null)
				{
					WfControlNextStep nextStep = new WfControlNextStep(transition, toActivity);

					this.Add(nextStep);
				}
			}
		}

		/// <summary>
		/// 转到Xml节点
		/// </summary>
		/// <param name="element"></param>
		public void ToXElement(XElement element)
		{
			element.NullCheck("element");

			foreach (WfControlNextStep step in this)
			{
				XElement stepNode = element.AddChildElement("Step");

				step.ToXElement(stepNode);
			}
		}
	}
}
