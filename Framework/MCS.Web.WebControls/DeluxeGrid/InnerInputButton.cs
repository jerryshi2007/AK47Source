using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 输入Button的值
	/// </summary>
	public enum InputButtonType
	{
		/// <summary>
		/// 
		/// </summary>
		CheckBox,
		
		/// <summary>
		/// 
		/// </summary>
		Radio
	}

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
				return WebControlUtility.GetViewStateValue(this.ViewState, "Value", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(this.ViewState, "Value", value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get
			{
				return WebControlUtility.GetViewStateValue(this.ViewState, "Name", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(this.ViewState, "Name", value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Checked
		{
			get
			{
				return WebControlUtility.GetViewStateValue(this.ViewState, "Checked", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue(this.ViewState, "Checked", value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public InputButtonType ButtonType
		{
			get
			{
				return WebControlUtility.GetViewStateValue(this.ViewState, "ButtonType", InputButtonType.CheckBox);
			}
			set
			{
				WebControlUtility.SetViewStateValue(this.ViewState, "ButtonType", value);
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

			//writer.WriteEndTag("input");
		}
	}
}
