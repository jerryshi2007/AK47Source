using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;

using MCS.Library.Caching;

namespace MCS.Library.Accredit.OguAdmin.Caching
{
	internal class GetObjectParentOrgsQueue : CacheQueue<string, DataSet>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly GetObjectParentOrgsQueue Instance = CacheManager.GetInstance<GetObjectParentOrgsQueue>();
		private GetObjectParentOrgsQueue() { }
	}
}
