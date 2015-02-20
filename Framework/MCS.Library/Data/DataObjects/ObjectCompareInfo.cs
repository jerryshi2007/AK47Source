using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 对象的比较信息，包括每一个属性的比较信息
    /// </summary>
    [Serializable]
    public class ObjectCompareInfo : IObjectCompareInfo
    {
        private PropertyCompareInfoCollection _Properties = null;

        #region IObjectCompareInfo
        /// <summary>
        /// 关键属性的名称
        /// </summary>
        public string KeyFields
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是列表对象
        /// </summary>
        public bool IsList
        {
            get;
            set;
        }
        #endregion IObjectCompareInfo

        /// <summary>
        /// 属性的比较信息
        /// </summary>
        public PropertyCompareInfoCollection Properties
        {
            get
            {
                if (this._Properties == null)
                    this._Properties = new PropertyCompareInfoCollection();

                return this._Properties;
            }
        }

        /// <summary>
        /// 对象的类型名称
        /// </summary>
        public string ObjectTypeName
        {
            get;
            set;
        }
    }
}
