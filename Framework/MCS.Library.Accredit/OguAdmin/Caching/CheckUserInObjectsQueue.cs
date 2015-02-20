using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Caching;

namespace MCS.Library.Accredit.OguAdmin.Caching
{

	internal class CheckUserInObjectsQueue : CacheQueue<string, XmlDocument>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly CheckUserInObjectsQueue Instance = CacheManager.GetInstance<CheckUserInObjectsQueue>();
		private CheckUserInObjectsQueue() { }
	}
}
