// -------------------------------------------------
// Assembly	：	MCS.Web.WebControls
// FileName	：	PagerSettings.cs
// Remark	：  
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		马泽锋(MYD)	    20070815		创建
// -------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using MCS.Web.Library;
namespace MCS.Web.WebControls
{
	/// <summary>
	/// 翻页页码类型
	/// </summary>
	/// <remarks>
	/// 翻页页码类型
	/// </remarks>
	public enum DeluxePagerMode
	{
		/// <summary>
		/// 数字类型
		/// </summary>
		Numeric,
		/// <summary>
		/// 上下页码类型
		/// </summary>
		NextPreviousFirstLast
	}

    /// <summary>
    /// Pager页码设置类
    /// </summary>
    /// <remarks>
    /// Pager页码设置类
    /// </remarks>
    [TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    
	[Serializable]
    public sealed class PagerSettings  : IStateManager
    {
        // Fields
		//private bool _isTracking;
        private StateBag viewState = new StateBag();
        //private bool marked;

        //private EventHandler PropertyChanged;

        // Events
        /// <summary>
        /// 属性改变事件句柄
        /// </summary>
        /// <remarks>
        /// 属性改变事件句柄
        /// </remarks>
        [Browsable(false)]
        public event EventHandler PropertyChanged;

        // Methods
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>
        /// 构造函数
        /// </remarks>
        public PagerSettings()
        {
            this.viewState = new StateBag();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sysPagerSettings"></param>
        /// <remarks>
        /// 构造函数
        /// </remarks>
        public PagerSettings(System.Web.UI.WebControls.PagerSettings sysPagerSettings)
            : this()
        {
            LoadData(sysPagerSettings);
        }

        /// <summary>
        /// 属性change
        /// </summary>
        /// <remarks>
        /// 属性change
        /// </remarks>
        private void OnPropertyChanged()
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// LoadViewState
        /// </summary>
        /// <param name="state"></param>
        /// <remarks>
        /// LoadViewState
        /// </remarks>
        void IStateManager.LoadViewState(object state)
        {
            if (state != null)
            {
                ((IStateManager)this.ViewState).LoadViewState(state);
            }
        }

        /// <summary>
        /// SaveViewState
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// SaveViewState
        /// </remarks>
        object IStateManager.SaveViewState()
        {
            return ((IStateManager)this.ViewState).SaveViewState();
        }

        /// <summary>
        /// TrackViewState
        /// </summary>
        /// <remarks>
        /// TrackViewState
        /// </remarks>
        void IStateManager.TrackViewState()
        {
            ((IStateManager)this.ViewState).TrackViewState();
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 重写ToString
        /// </remarks>
        public override string ToString()
        {
            return string.Empty;
        }
        /// <summary>
        /// 是否IsTrackingViewState
        /// </summary>
        /// <remarks>
        /// 是否IsTrackingViewState
        /// </remarks>
        bool IStateManager.IsTrackingViewState
        {
            get
            {
                return ((IStateManager)this.ViewState).IsTrackingViewState;
            }
        } 


        // Properties
        /// <summary>
        /// 第一页按钮显示的图片地址
        /// </summary>
        /// <remarks>
        /// 第一页按钮显示的图片地址
        /// </remarks>
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Category("Appearance"), UrlProperty, DefaultValue(""), NotifyParentProperty(true), Description("PagerSettings_FirstPageImageUrl")]
        public string FirstPageImageUrl
        {
            get
            {
                return WebControlUtility.GetViewStateValue<string>(this.ViewState, "FirstPageImageUrl", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue<string>(this.ViewState, "FirstPageImageUrl", value);
                this.OnPropertyChanged();

            }  
        }

        /// <summary>
        /// 第一页按钮显示的内容
        /// </summary>
        /// <remarks>
        /// 第一页按钮显示的内容
        /// </remarks>
        [Description("PagerSettings_FirstPageText"), Category("Appearance"), DefaultValue("&lt;&lt;"), NotifyParentProperty(true)]
        public string FirstPageText
        {
            get
            {
                return WebControlUtility.GetViewStateValue<string>(this.ViewState, "FirstPageText", "&lt;&lt;");
            }
            set
            {
                WebControlUtility.SetViewStateValue<string>(this.ViewState, "FirstPageText", value);
                this.OnPropertyChanged();
            }   
        }         

        /// <summary>
        /// 最后一页按钮显示的图片地址
        /// </summary>
        /// <remarks>
        /// 最后一页按钮显示的图片地址
        /// </remarks>
        [NotifyParentProperty(true), UrlProperty, DefaultValue(""), Description("PagerSettings_LastPageImageUrl"), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Category("Appearance")]
        public string LastPageImageUrl
        { 
            get
            {
                return WebControlUtility.GetViewStateValue<string>(this.ViewState, "LastPageImageUrl", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue<string>(this.ViewState, "LastPageImageUrl", value);
                this.OnPropertyChanged();
            }   
        }

        /// <summary>
        /// 最后一页按钮显示的内容
        /// </summary>
        /// <remarks>
        /// 最后一页按钮显示的内容
        /// </remarks>
        [NotifyParentProperty(true), Description("PagerSettings_LastPageText"), Category("Appearance"), DefaultValue("&gt;&gt;")]
        public string LastPageText
        {
            get
            {
                return WebControlUtility.GetViewStateValue<string>(this.ViewState, "LastPageText", "&gt;&gt;");
            }
            set
            {
                WebControlUtility.SetViewStateValue<string>(this.ViewState, "LastPageText", value);
                this.OnPropertyChanged();
            }    
        }

        /// <summary>
        /// 翻页模式
        /// </summary>
        /// <remarks>
        /// 翻页模式
        /// </remarks>
        [Description("PagerSettings_Mode"), Category("Appearance"), DefaultValue(1), NotifyParentProperty(true)]
        public DeluxePagerMode Mode
        {
            get
            {
                return WebControlUtility.GetViewStateValue<DeluxePagerMode>(this.ViewState, "PagerMode", DeluxePagerMode.Numeric);
            }
            set
            {
                WebControlUtility.SetViewStateValue<DeluxePagerMode>(this.ViewState, "PagerMode", value);
                this.OnPropertyChanged();
            }    
        }

        /// <summary>
        /// 下一页按钮显示的图片地址
        /// </summary>
        /// <remarks>
        /// 下一页按钮显示的图片地址
        /// </remarks>
        [UrlProperty, Category("Appearance"), DefaultValue(""), NotifyParentProperty(true), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Description("PagerSettings_NextPageImageUrl")]
        public string NextPageImageUrl
        {
            get
            {
                return WebControlUtility.GetViewStateValue<string>(this.ViewState, "NextPageImageUrl", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue<string>(this.ViewState, "NextPageImageUrl", value);
                this.OnPropertyChanged();
            }     
        }

        /// <summary>
        /// 下一页按钮显示的内容
        /// </summary>
        /// <remarks>
        /// 下一页按钮显示的内容
        /// </remarks>
        [NotifyParentProperty(true), Category("Appearance"), DefaultValue("&gt;"), Description("PagerSettings_NextPageText")]
        public string NextPageText
        {
            get
            {
                return WebControlUtility.GetViewStateValue<string>(this.ViewState, "NextPageText", "&gt;");
            }
            set
            {
                WebControlUtility.SetViewStateValue<string>(this.ViewState, "NextPageText", value);
                this.OnPropertyChanged();
            }   
        } 

        /// <summary>
        /// 翻页控件的页码按钮数
        /// </summary>
        /// <remarks>
        /// 翻页控件的页码按钮数
        /// </remarks>
        [DefaultValue(10), Category("Behavior"), Description("PagerSettings_PageButtonCount"), NotifyParentProperty(true)]
        public int PageButtonCount
        {
            get
            {
                return WebControlUtility.GetViewStateValue<int>(this.ViewState, "PageButtonCount", 10);
            }
            set
            {
                WebControlUtility.SetViewStateValue<int>(this.ViewState, "PageButtonCount", value);
                this.OnPropertyChanged();
            }                
        }  

        /// <summary>
        /// 上一页按钮显示的图片地址
        /// </summary>
        /// <remarks>
        /// 上一页按钮显示的图片地址
        /// </remarks>
        [DefaultValue(""), Category("Appearance"), NotifyParentProperty(true), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, Description("PagerSettings_PreviousPageImageUrl")]
        public string PreviousPageImageUrl
        {
            get
            {
                return WebControlUtility.GetViewStateValue<string>(this.ViewState, "PreviousPageImageUrl", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue<string>(this.ViewState, "PreviousPageImageUrl", value);
                this.OnPropertyChanged();
            }    
        } 

        /// <summary>
        /// 上一页按钮显示的内容
        /// </summary>
        /// <remarks>
        /// 上一页按钮显示的内容
        /// </remarks>
        [DefaultValue("&lt;"), Category("Appearance"), NotifyParentProperty(true), Description("PagerSettings_PreviousPageText")]
        public string PreviousPageText
        {
            get
            {
                return WebControlUtility.GetViewStateValue<string>(this.ViewState, "PreviousPageText", "&lt;");
            }
            set
            {
                WebControlUtility.SetViewStateValue<string>(this.ViewState, "PreviousPageText", value);
                this.OnPropertyChanged();
            }   
        }

        /// <summary>
        /// 设置pageSettings对象
        /// </summary>
        /// <param name="sysPageSettings"></param>
        /// <remarks>
        /// 设置pageSettings对象
        /// </remarks>
        public void LoadData(System.Web.UI.WebControls.PagerSettings sysPageSettings)
        {
            this.FirstPageImageUrl = sysPageSettings.FirstPageImageUrl;
            this.FirstPageText = sysPageSettings.FirstPageText;
            this.LastPageImageUrl = sysPageSettings.LastPageImageUrl;
            this.LastPageText = sysPageSettings.LastPageText;
            this.Mode = this.GetDeluxePagerMode(sysPageSettings.Mode);
            this.NextPageImageUrl = sysPageSettings.NextPageImageUrl;
            this.NextPageText = sysPageSettings.NextPageText;
            this.PageButtonCount = sysPageSettings.PageButtonCount;
            this.PreviousPageImageUrl = sysPageSettings.PreviousPageImageUrl;
            this.PreviousPageText = sysPageSettings.PreviousPageText;            
        }

        /// <summary>
        /// 获取当前Pager页码翻页模式
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <remarks>
        /// 获取当前Pager页码翻页模式
        /// </remarks>
        private DeluxePagerMode GetDeluxePagerMode(PagerButtons mode)
        {
            DeluxePagerMode result = DeluxePagerMode.NextPreviousFirstLast;
            switch (mode)
            {
                case PagerButtons.NextPrevious:
                case PagerButtons.NextPreviousFirstLast:
                    result = DeluxePagerMode.NextPreviousFirstLast;
                    break;
                
                case PagerButtons.Numeric:
                case PagerButtons.NumericFirstLast:
                    result = DeluxePagerMode.Numeric;
                    break;
            }

            return result;
        }

        /// <summary>
        /// ViewState
        /// </summary>
        /// <remarks>
        /// ViewState
        /// </remarks>
        private StateBag ViewState
        {
            get
            {
                return this.viewState;
            }
        }       
    } 
}
