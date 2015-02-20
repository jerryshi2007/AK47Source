using System;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Library.Principal;
using System.Web;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.WebControls.MaterialControl.DialogFileOpenTypeControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.Images.computer.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.inlineDemo.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.openType.gif", "image/gif")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 显示附件上传的页面
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.DialogFileOpenTypeControl",
	   "MCS.Web.WebControls.MaterialControl.DialogFileOpenTypeControl.js")]
	[ParseChildren(true), PersistChildren(true)]
	internal class DialogFileOpenTypeControl : ScriptControlBase, INamingContainer
	{
		const string AppName = "HB2008Portal";
		const string ProgName = "Portal";
		const string PropName = "FileOpenWay";
        const string Categoryname = "CommonSettings";
		public DialogFileOpenTypeControl()
			: base(true, System.Web.UI.HtmlTextWriterTag.Span)
		{
		
		}

		[ScriptControlProperty, ClientPropertyName("computerImagePath"), Browsable(false)]
		private string ComputerImagePath
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.DialogFileOpenTypeControl),
					"MCS.Web.WebControls.Images.computer.gif");
			}
		}

		[ScriptControlProperty, ClientPropertyName("currentFileExtensionNames"), Browsable(false)]
		private string CurrentFileExtensionNames
		{
			get
			{
				UserSettings settings = UserSettings.GetSettings(DeluxeIdentity.CurrentRealUser.ID);

				return settings.GetPropertyValue(AppName, ProgName, PropName).ToString();
			}
		}

		[ScriptControlProperty, ClientPropertyName("inlineDemoImagePath"), Browsable(false)]
		private string InlineDemoImagePath
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.DialogFileOpenTypeControl),
					"MCS.Web.WebControls.Images.inlineDemo.gif");
			}
		}

		[ScriptControlProperty, ClientPropertyName("openTypeImagePath"), Browsable(false)]
		private string OpenTypeImagePath
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.DialogFileOpenTypeControl),
					"MCS.Web.WebControls.Images.openType.gif");
			}
		}
	 
		/// <summary>
		/// 设置
		/// </summary>
		/// <param name="fileExtensionNames">拓展名</param>
		/// <returns></returns>
		[ScriptControlMethod]
		public string SetOpenInLineFileExtensionNames(string userID, string fileExtensionNames)
		{
			UserSettings settings = UserSettings.GetSettings(userID);

			//settings.SetPropertyValue(AppName, ProgName, PropName, fileExtensionNames);
            //settings.
            settings.Categories[Categoryname].Properties[PropName].StringValue = fileExtensionNames;
            //settings.Categories[Categoryname].AllProperties[PropName]
			//settings.Save();
            settings.Update();
			return fileExtensionNames;
		}
		 
		/// <summary>
		/// 获得默认值
		/// </summary>
		/// <returns></returns>
		[ScriptControlMethod]
		public string GetDefaultValue()
		{
			//object defaultValue = UserSettingsConfig.GetConfig().Applications[AppName].Programs[ProgName].SettingProperties[PropName].DefaultValue;
            object defaultValue = UserSettingsConfig.GetConfig().Categories[Categoryname].AllProperties[PropName].DefaultValue;
			if (defaultValue != null)
				return defaultValue.ToString();
			else
				return string.Empty;
		}
	}
}