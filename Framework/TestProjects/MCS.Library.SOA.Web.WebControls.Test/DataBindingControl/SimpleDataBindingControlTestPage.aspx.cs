using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;

using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Web.WebControls.Test.DataBindingControl
{
	public partial class SimpleDataBindingControlTestPage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
 
            bindingControl.Data = PrepareData();
            
 
		}

		private SimpleDataObject PrepareData()
		{
			SimpleDataObject data = new SimpleDataObject();
		    data.SimpleDataType = DataType.Int;
            data.DateInput = DateTime.MinValue;
            //data.User = new OguUser
            //{
            //    DisplayName = "newuser",
            //    Email = "",
            //    FullPath = "newuser",
            //    GlobalSortID = "",
            //    ID =null,
            //    IsSideline = false,
            //    Levels = -1,
            //    LogOnName = "newuser",
            //    Name = "newuser",
            //    ObjectType = MCS.Library.OGUPermission.SchemaType.Users,
            //    Rank = 0,
            //    SortID = ""
            //};
            data.IntegerInput = "22";
			return data;
            
		}

		protected void postButton_Click(object sender, EventArgs e)
		{
            try
            {
                bindingControl.CollectData(true);

                //postedDateTime.Text = ((SimpleDataObject)(bindingControl.Data)).TimeInput.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                WebUtility.ShowClientError(ex.Message, ex.Message, "error");
            }
		}

        protected void CheckBoxList1_DataBound(object sender, EventArgs e)
        {
            TextBox textBox=new TextBox();
            CheckBoxList1.Controls.AddAt(1, textBox);
        }
	}
}
