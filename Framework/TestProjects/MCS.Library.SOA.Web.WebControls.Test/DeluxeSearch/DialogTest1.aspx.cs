using System;
using MCS.Web.Library;

namespace MCS.Library.SOA.Web.WebControls.Test.DeluxeSearch
{
	public partial class DialogTest1 : System.Web.UI.Page
	{
		private void InitializeData()
		{
            ddlType.BindData(new TestResult().GetTypeItem(),"TypeValue", "TypeName");
            ddlScope.BindData(new TestResult().GetAreaItem(), "AreaValue", "AreaName");
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				InitializeData();
			}
		}

		protected void BtnOkClick(object sender, EventArgs e)
		{
			try
			{
				bindingControl.Data = new SupplierSearch();

				bindingControl.CollectData();

				Search1.RegisterReturnValue(bindingControl.Data);
			}
			catch (Exception ex)
			{
				WebUtility.RegisterClientErrorMessage(ex);
			}
		}
	}
}