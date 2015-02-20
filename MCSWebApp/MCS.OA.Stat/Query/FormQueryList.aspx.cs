using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.OA.Stat.Common;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using SinoOcean.OA.Stat;
using MCS.Library.SOA.DataObjects.Archive;
using MCS.Library.Data;
using MCS.OA.Stat.Query;

namespace MCS.OA.Stat
{
	public partial class FormQueryList : System.Web.UI.Page
	{
		private int LastQueryRowCount
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value);
			}
		}

		private int height = 520;
		private int Height
		{
			get
			{
				return height;
			}
			set
			{
				this.height = value;
			}
		}

		private FormQueryCondition QueryCondition
		{
			get
			{
				FormQueryCondition result = (FormQueryCondition)ViewState["QueryCondition"];
				if (result == null)
				{
					result = new FormQueryCondition();
					ViewState["QueryCondition"] = result;
				}
				return result;
			}
		}

		[ControllerMethod(true)]
		protected void DefaultProcess()
		{
			ExecQuery();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			bindingControl.Data = QueryCondition;
			//Note:讨论完成后在同一修改
			this.GridViewFormQuery.PageSize =
				UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID).GetPropertyValue("CommonSettings", "ToDoListPageSize", GridViewFormQuery.PageSize);

			if (this.queryHistoryFlag.Value == "QueryHistory")
			{
				ArchiveSettings.GetConfig().ConnectionMappings.CreateConnectionMappingContexts();
			}
		}


		/// <summary>
		/// 组织查询条件
		/// </summary>
		private void ExecQuery()
		{
			bindingControl.CollectData();

			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(QueryCondition,
				new AdjustConditionValueDelegate(AdjustQueryConditionValue));

			whereCondition.Value = builder.ToSqlString(TSqlBuilder.Instance);

			//申请部门
			if (QueryCondition.DraftDepartmentName != string.Empty)
			{
				if (string.IsNullOrEmpty(whereCondition.Value))
				{
					whereCondition.Value += string.Format(" CONTAINS(DRAFT_DEPARTMENT_NAME,'\"*"
					+ TSqlBuilder.Instance.CheckQuotationMark(QueryCondition.DraftDepartmentName, false) + "*\"')");
				}
				else
				{
					whereCondition.Value += string.Format(" AND CONTAINS(DRAFT_DEPARTMENT_NAME,'\"*"
					+ TSqlBuilder.Instance.CheckQuotationMark(QueryCondition.DraftDepartmentName, false) + "*\"')");
				}
			}

			LastQueryRowCount = -1;

			this.GridViewFormQuery.PageIndex = 0;
			this.GridViewFormQuery.SelectedKeys.Clear();
		}

		protected void ObjectDataSourceFormQuery_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["totalCount"] = LastQueryRowCount;
			if (!IsPostBack)
			{
				e.Cancel = true;
			}
		}

		protected void ObjectDataSourceFormQuery_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			LastQueryRowCount = (int)e.OutputParameters["totalCount"];
		}

		protected void GridViewFormQuery_RowDataBound(object sender, GridViewRowEventArgs e)
		{

			if (e.Row.RowType == DataControlRowType.Header)
			{
				if (this.GridViewFormQuery.ExportingDeluxeGrid)
				{
					e.Row.Cells[8].Visible = false;
				}
			}

			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				UserTaskCommon.SetRowStyleWhenMouseOver(e.Row);

				AppCommonInfoProcess entity = (AppCommonInfoProcess)e.Row.DataItem;

				UserTask userTask = new UserTask();

				if (this.GridViewFormQuery.ExportingDeluxeGrid)
				{
					e.Row.Cells[1].Text = EnumItemDescriptionAttribute.GetAttribute(entity.Emergency).ShortName;

					e.Row.Cells[2].Text = Server.HtmlEncode(entity.Subject).ToString().Replace(" ", "&nbsp;");
					//e.Row.Cells[8].Visible = false;
				}
				else
				{
					e.Row.Cells[1].Text = UserTaskCommon.GetEmergencyImageHtml(entity.Emergency);

					string url = entity.Url;

					if (RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin"))
					{
						NameValueCollection reqParams = UriHelper.GetUriParamsCollection(entity.Url);

						reqParams["mode"] = "Admin";

						url = UriHelper.CombineUrlParams(entity.Url, reqParams);
					}

					//title
					userTask.Url = url;
					userTask.SourceID = entity.ResourceID;
					userTask.ApplicationName = entity.ApplicationName;
					userTask.TaskTitle = entity.Subject;

					e.Row.Cells[2].Text = UserTaskCommon.GetTaskURL(userTask);
				}
			}
		}

		/// <summary>
		/// 对绑定的查询对象属性做更改的委托
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="data"></param>
		/// <param name="ignored"></param>
		/// <returns></returns>
		private static object AdjustQueryConditionValue(string propertyName, object data, ref bool ignored)
		{
			object result = data;
			switch (propertyName)
			{
				//标题使用全文检索，格式单独处理
				case "Subject":
					result = "%" + data + "%";
					break;
				case "Emergency":
					result = data;
					break;
				case "DraftDepartmentName":
					ignored = true;
					break;
				case "CreateTimeEnd":
					result = ((DateTime)data).AddDays(1);
					break;
			}

			return result;
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnQuery_Click(object sender, EventArgs e)
		{
			queryHistoryFlag.Value = "";
			DbConnectionMappingContext.ClearAllMappings();

			ExecQuery();
		}

		protected void btnQueryHistory_Click(object sender, EventArgs e)
		{
			DbConnectionMappingContext.ClearAllMappings();
			ArchiveSettings.GetConfig().ConnectionMappings.CreateConnectionMappingContexts();

			queryHistoryFlag.Value = "QueryHistory";

			ExecQuery();
		}

		protected void RefreshButton_Click(object sender, EventArgs e)
		{
			///// 更新列表数据，保留页码，这件事情将由Grid控件完成
			///// 若更新后的页码不存在，则取最大页码

		}

		protected void archiveProcess_ExecuteStep(object data)
		{
			ArchiveBasicInfo info = new ArchiveBasicInfo();

			info.ResourceID = (string)data;
			IArchiveExecutor executor = ArchiveSettings.GetConfig().GetFactory().GetArchiveExecutor(info);

			executor.Archive(info);
		}

		protected override void OnPreRender(EventArgs e)
		{
			archiveBtn.Visible = ArchiveSettings.GetConfig().Enabled;
			btnQueryHistory.Visible = ArchiveSettings.GetConfig().Enabled;

			this.GridViewFormQuery.DataBind();

			if (queryHistoryFlag.Value == "QueryHistory")
				showHistoryFlag.Text = "历史库中的内容";
			else
				showHistoryFlag.Text = string.Empty;

			base.OnPreRender(e);
		}
	}
}