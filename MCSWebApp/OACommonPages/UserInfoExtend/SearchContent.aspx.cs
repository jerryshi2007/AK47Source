using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using System.Data.SqlClient;
using System.Data;
using MCS.Web.Library.MVC;
using MCS.Web.WebControls;

namespace MCS.OA.CommonPages.UserInfoExtend
{
	public partial class SearchContent : System.Web.UI.Page
	{
		private SearchUserQueryCondition QueryCondition
		{
			get
			{
				SearchUserQueryCondition result = (SearchUserQueryCondition)ViewState["QueryCondition"];

				if (result == null)
				{
					result = new SearchUserQueryCondition();
					ViewState["QueryCondition"] = result;
				}

				return result;
			}
		}

		private int LastQueryCount
		{
			get
			{
				return ViewState.GetViewStateValue("LastQueryCount", -1);
			}
			set
			{
				ViewState.SetViewStateValue("LastQueryCount", value);
			}
		}

		[ControllerMethod(true)]
		protected void DefaultProcess(string id)
		{
			if (string.IsNullOrEmpty(id) == true)
				id = OguMechanismFactory.GetMechanism().GetRoot().ID;
			QueryCondition.DepartmentId = id;
			ExecQuery();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetNoStore();
			if (!IsPostBack)
			{
				ControllerHelper.ExecuteMethodByRequest(this);

				//更新公司的联系地址
				if (this.GridView1.Rows.Count == 0)
				{
					GetCompanyInfo();
				}
			}

			if (this.GridView1.Rows.Count != 0)
			{
				this.company.Visible = false;
			}
		}

		public void GetCompanyInfo()
		{
			string sql = "SELECT GUID,DISPLAY_NAME  FROM ORGANIZATIONS WHERE SYSCONTENT2 is not null AND SYSCONTENT3 is not null order by DISPLAY_NAME DESC";
			DataTable table = null;
			DbHelper.RunSql(db => table = db.ExecuteDataSet(CommandType.Text, sql).Tables[0], "AccreditAdmin");

			if (table.Rows.Count > 0)
			{
				foreach (DataRow row in table.Rows)
				{
					DDLCompany.Items.Clear();
					DDLCompany.Items.Add(new ListItem(row["DISPLAY_NAME"].ToString(), row["GUID"].ToString()));
					DDLCompany.SelectedIndex = 0;
					DDLCompany_SelectedIndexChanged(null, null);
				}
			}
		}

		protected void BtnSearch_Click(object sender, EventArgs e)
		{
			ExecQuery();
		}

		private void ExecQuery()
		{
			this.QueryCondition.Name = tbFormName.Text.Trim();

			if (string.IsNullOrEmpty(this.QueryCondition.Name))
				QueryCondition.Name = "@SearchAll@";

			this.userName.Value = QueryCondition.Name;
			this.departmentId.Value = QueryCondition.DepartmentId;

			LastQueryCount = -1;
			this.GridView1.PageIndex = 0;
		}

		protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				IUser user = (IUser)e.Row.DataItem;

				HtmlAnchor lbtUpdate = (HtmlAnchor)e.Row.FindControl("LinkBtnViewDetails");
				string fpath = HttpUtility.UrlEncode(user.FullPath);
				lbtUpdate.Attributes.Add("onclick", "onUpdateClick('" + fpath + "')");

				HtmlAnchor lbtBlog = (HtmlAnchor)e.Row.FindControl("LinkBtnViewBlogs");
				lbtBlog.HRef = myBlog + "userRedirect.asp?username=" + user.LogOnName;

				HtmlAnchor lbtPortal = (HtmlAnchor)e.Row.FindControl("LinkBtnViewKM");
				lbtPortal.HRef = MyPortal + user.LogOnName + @"/default.aspx";

				FillUserMobileToLabel(user, "Mobile", e.Row, "lblMobile");

