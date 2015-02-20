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
using MCS.Library.OGUPermission;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace MCS.OA.CommonPages.DelegationAuthorized
{
	public partial class AllOriginalDelegationList : System.Web.UI.Page
	{
		[Serializable]
		internal class SearchCondition
		{
			IOguObject activeUser = null, passiveUser = null;

			[SubConditionMapping("ID", "SOURCE_USER_ID")]
			public IOguObject ActiveUser
			{
				get
				{
					return this.activeUser;
				}

				set
				{
					this.activeUser = value != null ? OguBase.CreateWrapperObject(value) : null;
				}
			}

			[SubConditionMapping("ID", "DESTINATION_USER_ID")]
			public IOguObject PassiveUser
			{
				get
				{
					return this.passiveUser;
				}

				set
				{
					this.passiveUser = value != null ? OguBase.CreateWrapperObject(value) : null;
				}
			}

			[ConditionMapping("DESTINATION_USER_NAME", EscapeLikeString = true, Operation = "LIKE", Postfix = "%")]
			public string PassiveUserName { get; set; }

			[ConditionMapping("SOURCE_USER_NAME", EscapeLikeString = true, Operation = "LIKE", Postfix = "%")]
			public string ActiveUserName { get; set; }

			[ConditionMapping("ENABLED")]
			public bool Enabled { get; set; }

			public void Clear()
			{
				this.activeUser = null;
				this.passiveUser = null;
				this.ActiveUserName = null;
				this.PassiveUserName = null;
				this.Enabled = false;
			}
		}

		internal SearchCondition CurrentCondition
		{
			get
			{
				return this.ViewState["CurrentCondition"] as SearchCondition;
			}
			set
			{
				this.ViewState["CurrentCondition"] = value;
			}
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			if (this.IsPostBack == false)
			{
				this.CurrentCondition = new SearchCondition();
				this.lnkToLog.NavigateUrl = "~/UserOperationLog/UserOperationLogView.aspx?resourceID=" + HttpUtility.UrlEncode(LogUtil.ResourceID);
			}

			this.searchBinding.Data = this.CurrentCondition;
		}


		protected void ButtonSearchClick(object sender, EventArgs e)
		{
			this.DeluxeGridDelegation.PageIndex = 0;
			((SearchCondition)this.searchBinding.Data).Clear();
			this.searchBinding.CollectData();
			this.InnerRefreshList();
		}

		protected void DeluxeGridDelegationList_RowDataBound(object sender, GridViewRowEventArgs e)
		{
		}

		protected void DeluxeGridDelegationList_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DeleteDelegation")
			{
				var lnkBtn = e.CommandSource as LinkButton;
				if (lnkBtn != null)
				{
					string sid = lnkBtn.Attributes["data-sid"];
					string tid = lnkBtn.Attributes["data-tid"];

					WfDelegationCollection wfDelegation = WfDelegationAdapter.Instance.Load(builder =>
					{
						builder.AppendItem("SOURCE_USER_ID", sid);
						builder.AppendItem("DESTINATION_USER_ID", tid);
					});

					if (wfDelegation.Count == 1)
					{
						WfDelegationAdapter.Instance.Delete(wfDelegation[0]);

						LogUtil.AppendLogToDb(LogUtil.CreateDissassignLog(wfDelegation[0]));
					}
					else
					{
						throw new ApplicationException("不存在指定条件的委托");
					}

					this.InnerRefreshList();
				}
			}
		}

		protected void DeluxeGridDelegationList_ExportData(object sender, EventArgs e)
		{

		}


		protected void ObjectDataSourceDelegationList_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			var condition = this.CurrentCondition;

			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

			if (condition.Enabled)
			{
				builder.AppendItem("END_TIME", "getdate()", ">", true);
			}

			if (builder.IsEmpty == false)
				dataSourceMain.Condition = builder;
			else
				dataSourceMain.Condition = null;
		}

		protected void RefreshButton_Click(object sender, EventArgs e)
		{
			this.InnerRefreshList();
		}

		private void InnerRefreshList()
		{
			// 重新刷新列表
			this.dataSourceMain.LastQueryRowCount = -1;
			this.DeluxeGridDelegation.SelectedKeys.Clear();
			this.Page.PreRender += new EventHandler(this.DelayRefreshList);
		}

		private void DelayRefreshList(object sender, EventArgs e)
		{
			this.DeluxeGridDelegation.DataBind();
		}
	}
}