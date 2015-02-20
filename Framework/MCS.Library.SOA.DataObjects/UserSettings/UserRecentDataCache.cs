using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 表示用户最近数据的缓存
    /// </summary>
    internal sealed class UserRecentDataCache : CacheQueue<string, UserRecentData>
    {
        public static readonly UserRecentDataCache Instance = CacheManager.GetInstance<UserRecentDataCache>();

        UserRecentDataCache()
        {
        }

    }

    internal sealed class UserRecentDataCategoryCache : CacheQueue<UserRecentDataCategoryCacheKey, UserRecentDataCategory>
    {
        public static readonly UserRecentDataCategoryCache Instance = CacheManager.GetInstance<UserRecentDataCategoryCache>();

        UserRecentDataCategoryCache()
        {
        }

    }




    /// <summary>
    /// 依赖于当前的架构配置，配置文件修改后缓存即失效
    /// </summary>
    internal class UserRecentDataCacheItemDependency : DependencyBase
    {
        private int originalConfigHashCode = 0;

        public UserRecentDataCacheItemDependency()
        {
            this.originalConfigHashCode = UserRecentDataConfigurationSection.GetConfig().GetHashCode();
        }

        public override bool HasChanged
        {
            get
            {
                return this.originalConfigHashCode != UserRecentDataConfigurationSection.GetConfig().GetHashCode();
            }
        }
    }





}
