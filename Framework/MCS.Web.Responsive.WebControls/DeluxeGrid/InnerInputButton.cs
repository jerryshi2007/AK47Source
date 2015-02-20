using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Web.Responsive.Library;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// 
    /// </summary>
    public class InputButton : Control
    {
        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get
            {
                return this.ViewState.GetViewStateValue("Value", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("Value", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return this.ViewState.GetViewStateValue("Name", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("Name", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Checked
        {
            get
            {
                return this.ViewState.GetViewStateValue("Checked", false);
            }
            set
            {
                this.ViewState.SetViewStateValue("Checked", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public InputButtonType ButtonType
        {
            get
            {
                return this.ViewState.GetViewStateValue("ButtonType", InputButtonType.CheckBox);
            }
            set
            {
                this.ViewState.SetViewStateValue("ButtonType", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteBeginTag("input");
            writer.WriteAttribute("id", this.ClientID);
            writer.WriteAttribute("name", this.Name);
            writer.WriteAttribute("type", this.ButtonType.ToString().ToLower());
            writer.WriteAttribute("value", this.Value);

            if (this.Checked)
                writer.Write(" checked ");

            writer.Write(" />");
        }
    }
}
