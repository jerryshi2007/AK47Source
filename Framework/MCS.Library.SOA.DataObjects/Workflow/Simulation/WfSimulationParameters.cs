using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程仿真参数
	/// </summary>
	[Serializable]
	public class WfSimulationParameters
	{
		private WfVariableDescriptorCollection _Variables = null;
		private bool _EnableServiceCall = false;
		private IUser _Creator = null;

		/// <summary>
		/// 流程的起草人
		/// </summary>
		public IUser Creator
		{
			get
			{
				return this._Creator;
			}
			set
			{
				this._Creator = (IUser)OguBase.CreateWrapperObject(value);
			}
		}

		/// <summary>
		/// 是否启用服务调用
		/// </summary>
		public bool EnableServiceCall
		{
			get
			{
				return this._EnableServiceCall;
			}
			set
			{
				this._EnableServiceCall = value;
			}
		}

		/// <summary>
		/// 流程上下文参数
		/// </summary>
		public WfVariableDescriptorCollection Variables
		{
			get
			{
				if (this._Variables == null)
					this._Variables = new WfVariableDescriptorCollection();

				return this._Variables;
			}
		}
	}
}
