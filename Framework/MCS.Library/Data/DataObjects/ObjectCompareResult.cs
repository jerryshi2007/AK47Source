using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 对象的每个属性的比较结果
    /// </summary>
    [Serializable]
    public class ObjectCompareResult : SerializableEditableKeyedDataObjectCollectionBase<string, ObjectPropertyCompareResult>, IObjectCompareResult, ISimpleXmlSerializer, ISimpleXmlDeserializer
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="objectTypeName">对象的类型名称</param>
        public ObjectCompareResult(string objectTypeName)
        {
            this.ObjectTypeName = objectTypeName;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public ObjectCompareResult()
        {
        }

        /// <summary>
        /// 序列化构造方法
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ObjectCompareResult(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ObjectTypeName = info.GetString("ObjectTypeName");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ObjectTypeName", this.ObjectTypeName);

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(ObjectPropertyCompareResult item)
        {
            return item.PropertyName;
        }

        /// <summary>
        /// 对象的比较结果是否存在差异
        /// </summary>
        public bool AreDifferent
        {
            get
            {
                return this.Any();
            }
        }

        /// <summary>
        /// 对象的类型名称
        /// </summary>
        public string ObjectTypeName
        {
            get;
            private set;
        }

        /// <summary>
        /// 参与比较的对象是否是可列举的（集合）
        /// </summary>
        public bool AreEnumerable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 根据属性的原始值查找比较项
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public ObjectPropertyCompareResult FindByOldPropertyValue(string propertyName, object propertyValue)
        {
            return this.FindPropertyValue(propertyName, (pcr) => pcr.OldValue == propertyValue);
        }

        /// <summary>
        /// 根据的属性的新值查找比较项
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public ObjectPropertyCompareResult FindByNewPropertyValue(string propertyName, object propertyValue)
        {
            return this.FindPropertyValue(propertyName, (pcr) => pcr.NewValue == propertyValue);
        }

        /// <summary>
        /// 将整个对象序列化为一个XElement
        /// </summary>
        /// <returns></returns>
        public XElement ToXElement()
        {
            XElement root = XElement.Parse("<Object />");

            this.ToXElement(root, string.Empty);

            return root;
        }

        /// <summary>
        /// 将对象序列化到一个XElement元素上
        /// </summary>
        /// <param name="element"></param>
        /// <param name="refNodeName">可参照的节点名称，如果此参数不为空，则增加此名称的子节点</param>
        public void ToXElement(XElement element, string refNodeName)
        {
            element.NullCheck("element");

            if (refNodeName.IsNotEmpty())
                element = element.AddChildElement(refNodeName);

            this.ObjectTypeName.IsNotEmpty(tn => element.SetAttributeValue("typeName", tn));

            foreach (ObjectPropertyCompareResult pcr in this)
            {
                XElement propElement = element.AddChildElement("Property");

                propElement.SetAttributeValue("name", pcr.PropertyName);

                pcr.PropertyTypeName.IsNotEmpty(tn => propElement.SetAttributeValue("typeName", tn));
                propElement.SetAttributeValue("sortID", pcr.SortID);
                pcr.Description.IsNotEmpty(desp => propElement.SetAttributeValue("description", desp));

                pcr.OldValue.IsNotNull(oldValue => propElement.SetAttributeValue("oldValue", oldValue));
                pcr.NewValue.IsNotNull(newValue => propElement.SetAttributeValue("newValue", newValue));

                IObjectCompareResult subResult = pcr.SubObjectCompareResult;

                if (subResult.AreDifferent && subResult is ISimpleXmlSerializer)
                    ((ISimpleXmlSerializer)subResult).ToXElement(propElement, "Object");
            }
        }

        /// <summary>
        /// 从XElement序列化为对象的内容
        /// </summary>
        /// <param name="root"></param>
        public void FromXElement(XElement root)
        {
            this.FromXElement(root, string.Empty);
        }

        /// <summary>
        /// 从一个XElement元素序列化对象
        /// </summary>
        /// <param name="element"></param>
        /// <param name="refNodeName">可参照的节点名称，如果此参数不为空，则从此名称的子节点序列化对象</param>
        public void FromXElement(XElement element, string refNodeName)
        {
            this.Clear();

            if (element != null)
            {
                if (refNodeName.IsNotEmpty())
                    element = element.Element(refNodeName);
            }

            if (element != null)
            {
                this.ObjectTypeName = element.Attribute("typeName", string.Empty);

                foreach (XElement propElement in element.Elements("Property"))
                {
                    ObjectPropertyCompareResult pcr = new ObjectPropertyCompareResult();

                    pcr.PropertyName = propElement.Attribute("name", string.Empty);
                    pcr.PropertyTypeName = propElement.Attribute("typeName", string.Empty);
                    pcr.SortID = propElement.Attribute("sortID", 0);
                    pcr.Description = propElement.Attribute("description", string.Empty);

                    pcr.OldValue = propElement.Attribute("oldValue", (string)null);
                    pcr.NewValue = propElement.Attribute("newValue", (string)null);

                    XElement subElement = propElement.Element("Object");

                    if (subElement != null)
                    {
                        string objectTypeName = subElement.Attribute("typeName", string.Empty);

                        if (subElement.Element("Added") != null)
                            pcr.SubObjectCompareResult = new ObjectCollectionCompareResult(objectTypeName);
                        else
                            pcr.SubObjectCompareResult = new ObjectCompareResult();

                        ((ISimpleXmlDeserializer)pcr.SubObjectCompareResult).FromXElement(subElement, string.Empty);
                    }
                    else
                        pcr.SubObjectCompareResult = new ObjectCompareResult();

                    this.Add(pcr);
                }
            }
        }

        private ObjectPropertyCompareResult FindPropertyValue(string propertyName, Predicate<ObjectPropertyCompareResult> comparer)
        {
            propertyName.CheckStringIsNullOrEmpty("propertyName");

            ObjectPropertyCompareResult pcr = this.Find(r => r.PropertyName == propertyName);

            if (pcr != null)
                if (comparer(pcr) == false)
                    pcr = null;

            return pcr;
        }
    }

    /// <summary>
    /// 对象比较结果的集合
    /// </summary>
    [Serializable]
    public class ObjectCompareResultCollection : EditableDataObjectCollectionBase<ObjectCompareResult>, ISimpleXmlSerializer, ISimpleXmlDeserializer
    {
        /// <summary>
        /// 将对象序列化到一个XElement元素上
        /// </summary>
        /// <param name="element"></param>
        /// <param name="refNodeName">可参照的节点名称，如果此参数不为空，则增加此名称的子节点</param>
        public void ToXElement(XElement element, string refNodeName)
        {
            element.NullCheck("element");

            if (refNodeName.IsNotEmpty())
                element = element.AddChildElement(refNodeName);

            foreach (ObjectCompareResult compareResult in this)
                compareResult.ToXElement(element, "Object");
        }

        /// <summary>
        /// 从XElement元素反序列化集合对象
        /// </summary>
        /// <param name="element"></param>
        /// <param name="refNodeName"></param>
        public void FromXElement(XElement element, string refNodeName)
        {
            this.Clear();

            if (element != null)
            {
                if (refNodeName.IsNotEmpty())
                    element = element.Element(refNodeName);
            }

            foreach (XElement objElement in element.Elements("Object"))
            {
                ObjectCompareResult compareResult = new ObjectCompareResult();

                compareResult.FromXElement(objElement, string.Empty);

                this.Add(compareResult);
            }
        }
    }
}
