using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library.Script;
using MCS.Web.Library;
using System.Web.UI.HtmlControls;
using System.Threading;
using MCS.Library.Core;
using MCS.Library.Globalization;

[assembly: WebResource("MCS.Web.WebControls.UploadProgress.UploadProgressControl.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 处理上传的控件
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.UploadProgressControl",
		"MCS.Web.WebControls.UploadProgress.UploadProgressControl.js")]
	[DialogContent("MCS.Web.WebControls.UploadProgress.UploadProgressControlTemplate.htm", "MCS.Library.SOA.Web.WebControls")]
	[ToolboxData("<{0}:UploadProgressControl runat=server></{0}:UploadProgressControl>")]
	public class UploadProgressControl : DialogControlBase<UploadProgressControlParams>
	{
		private HiddenButtonWrapper buttonWrapper = new HiddenButtonWrapper();

		public event DoUploadProgressDelegate DoUploadProgress;
		public event UploadProgressContentInitedEventHandler UploadProgressContentInited;

		public UploadProgressControl()
		{
			JSONSerializerExecute.RegisterConverter(typeof(ProcessProgressConverter));
		}

		#region Properties
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

		/// <summary>
		/// 启动任务前的客户端事件,用于收集数据或判断是否能执行
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("beforeStart")]
		[Bindable(true), Category("ClientEventsHandler"), Description("启动任务前的客户端事件，用于收集数据或判断是否能执行")]
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
		/// 任务完成后的客户端事件，通过判断e.dataChanged属性来判断数据是否改变
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("completed")]
		[Bindable(true), Category("ClientEventsHandler"), Description("任务完成后的客户端事件，通过判断e.dataChanged属性来判断数据是否改变")]
		public string OnClientCompleted
		{
			get
			{
				return GetPropertyValue("OnClientCompleted", string.Empty);
			}
			set
			{
				SetPropertyValue("OnClientCompleted", value);
			}
		}

		/// <summary>
		/// 传递给客户端，再传递上来的参数
		/// </summary>
		[Browsable(true)]
		[ScriptControlProperty(), ClientPropertyName("postedData")]
		public string PostedData
		{
			get
			{
				return GetPropertyValue("PostedData", string.Empty);
			}
			set
			{
				SetPropertyValue("PostedData", value);
			}
		}
		#endregion Properties

		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 540;
			feature.Height = 300;
			feature.Resizable = false;
			feature.ShowStatusBar = false;
			feature.ShowScrollBars = false;
			feature.Center = true;

			return feature.ToDialogFeatureClientString();
		}

		protected override void OnLoad(EventArgs e)
		{
			if (Page.Request.Form["uploadFileBtn"] == "Upload File")
			{
				ProcessUploadFile();
			}

			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (this.CurrentMode == ControlShowingMode.Normal)
			{
				if (ControlToShowDialog != null)
					ControlToShowDialog.SetAttribute("onclick", string.Format("if (!event.srcElement.disabled) $find('{0}').showDialog(); return false;", this.ClientID));
			}
			else
			{
				this.OnClientBeforeStart = string.Empty;
				this.OnClientCompleted = string.Empty;
			}

			base.OnPreRender(e);
		}

		protected override void LoadViewState(object savedState)
		{
			HiddenButtonWrapper bw = GetPropertyValue("ButtonWrapper", (HiddenButtonWrapper)null);

			if (bw != null)
				buttonWrapper.TargetControlID = bw.TargetControlID;

			base.LoadViewState(savedState);
		}

		protected override object SaveViewState()
		{
			SetPropertyValue("ButtonWrapper", this.buttonWrapper);

			return base.SaveViewState();
		}

		protected override void InitDialogContent(Control container)
		{
			this.Page.Form.Enctype = "multipart/form-data";
			this.Page.Form.Target = "_innerFrame";

			base.InitDialogContent(container);

			if (this.UploadProgressContentInited != null)
			{
				UploadProgressContentEventArgs eventArgs = new UploadProgressContentEventArgs(container);

				this.UploadProgressContentInited(this, eventArgs);
			}
		}

		protected override void InitConfirmButton(HtmlInputButton confirmButton)
		{
			base.InitConfirmButton(confirmButton);

			confirmButton.Style["width"] = "80px";
			confirmButton.Value = Translator.Translate(Define.DefaultCulture, "上传(U)");
			confirmButton.Attributes["accesskey"] = "U";
			confirmButton.Attributes["onclick"] = "onUploadButtonClick();";
		}

		private void ProcessUploadFile()
		{
			HttpResponse response = HttpContext.Current.Response;
			HttpRequest request = HttpContext.Current.Request;

			try
			{
				ExceptionHelper.FalseThrow(request.Files.Count > 0 && request.Files[0].ContentLength > 0,
					Translator.Translate(Define.DefaultCulture, "请选择一个上传文件"));

				ProcessProgress.Current.RegisterResponser(UploadProgressResponser.Instance);

				this.PostedData = request.Form["postedData"];

				response.Buffer = false;
				response.BufferOutput = false;
				UploadProgressResult result = new UploadProgressResult();

				if (DoUploadProgress != null)
					DoUploadProgress(request.Files[0], result);

				result.Response();
			}
			catch (System.Exception ex)
			{
				response.Write(string.Format("<script type=\"text/javascript\">top.document.getElementById(\"resetInterfaceButton\").click();</script>"));

				WebUtility.ResponseShowClientErrorScriptBlock(ex.Message, ex.StackTrace,
					Translator.Translate(Define.DefaultCulture, "错误"));
			}
			finally
			{
				response.End();
			}
		}
	}
}
