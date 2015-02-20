using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using System.Collections;

namespace WorkflowDesigner.ModalDialog
{
	public partial class ServicesAddressList : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			//WfGlobalParameters.Default.ServiceAddressDefs.Clear();
			//WfGlobalParameters.Default.Update();
			//if (this.IsPostBack == false)
			//{
			//    GridBind();
			//}

			this.text_Key.Text = Request.QueryString["search_Key"];
			try
			{
				this.dropSendType.SelectedValue = Request.QueryString["search_Action"];
			}
			catch { }
		}

		protected void ServicesAddressDeluxeGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				var key = e.Row.Cells[1].Text;
				string htmlAnchor = "<a href='#' onclick=\"modifyServicesAddress('{0}');\">{1}</a>";

				e.Row.Cells[1].Text = string.Format(htmlAnchor,
					HttpUtility.JavaScriptStringEncode(key, false),
					HttpUtility.HtmlEncode(key));
			}
		}

		protected void ServicesAddressDeluxeGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			//ServicesAddressDeluxeGrid.PageIndex = e.NewPageIndex;
			//GridBind();
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			var keys = ServicesAddressDeluxeGrid.SelectedKeys;

			WfGlobalParameters parameters = WfGlobalParameters.Default;
			foreach (var item in keys)
			{
				parameters.ServiceAddressDefs.Remove(p => p.Key == item);
			}

			parameters.Update();
			this.ServicesAddressDeluxeGrid.SelectedKeys.Clear();
			this.ServicesAddressDeluxeGrid.DataBind();
		}

		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			this.ServicesAddressDeluxeGrid.SelectedKeys.Clear();
			this.ServicesAddressDeluxeGrid.DataBind();
		}

		protected void ConfirmClick(object sender, EventArgs e)
		{
			var key = ServicesAddressDeluxeGrid.SelectedKeys.FirstOrDefault();

			if (key != null)
			{
				WfGlobalParameters parameters = WfGlobalParameters.LoadProperties(WfGlobalParameters.Default.Key);
				var item = parameters.ServiceAddressDefs[key];

				hiddenValue.Value = MCS.Web.Library.Script.JSONSerializerExecute.Serialize(item);

				this.ClientScript.RegisterStartupScript(this.GetType(), "ConfirmDialog", @"window.returnValue = (Sys.Serialization.JavaScriptSerializer.deserialize($('#hiddenValue').val())); window.close(); ", true);
			}
		}
	}

	public class ServicesAddressListDataSource
	{
		WfGlobalParameters parameters = WfGlobalParameters.LoadProperties(WfGlobalParameters.Default.Key);
		int count = -1;

		public IEnumerable Query(string key, string actionMethod, int startRowIndex, int maximumRows, string orderBy)
		{
			WfServiceRequestMethod method;

			bool ignoreKey = string.IsNullOrWhiteSpace(key);
			bool ignoreMethod = !Enum.TryParse<WfServiceRequestMethod>(actionMethod, out method);

			count = (from obj in parameters.ServiceAddressDefs
					 where (ignoreKey || obj.Key.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0) && (ignoreMethod || obj.RequestMethod == method)
					 select obj).Count();

			return from obj in parameters.ServiceAddressDefs.Where(obj => (ignoreKey || obj.Key.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0) && (ignoreMethod || obj.RequestMethod == method)).Skip(startRowIndex).Take(maximumRows)
				   select new
					   {
						   Key = obj.Key,
						   Address = obj.Address,
						   RequestMethod = obj.RequestMethod.ToString(),
						   Credential = obj.Credential == null ? "" : obj.Credential.LogOnName
					   };
		}

		public int GetQueryCount(string key, string actionMethod)
		{
			return count < 0 ? 0 : count;
		}
	}
}