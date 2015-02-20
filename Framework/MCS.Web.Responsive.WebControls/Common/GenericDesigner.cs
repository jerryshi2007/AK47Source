using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.Design;
using System.Globalization;

namespace MCS.Web.Responsive.WebControls.Design
{
    public class GenericDesigner : ControlDesigner
    {
        private static readonly string placeHolderDesignTimeHtmlTemplate = @"<div style=""padding:4px; font:messagebox;color:buttontext;background-color:buttonface;border: solid 1px;border-top-color:buttonhighlight;border-left-color:buttonhighlight;border-bottom-color:buttonshadow;border-right-color:buttonshadow""><span style=""font-weight:bold"">{0}</span> - {1}<span style="" "">{2}</span></div>";

        public override string GetDesignTimeHtml()
        {
            return GetRowDesignTimeHtml("");
        }

        protected string GetRowDesignTimeHtml(string instruction)
        {
            string name = this.Component.GetType().Name;
            string ctrl = this.Component.Site.Name;
            return string.Format(CultureInfo.InvariantCulture, placeHolderDesignTimeHtmlTemplate, name, ctrl, instruction ?? string.Empty);
        }

        public override bool AllowResize
        {
            get
            {
                return false;
            }
        }
    }
}
