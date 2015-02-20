using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Library.Core;
using WfFormTemplate.List;
using MCS.Web.Library.Script;
using MCS.Library.Validation;
using System.ComponentModel;
using MCS.Web.WebControls;

namespace WfFormTemplate.List
{
	public partial class UnCompletedTaskList : System.Web.UI.Page
	{

		#region 查询条件

		public static readonly string ThisPageSearchResourceKey = "97172E3B-BEB1-4EF8-821E-D1BF281837AB";

		[Serializable]
		private class TaskQueryCondition
		{
			private string taskTitle = string.Empty;
			private DateTime taskStartTime = DateTime.MinValue;
			private DateTime deliverTimeBegin = DateTime.MinValue;
			private DateTime deliverTimeEnd = DateTime.MinValue;
			private string applicationName = string.Empty;
			private string sourceID = string.Empty;
			private string purpose = string.Empty;
			private DateTime expireTimeBegin = DateTime.MinValue;
			private DateTime expireTimeEnd = DateTime.MinValue;
			private string categoryID = string.Empty;
			private DateTime completedTimeBegin = DateTime.MinValue;
			private DateTime completedTimeEnd = DateTime.MinValue;
			private string userID = string.Empty;
			private string draftDepartmentName = string.Empty;
			private string programName = string.Empty;
			private string resourceID = string.Empty;
			private string draftUserID = string.Empty;
			private string draftUserName = string.Empty;

			[ConditionMapping("TASK_START_TIME")]
			public DateTime TaskStartTime
			{
				get { return taskStartTime; }
				set { taskStartTime = value; }
			}

			[ConditionMapping("DRAFT_USER_NAME")]
			public string DraftUserName
			{
				get { return draftUserName; }
				set { draftUserName = value; }
			}

			[ConditionMapping("DRAFT_USER_ID")]
			public string DraftUserID
			{
				get { return draftUserID; }
				set { draftUserID = value; }
			}

			/// <summary>
			/// 服务名称
			/// </summary>
			[ConditionMapping("RESOURCE_ID")]
			public string ResourceID
			{
				get { return resourceID; }
				set { draftUserID = value; }
			}

			/// <summary>
			/// 服务名称
			/// </summary>
			[ConditionMapping("PROGRAM_NAME")]
			public string ProgramName
			{
				get { return programName; }
				set { programName = value; }
			}

			/// <summary>
			/// 标题
			/// </summary>
			[ConditionMapping("TASK_TITLE", "like")]
			[StringLengthValidator(0, 100, MessageTemplate = "查询标题必须少于100字")]
			public string TaskTitle
			{
				get { return taskTitle; }
				set { taskTitle = value; }
			}

			/// <summary>
			/// 开始时间
			/// </summary>
			[ConditionMapping("DELIVER_TIME", ">=")]
			public DateTime DeliverTimeBegin
			{
				get { return deliverTimeBegin; }
				set { deliverTimeBegin = value; }
			}

			[ConditionMapping("DELIVER_TIME", "<")]
			public DateTime DeliverTimeEnd
			{
				get { return deliverTimeEnd; }
				set { deliverTimeEnd = value; }
			}

			/// <summary>
			/// 紧急程度
			/// </summary>
			//[ConditionMapping("EMERGENCY")]
			//public SinoOceanFileEmergency Emergency
			//{
			//    get { return this.emergency; }
			//    set { this.emergency = value; }
			//}

			[ConditionMapping("DRAFT_DEPARTMENT_NAME", IsExpression = true)]
			public string DraftDepartmentName
			{
				get { return draftDepartmentName; }
				set { draftDepartmentName = value; }
			}

			/// <summary>
			/// 应用名称
			/// </summary>
			[ConditionMapping("APPLICATION_NAME", "=")]
			public string ApplicationName
			{
				get { return applicationName; }
				set { applicationName = value; }
			}

			/// <summary>
			/// 发送人ID
			/// </summary>
			[ConditionMapping("SOURCE_ID", "IN", IsExpression = true)]
			public string SourceID
			{
				get { return sourceID; }
				set { sourceID = value; }
			}

			/// <summary>
			/// 目的
			/// </summary>
			[ConditionMapping("PURPOSE", "like")]
			[StringLengthValidator(0, 30, MessageTemplate = "查询目的必须少于30字")]
			public string Purpose
			{
				get { return purpose; }
				set { purpose = value; }
			}

			/// <summary>
			/// 办理期限
			/// </summary>
			[ConditionMapping("EXPIRE_TIME", ">=")]
			public DateTime ExpireTimeBegin
			{
				get { return expireTimeBegin; }
				set { expireTimeBegin = value; }
			}

