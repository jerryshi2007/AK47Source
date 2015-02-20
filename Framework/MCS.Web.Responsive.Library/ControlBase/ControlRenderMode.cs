using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// �ؼ����ģʽ
	/// </summary>
	/// <remarks>�ؼ����ģʽ</remarks>
    public class ControlRenderMode
    {
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <remarks>���캯��</remarks>
        public ControlRenderMode()
        {
            this._ContentTypeKey = ResponseContentTypeKey.HTML.ToString();
            this.DispositionType = ResponseDispositionType.InnerBrowser;
        }

		/// <summary>
		/// ͨ��ҳ�����ģʽ�������UniqueIDΪcontrolUniqueID�Ŀؼ����ģʽ
		/// </summary>
		/// <param name="pageRenderMode">ҳ�����ģʽ</param>
		/// <remarks>ͨ��ҳ�����ģʽ�������UniqueIDΪcontrolUniqueID�Ŀؼ����ģʽ</remarks>
        public ControlRenderMode(PageRenderMode pageRenderMode)
            : this()
        {
            LoadFromPageRenderMode(pageRenderMode);
        }

		private bool _UseNewPage;
		/// <summary>
		/// �Ƿ�ʹ����ҳ�����
		/// </summary>
		public bool UseNewPage
		{
			get { return _UseNewPage; }
			set { _UseNewPage = value; }
		}

        private bool _OnlyRenderSelf;
        /// <summary>
        /// �Ƿ�ֻ������ؼ�
        /// </summary>
		/// <remarks>�Ƿ�ֻ������ؼ�</remarks>
        public bool OnlyRenderSelf
        {
            get { return _OnlyRenderSelf; }
            set { _OnlyRenderSelf = value; }
        }

        private string _ContentTypeKey;
        /// <summary>
        /// ������ݵ�ContentTypeKey������ContentTypeKey���Բ鵽��Ӧ��ContentType
        /// </summary>
		/// <remarks>������ݵ�ContentTypeKey������ContentTypeKey���Բ鵽��Ӧ��ContentType</remarks>
        public string ContentTypeKey
        {
            get { return _ContentTypeKey; }
            set { _ContentTypeKey = value; }
        }

        private ResponseDispositionType _DispositionType;
        /// <summary>
        /// �ͻ��˴�����
        /// </summary>
		/// <remarks>�ͻ��˴�����</remarks>
        public ResponseDispositionType DispositionType
        {
            get { return _DispositionType; }
            set { _DispositionType = value; }
        }

        private string _AttachmentFileName;
        /// <summary>
        /// ���Ϊ�ļ�����������ļ����ơ�
        /// </summary>
		/// <remarks>���Ϊ�ļ�����������ļ�����</remarks>
        public string AttachmentFileName
        {
            get { return _AttachmentFileName; }
            set { _AttachmentFileName = value; }
        }

        private PageRenderModePageCache _PageCache;
        /// <summary>
        /// ҳ�滺��
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
        /// �������
        /// </summary>
		/// <remarks>�������</remarks>
        public string RenderArgument
        {
            get { return _RenderArgument; }
            set { _RenderArgument = value; }
        }

		/// <summary>
		/// �Ƿ�ΪHtml���״̬
		/// </summary>
		/// <remarks>�Ƿ�ΪHtml���״̬</remarks>
        public bool IsHtmlRender
        {
            get
            {
                return string.Compare(this.ContentTypeKey, ResponseContentTypeKey.HTML.ToString(), true) == 0;
            }
        }

		/// <summary>
		/// ͨ��ҳ�����״̬�����ؿؼ������״̬
		/// </summary>
		/// <param name="pageRenderMode">ҳ�����ģʽ</param>
		/// <remarks>ͨ��ҳ�����״̬�����ؿؼ������״̬</remarks>
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
