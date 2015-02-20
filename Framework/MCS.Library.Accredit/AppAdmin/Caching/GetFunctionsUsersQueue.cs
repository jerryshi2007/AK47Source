#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Accredit
// FileName	：	GetFunctionsUsersQueue.cs
// Remark	：		GetFunctionsUsers接口实现上的数据缓存队列的实现
// -------------------------------------------------
// VERSION  	AUTHOR				DATE					CONTENT
// 1.0				ccic\yuanyong		20081216			新创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Accredit.AppAdmin.Caching
{
	internal class GetFunctionsUsersQueue : CacheQueue<string, System.Data.DataSet>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly GetFunctionsUsersQueue Instance = CacheManager.GetInstance<GetFunctionsUsersQueue>();
		private GetFunctionsUsersQueue() { }
	}
}
