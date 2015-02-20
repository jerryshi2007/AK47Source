using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;

namespace WorkflowDesigner.ExternalDialogs
{
	public partial class DescriptorEditor : System.Web.UI.UserControl, INamingContainer
	{
		private PropertyGrid propertyGrid = null;

		public event EventHandler PreRenderControl;
		public event EventHandler SaveButtonClicked;

		/// <summary>
		/// 流程的最后更新标记
		/// </summary>
		public int ProcessUpdateTag
		{
			get
			{
				return WebUtility.GetRequestFormValue("originalProcessTag", 0);
			}
		}

		public string SaveButtonID
		{
			get
			{
				return ViewState.GetViewStateValue("SaveButtonID", string.Empty);
			}
			set
			{
				ViewState.SetViewStateValue("SaveButtonID", value);
			}
		}

		public string PropertyGridID
		{
			get
			{
				return ViewState.GetViewStateValue("PropertyGridID", string.Empty);
			}
			set
			{
				ViewState.SetViewStateValue("PropertyGridID", value);
			}
		}

		public string DialogTitle
		{
			get
			{
				return ViewState.GetViewStateValue("DialogTitle", string.Empty);
			}
			set
			{
				ViewState.SetViewStateValue("DialogTitle", value);
			}
		}

		public IWfProcess CurrentProcess
		{
			get
			{
				string processID = WebUtility.GetRequestQueryString("processID", string.Empty);

				IWfProcess process = null;

				if (processID.IsNotEmpty())
					process = WfRuntime.GetProcessByProcessID(processID);

				return process;
			}
		}

		public IWfProcessDescriptor CurrentProcessDescriptor
		{
			get
			{
				IWfProcessDescriptor result = null;

				if (this.CurrentProcess != null)
				{
					if (this.ShowMainStream)
						result = this.CurrentProcess.MainStream;
					else
						result = this.CurrentProcess.Descriptor;
				}

				return result;
			}
		}

		public bool ShowMainStream
		{
			get
			{
				return WebUtility.GetRequestQueryValue("mainStream", false);
			}
		}

		/// <summary>
		/// 注册客户端流程反序列化后的脚本
		/// </summary>
		/// <param name="funcBody"></param>
		public void RegisterAfterProcessDeserializedFunction(string funcBody)
		{
			StringBuilder strB = new StringBuilder();

			strB.AppendLine("\nfunction afterProcessDeserialized(process) {");
			strB.AppendLine("\t" + funcBody);
			strB.AppendLine("}");

			this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AfterProcessDeserializedFunction", strB.ToString(), true);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			PropertyEditorRegister();
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			WfConverterHelper.RegisterConverters();

			if (this.SaveButtonID.IsNotEmpty())
			{
				IButtonControl button = this.Page.FindControlByID(this.SaveButtonID, true) as IButtonControl;

				if (button != null)
					button.Click += new EventHandler(button_Click);
			}
		}

		private void button_Click(object sender, EventArgs e)
		{
			try
			{
				OnSaveButtonClicked(e);

				Response.ResponseWithScriptTag(writer =>
				{
					writer.WriteLine("top.window.returnValue = true;");
					writer.WriteLine("top.document.getElementById('resetAllStateBtn').click();");
				});

				WebUtility.ResponseCloseWindowScriptBlock();
			}
			catch (System.Exception ex)
			{
				Response.ResponseWithScriptTag(writer => writer.Write("top.document.getElementById('resetAllStateBtn').click()"));
				WebUtility.ResponseShowClientErrorScriptBlock(ex.GetRealException());
			}
		}

		protected virtual void OnPreRenderControl(EventArgs e)
		{
			if (this.PreRenderControl != null)
				this.PreRenderControl(this, e);
		}

		protected virtual void OnSaveButtonClicked(EventArgs e)
		{
			if (this.SaveButtonClicked != null)
			{
				if (this.CurrentProcess != null)
				{
					(this.CurrentProcess.UpdateTag == this.ProcessUpdateTag).FalseThrow("流程信息已经改变，请刷新页面或重新打开此页面");
				}

				this.SaveButtonClicked(this, e);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (this.PropertyGridID.IsNotEmpty())
			{
				this.propertyGrid = this.Page.FindControlByID(this.PropertyGridID, true) as PropertyGrid;

				if (this.propertyGrid != null)
					this.propertyGrid.OnClientClickEditor = "onClickEditor";
			}

			InitializeEnumStore();

			base.OnPreRender(e);

			if (this.Page.IsPostBack == false && this.Page.IsCallback == false)
			{
				string processDespJson = string.Empty;

				if (this.CurrentProcessDescriptor != null)
				{
					OnPreRenderControl(e);

					processDespJson =
						JSONSerializerExecute.Serialize(new List<IWfProcessDescriptor>() { this.CurrentProcessDescriptor });

					int originalUpdateTag = WebUtility.GetRequestQueryValue("updateTag", -1);

					if (originalUpdateTag == -1)
						originalUpdateTag = this.CurrentProcess.UpdateTag;

					this.Page.ClientScript.RegisterHiddenField("originalProcessTag", originalUpdateTag.ToString());
				}

				this.Page.ClientScript.RegisterHiddenField("instanceDescription", processDespJson);
			}

			RegisterScripts(this.Page);
		}

		private void RegisterScripts(Page page)
		{
			//注册脚本，将JSON格式的流程数据反序列化
			page.ClientScript.RegisterStartupScript(this.GetType(), "loadProcessInstanceDescription", "Sys.Application.add_load(loadProcessInstanceDescription);", true);

			page.ClientScript.RegisterStartupScript(this.GetType(), "WFObjectListPropertyEditor", "<script type='text/javascript' src='../js/WFObjectListPropertyEditor.js'></script>");
			page.ClientScript.RegisterStartupScript(this.GetType(), "BranchProcessPropertyEditor", "<script type='text/javascript' src='../js/BranchProcessPropertyEditor.js'></script>");
		}

		/// <summary>
		/// register enums used in PropertyGrid control
		/// </summary>
		private void InitializeEnumStore()
		{
			if (this.propertyGrid != null)
			{
				this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfProcessType, MCS.Library.SOA.DataObjects");
				this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfNavigatorDisplayMode, MCS.Library.SOA.DataObjects");
				this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfSubProcessApprovalMode, MCS.Library.SOA.DataObjects");
				this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfBranchGroupBlockingType, MCS.Library.SOA.DataObjects");
				this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfBranchProcessBlockingType, MCS.Library.SOA.DataObjects");
				this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfBranchProcessExecuteSequence, MCS.Library.SOA.DataObjects");
				this.propertyGrid.PredefinedEnumTypes.Add("WorkflowDesigner.PropertyEditor.BranchProcessKey, WorkflowDesigner");
				this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfSearchIDMode, MCS.Library.SOA.DataObjects");
				this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfAutoSendUserTaskMode, MCS.Library.SOA.DataObjects");
			}
		}

		private static void PropertyEditorRegister()
		{
			PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new EnumPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ObjectPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DatePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DateTimePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ObjectListPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ReceiversObjectListPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ConditionExpressionPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new KeyPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new CanActivityKeysEditor());
			PropertyEditorHelper.RegisterEditor(new BranchProcessKeyPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DynamicPropertyEditor());
		}
	}
}