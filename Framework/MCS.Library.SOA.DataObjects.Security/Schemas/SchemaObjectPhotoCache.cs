using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 对象的图片缓存
	/// </summary>
	public class SchemaObjectPhotoCache : CacheQueue<SchemaObjectPhotoKey, SchemaObjectPhoto>
	{
		public static readonly SchemaObjectPhotoCache Instance = CacheManager.GetInstance<SchemaObjectPhotoCache>();
	}
}
