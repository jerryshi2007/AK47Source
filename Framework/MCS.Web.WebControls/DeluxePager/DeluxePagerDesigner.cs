// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	DeluxePagerDesigner.cs
// Remark	��  
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		�����	    20070815		����
// -------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using MCS.Web.Library;
namespace MCS.Web.WebControls
{
    /// <summary>
    /// pager���ģʽ��
    /// </summary>
    /// <remarks>
    ///  pager���ģʽ��
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
