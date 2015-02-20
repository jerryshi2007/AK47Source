using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI;

namespace MCS.Web.Responsive.WebControls
{
    [PropertyEditorDescription("DatePropertyEditor", "MCS.Web.WebControls.DatePropertyEditor")]
	public class DatePropertyEditor : PropertyEditorBase
	{
		//private DeluxeCalendar _DeluxeCalendarControl = new DeluxeCalendar() { ID = "DatePropertyEditor_DeluxeCalendar", DisplayFormat = "yyyy-MM-dd" };

		protected internal override void OnPagePreRender(Page page)
		{
			if (page.Form != null)
			{
				HtmlGenericControl div = new HtmlGenericControl("div");
				div.Style["display"] = "none";

				DateTimePicker deluxeCalendarControl = new DateTimePicker() { ID = "DatePropertyEditor_DeluxeCalendar", DisplayFormat = "yyyy-MM-dd" };
				div.Controls.Add(deluxeCalendarControl);

				page.Form.Controls.AddAt(0, div);
			}
		}
	}
}
