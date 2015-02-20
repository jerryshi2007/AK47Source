using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Passport;
using MCS.Library.OGUPermission;
using MCS.Library.Data;
using MCS.Web.Library;

namespace MCS.OA.CommonPages.AppTrace
{
	public partial class Category : System.Web.UI.Page
	{
		#region 查询条件

		//public static readonly string ThisPageSearchResourceKey = "B0D7A139-F1D7-43C7-A705-0EB76B0F16D0";

		//[Serializable]
		//private class PageQueryCondition
		//{
		//    [ConditionMapping("APPLICATION_NAME")]
		//    public string ApplicationName { get; set; }

		//    [ConditionMapping("PROGRAM_NAME")]
		//    public string ProgramName { get; set; }

		//    [ConditionMapping("AUTH_TYPE", EnumUsage = EnumUsageTypes.UseEnumValue)]
		//    public WfApplicationAuthType AuthType { get; set; }

		//    //[ConditionMapping("B.CREATE_TIME", ">=")]
		//    //public DateTime CreationTimeFrom { get; set; }

		//    //[ConditionMapping("B.CREATE_TIME", "<=")]
		//    //public DateTime CreationTimeTo { get; set; }
		//}

		#endregion

		private bool supervisiorMode = false;
		private bool isSupervisiorModeRetrived = false;

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetNoStore();
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			this.panEdit.Visible = this.SupervisiorMode;
			this.gridViewTask.Columns[this.gridViewTask.Columns.Count - 1].Visible = this.SupervisiorMode;
		}

		protected void DoCommand(object sender, CommandEventArgs e)
		{
			if (e.CommandName == "toggle")
			{
				switch (e.CommandArgument.ToString())
				{
					case "ByCategory":
						this.views.ActiveViewIndex = 0;
						this.lastUser.Value = null;
						this.gridViewTask.DataBind();
						break;
					case "ByPerson":
						this.views.ActiveViewIndex = 1;
						this.gridViewTask.DataBind();
						break;
					default:
						break;
				}
			}
		}

		protected void FilterByPerson(object sender, EventArgs e)
		{
			this.lastUser.Value = this.user1.SelectedSingleData.ID;
			this.gridViewTask.DataBind();
		}

		protected void RefreshList(object sender, EventArgs e)
		{
			this.InnerRefreshList();
		}

		public bool SupervisiorMode
		{
			get
			{
				if (isSupervisiorModeRetrived == false)
				{
					this.supervisiorMode = RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin");

					isSupervisiorModeRetrived = true;
				}

				return this.supervisiorMode;
			}
		}

		protected void ServerChangeRole(object sender, EventArgs e)
		{
			if (this.SupervisiorMode)
			{
				string appName = postAppName.Value;
				string pgName = postProgram.Value;
				WfApplicationAuthType type = (WfApplicationAuthType)Enum.Parse(typeof(WfApplicationAuthType), postType.Value);

				string roleId = postRole.Value;

				var auth = WfApplicationAuthAdapter.Instance.Load(appName, pgName, type);

				if (auth != null)
				{
					auth.RoleID = roleId;
					auth.RoleDescription = postRoleName.Value;
					WfApplicationAuthAdapter.Instance.Update(auth);
				}
			}

			this.InnerRefreshList();
		}

		private void InnerRefreshList()
		{
            this.Page.DataBind();
		}

		private void DelayRefreshList(object sender, EventArgs e)
		{
			this.gridViewTask.DataBind();
		}

		protected void ObjectDataSourceSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			if (e.ExecutingSelectCount == false)
			{
				e.InputParameters["authType"] = (WfApplicationAuthType)int.Parse(this.rdoList.SelectedValue);
				if (views.ActiveViewIndex == 1)
				{
					if (string.IsNullOrEmpty(this.lastUser.Value))
					{
						e.Cancel = true;
					}
					else
					{
						e.InputParameters["appName"] = null;
						e.InputParameters["programName"] = null;
						e.InputParameters["authType"] = WfApplicationAuthType.None;

						WfApplicationAuthCollection authInfo = WfApplicationAuthAdapter.Instance.GetUserApplicationAuthInfo(new OguUser(lastUser.Value));

						string condition = "1=2";

						if (authInfo.Count > 0)
						{
							ConnectiveSqlClauseCollection allMatch = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or);

							foreach (var item in authInfo)
							{
								WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
								where.AppendItem("APPLICATION_NAME", item.ApplicationName);
								where.AppendItem("PROGRAM_NAME", item.ProgramName);
								where.AppendItem("AUTH_TYPE", item.AuthType);
								allMatch.Add(where);
							}

							condition = allMatch.ToSqlString(TSqlBuilder.Instance);
						}
						else
						{
							e.Cancel = true;
						}

						e.InputParameters["where"] = condition;
					}
				}
			}
		}

        protected void ReLoadPrograms(object sender, EventArgs e)
		{
			this.ddProgam.DataBind();
		}

		protected void HandleDelete(object sender, EventArgs e)
		{
			if (this.SupervisiorMode)
			{
				LinkButton lb = sender as LinkButton;
				if (lb != null)
				{
					string appName = lb.Attributes["data-appname"];
					string pgname = lb.Attributes["data-pgname"];
					WfApplicationAuthType type = (WfApplicationAuthType)Enum.Parse(typeof(WfApplicationAuthType), lb.Attributes["data-type"]);

					using (System.Transactions.TransactionScope scope = TransactionScopeFactory.Create())
					{
						WfApplicationAuth auth = WfApplicationAuthAdapter.Instance.Load(appName, pgname, type);
						if (auth != null)
						{
							WfApplicationAuthAdapter.Instance.Delete(auth);
						}

						scope.Complete();
					}
				}
			}
			else
			{
				WebUtility.ShowClientError("只有管理员可以进行删除", "", "失败");
			}

			this.InnerRefreshList();
		}
	}
}