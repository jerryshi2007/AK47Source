using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.OA.CommonPages.UserInfoExtend
{
	public partial class EditUserReportToInfo : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				string userId = Request.QueryString.GetValue("userId", string.Empty);
				var userReport = GetReportToUser(userId);
				if (userReport != null)
					OuUserInputControl.SelectedSingleData = userReport.ReportTo;
			}
		}

		private UserReportInfo GetReportToUser(string userId)
		{
			return UserReportInfoAdapter.Instance.LoadUserReportToInfo(userId);
		}

		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			string userId = Request.QueryString.GetValue("userId", string.Empty);
			try
			{
				UserReportInfo userReport = UserReportInfoAdapter.Instance.LoadUserReportToInfo(userId);
				if (userReport != null)
				{
					userReport.ReportTo = (IUser)OuUserInputControl.SelectedSingleData;
				}
				else
				{
					userReport = new UserReportInfo();
					userReport.User = new OguUser(userId);
					userReport.ReportTo = (IUser)OuUserInputControl.SelectedSingleData;
				}
				
				UserReportInfoAdapter.Instance.Update(userReport);
				
				Page.ClientScript.RegisterStartupScript(this.GetType(), "returnProcesses",
					string.Format("window.returnValue = true; top.close();"),
					true);
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
		}
	}
}