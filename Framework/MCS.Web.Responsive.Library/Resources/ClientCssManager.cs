using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Web.Responsive.Library.Script;

namespace MCS.Web.Responsive.Library
{
    /// <summary>
    ///
    /// </summary>
    public static class ClientHeaderContainerManager
    {
        private const string _C_HEADER_CONTAINER_ID = "_C_HEADER_CONTAINER_ID";

        public static Control GetContainerControl(Page page)
        {
            Control ctr = page.Header.FindControl(_C_HEADER_CONTAINER_ID);

            if (ctr == null)
            {
                ctr = new Control();
                ctr.ID = _C_HEADER_CONTAINER_ID;

                int position = FindProperlyPosition(page.Header.Controls);
                // 找一个合适的位置

                page.Header.Controls.AddAt(position, ctr);
            }

            return ctr;
        }

        /// <summary>
        /// 找一个合适的位置来放置样式表。
        /// </summary>
        /// <param name="controls">来自Header.Controls</param>
        /// <returns></returns>
        /// <remarks>
        /// CSS样式表通常有特定的加载顺序。为了防止css顺序混乱，需要采取一些特殊手法。
        /// 原则：1、建议link引用样式表应出现在任何title和meta元素之后。
        /// 2、必须首先加载的样式表，请为link使用topmost=""属性，并且顺序出现，不能间隔。
        /// 3、任何style和script元素应出现在link元素之后。
        /// </remarks>
        private static int FindProperlyPosition(ControlCollection controls)
        {
            int k = 0;
            Control ctrl;
            for (; k < controls.Count; k++)
            {
                ctrl = controls[k];
                if (ctrl is HtmlLink)
                {
                    HtmlLink link = (HtmlLink)ctrl;
                    if (string.Compare(link.Attributes["rel"], "stylesheet", true) == 0 || ((HtmlLink)ctrl).Attributes["type"] == "text/css")
                    {
                        if (link.Attributes["topmost"] == null)
                        {
                            break;
                        }
                    }
                }
                else if (ctrl is LiteralControl && (System.Text.RegularExpressions.Regex.IsMatch(((LiteralControl)ctrl).Text, "<script ") || System.Text.RegularExpressions.Regex.IsMatch(((LiteralControl)ctrl).Text, "<style ")))
                {
                    break;
                }
            }

            return k;
        }
    }

    /// <summary>
    /// 在页面中注册Css文件帮助函数
    /// </summary>
    /// <remarks>在页面中注册Css文件帮助函数</remarks>
    public static class ClientCssManager
    {
        private const string _C_CSS_CONTROL_ID = "__StyleSheetControlID";
        private const string _C_HEADER_END_CSS_CONTROL_ID = "__HeaderEndStyleSheetControlID";

        private static StyleSheetControl GetStyleSheetControl(Page page)
        {
            StyleSheetControl ctr = (StyleSheetControl)page.Header.FindControl(_C_CSS_CONTROL_ID);
            if (ctr == null)
            {
                ctr = new StyleSheetControl();
                ctr.ID = _C_CSS_CONTROL_ID;

                ClientHeaderContainerManager.GetContainerControl(page).Controls.AddAt(0, ctr);
            }

            return ctr;
        }

        private static StyleSheetControl GetHeaderEndStyleSheetControl(Page page)
        {
            StyleSheetControl ctr = (StyleSheetControl)page.Header.FindControl(_C_HEADER_END_CSS_CONTROL_ID);
            if (ctr == null)
            {
                ctr = new StyleSheetControl();
                ctr.ID = _C_HEADER_END_CSS_CONTROL_ID;

                ClientHeaderContainerManager.GetContainerControl(page).Controls.Add(ctr);
            }

            return ctr;
        }

        /// <summary>
        /// 在Head之间注册Css文件
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="cssUrl">css文件url路径</param>
        /// <remarks>在Head之间注册Css</remarks>
        public static void RegisterHeaderCss(Page page, string cssUrl)
        {
            StyleSheetControl ctr = GetStyleSheetControl(page);

            ctr.Add(cssUrl, StyleSheetPositionMode.Header);
        }

        /// <summary>
        /// 在Head结束前注册Css文件
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="cssUrl">css文件url路径</param>
        /// <remarks>在Head之间注册Css</remarks>
        public static void RegisterHeaderEndCss(Page page, string cssUrl)
        {
            StyleSheetControl ctr = GetHeaderEndStyleSheetControl(page);

            ctr.Add(cssUrl, StyleSheetPositionMode.Header);
        }

        /// <summary>
        /// 在body开始后注册Css文件
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="cssUrl">css文件url路径</param>
        /// <remarks>在body开始后注册Css</remarks>
        public static void RegisterBodyStartCss(Page page, string cssUrl)
        {
            StyleSheetControl ctr = GetStyleSheetControl(page);

            ctr.Add(cssUrl, StyleSheetPositionMode.BodyStart);
        }

        /// <summary>
        /// 在body结束前注册Css文件
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="cssUrl">css文件url路径</param>
        /// <remarks>在body结束前注册Css</remarks>
        public static void RegisterBodyEndCss(Page page, string cssUrl)
        {
            StyleSheetControl ctr = GetStyleSheetControl(page);
            ctr.Add(cssUrl, StyleSheetPositionMode.BodyEnd);
        }
    }

