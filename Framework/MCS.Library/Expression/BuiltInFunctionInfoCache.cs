using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Expression
{
	internal class BuiltInFunctionInfoCache : CacheQueue<Type, BuiltInFunctionInfoCollection>
	{
		public static readonly BuiltInFunctionInfoCache Instance = CacheManager.GetInstance<BuiltInFunctionInfoCache>();
	}
}
