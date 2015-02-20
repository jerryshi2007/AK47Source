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
    /// WebӦ���У�Xml�ĵ�����Ļ�����
    /// </summary>
    public static class WebXmlDocumentCache
    {
        /// <summary>
        /// ����Ŀ¼����Xml�ĵ������ع����ĵ��Ỻ����Cache��
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
        /// �õ�����Ŀ¼����Ӧ�ļ����ı�
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
    /// �洢XmlDocument��Cache Queue
    /// </summary>
    internal sealed class WebXmlDocumentCacheQueue : CacheQueue<string, XmlDocument>
    {
        public static readonly WebXmlDocumentCacheQueue Instance = CacheManager.GetInstance<WebXmlDocumentCacheQueue>();

        //ʵ��SingleTonģʽ
        private WebXmlDocumentCacheQueue()
        {
        }
    }
    #endregion

    internal sealed class WebDocumentCacheQueue : CacheQueue<string, string>
    {
        public static readonly WebDocumentCacheQueue Instance = CacheManager.GetInstance<WebDocumentCacheQueue>();

        //ʵ��SingleTonģʽ
        private WebDocumentCacheQueue()
        {
        }
    }
}
