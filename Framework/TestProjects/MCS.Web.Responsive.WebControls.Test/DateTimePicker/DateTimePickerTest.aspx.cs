using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;

namespace MCS.Web.Responsive.WebControls.Test.DateTimePicker
{
    public partial class DateTimePickerTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                var culture = Thread.CurrentThread.CurrentUICulture;
                bool hasThis = false;
                foreach (ListItem item in this.cultureList.Items)
                {
                    if (item.Value == culture.Name)
                    {
                        hasThis = true;
                        break;
                    }
                }

                if (hasThis == false)
                {
                    this.cultureList.Items.Add(new ListItem(culture.DisplayName, culture.Name));
                }

                this.cultureList.SelectedValue = culture.Name;
            }
        }

        protected void DoSubmit(object sender, EventArgs e)
        {
            ltDate.Text = freePicker.DateValue.ToShortDateString();
            ltTime.Text = freePicker.TimeValue.ToString();
            ltDateTime.Text = freePicker.Value.ToString();
        }

        protected void serverSetDate(object sender, EventArgs e)
        {
            freePicker.Value = DateTime.Parse(inputDate.Value);

        }

        protected void cultureList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string val = this.cultureList.SelectedValue;
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(val);
        }
    }
}