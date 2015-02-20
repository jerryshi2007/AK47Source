// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace MCS.Web.Responsive.Library.Script
{
    /// <summary>
	/// 标志此属性将输出到客户端控件事件
    /// Signifies that this Property should be exposed as a client-side event reference
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited=true)]
    public sealed class ScriptControlEventAttribute : Attribute
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification="Exposing this for user convenience")]
        private static ScriptControlEventAttribute Yes = new ScriptControlEventAttribute(true);
        private static ScriptControlEventAttribute No = new ScriptControlEventAttribute(false);
        private static ScriptControlEventAttribute Default = No;

        #region [ Fields ]

        private bool _isScriptEvent;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new ScriptControlEventAttribute
        /// </summary>
        public ScriptControlEventAttribute()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new ScriptControlEventAttribute
        /// </summary>
        /// <param name="isScriptEvent"></param>
        public ScriptControlEventAttribute(bool isScriptEvent)
        {
            _isScriptEvent = isScriptEvent;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Whether this is a valid ScriptEvent
        /// </summary>
        public bool IsScriptEvent
        {
            get { return _isScriptEvent; }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tests for object equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, this))
            {
                return true;
            }
            ScriptControlEventAttribute other = obj as ScriptControlEventAttribute;
            if (other != null)
            {
                return other._isScriptEvent == _isScriptEvent;
            }
            return false;
        }

        /// <summary>
        /// Gets a hash code for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _isScriptEvent.GetHashCode();
        }

        /// <summary>
        /// Gets whether this is the default value for this attribute
        /// </summary>
        /// <returns></returns>
        public override bool IsDefaultAttribute()
        {
            return Equals(Default);
        }

        #endregion
    }
}
