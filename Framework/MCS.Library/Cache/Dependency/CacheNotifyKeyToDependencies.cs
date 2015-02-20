using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MCS.Library.Caching
{
	/// <summary>
	/// CacheNotifyKey和依赖项之间的关系
	/// </summary>
	[Serializable]
	public class CacheNotifyKeyToDependencies : Dictionary<CacheNotifyKey, DependencyBase>
	{
        /// <summary>
        /// 
        /// </summary>
	    public CacheNotifyKeyToDependencies()
	    {
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected CacheNotifyKeyToDependencies(SerializationInfo info, StreamingContext context)
            : base(info, context)
	    {
	    }
	}
}
