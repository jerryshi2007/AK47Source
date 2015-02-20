using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.MultiProcessControl.MultiProcessControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.MultiProcessControl.MultiProcessControl.htm", "text/html")]
namespace MCS.Web.WebControls
{
	public interface IMultiProcessPrepareData
	{
		void ResponsePrepareDataInfo(string text);
	}

	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.MultiProcessControl", "MCS.Web.WebControls.MultiProcessControl.MultiProcessControl.js")]
	[DialogContent("MCS.Web.WebControls.MultiProcessControl.MultiProcessControl.htm")]
	[ToolboxData("<{0}:MutifyProcessControl runat=server></{0}:MutifyProcessControl>")]
	public class MultiProcessControl : DialogControlBase<MultiProcessControlParam>, IMultiProcessPrepareData
	{
		private HiddenButtonWrapper buttonWrapper = new HiddenButtonWrapper();
		private Button prepareDataButton = new Button() { ID = "prepareDataButton" };
		private HtmlTextArea errorMessages = null;

		/// <summary>
		/// 出错时
		/// </summary>
		/// <param name="ex"></param>
		public delegate void ErrorDelegate(Exception ex, object data, ref bool isThrow);
		public delegate void ExecuteStepDelegate(object data);
		public delegate object ExecutePrepareDataDelegate(object data, IMultiProcessPrepareData owner);

		/// <summary>
		/// 单个进程执行的内容
		/// </summary>
		public event ExecuteStepDelegate ExecuteStep;

		/// <summary>
		/// 出错时
		/// </summary>
		public event ErrorDelegate Error;

		/// <summary>
		/// 准备数据
		/// </summary>
		public event ExecutePrepareDataDelegate ExecutePrepareData;

		#region 属性
		/// <summary>
		/// 点击后，能够弹出对话框的控件ID
		/// </summary>
		[DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(DialogStartUpControlConverter))]
		public string ControlIDToShowDialog
		{
			get
			{
				return buttonWrapper.TargetControlID;
			}
			set
			{
				buttonWrapper.TargetControlID = value;
			}
		}

		/// <summary>
		/// 点击后，能够弹出对话框的控件的实例
		/// </summary>
		[Browsable(false)]
		public IAttributeAccessor ControlToShowDialog
		{
			get
			{
				return buttonWrapper.TargetControl;
			}
			set
			{
				buttonWrapper.TargetControl = value;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("showStepErrors")]
		public bool ShowStepErrors
		{
			get
			{
				return ControlParams.ShowStepErrors;
			}
			set
			{
				ControlParams.ShowStepErrors = value;
			}
		}

		/// <summary>
		/// 启动任务前的客户端事件,用于收集数据
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("beforeStart")]
		[Bindable(true), Category("ClientEventsHandler"), Description("启动任务前的客户端事件")]
		public string OnClientBeforeStart
		{
			get
			{
				return GetPropertyValue("OnClientBeforeStart", string.Empty);
			}
			set
			{
				SetPropertyValue("OnClientBeforeStart", value);
			}
		}

