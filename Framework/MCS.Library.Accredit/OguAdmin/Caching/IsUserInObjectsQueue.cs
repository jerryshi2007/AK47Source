using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Accredit.OguAdmin.Caching
{
	internal class IsUserInObjectsQueue : CacheQueue<string, bool>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly IsUserInObjectsQueue Instance = CacheManager.GetInstance<IsUserInObjectsQueue>();
		private IsUserInObjectsQueue() { }
	}
}
