using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Collections;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.Passport;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;
using MCS.Library.Globalization;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.WebControls
{
	public abstract class WfActivityDescriptorEditorBase : DialogControlBase<WfActivityDescriptorEditorParams>
	{
		protected HtmlInputText actName = null;
		protected HtmlInputCheckBox allAgreeWhenConsignCheckbox = null;

		public event AutoCompleteExtender.GetDataSourceDelegate GetDataSource;
		public event ValidateInputOuUserHandler ValidateInputOuUser;

		#region Properties

		protected virtual string ActNameClientID
		{
			get
			{
				EnsureChildControls();

				string result = string.Empty;

				if (actName != null)
					result = actName.ClientID;

				return result;
			}
		}

		protected void OnGetDataSource(string sPrefix, int iCount, object context, ref IEnumerable result)
		{
			if (this.GetDataSource != null)
				this.GetDataSource(sPrefix, iCount, context, ref result);
		}

		protected OguDataCollection<IOguObject> OnValidateInputOuUser(string chkString, object context = null)
		{
			OguDataCollection<IOguObject> result = null;

			if (this.ValidateInputOuUser != null)
				result = ValidateInputOuUser(chkString);
			else
				result = new OguDataCollection<IOguObject>();

			return result;
		}

		protected bool IsGetDataSourceEventHooked
		{
			get
			{
				return this.GetDataSource != null;
			}
		}

		protected bool IsValidateInputOuUserEventHooked
		{
			get
			{
				return this.ValidateInputOuUser != null;
			}
		}

		protected virtual string AllAgreeWhenConsignCheckboxClientID
		{
			get
			{
				EnsureChildControls();

				string result = string.Empty;

				if (allAgreeWhenConsignCheckbox != null)
					result = allAgreeWhenConsignCheckbox.ClientID;

				return result;
			}
		}

		/// <summary>
		/// 流程的ID
		/// </summary>
		public string ProcessID
		{
			get
			{
				return ControlParams.ProcessID;
			}
			set
			{
				ControlParams.ProcessID = value;
			}
		}

		/// <summary>
		/// 资源ID
		/// </summary>
		public string ResourceID
		{
			get
			{
				return ControlParams.ResourceID;
			}
			set
			{
				ControlParams.ResourceID = value;
			}
		}

		/// <summary>
		/// 是否显示传阅的用户
		/// </summary>
		public bool ShowCirculateUsers
		{
			get
			{
				return ControlParams.ShowCirculateUsers;
			}
			set
			{
				ControlParams.ShowCirculateUsers = value;
			}
		}

		[Browsable(true)]
		[Description("是否显示机构人员树选择控件")]
		[DefaultValue(true)]
		public bool ShowTreeSelector
		{
			get
			{
				return GetPropertyValue("ShowTreeSelector", true);
			}
			set
			{
				SetPropertyValue("ShowTreeSelector", value);
			}
		}

		/// <summary>
		/// 会签时,是否需要所有人都通过
		/// </summary>
		public bool AllAgreeWhenConsign
		{
			get
			{
				return ControlParams.AllAgreeWhenConsign;
			}
			set
			{
				ControlParams.AllAgreeWhenConsign = value;
			}
		}

		/// <summary>
		/// 是否允许多个用户
		/// </summary>
		protected bool AllowMultiUsers
		{
			get
			{
				bool result = true;

				if (MaximizeAssigneeCount == 1)
				{
					result = false;
				}
				else
				{
					if (CurrentActivityDescriptor != null)
						result = CurrentActivityDescriptor.Properties.GetValue("AllowAssignToMultiUsers", true);
				}

				return result;
			}
		}

		/// <summary>
		/// 最大的指派人的个数
		/// </summary>
		protected virtual int MaximizeAssigneeCount
		{
			get
			{
				int result = -1;

				if (WfClientContext.Current.InAdminMode == false)
				{
					if (CurrentActivityDescriptor != null)
					{
						result = CurrentActivityDescriptor.Properties.GetValue("MaximizeAssigneeCount", -1);

						if (result == -1)
							result = CurrentActivityDescriptor.Process.Properties.GetValue("MaximizeAssigneeCount", -1);

						if (result == -1)
							result = CurrentActivityDescriptor.Process.Properties.GetValue("MaximizeAssigneeCount", -1);
					}

					if (result == -1)
					{
						if (CurrentActivityDescriptor != null)
							result = WfGlobalParameters.GetValueRecursively(
								CurrentActivityDescriptor.Process.ApplicationName,
								CurrentActivityDescriptor.Process.ProgramName,
								"MaximizeAssigneeCount", -1);
						else
							result = WfGlobalParameters.Default.Properties.GetValue("MaximizeAssigneeCount", -1);
					}
				}

				return result;
			}
		}

		protected virtual string Operation
		{
			get
			{
				return ControlParams.Operation;
			}
		}

		protected virtual string CurrentActivityKey
		{
			get
			{
				return ControlParams.CurrentActivityKey;
			}
		}

		[Browsable(false)]
		protected IWfActivityDescriptor CurrentActivityDescriptor
		{
			get
			{
				IWfActivityDescriptor result = null;

				if (WfClientContext.Current.OriginalActivity != null)
				{
					if (string.IsNullOrEmpty(CurrentActivityKey) == false)
						result = WfClientContext.Current.OriginalActivity.ApprovalRootActivity.Descriptor.Process.Activities[CurrentActivityKey];
				}

				return result;
			}
		}

		#endregion Properties

		#region Protected
		/// <summary>
		/// 根据resourceID和processID生成弹出对话框的url地址
		/// </summary>
		/// <returns></returns>
		protected override string GetDialogUrl()
		{
			PageRenderMode pageRenderMode = new PageRenderMode(this.UniqueID, "DialogControl");

			string url = WebUtility.GetRequestExecutionUrl(pageRenderMode, "resourceID", "activityID", "processID");

			NameValueCollection originalParams = UriHelper.GetUriParamsCollection(url);

			originalParams.Remove(PassportManager.TicketParamName);

			return UriHelper.CombineUrlParams(url,
					originalParams,
					UriHelper.GetUriParamsCollection(this.ControlParams.ToRequestParams()));
		}

		protected override void InitDialogContent(Control container)
		{
			this.Page.EnableViewState = false;

			base.InitDialogContent(container);

			this.ID = "activityEditorDialog";

			actName = (HtmlInputText)WebControlUtility.FindControlByHtmlIDProperty(container, "actName", true);

			if (this.Operation == "update" && CurrentActivityDescriptor != null)
				actName.Value = CurrentActivityDescriptor.Name;

			InitAllAgreeWhenConsignCheckbox(WebControlUtility.FindControlByHtmlIDProperty(container, "allAgreeWhenConsignCheckbox", true));
			InitVariableRow(WebControlUtility.FindControlByHtmlIDProperty(container, "variableRow", true));
			InitVariableCheckBoxes(WebControlUtility.FindControlByHtmlIDProperty(container, "variableList", true));

			InitUserInputArea(container);
		}

		protected abstract void InitUserInputArea(Control container);

		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 440;
			feature.Height = 368;
			feature.Resizable = false;
			feature.ShowStatusBar = false;
			feature.ShowScrollBars = false;
			feature.Center = true;

			return feature.ToDialogFeatureClientString();
		}

		protected override void InitConfirmButton(HtmlInputButton confirmButton)
		{
			confirmButton.Attributes["onclick"] = "onDialogConfirm();";
		}

		protected virtual bool GetAllAgreeWhenConsignCheckboxValue()
		{
			bool result = false;

			if (ControlParams.Operation != "add")
			{
				if (CurrentActivityDescriptor != null && CurrentActivityDescriptor.Variables.Exists(v => v.Key == WfVariableDefine.AllAgreeWhenConsignVariableName))
					result = CurrentActivityDescriptor.Variables.GetValue(WfVariableDefine.AllAgreeWhenConsignVariableName, false);
				else
					result = AllAgreeWhenConsign;
			}
			else
				result = AllAgreeWhenConsign;

			return result;
		}
		#endregion Protected

		#region Private
		private void InitAllAgreeWhenConsignCheckbox(Control checkbox)
		{
			if (checkbox != null)
			{
				bool visible = true;

				visible = (CurrentActivityDescriptor != null && AllowMultiUsers);

				this.allAgreeWhenConsignCheckbox = (HtmlInputCheckBox)checkbox;

				this.allAgreeWhenConsignCheckbox.Checked = GetAllAgreeWhenConsignCheckboxValue();

				if (visible == false)
					this.allAgreeWhenConsignCheckbox.Style["display"] = "none";

				HtmlGenericControl label = (HtmlGenericControl)WebControlUtility.FindControlByHtmlIDProperty(allAgreeWhenConsignCheckbox.Parent, "allAgreeWhenConsignLabel", true);

				if (label != null)
				{
					label.Attributes["for"] = allAgreeWhenConsignCheckbox.ClientID;

					if (visible == false)
						label.Style["display"] = "none";
				}
			}
		}

		#region inner class
		protected class VariableObject
		{
			private string key;
			private string description;
			private bool defValue;

			/// <summary>
			/// Variable name key
			/// </summary>
			public string Key
			{
				get { return key; }
			}

			/// <summary>
			/// Variable description
			/// </summary>
			public string Description
			{
				get { return description; }
			}

			/// <summary>
			/// Variable default value
			/// </summary>
			public bool DefaultValue
			{
				get { return defValue; }
			}

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="nameKey">Variable name key</param>
			/// <param name="descriptionText">Variable description</param>
			/// <param name="defaultValue">Variable default value</param>
			public VariableObject(string nameKey, string descriptionText, bool defaultValue)
			{
				key = nameKey;
				description = descriptionText;
				defValue = defaultValue;
			}
		}
		#endregion inner class

		protected bool InSmallMode
		{
			get
			{
				return WebUtility.GetRequestQueryValue("small", false);
			}
		}

		private void InitVariableRow(Control row)
		{
			if (row != null)
			{
				bool visible = true;

				if (InSmallMode)
				{
					visible = false;
				}
				else
				{
					visible = (CurrentActivityDescriptor != null && CurrentActivityDescriptor.Properties.GetValue("AllowToBeModifiedVariables", true));
				}

				if (visible == false)
					((HtmlControl)row).Style["display"] = "none";
			}
		}

		private void InitVariableCheckBoxes(Control ul)
		{
			List<VariableObject> variables = new List<VariableObject>();

			InitVarialbeEditorByConfig(variables);

			/*
			if (WfControlSettings.GetConfig().Enabled)
				InitVarialbeEditorByConfig(variables);
			else
				InitDefaultVarialbeEditor(variables);
			*/

			VariablesInitialized(variables);

			if (ul != null)
			{
				foreach (VariableObject var in variables)
				{
					HtmlGenericControl li = new HtmlGenericControl("li");

					HtmlInputCheckBox checkbox = new HtmlInputCheckBox();
					checkbox.Name = "actVariable";
					checkbox.Value = var.Key;

					bool liChecked = var.DefaultValue;

					if (this.Operation == "update" && CurrentActivityDescriptor != null)
						liChecked = CurrentActivityDescriptor.Properties.GetValue(var.Key, var.DefaultValue);

					checkbox.Checked = liChecked;

					li.Controls.Add(checkbox);

					HtmlGenericControl text = new HtmlGenericControl("span");

					text.InnerText = Translator.Translate(Define.DefaultCulture, var.Description);
					li.Controls.Add(text);

					ul.Controls.Add(li);
				}
			}
		}

		protected virtual void VariablesInitialized(List<VariableObject> variables)
		{
		}

		private void InitVarialbeEditorByConfig(List<VariableObject> variables)
		{
			PropertyGroupConfigurationElement configProperties = WfActivitySettings.GetConfig().PropertyGroups["RuntimeAddedActivityTemplate"];

			if (configProperties != null)
			{
				foreach (PropertyDefineConfigurationElement propertyDefine in configProperties.AllProperties)
				{
					string name = propertyDefine.Name;

					if (propertyDefine.DisplayName.IsNotEmpty())
						name = propertyDefine.DisplayName;

					VariableObject vo = new VariableObject(propertyDefine.Name, propertyDefine.DisplayName, bool.Parse(propertyDefine.DefaultValue));
					variables.Add(vo);
				}
			}
		}

		/*
		private void InitVarialbeEditorByConfig(List<VariableObject> variables)
		{
			foreach (WfActivityEditorVariableElement elem in WfControlSettings.GetConfig().ActivityEditorVariables)
			{
				if (elem.Visible)
				{
					bool specialVariable = false;

					if (elem.Name == WfVariableDefine.AllowDeleteActivityVariableName && InSmallMode)
					{
						variables.Add(new VariableObject(elem.Name, elem.Description, true));
						specialVariable = true;
					}

					if (elem.Name == WfVariableDefine.AllowAppendActivityVariableName && InSmallMode)
					{
						variables.Add(new VariableObject(elem.Name, elem.Description, false));
						specialVariable = true;
					}

					if (specialVariable == false)
						variables.Add(new VariableObject(elem.Name, elem.Description, elem.DefaultValue));
				}
			}

			if (InSmallMode)
				variables.Add(new VariableObject(WfVariableDefine.UseSmallEditModeVariableName, "使用简单模式编辑环节", true));
		}

		private void InitDefaultVarialbeEditor(List<VariableObject> variables)
		{
			variables.Add(new VariableObject(WfVariableDefine.AddAppoverVariableName, "允许加签", true));
			variables.Add(new VariableObject(WfVariableDefine.ChangeAppoverVariableName, "允许转签", true));
			variables.Add(new VariableObject(WfVariableDefine.ConsignAppoverVariableName, "允许会签", true));
			variables.Add(new VariableObject(WfVariableDefine.AllowAppendActivityVariableName, "允许添加后续环节", this.Operation == "update" ? false : true));
			variables.Add(new VariableObject(WfVariableDefine.AllowDeleteActivityVariableName, "允许删除此环节", InSmallMode || this.Operation == "update" ? false : true));
			variables.Add(new VariableObject(WfVariableDefine.ForbidModifyNextStepsVariableName, "禁止修改后续环节", true));
			variables.Add(new VariableObject(WfVariableDefine.AddAttachmentVariableName, "可以增加附件", true));
			variables.Add(new VariableObject(WfVariableDefine.ChangeAttachmentVariableName, "可以修改附件", true));
			variables.Add(new VariableObject(WfVariableDefine.AllowEmptyOperatorVariableName, "允许环节审批人员为空", false));
			variables.Add(new VariableObject(WfVariableDefine.MustEmptyOperatorVariableName, "环节审批人员必须为空", false));

			if (InSmallMode)
				variables.Add(new VariableObject(WfVariableDefine.UseSmallEditModeVariableName, "使用简单模式编辑环节", true));
		}
		*/

		protected string GetSmallDialogUrl()
		{
			PageRenderMode pageRenderMode = new PageRenderMode(this.UniqueID, "DialogControl");

			string url = WebUtility.GetRequestExecutionUrl(pageRenderMode, "resourceID", "activityID", "processID");

			NameValueCollection originalParams = UriHelper.GetUriParamsCollection(url);

			originalParams.Remove(PassportManager.TicketParamName);

			originalParams.Add("small", "true");

			return UriHelper.CombineUrlParams(url,
					originalParams,
					UriHelper.GetUriParamsCollection(this.ControlParams.ToRequestParams()));
		}

		protected virtual string GetSmallDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 440;
			feature.Height = 256;
			feature.Resizable = false;
			feature.ShowStatusBar = false;
			feature.ShowScrollBars = false;
			feature.Center = true;

			return feature.ToDialogFeatureClientString();
		}
		#endregion
	}
}
