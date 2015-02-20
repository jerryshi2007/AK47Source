using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Validation
{
	internal class ValidatorCache : CacheQueue<string, List<Validator>> 
    {
        public static readonly ValidatorCache validatorCacheQueue = CacheManager.GetInstance<ValidatorCache>();

        private ValidatorCache()
        {
        }
    }
}
