using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	internal class WfApplicationAuthCache : CacheQueue<string, WfApplicationAuthCollection>
	{
		public static readonly WfApplicationAuthCache Instance = CacheManager.GetInstance<WfApplicationAuthCache>();
	}
}
