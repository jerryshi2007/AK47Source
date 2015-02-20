using MCS.Library.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 对象比较信息的缓存
    /// </summary>
    internal class ObjectCompareInfoCache : CacheQueue<System.Type, ObjectCompareInfo>
    {
        public static readonly ObjectCompareInfoCache Instance = CacheManager.GetInstance<ObjectCompareInfoCache>();

        private ObjectCompareInfoCache()
        {
        }
    }
}
