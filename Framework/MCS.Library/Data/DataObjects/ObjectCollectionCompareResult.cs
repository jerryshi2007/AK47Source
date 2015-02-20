using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 集合对象的比较结果
    /// </summary>
    [Serializable]
    public class ObjectCollectionCompareResult : IObjectCompareResult, ISimpleXmlSerializer, ISimpleXmlDeserializer
    {
        private ObjectCompareResultCollection _Added = null;
        private ObjectCompareResultCollection _Updated = null;
        private ObjectCompareResultCollection _Deleted = null;

        /// <summary>
        /// 对象的类型名称
        /// </summary>
        /// <param name="objectTypeName"></param>
        public ObjectCollectionCompareResult(string objectTypeName)
        {
            this.ObjectTypeName = objectTypeName;
        }

        /// <summary>
        /// 对象的比较结果是否存在差异
        /// </summary>
        public bool AreDifferent
        {
            get
            {
                return this.Added.Any() || this.Updated.Any() || this.Deleted.Any();
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
                return true;
            }
        }

        /// <summary>
        /// 新增的对象
        /// </summary>
        public ObjectCompareResultCollection Added
        {
            get
            {
                this._Added = this._Added ?? new ObjectCompareResultCollection();

                return this._Added;
            }
        }

        /// <summary>
        /// 修改的对象
        /// </summary>
        public ObjectCompareResultCollection Updated
        {
            get
            {
                this._Updated = this._Updated ?? new ObjectCompareResultCollection();

                return this._Updated;
            }
        }

        /// <summary>
        /// 删除的对象
        /// </summary>
        public ObjectCompareResultCollection Deleted
        {
            get
            {
                this._Deleted = this._Deleted ?? new ObjectCompareResultCollection();

                return this._Deleted;
            }
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

            this.Added.ToXElement(element, "Added");
            this.Updated.ToXElement(element, "Updated");
            this.Deleted.ToXElement(element, "Deleted");
        }

        /// <summary>
        /// 从XElement元素反序列化集合对象
        /// </summary>
        /// <param name="element"></param>
        /// <param name="refNodeName"></param>
        public void FromXElement(XElement element, string refNodeName)
        {
            if (element != null)
            {
                if (refNodeName.IsNotEmpty())
                    element = element.Element(refNodeName);
            }

            if (element != null)
            {
                this.Added.FromXElement(element, "Added");
                this.Updated.FromXElement(element, "Updated");
                this.Deleted.FromXElement(element, "Deleted");
            }
        }
    }
}
