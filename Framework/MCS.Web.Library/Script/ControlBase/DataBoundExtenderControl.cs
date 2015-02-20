using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Web.UI;

namespace MCS.Web.Library.Script
{
    /// <summary>
	/// ���Դ�������Դ���������ݰ󶨵�ExtenderControl����	
    /// </summary>
	/// <remarks>�ɰ����ݶ��������Դ�ؼ�</remarks>
    public class DataBoundExtenderControl : ExtenderControlBase
    {
        private bool _Inited;
        private bool _PreRendered;
        private bool _RequireDataBinding = false;
        private object _DataSource;
        private DataBoundControlInternal _InnerDataBoundControl;

        #region [ Constructor ]
        /// <summary>
		/// Constructor
        /// </summary>
        public DataBoundExtenderControl()
            : base()
        {
        }
        
        /// <summary>
		/// Constructor
        /// </summary>
        /// <param name="enableClientState">�Ƿ�֧��clientState</param>
        public DataBoundExtenderControl(bool enableClientState)            
            :base(enableClientState)
        {              
        }
        #endregion

        #region DataSource

        private DataBoundControlInternal InnerDataBoundControl
        {
            get
            {
                if (_InnerDataBoundControl == null)
                {
                    _InnerDataBoundControl = new DataBoundControlInternal();
                    this.Controls.Add(_InnerDataBoundControl);
                }

                return _InnerDataBoundControl;
            }
        }

		/// <summary>
		/// ����DataSource��DataSourceID������������Դ���
		/// </summary>
		/// <remarks>������ʹ�ô˴��������а󶨲���</remarks>
		protected IEnumerable DataSourceResult
        {
            get
            {
                if (RequiresDataBinding)
                {
                    RequiresDataBinding = false;
                    InnerDataBoundControl.DataSource = _DataSource;
                    InnerDataBoundControl.DataSourceID = DataSourceID;
                }
                return InnerDataBoundControl.DataSourceResult;
            }
        }

		/// <summary>
		/// ��ȡ����������Դ����
		/// </summary>
		/// <remarks>��ȡ����������Դ����</remarks>
        public object DataSource
        {
            get
            {
                return _DataSource;
            }
            set
            {
                _DataSource = value;
                OnDataPropertyChanged();
            }
        }

		/// <summary>
		/// ��ȡ����������Դ�ؼ�ID
		/// </summary>
		/// <remarks>��ȡ����������Դ�ؼ�ID</remarks>
        [IDReferenceProperty(typeof(DataSourceControl))]
        public string DataSourceID
        {
            get
            {
                return this.GetPropertyValue<string>("DataSourceID", string.Empty);
            }
            set
            {
                this.SetPropertyValue<string>("DataSourceID", value);
                OnDataPropertyChanged();
            }
        }

		/// <summary>
		/// ִ�����ݰ󶨣������ش˷�����������Դ�������󶨴���
		/// </summary>
		/// <param name="data">����Դ������</param>
		/// <remarks>ִ�����ݰ󶨣������ش˷�����������Դ�������󶨴���</remarks>
        protected virtual void PerformDataBinding(IEnumerable data)
        {
        }

		/// <summary>
		/// ������Դ�󶨵��ؼ�
		/// </summary>
		/// <remarks>������Դ�󶨵��ؼ�</remarks>
        public override void DataBind()
        {
            this.PerformDataBinding(this.DataSourceResult);
        }

		/// <summary>
		/// ��ʼ���ؼ�״̬
		/// </summary>
		/// <param name="e">�����¼����ݵ� EventArgs ����</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this._Inited = true;
            if (this.Page != null)
                this.RequiresDataBinding = !this.Page.IsPostBack;
        }

		/// <summary>
		/// �ڱ�����ͼ״̬�ͳ�������֮ǰ��ȷ������Դ��
		/// </summary>
		/// <param name="e">�����¼����ݵ� EventArgs ����</param>
        protected override void OnPreRender(EventArgs e)
        {
            this.EnsureDataBound();
            base.OnPreRender(e);
            this._PreRendered = true;
        }

        private void OnDataPropertyChanged()
        {
            if (this._Inited)
            {
                this.RequiresDataBinding = true;
            }
        }

		/// <summary>
		/// ��ȡ�����Ƿ���Ҫִ��DataBind�������ݰ�
		/// </summary>
		/// <remarks>��ȡ�����Ƿ���Ҫִ��DataBind�������ݰ�</remarks>
        protected bool RequiresDataBinding
        {
            get
            {
                return this._RequireDataBinding;
            }
            set
            {
                if (((value && this._PreRendered) && (this.Page != null)) && !this.Page.IsCallback)
                {
                    this.InnerDataBoundControl.SetRequiresDataBinding(value);
                    this._RequireDataBinding = true;
                    this.EnsureDataBound();
                }
                else
                {
                    this.InnerDataBoundControl.SetRequiresDataBinding(value);
                    this._RequireDataBinding = value;
                }
            }
        }

		/// <summary>
		/// ȷ�ϰ�
		/// </summary>
		/// <remarks>�����Ҫ�󶨣�����а󶨲��������򷵻�</remarks>
        protected virtual void EnsureDataBound()
        {
            if (this.RequiresDataBinding)
            {
                this.DataBind();
                this.RequiresDataBinding = false;
            }
        }
        #endregion

    }
}
