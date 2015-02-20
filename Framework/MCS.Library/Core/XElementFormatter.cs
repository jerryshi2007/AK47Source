using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using MCS.Library.Caching;
using MCS.Library.Properties;
using MCS.Library.Reflection;

namespace MCS.Library.Core
{
    /// <summary>
    /// 
    /// </summary>
    public enum XElementFormattingStatus
    {
        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        Serializing,

        /// <summary>
        /// 
        /// </summary>
        Deserializing
    }

    /// <summary>
    /// 对象序列化成XElement
    /// </summary>
    public class XElementFormatter
    {
        private bool _OutputShortType = true;

        /// <summary>
        /// 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event FieldCanXElementSerializeHandler FieldCanXElementSerialize;

        /// <summary>
        /// 序列化时是否输出ShortType
        /// </summary>
        public bool OutputShortType
        {
            get
            {
                return this._OutputShortType;
            }
            set
            {
                this._OutputShortType = value;
            }
        }

        /// <summary>
        /// 序列化时的Binder(类型映射)
        /// </summary>
        public SerializationBinder Binder
        {
            get;
            set;
        }

        /// <summary>
        /// 处理状态
        /// </summary>
        public static XElementFormattingStatus FormattingStatus
        {
            get
            {
                XElementFormattingStatus status = XElementFormattingStatus.None;

                status = (XElementFormattingStatus)ObjectContextCache.Instance.GetOrAddNewValue("XElementFormattingStatus", (cache, key) =>
                {
                    cache.Add(key, XElementFormattingStatus.None);
                    return XElementFormattingStatus.None;
                });

                return status;
            }
            set
            {
                ObjectContextCache.Instance["XElementFormattingStatus"] = value;
            }
        }

        #region 序列化
        /// <summary>
        /// 对象序列化为XElement
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public XElement Serialize(object graph)
        {
            XElementFormatter.FormattingStatus = XElementFormattingStatus.Serializing;

            try
            {
                XElement root = XElement.Parse("<Root/>");

                XmlSerializeContext context = new XmlSerializeContext() { RootElement = root, ContainerTypeName = graph.GetType().AssemblyQualifiedName };

                int objID = context.CurrentID;
                context.ObjectContext.Add(graph, objID);

                XElement graphElement = SerializeObjectToNode(root, graph, context);

                root.SetAttributeValue("v", objID);
                root.SetAttributeValue("r", true);

                context.SerializeTypeInfo(root);

                return root;
            }
            finally
            {
                XElementFormatter.FormattingStatus = XElementFormattingStatus.None;
            }
        }

        private bool OnFieldCanXElementSerialize(ExtendedFieldInfo efi)
        {
            bool result = true;

            if (this.FieldCanXElementSerialize != null)
                result = this.FieldCanXElementSerialize(efi);

            return result;
        }

        private XElement SerializeObjectToNode(XElement parent, object graph, XmlSerializeContext context)
        {
            System.Type type = graph.GetType();

            type.IsSerializable.FalseThrow<SerializationException>("XElementFormatter错误，序列化对象类型时{0}, 子类型{1}不可序列化",
                context.ContainerTypeName, type.AssemblyQualifiedName);

            XElement node = context.RootElement.AddChildElement("O");
            int objID = context.CurrentID++;

            node.SetAttributeValue("id", objID);

            if (this.OutputShortType)
                node.SetAttributeValue("shortType", type.Name);

            node.SetAttributeValue("oti", context.GetTypeID(type));

            if (graph is IXElementSerializable)
            {
                node.SetAttributeValue("cs", "true");
                ((IXElementSerializable)graph).Serialize(node, context);
            }
            else
            {
                if (TypeFields.GetTypeFields(type).XElementSerializable)
                {
                    if (graph is IEnumerable)
                        SerializeIEnumerableObject(node, (IEnumerable)graph, context);

                    SerializePropertiesToNodes(node, graph, context);
                }
                else
                {
                    node.AddChildElement("Value", SerializationHelper.SerializeObjectToString(graph, SerializationFormatterType.Binary));
                }
            }

            return node;
        }

