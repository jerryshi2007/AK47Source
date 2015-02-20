using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace MCS.Web.Responsive.WebControls
{
    [PropertyEditorDescription("DateTimePropertyEditor", "MCS.Web.WebControls.DateTimePropertyEditor")]
	public class DateTimePropertyEditor : PropertyEditorBase
	{
		//private DeluxeDateTime _DeluxeDateTimeControl = new DeluxeDateTime() { ID = "DateTimePropertyEditor_DeluxeDateTime" };

		protected internal override void OnPagePreRender(Page page)
		{
			if (page.Form != null)
			{
				HtmlGenericControl div = new HtmlGenericControl("div");

				div.Style["display"] = "none";

				DateTimePicker deluxeDateTimeControl = new DateTimePicker() { ID = "DateTimePropertyEditor_DeluxeDateTime" };

				div.Controls.Add(deluxeDateTimeControl);
				page.Form.Controls.AddAt(0, div);
			}
		}
	}
}