		/// <summary>
		/// 客户端事件，结束后的事件
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("finishedProcess")]
		[Bindable(true), Category("ClientEventsHandler"), Description("处理完成后客户端事件")]
		public string OnClientFinishedProcess
		{
			get
			{
				return GetPropertyValue("OnClientFinishedProcess", string.Empty);
			}
			set
			{
				SetPropertyValue("OnClientFinishedProcess", value);
			}
		}

		/// <summary>
		/// 客户端事件，结束后的事件
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("cancelProcess")]
		[Bindable(true), Category("ClientEventsHandler"), Description("处理中止后客户端事件")]
		public string OnClientCancelProcess
		{
			get
			{
				return GetPropertyValue("OnClientCancelProcess", string.Empty);
			}
			set
			{
				SetPropertyValue("OnClientCancelProcess", value);
			}
		}

		[DefaultValue(false)]
		[ClientPropertyName("addPrepareDataStep")]
		[ScriptControlProperty]
		[Bindable(true), Category("Appearance"), Description("是否增加准备数据的步骤")]
		public bool AddPrepareDataStep
		{
			get
			{
				return GetPropertyValue("AddPrepareDataStep", false);
			}
			set
			{
				SetPropertyValue("AddPrepareDataStep", value);
			}
		}

		[DefaultValue(false)]
		[ClientPropertyName("prepareDataStepButtonID")]
		[ScriptControlProperty]
		[Bindable(true), Category("Appearance"), Description("是否增加准备数据的步骤")]
		private string PrepareDataStepButtonID
		{
			get
			{
				return this.prepareDataButton.ClientID;
			}
		}

		[ClientPropertyName("errorMessagesClientID")]
		[ScriptControlProperty]
		private string ErrorMessagesClientID
		{
			get
			{
				string result = string.Empty;

				if (this.errorMessages != null)
					result = this.errorMessages.ClientID;

				return result;
			}
		}
		#endregion

		#region 重载/初始化
		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);

			HiddenButtonWrapper bw = GetPropertyValue("ButtonWrapper", (HiddenButtonWrapper)null);
			buttonWrapper.TargetControlID = bw.TargetControlID;
		}

		protected override object SaveViewState()
		{
			SetPropertyValue("ButtonWrapper", this.buttonWrapper);

			return base.SaveViewState();
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (DesignMode)
				writer.Write("MultiProgress");
			else
				base.Render(writer);
		}

		/// <summary>
		/// 重载，页面渲染之前
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			if (!DesignMode)
			{
				if (Page.IsCallback == false)
				{
					if (CurrentMode == ControlShowingMode.Dialog)
					{
						Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

						if (!Page.IsPostBack && !Page.IsCallback && !Page.IsCrossPagePostBack)
						{
							OnClientBeforeStart = string.Empty;
							OnClientFinishedProcess = string.Empty;
							OnClientCancelProcess = string.Empty;
						}

						Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ControlID",
							string.Format("var m_controlID = \"{0}\";", WebUtility.CheckScriptString(this.ClientID, false)),
							true);
					}
					else
					{
						if (ShowingMode == ControlShowingMode.Dialog)
							InitShowDialogControl();
					}
				}
			}
			base.OnPreRender(e);
		}

		/// <summary>
		/// 重载，获取对话框属性
		/// </summary>
		/// <returns></returns>
		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Center = true;
			feature.Width = 450;
			feature.Height = 240;
			feature.Resizable = true;
			feature.ShowScrollBars = false;
			feature.ShowToolBar = false;
			feature.ShowStatusBar = false;

			return feature.ToDialogFeatureClientString();
		}

		/// <summary>
		/// 重载，初始化确定按钮
		/// </summar~y>
		/// <param name="confirmButton"></param>
		protected override void InitConfirmButton(HtmlInputButton confirmButton)
		{
			confirmButton.Style["display"] = "none";
			confirmButton.ID = "confirmBtn";
		}

		/// <summary>
		/// 重载，初始化取消按钮
		/// </summary>
		/// <param name="cancelButton"></param>
		protected override void InitCancelButton(System.Web.UI.HtmlControls.HtmlInputButton cancelButton)
		{
			base.InitCancelButton(cancelButton);
		}

		protected override void InitDialogContent(Control container)
		{
			this.Page.Form.Target = "innerFrame";
			this.prepareDataButton.Style["display"] = "none";
			this.prepareDataButton.Click += new EventHandler(PrepareDataButton_Click);
			container.Controls.Add(this.prepareDataButton);

			this.errorMessages = (HtmlTextArea)container.FindControlByID("errorMessages", true);

			if (this.errorMessages != null)
			{
				if (this.ShowStepErrors == false)
					this.errorMessages.Style["display"] = "none";
			}

			base.InitDialogContent(container);
		}
		#endregion

		#region 私有方法
		private void InitShowDialogControl()
		{
			if (ControlToShowDialog != null)
				ControlToShowDialog.SetAttribute("onclick", string.Format("$find('{0}').start(); return false;", this.ClientID));
		}

		private void PrepareDataButton_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.ExecutePrepareData != null)
				{
					HttpContext.Current.Response.Buffer = false;

					string serializedData = HttpContext.Current.Request.Form["preparedData"];
					object data = JSONSerializerExecute.DeserializeObject(serializedData);

					object result = this.ExecutePrepareData(data, this);
					ResponsePrepareDataResult(result);
				}
			}
			catch (System.Exception ex)
			{
				ResponseExceptionInfo(ex);
			}
			finally
			{
				HttpContext.Current.Response.End();
			}
		}

		void IMultiProcessPrepareData.ResponsePrepareDataInfo(string text)
		{
			HttpContext.Current.Response.ResponseWithScriptTag((writer) =>
			{
				writer.Write("top.document.getElementById(\"notifyText\").value = ");
				writer.WriteLine("\"" + WebUtility.CheckScriptString(text, false) + "\";");

				writer.WriteLine("top.document.getElementById(\"changeTextBtn\").click();");
			});
		}

		private static void ResponseExceptionInfo(System.Exception ex)
		{
			string stackTrace = string.Empty;

			if (WebUtility.AllowResponseExceptionStackTrace())
				stackTrace = ex.StackTrace;

			var errorObj = new { message = ex.Message, description = stackTrace };

			string serializedError = JSONSerializerExecute.Serialize(errorObj);

			HttpContext.Current.Response.ResponseWithScriptTag((writer) =>
			{
				writer.Write("top.document.getElementById(\"prepareDataError\").value = ");
				writer.WriteLine("\"" + WebUtility.CheckScriptString(serializedError, false) + "\";");

				writer.WriteLine("top.document.getElementById(\"raiseErrorBtn\").click();");
			});
		}

		private static void ResponsePrepareDataResult(object data)
		{
			string serializedData = JSONSerializerExecute.Serialize(data);

			HttpContext.Current.Response.ResponseWithScriptTag((writer) =>
			{
				writer.Write("top.document.getElementById(\"preparedData\").value = ");
				writer.WriteLine("\"" + WebUtility.CheckScriptString(serializedData, false) + "\";");

				writer.WriteLine("top.document.getElementById(\"prepareDataFinished\").click();");
			});
		}
		#endregion

		#region 事件
		[ScriptControlMethod]
		public string OnExecuteStep(object data)
		{
			string result = string.Empty;

			try
			{
				if (ExecuteStep != null)
					ExecuteStep(data);
			}
			catch (Exception ex)
			{
				bool isThrow = true;
				if (this.Error != null)
				{
					this.Error(ex, data, ref isThrow);

					if (isThrow)
					{
						throw ex.GetRealException();
					}
					else
						result = ex.Message;
				}
				else
				{
					throw ex.GetRealException();
				}
			}

			return result;
		}
		#endregion
	}
}
