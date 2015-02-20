using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.PropertyDefine
{
    public enum ClientPropertyDataType
    {
        /// <summary>
        /// JSON格式，其实还是字符串
        /// </summary>
        DataObject = 1,
        
        /// <summary>
        /// 布尔
        /// </summary>
        Boolean = 3,

        /// <summary>
        /// 整型
        /// </summary>
        Integer = 9,

        /// <summary>
        /// 浮点
        /// </summary>
        Decimal = 15,

        /// <summary>
        /// 时间
        /// </summary>
        DateTime = 16,

        /// <summary>
        /// 文本
        /// </summary>
        String = 18,

        /// <summary>
        /// 枚举
        /// </summary>
        Enum = 20
    }

    [Serializable]
    [DataContract]
    public class ClientPropertyDefine
    {
        public ClientPropertyDefine()
        {
        }

        public bool AllowOverride
        {
            get;
            set;
        }

        public string Category
        {
            get;
            set;
        }

        public ClientPropertyDataType DataType
        {
            get;
            set;
        }

        public string DefaultValue
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public string EditorKey
        {
            get;
            set;
        }

        public string EditorParams
        {
            get;
            set;
        }

        public bool IsRequired
        {
            get;
            set;
        }

        public int MaxLength
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string PersisterKey
        {
            get;
            set;
        }

        public bool ReadOnly
        {
            get;
            set;
        }

        public int SortOrder
        {
            get;
            set;
        }

        public bool Visible
        {
            get;
            set;
        }
    }

    [Serializable]
    public class ClientPropertyDefineCollection : SerializableEditableKeyedDataObjectCollectionBase<string, ClientPropertyDefine>
    {
        public ClientPropertyDefineCollection()
            : base()
        {
        }

        protected ClientPropertyDefineCollection(int capacity)
            : base(capacity)
        {
        }

        protected ClientPropertyDefineCollection(int capacity, IEqualityComparer comparer)
            : base(capacity, comparer)
        {
        }

        protected ClientPropertyDefineCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ClientPropertyDefineCollection(ClientPropertyDefine[] items)
            : this()
        {
            items.NullCheck("items");

            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        protected override string GetKeyForItem(ClientPropertyDefine item)
        {
            return item.Name;
        }
    }
}
