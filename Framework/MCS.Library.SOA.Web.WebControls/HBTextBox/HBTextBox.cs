#region
// -------------------------------------------------
// Assembly	：	MCS.Web.WebControls
// FileName	：	HBTextBox.cs
// Remark	：	输入控件
// -------------------------------------------------
// VERSION		AUTHOR		     DATE			CONTENT
// 1.0			英雄不留名		20080324    	果果14天 创建 
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.IO;
using System.ComponentModel;
using MCS.Web.Library;
using System.Collections.Specialized;

namespace MCS.Web.WebControls
{
    [DefaultProperty("Text")]
    public class HBTextBox : TextBox
    {
        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            string text = postCollection[postDataKey];

            if (text != null)
                this.Text = text;

            return text != null;
        }

        [DefaultValue(false)]
        public bool KeepControlWhenReadOnly
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "KeepControlWhenReadOnly", false);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "KeepControlWhenReadOnly", value);
            }
        }

        /// <summary>
        /// 输入控件只读属性为true时候，把TextBox变为Label控件
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.ReadOnly && KeepControlWhenReadOnly == false)
            {
                RenderLabel(writer, this.ClientID);
            }
            else
            {
                string lableID = string.Empty;

                if (KeepControlWhenReadOnly && ReadOnly)
                {
                    lableID = this.ClientID + "_Label";

                    RenderLabel(writer, lableID);
                }

                if (string.IsNullOrEmpty(lableID) == false)
                {
                    writer.AddAttribute("relativeLabel", lableID);
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                }

                if (ReadOnly && KeepControlWhenReadOnly)
                {
                    string script = "if (event.propertyName=='value' || event.propertyName=='innerText') {var textValue; if (event.srcElement.tagName=='INPUT') {textValue=event.srcElement.value;} else {textValue=event.srcElement.innerText;}\ndocument.getElementById('{0}').innerText=textValue;}";

                    script = script.Replace("{0}", lableID);
                    writer.AddAttribute("onpropertychange", script);
                }

                if (TextMode == TextBoxMode.MultiLine && this.ReadOnly == false)
                {
                    writer.AddAttribute("onpropertychange",
                        "this.style.posHeight = (this.scrollHeight > parseInt(this.style.minHeight.replace('px', '')))" +
                        " ? this.scrollHeight : parseInt(this.style.minHeight.replace('px', ''));");
                    if (string.IsNullOrEmpty(Height.ToString()))
                    {
                        Height = Unit.Pixel(20);
                    }
                    writer.AddStyleAttribute("min-height", Height.ToString());

                    base.Render(writer);

                    writer.WriteLine("<script type='text/javascript'>");

                    string adjustScript = string.Format("{0}.style.posHeight = ({0}.scrollHeight > parseInt({0}.style.minHeight.replace('px', ''))) ? {0}.scrollHeight : parseInt({0}.style.minHeight.replace('px', ''));",
                        "document.getElementById('" + this.ClientID + "')");

                    string loadScript = "function(){" + adjustScript + "}";
                    writer.WriteLine("if (window.attachEvent){window.attachEvent('onload', " + loadScript + ")}else{window.addEventListener('onload', " + loadScript + ", false)}");
                    writer.WriteLine("</script>");
                }
                else
                    base.Render(writer);
            }
        }

        private void RenderLabel(HtmlTextWriter writer, string id)
        {
            Label lb = new Label();

            lb.ID = id;
            lb.AccessKey = this.AccessKey;
            lb.AppRelativeTemplateSourceDirectory = this.AppRelativeTemplateSourceDirectory;

            foreach (string s in this.Attributes.Keys)
                lb.Attributes.Add(s, this.Attributes[s]);

            foreach (string s in this.Style.Keys)
                lb.Style.Add(s, this.Style[s]);

            lb.ForeColor = this.ForeColor;
            lb.Font.CopyFrom(this.Font);
            lb.BackColor = this.BackColor;
            lb.BorderColor = this.BorderColor;
            lb.BorderStyle = this.BorderStyle;
            lb.BorderWidth = this.BorderWidth;
            lb.ControlStyle.CopyFrom(this.ControlStyle);
            lb.Style["min-height"] = this.Height.ToString();
            lb.Style["height"] = string.Empty;

            lb.TabIndex = this.TabIndex;
            lb.TemplateControl = this.TemplateControl;

            string txt = HttpUtility.HtmlEncode(this.Text);

            txt = txt.Replace("\r\n", "<br>");
            lb.Text = txt;
            lb.ToolTip = this.ToolTip;
            lb.Visible = this.Visible;
            lb.Width = this.Width;
            lb.CssClass = this.CssClass;
            lb.Style.Add("word-wrap", "break-word");

            lb.RenderControl(writer);
        }
    }
}
