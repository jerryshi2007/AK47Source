using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.Script;
using MCS.Library.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Web.Library;

namespace MCS.Web.Library
{
	/// <summary>
	/// ����PageContentModule
	/// </summary>
	public class PageContentModule : BasePageModule
	{
		/// <summary>
		/// Init
		/// </summary>
		/// <param name="page"></param>
		protected override void Init(System.Web.UI.Page page)
		{
			page.Load += new EventHandler(PageLoadHandler);
			page.PreRenderComplete += new EventHandler(page_PreRenderComplete);
		}

		private void page_PreRenderComplete(object sender, EventArgs e)
		{
			RegisterDefaultNameTable();

			AutoEncryptControlValueHelper.EncryptControlsValue(
				ConfigSectionFactory.GetPageExtensionSection().AutoEncryptControlIDs.Split(new char[] { ',', ';' },
					StringSplitOptions.RemoveEmptyEntries), (Page)sender);

			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("PageContentModule-RecursiveTranslate",
				() => TranslatorControlHelper.RecursiveTranslate((Page)sender));

			DeluxeNameTableContextCache.Instance.RegisterNameTable((Page)sender);

			((Page)sender).ClientScript.RegisterClientScriptBlock(this.GetType(), "DeluxeApplicationPath", 
				string.Format("var _DeluxeApplicationPath = \"{0}\";", HttpContext.Current.Request.ApplicationPath), true);
		}

		private static void PageLoadHandler(object sender, EventArgs e)
		{
			WebUtility.LoadConfigPageContent(true);

			if (((Page)sender).IsPostBack)
			{
				AutoEncryptControlValueHelper.DecryptControlsValue(
					ConfigSectionFactory.GetPageExtensionSection().AutoEncryptControlIDs.Split(new char[] { ',', ';' },
						StringSplitOptions.RemoveEmptyEntries), (Page)sender);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void RegisterDefaultNameTable()
		{
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "��ʾ");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "����");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "����");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "ѡ��");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "����˴��ر���ϸ��Ϣ...");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "����˴�չ����ϸ��Ϣ...");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "������Ϣ");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "ȷ��(O)");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "ȡ��(C)");
		}
	}
}
