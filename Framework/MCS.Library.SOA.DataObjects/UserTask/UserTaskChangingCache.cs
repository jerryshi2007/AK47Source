using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 用户UserTask变更状态的Cache
	/// </summary>
	public sealed class UserTaskChangingCache : CacheQueue<string, string>
	{
		public static readonly UserTaskChangingCache Instance = CacheManager.GetInstance<UserTaskChangingCache>();

		private UserTaskChangingCache()
		{
		}
	}
}
