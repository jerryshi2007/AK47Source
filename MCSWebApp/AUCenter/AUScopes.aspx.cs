using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Actions;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.Core;

namespace AUCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AUScopes : System.Web.UI.Page
	{
		#region 高级查询

		private string ThisPageSearchResourceKey = "AE5744E2-7E01-47C1-BE93-17482E0160DE";

		private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("S.CodeName")]
			public string CodeName { get; set; }
		}

		#endregion

		protected SCSimpleObject AdminUnitObject
		{
			get
			{
				return this.ViewState["AdminUnitObject"] as SCSimpleObject;
			}

			set
			{
				this.ViewState["AdminUnitObject"] = value;
			}
		}

		public SCSimpleObject AUSchemaObject
		{
			get { return this.ViewState["AUSchemaObject"] as SCSimpleObject; }

			set { this.ViewState["AUSchemaObject"] = value; }
		}

		protected string[] Scopes
		{
			get { return this.ViewState["Scopes"] as string[]; }

			set { this.ViewState["Scopes"] = value; }
		}

		protected SCSimpleObject CurrentScope
		{
			get { return this.ViewState["CurrentScope"] as SCSimpleObject; }

			set { this.ViewState["CurrentScope"] = value; }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			string unitID = Request.QueryString["unitId"];

			if (string.IsNullOrEmpty(unitID))
				throw new HttpException("没有提供unitId查询参数");

			if (Page.IsPostBack == false)
			{
				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				var unit = DbUtil.GetEffectiveObject<AU.AdminUnit>(unitID);
				this.AdminUnitObject = unit.ToSimpleObject();

				var schema = unit.GetUnitSchema();
				this.AUSchemaObject = schema.ToSimpleObject();

				var scope = unit.GetNormalScopes().GetScope(this.Request.QueryString["schemaType"]);
				if (scope == null)
					throw new HttpException("schemaType参数指定的管理范围不存在或已删除");

				this.CurrentScope = scope.ToSimpleObject();

				this.Scopes = schema.Scopes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			}

			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
			this.calcProgress.Tag = this.CurrentScope.ID;
			this.calcProgressAll.Tag = this.Request.QueryString["schemaType"];
		}

		protected string GetMenuCssClass(string schemaType)
		{
			if (schemaType == Request.QueryString["schemaType"])
			{
				return "au-item active";
			}
			else
			{
				return "au-item";
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.scopeRepeater.DataSource = this.Scopes;
			this.scopeRepeater.DataBind();
			this.schemaLabel.InnerText = this.AUSchemaObject.DisplayName;
			this.schemUnitLabel.InnerText = this.AdminUnitObject.DisplayName;
			string paraPattern = "schemaType=" + this.Request["schemaType"] + "&unitId=" + this.AdminUnitObject.ID;
			this.lnkToCondition.NavigateUrl = "AUScopesCondition.aspx?" + paraPattern;
			this.lnkToConst.NavigateUrl = "AUScopesConst.aspx?" + paraPattern;
			this.lnkToPreview.NavigateUrl = "AUScopes.aspx?" + paraPattern;
		}

		protected string GetSchemaName(string key)
		{
			return SchemaDefine.GetSchemaConfig(key).Description;
		}

		protected void SearchButtonClick(object sender, MCS.Web.WebControls.SearchEventArgs e)
		{
			this.gridMain.PageIndex = 0;

			this.searchBinding.CollectData();

			Util.SaveSearchCondition(e, this.DeluxeSearch, ThisPageSearchResourceKey, this.searchBinding.Data);

			this.InnerRefreshList();
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

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			var condition = this.CurrentAdvancedSearchCondition;

			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

			this.dataSourceMain.Condition = new ConnectiveSqlClauseCollection(builder, this.DeluxeSearch.GetCondition());
		}

		protected void ProcessCaculating(object sender, MCS.Web.WebControls.PostProgressDoPostedDataEventArgs e)
		{
			Util.EnsureOperationSafe();

			var scope = DbUtil.GetEffectiveObject<AU.AUAdminScope>(this.calcProgress.Tag);

			AUConditionCalculator calc = new AUConditionCalculator(scope.ScopeSchemaType);

			calc.GenerateItemAndContainerSnapshot(new SchemaObjectBase[] { scope });

			SCCacheHelper.InvalidateAllCache();

			e.Result.DataChanged = true;
			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		protected void ProcessGlobalCaculating(object sender, MCS.Web.WebControls.PostProgressDoPostedDataEventArgs e)
		{
			Util.EnsureOperationSafe();

			AUConditionCalculator calc = new AUConditionCalculator(calcProgressAll.Tag);

			calc.GenerateAllItemAndContainerSnapshot();

			SCCacheHelper.InvalidateAllCache();

			e.Result.DataChanged = true;
			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		protected void drpScopeMode_SelectedIndexChanged(object sender, EventArgs e)
		{
		}
	}
}