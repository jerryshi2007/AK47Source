using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程运行时的参数，通常是上下文有效
	/// </summary>
	[Serializable]
	public class WfRuntimeParameters
	{
		private bool _AutoloadActions = true;

		/// <summary>
		/// 是否从配置文件自动加载Action
		/// </summary>
		public bool AutoloadActions
		{
			get
			{
				return this._AutoloadActions;
			}
			set
			{
				this._AutoloadActions = value;
			}
		}
	}
}
