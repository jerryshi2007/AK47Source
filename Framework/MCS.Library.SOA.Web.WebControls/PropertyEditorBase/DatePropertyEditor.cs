using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace MCS.Web.WebControls
{
	public class DatePropertyEditor : PropertyEditorBase
	{
        //private DeluxeCalendar _DeluxeCalendarControl = new DeluxeCalendar() { ID = "DatePropertyEditor_DeluxeCalendar", DisplayFormat = "yyyy-MM-dd" };

        protected internal override void OnPagePreRender(Page page)
        {
            if (page.Form != null)
            {
                HtmlGenericControl div = new HtmlGenericControl();
                div.Style["display"] = "none";

                DeluxeCalendar deluxeCalendarControl = new DeluxeCalendar() { ID = "DatePropertyEditor_DeluxeCalendar", DisplayFormat = "yyyy-MM-dd" };
                div.Controls.Add(deluxeCalendarControl);

                page.Form.Controls.AddAt(0, div);
            }
        }
	}
}
