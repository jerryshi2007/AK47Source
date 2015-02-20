using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;

namespace WorkflowDesigner.ExternalDialogs
{
	public partial class OperationEditor : System.Web.UI.UserControl
	{
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

		protected void Page_Load(object sender, EventArgs e)
		{

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
				WebUtility.ResponseShowClientErrorScriptBlock(ex);
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
			base.OnPreRender(e);

			if (this.Page.IsPostBack == false && this.Page.IsCallback == false)
			{
				string processDespJson = string.Empty;

				if (this.CurrentProcessDescriptor != null)
				{
					OnPreRenderControl(e);

					int originalUpdateTag = WebUtility.GetRequestQueryValue("updateTag", -1);

					if (originalUpdateTag == -1)
						originalUpdateTag = this.CurrentProcess.UpdateTag;

					this.Page.ClientScript.RegisterHiddenField("originalProcessTag", originalUpdateTag.ToString());
				}
			}
		}
	}
}