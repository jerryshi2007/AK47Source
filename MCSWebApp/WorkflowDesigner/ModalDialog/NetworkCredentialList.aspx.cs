using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;

namespace WorkflowDesigner.ModalDialog
{
	public partial class NetworkCredentialList : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (!IsPostBack)
			{
				GridBind();
			}
		}

		protected void CredentialDeluxeGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				var key = e.Row.Cells[1].Text;
				string htmlAnchor = "<a href='#' onclick=\"modifyCredential('{0}');\">{1}</a>";

				e.Row.Cells[1].Text = string.Format(htmlAnchor,
					HttpUtility.JavaScriptStringEncode(key, false),
					HttpUtility.HtmlEncode(key));
			}
		}

		protected void CredentialDeluxeGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			CredentialDeluxeGrid.PageIndex = e.NewPageIndex;
			GridBind();
		}

		private void GridBind()
		{
			WfGlobalParameters parameters = WfGlobalParameters.LoadProperties(WfGlobalParameters.Default.Key);
			WfNetworkCredentialCollection coll = parameters.Credentials;

			CredentialDeluxeGrid.DataSource = coll;
			CredentialDeluxeGrid.DataBind();
		}

		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			var keys = CredentialDeluxeGrid.SelectedKeys;

			WfGlobalParameters parameters = WfGlobalParameters.Default;
			foreach (var item in keys)
			{
				parameters.Credentials.Remove(p => p.Key == item);
			}

			parameters.Update();
			GridBind();
		}

		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			GridBind();
		}
	}
}