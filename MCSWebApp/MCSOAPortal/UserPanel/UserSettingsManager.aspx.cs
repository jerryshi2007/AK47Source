using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;
using MCS.Library.Validation;

namespace MCS.OA.Portal.UserPanel
{
	public partial class UserSettingsManager : System.Web.UI.Page
	{
		protected const string TreeRootText = "用户设置";

		protected override void OnPreRender(EventArgs e)
		{
			Response.Cache.SetNoStore();
			JSONSerializerExecute.RegisterConverter(typeof(PropertyValueConverter));
			JSONSerializerExecute.RegisterConverter(typeof(UserSettingsConverter));
			JSONSerializerExecute.RegisterConverter(typeof(UserSettingsCategoryConverter));

			txtDataSource.Attributes.Add("style", "display:none;");
			btnSubmit.Attributes.Add("onclick", "updateHiddenText();");

			tree.Nodes.Clear();
			tree.OnNodeSelecting = "nodeClick";

			//根据当前登录的用户取得UserSettings的信息
			UserSettings objUserSettings = UserSettings.LoadSettings(DeluxeIdentity.CurrentUser.ID);

			RegisterEnumTypes(objUserSettings);

			DeluxeTreeNode rootNode = new DeluxeTreeNode(TreeRootText, TreeRootText)
										  {
											  NodeOpenImg = "../images/computer.gif",
											  NodeCloseImg = "../images/computer.gif",
											  Expanded = true
										  };

			DeluxeTreeNode node;
			string fsNodeText = string.Empty;
			string fsNodeValue = string.Empty;
			//循环添加二级结点
			for (int i = 0; i < objUserSettings.Categories.Count; i++)
			{
				fsNodeText = objUserSettings.Categories[i].Description;
				fsNodeValue = objUserSettings.Categories[i].Name;
				node = new DeluxeTreeNode(fsNodeText, fsNodeValue)
						   {
							   NodeOpenImg = "../images/accomplished.gif",
							   NodeCloseImg = "../images/accomplished.gif",
							   ExtendedData = objUserSettings.Categories[i].Description
						   };
				rootNode.Nodes.Add(node);
			}

			tree.Nodes.Add(rootNode);
			if (!IsPostBack)
			{
				if (objUserSettings.Categories.Count != 0)
				{
					txtDataSource.Text = JSONSerializerExecute.Serialize(objUserSettings);
				}
			}

			base.OnPreRender(e);
		}

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetNoStore();
            RegisterPropertyEditor();

            if (this.IsCallback == false)
            {
                CountrycodeCollection collection = CountrycodeAdapter.Instance.LoadAllFromCache();

                List<EnumItemPropertyDescription> result = new List<EnumItemPropertyDescription>();

                foreach (Countrycode item in collection)
                {
                    result.Add(new EnumItemPropertyDescription(item.Code, item.CnName));
                }

                this.hiddenSource.Value = JSONSerializerExecute.Serialize(result);
            }
        }

	    protected void BtnUpdateUserSetting(object sender, EventArgs e)
		{
			try
			{
				UserSettings objUserSettings = (UserSettings)JSONSerializerExecute.DeserializeObject(txtDataSource.Text, typeof(UserSettings));
				objUserSettings.Update();
				this.ClientScript.RegisterClientScriptBlock(this.GetType(), "RefreshParent", "<script type = 'text/javascript'>window.returnValue = 'reload';</script>");
				WebUtility.CloseWindow();
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
		}

		private void RegisterEnumTypes(UserSettings objUserSettings)
		{
			foreach (UserSettingsCategory category in objUserSettings.Categories)
			{
				foreach (PropertyValue pv in category.Properties)
				{
					if (pv.Definition.DataType == PropertyDataType.Enum && pv.Definition.EditorParams.IsNotEmpty())
						this.PropertyGrid1.PredefinedEnumTypes.Add(pv.Definition.EditorParams);

					#region “由于没有给PropertyGrid 数据源，没有加载对应的客户端验证” ydz
					foreach (PropertyValidatorDescriptor propValidator in pv.Definition.Validators)
					{
						Validator vali = propValidator.GetValidator();
						if (vali is IClientValidatable)
						{
							ClientVdtAttribute cvArt = new ClientVdtAttribute(propValidator);
							if (string.IsNullOrEmpty(cvArt.ClientValidateMethodName) == false)
							{
								this.Page.ClientScript.RegisterStartupScript(this.GetType(), cvArt.ClientValidateMethodName, ((IClientValidatable)vali).GetClientValidateScript(), true);
							}
						}
					}
					#endregion
				}
			}
		}

        private void RegisterPropertyEditor()
        {
            PropertyEditorHelper.RegisterEditor(new SignaturePropertyEditor());
        }
	}
}