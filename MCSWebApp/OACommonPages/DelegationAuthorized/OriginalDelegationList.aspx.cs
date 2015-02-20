using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data;
using MCS.Library;
using MCS.Web.Library;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.OA.CommonPages.DelegationAuthorized
{
	public partial class OriginalDelegationList : System.Web.UI.Page
	{

		public int LastQueryRowCount
		{
			get { return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1); }
			set { WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value); }
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			ExecQuery();
		}

		public void ExecQuery()
		{
			LastQueryRowCount = -1;
			whereCondition.Value = string.Format("SOURCE_USER_ID = {0}", MCS.Library.Data.Builder.TSqlBuilder.Instance.CheckQuotationMark(DeluxeIdentity.Current.User.ID, true));


		}

		protected void DeluxeGridDelegationList_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				WfDelegation view = (WfDelegation)e.Row.DataItem;

				HtmlAnchor updateItme = (HtmlAnchor)e.Row.FindControl("LinkBtnUpdate");

				updateItme.Attributes.Add("onclick", "onUpdateClick('" + view.DestinationUserID + "')");

				LinkButton delItme = (LinkButton)e.Row.FindControl("LinkBtnDel");
				delItme.CommandArgument = view.DestinationUserID;
				delItme.OnClientClick = "return window.confirm('确认要删除吗？');";

				e.Row.Cells[1].Text = view.StartTime.ToString("yyyy-MM-dd");
				e.Row.Cells[2].Text = view.EndTime.ToString("yyyy-MM-dd");

			}
		}

		protected void DeluxeGridDelegationList_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DeleteDelegation")
			{
				WfDelegationCollection wfDelegation = WfDelegationAdapter.Instance.Load(builder =>
				{
					builder.AppendItem("DESTINATION_USER_ID", e.CommandArgument.ToString());
					builder.AppendItem("SOURCE_USER_ID", DeluxeIdentity.CurrentUser.ID);

				});


				WfDelegationAdapter.Instance.Delete(wfDelegation[0]);

				LogUtil.AppendLogToDb(LogUtil.CreateDissassignLog(wfDelegation[0]));
			}
		}

		protected void DeluxeGridDelegationList_ExportData(object sender, EventArgs e)
		{
			ExecQuery();
		}

		protected void ObjectDataSourceDelegationList_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			LastQueryRowCount = (int)e.OutputParameters["totalCount"];
		}

		protected void ObjectDataSourceDelegationList_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["totalCount"] = LastQueryRowCount;
		}

		protected void RefreshButton_Click(object sender, EventArgs e)
		{
			LastQueryRowCount = -1;
			this.DeluxeGridDelegation.DataBind();
		}
	}
}