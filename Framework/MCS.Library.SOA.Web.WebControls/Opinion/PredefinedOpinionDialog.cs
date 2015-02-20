using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Web.Library.Script;
using MCS.Web.Library;
using System.Web.UI.HtmlControls;
using MCS.Library.SOA.DataObjects;
using System.ComponentModel;

[assembly: WebResource("MCS.Web.WebControls.Opinion.PredefinedOpinionDialog.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.PredefinedOpinionDialog",
		"MCS.Web.WebControls.Opinion.PredefinedOpinionDialog.js")]
	[DialogContent("MCS.Web.WebControls.Opinion.PredefinedOpinionDialog.htm", "MCS.Library.SOA.Web.WebControls")]
	[ToolboxData("<{0}:PredefinedOpinionDialog runat=server></{0}:PredefinedOpinionDialog>")]
	public class PredefinedOpinionDialog : DialogControlBase<PredefinedOpinionDialogParams>
	{
		//internal const string AppName = "OAPortal";
		internal const string CategoryName = "CommonSettings";
		internal const string SettingName = "CommonOpinion";

		private HtmlTextArea _OpinionText = null;

		#region Properties
		/// <summary>
		/// 用户ID
		/// </summary>
		public string UserID
		{
			get
			{
				return ControlParams.UserID;
			}
			set
			{
				ControlParams.UserID = value;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("opinionTextClientID")]
		private string OpinionTextClientID
		{
			get
			{
				string result = string.Empty;

				if (_OpinionText != null)
					result = _OpinionText.ClientID;

				return result;
			}
		}
		#endregion Properties

		#region Protected
		/// <summary>
		/// 根据UserID生成弹出对话框的url地址
		/// </summary>
		/// <returns></returns>
		protected override string GetDialogUrl()
		{
			PageRenderMode pageRenderMode = new PageRenderMode(this.UniqueID, "DialogControl");

			string url = WebUtility.GetRequestExecutionUrl(pageRenderMode, "userID");

			return url + "&" + this.ControlParams.ToRequestParams();
		}

		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			EnsureChildControls();
			base.OnPagePreLoad(sender, e);
		}

		/// <summary>
		/// 初始化对话框内容
		/// </summary>
		/// <param name="container"></param>
		protected override void InitDialogContent(Control container)
		{
			base.InitDialogContent(container);

			this.ID = "PredefinedOpinionDialog";

			_OpinionText = (HtmlTextArea)WebControlUtility.FindControlByHtmlIDProperty(container, "opinionText", true);
			InitOpinionInputBox(_OpinionText);
		}

		/// <summary>
		/// 获取对话框外观属性
		/// </summary>
		/// <returns></returns>
		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 400;
			feature.Height = 300;
			feature.Resizable = false;
			feature.ShowStatusBar = false;
			feature.ShowScrollBars = false;
			feature.Center = true;

			return feature.ToDialogFeatureClientString();
		}

		/// <summary>
		/// 初始化确认按钮
		/// </summary>
		/// <param name="confirmButton"></param>
		protected override void InitConfirmButton(HtmlInputButton confirmButton)
		{
			confirmButton.Attributes["onclick"] = "onDialogConfirm();";
		}
		#endregion Protected

		#region callback
		/// <summary>
		/// 页面回调公共方法
		/// </summary>
		/// <param name="predefinedOpinion">常用意见对象</param>
		[ScriptControlMethod]
		public void CallBackSavePredifinedOpinionMethod(string predefinedOpinion)
		{
			SavePredefinedOpinion(predefinedOpinion, PredefinedOpinionDialog.CategoryName, PredefinedOpinionDialog.SettingName);
		}
		#endregion

		#region
		protected override void OnPreRender(EventArgs e)
		{
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "内容不能超过200个字");

			base.OnPreRender(e);
		}
		#endregion

		#region Private
		/// <summary>
		/// 初始化用户输入框
		/// </summary>
		/// <param name="container"></param>
		private void InitOpinionInputBox(HtmlTextArea textArea)
		{
			string text =
				UserSettings.LoadSettings(this.UserID).GetPropertyValue<string>(
					PredefinedOpinionDialog.CategoryName,
					PredefinedOpinionDialog.SettingName,
					string.Empty
					);
			string[] textArray = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			StringBuilder strB = new StringBuilder();

			foreach (string t in textArray)
			{
				if (strB.Length > 0)
					strB.Append("\r\n");

				strB.Append(t);
			}

			_OpinionText.InnerText = strB.ToString();
		}

		/// <summary>
		/// 保存常用意见
		/// </summary>
		/// <param name="predefinedOpinion">常用意见对象</param>
		/// <param name="appName">应用名</param>
		/// <param name="programName">模块名</param>
		private void SavePredefinedOpinion(string predefinedOpinion, string categoryName, string settingName)
		{
			try
			{
				UserSettings us = UserSettings.GetSettings(this.UserID);
				us.Categories[categoryName].Properties[settingName].StringValue = predefinedOpinion;
				us.Update();
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}
		#endregion
	}
}
