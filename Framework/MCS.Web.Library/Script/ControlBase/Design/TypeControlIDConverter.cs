// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;

namespace MCS.Web.Library.Script
{
    /// <summary>
    /// A simple derived class of ControlID converter that lets us filter based on a type.
    /// 
    /// This lets us have drop-downs in the property browser that pick a given Button or TextBox, etc.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification="Following ASP.NET AJAX pattern")]
    public class TypedControlIDConverter<T> : ControlIDConverter
    {
		/// <summary>
		/// 判断control是否类型为T
		/// </summary>
		/// <param name="control">control实例</param>
		/// <returns>是否类型为T</returns>
        protected override bool FilterControl(Control control)
        {
            return typeof(T).IsInstanceOfType(control);
        }
    } 
}
