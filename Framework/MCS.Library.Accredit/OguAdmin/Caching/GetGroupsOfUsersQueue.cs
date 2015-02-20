using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;

using MCS.Library.Caching;

namespace MCS.Library.Accredit.OguAdmin.Caching
{
	internal class GetGroupsOfUsersQueue : CacheQueue<string, DataSet>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly GetGroupsOfUsersQueue Instance = CacheManager.GetInstance<GetGroupsOfUsersQueue>();
		private GetGroupsOfUsersQueue() { }
	}

}
