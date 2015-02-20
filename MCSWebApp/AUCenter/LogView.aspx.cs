using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace AUCenter
{
	public partial class LogView : System.Web.UI.Page
	{
		public static readonly string ThisPageSearchResourceKey = "F1417696-9F44-4856-840C-2394A699FE5E";

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("OperatorName")]
			public string OperatorName { get; set; }

			[ConditionMapping("RealOperatorName")]
			public string RealOperatorName { get; set; }

			[NoMapping]
			public DateTime AfterForBind { get; set; }

			[NoMapping]
			public DateTime BeforeForBind { get; set; }

			[ConditionMapping("CreateTime", Operation = ">=")]
			public DateTime? After
			{
				get
				{
					if (this.AfterForBind == default(DateTime))
					{
						return null;
					}
					else
					{
						return this.AfterForBind;
					}
				}

				set
				{
					if (value.HasValue && value.Value != default(DateTime))
					{
						this.AfterForBind = value.Value;
					}
					else
					{
						this.AfterForBind = default(DateTime);
					}
				}
			}

			[ConditionMapping("CreateTime", Operation = "<=")]
			public DateTime? Before
			{
				get
				{
					if (this.BeforeForBind == default(DateTime))
					{
						return null;
					}
					else
					{
						return this.BeforeForBind;
					}
				}

				set
				{
					if (value.HasValue && value.Value != default(DateTime))
					{
						this.BeforeForBind = value.Value;
					}
					else
					{
						this.BeforeForBind = default(DateTime);
					}
				}
			}
		}

		//protected bool AdvanceSearchEnabled
		//{
		//    get
		//    {
		//        object o = this.ViewState["PageAdvanceSearch"];
		//        return (o is bool) ? (bool)o : false;
		//    }

		//    set
		//    {
		//        this.ViewState["PageAdvanceSearch"] = value;
		//    }
		//}

		private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			this.Page.Response.CacheControl = "no-cache";

			if (this.IsPostBack == false)
			{
				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();
				if (string.IsNullOrEmpty(this.gridMain.SortExpression))
				{
					this.gridMain.Sort("CreateTime", SortDirection.Descending);
				}

				//this.gridMain.PageSize = ProfileUtil.PageSize;
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
			this.dataSourceMain.LastQueryRowCount = -1;
			this.gridMain.SelectedKeys.Clear();
			this.Page.PreRender += new EventHandler(this.DelayRefreshList);
		}

		private void DelayRefreshList(object sender, EventArgs e)
		{
			this.gridMain.DataBind();
		}

		protected void SearchButtonClick(object sender, MCS.Web.WebControls.SearchEventArgs e)
		{
			this.gridMain.PageIndex = 0;

			this.searchBinding.CollectData();

			Util.SaveSearchCondition(e, this.DeluxeSearch, ThisPageSearchResourceKey, this.searchBinding.Data);

			this.InnerRefreshList();
		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			//if (this.AdvanceSearchEnabled)
			{
				var condition = this.CurrentAdvancedSearchCondition;

				WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

				this.dataSourceMain.Condition = new ConnectiveSqlClauseCollection(builder, this.DeluxeSearch.GetCondition());
			}
			//else
			//{
			//    this.dataSourceMain.Condition = this.DeluxeSearch.GetCondition();
			//}

			string cate = Page.Request["catelog"];
			string opType = null;
			if (cate != null)
			{
				var num = cate.IndexOf(".");
				if (num > 0)
				{
					string[] arrString = cate.Split('.');
					cate = arrString[0];
					opType = arrString[1];
				}
			}

			e.InputParameters["catelog"] = cate;
			e.InputParameters["operationType"] = opType;
		}

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "Shuttle":
					this.ShuttleToTime(DateTime.Parse(e.CommandArgument.ToString()));
					break;
				default:
					break;
			}
		}

		private void ShuttleToTime(DateTime dateTime)
		{
			AUCenter.WebControls.Banner.ChangeAndSaveTimePoint(dateTime);
			Response.Write(Util.SurroundScriptBlock("window.top.location = window.top.location;"));
			Response.End();
		}
	}
}