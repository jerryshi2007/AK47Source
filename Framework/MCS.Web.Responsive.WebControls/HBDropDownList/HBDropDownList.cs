#region
// -------------------------------------------------
// Assembly	：	MCS.Web.Responsive.WebControls
// FileName	：	HBDropDownList.cs
// Remark	：	选择控件
// -------------------------------------------------
// VERSION		AUTHOR		     DATE			CONTENT
// 1.0			英雄不留名		20080325    	果果15天 创建 
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Web.Responsive.Library;
using MCS.Web.Library.Script;

namespace MCS.Web.Responsive.WebControls
{
    [DefaultProperty("SelectedValue")]
    public class HBDropDownList : DropDownList
    {
        private string lastPostKey = string.Empty;
        private HtmlInputHidden selectedTextHidden = new HtmlInputHidden();
        private HtmlInputHidden selectedValueHidden = new HtmlInputHidden();

        [Browsable(true),
        Description("只读属性"),
        DefaultValue(false),
        Category("扩展")]
        public bool ReadOnly
        {
            get
            {
                return ViewState.GetViewStateValue<bool>("ReadOnly", false);
            }
            set
            {
                ViewState.SetViewStateValue<bool>("ReadOnly", value);
            }
        }

        [DefaultValue(false)]
        public bool KeepControlWhenReadOnly
        {
            get
            {
                return ViewState.GetViewStateValue("KeepControlWhenReadOnly", false);
            }
            set
            {
                ViewState.SetViewStateValue("KeepControlWhenReadOnly", value);
            }
        }

        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            lastPostKey = postDataKey;
            return base.LoadPostData(postDataKey, postCollection);
        }

        protected override void OnPreRender(EventArgs e)
        {
            string script = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
                    "MCS.Web.Responsive.WebControls.HBDropDownList.HBDropDownList.js");

            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "HBDropDownListClient", script, true);

            //this.Page.ClientScript.RegisterStartupScript(
            //    this.GetType(),
            //    this.ClientID,
            //    string.Format("attachHBDropDownListEvents(document.getElementById('{0}'));", this.ClientID),
            //    true);

            base.OnPreRender(e);
        }

        public override string SelectedValue
        {
            get
            {
                string result = base.SelectedValue;

                if (this.Page.IsPostBack || this.Page.IsCallback)
                {
                    string postedValue = this.Page.Request.Form[lastPostKey];

                    if (postedValue != null)
                        result = postedValue;
                }

                this.selectedValueHidden.Value = result;

                return result;
            }
            set
            {
                this.selectedValueHidden.Value = value;
                base.SelectedValue = value;
            }
        }

        public string SelectedText
        {
            get
            {
                if (this.Page.IsPostBack || this.Page.IsCallback)
                {
                    string postedText = this.Page.Request.Form[lastPostKey + "_SelectedText"];

                    if (postedText != null)
                        this.selectedTextHidden.Value = postedText;
                    else
                        this.selectedTextHidden.Value = (string)this.ViewState["SelectedText"];
                }
                else
                {
                    if (SelectedItem != null)
                        this.selectedTextHidden.Value = SelectedItem.Text;
                }

                return this.selectedTextHidden.Value;
            }
            set
            {
                this.selectedTextHidden.Value = value;
            }
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            base.RenderControl(writer);

            RenderHiddenFields(writer);
        }

        protected override object SaveViewState()
        {
            InitHiddenFieldsAttributes();

            return base.SaveViewState();
        }

        /// <summary>
        /// 输入控件只读属性为true时候，把DropDownList变为Label控件
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
                    writer.AddAttribute("data-relativeLabel", lableID);
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                }

                writer.AddAttribute("data-relativeHidden", this.selectedTextHidden.ID);
                writer.AddAttribute("data-relativeValueHidden", this.selectedValueHidden.ID);

                base.Render(writer);
            }
        }

        private void InitHiddenFieldsAttributes()
        {
            this.selectedTextHidden.ID = this.ClientID + "_SelectedText";
            this.selectedValueHidden.ID = this.ClientID + "_SelectedValue";

            if (this.SelectedItem != null)
            {
                this.selectedTextHidden.Value = this.SelectedItem.Text;
                this.selectedValueHidden.Value = this.SelectedItem.Value;
            }
            else
            {
                this.selectedTextHidden.Value = "";
                this.selectedValueHidden.Value = "";
            }

            this.ViewState["SelectedText"] = this.selectedTextHidden.Value;
        }

        private void RenderHiddenFields(HtmlTextWriter writer)
        {
            writer.Write(this.selectedTextHidden.GetControlHtml());
            writer.Write(this.selectedValueHidden.GetControlHtml());
        }

        private void RenderLabel(HtmlTextWriter writer, string id)
        {
            Label lb = new Label();
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
            lb.BorderWidth = this.BorderWidth; ;
            lb.ControlStyle.CopyFrom(this.ControlStyle);
            lb.Height = this.Height;
            lb.ID = id;
            lb.TabIndex = this.TabIndex;
            lb.TemplateControl = this.TemplateControl;

            if (this.SelectedItem != null)
                lb.Text = HttpUtility.HtmlEncode(this.SelectedItem.Text);
            else
                lb.Text = "";

            lb.ToolTip = this.ToolTip;
            lb.Visible = this.Visible;
            lb.Width = this.Width;
            lb.CssClass = this.CssClass;

            lb.RenderControl(writer);
        }
    }
}