			[ConditionMapping("EXPIRE_TIME", "<")]
			public DateTime ExpireTimeEnd
			{
				get { return expireTimeEnd; }
				set { expireTimeEnd = value; }
			}

			/// <summary>
			/// 类别ID
			/// </summary>
			[ConditionMapping("CATEGORY_GUID")]
			public string CategoryID
			{
				get { return categoryID; }
				set { categoryID = value; }
			}

			/// <summary>
			/// 完成时间
			/// </summary>
			[ConditionMapping("COMPLETED_TIME", ">=")]
			public DateTime CompletedTimeBegin
			{
				get { return completedTimeBegin; }
				set { completedTimeBegin = value; }
			}

			[ConditionMapping("COMPLETED_TIME", "<")]
			public DateTime CompletedTimeEnd
			{
				get { return completedTimeEnd; }
				set { completedTimeEnd = value; }
			}

			/// <summary>
			/// 接收人ID
			/// </summary>
			[ConditionMapping("SEND_TO_USER")]
			public string UserID
			{
				get { return this.userID; }
				set { this.userID = value; }
			}
		}
		#endregion

		/// <summary>
		/// 获取或设置当前页的高级查询条件
		/// </summary>
		private TaskQueryCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as TaskQueryCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		private IConnectiveSqlClause Condition
		{
			get;
			set;
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
				//由于枚举类型首项一般为0，ConditionMapping.GetWhereSqlClauseBuilder会把0值当成int的默认值过滤掉
				//所以枚举类型的查询条件应当单独处理
				case "Emergency":
				case "CategoryID":
					ignored = true;
					break;
				case "Purpose":
					data = ((string)data).Trim();
					result = "%" + TSqlBuilder.Instance.EscapeLikeString(((string)data).Trim()) + "%";
					break;
				case "DeliverTimeEnd":
				case "ExpireTimeEnd":
					result = ((DateTime)data).AddDays(1);
					break;
				case "SourceID":
					result = "(" + data + ")";
					break;
				case "DraftDepartmentName":
					ignored = true;
					break;
				case "ApplicationName":
					if (data.ToString() == "全部")
					{
						ignored = true;
					}
					break;
				case "TaskTitle":
					break;
			}

			return result;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetNoStore();

			if (this.Page.IsPostBack == false)
			{
				this.search1.UserCustomSearchConditions = Util.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new TaskQueryCondition();

				if (Request.QueryString.GetValue("mode", TaskStatus.Ban).Equals(TaskStatus.Ban))
				{
					UserSettings userSettings = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID);
					gridViewTask.PageSize = userSettings.GetPropertyValue("CommonSettings", "ToDoListPageSize", this.gridViewTask.PageSize);
				}
			}

			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected void RefreshList(object sender, EventArgs e)
		{
			this.InnerRefreshList();
		}

		private void InnerRefreshList()
		{
			// 重新刷新列表
			this.src1.LastQueryRowCount = -1;
			this.gridViewTask.SelectedKeys.Clear();
			this.Page.PreRender += new EventHandler(this.DelayRefreshList);
		}

		private void DelayRefreshList(object sender, EventArgs e)
		{
			this.gridViewTask.DataBind();
		}

		protected void SearchButtonClick(object sender, SearchEventArgs e)
		{
			this.gridViewTask.PageIndex = 0;
			this.searchBinding.CollectData();

			Util.SaveSearchCondition(e, this.search1, ThisPageSearchResourceKey, this.searchBinding.Data);
			this.InnerRefreshList();
		}

		protected void GridViewTask_ExportData(object sender, EventArgs e)
		{
			this.InnerRefreshList();
		}

		protected void ObjectDataSourceTask_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			if (this.Condition == null)
			{
				var condition = this.CurrentAdvancedSearchCondition;

				WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition, new AdjustConditionValueDelegate(AdjustQueryConditionValue));

