using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
	/// <summary>
	/// 这个管理类管理了每一个服务方法的缓存队列，它本身受全局的CacheManager管理
	/// </summary>
	public class ServiceMethodCacheManager : CacheQueue<string, ServiceMethodCache>
	{
		/// <summary>
		/// Cache的实例
		/// </summary>
		public static readonly ServiceMethodCacheManager Instance = CacheManager.GetInstance<ServiceMethodCacheManager>();
	}
}