using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Runtime
{
	public class WfClientProcessInfoConverter
	{
		public static readonly WfClientProcessInfoConverter Instance = new WfClientProcessInfoConverter();

		private WfClientProcessInfoConverter()
		{
		}

		public WfClientProcessInfo ServerToClient(IWfProcess process, ref WfClientProcessInfo client)
		{
			process.NullCheck("process");

			if (client == null)
				client = new WfClientProcessInfo();

			WfClientProcessInfoBaseConverter.Instance.ServerToClient(process, client);

			WfClientActivity currentActivity = null;

			WfClientActivityConverter.Instance.ServerToClient(process.CurrentActivity, ref currentActivity);

			client.CurrentActivity = currentActivity;

			IWfActivity previousServerActivity = FindPreviousActivity(process.CurrentActivity);

			if (previousServerActivity != null)
			{
				WfClientActivity previousActivity = null;

				WfClientActivityConverter.Instance.ServerToClient(previousServerActivity, ref previousActivity);

				client.PreviousActivity = previousActivity;
			}

			client.NextActivities.Clear();

			if (client.CurrentActivity != null)
				ServerTargetsToClient(process.CurrentActivity, client);

			return client;
		}

		private static void ServerTargetsToClient(IWfActivity server, WfClientProcessInfo client)
		{
			WfClientActivity ccActivity = client.CurrentActivity;

			foreach (WfTransitionDescriptor st in server.Descriptor.ToTransitions.GetAllCanTransitTransitions(true))
			{
				WfClientActivity targetActivity = null;

				if (st.Key != ccActivity.Descriptor.Key)
					WfClientActivityConverter.Instance.ServerToClient(st.ToActivity.Instance, ref targetActivity);
				else
					targetActivity = ccActivity;

				if (targetActivity != null)
				{
					WfClientTransitionDescriptor ct = null;

					WfClientTransitionDescriptorConverter.Instance.ServerToClient(st, ref ct);

					WfClientNextActivity cnat = new WfClientNextActivity();

					cnat.Transition = ct;
					cnat.Activity = targetActivity;

					client.NextActivities.Add(cnat);
				}
			}
		}

		private static IWfActivity FindPreviousActivity(IWfActivity currentActivity)
		{
			IWfActivity result = null;

			int startIndex = currentActivity.Process.ElapsedActivities.Count - 1;

			if (currentActivity.Descriptor.ActivityType == WfActivityType.CompletedActivity)
				startIndex--;

			if (startIndex >= 0)
				result = currentActivity.Process.ElapsedActivities[startIndex];

			return result;
		}
	}
}
