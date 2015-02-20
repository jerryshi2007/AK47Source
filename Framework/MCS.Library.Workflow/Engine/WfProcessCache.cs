using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Caching;
using MCS.Library.Workflow.Engine;

namespace MCS.Library.Workflow.Engine
{
	/// <summary>
	/// 在上下文中缓存的流程实例
	/// </summary>
	internal class WfProcessContextCache : ContextCacheQueueBase<string, IWfProcess>
	{
		private WfProcessContextCache()
			: base()
		{
		}

		public static WfProcessContextCache Instance
		{
			get
			{
				return ContextCacheManager.GetInstance<WfProcessContextCache>();
			}
		}
	}

	/// <summary>
	/// 在AppDomain中缓存的流程实例
	/// </summary>
	internal class WfProcessCache : CacheQueue<string, IWfProcess>
	{
		public static readonly WfProcessCache Instance = CacheManager.GetInstance<WfProcessCache>();
	}
}
