using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;

namespace MCS.Web.Responsive.Library
{
    /// <summary>
    /// 控件设计器基类
    /// </summary>
    abstract public class DesignerBase : ControlDesigner
    {
        /// <summary>
        /// 获得设计时的HTML
        /// </summary>
        /// <returns>字符串</returns>
        public override string GetDesignTimeHtml()
        {
            try
            {
                string output = "";
                Control oControl = ((Control)Component);
                StringBuilder strB = new StringBuilder(1024);
                using (StringWriter sw = new StringWriter(strB))
                {
                    HtmlTextWriter oWriter = new HtmlTextWriter(sw);
                    oControl.RenderControl(oWriter);
                    oWriter.Flush();
                    sw.Flush();
                    output = strB.ToString();
                    if (output == null || output == String.Empty)
                    {
                        return GetEmptyDesignTimeHtml();
                    }
                    return output;

                }
            }
            catch (Exception ex)
            {
                return CreatePlaceHolderDesignTimeHtml(Resources.DeluxeWebResource.DesignerTimeErrorMessage + ex.ToString());
            }
        }
        /// <summary>
        /// if null,return these markups;
        /// </summary>
        /// <returns></returns>
        protected override string GetEmptyDesignTimeHtml()
        {
            return CreatePlaceHolderDesignTimeHtml("Right-click and select Build Menu for a quick start.");
        }
    }

}
