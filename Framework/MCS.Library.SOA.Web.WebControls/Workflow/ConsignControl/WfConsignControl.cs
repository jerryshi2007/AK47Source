using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using MCS.Web.Library.Script;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 会签控件
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	//[RequiredScript(typeof(WfNextStep), 8)]
	[ClientScriptResource("MCS.Web.WebControls.WfConsignControl", "MCS.Web.WebControls.Workflow.Abstract.WfControls.js")]
	[ToolboxData("<{0}:ConsignControl runat=server></{0}:ConsignControl>")]
	public class WfConsignControl : ScriptControlBase, INamingContainer
	{
		private HiddenButtonWrapper buttonWrapper = new HiddenButtonWrapper();
		private ExtConsignUserSelector userSelector = new ExtConsignUserSelector() { InvokeWithoutViewState = true };

		public WfConsignControl()
			: base(true, HtmlTextWriterTag.Div)
		{
		}

		#region properties
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

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("userSelectorClientID")]
		private string UserSelectorClientID
		{
			get
			{
				return this.userSelector.ClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("moveToControlClientID")]
		private string MoveToControlClientID
		{
			get
			{
				string result = string.Empty;

				if (WfMoveToControl.Current != null)
					result = WfMoveToControl.Current.ClientID;

				return result;
			}
		}

		[Browsable(false)]
		private bool NeedToRender
		{
			get
			{
				bool result = this.Visible && !this.ReadOnly;

				if (result)
				{
					IWfActivity originalActivity = WfClientContext.Current.OriginalActivity;

					if (originalActivity != null)
					{
						result = originalActivity.Process.Status == WfProcessStatus.Running &&
							originalActivity.Status == WfActivityStatus.Running &&
							originalActivity.Descriptor.Properties.GetValue("AllowAddConsignApprover", false);

						if (result)
							result = WfClientContext.Current.InMoveToMode;
					}
				}

				return result;
			}
		}
		#endregion properties

		#region protected
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (Page.IsCallback)
				EnsureChildControls();
		}

		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			EnsureChildControls();

			base.OnPagePreLoad(sender, e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (DesignMode == false)
			{
				ExceptionHelper.FalseThrow(WfMoveToControl.Current != null,
					"WfConsignControl控件必须和WfExtMoveToControl一起使用");

				this.userSelector.DialogTitle = string.Format("请选择处理人");
			}

			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode)
				writer.Write(string.Format("<img src=\"{0}\"/><span>{1}</span>",
					Page.ClientScript.GetWebResourceUrl(typeof(WfProcessActivitiesListControl),
						"MCS.Web.WebControls.Resources.Images.activity.gif"),
					HttpUtility.HtmlEncode("会签")));
			else
				base.Render(writer);
		}

		protected override void CreateChildControls()
		{
			if (DesignMode == false)
			{
				this.userSelector.ID = "userSelector";

				this.userSelector.SelectMask = UserControlObjectMask.User | UserControlObjectMask.Sideline;
				this.userSelector.ListMask = UserControlObjectMask.User | UserControlObjectMask.Sideline | UserControlObjectMask.Organization;
				this.userSelector.MultiSelect = true;
				this.userSelector.IsConsign = true;

				Controls.Add(this.userSelector);

				InitHiddenButton();
			}

			base.CreateChildControls();
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

		protected override void OnPagePreRenderComplete(object sender, EventArgs e)
		{
			base.OnPagePreRenderComplete(sender, e);

			if (RenderMode.OnlyRenderSelf == false)
				InitTargetControl(this.buttonWrapper.TargetControl);
		}

		#endregion protected

		#region private
		private void InitHiddenButton()
		{
			this.buttonWrapper.CreateHiddenButton("innerBtn", null);
		}

		private void InitTargetControl(IAttributeAccessor target)
		{
			if (target != null)
			{
				target.SetAttribute("onclick", string.Format("event.returnValue = false; if (!this.disabled) $find(\"{0}\")._doOperation();return false;", this.ClientID));
				target.SetAttribute("class", "enable");

				if (NeedToRender == false)
				{
					//target.SetAttribute("disabled", "true");
					target.SetAttribute("class", "invisible");
				}
			}
		}
		#endregion private
	}
}
