using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Core;
using MCS.Web.Library;

namespace AUCenter.Dialogs
{
	public partial class PropertyEditor : System.Web.UI.Page
	{
		private AU.AUSchema schema;

		public bool EditEnabled
		{
			get
			{
				return Util.SuperVisiorMode && TimePointContext.Current.UseCurrentTime;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string auSchemaID = Request.QueryString["auSchemaID"];
			if (string.IsNullOrEmpty(auSchemaID))
				throw new HttpException("必须提供有效的auSchemaID参数");

			this.schema = GetAuSchema(auSchemaID);
		}

		private static AU.AUSchema GetAuSchema(string auSchemaID)
		{
			AU.AUSchema schema = null;
			AU.AUCommon.DoDbAction(() =>
			{
				schema = (AU.AUSchema)PC.Adapters.SchemaObjectAdapter.Instance.Load(auSchemaID);
			});

			if (schema == null || schema.Status != MCS.Library.SOA.DataObjects.Schemas.SchemaProperties.SchemaObjectStatus.Normal)
				throw new PC.ObjectNotFoundException("未找到指定的管理架构，此架构可能已经被删除。");

			return schema;
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			var defin = AU.Adapters.SchemaPropertyExtensionAdapter.Instance.Load("AdminUnits", this.schema.ID) ?? new AU.SchemaPropertyExtension("AdminUnits", this.schema.ID, "管理单元属性扩展定义");

			var schemaDefine = MCS.Library.SOA.DataObjects.Security.SchemaDefine.GetSchema("AdminUnits");
			var ppts = schemaDefine.Properties;
			string[] names = new string[ppts.Count];
			for (int i = ppts.Count - 1; i >= 0; i--)
				names[i] = "'" + ppts[i].Name + "'";

			var scripts = "var reservedPropertiesNames = [" + string.Join(",", names) + "];\r\n";

			names = new string[schemaDefine.Tabs.Count];
			for (int i = 0; i < names.Length; i++)
			{
				names[i] = "'" + schemaDefine.Tabs[i].Name + "'";
			}

			scripts += "var tabNames = [" + string.Join(",", names) + "];\r\n";

			scripts += "var initProperties = " + MCS.Web.Library.Script.JSONSerializerExecute.Serialize(defin.Properties) + ";";

			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "initProperties", scripts, true);
			this.txtDescription.Value = defin.Description;
			this.lblTitle.InnerText = this.schema.GetQualifiedName();
			this.panAdd.Visible = this.btnSave.Visible = this.btnSubmit.Enabled = this.EditEnabled;
			this.txtDescription.Disabled = this.panAdd.Visible == false;
		}

		protected void TestClick(object sender, EventArgs e)
		{
			string auSchemaID = Request.QueryString["auSchemaID"];
			var ext = new AU.SchemaPropertyExtension("AdminUnits", auSchemaID, "Demo");
			var ppt = new MCS.Library.SOA.DataObjects.Schemas.SchemaProperties.SchemaPropertyDefine()
			{
				Name = "Test",
				DataType = MCS.Library.SOA.DataObjects.PropertyDataType.String,
				DefaultValue = "222",
				MaxLength = 12,
				Category = "ABC",
				Description = "描述",
				DisplayName = "测试属性",
				IsRequired = true,
				ShowTitle = true,
				Visible = true,
				Tab = "Basic",
				ReadOnly = false,
			};

			ppt.Validators.Add(new MCS.Library.SOA.DataObjects.PropertyValidatorDescriptor()
			{
				MessageTemplate = "属性值有误",
				Name = "DefaultValidator",
				Tag = "bb",
				TypeDescription = "aaa"
			});

			ppt.Validators[0].Parameters.Add(new MCS.Library.SOA.DataObjects.PropertyValidatorParameterDescriptor()
			{
				DataType = MCS.Library.SOA.DataObjects.PropertyDataType.String,
				ParamName = "abc",
				ParamValue = "haha"
			});

			ext.Properties.Add(ppt);

			AU.Adapters.SchemaPropertyExtensionAdapter.Instance.Update(ext);
		}

		protected void DoPost(object sender, EventArgs e)
		{
			Util.EnsureOperationSafe();

			if (this.EditEnabled)
			{
				string json = hfPostData.Value;
				var obj = MCS.Web.Library.Script.JSONSerializerExecute.Deserialize<SchemaPropertyDefineCollection>(json);
				var ext = new AU.SchemaPropertyExtension("AdminUnits", schema.ID, txtDescription.Value);
				ext.Properties.CopyFrom(obj);
				AU.Adapters.SchemaPropertyExtensionAdapter.Instance.Update(ext);
				WebUtility.ResponseCloseWindowScriptBlock();
			}
			else
			{
				WebUtility.ResponseShowClientErrorScriptBlock("没有操作权限", "此操作只能由管理员操作", "操作权限不足");
			}
		}
	}
}