				this.Condition = new ConnectiveSqlClauseCollection(builder, this.GetCommonCondition(), this.search1.GetCondition());
			}

			this.src1.Condition = this.Condition;
		}

		private IConnectiveSqlClause GetCommonCondition()
		{
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
			builder.AppendItem("SEND_TO_USER", DeluxeIdentity.CurrentUser.ID);

			WhereSqlClauseBuilder wBuilder1 = new WhereSqlClauseBuilder();
			wBuilder1.AppendItem("EXPIRE_TIME", "null", "is", true);
			WhereSqlClauseBuilder wBuilder2 = new WhereSqlClauseBuilder();
			wBuilder2.AppendItem("EXPIRE_TIME", "not null", "is", true);
			wBuilder2.AppendItem("EXPIRE_TIME", "getdate()", ">", true);
			ConnectiveSqlClauseCollection collection = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or);
			collection.Add(wBuilder1);
			collection.Add(wBuilder2);

			WhereSqlClauseBuilder wBuilder3 = new WhereSqlClauseBuilder();
			wBuilder3.AppendItem("STATUS", TaskStatus.Yue.ToString("d"));

			ConnectiveSqlClauseCollection collection1 = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And);
			collection1.Add(wBuilder3);
			collection1.Add(collection);

			ConnectiveSqlClauseCollection collection2 = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or);
			WhereSqlClauseBuilder wBuilder4 = new WhereSqlClauseBuilder();
			wBuilder4.AppendItem("STATUS", TaskStatus.Ban.ToString("d"));
			collection2.Add(collection1);
			collection2.Add(wBuilder4);

			return new ConnectiveSqlClauseCollection(builder, collection2);
		}

		protected void GridViewTask_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				HyperLink lnk = (HyperLink)e.Row.FindControl("lnkTaskTitle");
				lnk.EnableViewState = false;
				lnk.Text = Server.HtmlEncode((string)DataBinder.Eval(e.Row.DataItem, "TASK_TITLE"));

				if (this.gridViewTask.ExportingDeluxeGrid)
				{
					lnk.NavigateUrl = "";
				}
				else
				{
					lnk.NavigateUrl = "javascript:void(0)";
					lnk.Attributes["taskid"] = (string)DataBinder.Eval(e.Row.DataItem, "TASK_GUID");
					lnk.Attributes["unreadflag"] = object.Equals(DateTime.MinValue, DataBinder.Eval(e.Row.DataItem, "READ_TIME")) ? "True" : "False";
					string appName = (string)DataBinder.Eval(e.Row.DataItem, "APPLICATION_NAME");
					string url = (string)DataBinder.Eval(e.Row.DataItem, "URL");
					TaskLevel level = (TaskLevel)DataBinder.Eval(e.Row.DataItem, "TASK_LEVEL");
					TaskStatus status;
					Enum.TryParse<TaskStatus>(DataBinder.Eval(e.Row.DataItem, "STATUS").ToString(), out status);

					lnk.Attributes["onclick"] = "onTaskLinkClick('" + UserTask.GetNormalizedUrl(url) + "','" + GetFeature(appName, level, status, url) + "');";
				}
			}
		}

		/// <summary>
		/// 获取应用对应的窗口控制代码
		/// </summary>
		/// <param name="task">消息对象</param>
		/// <returns>用于JS的弹出窗口控制代码</returns>
		internal static string GetFeature(string appName, TaskLevel level, TaskStatus status, string url)
		{
			string feature = string.Empty;
			string featureID = string.Empty;
			//判断消息级别，同时消息状态是阅
			//老系统中的url会自动添加 http:// 字符串，新系统中没有
			if ((level < TaskLevel.Normal && status == TaskStatus.Yue && !url.StartsWith("http://"))
				|| url.StartsWith("/MCSWebApp/MCSOAPortal/TaskList/NoticeTaskDetail.aspx"))
			{
				if (ResourceUriSettings.GetConfig().Features.ContainsKey("MessageRemind"))
				{
					feature = ResourceUriSettings.GetConfig().Features["MessageRemind"].Feature.ToWindowFeatureClientString();
				}
				else
				{
					feature = "width=800,height=600,left=' + ((window.screen.width - 800) / 2) + ',top=' + ((window.screen.height - 600) / 2) + ',resizable=yes,scrollbars=yes,toolbar=no,location=no";
				}
			}
			else
			{ //Note:获取js的窗口形态（待办）ApplicationInfoConfig.GetConfig().Applications.ContainsKey(appName) ? ApplicationInfoConfig.GetConfig().Applications[appName].FeatureID : string.Empty;
				feature = "width=800,height=600,left=' + ((window.screen.width - 800) / 2) + ',top=' + ((window.screen.height - 600) / 2) + ',resizable=yes,scrollbars=yes,toolbar=no,location=no";
			}
			return feature;
		}

		//执行修改
		protected void multiProcess_ExecuteStep(object data)
		{
			throw new NotImplementedException("Demo页面不提供此功能");
			//ReplaceAssigneeHelperData rahd = JSONSerializerExecute.Deserialize<ReplaceAssigneeHelperData>(data);
			//ReplaceAssigneeHelper.ExecuteReplace(rahd);
		}
	}
}