    /// <summary>
    /// 在页面中注册Script文件帮助函数
    /// </summary>
    /// <remarks>在页面中注册Script文件帮助函数</remarks>
    public static class DeluxeClientScriptManager
    {
        private const string _C_SCRIPT_CONTROL_ID = "__ScriptContainerControlID";
        private const string _C_HEADER_END_SCRIPT_CONTROL_ID = "__HeaderEndScriptContainerControlID";
        private const string _C_SCRIPT_BLOCK_FORMAT = "<script language=\"javascript\" type=\"text/javascript\">\n{0}\n</script>";
        private const string _C_SCRIPT_FORMAT = "<script type=\"text/javascript\" src=\"{0}\"></script>";

        private static ScriptContainerControl GetScriptContainerControl(Page page)
        {
            ScriptContainerControl ctr = (ScriptContainerControl)page.Header.FindControl(_C_SCRIPT_CONTROL_ID);
            if (ctr == null)
            {
                ctr = new ScriptContainerControl();
                ctr.ID = _C_SCRIPT_CONTROL_ID;

                ClientHeaderContainerManager.GetContainerControl(page).Controls.AddAt(0, ctr);
            }

            return ctr;
        }

        private static ScriptContainerControl GetHeaderEndScriptContainerControl(Page page)
        {
            ScriptContainerControl ctr = (ScriptContainerControl)page.Header.FindControl(_C_HEADER_END_SCRIPT_CONTROL_ID);
            if (ctr == null)
            {
                ctr = new ScriptContainerControl();
                ctr.ID = _C_HEADER_END_SCRIPT_CONTROL_ID;

                ClientHeaderContainerManager.GetContainerControl(page).Controls.Add(ctr);
            }

            return ctr;
        }

        /// <summary>
        /// 在Head之间注册Script文件
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="scriptUrl">script文件url路径</param>
        /// <remarks>在Head之间注册Script</remarks>
        public static void RegisterHeaderScript(Page page, string scriptUrl)
        {
            ScriptContainerControl ctr = GetScriptContainerControl(page);

            ctr.Add(scriptUrl, ScriptPositionMode.Header);
        }

        /// <summary>
        /// 在Head结束前注册Script文件
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="scriptUrl">script文件url路径</param>
        /// <remarks>在Head之间注册Script</remarks>
        public static void RegisterHeaderEndScript(Page page, string scriptUrl)
        {
            ScriptContainerControl ctr = GetHeaderEndScriptContainerControl(page);

            ctr.Add(scriptUrl, ScriptPositionMode.Header);
        }

        /// <summary>
        /// 在body开始后注册Script文件
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="scriptUrl">script文件url路径</param>
        /// <remarks>在body开始后注册Script</remarks>
        public static void RegisterBodyStartScript(Page page, string scriptUrl)
        {
            ScriptContainerControl ctr = GetScriptContainerControl(page);

            ctr.Add(scriptUrl, ScriptPositionMode.BodyStart);
        }

        /// <summary>
        /// 在body开始后注册Script文件
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="scriptUrl">script文件url路径</param>
        /// <remarks>在body开始后注册Script</remarks>
        public static void RegisterScriptManagerScript(Page page, string scriptUrl)
        {
            ScriptContainerControl ctr = GetScriptContainerControl(page);

            ctr.Add(scriptUrl, ScriptPositionMode.ScriptManager);
        }

        /// <summary>
        /// 在body结束前注册Script文件
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="scriptUrl">script文件url路径</param>
        /// <remarks>在body结束前注册Script</remarks>
        public static void RegisterBodyEndScript(Page page, string scriptUrl)
        {
            ScriptContainerControl ctr = GetScriptContainerControl(page);

            ctr.Add(scriptUrl, ScriptPositionMode.BodyEnd);
        }

        /// <summary>
        /// 注册脚本
        /// </summary>
        /// <param name="page"></param>
        /// <param name="script"></param>
        public static void RegisterClientScriptBlock(Page page, string script)
        {
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        /// <summary>
        /// 注册脚本
        /// </summary>
        /// <param name="page"></param>
        /// <param name="script"></param>
        public static void RegisterStartupScript(Page page, string script)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        /// <summary>
        /// 注册响应onload事件的脚本
        /// </summary>
        /// <param name="page"></param>
        /// <param name="script"></param>
        public static void RegisterOnLoadScriptBlock(Page page, string script)
        {
            string loadScript = string.Format("window.attachEvent('onload', function(){{{0}}});", script);
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), loadScript, true);
        }

        /// <summary>
        /// 注册响应Ajax中Application的Load事件
        /// </summary>
        /// <param name="page"></param>
        /// <param name="script"></param>
        public static void RegisterAjaxApplicationLoadScriptBlock(Page page, string script)
        {
            string loadScript = string.Format(@"Sys.Application.add_load(function(){{{0}}});", script);
            page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString(), loadScript, true);
        }

        /// <summary>
        /// 在脚本外增加<script></script>标签
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static string AddScriptTags(string script)
        {
            return string.Format(_C_SCRIPT_BLOCK_FORMAT, script);
        }

        /// <summary>
        /// 获取src为scriptSrc的脚本
        /// <script type="text/javascript" src="{0}"></script>
        /// </summary>
        /// <param name="scriptSrc"></param>
        /// <returns></returns>
        public static string GetScriptString(string scriptSrc)
        {
            return string.Format(_C_SCRIPT_FORMAT, scriptSrc);
        }
    }
}