        private void SerializeIEnumerableObject(XElement parent, IEnumerable graph, XmlSerializeContext context)
        {
            //此处对一维数组与list<类型>的处理,其它的都以base64string处理
            Type objectType = graph.GetType();

            if (objectType.IsArray)
            {
                (objectType.GetArrayRank() == 1).FalseThrow<InvalidOperationException>("{0}不是一维数组，我们只支持一维数组", objectType.FullName);

                int rank = GetArrayRank(objectType);
                if (rank == 1)
                {
                    parent.SetAttributeValue("dimLength", GetArrayDimensionsLength(objectType.GetArrayRank(), (Array)graph));
                    SerializeIEnumerableOjbectToNode(graph, context, parent);
                }
                else
                {
                    //嵌套数组
                    parent.AddChildElement("Value", SerializationHelper.SerializeObjectToString(graph, SerializationFormatterType.Binary));
                }
            }
            else
            {
                SerializeIEnumerableOjbectToNode(graph, context, parent);
            }
        }

        private void SerializeIEnumerableOjbectToNode(IEnumerable graph, XmlSerializeContext context, XElement xElement)
        {
            XElement itemsNode = xElement.AddChildElement("Items");

            foreach (object data in graph)
            {
                if (data != null)
                {
                    XElement itemNode = itemsNode.AddChildElement("I");

                    if (Type.GetTypeCode(data.GetType()) != TypeCode.Object)
                    {
                        itemNode.SetAttributeValue("v", data);

                        if (this.OutputShortType)
                            itemNode.SetAttributeValue("shortType", data.GetType().Name);

                        itemNode.SetAttributeValue("oti", context.GetTypeID(data.GetType()));
                    }
                    else
                    {
                        int objID = 0;

                        if (context.ObjectContext.TryGetValue(data, out objID) == false)
                        {
                            objID = context.CurrentID;
                            context.ObjectContext.Add(data, objID);
                            context.ContainerTypeName = graph.GetType().AssemblyQualifiedName;
                            SerializeObjectToNode(xElement, data, context);
                        }

                        itemNode.SetAttributeValue("v", objID);

                        if (this.OutputShortType)
                            itemNode.SetAttributeValue("shortType", data.GetType().Name);

                        itemNode.SetAttributeValue("oti", context.GetTypeID(data.GetType()));
                        itemNode.SetAttributeValue("r", true);
                    }
                }
            }
        }

        private string GetArrayDimensionsLength(int dimensions, Array array)
        {
            StringBuilder strB = new StringBuilder();

            for (int i = 0; i < dimensions; i++)
            {
                if (strB.Length > 0)
                    strB.Append(",");

                strB.Append(array.GetLength(i));
            }
            return strB.ToString();
        }

        private void SerializePropertiesToNodes(XElement parent, object graph, XmlSerializeContext context)
        {
            TypeFields tf = TypeFields.GetTypeFields(graph.GetType());

            foreach (KeyValuePair<TypeFieldInfo, ExtendedFieldInfo> kp in tf.Fields)
            {
                ExtendedFieldInfo efi = kp.Value;

                if (efi.IsNotSerialized == false && OnFieldCanXElementSerialize(efi))
                {
                    object data = GetValueFromObject(efi.FieldInfo, graph);

                    if ((data == null || data == DBNull.Value) == false)
                    {
                        if (Type.GetTypeCode(data.GetType()) == TypeCode.Object)
                        {
                            int objID = 0;

                            if (context.ObjectContext.TryGetValue(data, out objID) == false)
                            {
                                objID = context.CurrentID;
                                context.ObjectContext.Add(data, objID);
                                context.ContainerTypeName = graph.GetType().AssemblyQualifiedName + "." + efi.FieldInfo.Name;

                                SerializeObjectToNode(parent, data, context);
                            }

                            XElement propertyElem = parent.AddChildElement("F");

                            propertyElem.SetAttributeValue("n", efi.AlternateFieldName);
                            propertyElem.SetAttributeValue("v", objID);
                            propertyElem.SetAttributeValue("r", true);
                            propertyElem.SetAttributeValue("oti", context.GetTypeID(kp.Key.ObjectType));

                            if (efi.IgnoreDeserializeError)
                                propertyElem.SetAttributeValue("ide", efi.IgnoreDeserializeError);
                        }
                        else
                        {
                            XElement propertyElem = parent.AddChildElement("F");
                            propertyElem.SetAttributeValue("n", efi.AlternateFieldName);
                            propertyElem.SetAttributeValue("v", data);
                            propertyElem.SetAttributeValue("oti", context.GetTypeID(kp.Key.ObjectType));

                            if (efi.IgnoreDeserializeError)
                                propertyElem.SetAttributeValue("ide", efi.IgnoreDeserializeError);
                        }
                    }
                }
            }
        }

