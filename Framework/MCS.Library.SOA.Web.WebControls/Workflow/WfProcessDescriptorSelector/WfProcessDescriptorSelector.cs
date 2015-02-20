using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Web.Library.Script;
using System.ComponentModel;
using System.Web;
using MCS.Web.Library;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Workflow.DTO;
using MCS.Library.Core;

[assembly: WebResource("MCS.Web.WebControls.Workflow.WfProcessDescriptorSelector.WfProcessDescriptorSelector.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.Workflow.WfProcessDescriptorSelector.WfProcessDescriptorSelectorTemplate.htm", "text/html")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.WfProcessDescriptorSelector", "MCS.Web.WebControls.Workflow.WfProcessDescriptorSelector.WfProcessDescriptorSelector.js")]
	[DialogContent("MCS.Web.WebControls.Workflow.WfProcessDescriptorSelector.WfProcessDescriptorSelectorTemplate.htm", true)]

	public class WfProcessDescriptorSelector : DialogControlBase<WfProcessDescriptorSelectorParams>
	{

		private Button serverConfirmButton = null;
		private DeluxeGrid processGrid = null;
		private DropDownList ddlEnabled = null;
		private TextBoxDropdownExtender dropdownExtender = null;
		private Button btnSearch = null;
		private TextBox txtApplicationName;
		private TextBox txtProgramName;
		private TextBox txtProcessKey;
		private TextBox txtProcessName;

		private int LastQueryRowCount
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value);
			}
		}

		[Bindable(true), Category("ControlDialogTitle"), Description("控件的标题")]
		public string ControlDialogTitle
		{
			get
			{
				return GetPropertyValue("ControlDialogTitle", string.Empty);
			}
			set
			{
				SetPropertyValue("ControlDialogTitle", value);
			}
		}

		/// <summary>
		/// 是否多选
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("multiSelect")]
		[DefaultValue(false)]
		public bool MultiSelect
		{
			get
			{
				return ControlParams.MultiSelect;
			}
			set
			{
				ControlParams.MultiSelect = value;
			}
		}

		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Center = true;
			feature.Width = 720;
			feature.Height = 550;
			feature.Resizable = true;
			feature.ShowScrollBars = false;
			feature.ShowToolBar = false;
			feature.ShowStatusBar = false;

			return feature.ToDialogFeatureClientString();
		}


		protected override System.Web.UI.Control LoadDialogTemplate()
		{
			ScriptManager sm = null;
			ScriptControlHelper.EnsureScriptManager(ref sm, this.Page);

			return base.LoadDialogTemplate();
		}

		protected override void InitDialogTitle(HtmlHead header)
		{
			if (ControlDialogTitle == null || string.IsNullOrEmpty(ControlDialogTitle))
				this.DialogTitle = "流程模板列表";
			else
				this.DialogTitle = ControlDialogTitle;

			base.InitDialogTitle(header);
		}

		protected override void InitDialogContent(Control container)
		{
			base.InitDialogContent(container);
			this.serverConfirmButton = (Button)this.FindControlByID("serverConfirmButton", true);
			ddlEnabled = (DropDownList)this.FindControlByID("ddlEnabled", true);
			dropdownExtender = (TextBoxDropdownExtender)this.FindControlByID("dropdownExtender", true);
			btnSearch = (Button)this.FindControlByID("btnSearch", true);
			this.processGrid = (DeluxeGrid)this.FindControlByID("ProcessDescInfoDeluxeGrid", true);
			txtApplicationName = (TextBox)this.FindControlByID("txtApplicationName", true);
			txtProgramName = (TextBox)this.FindControlByID("txtProgramName", true);
			txtProcessKey = (TextBox)this.FindControlByID("txtProcessKey", true);
			txtProcessName = (TextBox)this.FindControlByID("txtProcessName", true);

			btnSearch.Click += new EventHandler(btnSearch_Click);

			this.serverConfirmButton.Click += new EventHandler(serverConfirmButton_Click);
			ObjectDataSource objectDataSource = container.FindControl<ObjectDataSource>(true);

			objectDataSource.Selecting += new ObjectDataSourceSelectingEventHandler(objectDataSource_Selecting);
			objectDataSource.Selected += new ObjectDataSourceStatusEventHandler(objectDataSource_Selected);

			if (!Page.IsPostBack)
			{
				dropdownExtender.DataSource = WfProcessDescriptionCategoryAdapter.Instance.Load(p => p.AppendItem("ID", "", "<>"));
				dropdownExtender.DataValueField = "Name";
				dropdownExtender.DataTextField = "Name";
				dropdownExtender.DataBind();

				this.processGrid.MultiSelect = this.MultiSelect;

				//ddlEnabled.Items.Add(new ListItem("请选择", ""));
				//ddlEnabled.Items.Add(new ListItem("是", "1"));
				//ddlEnabled.Items.Add(new ListItem("否", "0"));

				ExecuteQuery();
			}

		}

		protected override string ReplaceDialogTemplateString(string html)
		{

			return string.Format(html, "", this.MultiSelect.ToString());
		}

		void btnSearch_Click(object sender, EventArgs e)
		{
			ExecuteQuery();
		}

		private void ExecuteQuery()
		{
			LastQueryRowCount = -1;
			var whereCondition = (HtmlInputHidden)this.FindControlByID("whereCondition", true);

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			if (!string.IsNullOrEmpty(txtApplicationName.Text))
				builder.AppendItem("APPLICATION_NAME", TSqlBuilder.Instance.CheckQuotationMark(txtApplicationName.Text, false));

			if (!string.IsNullOrEmpty(txtProgramName.Text))
				builder.AppendItem("PROGRAM_NAME", "%" + TSqlBuilder.Instance.CheckQuotationMark(txtProgramName.Text, false) + "%", "like");

			if (!string.IsNullOrEmpty(txtProcessKey.Text))
				builder.AppendItem("PROCESS_KEY", "%" + TSqlBuilder.Instance.CheckQuotationMark(txtProcessKey.Text, false) + "%", "like");

			if (!string.IsNullOrEmpty(txtProcessName.Text))
				builder.AppendItem("PROCESS_NAME", "%" + TSqlBuilder.Instance.CheckQuotationMark(txtProcessName.Text, false) + "%", "like");

			if (!string.IsNullOrEmpty(ddlEnabled.SelectedValue))
				builder.AppendItem("ENABLED", TSqlBuilder.Instance.CheckQuotationMark(ddlEnabled.SelectedValue, false));

			whereCondition.Value = builder.ToSqlString(TSqlBuilder.Instance);

			//this.processGrid.SelectedKeys.Clear();
			this.processGrid.PageIndex = 0;
		}

		private void serverConfirmButton_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.processGrid != null)
				{
					List<WorkflowInfo> processInfoList = new List<WorkflowInfo>();

					foreach (string key in this.processGrid.SelectedKeys)
					{
						IWfProcess process = WfRuntime.GetProcessByProcessID(key);
						var processInfo = WorkflowInfo.ProcessAdapter(process);

						processInfoList.Add(processInfo);
					}

					HtmlInputHidden resultData = (HtmlInputHidden)WebControlUtility.FindControlByID(this, "resultData", true);

					if (resultData != null)
						resultData.Value = JSONSerializerExecute.Serialize(processInfoList);

					ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "Close",
						string.Format("window.returnValue = $get('{0}').value; top.close()", resultData.ClientID),
						true);
				}
			}
			catch (System.Exception ex)
			{
				string errorScript = WebUtility.GetShowClientErrorScript(ex.Message, ex.StackTrace, "错误");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "ShowError",
					"SubmitButton.resetAllStates();" + errorScript,
					true);
			}
		}

		protected void objectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["totalCount"] = LastQueryRowCount;
		}

		protected void objectDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			LastQueryRowCount = (int)e.OutputParameters["totalCount"];
		}

		protected override void InitConfirmButton(HtmlInputButton confirmButton)
		{
			base.InitConfirmButton(confirmButton);
			confirmButton.ServerClick += new EventHandler(confirmButton_ServerClick);
			///confirmButton.Attributes["onclick"] = string.Format("window.returnValue = '{0}';window.close();", JSONSerializerExecute.Serialize(this.processGrid.SelectedDataKey));
		}

		void confirmButton_ServerClick(object sender, EventArgs e)
		{
			try
			{
				var selectedKes = processGrid.SelectedKeys;
				List<SimpleProcess> simpleProcessColl = new List<SimpleProcess>();
				foreach (var item in selectedKes)
				{

					var process = WfProcessDescriptorInfoAdapter.Instance.Load(item);
					simpleProcessColl.Add(new SimpleProcess() { Key = item, Name = process.ProcessName });
				}
				Page.ClientScript.RegisterStartupScript(this.GetType(), "returnProcesses",
				string.Format("window.returnValue = {0}; top.close();", JSONSerializerExecute.Serialize(simpleProcessColl)),
				true);

			}
			catch (System.Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}

		}

	}

	class SimpleProcess
	{
		public string Key { get; set; }
		public string Name { get; set; }

	}
}
