using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// Activity变化的上下文
	/// </summary>
    [Serializable]
	public class WfActivityChangingContext
	{
		internal WfActivityChangingContext()
		{
		}

		public string AssociatedActivityKey
		{
			get;
			set;
		}

		public string CreatorInstanceID
		{
			get;
			set;
		}
	}
}
