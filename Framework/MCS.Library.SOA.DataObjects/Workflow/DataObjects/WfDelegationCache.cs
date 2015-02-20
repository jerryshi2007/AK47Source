using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 用户委托信息的缓存，Key是SourceUserID
	/// </summary>
	internal sealed class WfDelegationCache : CacheQueue<string, WfDelegationCollection>
	{
		public static readonly WfDelegationCache Instance = CacheManager.GetInstance<WfDelegationCache>();

		private WfDelegationCache()
		{
		}
	}
}
