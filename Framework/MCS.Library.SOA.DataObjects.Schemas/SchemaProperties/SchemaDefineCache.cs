using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	/// <summary>
	/// SchemaDefine的缓存类
	/// </summary>
	public class SchemaDefineCache : ContextCacheQueueBase<string, SchemaDefineBase>
	{
		public static SchemaDefineCache Instance
		{
			get
			{
				return ContextCacheManager.GetInstance<SchemaDefineCache>();
			}
		}
	}
}
