using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 非关联的活动点
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WfMainStreamActivityDescriptor : IWfMainStreamActivityDescriptor
	{
		private WfAssociatedActivitiesDescriptorCollection _AssociatedActivities = null;
		private readonly IWfActivityDescriptor _SourceActivity = null;
		private int _Level = 0;

		internal WfMainStreamActivityDescriptor(IWfActivityDescriptor sourceActDesp)
		{
			sourceActDesp.NullCheck("sourceActDesp");
			this._SourceActivity = sourceActDesp;
		}

		#region IWfMainStreamActivityDescriptor Members
		public IWfActivityDescriptor Activity
		{
			get
			{
				return this._SourceActivity;
			}
		}

		public WfAssociatedActivitiesDescriptorCollection AssociatedActivities
		{
			get
			{
				if (this._AssociatedActivities == null)
					this._AssociatedActivities = new WfAssociatedActivitiesDescriptorCollection();

				return this._AssociatedActivities;
			}
		}

		public int Level
		{
			get
			{
				return this._Level;
			}
			internal set
			{
				this._Level = value;
			}
		}
		#endregion
	}

	/// <summary>
	/// 没有关联的活动描述集合
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WfMainStreamActivityDescriptorCollection : SerializableEditableKeyedDataObjectCollectionBase<string, IWfMainStreamActivityDescriptor>
	{
		protected override string GetKeyForItem(IWfMainStreamActivityDescriptor item)
		{
			return item.Activity.Key;
		}
	}

	[Serializable]
	[XElementSerializable]
	public class WfAssociatedActivitiesDescriptorCollection : SerializableEditableKeyedDataObjectCollectionBase<string, IWfActivityDescriptor>
	{
		protected override string GetKeyForItem(IWfActivityDescriptor item)
		{
			return item.Key;
		}
	}
}
