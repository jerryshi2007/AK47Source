﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MCS.Library.SOA.DataObjects.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MCS.Library.SOA.DataObjects.Properties.ScriptResources", typeof(ScriptResources).Assembly);
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
        ///   Looks up a localized string similar to $HGRootNS.ValidatorManager.IOguObjectNullValidator = function () {
        ///    this.validate = function (cvalue) {
        ///        var isValidate = false;
        ///
        ///        if (cvalue) {
        ///            if (cvalue.length &gt; 0 &amp;&amp; cvalue[0].id) {
        ///                isValidate = true;
        ///            }
        ///        }
        ///
        ///        return isValidate;
        ///    }
        ///};
        ///.
        /// </summary>
        internal static string IOguObjectNullValidator {
            get {
                return ResourceManager.GetString("IOguObjectNullValidator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to $HGRootNS.ValidatorManager.RangeValidator = function () {
        ///    this.validate = function (cvalue, additionalData) {
        ///        var sourcevalue = cvalue;
        ///        var lowerBound = additionalData.lowerBound;
        ///        var upperBound = additionalData.upperBound;
        ///
        ///        sourcevalue = sourcevalue.toString().replace(/,/g, &apos;&apos;);
        ///
        ///        if (isNaN(sourcevalue * 1) == true) {
        ///            return false;
        ///        }
        ///
        ///        if (sourcevalue * 1 &lt; lowerBound * 1 || sourcevalue * 1 &gt; upperBound * 1) {
        ///            re [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RangeValidator {
            get {
                return ResourceManager.GetString("RangeValidator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to $HGRootNS.ValidatorManager.StringByteLengthValidator = function () {
        ///    this.validate = function (cvalue, additionalData) {
        ///        var isValidate = false;
        ///        var sourcevalue = cvalue.replace(/[^\x00-\xff]/g, &apos;**&apos;).length;
        ///        var lowerBound = additionalData.lowerBound;
        ///        var upperBound = additionalData.upperBound;
        ///
        ///        if (sourcevalue * 1 &lt; lowerBound * 1 || sourcevalue * 1 &gt; upperBound * 1) {
        ///            isValidate = false;
        ///        }
        ///        else {
        ///            isValidate = t [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string StringByteLengthValidator {
            get {
                return ResourceManager.GetString("StringByteLengthValidator", resourceCulture);
            }
        }
    }
}
