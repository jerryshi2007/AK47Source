#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	ObjectContextCache.cs
// Remark	��	ͨ�����͵��������л�����
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Caching
{
    /// <summary>
    /// ���Key��Value����object��ContextCache
    /// </summary>
    public sealed class ObjectContextCache : ContextCacheQueueBase<object, object>
    {
        /// <summary>
        /// ObjectContextCache��ʵ�����˴����������ԣ���̬����
        /// </summary>
        public static ObjectContextCache Instance
        {
            get
            {
                return ContextCacheManager.GetInstance<ObjectContextCache>();
            }
        }

        private ObjectContextCache()
        {
        }
    }
}
