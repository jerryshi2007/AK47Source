using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Accredit.OguAdmin.Caching
{
	internal class SignInCheckQueue : CacheQueue<string, bool>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly SignInCheckQueue Instance = CacheManager.GetInstance<SignInCheckQueue>();
		private SignInCheckQueue() { }
	}
}
