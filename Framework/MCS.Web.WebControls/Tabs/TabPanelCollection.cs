// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// TabPanelCollection
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1035:ICollectionImplementationsHaveStronglyTypedMembers", Justification="Unnecessary for this specialized class")]
    public class TabPanelCollection : ControlCollection
    {
        /// <summary>
        /// TabPanelCollection
        /// </summary>
        /// <param name="owner"></param>
        public TabPanelCollection(Control owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="child"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "Assembly is not localized")]
        public override void Add(Control child)
        {
            if (!(child is TabPanel))
            {
                throw new ArgumentException("TabPanelCollection can only contain TabPanel controls.");
            }
            base.Add(child);
        }

        /// <summary>
        /// AddAt
        /// </summary>
        /// <param name="index"></param>
        /// <param name="child"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "Assembly is not localized")]
        public override void AddAt(int index, Control child)
        {
            if (!(child is TabPanel))
            {
                throw new ArgumentException("TabPanelCollection can only contain TabPanel controls.");
            }
            base.AddAt(index, child);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public new TabPanel this[int index]
        {
            get { return (TabPanel)base[index]; }
        }
    }
}
