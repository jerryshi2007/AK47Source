using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Web.WebControls
{
	internal class RejectActivityList : EditableKeyedDataObjectCollectionBase<string, IWfActivity>
	{
		protected override string GetKeyForItem(IWfActivity item)
		{
			return item.Descriptor.GetAssociatedActivity().Key;
		}

		public static RejectActivityList CreateFromProcess(IWfProcess process)
		{
			RejectActivityList result = null;

			if (process.MainStream != null)
				result = CreateFromProcessWithMainStream(process);
			else
				result = OldCreateFromProcessWithoutMainStream(process);

			return result;
		}

		private static RejectActivityList CreateFromProcessWithMainStream(IWfProcess process)
		{
			RejectActivityList result = new RejectActivityList();

			string currentMainStreamKey = process.CurrentActivity.MainStreamActivityKey;

			for (int i = process.ElapsedActivities.Count - 1; i >= 0; i--)
			{
				IWfActivity activity = process.ElapsedActivities[i];

				if (activity.MainStreamActivityKey.IsNotEmpty() &&
					activity.Descriptor.AssociatedActivityKey.IsNullOrEmpty() &&
					currentMainStreamKey.IsNotEmpty() && activity.MainStreamActivityKey != currentMainStreamKey)
				{
					if (result.ContainsKey(activity.Descriptor.GetAssociatedActivity().Key) == false)
						result.Add(activity);
				}
			}

			return result;
		}

		/// <summary>
		/// 这是兼容于旧流程的处理方式
		/// </summary>
		/// <param name="process"></param>
		/// <returns></returns>
		private static RejectActivityList OldCreateFromProcessWithoutMainStream(IWfProcess process)
		{
			RejectActivityList result = new RejectActivityList();

			for (int i = process.ElapsedActivities.Count - 1; i >= 0; i--)
			{
				IWfActivity activity = process.ElapsedActivities[i];

				if (activity.Descriptor.IsReturnSkipped == false &&
					activity.Descriptor.GetAssociatedActivity() != process.CurrentActivity.Descriptor.GetAssociatedActivity())
				{
					if (result.ContainsKey(activity.Descriptor.GetAssociatedActivity().Key) == false)
						result.Add(activity);
				}
			}

			return result;
		}
	}
}
