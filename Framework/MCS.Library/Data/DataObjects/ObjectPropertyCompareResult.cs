using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 两个对象的属性比较结果
    /// </summary>
    [Serializable]
    public class ObjectPropertyCompareResult
    {
        private IObjectCompareResult _SubObjectCompareResult = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        public ObjectPropertyCompareResult()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="pci"></param>
        /// <param name="sourceValue"></param>
        /// <param name="targetValue"></param>
        public ObjectPropertyCompareResult(PropertyCompareInfo pci, object sourceValue, object targetValue)
        {
            pci.NullCheck("pci");

            this.PropertyName = pci.PropertyName;
            this.PropertyTypeName = pci.PropertyTypeName;
            this.SortID = pci.SortID;
            this.Description = pci.Description;

            this.OldValue = sourceValue;
            this.NewValue = targetValue;
        }

        /// <summary>
        /// 保存修改前的值
        /// </summary>
        public object OldValue
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
        /// 保存修改后的值
        /// </summary>
        public object NewValue
        {
            get;
            set;
        }

        /// <summary>
        /// 属性名称
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

        /// <summary>
        /// 对该属性的文字描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 子对象的比较结果
        /// </summary>
        public IObjectCompareResult SubObjectCompareResult
        {
            get
            {
                return this._SubObjectCompareResult;
            }
            internal set
            {
                this._SubObjectCompareResult = value;
            }
        }
    }
}
