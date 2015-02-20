using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;

namespace PermissionCenter.WebControls
{
	[DefaultProperty("OperationType")]
	[ToolboxData("<{0}:LogOperationLabel runat=server></{0}:LogOperationLabel>")]
	public class LogOperationLabel : Label
	{
		private SCOperationType operationType = SCOperationType.None;

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string OperationType
		{
			get
			{
				string s = (string)this.ViewState["OperationType"];
				return (s == null) ? string.Empty : s;
			}

			set
			{
				this.operationType = (SCOperationType)Enum.Parse(typeof(SCOperationType), value);
				this.ViewState["OperationType"] = value;
			}
		}

		[Browsable(false)]
		public override string Text
		{
			get
			{
				return base.Text;
			}

			set
			{
				base.Text = value;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.Text = EnumItemDescriptionAttribute.GetDescription(this.operationType);
			base.OnPreRender(e);
		}

		protected override void RenderContents(HtmlTextWriter output)
		{
			output.Write(this.Text);
		}
	}
}
