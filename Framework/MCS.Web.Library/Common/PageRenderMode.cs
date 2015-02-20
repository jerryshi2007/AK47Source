using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Web.UI;

using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Web.Library
{
	/// <summary>
	/// 定义页面的输出状态
	/// </summary>
	/// <remarks>定义页面的输出状态</remarks>
    public class PageRenderMode
    {
        /// <summary>
        /// 页面输出状态
        /// </summary>
		/// <remarks>页面输出状态</remarks>
        public PageRenderMode()
        {
            this._ContentTypeKey = ResponseContentTypeKey.HTML.ToString();
            this._DispositionType = ResponseDispositionType.InnerBrowser;
        }
        
        /// <summary>
        /// 单独页面输出某控件状态
        /// </summary>
		/// <param name="renderControlUniqueID">单独输出控件UniqueID</param>
		/// <param name="renderArgument">输出参数</param>
		/// <remarks>单独页面输出某控件状态</remarks>
        public PageRenderMode(string renderControlUniqueID, string renderArgument)
            : this()
        {
			this._UseNewPage = true;
            this._RenderControlUniqueID = renderControlUniqueID;
            this._RenderArgument = renderArgument;
        }

        /// <summary>
        /// 文件方式输出页面状态
        /// </summary>
        /// <param name="contentTypeKey">页面内容类型ContentType的Key值</param>
        /// <param name="dispositionType">页面打开文件类型</param>
        /// <param name="attachmentFileName">文件名称</param>
        /// <param name="renderArgument">输出参数</param>
        public PageRenderMode(string contentTypeKey, ResponseDispositionType dispositionType, string attachmentFileName, string renderArgument)
        {
			this._UseNewPage = true;
            this._ContentTypeKey = contentTypeKey;
            this._DispositionType = dispositionType;
            this._AttachmentFileName = attachmentFileName;
            this._RenderArgument = renderArgument;
        }

        /// <summary>
        /// 文件方式输出页面状态
        /// </summary>
		/// <param name="contentTypeKey">页面内容类型ContentType的Key值</param>
		/// <param name="dispositionType">页面内容类型</param>
		/// <param name="attachmentFileName">页面打开文件类型</param>
		/// <param name="renderArgument">输出参数</param>
        public PageRenderMode(ResponseContentTypeKey contentTypeKey, ResponseDispositionType dispositionType, string attachmentFileName, string renderArgument)
            : this(contentTypeKey.ToString(), dispositionType, attachmentFileName, renderArgument)
        {          
        }

        /// <summary>
        /// 文件方式输出某控件状态
        /// </summary>
		/// <param name="contentTypeKey">页面内容类型ContentType的Key值</param>
		/// <param name="dispositionType">页面内容类型</param>
		/// <param name="attachmentFileName">页面打开文件类型</param>
		/// <param name="renderControlUniqueID">单独输出控件UniqueID</param>
		/// <param name="renderArgument">输出参数</param>
        public PageRenderMode(string contentTypeKey, ResponseDispositionType dispositionType, string attachmentFileName, string renderControlUniqueID, string renderArgument)
            : this(contentTypeKey, dispositionType, attachmentFileName, renderArgument)
        {
			this._UseNewPage = false;
            this._RenderControlUniqueID = renderControlUniqueID;          
        }
        /// <summary>
        /// 文件方式输出某控件状态
        /// </summary>
		/// <param name="contentTypeKey">页面内容类型ContentType的Key值</param>
		/// <param name="dispositionType">页面内容类型</param>
		/// <param name="attachmentFileName">页面打开文件类型</param>
		/// <param name="renderControlUniqueID">单独输出控件UniqueID</param>
		/// <param name="renderArgument">输出参数</param>
        public PageRenderMode(ResponseContentTypeKey contentTypeKey, ResponseDispositionType dispositionType, string attachmentFileName, string renderControlUniqueID, string renderArgument)
            : this(contentTypeKey.ToString(), dispositionType, attachmentFileName, renderControlUniqueID, renderArgument)
        {
        }

		/// <summary>
		/// 通过字符串数据构造实例
		/// </summary>
		/// <param name="strData">字符串数据</param>
        public PageRenderMode(string strData)
        {
            this.Load(strData);
        }

		private bool _UseNewPage;
		/// <summary>
		/// 是否使用新页面输出
		/// </summary>
		public bool UseNewPage
		{
			get { return _UseNewPage; }
			set { _UseNewPage = value; }
		}

        private string _ContentTypeKey;
        /// <summary>
        /// 输出内容的ContentTypeKey，根据ContentTypeKey可以查到相应的ContentType
        /// </summary>
        public string ContentTypeKey
        {
            get { return _ContentTypeKey; }
            set { _ContentTypeKey = value; }
        }

        private ResponseDispositionType _DispositionType;

        /// <summary>
        /// HttpResponse 返回流 客户端打开类型
        /// </summary>
        public ResponseDispositionType DispositionType
        {
            get { return _DispositionType; }
            set { _DispositionType = value; }
        }

        private string _AttachmentFileName;

        /// <summary>
        /// 如果为文件输出则设置文件名称。
        /// </summary>
        public string AttachmentFileName
        {
            get { return _AttachmentFileName; }
            set { _AttachmentFileName = value; }
        }

        private string _RenderControlUniqueID;
        /// <summary>
        /// 输出控件的UniqueID，如果不单独输出某一控件，则不设置
        /// </summary>
        public string RenderControlUniqueID
        {
            get { return _RenderControlUniqueID; }
            set { _RenderControlUniqueID = value; }
        }

        private PageRenderModePageCache _PageCache = null;
        /// <summary>
        /// 页面缓存
        /// </summary>
        public PageRenderModePageCache PageCache
        {
            get 
            {
                return _PageCache == null ? PageRenderModeHelper.GetPageRenderModeCache(WebUtility.GetCurrentPage()) : _PageCache;
            }
            set { _PageCache = value; }
        }

        private string _RenderArgument;
        /// <summary>
        /// 输出参数
        /// </summary>
        public string RenderArgument
        {
            get { return _RenderArgument; }
            set { _RenderArgument = value; }
        }
        
		/// <summary>
		/// 转换成字符串数据
		/// 
		/// </summary>
		/// <returns>字符串数据</returns>
        public override string ToString()
        {
            return string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
               _UseNewPage, _ContentTypeKey, _DispositionType, _AttachmentFileName, _RenderControlUniqueID, PageRenderModeHelper.GetStringFromPageRenderModeCache(PageCache), _RenderArgument);
        }

		/// <summary>
		/// 是否位Html输出状态
		/// </summary>
        public bool IsHtmlRender
        {
            get
            {
                return string.Compare(this.ContentTypeKey, ResponseContentTypeKey.HTML.ToString(), true) == 0;
            }
        }

        private void Load(string strData)
        {
            string[] strArray = strData.Split('^');
			_UseNewPage = bool.Parse(strArray[0]);
            _ContentTypeKey = strArray[1];
            _DispositionType = (ResponseDispositionType)Enum.Parse(typeof(ResponseDispositionType), strArray[2], true);
            _AttachmentFileName = strArray[3];
            _RenderControlUniqueID = strArray[4];
            _PageCache = PageRenderModeHelper.GetPageRenderModeCacheFromString(strArray[5]);
            _RenderArgument = strArray[6];
        }
    }

    /// <summary>
    /// PageRenderModePageCache
    /// </summary>
    public class PageRenderModePageCache : Dictionary<string, object>
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defautValue"></param>
        /// <returns></returns>
        public T GetValue<T>(string key, T defautValue)
        {
            T v = DictionaryHelper.GetValue<string, object, T>(this, key, defautValue);

            return v;
        }
    }

    internal static class PageRenderModeHelper
    {
        private static object _S_PageRenderModeCacheKey = new object();

        public static PageRenderModePageCache GetPageRenderModeCache(Page page)
        {
            PageRenderModePageCache cache = (PageRenderModePageCache)page.Items[_S_PageRenderModeCacheKey];

            if (cache == null)
            {
                cache = new PageRenderModePageCache();
                page.Items[_S_PageRenderModeCacheKey] = cache;
            }

            return cache;
        }

        public static string GetStringFromPageRenderModeCache(PageRenderModePageCache cache)
        {
            return JSONSerializerExecute.Serialize(cache);
        }

        public static PageRenderModePageCache GetPageRenderModeCacheFromString(string str)
        {
            return String.IsNullOrEmpty(str) ? new PageRenderModePageCache() : JSONSerializerExecute.Deserialize<PageRenderModePageCache>(str);
        }
    }
}
