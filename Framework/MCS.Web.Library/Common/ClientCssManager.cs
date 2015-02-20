using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace MCS.Web.Library
{
    /// <summary>
    /// ��ҳ����ע��Css�ļ���������
    /// </summary>
	/// <remarks>��ҳ����ע��Css�ļ���������</remarks>
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
                page.Header.Controls.AddAt(0, ctr);
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
                page.Header.Controls.Add(ctr);
            }

            return ctr;
        }

        /// <summary>
		/// ��Head֮��ע��Css�ļ�
        /// </summary>
        /// <param name="page">��ǰҳ��</param>
        /// <param name="cssUrl">css�ļ�url·��</param>
		/// <remarks>��Head֮��ע��Css</remarks>
        public static void RegisterHeaderCss(Page page, string cssUrl)
        {
            StyleSheetControl ctr = GetStyleSheetControl(page);
            ctr.Add(cssUrl, StyleSheetPositionMode.Header);
        }

        /// <summary>
        /// ��Head����ǰע��Css�ļ�
        /// </summary>
        /// <param name="page">��ǰҳ��</param>
        /// <param name="cssUrl">css�ļ�url·��</param>
        /// <remarks>��Head֮��ע��Css</remarks>
        public static void RegisterHeaderEndCss(Page page, string cssUrl)
        {
            StyleSheetControl ctr = GetHeaderEndStyleSheetControl(page);
            ctr.Add(cssUrl, StyleSheetPositionMode.Header);
        }

        /// <summary>
		/// ��body��ʼ��ע��Css�ļ�
        /// </summary>
		/// <param name="page">��ǰҳ��</param>
		/// <param name="cssUrl">css�ļ�url·��</param>
		/// <remarks>��body��ʼ��ע��Css</remarks>
        public static void RegisterBodyStartCss(Page page, string cssUrl)
        {
            StyleSheetControl ctr = GetStyleSheetControl(page);
            ctr.Add(cssUrl, StyleSheetPositionMode.BodyStart);
        }

        /// <summary>
		/// ��body����ǰע��Css�ļ�
        /// </summary>
		/// <param name="page">��ǰҳ��</param>
		/// <param name="cssUrl">css�ļ�url·��</param>
		/// <remarks>��body����ǰע��Css</remarks>
        public static void RegisterBodyEndCss(Page page, string cssUrl)
        {
            StyleSheetControl ctr = GetStyleSheetControl(page);
            ctr.Add(cssUrl, StyleSheetPositionMode.BodyEnd);
        }
    }

    /// <summary>
    /// ��ҳ����ע��Script�ļ���������
    /// </summary>
    /// <remarks>��ҳ����ע��Script�ļ���������</remarks>
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
                page.Header.Controls.AddAt(0, ctr);
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
                page.Header.Controls.Add(ctr);
            }

            return ctr;
        }

        /// <summary>
        /// ��Head֮��ע��Script�ļ�
        /// </summary>
        /// <param name="page">��ǰҳ��</param>
        /// <param name="scriptUrl">script�ļ�url·��</param>
        /// <remarks>��Head֮��ע��Script</remarks>
        public static void RegisterHeaderScript(Page page, string scriptUrl)
        {
            ScriptContainerControl ctr = GetScriptContainerControl(page);
            ctr.Add(scriptUrl, ScriptPositionMode.Header);
        }

        /// <summary>
        /// ��Head����ǰע��Script�ļ�
        /// </summary>
        /// <param name="page">��ǰҳ��</param>
        /// <param name="scriptUrl">script�ļ�url·��</param>
        /// <remarks>��Head֮��ע��Script</remarks>
        public static void RegisterHeaderEndScript(Page page, string scriptUrl)
        {
            ScriptContainerControl ctr = GetHeaderEndScriptContainerControl(page);
            ctr.Add(scriptUrl, ScriptPositionMode.Header);
        }

        /// <summary>
        /// ��body��ʼ��ע��Script�ļ�
        /// </summary>
        /// <param name="page">��ǰҳ��</param>
        /// <param name="scriptUrl">script�ļ�url·��</param>
        /// <remarks>��body��ʼ��ע��Script</remarks>
        public static void RegisterBodyStartScript(Page page, string scriptUrl)
        {
            ScriptContainerControl ctr = GetScriptContainerControl(page);
            ctr.Add(scriptUrl, ScriptPositionMode.BodyStart);
        }

        /// <summary>
        /// ��body��ʼ��ע��Script�ļ�
        /// </summary>
        /// <param name="page">��ǰҳ��</param>
        /// <param name="scriptUrl">script�ļ�url·��</param>
        /// <remarks>��body��ʼ��ע��Script</remarks>
        public static void RegisterScriptManagerScript(Page page, string scriptUrl)
        {
            ScriptContainerControl ctr = GetScriptContainerControl(page);
            ctr.Add(scriptUrl, ScriptPositionMode.ScriptManager);
        }

        /// <summary>
        /// ��body����ǰע��Script�ļ�
        /// </summary>
        /// <param name="page">��ǰҳ��</param>
        /// <param name="scriptUrl">script�ļ�url·��</param>
        /// <remarks>��body����ǰע��Script</remarks>
        public static void RegisterBodyEndScript(Page page, string scriptUrl)
        {
            ScriptContainerControl ctr = GetScriptContainerControl(page);
            ctr.Add(scriptUrl, ScriptPositionMode.BodyEnd);
        }

        /// <summary>
        /// ע��ű�
        /// </summary>
        /// <param name="page"></param>
        /// <param name="script"></param>
        public static void RegisterClientScriptBlock(Page page, string script)
        {
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        /// <summary>
        /// ע��ű�
        /// </summary>
        /// <param name="page"></param>
        /// <param name="script"></param>
        public static void RegisterStartupScript(Page page, string script)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        /// <summary>
        /// ע����Ӧonload�¼��Ľű�
        /// </summary>
        /// <param name="page"></param>
        /// <param name="script"></param>
        public static void RegisterOnLoadScriptBlock(Page page, string script)
        {
            string loadScript = string.Format("window.attachEvent('onload', function(){{{0}}});", script);
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), loadScript, true);
        }

        /// <summary>
        /// ע����ӦAjax��Application��Load�¼�
        /// </summary>
        /// <param name="page"></param>
        /// <param name="script"></param>
        public static void RegisterAjaxApplicationLoadScriptBlock(Page page, string script)
        {
            string loadScript = string.Format(@"Sys.Application.add_load(function(){{{0}}});", script);
            page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString(), loadScript, true);
        }

        /// <summary>
        /// �ڽű�������<script></script>��ǩ
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static string AddScriptTags(string script)
        {
            return string.Format(_C_SCRIPT_BLOCK_FORMAT, script);
        }

        /// <summary>
        /// ��ȡsrcΪscriptSrc�Ľű�
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
