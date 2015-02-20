using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.AdvancedSearch
{
    public class Login : CompositeControl
    {
        private TextBox txtUserName;

        private TextBox txtPassword;

        public string UserName
        {

            get
            {

                EnsureChildControls();

                return txtUserName.Text;

            }

            set
            {

                EnsureChildControls();

                txtUserName.Text = value;

            }

        }

        public string Password
        {

            get
            {

                EnsureChildControls();

                return txtPassword.Text;

            }

            set
            {

                EnsureChildControls();

                txtPassword.Text = value;

            }

        }

        protected override void CreateChildControls()
        {

            txtUserName = new TextBox();

            txtUserName.ID = "txtUserName";

            this.Controls.Add(txtUserName);

            txtPassword = new TextBox();

            txtPassword.ID = "txtPassword";

            this.Controls.Add(txtPassword);

        }

        protected override void RenderContents(HtmlTextWriter writer)
        {

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            writer.AddAttribute(HtmlTextWriterAttribute.For, txtUserName.ClientID);

            writer.RenderBeginTag(HtmlTextWriterTag.Label);

            writer.Write("User Name:");

            writer.RenderEndTag();

            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            txtUserName.RenderControl(writer);

            writer.RenderEndTag();

            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            writer.AddAttribute(HtmlTextWriterAttribute.For, txtPassword.ClientID);

            writer.RenderBeginTag(HtmlTextWriterTag.Label);

            writer.Write("Password:");

            writer.RenderEndTag();

            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            txtPassword.RenderControl(writer);

            writer.RenderEndTag();

            writer.RenderEndTag();

        }

        protected override HtmlTextWriterTag TagKey
        {

            get { return HtmlTextWriterTag.Table; }

        }

    }
}