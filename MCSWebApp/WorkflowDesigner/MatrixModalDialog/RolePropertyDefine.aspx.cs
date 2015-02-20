using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;
using System.Diagnostics;
using MCS.Library.Data.DataObjects;

namespace MCS.Applications.AppAdmin.Dialogs
{
	public partial class RolePropertyDefine : System.Web.UI.Page, ICallbackEventHandler
	{
		string roleID = string.Empty;

		public string RoleID
		{
			get
			{
				if (string.IsNullOrEmpty(roleID))
				{
					roleID = Request["RoleID"] ?? "";
				}
				return roleID;
			}
			set { roleID = value; }
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (this.Request.QueryString["editMode"] == "readOnly")
			{
				this.btnConfirm.Visible = false;
				this.Title = "角色扩展属性（只读模式）";
				this.under.Visible = false;
				this.detailGrid.ReadOnly = true;
			}

			if (this.Page.IsPostBack == false)
			{
				SetHiddenJsonData();

				if (this.detailGrid.InitialData.Count == 0)
				{
					detailGrid.InitialData = new SOARolePropertyDefinitionCollection() {
                     new SOARolePropertyDefinition()
                     { 
                        DataType = ColumnDataType.String,
                        Description = "操作人",
                        Name = "Operator",
                        SortOrder = 0},
                     new SOARolePropertyDefinition()
                     { 
                        DataType = ColumnDataType.String,
                        Description = "操作人类型",
                        Name="OperatorType",
                        SortOrder = 1}
                    };
				}
			}

			base.OnPreRender(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			WebUtility.RequiredScript(typeof(ClientGrid));

			this.txtRoleID.Text = RoleID;
			//数据库类型下拉控件数据绑定
			var dataTypeList = EnumItemDescriptionAttribute.GetDescriptionList(typeof(PropertyDataType));
			dataTypeDropDownList.DataSource = dataTypeList;
			dataTypeDropDownList.DataTextField = "Name";
			dataTypeDropDownList.DataValueField = "EnumValue";
			dataTypeDropDownList.DataBind();
		}

		private void SetHiddenJsonData()
		{
			try
			{
				RoleID = MCS.Web.Library.WebUtility.GetRequestParamString("RoleID", string.Empty);

				if (string.IsNullOrEmpty(RoleID) == false)
				{
					var roleDefColl = SOARolePropertyDefinitionAdapter.Instance.LoadByRoleID(RoleID);
					hiddenMatrixDimDefJsonData.Value = JSONSerializerExecute.Serialize(roleDefColl);
					this.detailGrid.InitialData = roleDefColl;

				}
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
		}

		public string GetCallbackResult()
		{
			return CheckRoleID(RoleID).ToString();
		}

		public bool CheckRoleID(string RoleID)
		{
			try
			{
				SOARolePropertyDefinitionAdapter.Instance.LoadByRoleID(RoleID);
				return false;
			}
			catch (Exception)
			{
				return true;
			}
		}

		public void RaiseCallbackEvent(string eventArgument)
		{
			RoleID = eventArgument;
		}

		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			if (this.Request.QueryString["editMode"] == "readOnly")
				throw new InvalidOperationException("只读模式不可以编辑");

			RoleID = MCS.Web.Library.WebUtility.GetRequestParamString("RoleID", string.Empty);
			SOARole role = new SOARole() { ID = RoleID };

			SOARolePropertyRowCollection rowsColl = SOARolePropertiesAdapter.Instance.LoadByRole(role);

			/*沈峥注释，不需要这个限制
			if (rowsColl.Count > 0)
			{
				WebUtility.ShowClientMessage("矩阵定义已经被使用，无法再修改！", "", "无法进行此操作");
				return;
			}
			*/

			var coll = detailGrid.InitialData as SOARolePropertyDefinitionCollection;
			SOARolePropertyDefinitionAdapter.Instance.Update(role, coll);

			Page.ClientScript.RegisterStartupScript(this.GetType(), "returnProcesses",
					string.Format("window.returnValue = true; top.close();"),
					true);
		}
	}
}