using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Web.UI;

namespace MCS.Web.Library.Script
{
    /// <summary>
	/// 可以处理数据源、进行数据绑定的ExtenderControl基类	
    /// </summary>
	/// <remarks>可绑定数据对象或数据源控件</remarks>
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
        /// <param name="enableClientState">是否支持clientState</param>
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
		/// 根据DataSource或DataSourceID，处理后的数据源结果
		/// </summary>
		/// <remarks>开发者使用此处理结果进行绑定操作</remarks>
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
		/// 获取或设置数据源对象
		/// </summary>
		/// <remarks>获取或设置数据源对象</remarks>
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
		/// 获取或设置数据源控件ID
		/// </summary>
		/// <remarks>获取或设置数据源控件ID</remarks>
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
		/// 执行数据绑定，可重载此方法，对数据源处理结果绑定处理
		/// </summary>
		/// <param name="data">数据源处理结果</param>
		/// <remarks>执行数据绑定，可重载此方法，对数据源处理结果绑定处理</remarks>
        protected virtual void PerformDataBinding(IEnumerable data)
        {
        }

		/// <summary>
		/// 将数据源绑定到控件
		/// </summary>
		/// <remarks>将数据源绑定到控件</remarks>
        public override void DataBind()
        {
            this.PerformDataBinding(this.DataSourceResult);
        }

		/// <summary>
		/// 初始化控件状态
		/// </summary>
		/// <param name="e">包含事件数据的 EventArgs 对象</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this._Inited = true;
            if (this.Page != null)
                this.RequiresDataBinding = !this.Page.IsPostBack;
        }

		/// <summary>
		/// 在保存视图状态和呈现内容之前，确认数据源绑定
		/// </summary>
		/// <param name="e">包含事件数据的 EventArgs 对象</param>
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
		/// 获取设置是否需要执行DataBind进行数据绑定
		/// </summary>
		/// <remarks>获取设置是否需要执行DataBind进行数据绑定</remarks>
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
		/// 确认绑定
		/// </summary>
		/// <remarks>如果需要绑定，则进行绑定操作，否则返回</remarks>
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
