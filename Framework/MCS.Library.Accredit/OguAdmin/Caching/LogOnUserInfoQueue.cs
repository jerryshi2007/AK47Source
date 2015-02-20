using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MCS.Library.Caching;

namespace MCS.Library.Accredit.OguAdmin.Caching
{
	internal class LogOnUserInfoQueue : CacheQueue<string, DataSet>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly LogOnUserInfoQueue Instance = CacheManager.GetInstance<LogOnUserInfoQueue>();
		private LogOnUserInfoQueue() { }
	}
}
