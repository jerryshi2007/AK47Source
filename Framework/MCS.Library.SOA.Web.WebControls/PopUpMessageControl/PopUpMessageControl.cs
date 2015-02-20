#region
// -------------------------------------------------
// Assembly	：	MCS.Web.WebControls
// FileName	：	PopUpMessageControl.cs
// Remark	：	消息提醒
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20071127		创建
// -------------------------------------------------
#endregion

using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Text;
using System.ComponentModel;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.WebControls;
using MCS.Web.Library.Script;
using MCS.Library.Principal;

#region assembly

[assembly: WebResource("MCS.Web.WebControls.PopUpMessageControl.PopUpMessageControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.Css.OAWebControls.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.WebControls.PopUpMessageControl.MessageIcon.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.PopUpMessageControl.MessageTitle.gif", "image/gif")]

#endregion

namespace MCS.Web.WebControls
{
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.PopUpMessageControl",
    "MCS.Web.WebControls.PopUpMessageControl.PopUpMessageControl.js")]
    public class PopUpMessageControl : ScriptControlBase
    {
        public PopUpMessageControl()
            : base(true, HtmlTextWriterTag.Div)
        { }

        #region 属性

        /// <summary>
        /// 要显示的消息
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("showText"), Description("要显示的消息"), DefaultValue("")]
        public string ShowText
        {
            get
            {
                return GetPropertyValue<string>("showText", string.Empty);
            }
            set
            {
                SetPropertyValue<string>("showText", value);
            }
        }

        /// <summary>
        /// 默认显示的时间
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("showTime"), Description("默认显示的时间"), Browsable(true)]
        public TimeSpan ShowTime
        {
            get
            {
                return GetPropertyValue<TimeSpan>("showTime", new TimeSpan(0, 0, 4));
            }
            set
            {
                SetPropertyValue<TimeSpan>("showTime", value);
            }
        }

        /// <summary>
        /// 是否有效
        /// </summary>
        [DefaultValue(true), Category("Appearance"), ScriptControlProperty, ClientPropertyName("enabled"), Description("是否有效")]
        public override bool Enabled
        {
            get
            {
                return GetPropertyValue<bool>("Enabled", true);
            }
            set
            {
                SetPropertyValue<bool>("Enabled", value);
            }
        }

        /// <summary>
        /// 要播放的提示音路径
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("playSoundPath"), Description("要播放的提示音路径"), Browsable(true)]
        public string PlaySoundPath
        {
            get
            {
                return GetPropertyValue<string>("PlaySoundPath", string.Empty);
            }
            set
            {
                SetPropertyValue<string>("PlaySoundPath", value);
            }
        }

        [ScriptControlProperty, ClientPropertyName("cssPath"), Browsable(false)]
        private string CssPath
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.PopUpMessageControl),
                     "MCS.Web.WebControls.Css.OAWebControls.css");
            }
        }

        [ScriptControlProperty, ClientPropertyName("titleImagePath"), Browsable(false)]
        private string TitleImagePath
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.PopUpMessageControl),
                     "MCS.Web.WebControls.PopUpMessageControl.MessageTitle.gif");
            }
        }

        [ScriptControlProperty, ClientPropertyName("messageIconPath"), Browsable(false)]
        private string MessageIconPath
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.PopUpMessageControl),
                     "MCS.Web.WebControls.PopUpMessageControl.MessageIcon.gif");
            }
        }

        [ScriptControlProperty, ClientPropertyName("positionElementID"), Description("显示时的当作标准的页面元素ID"), DefaultValue("")]
        public string PositionElementID
        {
            get
            {
                return GetPropertyValue<string>("PositionElementID", string.Empty);
            }
            set
            {
                SetPropertyValue<string>("PositionElementID", value);
            }
        }

        [ScriptControlProperty, ClientPropertyName("positionX"), Description("显示位置的X坐标"), DefaultValue("0")]
        public int PositionX
        {
            get
            {
                return GetPropertyValue<int>("PositionX", 0);
            }
            set
            {
                SetPropertyValue<int>("PositionX", value);
            }
        }

        [ScriptControlProperty, ClientPropertyName("positionY"), Description("显示位置的Y坐标"), DefaultValue("0")]
        public int PositionY
        {
            get
            {
                return GetPropertyValue<int>("positionY", 0);
            }
            set
            {
                SetPropertyValue<int>("positionY", value);
            }
        }


        /// <summary>
        /// 单击客户端事件
        /// </summary>
        [DefaultValue(""), ScriptControlEvent, ClientPropertyName("onClick"), Bindable(true), Category("ClientEventsHandler"), Description("单击客户端事件")]
        public string OnClick
        {
            get
            {
                return GetPropertyValue("OnClick", string.Empty);
            }
            set
            {
                SetPropertyValue("OnClick", value);
            }
        }

        #endregion

        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                Label label = new Label();
                label.Text = "PopUpMessageControl";
                Controls.Add(label);
                base.Render(writer);
            }
            else
            {
                base.Render(writer);
            }
        }

    }
}
