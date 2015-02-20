using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
	/// <summary>
	/// 客户端的流程信息
	/// </summary>
	[DataContract]
	[Serializable]
	public class WfClientProcess : WfClientProcessInfoBase
	{
		private WfClientActivityCollection _Activities = null;
		private WfClientProcessDescriptor _Descriptor = null;
		private WfClientProcessDescriptor _MainStream = null;

		private WfClientActivity _InitialActivity = null;
		private WfClientActivity _CompletedActivity = null;
		private WfClientActivity _CurrentActivity = null;

		public WfClientProcess()
		{
		}

		public WfClientProcess(WfClientProcessDescriptor descriptor)
		{
			this._Descriptor = descriptor;
		}

		public WfClientProcess(WfClientProcessDescriptor descriptor, WfClientProcessDescriptor mainStream)
		{
			this._Descriptor = descriptor;
			this._MainStream = mainStream;
		}

		public WfClientActivity InitialActivity
		{
			get
			{
				if (this._InitialActivity == null)
					this._InitialActivity = this.Activities.Find(act => act.Descriptor != null && act.Descriptor.ActivityType == WfClientActivityType.InitialActivity);

				return this._InitialActivity;
			}
		}

		public WfClientActivity CompletedActivity
		{
			get
			{
				if (this._CompletedActivity == null)
					this._CompletedActivity = this.Activities.Find(act => act.Descriptor != null && act.Descriptor.ActivityType == WfClientActivityType.CompletedActivity);

				return this._CompletedActivity;
			}
		}

		public WfClientActivity CurrentActivity
		{
			get
			{
				if (this._CurrentActivity == null)
					this._CurrentActivity = this.Activities.Find(act => act.Descriptor != null && act.Descriptor.Key == this.CurrentActivityKey);

				return this._CurrentActivity;
			}
		}

		public WfClientProcessDescriptor Descriptor
		{
			get
			{
				return this._Descriptor;
			}
		}

		public WfClientProcessDescriptor MainStream
		{
			get
			{
				return this._MainStream;
			}
		}

		public WfClientActivityCollection Activities
		{
			get
			{
				if (this._Activities == null)
					this._Activities = new WfClientActivityCollection();

				return _Activities;
			}
		}

		/// <summary>
		/// 根据Descriptor，重新整理一下Activity对象
		/// </summary>
		public void NormalizeActivities()
		{
			if (this.Descriptor != null)
			{
				foreach (WfClientActivity activity in this.Activities)
				{
					if (this.Descriptor.Activities.ContainsKey(activity.DescriptorKey))
						activity.Descriptor = this.Descriptor.Activities[activity.DescriptorKey];
				}
			}
		}
	}

	/// <summary>
	/// 流程摘要信息集合
	/// </summary>
	[DataContract]
	[Serializable]
	public class WfClientProcessCollection : WfClientProcessInfoCollectionBase<WfClientProcess>
	{
		public WfClientProcessCollection()
		{
		}

		protected WfClientProcessCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}
	}
}
