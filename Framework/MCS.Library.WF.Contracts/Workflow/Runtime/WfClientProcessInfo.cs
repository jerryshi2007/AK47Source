using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Ogu;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
	/// <summary>
	/// 流程描述的一个简要信息
	/// </summary>
	[DataContract]
	[Serializable]
	public class WfClientProcessInfo : WfClientProcessInfoBase
	{
		private List<WfClientNextActivity> _NextActivities = null;

		/// <summary>
		/// 当前活动
		/// </summary>
		public WfClientActivity CurrentActivity
		{
			get;
			set;
		}

		/// <summary>
		/// 前一个活动
		/// </summary>
		public WfClientActivity PreviousActivity
		{
			get;
			set;
		}

		public List<WfClientNextActivity> NextActivities
		{
			get
			{
				if (this._NextActivities == null)
					this._NextActivities = new List<WfClientNextActivity>();

				return this._NextActivities;
			}
		}
	}

	/// <summary>
	/// 流程摘要信息集合
	/// </summary>
	[DataContract]
	[Serializable]
	public class WfClientProcessInfoCollection : WfClientProcessInfoCollectionBase<WfClientProcessInfo>
	{
		public WfClientProcessInfoCollection()
		{
		}

		protected WfClientProcessInfoCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}
	}
}
