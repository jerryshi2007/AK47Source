using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;

namespace MCS.Web.Responsive.Library.Script
{
    internal enum StyleSheetPositionMode
    {
        Header = 1,
        BodyStart = 2,
        BodyEnd = 3
    }

    internal class StyleSheetControl : Control
    {
        private const string _C_CSS_LINK_FORMAT = "<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />";
        private Dictionary<string, StyleSheetPositionMode> _UrlContainer = new Dictionary<string, StyleSheetPositionMode>();

        public void Add(string cssUrl, StyleSheetPositionMode mode)
        {
            if (_UrlContainer.ContainsKey(cssUrl) == false)
            {
                _UrlContainer.Add(cssUrl, mode);
                this.BuildChildControl(cssUrl, mode);
            }
        }

        private void BuildChildControl(string cssUrl, StyleSheetPositionMode mode)
        {
            switch (mode)
            {
                case StyleSheetPositionMode.Header:
                    HtmlLink link = new HtmlLink();
                    link.Href = cssUrl;
                    link.Attributes.Add("type", "text/css");
                    link.Attributes.Add("rel", "stylesheet");
                    this.Controls.Add(link);
                    break;

                case StyleSheetPositionMode.BodyStart:
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(),
                        string.Format(_C_CSS_LINK_FORMAT, cssUrl), false);
                    break;

                case StyleSheetPositionMode.BodyEnd:
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
                        string.Format(_C_CSS_LINK_FORMAT, cssUrl), false);
                    break;
            }
        }
    }

    internal enum ScriptPositionMode
    {
        Header = 1,
        BodyStart = 2,
        ScriptManager = 3,
        BodyEnd = 4
    }

    internal class ScriptContainerControl : Control
    {
        private const string _C_SCRIPT_FORMAT = "<script type=\"text/javascript\" src=\"{0}\"></script>";
        private Dictionary<string, ScriptPositionMode> _UrlContainer = new Dictionary<string, ScriptPositionMode>();

        public void Add(string scriptUrl, ScriptPositionMode mode)
        {
            if (_UrlContainer.ContainsKey(scriptUrl) == false)
            {
                _UrlContainer.Add(scriptUrl, mode);
                this.BuildChildControl(scriptUrl, mode);
            }
        }

        private void BuildChildControl(string scriptUrl, ScriptPositionMode mode)
        {
            switch (mode)
            {
                case ScriptPositionMode.Header:
                    HtmlGenericControl ctr = new HtmlGenericControl("script");
                    ctr.Attributes.Add("language", "javascript");
                    ctr.Attributes.Add("type", "text/javascript");
                    ctr.Attributes.Add("src", ctr.ResolveUrl(scriptUrl));
                    this.Controls.Add(ctr);
                    break;

                case ScriptPositionMode.ScriptManager:
                    ScriptManager sm = ScriptManager.GetCurrent(this.Page);

                    ExceptionHelper.TrueThrow(sm == null, Resources.DeluxeWebResource.E_NoScriptManager);

                    sm.Scripts.Add(new ScriptReference(scriptUrl));
                    break;

                case ScriptPositionMode.BodyStart:
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(),
                        string.Format(_C_SCRIPT_FORMAT, scriptUrl), false);
                    break;

                case ScriptPositionMode.BodyEnd:
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
                        string.Format(_C_SCRIPT_FORMAT, scriptUrl), false);
                    break;

            }
        }
    }
}
