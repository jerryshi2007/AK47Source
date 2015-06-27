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
        private bool _Elapsed = false;

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

        /// <summary>
        /// 根据连线遍历时的活动的层级
        /// </summary>
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

        /// <summary>
        /// 这个活动是否执行过
        /// </summary>
        public bool Elapsed
        {
            get
            {
                return this._Elapsed;
            }
            internal set
            {
                this._Elapsed = value;
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
