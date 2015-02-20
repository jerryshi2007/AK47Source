using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Web.Library;
using MCS.Library.Principal;
using MCS.Library.Core;

namespace AUCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AUScopesConst : System.Web.UI.Page, INormalSceneDescriptor
	{
		#region 高级查询

		private string ThisPageSearchResourceKey = "6858D7BE-F86D-41D8-AB53-C7931226E994";

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

		public void AfterNormalSceneApplied()
		{
			this.btnAddScope.Enabled = this.lnkAdd.Enabled = this.lnkDelete.Enabled = this.EditEnabled;
		}

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
			get
			{
				return this.ViewState["AUSchemaObject"] as SCSimpleObject;
			}

			set
			{
				this.ViewState["AUSchemaObject"] = value;
			}
		}

		public bool EditEnabled
		{
			get
			{
				var obj = this.ViewState["EditEnabled"];
				if (obj != null)
					return (bool)obj;

				return false;
			}

			set
			{
				this.ViewState["EditEnabled"] = value;
			}
		}

		protected string[] Scopes
		{
			get
			{
				return this.ViewState["Scopes"] as string[];
			}

			set
			{
				this.ViewState["Scopes"] = value;
			}
		}

		protected string CurrentScopeType
		{
			get
			{
				return this.ViewState["CurrentScopeType"] as string;
			}

			set
			{
				this.ViewState["CurrentScopeType"] = value;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			string unitID = Request.QueryString["unitId"];
			string scopeType = Request.QueryString["schemaType"];

			if (string.IsNullOrEmpty(unitID))
				throw new HttpException("没有提供unitId查询参数");

			if (scopeType == null)
				throw new HttpException("没有提供schemaType参数");

			if (Page.IsPostBack == false)
			{
				this.CurrentScopeType = scopeType;
				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				var unit = DbUtil.GetEffectiveObject<AU.AdminUnit>(unitID);
				this.AdminUnitObject = unit.ToSimpleObject();

				var schema = unit.GetUnitSchema();
				this.AUSchemaObject = schema.ToSimpleObject();
				this.DeterminPermission(schema, unit);

				this.Scopes = schema.Scopes.Split(',');
			}

			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		private void DeterminPermission(AUSchema schema, AdminUnit unit)
		{
			bool editEnabled = TimePointContext.Current.UseCurrentTime;
			if (editEnabled)
			{
				if (Util.SuperVisiorMode == false)
				{
					if (string.IsNullOrEmpty(schema.MasterRole) == false)
					{
						editEnabled = DeluxePrincipal.Current.IsInRole(schema.MasterRole);

						if (editEnabled == false)
						{
							var permissions = AU.Adapters.AUAclAdapter.Instance.LoadCurrentContainerAndPermissions(DeluxeIdentity.CurrentUser.ID, new string[] { unit.ID });

							editEnabled = Util.ContainsPermission(permissions, unit.ID, "EditAdminScope"); ;
						}
					}
				}
			}

			this.EditEnabled = editEnabled;
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
			string paraPattern = "schemaType=" + this.CurrentScopeType + "&unitId=" + this.AdminUnitObject.ID;
			this.lnkToCondition.NavigateUrl = "AUScopesCondition.aspx?" + paraPattern;
			this.lnkToConst.NavigateUrl = "AUScopesConst.aspx?" + paraPattern;
			this.lnkToPreview.NavigateUrl = "AUScopes.aspx?" + paraPattern;
			this.scopeType.Value = this.CurrentScopeType;
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

		protected void AddScopesClick(object sender, EventArgs e)
		{
			var keys = this.postData.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (keys.Length > 0)
			{
				try
				{
					var unit = DbUtil.GetEffectiveObject<AU.AdminUnit>(this.AdminUnitObject.ID);

					var scope = unit.GetNormalScopes().GetScope(this.CurrentScopeType);

					if (scope == null)
						throw new ObjectNotFoundException("指定的管理范围已不存在。");

					SchemaObjectCollection objs = DbUtil.GetEffectiveObjects(keys);
					foreach (AU.AUAdminScopeItem item in objs)
					{
						AU.Operations.Facade.InstanceWithPermissions.AddObjectToScope(item, scope);
					}
				}
				catch (Exception ex)
				{
					WebUtility.ShowClientError(ex);
				}

				this.InnerRefreshList();
			}
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

		protected void DoDelete(object sender, EventArgs e)
		{
			if (this.gridMain.SelectedKeys.Count > 0)
			{
				string unitID = Request.QueryString["unitId"];
				string scopeType = Request.QueryString["schemaType"];

				try
				{
					AU.AUAdminScope scope = (AU.AUAdminScope)AU.Adapters.AUSnapshotAdapter.Instance.LoadAUScope(unitID, scopeType, true, DateTime.MinValue).FirstOrDefault();
					if (scope == null || scope.Status != SchemaObjectStatus.Normal)
						throw new AUObjectException("指定的管理范围不存在");

					var items = AU.Adapters.AUSnapshotAdapter.Instance.LoadScopeItems(this.gridMain.SelectedKeys.ToArray(), scopeType, true, DateTime.MinValue);

					foreach (var item in items)
					{
						AU.Operations.Facade.InstanceWithPermissions.RemoveObjectFromScope((AU.AUAdminScopeItem)item, scope);
					}
				}
				catch (Exception ex)
				{
					WebUtility.ShowClientError(ex);
				}
			}

			this.InnerRefreshList();
		}
	}
}