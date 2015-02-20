// -------------------------------------------------
// Assembly	：	MCS.Web.Responsive.WebControls
// FileName	：	DeluxePagerDesigner.cs
// Remark	：  
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		马泽锋	    20070815		创建
// -------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using MCS.Web.Responsive.Library;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// pager设计模式类
    /// </summary>
    /// <remarks>
    ///  pager设计模式类
    /// </remarks>
    internal class DeluxePagerDesigner : DesignerBase
    {
        /// <summary>
        /// GetEmptyDesignTimeHtml
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///  GetEmptyDesignTimeHtml
        /// </remarks>
        protected override string GetEmptyDesignTimeHtml()
        {
            return CreatePlaceHolderDesignTimeHtml("To configure and style this GridView, please switch to HTML view.");
        }
    }
}
