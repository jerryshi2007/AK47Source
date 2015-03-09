using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using System.Runtime.Serialization;
using System.Collections;
using MCS.Library.Core;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// Table定义中的列定义
    /// </summary>
    [Serializable]
    public abstract class ColumnDefinitionBase
    {
        private ColumnDataType _DataType = ColumnDataType.String;
        private string _DefaultValue = (string)null;

        /// <summary>
        /// 默认值
        /// </summary>
        public virtual string DefaultValue
        {
            get
            {
                return this._DefaultValue;
            }
            set
            {
                this._DefaultValue = value;
            }
        }

        /// <summary>
        /// 实际的数据类型
        /// </summary>
        [NoMapping]
        public virtual Type RealDataType
        {
            get
            {
                return this.GetRealType(this.DataType);
            }
        }

        /// <summary>
        /// 默认的数据类型
        /// </summary>
        public virtual ColumnDataType DataType
        {
            get
            {
                return this._DataType;
            }
            set
            {
                this._DataType = value;
            }
        }

        /// <summary>
        /// 列名称
        /// </summary>
        public virtual string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 列标题
        /// </summary>
        public virtual string Caption
        {
            get;
            set;
        }

        private Type GetRealType(ColumnDataType dataType)
        {
            Type result = typeof(string);

            switch (dataType)
            {
                case ColumnDataType.Boolean:
                    result = typeof(bool);
                    break;
                case ColumnDataType.DataObject:
                    result = typeof(object);
                    break;
                case ColumnDataType.DateTime:
                    result = typeof(DateTime);
                    break;
                case ColumnDataType.Decimal:
                    result = typeof(decimal);
                    break;
                case ColumnDataType.Integer:
                    result = typeof(int);
                    break;
                case ColumnDataType.String:
                    result = typeof(string);
                    break;
                case ColumnDataType.Enum:
                    throw new ApplicationException("不支持ColumnDataType.Enum的数据类型");
            }

            return result;
        }
    }

    /// <summary>
    /// Table中的列定义集合
    /// </summary>
    /// <typeparam name="TColumnDefinition"></typeparam>
    [Serializable]
    public abstract class ColumnDefinitionCollectionBase<TColumnDefinition> : SerializableEditableKeyedDataObjectCollectionBase<string, TColumnDefinition> where TColumnDefinition : ColumnDefinitionBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ColumnDefinitionCollectionBase() :
            base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// 得到列的默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetColumnDefaultValue<T>(string name, T defaultValue)
        {
            T result = defaultValue;

            if (this.ContainsKey(name))
            {
                object data = this[name].DefaultValue;

                if (data != null)
                    result = (T)DataConverter.ChangeType(data, typeof(T));
            }

            return result;
        }

        /// <summary>
        /// 构造方法，指定集合的Key比较器
        /// </summary>
        /// <param name="comparer"></param>
        protected ColumnDefinitionCollectionBase(IEqualityComparer comparer)
            : base(comparer)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ColumnDefinitionCollectionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 构造方法。集合增加时的分配冗余
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="comparer"></param>
        protected ColumnDefinitionCollectionBase(int capacity, IEqualityComparer comparer)
            : base(capacity, comparer)
        {
            this._Comparer = comparer;
        }

        /// <summary>
        /// 从列定义中获取Key
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(TColumnDefinition item)
        {
            return item.Name;
        }
    }
}
