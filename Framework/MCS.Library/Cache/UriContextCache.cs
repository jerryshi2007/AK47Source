using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
	/// <summary>
	/// Uri对象的上下文缓存
	/// </summary>
	public sealed class UriContextCache : ContextCacheQueueBase<string, Uri>
	{
		/// <summary>
		/// 此处必须是属性，动态计算，不能是ReadOnly变量
		/// </summary>
		public static UriContextCache Instance
		{
			get
			{
				return ContextCacheManager.GetInstance<UriContextCache>();
			}
		}

		/// <summary>
		/// 得到字符串所对应的Uri对象。
		/// </summary>
		/// <param name="uriString"></param>
		/// <returns></returns>
		public static Uri GetUri(string uriString)
		{
			Uri result = null;

			if (uriString.IsNotEmpty())
			{
				if (UriContextCache.Instance.TryGetValue(uriString, out result) == false)
				{
					result = UriHelper.ResolveUri(uriString);

					UriContextCache.Instance.Add(uriString, result);
				}
			}

			return result;
		}

		private UriContextCache()
		{
		}
	}
}
