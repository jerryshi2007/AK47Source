using CIIC.HSR.TSP.Cache;
using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.WF.Bizlet.Contract.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Cache
{
    public class TaskCachePool : ITaskCachePool
    {
        private static Dictionary<string, ITaskDbLoader> _DbLoaders = new Dictionary<string, ITaskDbLoader>();
        private static object o = new object();
        private static TaskCachePool _Instance = null;
        /// <summary>
        /// 当前实例
        /// </summary>
        public static ITaskCachePool Instance
        {
            get
            {
                lock (o)
                {
                    if (null == _Instance)
                    {
                        RegisterDataLoader();
                        _Instance = new TaskCachePool();
                    }
                }

                return _Instance;
            }
        }

        public static void RegisterDataLoader(string key, ITaskDbLoader dbLoader)
        {
            lock (o)
            {
                if (!_DbLoaders.ContainsKey(key))
                {
                    _DbLoaders.Add(key, dbLoader);
                }
            }
        }
        public void RefreshCacheData(string userId, string tenantCode, bool isTenantMode)
        {
            _DbLoaders.Keys.ToList().ForEach(p =>
            {
                RefreshCacheData(p, userId, tenantCode, isTenantMode);
            });
        }
        public void RefreshCacheData(string key, string userId, string tenantCode, bool isTenantMode)
        {
            string appfabricKey = GetKey(key, userId);
            ICaching chacheManger = Caches.Temporarily;
            chacheManger.Remove(appfabricKey);
        }

        public T GetCacheData<T>(string key, string userId, string tenantCode, bool isTenantMode) where T : class
        {
            ICaching chacheManger = Caches.Temporarily;

            string appfabricKey = GetKey(key, userId);
            T dataCached = (T)chacheManger.Get(appfabricKey);

            if (null == dataCached)
            {
                ITaskDbLoader cacheNeeded = _DbLoaders[key];

                dataCached = cacheNeeded.LoadDataFromDb<T>(userId, tenantCode, isTenantMode);
                chacheManger.Put(appfabricKey, dataCached, cacheNeeded.ExpiredTimeSpan);
            }

            return dataCached;
        }
        private string GetKey(string key, string userId)
        {
            var keyGenerator = new TaskCacheKeyGenerator();
            string generatedKey = keyGenerator.Generate(key, userId);
            return generatedKey;
        }
        private static void RegisterDataLoader()
        {
            var loaders = Containers.Global.Singleton.ResolveAll<ITaskDbLoader>();
            if (null != loaders)
            {
                loaders.ToList().ForEach(p => { RegisterDataLoader(p.ChacheKey, p); });
            }
        }
    }
}
