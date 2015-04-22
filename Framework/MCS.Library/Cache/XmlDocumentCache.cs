using MCS.Library.Caching;
using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace MCS.Web.Library
{
    /// <summary>
    /// Web应用中，Xml文档对象的缓存类
    /// </summary>
    public static class WebXmlDocumentCache
    {
        /// <summary>
        /// 从虚目录加载Xml文档，加载过的文档会缓存在Cache中
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static XmlDocument GetXmlDocument(string virtualPath)
        {
            return GetContentFromVirtualPath(
                virtualPath,
                WebXmlDocumentCacheQueue.Instance,
                (path) => XmlHelper.LoadDocument(path));
        }

        /// <summary>
        /// 异步从虚目录加载Xml文档，加载过的文档会缓存在Cache中
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static async Task<XmlDocument> GetXmlDocumentAsync(string virtualPath)
        {
            return await GetContentFromVirtualPathAsync(
                virtualPath,
                WebXmlDocumentCacheQueue.Instance,
                (path) => XmlHelper.LoadDocumentAsync(path));
        }

        /// <summary>
        /// 异步得到虚拟目录所对应文件的文本
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static async Task<string> GetDocumentAsync(string virtualPath)
        {
            return await GetContentFromVirtualPathAsync(
                virtualPath,
                WebDocumentCacheQueue.Instance,
                (path) =>
                {
                    using (Stream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        StreamReader reader = new StreamReader(stream);

                        return reader.ReadToEndAsync();
                    }
                });
        }

        /// <summary>
        /// 得到虚拟目录所对应文件的文本
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static string GetDocument(string virtualPath)
        {
            return GetContentFromVirtualPath(
                virtualPath,
                WebDocumentCacheQueue.Instance,
                (path) => File.ReadAllText(path));
        }

        private static TResult GetContentFromVirtualPath<TResult, TCache>(string virtualPath, TCache cache, Func<string, TResult> getContent) where TCache : CacheQueue<string, TResult>
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(virtualPath, "virtualPath");

            TResult result = default(TResult);

            HttpContext context = HttpContext.Current;

            string strPath = context.Server.MapPath(virtualPath).ToLower();

            if (cache.TryGetValue(strPath, out result) == false)
            {
                result = getContent(strPath);

                FileCacheDependency dependency = new FileCacheDependency(strPath);

                cache.Add(strPath, result, dependency);
            }

            return result;
        }

        private static async Task<TResult> GetContentFromVirtualPathAsync<TResult, TCache>(string virtualPath, TCache cache, Func<string, Task<TResult>> getContent) where TCache : CacheQueue<string, TResult>
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(virtualPath, "virtualPath");

            TResult result = default(TResult);

            HttpContext context = HttpContext.Current;

            string strPath = context.Server.MapPath(virtualPath).ToLower();

            if (cache.TryGetValue(strPath, out result) == false)
            {
                result = await getContent(strPath);

                FileCacheDependency dependency = new FileCacheDependency(strPath);

                cache.Add(strPath, result, dependency);
            }

            return result;
        }
    }

    #region WebXmlDocumentCacheQueue
    /// <summary>
    /// 存储XmlDocument的Cache Queue
    /// </summary>
    internal sealed class WebXmlDocumentCacheQueue : CacheQueue<string, XmlDocument>
    {
        public static readonly WebXmlDocumentCacheQueue Instance = CacheManager.GetInstance<WebXmlDocumentCacheQueue>();

        //实现SingleTon模式
        private WebXmlDocumentCacheQueue()
        {
        }
    }
    #endregion

    internal sealed class WebDocumentCacheQueue : CacheQueue<string, string>
    {
        public static readonly WebDocumentCacheQueue Instance = CacheManager.GetInstance<WebDocumentCacheQueue>();

        //实现SingleTon模式
        private WebDocumentCacheQueue()
        {
        }
    }
}
