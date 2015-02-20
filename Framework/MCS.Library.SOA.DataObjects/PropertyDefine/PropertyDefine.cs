using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Xml.Linq;
using MCS.Library.Validation;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects
{
    [Serializable]
    [XElementSerializable]
    public class PropertyDefine : IXElementSerializable
    {
        [XElementFieldSerialize(AlternateFieldName = "_Name")]
        [ORFieldMapping("Name", PrimaryKey = true)]
        public string Name { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_DisplayName")]
        public string DisplayName { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_Category")]
        public string Category { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_Description")]
        public string Description { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_DefaultValue")]
        public string DefaultValue { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_ReadOnly")]
        public bool ReadOnly { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_EditorKey")]
        public string EditorKey { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_PersisterKey")]
        public string PersisterKey { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_EditorParamsSettingsKey")]
        public string EditorParamsSettingsKey { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_EditorParams")]
        public string EditorParams { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_SortOrder")]
        public int SortOrder { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_MaxLength")]
        public int MaxLength { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_IsRequired")]
        public bool IsRequired { get; set; }

        [XElementFieldSerialize(AlternateFieldName = "_ShowTitle")]
        public bool ShowTitle { get; set; }

        private PropertyDataType _DataType = PropertyDataType.String;

        private PropertyValidatorDescriptorCollection _Validators = null;

        [NoMapping]
        public PropertyValidatorDescriptorCollection Validators
        {
            get
            {
                if (this._Validators == null)
                    this._Validators = new PropertyValidatorDescriptorCollection();

                return this._Validators;
            }
        }

        private bool _Visible = true;
        private bool _AllowOverride = true;

        public PropertyDefine()
        {
        }

        public bool Visible
        {
            get
            {
                return this._Visible;
            }
            set
            {
                this._Visible = value;
            }
        }

        /// <summary>
        /// 属性合并的时候是否允许覆盖之前的属性
        /// </summary>
        [NoMapping]
        public bool AllowOverride
        {
            get
            {
                return this._AllowOverride;
            }
            set
            {
                this._AllowOverride = value;
            }
        }

        public PropertyDefine(PropertyDefineConfigurationElement propDefineElem)
        {
            this.Name = propDefineElem.Name;
            this.DisplayName = propDefineElem.DisplayName;
            this.Category = propDefineElem.Category;
            this.DataType = propDefineElem.DataType;
            this.DefaultValue = propDefineElem.DefaultValue;
            this.Description = propDefineElem.Description;
            this.EditorKey = propDefineElem.EditorKey;
            this.ReadOnly = propDefineElem.ReadOnly;
            this.Visible = propDefineElem.Visible;
            this.AllowOverride = propDefineElem.AllowOverride;
            this.EditorParamsSettingsKey = propDefineElem.EditorParamsSettingsKey;
            this.EditorParams = propDefineElem.EditorParams;
            this.PersisterKey = propDefineElem.PersisterKey;
            this.SortOrder = propDefineElem.SortOrder;
            this.MaxLength = propDefineElem.MaxLength;
            this.IsRequired = propDefineElem.IsRequired;
            this.ShowTitle = propDefineElem.ShowTitle;

            this.Validators.LoadValidatorsFromConfiguration(propDefineElem.Validators);
        }

        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumString)]
        public PropertyDataType DataType
        {
            get { return this._DataType; }
            set { this._DataType = value; }
        }

        public virtual void Serialize(XElement node, XmlSerializeContext context)
        {
            if (this.Name.IsNotEmpty())
                node.SetAttributeValue("_n", this.Name);

            if (this.DisplayName.IsNotEmpty())
                node.SetAttributeValue("_dn", this.DisplayName);

            if (this.Category.IsNotEmpty())
                node.SetAttributeValue("_category", this.Category);

            if (this._DataType != PropertyDataType.String)
                node.SetAttributeValue("_dataType", this._DataType);

            if (this.DefaultValue.IsNotEmpty())
                node.SetAttributeValue("_dv", this.DefaultValue);

            if (this.Description.IsNotEmpty())
                node.SetAttributeValue("_desp", this.Description);

            if (this.EditorKey.IsNotEmpty())
                node.SetAttributeValue("_eKey", this.EditorKey);

            if (this.PersisterKey.IsNotEmpty())
                node.SetAttributeValue("_perKey", this.PersisterKey);

            if (this.EditorParamsSettingsKey.IsNotEmpty())
                node.SetAttributeValue("_ePS", this.EditorParamsSettingsKey);

            if (this.EditorParams.IsNotEmpty())
                node.SetAttributeValue("_eP", this.EditorParams);

            if (this.ReadOnly)
                node.SetAttributeValue("_ro", this.ReadOnly);

            if (this._Visible == false)
                node.SetAttributeValue("_visible", this._Visible);

            if (this._AllowOverride == false)
                node.SetAttributeValue("_override", this._AllowOverride);

            if (this.SortOrder != 0)
                node.SetAttributeValue("_sortOrder", this.SortOrder);

            if (this.MaxLength != 0)
                node.SetAttributeValue("_ml", this.MaxLength);

            if (this.IsRequired)
                node.SetAttributeValue("_isRequired", this.SortOrder);

            if (!this.ShowTitle)
            {
                node.SetAttributeValue("_showTitle", this.ShowTitle);
            }

            if (this._Validators != null)
                this._Validators.Serialize(node, context);
        }

        public virtual void Deserialize(XElement node, XmlDeserializeContext context)
        {
            this.Name = node.Attribute("_n", this.Name);
            this.DisplayName = node.Attribute("_dn", this.DisplayName);
            this.Category = node.Attribute("_category", this.Category);
            this._DataType = node.Attribute("_dataType", PropertyDataType.String);
            this.DefaultValue = node.Attribute("_dv", this.DefaultValue);
            this.Description = node.Attribute("_desp", this.Description);
            this.EditorKey = node.Attribute("_eKey", this.EditorKey);
            this.PersisterKey = node.Attribute("_perKey", this.PersisterKey);
            this.EditorParamsSettingsKey = node.Attribute("_ePS", this.EditorParamsSettingsKey);
            this.EditorParams = node.Attribute("_eP", this.EditorParams);
            this.ReadOnly = node.Attribute("_ro", false);
            this.Visible = node.Attribute("_visible", true);
            this.AllowOverride = node.Attribute("_override", true);
            this.SortOrder = node.Attribute("_sortOrder", 0);
            this.MaxLength = node.Attribute("_ml", 0);
            this.IsRequired = node.Attribute("_isRequired", false);
            this.ShowTitle = node.Attribute("_showTitle", true);

            if (node.HasElements)
            {
                if (this._Validators == null)
                    this._Validators = new PropertyValidatorDescriptorCollection();
                else
                    this._Validators.Clear();

                this._Validators.Deserialize(node, context);
            }
        }

        public ClientVdtData GetPropertyValidator()
        {
            ClientVdtData cdtData = new ClientVdtData();
            cdtData.IsValidateOnSubmit = true;
            cdtData.ValidateProp = "value";
            cdtData.ClientIsHtmlElement = true;

            switch (this.DataType)
            {
                case PropertyDataType.Integer:
                    cdtData.IsOnlyNum = true;
                    break;
                case PropertyDataType.Decimal:
                    cdtData.IsFloat = true;
                    break;
            }

            foreach (PropertyValidatorDescriptor propValidator in this.Validators)
            {
                Validator vali = propValidator.GetValidator();
                if (vali is IClientValidatable)
                {
                    ClientVdtAttribute cvArt = new ClientVdtAttribute(propValidator);
                    cdtData.CvtList.Add(cvArt);

                    if (string.IsNullOrEmpty(cvArt.ClientValidateMethodName) == false)
                        cvArt.AdditionalData = ((IClientValidatable)vali).GetClientValidateAdditionalData(vali.Tag);
                }
            }

            return cdtData;
        }

        public virtual PropertyDefine Clone()
        {
            PropertyDefine newDefine = new PropertyDefine();

            newDefine.Name = this.Name;
            newDefine.DisplayName = this.DisplayName;
            newDefine.Category = this.Category;
            newDefine.DataType = this.DataType;
            newDefine.DefaultValue = this.DefaultValue;
            newDefine.Description = this.Description;
            newDefine.EditorKey = this.EditorKey;
            newDefine.ReadOnly = this.ReadOnly;
            newDefine.Visible = this.Visible;
            newDefine.AllowOverride = this.AllowOverride;
            newDefine.EditorParamsSettingsKey = this.EditorParamsSettingsKey;
            newDefine.EditorParams = this.EditorParams;
            newDefine.PersisterKey = this.PersisterKey;
            newDefine.SortOrder = this.SortOrder;
            newDefine.MaxLength = this.MaxLength;
            newDefine.IsRequired = this.IsRequired;
            newDefine.ShowTitle = this.ShowTitle;

            newDefine.Validators.CopyFrom(this.Validators);

            return newDefine;
        }
    }
}
