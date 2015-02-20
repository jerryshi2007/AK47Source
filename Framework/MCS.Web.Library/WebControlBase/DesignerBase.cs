using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.Design;
using System.IO;
using System.Web.UI;
using MCS.Web.Library.Resources;

namespace MCS.Web.Library
{
    /// <summary>
    /// �ؼ����������
    /// </summary>
    abstract public class DesignerBase : ControlDesigner
    {
        /// <summary>
        /// ������ʱ��HTML
        /// </summary>
        /// <returns>�ַ���</returns>
        public  override string GetDesignTimeHtml()
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
                    if (output == null ||output==String.Empty)
                    {
                        return GetEmptyDesignTimeHtml();
                    }
                    return output ;
                   
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
