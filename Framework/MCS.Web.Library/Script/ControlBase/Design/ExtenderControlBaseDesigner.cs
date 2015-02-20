// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Reflection;

namespace MCS.Web.Library.Script
{
    /// <summary>
    /// The designer here has two main jobs:
    /// 
    /// 1) Modifying the properties of the TargetProperties objects to remove the "TargetControlID" property and add the Control converter.
    ///    We remove "TargetControlID" because the value will be hanging off of the target control's properties already so it's confusing/redundant.
    /// 2) Modifying the name of the Extender property so it's user-configurable.
    /// 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Justification="Security handled by base class")]
    public class ExtenderControlBaseDesigner<T> :
      ExtenderControlDesigner, IExtenderProvider
        where T : ExtenderControlBase
    {

        // we keep track of the providers we've created.
        //
        ExtenderPropertyRenameDescProv _renameProvider;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Call to AddAttributes can not be moved inline")]
        static ExtenderControlBaseDesigner()
        {
            TypeDescriptor.AddAttributes(typeof(ExtenderControlBaseDesigner<T>), new ProvidePropertyAttribute("Extender", typeof(Control)));
        }

		/// <summary>
		/// Constructor
		/// </summary>
        public ExtenderControlBaseDesigner()
        {
        }

        /// <summary>
        /// The ExtenderControl this designer is attached to.
        /// </summary>
        protected T ExtenderControl
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods", Justification = "Security handled by base class")]
            get
            {
                return Component as T;
            }
        }

        /// <summary>
        /// The name of the ExtenderProperty that will appear in the properties of potential target controls.
        /// 
        /// By default this is the name of the Extender (e.g. "TextBoxWatermarkExtender") but can be customized by overriding this property.
        /// </summary>
        protected virtual string ExtenderPropertyName
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods", Justification = "Security handled by base class")]
            get
            {
               return String.Format(CultureInfo.InvariantCulture, "{0} ({1})",TypeDescriptor.GetComponentName(Component), ExtenderControl.GetType().Name);               
            }
        }

        /// <summary>
        /// Called to check if we can extend a given control.  We return true if the control has the same ID a
        /// our extender control target.
        /// </summary>
		/// <param name="extendee"></param>
        /// <returns></returns>
        public bool CanExtend(object extendee)
        {
            Control extendeeControl = extendee as Control;
            bool canExtend = false;
            if (extendeeControl != null)
            {

                canExtend = (extendeeControl.ID == ExtenderControl.TargetControlID);

                if (canExtend) {
                    if (_renameProvider == null) {
                        // hook up the rename prvider.
                        _renameProvider = new ExtenderPropertyRenameDescProv(this, extendeeControl);
                        TypeDescriptor.AddProvider(_renameProvider, extendeeControl);
                    }
                }
            }
            return canExtend;
        }

		/// <summary>
		/// Disopose Resources
		/// </summary>
		/// <param name="disposing"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods", Justification = "Security handled by base class")]
        protected override void Dispose(bool disposing)
        {   
            if (disposing)
            {
                if (_renameProvider != null)
                {
                    TypeDescriptor.RemoveProvider(_renameProvider, Component);
                    _renameProvider.Dispose();
                    _renameProvider = null;
                }
            }

            base.Dispose(disposing);
        }
        
		/// <summary>
		/// GetExtender
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
        [
        Category("Extenders"),
        Browsable(true),
        TypeConverter(typeof(ExtenderPropertiesTypeDescriptor))
        ]
        public object GetExtender(object control)
        {
           Control c = control as Control;
           
           // we return a wrapper object here.  This lets us avoid creating
           // the actual target properties object until the '+' is clicked in
           // the property browser.
           //
           // If we don't do this, an empty properties would get added and then (potentially) serialized out to code.
           //
           if (c != null)
           {   
               return new ExtenderPropertiesProxy(ExtenderControl, "TargetControlID");
           }
           return null;
       }

		/// <summary>
	   /// Initialize component
		/// </summary>
	   /// <param name="component">component</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods", Justification = "Security handled by base class")]
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="properties"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods", Justification = "Security handled by base class")]
        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            base.PreFilterProperties(properties);

            string[] propertyNames = new string[properties.Keys.Count];

            properties.Keys.CopyTo(propertyNames, 0);

            foreach (string propertyName in propertyNames) {
                PropertyDescriptor pd = (PropertyDescriptor)properties[propertyName];

                if (propertyName == "TargetControlID") {
                    // get the control type.
                    //
                    TargetControlTypeAttribute targetControlType = (TargetControlTypeAttribute)TypeDescriptor.GetAttributes(ExtenderControl)[typeof(TargetControlTypeAttribute)];

                    if (targetControlType != null && !targetControlType.IsDefaultAttribute()) {
                        Type genericType = typeof(TypedControlIDConverter<>).MakeGenericType(targetControlType.TargetControlType);
                        properties[propertyName] = TypeDescriptor.CreateProperty(pd.ComponentType, pd, new TypeConverterAttribute(genericType));
                    }
                }

                ScriptControlPropertyAttribute extenderPropAttr = (ScriptControlPropertyAttribute)pd.Attributes[typeof(ScriptControlPropertyAttribute)];

                if (extenderPropAttr == null || !extenderPropAttr.IsScriptProperty) {
                    continue;
                }

                BrowsableAttribute browsableAttr = (BrowsableAttribute)pd.Attributes[typeof(BrowsableAttribute)];

                if (browsableAttr.Browsable == BrowsableAttribute.Yes.Browsable) {
                    properties[propertyName] = TypeDescriptor.CreateProperty(pd.ComponentType, pd, BrowsableAttribute.No, ExtenderVisiblePropertyAttribute.Yes);
                }
            }
        }
        
        /// <summary>
        /// This provider allows us to rename the extender provided property
        /// </summary>
        private class ExtenderPropertyRenameDescProv : FilterTypeDescriptionProvider<IComponent>
        {
            private ExtenderControlBaseDesigner<T> _owner;

            public ExtenderPropertyRenameDescProv(ExtenderControlBaseDesigner<T> owner, IComponent target)
                  : base(target)
            {
                _owner = owner;
                FilterExtendedProperties = true;
            }            
            protected override PropertyDescriptor ProcessProperty(PropertyDescriptor baseProp)
            {
                if (baseProp.Name == "Extender" && baseProp.ComponentType == _owner.GetType() &&  _owner.ExtenderPropertyName != null)
                {
                    // if the property is called "Extender", rename it with the name specified by the designer.
                    //
                    return TypeDescriptor.CreateProperty(baseProp.ComponentType, baseProp, 
                        new DisplayNameAttribute(_owner.ExtenderPropertyName));
                }
                return base.ProcessProperty(baseProp);
            }
        }


      

       
    }
}
