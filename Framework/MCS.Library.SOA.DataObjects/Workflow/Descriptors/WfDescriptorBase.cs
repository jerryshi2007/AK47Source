using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Globalization;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// Descriptor公共接口的实现类
    /// 所有的流程描述对象的基类。包括初始化属性操作。
    /// 初始化属性的过程是：
    /// InitProperties
    ///		GetPropertyDefineCollection
    ///			GetCachedPropertyDefineCollection
    ///			GetDefaultPropertyDefineCollection
    ///	AfterDeserialize
    ///		MergeDefinedProperties
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public abstract class WfDescriptorBase : IWfDescriptor
    {

        private IWfProcess _ProcessInstance = null;

        #region IWfDescriptor Members

        public IWfProcess ProcessInstance
        {
            get
            {
                return this._ProcessInstance;
            }
            set
            {
                this._ProcessInstance = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// 工作流基本属性
    /// </summary>
    /// <remarks>
    /// WfDescriptorBase是工作流的基础，其中包含的Key、Name、Description
    /// 是从工作留抽象出来的基本属性，工作流中所有Key、Name、Description均继承于此。
    /// </remarks>
    [Serializable]
    [XElementSerializable]
    [DebuggerDisplay("Key = {Key}, Name= {Name}, Description= {Description}")]
    public abstract class WfKeyedDescriptorBase : WfDescriptorBase, IWfKeyedDescriptor, IXmlDeserialize
    {
        [XElementFieldSerialize(AlternateFieldName = "_Props")]
        private PropertyValueCollection _Properties = null;

        /// <summary>
        /// 
        /// </summary>
        protected WfKeyedDescriptorBase()
        {
            InitProperties();
        }

        /// <summary>
        /// 给流程赋予Key值
        /// </summary>
        /// <param name="key">Key</param>
        /// <remarks>工作流中所有需要的Key值都是由此获得（Process、Activity、Transition）</remarks>
        protected WfKeyedDescriptorBase(string key)
        {
            InitProperties();

            Key = key;
        }

        /// <summary>
        /// 工作流Description属性访问器
        /// </summary>
        /// <remarks>工作流Description属性访问器</remarks>
        public string Description
        {
            get { return Properties.GetValue("Description", string.Empty); }
            set { Properties.SetValue("Description", value); }
        }

        /// <summary>
        /// 工作流Name属性访问器
        /// </summary>
        /// <remarks>工作流Name属性访问器</remarks>
        public string Name
        {
            get { return Properties.GetValue("Name", string.Empty); }
            set { Properties.SetValue("Name", value); }
        }

        /// <summary>
        /// 工作流Key属性访问器
        /// </summary>
        /// <remarks>工作流Key属性访问器</remarks>
        public virtual string Key
        {
            get { return Properties.GetValue("Key", string.Empty); }
            set { Properties.SetValue("Key", value); }
        }

        public bool Enabled
        {
            get { return Properties.GetValue("Enabled", true); }
            set { Properties.SetValue("Enabled", value); }
        }

        public PropertyValueCollection Properties
        {
            get
            {
                if (this._Properties == null)
                    this._Properties = new PropertyValueCollection();

                return this._Properties;
            }
        }

        public virtual void MergeDefinedProperties()
        {
            PropertyDefineCollection definedProperties = GetPropertyDefineCollection();

            this.Properties.MergeDefinedProperties(definedProperties);
        }

        /// <summary>
        /// 同步属性集合到内部成员，保持属性成员的一致性
        /// </summary>
        public virtual void SyncPropertiesToFields()
        {
        }

        internal protected virtual void CloneProperties(WfKeyedDescriptorBase destObject)
        {
            destObject.NullCheck("destObject");

            destObject.Properties.Clear();

            destObject.Properties.CopyFrom(this.Properties, p => p.Clone());

            destObject.ProcessInstance = this.ProcessInstance;
        }

        protected virtual PropertyDefineCollection GetPropertyDefineCollection()
        {
            return GetDefaultPropertyDefineCollection();
        }

        protected PropertyDefineCollection GetDefaultPropertyDefineCollection()
        {
            PropertyDefineCollection pdc = new PropertyDefineCollection();

            pdc.LoadPropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["AllDescriptorProperties"]);

            return pdc;
        }

        protected PropertyDefineCollection GetCachedPropertyDefineCollection(string cacheKey, Func<PropertyDefineCollection> getExtProperties)
        {
            WfActivitySettings settings = WfActivitySettings.GetConfig();

            PropertyDefineCollection pdc = (PropertyDefineCollection)settings.Context[cacheKey];

            if (pdc == null)
            {
                lock (settings.Context.SyncRoot)
                {
                    pdc = (PropertyDefineCollection)settings.Context[cacheKey];

                    if (pdc == null)
                    {
                        pdc = GetDefaultPropertyDefineCollection();

                        if (getExtProperties != null)
                        {
                            PropertyDefineCollection extraProperties = getExtProperties();
                            extraProperties.ForEach(p => pdc.Add(p));
                        }

                        settings.Context[cacheKey] = pdc;
                    }
                }
            }

            return pdc;
        }

        protected virtual void InitProperties()
        {
            PropertyDefineCollection pdc = GetPropertyDefineCollection();

            Properties.InitFromPropertyDefineCollection(pdc);
        }

        #region IXmlDeserialize Members

        public void AfterDeserialize(XmlDeserializeContext context)
        {
            this.MergeDefinedProperties();
        }

        #endregion
    }

    /// <summary>
    /// T类型对象的集合
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <remarks>流程定义集合基类</remarks>
    [Serializable]
    [XElementSerializable]
    public abstract class WfKeyedDescriptorCollectionBase<T> : SerializableEditableKeyedDataObjectCollectionBase<string, T> where T : IWfKeyedDescriptor
    {
        public WfKeyedDescriptorCollectionBase(IWfDescriptor owner)
        {
            this.Owner = owner;
        }

        protected WfKeyedDescriptorCollectionBase(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        public void SyncPropertiesToFields(PropertyValue property)
        {
            if (property != null)
            {
                this.Clear();

                if (property.StringValue.IsNotEmpty())
                {
                    IEnumerable<T> deserializedData = (IEnumerable<T>)JSONSerializerExecute.DeserializeObject(property.StringValue, this.GetType());

                    this.CopyFrom(deserializedData);
                }
            }
        }

        /// <summary>
        /// 集合的所有者
        /// </summary>
        [XElementFieldSerialize(AlternateFieldName = "_Owner")]
        public IWfDescriptor Owner
        {
            get;
            private set;
        }

        protected override string GetKeyForItem(T item)
        {
            return item.Key;
        }

        protected override void OnValidate(object value)
        {
            value.NullCheck("value");

            //if (XElementFormatter.FormattingStatus != XElementFormattingStatus.Deserializing)
            ((IWfKeyedDescriptor)value).Key.IsNotEmpty().FalseThrow<WfDescriptorException>(
                Translator.Translate(WfHelper.CultureCategory, "{0}的Key不能为空", value.GetType().Name));

            base.OnValidate(value);
        }
    }
}
