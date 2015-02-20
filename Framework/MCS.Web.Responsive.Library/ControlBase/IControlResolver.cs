// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace MCS.Web.Responsive.Library.Script
{
    /// <summary>
    /// Describes an object that can be used to resolve references to a control by its ID
    /// </summary>
    public interface IControlResolver
    {
        /// <summary>
        /// Resolves a reference to a control by its ID
        /// </summary>
		/// <param name="controlId">�ؼ�ID</param>
        /// <returns></returns>
        Control ResolveControl(string controlId);
    }
}