				FillUserPropertyToLabel(user, "OfficeTel", e.Row, "lblTel");
				FillUserPropertyToLabel(user, "IntranetEmail", e.Row, "lblIntranetEmail");
				FillUserPropertyToLabel(user, "InternetEmail", e.Row, "lblInternetEmail");
				FillUserPropertyToLabel(user, "IMAddress", e.Row, "lblIMAddress");
                if (user != null)
                {
                    FillReportToInfo(user, "ReportTo", e.Row, "linkReportTo");
                }
				
			}
		}

		private static void FillReportToInfo(IUser user, string propertyName, Control parent, string labelID)
		{
			HyperLink lbl = (HyperLink)parent.FindControl(labelID);
			UserPresence presence = (UserPresence)parent.FindControl("reportTouserPresence");

			if (lbl != null)
			{
				UserReportInfo reportInfo = GetUserProperty(user, "ReportTo", (UserReportInfo)null);

                if (reportInfo != null)
                {
                    presence.UserID = reportInfo.ReportTo.ID;
                    presence.UserDisplayName = reportInfo.ReportTo.DisplayName;

                    lbl.Text = HttpUtility.HtmlEncode(reportInfo.ReportTo.DisplayName);
                    lbl.NavigateUrl = "editUserReportToInfo.aspx?userID=" + HttpUtility.UrlEncode(reportInfo.User.ID);
                }
                else
                {
                    lbl.Style[HtmlTextWriterStyle.PaddingLeft] = "16px";
                    lbl.Text = HttpUtility.HtmlEncode("增加");
                    lbl.NavigateUrl = "editUserReportToInfo.aspx?userID=" + HttpUtility.UrlEncode(user.ID);

                }

			}
		}

		private static void FillUserMobileToLabel(IUser user, string propertyName, Control parent, string labelID)
		{
			LinkButton lbl = (LinkButton)parent.FindControl(labelID);

			if (lbl != null)
			{
				lbl.Text = HttpUtility.HtmlEncode(GetUserProperty(user, propertyName, string.Empty));
				lbl.OnClientClick = "onMobileClick('" + lbl.Text + "');return false;";
			}

		}

		public void FillUserPropertyToLabel(IUser user, string propertyName, Control obj, string rowId)
		{
			Label lbl = (Label)obj.FindControl(rowId);

			if (lbl != null)
				lbl.Text = HttpUtility.HtmlEncode(GetUserProperty(user, propertyName, string.Empty));
		}

		private static T GetUserProperty<T>(IUser user, string propertyName, T defaultValue)
		{
			T result = defaultValue;

			if (user.Properties.Contains(propertyName))
				result = (T)DataConverter.ChangeType(user.Properties[propertyName], typeof(T));

			return result;
		}

		public string myBlog
		{
			get { return System.Configuration.ConfigurationManager.AppSettings["BlogSite"]; }
		}

		public string MyPortal
		{
			get
			{
				return System.Configuration.ConfigurationManager.AppSettings["MyPortal"];
			}
		}

		protected void ObjectDataSource1_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			LastQueryCount = (int)e.OutputParameters["totalCount"];
			if (LastQueryCount > 0)
			{
				this.company.Visible = false;
			}
		}

		protected void ObjectDataSource1_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["totalCount"] = LastQueryCount;
		}

		protected void DDLCompany_SelectedIndexChanged(object sender, EventArgs e)
		{
			string sql = string.Format("SELECT SYSDISTINCT1,SYSDISTINCT2,SYSCONTENT2,SYSCONTENT3  FROM ORGANIZATIONS WHERE GUID = '{0}'", DDLCompany.SelectedValue);

			DataTable table = null;
			DbHelper.RunSql(db => table = db.ExecuteDataSet(CommandType.Text, sql).Tables[0], "AccreditAdmin");

			if (table != null && table.Rows.Count != 0)
			{
				address.Text = "公司地址：" + table.Rows[0]["SYSCONTENT3"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;邮政编码：" + table.Rows[0]["SYSCONTENT2"].ToString();
				Fax.Text = "公司总机：" + table.Rows[0]["SYSDISTINCT1"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;公司传真：" + table.Rows[0]["SYSDISTINCT2"].ToString();
			}
		}

        protected void hiddenServerBtn_Click(object sender, EventArgs e)
        {
            LastQueryCount = -1;
        }
	}
}