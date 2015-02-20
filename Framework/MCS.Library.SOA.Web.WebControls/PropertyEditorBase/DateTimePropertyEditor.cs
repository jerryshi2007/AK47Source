using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace MCS.Web.WebControls
{
    public class DateTimePropertyEditor : PropertyEditorBase
    {
        //private DeluxeDateTime _DeluxeDateTimeControl = new DeluxeDateTime() { ID = "DateTimePropertyEditor_DeluxeDateTime" };

        protected internal override void OnPagePreRender(Page page)
        {
            if (page.Form != null)
            {
                HtmlGenericControl div = new HtmlGenericControl();

                div.Style["display"] = "none";

                DeluxeDateTime deluxeDateTimeControl = new DeluxeDateTime() { ID = "DateTimePropertyEditor_DeluxeDateTime" };

                div.Controls.Add(deluxeDateTimeControl);
                page.Form.Controls.AddAt(0, div);
            }
        }
    }
}
