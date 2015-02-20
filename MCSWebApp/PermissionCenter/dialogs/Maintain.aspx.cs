using System;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace PermissionCenter.Dialogs
{
	public partial class Maintain : System.Web.UI.Page
	{
		#region 受保护的方法

		public override void ProcessRequest(System.Web.HttpContext context)
		{
			if (Util.SuperVisiorMode == false)
			{
				context.Response.Redirect("~/dialogs/Profile.aspx");
			}
			else
			{
				if (context.Request.QueryString["action"] == "genSnapshot")
				{
					context.Server.ScriptTimeout = 60 * 60;
					SCSnapshotBasicAdapter.Instance.GenerateAllSchemaSnapshot();
					context.Response.ContentType = "image/gif";
					context.Response.WriteFile("~/images/ajax-loader2.gif");
				}
				else
				{
					base.ProcessRequest(context);
				}
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Page.IsPostBack == false)
			{
				this.timeToDeleteAfter.Text = DateTime.Now.ToString();

			}

			this.testPan.Visible = Request.QueryString["test"] == "on";
			this.factoryPan.Visible = Request.QueryString["factory"] == "on";
		}

		protected void ClearDataClick(object sender, EventArgs e)
		{
			PC.Adapters.SchemaObjectAdapter.Instance.ClearAllData();
		}

		protected void StartAdSync(object sender, EventArgs e)
		{
		}

		#endregion

		protected void GenRoots(object sender, EventArgs e)
		{
			var root = SCOrganization.GetRoot();
			for (int i = 0; i < 20; i++)
			{
				SCOrganization org = new SCOrganization();
				org.ID = UuidHelper.NewUuidString();
				org.Name = org.ID;
				org.DisplayName = org.ID;
				org.CreateDate = DateTime.MinValue;
				org.CodeName = org.ID;
				PC.Executors.SCObjectOperations.InstanceWithPermissions.AddOrganization(org, root);
			}
		}

		protected void btnGenInitData_Click(object sender, EventArgs e)
		{
			SCApplication app = new SCApplication();
			app.ID = "68DB2697-59B2-414B-8591-58CE06C4B44F";
			app.Name = "权限中心";
			app.CodeName = "OGU_ADMIN";
			app.DisplayName = "权限中心";

			PC.Executors.SCObjectOperations.Instance.AddApplication(app);

			SCRole role = new SCRole();
			role.ID = "6BEA73AB-0924-483B-BEE0-55C0847CFDAB";
			role.DisplayName = role.Name = "权限中心总管";
			role.CodeName = "SYSTEM_ADMINISTRATOR";

			PC.Executors.SCObjectOperations.Instance.AddRole(role, app);
		}

		protected void ResetPass(object sender, EventArgs e)
		{
			var users = PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeNameAndSchema(SchemaInfo.FilterByCategory("Users").ToSchemaNames(), new string[] { this.userCodeName.Text }, true, false, DateTime.MinValue).FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			if (users.Count > 0)
			{
				var user = (PC.SCUser)users[0];
				PC.Adapters.UserPasswordAdapter.Instance.SetPassword(user.ID, PC.Adapters.UserPasswordAdapter.GetPasswordType(), PC.Adapters.UserPasswordAdapter.GetDefaultPassword());
				this.prompt.InnerText = user.DisplayName + "(" + user.CodeName + ")" + "已设置密码";
			}
			else
			{
				this.prompt.InnerText = "未找到指定的人";
			}
		}

		protected void DeleteAfterClick(object sender, EventArgs e)
		{
			DateTime dt;
			if (DateTime.TryParse(this.timeToDeleteAfter.Text, out dt))
			{
				try
				{
					PC.Adapters.SchemaObjectAdapter.Instance.HistoryMoveBack(dt);
					this.prompt2.InnerText = "完成";
				}
				catch (Exception ex)
				{
					this.prompt2.InnerText = ex.Message;
				}
			}
			else
			{
				this.prompt2.InnerText = "日期格式错误";
			}
		}

		protected void GenSnapshot(object sender, EventArgs e)
		{
			SCSnapshotBasicAdapter.Instance.GenerateAllSchemaSnapshot();
		}

		protected void GenSchemaTable(object sender, EventArgs e)
		{
			SchemaDefineCollection schemas = SchemaExtensions.CreateSchemasDefineFromConfiguration();

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				schemas.ForEach(schema => SchemaDefineAdapter.Instance.Update(schema));
				scope.Complete();
			}
		}

		protected void DetectConflict(object sender, EventArgs e)
		{
			var dataSet = PC.Adapters.SCSnapshotHelper.Instance.DetectConflict();
			this.viewList1.DataSource = dataSet.Tables[0];
			this.viewList1.DataBind();

			this.viewList2.DataSource = dataSet.Tables[1];
			this.viewList2.DataBind();

			this.viewList3.DataSource = dataSet.Tables[2];
			this.viewList3.DataBind();

			this.viewList4.DataSource = dataSet.Tables[3];
			this.viewList4.DataBind();

			this.viewList5.DataSource = dataSet.Tables[4];
			this.viewList5.DataBind();

			this.detectResult.Visible = true;
		}

		protected void CleanSecretary(object sender, EventArgs e)
		{
			PC.Adapters.SCSnapshotHelper.Instance.ClearDbErrors();
		}
	}
}