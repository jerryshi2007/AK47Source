using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using MCS.Library.Core;
using System.IO;

namespace MCS.Library.Caching
{
    /// <summary>
    /// 用于Web应用清理Cache的HttpModule
    /// </summary>
    public sealed class CacheModule : IHttpModule
    {
        private static DateTime lastScavengeTime = DateTime.Now;
        private static object syncObject = new object();

        #region IHttpModule 成员
        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            return;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context">HttpApplication对象</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        #endregion

        #region 私有方法
        private void context_BeginRequest(object sender, EventArgs e)
        {
            //CheckAndExecuteScavenge();

            if (CacheSettingsSection.GetConfig().EnableCacheInfoPage)
            {
                if (HttpContext.Current.Request.Url.ToString().LastIndexOf("DeluxeCacheInfo.axd", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;

                    if (HttpContext.Current.Request.HttpMethod.ToUpper() == "POST")
                    {
                        ClearCacheQueue(HttpContext.Current.Request.Form["cacheQueueTypeName"]);
                    }

                    ShowCacheInfo();

                    HttpContext.Current.Response.End();
                }
            }
        }

        private static void ClearCacheQueue(string queueTypeName)
        {
            if (queueTypeName.IsNotEmpty())
            {
                if (queueTypeName == "ClearAllCache")
                {
                    CacheManager.ClearAllCache();
                }
                else
                {
                    Type type = TypeCreator.GetTypeInfo(queueTypeName);

                    CacheManager.GetInstance(type).Clear();
                }
            }
        }

        private static void ShowCacheInfo()
        {
            CacheQueueInfoCollection queues = CacheManager.GetAllCacheInfo();

            string template = GetPageTemplate();

            template = template.Replace("$Summary$", GetSummary(queues));

            string queuesInfo = GetAllQueueInfo(queues);

            template = template.Replace("$QueueInfoLines$", GetAllQueueInfo(queues));

            HttpContext.Current.Response.Write(template);
        }

        private static string GetAllQueueInfo(CacheQueueInfoCollection queues)
        {
            StringBuilder strB = new StringBuilder();

            using (TextWriter sw = new StringWriter(strB))
            {
                HtmlTextWriter writer = new HtmlTextWriter(sw);

                foreach (CacheQueueInfo queueInfo in queues)
                    RenderQueueInfo(writer, queueInfo);

                return strB.ToString();
            }
        }

        /// <summary>
        /// 渲染一个Cache队列的内容
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="queueInfo"></param>
        private static void RenderQueueInfo(HtmlTextWriter writer, CacheQueueInfo queueInfo)
        {
            writer.WriteBeginTag("div");

            writer.WriteAttribute("class", "queueInfoTitle");
            writer.Write(">");

            writer.Write(HttpUtility.HtmlEncode(GetQueueInfo(queueInfo)));

            RenderCacheQueueClearButton(writer, queueInfo.QueueTypeFullName);

            writer.WriteEndTag("div");

            RenderCacheQueueItemsInfo(writer, queueInfo);
        }

        /// <summary>
        /// 渲染清理Cache的按钮
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="queueTypeName"></param>
        private static void RenderCacheQueueClearButton(HtmlTextWriter writer, string queueTypeName)
        {
            writer.WriteBeginTag("input");

            writer.WriteAttribute("type", "submit");
            writer.WriteAttribute("class", "clearButton");
            writer.WriteAttribute("value", "清空");
            writer.WriteAttribute("onclick",
                string.Format("document.getElementById('cacheQueueTypeName').value='{0}'", queueTypeName));
            writer.Write("/>");
        }

        /// <summary>
        /// 渲染每一条Cache项
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="queueInfo"></param>
        private static void RenderCacheQueueItemsInfo(HtmlTextWriter writer, CacheQueueInfo queueInfo)
        {
            Type type = TypeCreator.GetTypeInfo(queueInfo.QueueTypeFullName);

            CacheQueueBase cacheQueue = CacheManager.GetInstance(type);

            CacheItemInfoCollection itemsInfo = cacheQueue.GetAllItemsInfo();

            foreach (CacheItemInfo itemInfo in itemsInfo)
            {
                writer.WriteBeginTag("div");

                writer.WriteAttribute("class", "queueInfoDetail");
                writer.Write(">");

                writer.Write(HttpUtility.HtmlEncode(string.Format("Key={0}, Value={1}", itemInfo.Key, itemInfo.Value)));

                writer.WriteEndTag("div");
            }
        }

        private static string GetQueueInfo(CacheQueueInfo queueInfo)
        {
            return string.Format("{0}，项数为{1:#,##0}", queueInfo.QueueTypeName, queueInfo.Count);
        }

        private static string GetSummary(CacheQueueInfoCollection queues)
		{
			StringBuilder strB = new StringBuilder();

			using (TextWriter sw = new StringWriter(strB))
			{
			    HtmlTextWriter writer = new HtmlTextWriter(sw);
				
                if (queues.Count > 0)
				{
					writer.Write("(总共{0}个缓存队列)", queues.Count);

					RenderCacheQueueClearButton(writer, "ClearAllCache");
				}
			}

			return strB.ToString();
		}

        private static string GetPageTemplate()
        {
            return Assembly.GetExecutingAssembly().LoadStringFromResource("MCS.Library.Cache.Templates.cacheInfoTemplate.htm");
        }

        //private void CheckAndExecuteScavenge()
        //{
        //	以后不使用这种方式清理缓存了
        //    TimeSpan interval = CacheSettingsSection.GetConfig().ScanvageInterval;
        //
        //    if (DateTime.Now.Subtract(CacheModule.lastScavengeTime) > interval && CacheManager.InScavengeThread == false)
        //    {
        //        lock (syncObject)
        //        {
        //            if (CacheManager.InScavengeThread == false &&
        //                DateTime.Now.Subtract(CacheModule.lastScavengeTime) > interval)
        //            {
        //                CacheManager.StartScavengeThread();
        //                CacheModule.lastScavengeTime = DateTime.Now;
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
