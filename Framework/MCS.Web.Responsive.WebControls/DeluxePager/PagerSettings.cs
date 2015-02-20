// -------------------------------------------------
// Assembly	��	MCS.Web.Responsive.WebControls
// FileName	��	PagerSettings.cs
// Remark	��  
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		�����(MYD)	    20070815		����
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
using MCS.Web.Responsive.Library;
namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// ��ҳҳ������
    /// </summary>
    /// <remarks>
    /// ��ҳҳ������
    /// </remarks>
    public enum DeluxePagerMode
    {
        /// <summary>
        /// ��������
        /// </summary>
        Numeric,
        /// <summary>
        /// ����ҳ������
        /// </summary>
        NextPreviousFirstLast
    }

    /// <summary>
    /// Pagerҳ��������
    /// </summary>
    /// <remarks>
    /// Pagerҳ��������
    /// </remarks>
    [TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]

    [Serializable]
    public sealed class PagerSettings : IStateManager
    {
        // Fields
        //private bool _isTracking;
        private StateBag viewState = new StateBag();
        //private bool marked;

        //private EventHandler PropertyChanged;

        // Events
        /// <summary>
        /// ���Ըı��¼����
        /// </summary>
        /// <remarks>
        /// ���Ըı��¼����
        /// </remarks>
        [Browsable(false)]
        public event EventHandler PropertyChanged;

        // Methods
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <remarks>
        /// ���캯��
        /// </remarks>
        public PagerSettings()
        {
            this.viewState = new StateBag();
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="sysPagerSettings"></param>
        /// <remarks>
        /// ���캯��
        /// </remarks>
        public PagerSettings(System.Web.UI.WebControls.PagerSettings sysPagerSettings)
            : this()
        {
            LoadData(sysPagerSettings);
        }

        /// <summary>
        /// ����change
        /// </summary>
        /// <remarks>
        /// ����change
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
        /// ��дToString
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// ��дToString
        /// </remarks>
        public override string ToString()
        {
            return string.Empty;
        }
        /// <summary>
        /// �Ƿ�IsTrackingViewState
        /// </summary>
        /// <remarks>
        /// �Ƿ�IsTrackingViewState
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
        /// ��һҳ��ť��ʾ��ͼƬ��ַ
        /// </summary>
        /// <remarks>
        /// ��һҳ��ť��ʾ��ͼƬ��ַ
        /// </remarks>
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Category("Appearance"), UrlProperty, DefaultValue(""), NotifyParentProperty(true), Description("PagerSettings_FirstPageImageUrl")]
        public string FirstPageImageUrl
        {
            get
            {
                return this.ViewState.GetViewStateValue<string>("FirstPageImageUrl", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue<string>("FirstPageImageUrl", value);
                this.OnPropertyChanged();

            }
        }

        /// <summary>
        /// ��һҳ��ť��ʾ������
        /// </summary>
        /// <remarks>
        /// ��һҳ��ť��ʾ������
        /// </remarks>
        [Description("PagerSettings_FirstPageText"), Category("Appearance"), DefaultValue("&lt;&lt;"), NotifyParentProperty(true)]
        public string FirstPageText
        {
            get
            {
                return this.ViewState.GetViewStateValue<string>("FirstPageText", "&lt;&lt;");
            }
            set
            {
                this.ViewState.SetViewStateValue<string>("FirstPageText", value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// ���һҳ��ť��ʾ��ͼƬ��ַ
        /// </summary>
        /// <remarks>
        /// ���һҳ��ť��ʾ��ͼƬ��ַ
        /// </remarks>
        [NotifyParentProperty(true), UrlProperty, DefaultValue(""), Description("PagerSettings_LastPageImageUrl"), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Category("Appearance")]
        public string LastPageImageUrl
        {
            get
            {
                return this.ViewState.GetViewStateValue<string>("LastPageImageUrl", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue<string>("LastPageImageUrl", value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// ���һҳ��ť��ʾ������
        /// </summary>
        /// <remarks>
        /// ���һҳ��ť��ʾ������
        /// </remarks>
        [NotifyParentProperty(true), Description("PagerSettings_LastPageText"), Category("Appearance"), DefaultValue("&gt;&gt;")]
        public string LastPageText
        {
            get
            {
                return this.ViewState.GetViewStateValue<string>("LastPageText", "&gt;&gt;");
            }
            set
            {
                this.ViewState.SetViewStateValue<string>("LastPageText", value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// ��ҳģʽ
        /// </summary>
        /// <remarks>
        /// ��ҳģʽ
        /// </remarks>
        [Description("PagerSettings_Mode"), Category("Appearance"), DefaultValue(1), NotifyParentProperty(true)]
        public DeluxePagerMode Mode
        {
            get
            {
                return this.ViewState.GetViewStateValue<DeluxePagerMode>("PagerMode", DeluxePagerMode.Numeric);
            }
            set
            {
                this.ViewState.SetViewStateValue<DeluxePagerMode>("PagerMode", value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// ��һҳ��ť��ʾ��ͼƬ��ַ
        /// </summary>
        /// <remarks>
        /// ��һҳ��ť��ʾ��ͼƬ��ַ
        /// </remarks>
        [UrlProperty, Category("Appearance"), DefaultValue(""), NotifyParentProperty(true), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Description("PagerSettings_NextPageImageUrl")]
        public string NextPageImageUrl
        {
            get
            {
                return this.ViewState.GetViewStateValue<string>("NextPageImageUrl", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue<string>("NextPageImageUrl", value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// ��һҳ��ť��ʾ������
        /// </summary>
        /// <remarks>
        /// ��һҳ��ť��ʾ������
        /// </remarks>
        [NotifyParentProperty(true), Category("Appearance"), DefaultValue("&gt;"), Description("PagerSettings_NextPageText")]
        public string NextPageText
        {
            get
            {
                return this.ViewState.GetViewStateValue<string>("NextPageText", "&gt;");
            }
            set
            {
                this.ViewState.SetViewStateValue<string>("NextPageText", value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// ��ҳ�ؼ���ҳ�밴ť��
        /// </summary>
        /// <remarks>
        /// ��ҳ�ؼ���ҳ�밴ť��
        /// </remarks>
        [DefaultValue(5), Category("Behavior"), Description("PagerSettings_PageButtonCount"), NotifyParentProperty(true)]
        public int PageButtonCount
        {
            get
            {
                return this.ViewState.GetViewStateValue<int>("PageButtonCount", 5);
            }
            set
            {
                this.ViewState.SetViewStateValue<int>("PageButtonCount", value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// ��һҳ��ť��ʾ��ͼƬ��ַ
        /// </summary>
        /// <remarks>
        /// ��һҳ��ť��ʾ��ͼƬ��ַ
        /// </remarks>
        [DefaultValue(""), Category("Appearance"), NotifyParentProperty(true), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, Description("PagerSettings_PreviousPageImageUrl")]
        public string PreviousPageImageUrl
        {
            get
            {
                return this.ViewState.GetViewStateValue<string>("PreviousPageImageUrl", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue<string>("PreviousPageImageUrl", value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// ��һҳ��ť��ʾ������
        /// </summary>
        /// <remarks>
        /// ��һҳ��ť��ʾ������
        /// </remarks>
        [DefaultValue("&lt;"), Category("Appearance"), NotifyParentProperty(true), Description("PagerSettings_PreviousPageText")]
        public string PreviousPageText
        {
            get
            {
                return this.ViewState.GetViewStateValue<string>("PreviousPageText", "&lt;");
            }
            set
            {
                this.ViewState.SetViewStateValue<string>("PreviousPageText", value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// ����pageSettings����
        /// </summary>
        /// <param name="sysPageSettings"></param>
        /// <remarks>
        /// ����pageSettings����
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
        /// ��ȡ��ǰPagerҳ�뷭ҳģʽ
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <remarks>
        /// ��ȡ��ǰPagerҳ�뷭ҳģʽ
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
