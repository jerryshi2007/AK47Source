using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Caching;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 应用路径映射的Context
    /// </summary>
    public class AppPathMappingContext : IDisposable
    {
        private string sourcePathName = string.Empty;
        private string destinationPathName = string.Empty;

        private AppPathMappingContext()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public string SourcePathName
        {
            get { return sourcePathName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DestinationPathName
        {
            get { return destinationPathName; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcPathName"></param>
        /// <param name="destPathName"></param>
        /// <returns></returns>
        public static AppPathMappingContext CreateMapping(string srcPathName, string destPathName)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(srcPathName, "srcPathName");
            ExceptionHelper.CheckStringIsNullOrEmpty(destPathName, "destPathName");

            ExceptionHelper.TrueThrow(AppPathMappingContextCache.Instance.ContainsKey(srcPathName),
                "已经为AppPath\"{0}\"建立了映射关系", srcPathName);

            AppPathMappingContext context = new AppPathMappingContext();

            context.sourcePathName = srcPathName;
            context.destinationPathName = destPathName;

            AppPathMappingContextCache.Instance.Add(context.sourcePathName, context);

            return context;
        }

        public static string GetMappedPathName(string srcPathName)
        {
            AppPathMappingContext context = null;
            string result = srcPathName;

            if (AppPathMappingContextCache.Instance.TryGetValue(srcPathName, out context))
                result = context.destinationPathName;

            return result;
        }

        #region IDisposable Members

        public void Dispose()
        {
            AppPathMappingContext context = null;

            if (AppPathMappingContextCache.Instance.TryGetValue(this.sourcePathName, out context))
            {
                AppPathMappingContextCache.Instance.Remove(this.sourcePathName);
            }
        }

        #endregion
    }

    internal class AppPathMappingContextCache : ContextCacheQueueBase<string, AppPathMappingContext>
    {
        public static AppPathMappingContextCache Instance
        {
            get
            {
                return ContextCacheManager.GetInstance<AppPathMappingContextCache>();
            }
        }
    }
}
