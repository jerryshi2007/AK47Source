using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	public class InvokeServiceTaskAdapter : SysTaskAdapter
	{
		public static readonly new InvokeServiceTaskAdapter Instance = new InvokeServiceTaskAdapter();

		protected InvokeServiceTaskAdapter()
		{
		}

		public override SysTask CreateNewData()
		{
			return new InvokeServiceTask();
		}
	}
}
