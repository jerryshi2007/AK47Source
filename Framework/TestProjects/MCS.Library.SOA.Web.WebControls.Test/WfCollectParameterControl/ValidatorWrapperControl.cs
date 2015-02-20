using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace MCS.Library.SOA.Web.WebControls.Test.WfCollectParameterControl
{
	public class ValidatorWrapperControl : Control, INamingContainer
	{
		protected override void OnInit(EventArgs e)
		{
			this.Controls.Add(new TestValidator() { ErrorMessage = "Error" });
			base.OnInit(e);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode == true)
				writer.Write("ValidatorWrapperControl");
			else
				base.Render(writer);
		}
	}
}