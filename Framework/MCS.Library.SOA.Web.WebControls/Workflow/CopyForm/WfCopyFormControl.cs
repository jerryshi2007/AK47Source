using System;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using System.ComponentModel;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Library.SOA.DataObjects;
using System.Web;
using System.Reflection;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 表单复制的控件，复制表单并启动一个新的流程
	/// </summary>
	[ToolboxData("<{0}:WfCopyFormControl runat=server></{0}:WfCopyFormControl>")]
	public class WfCopyFormControl : Control, INamingContainer
	{
		private HiddenButtonWrapper buttonWrapper = new HiddenButtonWrapper();

		#region Properties
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
		private bool NeedToRender
		{
			get
			{
				bool result = this.Visible && (WfClientContext.Current.OriginalActivity != null);

				if (result)
				{
					if (this.Visible && WfClientContext.Current.OriginalActivity != null)
					{
						IWfActivity initActivity = WfClientContext.Current.OriginalActivity.Process.ApprovalRootProcess.InitialActivity;

						result = initActivity.Status == WfActivityStatus.Completed || initActivity.Status == WfActivityStatus.Aborted;
					}
				}

				return result;
			}
		}
		#endregion Properties

		#region Protected
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

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			InitTargetControl(this.buttonWrapper.TargetControl);

			RegisterOpenFormScripts(this.Page);
		}
		#endregion

		#region Private
		private static void RegisterOpenFormScripts(Page page)
		{
			System.Type type = typeof(WfCopyFormControl);

			page.ClientScript.RegisterClientScriptBlock(type,
				"WfCopyFormControl",
				Assembly.GetExecutingAssembly().LoadStringFromResource(type.Namespace + ".Workflow.CopyForm.openFormScript.js"),
				true);
		}

		private void InitTargetControl(IAttributeAccessor target)
		{
			if (NeedToRender == false)
			{
				target.SetAttribute("disabled", "true");
				target.SetAttribute("class", "disable");
				target.SetAttribute("onclick", "return false;");
			}
			else
			{
				InitCopyFormTargetControl(target);
				target.SetAttribute("class", "enable");
			}
		}

		private void InitCopyFormTargetControl(IAttributeAccessor target)
		{
			string resourceID = WfClientContext.Current.OriginalActivity.Process.ResourceID;
			string processDescKey = WfClientContext.Current.OriginalActivity.RootActivity.Descriptor.Process.Key;
			string appName = WfClientContext.Current.OriginalActivity.Process.Descriptor.ApplicationName;
			string programName = WfClientContext.Current.OriginalActivity.Process.Descriptor.ProgramName;

			target.SetAttribute("resourceID", resourceID);
			target.SetAttribute("feature", GetWindowFeature().ToWindowFeatureClientString());

			ResourceUriSettings settings = ResourceUriSettings.GetConfig();

			string path = WfClientContext.Current.EntryUri.ToString();

			target.SetAttribute("href",
				string.Format(path + "?processDescKey={0}&sourceResourceID={1}&appName={2}&programName={3}",
				HttpUtility.UrlEncode(processDescKey),
				HttpUtility.UrlEncode(resourceID),
				HttpUtility.UrlEncode(appName),
				HttpUtility.UrlEncode(programName)));

			target.SetAttribute("target", "_blank" + resourceID.Replace("-", string.Empty));
			target.SetAttribute("onclick", "onOpenFormButtonClick();top.$HBRootNS.WfProcessControlBase.close();");
		}

		private static WindowFeature GetWindowFeature()
		{
			WindowFeature feature = null;
			if (ResourceUriSettings.GetConfig().Features.ContainsKey("genericProcess"))
			{
				feature = ResourceUriSettings.GetConfig().Features["genericProcess"].Feature;
			}
			else
			{
				feature = new WindowFeature();

				feature.Width = 800;
				feature.Height = 650;
				feature.Resizable = true;
				feature.ShowAddressBar = false;
				feature.ShowMenuBar = false;
				feature.Center = true;
				feature.ShowStatusBar = false;
			}

			return feature;
		}
		#endregion
	}
}
