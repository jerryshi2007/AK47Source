using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 对象属性的比较信息
    /// </summary>
    [Serializable]
    public class PropertyCompareInfo : IPropertyCompareInfo
    {
        #region IPropertyCompareInfo
        /// <summary>
        /// 是否需要比较
        /// </summary>
        public bool RequireCompare
        {
            get;
            set;
        }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortID
        {
            get;
            set;
        }

        /// <summary>
        /// 属性描述信息
        /// </summary>
        public string Description
        {
            get;
            set;
        }
        #endregion IPropertyCompareInfo

        /// <summary>
        /// 属性的名称
        /// </summary>
        public string PropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// 属性的类型名称
        /// </summary>
        public string PropertyTypeName
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PropertyCompareInfoCollection : SerializableEditableKeyedDataObjectCollectionBase<string, PropertyCompareInfo>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public PropertyCompareInfoCollection()
        {
        }

        /// <summary>
        /// 序列化构造方法
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected PropertyCompareInfoCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 得到Key
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(PropertyCompareInfo item)
        {
            return item.PropertyName;
        }
    }
}
