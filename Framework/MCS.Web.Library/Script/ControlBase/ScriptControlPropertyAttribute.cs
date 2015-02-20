// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Web.Library.Script
{
    /// <summary>
	/// 标志控件属性将被映射到客户端控件属性
    /// Signifies that this property is to be emitted as a client script property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ScriptControlPropertyAttribute : Attribute
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Exposing this for user convenience")]
        private static ScriptControlPropertyAttribute Yes = new ScriptControlPropertyAttribute(true);
        private static ScriptControlPropertyAttribute No = new ScriptControlPropertyAttribute(false);
        private static ScriptControlPropertyAttribute Default = No;
        
        #region [ Fields ]

        private bool _isScriptProperty;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new ScriptControlPropertyAttribute
        /// </summary>
        public ScriptControlPropertyAttribute()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new ScriptControlPropertyAttribute
        /// </summary>
        /// <param name="isScriptProperty"></param>
        public ScriptControlPropertyAttribute(bool isScriptProperty)
        {
            _isScriptProperty = isScriptProperty;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Whether this property should be exposed to the client
        /// </summary>
        public bool IsScriptProperty
        {
            get { return _isScriptProperty; }
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
            if (object.ReferenceEquals(obj, this)) {
                return true;
            }
            ScriptControlPropertyAttribute other = obj as ScriptControlPropertyAttribute;
            if (other != null)
            {
                return other._isScriptProperty == _isScriptProperty;
            }
            return false;
        }

        /// <summary>
        /// Gets a hash code for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _isScriptProperty.GetHashCode();
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
