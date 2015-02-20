using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	public class WfRuntimeException : SystemSupportException
	{
		public WfRuntimeException()
			: base()
		{
		}

		public WfRuntimeException(string message)
			: base(message)
		{
		}

		public WfRuntimeException(string message, System.Exception ex)
			: base(message, ex)
		{
		}
	}
}
