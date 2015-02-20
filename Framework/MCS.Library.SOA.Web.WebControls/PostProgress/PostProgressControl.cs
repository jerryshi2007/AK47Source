using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Web.Library;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.PostProgress.PostProgressControl.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 将客户端提交的数据Post到服务端进行处理。适用于服务器端处理过程较长的操作
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.PostProgressControl",
		"MCS.Web.WebControls.PostProgress.PostProgressControl.js")]
	[DialogContent("MCS.Web.WebControls.PostProgress.PostProgressControlTemplate.htm", "MCS.Library.SOA.Web.WebControls")]
	[ToolboxData("<{0}:PostProgressControl runat=server></{0}:PostProgressControl>")]
	public class PostProgressControl : DialogControlBase<PostProgressControlParams>
	{
		private HiddenButtonWrapper buttonWrapper = new HiddenButtonWrapper();

		public PostProgressControl()
		{
			JSONSerializerExecute.RegisterConverter(typeof(ProcessProgressConverter));
		}

		#region Events
		[Description("准备数据，将JSON序列化的数据反序列化为对象数组")]
		public event PostProgressPrepareDataHandler PreapreData;

		[Description("处理数据")]
		public event PostProgressDoPostedDataEventHandler DoPostedData;

		#endregion Events

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
		/// 数据选择控件的ID，主要针对于Grid等能够选择数据的控件
		/// </summary>
		[DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(DialogStartUpControlConverter))]
		public string DataSelectorControlID
		{
			get
			{
				return GetPropertyValue("DataSelectorControlID", string.Empty);
			}
			set
			{
				SetPropertyValue("DataSelectorControlID", value);
			}
		}

		/// <summary>
		/// 数据选择控件的ID，主要针对于Grid等能够选择数据的控件
		/// </summary>
		[Browsable(false)]
		[ScriptControlProperty(), ClientPropertyName("dataSelectorControlClientID")]
		private string DataSelectorControlClientID
		{
			get
			{
				string result = null;

				if (this.Page != null && this.DataSelectorControlID.IsNotEmpty())
				{
					Control target = WebControlUtility.FindControlByID(this.Page, this.DataSelectorControlID, true);

					if (target != null)
						result = target.ClientID;
				}

				return result;
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
		/// 传递给客户端，再传递上来的额外参数。客户端可以修改此参数
		/// </summary>
		[Browsable(true)]
		[ScriptControlProperty(), ClientPropertyName("clientExtraPostedData")]
		public string ClientExtraPostedData
		{
			get
			{
				return GetPropertyValue("ClientExtraPostedData", string.Empty);
			}
			set
			{
				SetPropertyValue("ClientExtraPostedData", value);
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
			if (Page.Request.Form["uploadDataBtn"] == "Upload Data")
				ProcessUploadData();

			base.OnLoad(e);
		}

		protected override void InitDialogContent(Control container)
		{
			this.Page.Form.Target = "_innerFrame";

			base.InitDialogContent(container);
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
			{
				buttonWrapper.TargetControlID = bw.TargetControlID;
			}

			base.LoadViewState(savedState);
		}

		protected override object SaveViewState()
		{
			SetPropertyValue("ButtonWrapper", this.buttonWrapper);

			return base.SaveViewState();
		}

		protected override void InitConfirmButton(HtmlInputButton confirmButton)
		{
			confirmButton.Style["display"] = "none";
		}

		protected override void RenderContents(HtmlTextWriter output)
		{
			if (this.DesignMode)
				output.Write("PostProgressControl");
			else
				base.RenderContents(output);
		}

		protected virtual void OnPrepareData(object sender, PostProgressPrepareDataEventArgs eventArgs)
		{
			if (this.PreapreData != null)
				this.PreapreData(sender, eventArgs);
		}

		protected virtual void OnDoPostedData(object sender, PostProgressDoPostedDataEventArgs eventArgs)
		{
			if (this.DoPostedData != null)
				this.DoPostedData(sender, eventArgs);
		}

		private void ProcessUploadData()
		{
			HttpResponse response = HttpContext.Current.Response;
			HttpRequest request = HttpContext.Current.Request;

			try
			{
				ExceptionHelper.FalseThrow(request["postedData"].IsNotEmpty(),
					Translator.Translate(Define.DefaultCulture, "没有上传的数据"));

				ProcessProgress.Current.RegisterResponser(UploadProgressResponser.Instance);

				this.ClientExtraPostedData = request.Form["clientExtraPostedData"];

				PostProgressPrepareDataEventArgs prepareDataArgs = new PostProgressPrepareDataEventArgs() { SerializedData = request["postedData"] };

				OnPrepareData(this, prepareDataArgs);

				if (prepareDataArgs.DeserializedData == null)
					prepareDataArgs.DeserializedData = (IList)JSONSerializerExecute.DeserializeObject(prepareDataArgs.SerializedData);

				response.Buffer = false;
				response.BufferOutput = false;

				UploadProgressResult result = new UploadProgressResult();

				OnDoPostedData(this, new PostProgressDoPostedDataEventArgs() { Result = result, ClientExtraPostedData = this.ClientExtraPostedData, Steps = prepareDataArgs.DeserializedData });

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
