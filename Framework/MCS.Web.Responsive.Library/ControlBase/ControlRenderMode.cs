using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// 控件输出模式
	/// </summary>
	/// <remarks>控件输出模式</remarks>
    public class ControlRenderMode
    {
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <remarks>构造函数</remarks>
        public ControlRenderMode()
        {
            this._ContentTypeKey = ResponseContentTypeKey.HTML.ToString();
            this.DispositionType = ResponseDispositionType.InnerBrowser;
        }

		/// <summary>
		/// 通过页面输出模式，构造出UniqueID为controlUniqueID的控件输出模式
		/// </summary>
		/// <param name="pageRenderMode">页面输出模式</param>
		/// <remarks>通过页面输出模式，构造出UniqueID为controlUniqueID的控件输出模式</remarks>
        public ControlRenderMode(PageRenderMode pageRenderMode)
            : this()
        {
            LoadFromPageRenderMode(pageRenderMode);
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

        private bool _OnlyRenderSelf;
        /// <summary>
        /// 是否只输出本控件
        /// </summary>
		/// <remarks>是否只输出本控件</remarks>
        public bool OnlyRenderSelf
        {
            get { return _OnlyRenderSelf; }
            set { _OnlyRenderSelf = value; }
        }

        private string _ContentTypeKey;
        /// <summary>
        /// 输出内容的ContentTypeKey，根据ContentTypeKey可以查到相应的ContentType
        /// </summary>
		/// <remarks>输出内容的ContentTypeKey，根据ContentTypeKey可以查到相应的ContentType</remarks>
        public string ContentTypeKey
        {
            get { return _ContentTypeKey; }
            set { _ContentTypeKey = value; }
        }

        private ResponseDispositionType _DispositionType;
        /// <summary>
        /// 客户端打开类型
        /// </summary>
		/// <remarks>客户端打开类型</remarks>
        public ResponseDispositionType DispositionType
        {
            get { return _DispositionType; }
            set { _DispositionType = value; }
        }

        private string _AttachmentFileName;
        /// <summary>
        /// 如果为文件输出则设置文件名称。
        /// </summary>
		/// <remarks>如果为文件输出则设置文件名称</remarks>
        public string AttachmentFileName
        {
            get { return _AttachmentFileName; }
            set { _AttachmentFileName = value; }
        }

        private PageRenderModePageCache _PageCache;
        /// <summary>
        /// 页面缓存
        /// </summary>
        public PageRenderModePageCache PageCache
        {
            get
            {
                return _PageCache;
            }
        }

        private string _RenderArgument;
        /// <summary>
        /// 输出参数
        /// </summary>
		/// <remarks>输出参数</remarks>
        public string RenderArgument
        {
            get { return _RenderArgument; }
            set { _RenderArgument = value; }
        }

		/// <summary>
		/// 是否为Html输出状态
		/// </summary>
		/// <remarks>是否为Html输出状态</remarks>
        public bool IsHtmlRender
        {
            get
            {
                return string.Compare(this.ContentTypeKey, ResponseContentTypeKey.HTML.ToString(), true) == 0;
            }
        }

		/// <summary>
		/// 通过页面输出状态，加载控件的输出状态
		/// </summary>
		/// <param name="pageRenderMode">页面输出模式</param>
		/// <remarks>通过页面输出状态，加载控件的输出状态</remarks>
		public void LoadFromPageRenderMode(PageRenderMode pageRenderMode)
        {
            //if (!string.IsNullOrEmpty(controlUniqueID) && controlUniqueID == pageRenderMode.RenderControlUniqueID)
            //{
            //    this._OnlyRenderSelf = true;
            //}
			this._UseNewPage = pageRenderMode.UseNewPage;
            this._ContentTypeKey = pageRenderMode.ContentTypeKey;
            this._DispositionType = pageRenderMode.DispositionType;
            this._AttachmentFileName = pageRenderMode.AttachmentFileName;
            this._PageCache = pageRenderMode.PageCache;
            this._RenderArgument = pageRenderMode.RenderArgument;

        }
    }
}
