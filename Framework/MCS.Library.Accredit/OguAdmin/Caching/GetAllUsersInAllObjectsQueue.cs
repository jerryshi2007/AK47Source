using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Accredit.OguAdmin.Caching
{
	internal class GetAllUsersInAllObjectsQueue : CacheQueue<string, DataSet>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly GetAllUsersInAllObjectsQueue Instance = CacheManager.GetInstance<GetAllUsersInAllObjectsQueue>();
		private GetAllUsersInAllObjectsQueue() { }
	}
}
