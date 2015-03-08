using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// Table中每一行中每一列的值
    /// </summary>
    [Serializable]
    public abstract class RowValueBase<TColumnDefinition, TValue> where TColumnDefinition : ColumnDefinitionBase
    {
        /// <summary>
        /// 值
        /// </summary>
        public virtual TValue Value
        {
            get;
            set;
        }

        /// <summary>
        /// 列信息
        /// </summary>
        [NoMapping]
        public virtual TColumnDefinition Column
        {
            get;
            internal protected set;
        }
    }

    /// <summary>
    /// Table中每一行中每一列的值的集合
    /// </summary>
    [Serializable]
    public abstract class RowValueCollectionBase<TColumnDefinition, TRowValue, TValue> : EditableDataObjectCollectionBase<TRowValue>
        where TRowValue : RowValueBase<TColumnDefinition, TValue>
        where TColumnDefinition : ColumnDefinitionBase
    {
        /// <summary>
        /// 根据列名找到行的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TRowValue FindByColumnName(string name)
        {
            name.NullCheck("name");

            TRowValue result = null;

            foreach (TRowValue property in this)
            {
                if (property.Column == null)
                    continue;

                if (string.Compare(property.Column.Name, name, true) == 0)
                {
                    result = property;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 根据列名得到值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(string name, T defaultValue)
        {
            TRowValue cell = FindByColumnName(name);

            T result = defaultValue;

            if (cell != null && cell.Value != null)
                result = (T)DataConverter.ChangeType<object>(cell.Value, typeof(T));

            return result;
        }
    }
}
