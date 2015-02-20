using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Security;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Actions;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.Principal;
using MCS.Library.Expression;

namespace AUCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AUScopesCondition : System.Web.UI.Page, INormalSceneDescriptor
	{
		protected SCSimpleObject AdminUnitObject
		{
			get { return this.ViewState["AdminUnitObject"] as SCSimpleObject; }

			set { this.ViewState["AdminUnitObject"] = value; }
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
			get { return this.ViewState["SCSimpleObject"] as SCSimpleObject; }

			set { this.ViewState["SCSimpleObject"] = value; }
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

		public void AfterNormalSceneApplied()
		{
			this.txtDesc.Enabled = this.expression.Enabled = this.btnRecalc.Enabled = this.btnSave.Enabled = this.chkAutoCalc.Enabled = this.calcProgress.Enabled = this.EditEnabled;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			string unitID = Request.QueryString["unitId"];

			if (string.IsNullOrEmpty(unitID))
				throw new HttpException("没有提供unitId查询参数");

			if (Page.IsPostBack == false)
			{
				var unit = DbUtil.GetEffectiveObject<AU.AdminUnit>(unitID);
				this.AdminUnitObject = unit.ToSimpleObject();

				var schema = unit.GetUnitSchema();
				this.AUSchemaObject = schema.ToSimpleObject();

				this.Scopes = schema.Scopes.Split(',');

				var scopeItem = unit.GetNormalScopes().GetScope(Request.QueryString["schemaType"]);

				if (scopeItem == null)
					throw new HttpException("schemaType参数指定的管理范围不存在或已删除");
				this.CurrentScope = scopeItem.ToSimpleObject();

				this.DeterminPermission(schema, unit);

				var condition = AU.Adapters.AUConditionAdapter.Instance.Load(scopeItem.ID, AU.AUCommon.ConditionType).Find(m => m.Status == MCS.Library.SOA.DataObjects.Schemas.SchemaProperties.SchemaObjectStatus.Normal);

				if (condition != null && condition.Status == MCS.Library.SOA.DataObjects.Schemas.SchemaProperties.SchemaObjectStatus.Normal)
				{
					this.expression.Text = condition.Condition;
					this.txtDesc.Text = condition.Description;
				}
			}

			this.calcProgress.Tag = this.CurrentScope.ID;
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
			string schemaType = this.Request["schemaType"];
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			this.scopeRepeater.DataSource = this.Scopes;
			this.scopeRepeater.DataBind();
			this.schemaLabel.InnerText = this.AUSchemaObject.DisplayName;
			this.schemUnitLabel.InnerText = this.AdminUnitObject.DisplayName;
			string paraPattern = "schemaType=" + schemaType + "&unitId=" + this.AdminUnitObject.ID;
			this.lnkToCondition.NavigateUrl = "AUScopesCondition.aspx?" + paraPattern;
			this.lnkToConst.NavigateUrl = "AUScopesConst.aspx?" + paraPattern;
			this.lnkToPreview.NavigateUrl = "AUScopes.aspx?" + paraPattern;
			this.lblSchemaName.Text = schemaType;
			this.propRepeater.DataSource = SchemaDefine.GetSchema(schemaType).Properties;
			this.propRepeater.DataBind();
		}

		protected string GetSchemaName(string key)
		{
			return SchemaDefine.GetSchemaConfig(key).Description;
		}

		protected void ProcessCaculating(object sender, MCS.Web.WebControls.PostProgressDoPostedDataEventArgs e)
		{
			var scope = DbUtil.GetEffectiveObject<AU.AUAdminScope>(this.calcProgress.Tag);
			AUConditionCalculator calc = new AUConditionCalculator(scope.ScopeSchemaType);
			AU.AUCommon.DoDbAction(() =>
			{
				calc.GenerateItemAndContainerSnapshot(new[] { scope });
			});

			SCCacheHelper.InvalidateAllCache();

			e.Result.DataChanged = true;
			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		protected void Nop(object sender, EventArgs e)
		{

		}

		protected void SaveClick(object sender, EventArgs e)
		{
			try
			{
				var conditions = AU.Adapters.AUConditionAdapter.Instance.Load(this.CurrentScope.ID, AU.AUCommon.ConditionType);
				SCConditionCollection newCondition = new SCConditionCollection();
				newCondition.Add(new SCCondition(this.expression.Text) { Description = this.txtDesc.Text, Type = AU.AUCommon.ConditionType });
				conditions.ReplaceItemsWith(newCondition, this.CurrentScope.ID, AU.AUCommon.ConditionType);
				AU.Adapters.AUConditionAdapter.Instance.UpdateConditions(this.CurrentScope.ID, AU.AUCommon.ConditionType, conditions);
				this.prompt.InnerText = "保存成功";

				if (this.chkAutoCalc.Checked)
				{
					this.postScript.Text = Util.SurroundScriptBlock("Sys.Application.add_init(function(){ document.getElementById('btnRecalc').click();});");
				}
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex);
			}
		}
	}
}