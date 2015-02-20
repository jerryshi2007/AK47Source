using MCS.Library.Core;
using MCS.Web.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace MCS.Web.Responsive.Library
{
    /// <summary>
    /// 加载PageContentModule
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
            HttpContext.Current.LoadConfigPageContent(true);

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
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "提示");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "警告");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "错误");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "选择");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "点击此处关闭详细信息...");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "点击此处展开详细信息...");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "复制信息");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "确定(O)");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "取消(C)");
        }
    }
}
