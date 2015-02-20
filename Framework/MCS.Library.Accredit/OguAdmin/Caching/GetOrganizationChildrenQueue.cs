using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using MCS.Library.Caching;

namespace MCS.Library.Accredit.OguAdmin.Caching
{
	internal class GetOrganizationChildrenQueue : CacheQueue<string, DataSet>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly GetOrganizationChildrenQueue Instance = CacheManager.GetInstance<GetOrganizationChildrenQueue>();
		private GetOrganizationChildrenQueue() { }
	}
}
