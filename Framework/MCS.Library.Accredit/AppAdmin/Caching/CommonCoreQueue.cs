#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Accredit
// FileName	：	CommonCoreQueue.cs
// Remark	：		通用字符串数据缓存队列的实现
// -------------------------------------------------
// VERSION  	AUTHOR				DATE					CONTENT
// 1.0				ccic\yuanyong		2008121630		新创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Accredit.AppAdmin.Caching
{
	internal class CommonCoreQueue : CacheQueue<string, string>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly CommonCoreQueue Instance = CacheManager.GetInstance<CommonCoreQueue>();
		private CommonCoreQueue() { }
	}
}
