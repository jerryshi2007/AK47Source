using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	internal class WfProcessContextCache : ContextCacheQueueBase<string, IWfProcess>
	{
		public static WfProcessContextCache Instance
		{
			get
			{
				return ContextCacheManager.GetInstance<WfProcessContextCache>();
			}
		}
	}
}
