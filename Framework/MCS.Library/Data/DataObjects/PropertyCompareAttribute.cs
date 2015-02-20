using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 属性比较的相关信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyCompareAttribute : Attribute, IPropertyCompareInfo
    {
        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public PropertyCompareAttribute() { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="requireCompare">是否需要进行比较</param>
        public PropertyCompareAttribute(bool requireCompare)
        {
            this.RequireCompare = requireCompare;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="requireCompare">是否需要进行比较</param>
        /// <param name="description">文字描述</param>
        public PropertyCompareAttribute(bool requireCompare, string description)
        {
            this.RequireCompare = requireCompare;
            this.Description = description;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="requireCompare">是否需要进行比较</param>
        /// <param name="sortID">排序</param>
        public PropertyCompareAttribute(bool requireCompare, int sortID)
        {
            this.RequireCompare = requireCompare;
            this.SortID = sortID;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="requireCompare">是否需要进行比较</param>
        /// <param name="sortID">排序</param>
        /// <param name="description">文字描述</param>
        public PropertyCompareAttribute(bool requireCompare, int sortID, string description)
        {
            this.RequireCompare = requireCompare;
            this.SortID = sortID;
            this.Description = description;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="description">文字描述</param>
        public PropertyCompareAttribute(string description)
        {
            this.Description = description;
            this.RequireCompare = true;
        }
        #endregion

        #region Public
        /// <summary>
        /// 是否需要进行比较
        /// </summary>
        public bool RequireCompare
        {
            get;
            set;
        }

        /// <summary>
        /// 展示修改信息时的排序
        /// </summary>
        public int SortID
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
        #endregion
    }
}
