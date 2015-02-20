using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Caching;
using MCS.Library.Workflow.Engine;

namespace MCS.Library.Workflow.Engine
{
	/// <summary>
	/// ���������л��������ʵ��
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
	/// ��AppDomain�л��������ʵ��
	/// </summary>
	internal class WfProcessCache : CacheQueue<string, IWfProcess>
	{
		public static readonly WfProcessCache Instance = CacheManager.GetInstance<WfProcessCache>();
	}
}
