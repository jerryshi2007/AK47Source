using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Caching
{
	/// <summary>
	/// 内存映射文件的Cache依赖项
	/// </summary>
	public class MemoryMappedFileNotifierCacheDependency : NotifierCacheDependencyBase
	{
		/// <summary>
		/// 得到监听器
		/// </summary>
		/// <returns></returns>
		protected override NotifierCacheMonitorBase GetMonitor()
		{
			return MemoryMappedFileCacheMonitor.Instance;
		}
	}
}
