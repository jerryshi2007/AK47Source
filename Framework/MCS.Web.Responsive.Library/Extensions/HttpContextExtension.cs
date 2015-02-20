using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Web.Responsive.Library.Resources;
using MCS.Web.Library;

namespace MCS.Web.Responsive.Library
{
    /// <summary>
    /// HttpContext的扩展方法
    /// </summary>
    public static class HttpContextExtension
    {
        private const string _CurrentPageKey = "DeluxeCurrentPageKey";

        /// <summary>
        /// 从HttpContext中获取当前页
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns></returns>
        public static Page GetCurrentPage(this HttpContext context)
        {
            context.NullCheck("context");

            Page page = (Page)context.Items[_CurrentPageKey];

            if (page == null)
                page = context.CurrentHandler as Page;

            return page;
        }

        /// <summary>
        /// 在HttpContext中设置当前页
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="page"></param>
        public static void SetCurrentPage(this HttpContext context, Page page)
        {
            context.NullCheck("context");

            context.Items[_CurrentPageKey] = page;
        }

        /// <summary>
        /// 向页面添加配置扩展信息
        /// </summary>
        /// <param name="context">HttpContext</param>
        public static void LoadConfigPageContent(this HttpContext context)
        {
            LoadConfigPageContent(context, false);
        }

        /// <summary>
        /// 向页面添加配置扩展信息
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="checkAutoLoad"></param>
        public static void LoadConfigPageContent(this HttpContext context, bool checkAutoLoad)
        {
            if (context != null)
            {
                PageContentSection section = ConfigSectionFactory.GetPageExtensionSection();

                Page page = context.GetCurrentPage();

                if (page != null)
                {
                    if (checkAutoLoad)
                    {
                        if (section.AutoLoad == false)
                            return;

                        if (page.Header == null)
                            return;

                        string headerAutoLoad = page.Header.Attributes["autoLoad"];

                        if (headerAutoLoad.IsNotEmpty() && headerAutoLoad.ToLower() == "false")
                            return;
                    }

                    if (WebAppSettings.IsWebApplicationCompilationDebug())
                        RegisterCssInHeader(page, section.CssClasses.Debug);
                    else
                        RegisterCssInHeader(page, section.CssClasses.Release);

                    RegisterCssInHeader(page, section.CssClasses.All);

                    if (WebAppSettings.IsWebApplicationCompilationDebug())
                        RegisterScriptInHeader(page, section.Scripts.Debug);
                    else
                        RegisterScriptInHeader(page, section.Scripts.Release);

                    RegisterScriptInHeader(page, section.Scripts.All);
                }
            }
        }

        private static void RegisterCssInHeader(Page page, FilePathConfigElementCollection urls)
        {
            foreach (FilePathConfigElement cssElement in urls)
            {
                string path = cssElement.Path;

                if (path.IsNotEmpty())
                    ClientCssManager.RegisterHeaderEndCss(page, path);
            }
        }

        private static void RegisterScriptInHeader(Page page, FilePathConfigElementCollection urls)
        {
            foreach (FilePathConfigElement cssElement in urls)
            {
                string path = cssElement.Path;

                if (path.IsNotEmpty())
                    DeluxeClientScriptManager.RegisterHeaderEndScript(page, path);
            }
        }

        /// <summary>
        /// 如果当前的HtppHandler是Page，判断它是否是PostBack状态
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>是否是PostBack状态</returns>
        public static bool IsCurrentHandlerPostBack(this HttpContext context)
        {
            bool result = false;

            if (context != null && context.CurrentHandler is Page)
            {
                Page page = (Page)context.CurrentHandler;

                PropertyInfo pi = page.GetType().GetProperty("IsPostBack", BindingFlags.Instance | BindingFlags.Public);

                if (pi != null)
                    result = (bool)pi.GetValue(page, null);
            }

            return result;
        }

        /// <summary>
        /// 如果当前的HtppHandler是Page，判断它是否是Callback状态
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>是否是Callback状态</returns>
        public static bool IsCurrentHandlerIsCallback(this HttpContext context)
        {
            bool result = false;

            if (context != null && context.CurrentHandler is Page)
            {
                Page page = (Page)context.CurrentHandler;

                PropertyInfo pi = page.GetType().GetProperty("IsCallback", BindingFlags.Instance | BindingFlags.Public);

                if (pi != null)
                    result = (bool)pi.GetValue(page, null);
            }

            return result;
        }

