using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
	/// <summary>
	/// Web服务方法的Cache队列。这里面存放了同一个方法，不同的参数所对应的返回值
	/// </summary>
	public class ServiceMethodCache : CacheQueue<string, string>
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="instanceName"></param>
		/// <param name="maxLength"></param>
		public ServiceMethodCache(string instanceName, int maxLength)
			: base(true, instanceName, maxLength)
		{
		}
	}
}