        private void SerializableBinaryToNode(XElement parent, XmlObjectMappingItem item, object graph, XmlSerializeContext context)
        {
            //此处对没有标记为序列化的需要做一个异常处理,还是?
            XElement xElement = context.RootElement.AddChildElement("object");
            xElement.SetAttributeValue("id", context.CurrentID++);

            if (this.OutputShortType)
                xElement.SetAttributeValue("shortType", graph.GetType().Name);

            xElement.SetAttributeValue("oti", context.GetTypeID(graph.GetType()));

            XElement itemNode = xElement.AddChildElement("value");

            byte[] buffer = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, graph);
                buffer = ms.GetBuffer();
            }
            string imageBase64String = Convert.ToBase64String(buffer);

            itemNode.Add(imageBase64String);
        }
        #endregion

        #region 反序列化
        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="root"></param>
        public object Deserialize(XElement root)
        {
            (root != null).FalseThrow<ArgumentNullException>("root");

            XElementFormatter.FormattingStatus = XElementFormattingStatus.Deserializing;
            try
            {
                XmlDeserializeContext context = new XmlDeserializeContext();

                context.RootElement = root;

                context.DeserializeTypeInfo(root);

                XElement objectProperty = GetObjectElementByID(root, root.AttributeWithAlterName("value", "v", 0));

                object result = DeserializeNodeToObject(objectProperty, false, context);

                context.FillListItems();

                return result;
            }
            finally
            {
                XElementFormatter.FormattingStatus = XElementFormattingStatus.None;
            }
        }

        private static XElement GetObjectElementByID(XElement parent, int id)
        {
            XElement objectProperty = (from property in parent.Descendants("O")
                                       where property.Attribute("id", 0) == id
                                       select property).FirstOrDefault();

            if (objectProperty == null)
                objectProperty = (from property in parent.Descendants("Object")
                                  where property.Attribute("id", 0) == id
                                  select property).FirstOrDefault();

            return objectProperty;
        }

        private object DeserializeNodeToObject(XElement objectNode, bool ignoreError, XmlDeserializeContext context)
        {
            object data = null;

            int objID = objectNode.Attribute("id", 0);

            if (context.ObjectContext.TryGetValue(objID, out data) == false)
            {
                try
                {
                    Type type = context.GetTypeInfo(objectNode.AttributeValueWithAlterName("ownerTypeID", "oti"));

                    XElementFieldSerializeAttribute attr = AttributeHelper.GetCustomAttribute<XElementFieldSerializeAttribute>(type);

                    if (attr != null && attr.IgnoreDeserializeError == true)
                        ignoreError = attr.IgnoreDeserializeError;

                    data = CreateSerializableInstance(type);

                    context.ObjectContext.Add(objID, data);

                    if ((data is IXElementSerializable) && objectNode.Attribute("cs", false) == true)
                    {
                        ((IXElementSerializable)data).Deserialize(objectNode, context);
                    }
                    else
                    {
                        string objectValue = objectNode.Element("Value", string.Empty);

                        if (objectValue.IsNotEmpty())
                        {
                            SerializationBinder binder = this.Binder ?? UnknownTypeStrategyBinder.Instance;

                            data = SerializationHelper.DeserializeStringToObject(objectValue, SerializationFormatterType.Binary, binder);
                        }
                        else
                        {
                            DeserializeNodesToProperties(objectNode, data, context);
                        }
                    }

                    if (data is IXmlDeserialize)
                        ((IXmlDeserialize)data).AfterDeserialize(context);
                }
                catch (System.Exception)
                {
                    if (ignoreError == false)
                        throw;
                }
            }

            return data;
        }

        private static int[] CreateDimensionLengthArray(string dimLength)
        {
            string[] lengthDesps = dimLength.Split(',');

            int[] result = new int[lengthDesps.Length];

            for (int i = 0; i < lengthDesps.Length; i++)
                result[i] = int.Parse(lengthDesps[i]);

            return result;
        }

        private static object ConvertData(FieldInfo fi, object data)
        {
            try
            {
                System.Type realType = fi.FieldType;

                if (realType.IsGenericType)
                {
                    Type[] args = realType.GetGenericArguments();
                    realType = args[0];
                }

                return DataConverter.ChangeType(data, realType);
            }
            catch (System.Exception ex)
            {
                throw new SystemSupportException(
                    string.Format(Resource.ConvertXmlNodeToPropertyError,
                        fi.Name, fi.Name, ex.Message),
                        ex
                    );
            }
        }

        private void DeserializeNodesToProperties(XElement parent, object graph, XmlDeserializeContext context)
        {
            TypeFields tf = TypeFields.GetTypeFields(graph.GetType());

            foreach (KeyValuePair<TypeFieldInfo, ExtendedFieldInfo> kp in tf.Fields)
            {
                ExtendedFieldInfo efi = kp.Value;

                if (efi.IsNotSerialized == false && OnFieldCanXElementSerialize(efi))
                {
                    System.Type realType = efi.FieldInfo.FieldType;

                    var propertiesElement = from property in parent.Descendants("F")
                                            where (property.AttributeWithAlterName("name", "n", string.Empty) == efi.AlternateFieldName
                                                || property.AttributeWithAlterName("name", "n", string.Empty) == efi.FieldInfo.Name) &&
                                                context.TypeContext[property.AttributeWithAlterName("ownerTypeID", "oti", -1)] == kp.Key.ObjectType
                                            select property;

                    if (propertiesElement.FirstOrDefault() == null)
                        propertiesElement = from property in parent.Descendants("Field")
                                            where (property.AttributeWithAlterName("name", "n", string.Empty) == efi.AlternateFieldName
                                                || property.AttributeWithAlterName("name", "n", string.Empty) == efi.FieldInfo.Name) &&
                                                context.TypeContext[property.AttributeWithAlterName("ownerTypeID", "oti", -1)] == kp.Key.ObjectType
                                            select property;

                    XElement propertyElement = propertiesElement.FirstOrDefault();

                    if (propertyElement != null)
                    {
                        object data = null;

                        if (propertyElement.AttributeWithAlterName("isRef", "r", false) == true)
                        {
                            XElement objectElement = GetObjectElementByID(context.RootElement, propertyElement.AttributeWithAlterName("value", "v", 0));

                            if (objectElement != null)
                            {
                                bool ignoreError = propertyElement.Attribute("ide", false);

                                if (ignoreError == false)
                                {
                                    XElementFieldSerializeAttribute attr = AttributeHelper.GetCustomAttribute<XElementFieldSerializeAttribute>(efi.FieldInfo);

                                    if (attr != null)
                                        ignoreError = attr.IgnoreDeserializeError;
                                }

                                data = DeserializeNodeToObject(objectElement, ignoreError, context);

                                SetValueToObject(efi.FieldInfo, graph, data);
                            }
                        }
                        else
                        {
                            data = propertyElement.AttributeWithAlterName("value", "v", TypeCreator.GetTypeDefaultValue(realType));

                            if (Convertible(realType, data))
                                SetValueToObject(efi.FieldInfo, graph, ConvertData(efi.FieldInfo, data));
                        }
                    }
                }
            }

            DeserializeXmlSerilizableList(parent, graph, context);
        }

        private void DeserializeXmlSerilizableList(XElement parent, object graph, XmlDeserializeContext context)
        {
            if (graph is IXmlSerilizableList)
            {
                var valueNodes = from vNodes in parent.Descendants("Items").Descendants("I")
                                 select vNodes;

                if (valueNodes.FirstOrDefault() == null)
                    valueNodes = from vNodes in parent.Descendants("Items").Descendants("Item")
                                 select vNodes;

                IXmlSerilizableList list = (IXmlSerilizableList)graph;

                list.Clear();

                //DeserializeNodeToCollection(valueNodes, context, (i, itemData) => list.Add(itemData));
                DeserializeNodeToCollection(valueNodes, context, (i, itemData) =>
                {
                    context.ListItems.Add(new XmlListDeserializeItem(list, itemData));
                });
            }
        }

        private void DeserializeNodeToCollection(IEnumerable<XElement> valueNodes, XmlDeserializeContext context, Action<int, object> action)
        {
            int i = 0;

            foreach (XElement item in valueNodes)
            {
                object itemData = null;

                string elementID = item.Attribute("v").Value;

                if (item.AttributeWithAlterName("isRef", "r", false) == true)
                {
                    XElement objectElement = GetObjectElementByID(context.RootElement, item.AttributeWithAlterName("value", "v", 0));
                    itemData = DeserializeNodeToObject(objectElement, false, context);
                }
                else
                {
                    Type type = context.GetTypeInfo(item.AttributeWithAlterName("ownerTypeID", "oti", string.Empty));

                    itemData = item.AttributeWithAlterName("value", "v", TypeCreator.GetTypeDefaultValue(type));

                    itemData = DataConverter.ChangeType(itemData, type);
                }

                action(i, itemData);

                i++;
            }
        }

        private static bool Convertible(System.Type targetType, object data)
        {
            bool result = true;

            if (data == null && targetType.IsValueType)
                result = false;
            else
            {
                if (data == DBNull.Value)
                {
                    if (targetType != typeof(DBNull) && targetType != typeof(string))
                        result = false;
                }
            }

            return result;
        }
        #endregion 反序列化

        #region 获取属性
        private static object GetValueFromObject(FieldInfo fi, object graph)
        {
            object data = null;

            if (graph != null)
            {
                //data = DynamicFieldValueAccessor.Instance.GetValue(fi.DeclaringType, graph, fi.Name);
                data = fi.GetValue(graph);

                if (data != null)
                {
                    System.Type dataType = data.GetType();
                    if (dataType.IsEnum)
                    {
                        data = data.ToString();
                    }
                    else
                        if (dataType == typeof(TimeSpan))
                            data = ((TimeSpan)data).TotalSeconds;
                }
            }

            return data;
        }

        private static void SetValueToObject(FieldInfo fi, object graph, object data)
        {
            fi.SetValue(graph, data);
            //DynamicFieldValueAccessor.Instance.SetValue(fi.DeclaringType, graph, fi.Name, data);
        }

        private static object CreateSerializableInstance(string typeDescription, params object[] constructorParams)
        {
            Type type = TypeCreator.GetTypeInfo(typeDescription);

            ExceptionHelper.FalseThrow<TypeLoadException>(type != null, Resource.TypeLoadException, typeDescription);

            return CreateSerializableInstance(type, constructorParams);
        }

        /// <summary>
        /// 根据类型信息创建对象，该对象即使没有公有的构造方法，也可以创建实例
        /// </summary>
        /// <param name="type">创建类型时的类型信息</param>
        /// <param name="constructorParams">创建实例的初始化参数</param>
        /// <returns>实例对象</returns>
        /// <remarks>运用晚绑定方式动态创建一个实例</remarks>
        public static object CreateSerializableInstance(System.Type type, params object[] constructorParams)
        {
            type.IsSerializable.FalseThrow<SerializationException>("XElementFormatter错误，类型{0}不可序列化", type.AssemblyQualifiedName);

            return TypeCreator.CreateInstance(type, constructorParams);
        }
        #endregion 获取属性

        #region

        private static int GetArrayRank(Type type)
        {
            Regex rgx = new Regex(@"(\[\])", RegexOptions.IgnorePatternWhitespace);
            string s = type.ToString();
            MatchCollection mchs = rgx.Matches(s);
            return mchs.Count;
        }

        private bool ContainDictionary(Type objectType)
        {
            Regex rgx = new Regex(@"(\System.Collections.Generic.Dictionary)", RegexOptions.IgnorePatternWhitespace);
            string s = objectType.ToString();
            return rgx.IsMatch(s);
        }

        #endregion
    }
}