        /// <summary>
        /// 如果当前的HtppHandler是Page，那么找到其ViewState属性，获取其中的值
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="key">ViewState的key</param>
        /// <returns>ViewState中的对象</returns>
        public static object LoadViewStateFromCurrentHandler(this HttpContext context, string key)
        {
            context.NullCheck("context");
            ExceptionHelper.CheckStringIsNullOrEmpty(key, "key");

            object result = null;

            if (context.CurrentHandler is Page)
            {
                Page page = (Page)context.CurrentHandler;

                PropertyInfo pi = page.GetType().GetProperty("ViewState", BindingFlags.Instance | BindingFlags.NonPublic);

                if (pi != null)
                {
                    StateBag vs = (StateBag)pi.GetValue(page, null);

                    if (vs != null)
                        result = vs[key];
                }
            }

            return result;
        }

        #region Private Methods
        private static readonly object ClientMessageCommonScriptKey = new object();

        internal static void ResponseClientMessageCommonScriptBlock()
        {
            StringBuilder strB = new StringBuilder();

            PageContentModule.RegisterDefaultNameTable();

            strB.AppendFormat("<script type='text/javascript'>\n{0}\n</script>", DeluxeNameTableContextCache.Instance.GetNameTableScript());
            strB.Append("\n");

            strB.Append(WebUtility.GetRequiredScript(typeof(ClientMsgResources)));
            strB.Append("\n");

            ResponseString(HttpContext.Current, ClientMessageCommonScriptKey, strB.ToString());
        }

        private static void RequireWindowCommandScript()
        {
            WebUtility.RequiredScript(typeof(DeluxeScript));
            string script = string.Format("$HGRootNS.WindowCommand.set_commandInputID('{0}');", DeluxeScript.C_CommandIputClientID);
            script = DeluxeClientScriptManager.AddScriptTags(script);
            Page page = WebUtility.GetCurrentPage();
            page.ClientScript.RegisterStartupScript(page.GetType(), "RequireWindowCommandScript", script);
        }

        private static readonly object RequireWindowCommandScriptKey = new object();

        internal static void ResponseRequireWindowCommandScriptBlock()
        {
            StringBuilder strB = new StringBuilder();
            strB.Append(WebUtility.GetRequiredScript(typeof(DeluxeScript)));
            strB.Append("\n");
            string script = string.Format("$HGRootNS.WindowCommand.set_commandInputID('{0}');", DeluxeScript.C_CommandIputClientID);
            strB.Append(DeluxeClientScriptManager.AddScriptTags(script));
            strB.Append("\n");

            ResponseString(HttpContext.Current, RequireWindowCommandScriptKey, strB.ToString());
        }

        /// <summary>
        /// 如果当前的HtppHandler是Page，那么将数据存入到ViewState中
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="key">ViewState的键值</param>
        /// <param name="data">需要存入的数据</param>
        public static void SaveViewStateToCurrentHandler(HttpContext context, string key, object data)
        {
            context.NullCheck("context");
            ExceptionHelper.CheckStringIsNullOrEmpty(key, "key");

            if (context.CurrentHandler is Page)
            {
                Page page = (Page)context.CurrentHandler;

                PropertyInfo pi = page.GetType().GetProperty("ViewState", BindingFlags.Instance | BindingFlags.NonPublic);

                if (pi != null)
                {
                    StateBag vs = (StateBag)pi.GetValue(page, null);

                    if (vs != null)
                        vs[key] = data;
                }
            }
        }

        private static readonly object ResponseStringKey = new object();

        private static void ResponseString(HttpContext context, object key, string str)
        {
            if (context.Items.Contains(ResponseStringKey) == false)
                context.Items.Add(ResponseStringKey, new Dictionary<object, object>());

            Dictionary<object, object> dict = (Dictionary<object, object>)context.Items[ResponseStringKey];

            if (!dict.ContainsKey(key))
            {
                dict.Add(key, null);
                HttpContext.Current.Response.Write(str);
            }
        }
        #endregion Private Methods
    }
}
