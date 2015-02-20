#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	ORMappingsCache.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���ķ�	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Caching;

namespace MCS.Library.Data.Mapping
{
    internal sealed class ORMappingsCache : CacheQueue<System.Type, ORMappingItemCollection>
    {
        public static readonly ORMappingsCache Instance = CacheManager.GetInstance<ORMappingsCache>();

        private ORMappingsCache()
        {
        }

		internal static object syncRoot = new object();
    }

	/// <summary>
	/// �������е�ORMapping Cache��ͨ��������������ң����û�У���ȥORMappingsCache����
	/// </summary>
	public sealed class ORMappingContextCache : ContextCacheQueueBase<System.Type, ORMappingItemCollection>
	{
		/// <summary>
		/// ��ȡʵ��
		/// </summary>
		public static readonly ORMappingContextCache Instance = ContextCacheManager.GetInstance<ORMappingContextCache>();

		private ORMappingContextCache()
		{
		}
	}
}
