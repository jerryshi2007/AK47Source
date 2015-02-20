using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.HtmlControls;

using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Library.Globalization;

namespace MCS.Web.WebControls
{
	public abstract class WfProcessControlBase : WfControlBase, INamingContainer
	{
		private HiddenButtonWrapper buttonWrapper = new HiddenButtonWrapper();

		protected WfProcessControlBase()
			: base()
		{
		}

		#region Properties
		/// <summary>
		/// 流程ID
		/// </summary>
		[DefaultValue("")]
		[Category("文档")]
		[Description("流程ID")]
		public string ProcessID
		{
			get
			{
				return GetPropertyValue("ProcessID", string.Empty);
			}
			set
			{
				SetPropertyValue("ProcessID", value);
			}
		}

		/// <summary>
		/// 目标控件的ID。目标控件通常是一个Button(客户端和服务器端)，由目标控件来触发流程操作。
		/// </summary>
		[DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(DialogStartUpControlConverter))]
		[Category("Appearance")]
		public string TargetControlID
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
		/// 目标控件实例。通常由目标控件的ID来计算出实例
		/// </summary>
		[Browsable(false)]
		public IAttributeAccessor TargetControl
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
		/// 内部隐藏的Button的ClientID
		/// </summary>
		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("innerButtonClientID")]
		public string InnerButtonClientID
		{
			get
			{
				return buttonWrapper.HiddenButtonClientID;
			}
		}
		#endregion

		#region Protected

		protected override void OnInit(EventArgs e)
		{
			WfControlHelperExt.InitWfControlPostBackErrorHandler(this.Page);
			base.OnInit(e);
		}

		/// <summary>
		/// 当前流程的实例对象。该实例从ProcessContext或ProcessID属性中恢复
		/// </summary>
		protected IWfProcess CurrentProcess
		{
			get
			{
				IWfProcess result = null;

				if (string.IsNullOrEmpty(ProcessID) == false)
				{
					result = WfRuntime.GetProcessByProcessID(ProcessID);
				}
				else
				{
					try
					{
						if (WfClientContext.Current.OriginalActivity != null)
							result = WfClientContext.Current.OriginalActivity.Process;
					}
					catch (System.Exception)
					{
					}
				}

				return result;
			}
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);

			this.buttonWrapper = (HiddenButtonWrapper)ViewState["ButtonWrapper"];
		}

		protected override object SaveViewState()
		{
			ViewState["ButtonWrapper"] = this.buttonWrapper;
			return base.SaveViewState();
		}

		/// <summary>
		/// 在Load的过程中，注册一个iframe来处理回调操作
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Page.ClientScript.RegisterClientScriptBlock(typeof(WfControlBase),
				"wfExternalFrame",
				"<div id='wfExternalFrameContainer'><iframe id='wfExternalFrame' name='wfExternalFrame' style='display:none'></iframe></div>");
		}

		protected override void CreateChildControls()
		{
			InitHiddenButton();
			base.CreateChildControls();
		}

		protected virtual void InitHiddenButton()
		{
			this.buttonWrapper.CreateHiddenButton("innerBtn", new EventHandler(innerButton_Click));

			OnCreateHiddenButton(this.buttonWrapper);

			this.Controls.Add(this.buttonWrapper.HiddenButton);
		}

		/// <summary>
		/// 根据当前控件的Visible属性，设置目标控件是否可显示
		/// </summary>
		/// <param name="target"></param>
		protected virtual void SetTargetControlVisible(Control target)
		{
			target.Visible = this.Visible;
		}

		protected virtual void OnCreateHiddenButton(HiddenButtonWrapper buttonWrapper)
		{
		}

		protected virtual void OnSuccess()
		{
			HttpContext.Current.Response.Write(ExtScriptHelper.GetRefreshBridgeScript());
		}

		protected virtual void OnError(System.Exception ex, ref bool reThrow)
		{
		}

		/// <summary>
		/// 创建一个执行器
		/// </summary>
		/// <returns></returns>
		protected abstract WfExecutorBase CreateExecutor();
		#endregion

		#region Private
		private void innerButton_Click(object sender, EventArgs e)
		{
			StringBuilder strScript = new StringBuilder();

			try
			{
				WfExecutorBase executor = CreateExecutor();

				ExceptionHelper.FalseThrow(WfClientContext.Current.HasProcessChanged == false,
					Translator.Translate(Define.DefaultCulture,
						"当前流程的状态已经改变，不能{0}",
						Translator.Translate(Define.DefaultCulture, EnumItemDescriptionAttribute.GetDescription(executor.OperationType))));

				OnAfterCreateExecutor(executor);
				WfClientContext.Current.Execute(executor);

				OnSuccess();

				ResponsePostBackPlaceHolder();
			}
			catch (System.Exception ex)
			{
				ResponsePostBackPlaceHolder();

				bool reThrow = true;
				OnError(ex, ref reThrow);

				if (reThrow)
				{
					System.Exception innerEx = ExceptionHelper.GetRealException(ex);

					WebUtility.ResponseShowClientErrorScriptBlock(innerEx.Message, innerEx.StackTrace, Translator.Translate(Define.DefaultCulture, "错误"));
				}
			}
			finally
			{
				strScript.Insert(0, "if (parent.document.all('wfOperationNotifier')) parent.document.all('wfOperationNotifier').value = '';");

				WebUtility.ResponseTimeoutScriptBlock(strScript.ToString(), ExtScriptHelper.DefaultResponseTimeout);

				Page.Response.End();
			}
		}

		private void InitProcessControl(IAttributeAccessor target)
		{
			if (target != null)
			{
				target.SetAttribute("onclick", string.Format("if (!event.srcElement.disabled) $find('{0}').doInternalOperation(); return false;", this.ClientID));

				SetTargetControlVisible((Control)target);
			}
		}
		#endregion

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("moveToControlClientID")]
		protected virtual string MoveToControlClientID
		{
			get
			{
				return GetPropertyValue("MoveToControlClientID", string.Empty);
			}
			set
			{
				SetPropertyValue("MoveToControlClientID", value);
			}
		}

		protected override void OnPagePreRenderComplete(object sender, EventArgs e)
		{
			base.OnPagePreRenderComplete(sender, e);

			if (RenderMode.OnlyRenderSelf == false)
				InitProcessControl(this.buttonWrapper.TargetControl);

			if (WfMoveToControl.Current != null)
			{
				this.MoveToControlClientID = WfMoveToControl.Current.ClientID;
			}
		}
	}
}
