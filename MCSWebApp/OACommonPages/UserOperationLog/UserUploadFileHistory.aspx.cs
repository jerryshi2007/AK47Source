using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Principal;

namespace MCS.OA.CommonPages.UserOperationLog
{
	public partial class UserUploadFileHistory : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				this.BindStatusList();
				UserUploadQueryCondition initCondition = new UserUploadQueryCondition();
				if (string.IsNullOrEmpty(Request.Params["operatorID"]) == false)
				{
					initCondition.Operator = Request.Params["operatorID"];
					this.processUsers.ReadOnly = true;
				}

				if (string.IsNullOrEmpty(Request.Params["appName"]) == false)
					initCondition.ApplicationName = Request.Params["appName"];

				if (string.IsNullOrEmpty(Request.Params["programName"]) == false)
					initCondition.ProgramName = Request.Params["programName"];

				this.UserUploadCondition = initCondition;
				this.bindingControl.Data = this.UserUploadCondition;
				this.BindDataGrid();
			}
			else
			{
				this.bindingControl.Data = this.UserUploadCondition;
			}
		}

		private void BindStatusList()
		{
			EnumItemDescriptionList bindSelectStatus = EnumItemDescriptionAttribute.GetDescriptionList(typeof(UploadFileHistoryStatus));
			this.dr_Status.Items.Add(new ListItem());
			foreach (EnumItemDescription item in bindSelectStatus)
			{
				this.dr_Status.Items.Add(new ListItem() { Text = HttpUtility.HtmlEncode(item.Description), Value = HttpUtility.HtmlEncode(item.EnumValue) });
			}
		}

		private void BindDataGrid()
		{
			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(this.UserUploadCondition);

			if (this.processUsers.ReadOnly)
			{
				this.ObjectDataSourceUploadLogs.Condition = builder;
			}
			else
			{
				ConnectiveSqlClauseCollection collection = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And);
				collection.Add(builder);

				WhereSqlClauseBuilder userwherebuilder = new WhereSqlClauseBuilder();
				userwherebuilder.LogicOperator = LogicOperatorDefine.Or;
				foreach (var item in this.processUsers.SelectedOuUserData)
				{
					userwherebuilder.AppendItem("OPERATOR_ID", item.ID);
				}
				collection.Add(userwherebuilder);

				this.ObjectDataSourceUploadLogs.Condition = collection.ToSqlString(TSqlBuilder.Instance);
			}
			this.ObjectDataSourceUploadLogs.LastQueryRowCount = -1;

			this.DeluxeGridUploadLog.PageIndex = 0;
			this.DeluxeGridUploadLog.SelectedKeys.Clear();
			this.DeluxeGridUploadLog.DataBind();
		}

		protected UserUploadQueryCondition UserUploadCondition
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "UserUploadCondition", default(UserUploadQueryCondition));
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "UserUploadCondition", value);
			}
		}

		/// <summary>
		/// 查询按钮事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Srarch_Click(object sender, EventArgs e)
		{
			this.bindingControl.CollectData(false);
			this.UserUploadCondition = this.bindingControl.Data as UserUploadQueryCondition;

			this.BindDataGrid();
		}

		/*	private int LastQueryRowCount
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

			protected void ObjectDataSourceLogs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
			{
				e.InputParameters["totalCount"] = this.LastQueryRowCount;
			}

			protected void ObjectDataSourceLogs_Selected(object sender, ObjectDataSourceStatusEventArgs e)
			{
				this.LastQueryRowCount = (int)e.OutputParameters["totalCount"];
			}

			 private void BuilderQueryString()
			 {
				 WhereSqlClauseBuilder wherebuilder = new WhereSqlClauseBuilder();
				 WhereSqlClauseBuilder userwherebuilder = new WhereSqlClauseBuilder();
				 if (this.processUsers.ReadOnly)
				 {
					 if (string.IsNullOrEmpty(this.tb_operUser.Value) == false)
						 wherebuilder.AppendItem("OPERATOR_ID", this.operUser.Value);
				 }
				 else
				 {
					 userwherebuilder.LogicOperator = LogicOperatorDefine.Or;
					 foreach (var item in this.processUsers.SelectedOuUserData)
					 {
						 userwherebuilder.AppendItem("OPERATOR_ID", item.ID);
					 }
				 }

				 //if (string.IsNullOrEmpty(this.tb_ProgramName.Value) == false)
				 //{
				 //    wherebuilder.AppendItem("PROGRAM_NAME", string.Format("%{0}%", this.tb_ProgramName.Value), "like", false);
				 //}

				 //if (string.IsNullOrEmpty(this.tb_ApplicationName.Value) == false)
				 //{
				 //    wherebuilder.AppendItem("APPLICATION_NAME", string.Format("%{0}%", this.tb_ApplicationName.Value), "like", false);
				 //}

				 if (string.IsNullOrEmpty(this.dr_Status.SelectedValue) == false)
				 {
					 wherebuilder.AppendItem("STATUS", int.Parse(this.dr_Status.SelectedValue));
				 }

				 if (this.StartDate.Value != default(DateTime) && this.EndDate.Value != default(DateTime))
				 { }

				 if (userwherebuilder.IsEmpty == false && wherebuilder.IsEmpty == false)
				 {
					 ConnectiveSqlClauseCollection collection = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And);
					 collection.Add(wherebuilder);
					 collection.Add(userwherebuilder);
					 this.whereCondition.Value = collection.ToSqlString(TSqlBuilder.Instance);
				 }
				 else if (wherebuilder.IsEmpty == false)
				 {
					 this.whereCondition.Value = wherebuilder.ToSqlString(TSqlBuilder.Instance);
				 }
				 else if (userwherebuilder.IsEmpty == false)
				 {
					 this.whereCondition.Value = userwherebuilder.ToSqlString(TSqlBuilder.Instance);
				 }
			 } */
	}
}