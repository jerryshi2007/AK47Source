﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MCS.Library.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ScriptResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ScriptResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MCS.Library.Properties.ScriptResources", typeof(ScriptResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to if (typeof ($$methodname) != &quot;undefined&quot;) {
        ///    alert(&quot;方法冲突，$$methodname已定义!&quot;);
        ///}
        ///else {
        ///    var $$methodname = function (cvalue) {
        ///        var isValidate = false;
        ///        isValidate = (isNaN(cvalue) == false);
        ///
        ///        if (isValidate)
        ///            isValidate = Date.isMinDate(cvalue) == false;
        ///        return isValidate;
        ///    }
        ///}.
        /// </summary>
        internal static string DatetimeEmptyValidator {
            get {
                return ResourceManager.GetString("DatetimeEmptyValidator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to if (typeof ($$methodname) != &quot;undefined&quot;) {
        ///    alert(&quot;方法冲突，$$methodname已定义!&quot;);
        ///}
        ///else {
        ///    var $$methodname = function (cvalue, additionalDatas) {
        ///        var isValidate = false;
        ///        sourcevalue = cvalue;
        ///        lowerBound = additionalDatas[0];
        ///        upperBound = additionalDatas[1];
        ///
        ///        if (sourcevalue * 1 &lt; lowerBound * 1 || sourcevalue * 1 &gt; upperBound * 1) {
        ///            isValidate = false;
        ///        }
        ///        else {
        ///            isValidate = true;
        ///        }
        ///
        ///        return isV [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DateTimeRangeValidator {
            get {
                return ResourceManager.GetString("DateTimeRangeValidator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to if (typeof ($$methodname) != &quot;undefined&quot;) {
        ///    alert(&quot;方法冲突，$$methodname已定义!&quot;);
        ///}
        ///else {
        ///    var $$methodname = function (cvalue,additionalDatas) {
        ///        var isValidate = false;
        ///        if (cvalue) {
        ///            isValidate = cvalue != additionalDatas[0];
        ///        }
        ///        return isValidate;
        ///    }
        ///}.
        /// </summary>
        internal static string EnumDefaultValueValidator {
            get {
                return ResourceManager.GetString("EnumDefaultValueValidator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to if (typeof ($$methodname) != &quot;undefined&quot;) {
        ///    alert(&quot;方法冲突，$$methodname已定义!&quot;);
        ///}
        ///else {
        ///    var $$methodname = function (cvalue, additionalDatas) {
        ///        var isValidate = false;
        ///        sourcevalue = cvalue;
        ///        lowerBound = additionalDatas[0];
        ///        upperBound = additionalDatas[1];
        ///
        ///        if (sourcevalue * 1 &lt; lowerBound * 1 || sourcevalue * 1 &gt; upperBound * 1) {
        ///            isValidate = false;
        ///        }
        ///        else {
        ///            isValidate = true;
        ///        }
        ///
        ///        return isV [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string IntegerRangeValidator {
            get {
                return ResourceManager.GetString("IntegerRangeValidator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to if (typeof ($$methodname) != &quot;undefined&quot;) {
        ///    alert(&quot;方法冲突，$$methodname已定义!&quot;);
        ///}
        ///else {
        ///    var $$methodname = function (cvalue) {
        ///        var isValidate = false;
        ///        if (cvalue) {
        ///            isValidate = true;
        ///        }
        ///        return isValidate;
        ///    }
        ///}.
        /// </summary>
        internal static string NotNullValidator {
            get {
                return ResourceManager.GetString("NotNullValidator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to if (typeof ($$methodname) != &quot;undefined&quot;) {
        ///    alert(&quot;方法冲突，$$methodname已定义!&quot;);
        ///}
        ///else {
        ///    var $$methodname = function (cvalue) {
        ///        var isValidate = false;
        ///        if (cvalue) {
        ///            isValidate = true;
        ///        }
        ///        return isValidate;
        ///    }
        ///}.
        /// </summary>
        internal static string ObjectNullValidator {
            get {
                return ResourceManager.GetString("ObjectNullValidator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  if (typeof ($$methodname) != &quot;undefined&quot;) {
        ///    alert(&quot;方法冲突，$$methodname已定义!&quot;);
        ///}
        ///else {
        ///    var $$methodname = function (cvalue, additionalDatas) {
        ///        if (additionalDatas[0] === true) {
        ///            cvalue = cvalue.toString().replace(/,/g, &apos;&apos;);
        ///        }
        ///
        ///        var isValidate = false;
        ///        var reg = new RegExp(additionalDatas[1].toString());
        ///        isValidate = reg.test(cvalue);
        ///
        ///        return isValidate;
        ///    }
        ///}
        ///.
        /// </summary>
        internal static string RegexValidator {
            get {
                return ResourceManager.GetString("RegexValidator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string StringEmptyValidator {
            get {
                return ResourceManager.GetString("StringEmptyValidator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string StringLengthValidator {
            get {
                return ResourceManager.GetString("StringLengthValidator", resourceCulture);
            }
        }
    }
}
