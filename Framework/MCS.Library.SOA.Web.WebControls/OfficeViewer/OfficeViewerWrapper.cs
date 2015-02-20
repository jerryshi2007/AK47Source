using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MCS.Web.Library.Script;
using System.ComponentModel;
using MCS.Library.Core;
using MCS.Web.Library;

[assembly: WebResource("MCS.Web.WebControls.OfficeViewer.OfficeViewerWrapper.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// Office Viewer的包装类
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.OfficeViewerWrapper", "MCS.Web.WebControls.OfficeViewer.OfficeViewerWrapper.js")]
	[ToolboxData("<{0}:OfficeViewerWrapper runat=server></{0}:OfficeViewerWrapper>")]
	public class OfficeViewerWrapper : ScriptControlBase, INamingContainer
	{
		public OfficeViewerWrapper()
			: base(true, HtmlTextWriterTag.Div)
		{
		}

		#region Properties
		/// <summary>
		/// 是否自动打开默认的url
		/// </summary>
		[ScriptControlProperty, ClientPropertyName("autoOpenDefaultUrl")]
		[DefaultValue(true)]
		public bool AutoOpenDefaultUrl
		{
			get
			{
				return GetPropertyValue("AutoOpenDefaultUrl", true);
			}
			set
			{
				SetPropertyValue("AutoOpenDefaultUrl", value);
			}
		}

		[ScriptControlProperty, ClientPropertyName("showToolbars")]
		[DefaultValue(true)]
		public bool ShowToolbars
		{
			get
			{
				return GetPropertyValue("ShowToolbars", true);
			}
			set
			{
				SetPropertyValue("ShowToolbars", value);
			}
		}

		[ScriptControlProperty, ClientPropertyName("defaultOpenUrl")]
		[DefaultValue("")]
		public string DefaultOpenUrl
		{
			get
			{
				return GetPropertyValue("DefaultOpenUrl", string.Empty);
			}
			set
			{
				SetPropertyValue("DefaultOpenUrl", value);
			}
		}

		[ScriptControlProperty, ClientPropertyName("absoluteDefaultOpenUrl"), Browsable(false)]
		public string AbsoluteDefaultOpenUrl
		{
			get
			{
				string result = DefaultOpenUrl;

				if (result.IsNotEmpty())
				{
					Uri uri = UriHelper.ResolveUri(DefaultOpenUrl);

					if (uri.IsAbsoluteUri == false)
						uri = new Uri(HttpContext.Current.Request.Url, uri);

					result = uri.ToString();
				}

				return result;
			}
		}

		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("documentOpened")]
		[Bindable(true), Category("ClientEventsHandler"), Description("文档打开以后")]
		public string OnDocumentOpened
		{
			get
			{
				return GetPropertyValue("OnDocumentOpened", string.Empty);
			}
			set
			{
				SetPropertyValue("OnDocumentOpened", value);
			}
		}

		[ScriptControlProperty, ClientPropertyName("viewerControlID"), Browsable(false)]
		private string ViewerControlID
		{
			get
			{
				return this.ClientID + "_" + "Viewer";
			}
		}
		#endregion

		protected override void CreateChildControls()
		{
			HtmlGenericControl activeX = new HtmlGenericControl("object");

			activeX.Attributes["classid"] = OfficeViewerWrapperSettings.GetConfig().ClassID;
			activeX.Attributes["id"] = ViewerControlID;
			activeX.Attributes["codebase"] = OfficeViewerWrapperSettings.GetConfig().Codebase;
			activeX.Attributes["width"] = "100%";
			activeX.Attributes["height"] = "100%";

            activeX.Controls.Add(CreateParamElement("LicenseName", OfficeViewerWrapperSettings.GetConfig().LicenseName));
            activeX.Controls.Add(CreateParamElement("LicenseCode", OfficeViewerWrapperSettings.GetConfig().LicenseCode));
			activeX.Controls.Add(CreateParamElement("Toolbars", ShowToolbars ? -1 : 0));

			this.Controls.Add(activeX);

			base.CreateChildControls();
		}

		protected override void OnPreRender(EventArgs e)
		{
			RegisterClientEventHandler("NotifyCtrlReady", GetOpenFileScript());
			RegisterClientEventHandler("DocumentOpened()", GetFileOpenedScript());

			base.OnPreRender(e);
		}

		private string GetOpenFileScript()
		{
			string result = string.Empty;

			if (this.AutoOpenDefaultUrl && this.AbsoluteDefaultOpenUrl.IsNotEmpty())
			{

                result = "(function () { var url = '{0}'; var ctlID = '{1}'; var ctrl = $get(ctlID); ";
                //result += "  $get(ctlID).LicenseName = '{2}'; ";
                //result += "  $get(ctlID).LicenseCode = '{3}'; ";
                //result += "  if(url.endsWith('.xls')||url.endsWith('.xlsx')){";
                ////result += "     ctrl.CreateNew('Excel.Application'); ";
                //result += "  }";
			    result += "  var path = ctrl.HttpDownloadFileToTempDir(url); ";
                result += "  window.setTimeout(function() { ctrl.open(path);},100); ";
                //result += "  ctrl.SetAppFocus();";
                //result += "  if(url.endsWith('.xls')||url.endsWith('.xlsx')){";
                //result += "    if(ctrl.GetApplication().ActiveSheet) {";
                //result += "	     alert(0);  ctrl.GetApplication().ActiveSheet.Activate;";
                //result += "    }";
                //result += "  }";
				result += "})();";
				result = result.Replace("{0}", WebUtility.CheckScriptString(this.AbsoluteDefaultOpenUrl));
				result = result.Replace("{1}", WebUtility.CheckScriptString(this.ViewerControlID));
                //result = result.Replace("{2}", OfficeViewerWrapperSettings.GetConfig().LicenseName);
                //result = result.Replace("{3}", OfficeViewerWrapperSettings.GetConfig().LicenseCode);
			}

			return result;
		}

		private string GetFileOpenedScript()
		{
			string result = string.Empty;

            result = "window.setTimeout(function() { $find(\"{0}\").raiseDocumentOpened();}, 0);$get(\"{1}\").SetAppFocus(); ";
			result = result.Replace("{0}", WebUtility.CheckScriptString(this.ClientID));
			result = result.Replace("{1}", WebUtility.CheckScriptString(this.ViewerControlID));

			return result;
		}

		private static HtmlGenericControl CreateParamElement<T>(string name, T data)
		{
			HtmlGenericControl param = new HtmlGenericControl("param");

			param.Attributes["name"] = name;
			param.Attributes["value"] = data.ToString();

			return param;
		}

		private void RegisterClientEventHandler(string activeXEventName, string script)
		{
			StringBuilder result = new StringBuilder();

			result.AppendFormat("<script language=\"javascript\" for=\"{0}\" event=\"{1}\">\n",
				this.ViewerControlID,
				activeXEventName);

			result.Append(script);
			result.Append("\n</script>\n");

			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.ClientID + "_" + activeXEventName, result.ToString());
		}
	}
}
