using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MCS.Library.Core
{
    /// <summary>
    /// 决定属性是否可以序列化的的委托定义
    /// </summary>
    /// <param name="efi"></param>
    /// <returns></returns>
    public delegate bool FieldCanXElementSerializeHandler(ExtendedFieldInfo efi);

    /// <summary>
    /// 
    /// </summary>
    public class XmlSerializeContext
    {
        private Dictionary<object, int> _ObjectContext = new Dictionary<object, int>();
        private Dictionary<Type, int> _TypeContext = new Dictionary<Type, int>();

        private int _CurrentID = 0;
        private int _CurrentTypeID = 0;

        internal XElement RootElement
        {
            get;
            set;
        }

        internal string ContainerTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 当前对象ID
        /// </summary>
        public int CurrentID
        {
            get { return _CurrentID; }
            set { _CurrentID = value; }
        }
        /// <summary>
        /// 实例上下文
        /// </summary>
        public Dictionary<object, int> ObjectContext
        {
            get { return _ObjectContext; }
            set { _ObjectContext = value; }
        }
        /// <summary>
        /// 类型上下文
        /// </summary>
        public Dictionary<Type, int> TypeContext
        {
            get { return _TypeContext; }
            set { _TypeContext = value; }
        }

        internal int GetTypeID(System.Type type)
        {
            int result = -1;

            if (TypeContext.TryGetValue(type, out result) == false)
            {
                result = _CurrentTypeID++;
                TypeContext.Add(type, result);
            }

            return result;
        }

        internal void SerializeTypeInfo(XElement parent)
        {
            XElement typesNode = parent.AddChildElement("Types");

            foreach (KeyValuePair<Type, int> kp in TypeContext)
            {
                XElement typeNode = typesNode.AddChildElement("T");

                typeNode.SetAttributeValue("id", kp.Value);
                typeNode.SetAttributeValue("n", kp.Key.AssemblyQualifiedName);
            }
        }
    }

    /// <summary>
    /// IXmlSerilizableList类型的列表的操作项
    /// </summary>
    internal class XmlListDeserializeItem
    {
        public XmlListDeserializeItem()
        {
        }

        public XmlListDeserializeItem(IXmlSerilizableList list, object item)
        {
            this.List = list;
            this.Item = item;
        }

        public IXmlSerilizableList List
        {
            get;
            set;
        }

        public object Item
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class XmlDeserializeContext
    {
        internal XElement RootElement
        {
            get;
            set;
        }

        internal List<XmlListDeserializeItem> ListItems
        {
            get
            {
                return this._ListItems;
            }
        }

        private Dictionary<int, object> _ObjectContext = new Dictionary<int, object>();
        private Dictionary<int, Type> _TypeContext = new Dictionary<int, Type>();
        private List<XmlListDeserializeItem> _ListItems = new List<XmlListDeserializeItem>();

        /// <summary>
        /// 实例上下文
        /// </summary>
        public Dictionary<int, object> ObjectContext
        {
            get
            {
                return this._ObjectContext;
            }
            set
            {
                this._ObjectContext = value;
            }
        }

        /// <summary>
        /// 类型上下文
        /// </summary>
        public Dictionary<int, Type> TypeContext
        {
            get
            {
                return this._TypeContext;
            }
            set
            {
                this._TypeContext = value;
            }
        }

        internal void FillListItems()
        {
            this.ListItems.ForEach(itemAction => itemAction.List.Add(itemAction.Item));
        }

        internal Type GetTypeInfo(int id)
        {
            Type result = null;

            TypeContext.TryGetValue(id, out result).FalseThrow("不能找到ID为{0}的Type", id);

            return result;
        }

        internal Type GetTypeInfo(string strID)
        {
            int id = -1;

            int.TryParse(strID, out id).FalseThrow("不能将{0}转换为整数", strID);

            return GetTypeInfo(id);
        }

        internal void DeserializeTypeInfo(XElement parent)
        {
            var types = from typeNodes in parent.Descendants("Types").Descendants("T")
                        select new
                        {
                            ID = typeNodes.Attribute("id", -1),
                            TypeDesp = typeNodes.AttributeWithAlterName("name", "n", string.Empty),
                        };

            if (types.FirstOrDefault() == null)
                types = from typeNodes in parent.Descendants("Types").Descendants("Type")
                        select new
                        {
                            ID = typeNodes.Attribute("id", -1),
                            TypeDesp = typeNodes.AttributeWithAlterName("name", "n", string.Empty),
                        };

            types.ForEach(t =>
            {
                try
                {
                    TypeContext[t.ID] = TypeCreator.GetTypeInfo(t.TypeDesp);
                }
                catch (System.Exception)
                {
                }
            });
        }
    }

    /// <summary>
    /// Xml反序列化的相关接口
    /// </summary>
    public interface IXmlDeserialize
    {
        /// <summary>
        /// 反序列化完成
        /// </summary>
        /// <param name="context">上下文对象，暂时内部使用</param>
        void AfterDeserialize(XmlDeserializeContext context);
    }
}
