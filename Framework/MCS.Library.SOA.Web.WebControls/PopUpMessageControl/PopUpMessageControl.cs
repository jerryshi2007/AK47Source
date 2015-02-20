#region
// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	PopUpMessageControl.cs
// Remark	��	��Ϣ����
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			����		20071127		����
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

        #region ����

        /// <summary>
        /// Ҫ��ʾ����Ϣ
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("showText"), Description("Ҫ��ʾ����Ϣ"), DefaultValue("")]
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
        /// Ĭ����ʾ��ʱ��
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("showTime"), Description("Ĭ����ʾ��ʱ��"), Browsable(true)]
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
        /// �Ƿ���Ч
        /// </summary>
        [DefaultValue(true), Category("Appearance"), ScriptControlProperty, ClientPropertyName("enabled"), Description("�Ƿ���Ч")]
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
        /// Ҫ���ŵ���ʾ��·��
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("playSoundPath"), Description("Ҫ���ŵ���ʾ��·��"), Browsable(true)]
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

        [ScriptControlProperty, ClientPropertyName("positionElementID"), Description("��ʾʱ�ĵ�����׼��ҳ��Ԫ��ID"), DefaultValue("")]
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

        [ScriptControlProperty, ClientPropertyName("positionX"), Description("��ʾλ�õ�X����"), DefaultValue("0")]
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

        [ScriptControlProperty, ClientPropertyName("positionY"), Description("��ʾλ�õ�Y����"), DefaultValue("0")]
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
        /// �����ͻ����¼�
        /// </summary>
        [DefaultValue(""), ScriptControlEvent, ClientPropertyName("onClick"), Bindable(true), Category("ClientEventsHandler"), Description("�����ͻ����¼�")]
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
