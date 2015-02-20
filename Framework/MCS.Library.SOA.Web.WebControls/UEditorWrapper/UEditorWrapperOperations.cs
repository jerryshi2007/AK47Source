using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Library;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace MCS.Web.WebControls
{
    public partial class UEditorWrapper
    {
        #region Private
        private void RegisterScriptAndCss()
        {
            string rootPath = UriHelper.ResolveUri(UEditorWrapperSettings.GetConfig().ScriptRootPath).ToString();

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ueditorConfig",
                string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>",
                UriHelper.CombinePath(rootPath, "/editor_config.js")));

            string editorAllScript = "/editor_all.js"; // WebUtility.IsWebApplicationCompilationDebug() ?"/editor_all.js" : "/editor_all_min.js";

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ueditorAll",
                string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>",
                UriHelper.CombinePath(rootPath, editorAllScript)));

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ueditorCSS",
                string.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />",
                UriHelper.CombinePath(rootPath, "/themes/default/ueditor.css")));
        }

        private void RegisterHiddenFields()
        {
            Page.ClientScript.RegisterHiddenField(PostedDataFormName, string.Empty);
        }

        private void RenderEditorContainer(HtmlTextWriter writer)
        {
            HtmlGenericControl script = new HtmlGenericControl("script");

            script.ID = EditorContainerClientID;
            script.Attributes["type"] = "text/plain";

            if (Width != Unit.Empty)
                script.Style["width"] = Width.ToString();
            else
                script.Style["width"] = "800px";

            if (Height != Unit.Empty)
                script.Style["height"] = Height.ToString();

            script.InnerHtml = this._InitialData;

            writer.Write(WebControlUtility.GetControlHtml(script));
        }

        /// <summary>
        /// UEditor在Submit的时候，会自动创建一个TextArea，将导致ASP.net的页面检查不过（含有Html Tag）。
        /// 因此先渲染一个同ID的SPAN。SPAN和TextArea不同，不会Post的。
        /// </summary>
        /// <param name="writer"></param>
        private void RenderFakeElement(HtmlTextWriter writer)
        {
            HtmlGenericControl span = new HtmlGenericControl("span");

            span.ID = "ueditor_textarea_" + this.PostedDataFormName;
            span.Attributes["name"] = this.PostedDataFormName;
            span.Style["display"] = "none";

            writer.Write(WebControlUtility.GetControlHtml(span));
        }

        private UEditorConfigWrapper PrepareConfigWrapper(string strToolBars)
        {
            var settings = UEditorWrapperSettings.GetConfig();
            string rootPath = UriHelper.ResolveUri(settings.ScriptRootPath).ToString();

            UEditorConfigWrapper config = new UEditorConfigWrapper();

            config.UEDITOR_HOME_URL = rootPath;
            config.AutoHeightEnabled = settings.AutoHeightEnabled;
            //config.Toolbars = new string[] {
            //    "|", "Undo", "Redo", "|",
            //    "Bold", "Italic", "Underline", "StrikeThrough", "Superscript", "Subscript", "RemoveFormat", "FormatMatch", "|",
            //    "BlockQuote", "|", "PastePlain", "|", "ForeColor", "BackColor", "InsertOrderedList", "InsertUnorderedList", "|", "CustomStyle",
            //    "Paragraph", "RowSpacing", "LineHeight", "FontFamily", "FontSize", "|",
            //    "DirectionalityLtr", "DirectionalityRtl", "|", "", "Indent", "|",
            //    "JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyJustify", "|",
            //    "Link", "Unlink", "Anchor", "|", "InsertFrame", "PageBreak", "HighlightCode", "|",
            //    "Horizontal", "Date", "Time", "Spechars", "|",
            //    "InsertTable", "DeleteTable", "InsertParagraphBeforeTable", "InsertRow", "DeleteRow", "InsertCol", "DeleteCol", "MergeCells", "MergeRight", "MergeDown", "SplittoCells", "SplittoRows", "SplittoCols", "|",
            //    "SelectAll", "ClearDoc", "SearchReplace", "Print", "Preview", "CheckImage","WordImage","InsertImage","DownloadImage",
            //    "Source"};

            if (strToolBars.IsNotEmpty())
            {
                config.Toolbars = strToolBars.Trim().Replace("\r\n", "").Split(',');
            }

            config.RelativePath = true;
            config.HighlightJsUrl = UriHelper.CombinePath(rootPath, "/third-party/SyntaxHighlighter/shCore.js");
            config.HighlightCssUrl = UriHelper.CombinePath(rootPath, "/third-party/SyntaxHighlighter/shCoreDefault.css");

            //config.InitialContent = this._InitialData;

            //请参见RenderFakeElement
            config.TextArea = this.PostedDataFormName;

            return config;
        }

        #endregion
    }
}
