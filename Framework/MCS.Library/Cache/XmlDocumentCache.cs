using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Caching;

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
            virtualPath.CheckStringIsNullOrEmpty("virtualPath");

            XmlDocument result = null;

            HttpContext context = HttpContext.Current;

            string strPath = context.Server.MapPath(virtualPath).ToLower();

            if (WebXmlDocumentCacheQueue.Instance.TryGetValue(strPath, out result) == false)
            {
                result = XmlHelper.LoadDocument(strPath);

                FileCacheDependency dependency = new FileCacheDependency(strPath);

                WebXmlDocumentCacheQueue.Instance.Add(strPath, result, dependency);
            }

            return result;
        }

        /// <summary>
        /// 得到虚拟目录所对应文件的文本
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static string GetDocument(string virtualPath)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(virtualPath, "virtualPath");

            string result = null;

            HttpContext context = HttpContext.Current;

            string strPath = context.Server.MapPath(virtualPath).ToLower();

            if (WebDocumentCacheQueue.Instance.TryGetValue(strPath, out result) == false)
            {
                result = File.ReadAllText(strPath);

                FileCacheDependency dependency = new FileCacheDependency(strPath);

                WebDocumentCacheQueue.Instance.Add(strPath, result, dependency);
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
