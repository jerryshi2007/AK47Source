using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MCS.Library.Caching;

namespace MCS.Library.Accredit.OguAdmin.Caching
{
	internal class GetObjectsDetailQueue : CacheQueue<string, DataSet>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly GetObjectsDetailQueue Instance = CacheManager.GetInstance<GetObjectsDetailQueue>();
		private GetObjectsDetailQueue() { }
	}
}
