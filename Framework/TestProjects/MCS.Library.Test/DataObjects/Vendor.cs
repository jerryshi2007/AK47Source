using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Test.DataObjects
{
    /// <summary>
    /// 用于测试对象比较的供应商类
    /// </summary>
    [Serializable]
    [ObjectCompare("Code")]
    public class Vendor
    {
        /// <summary>
        /// 不参与比较
        /// </summary>
        public string VendorID { get; set; }

        [PropertyCompare(true, "Vendor Code")]
        public string Code { get; set; }

        [PropertyCompare(true, "Vendor Name")]
        public string Name { get; set; }

        [PropertyCompare(true, "Vendor Description")]
        public string Description { get; set; }

        [PropertyCompare(true, "Type Name")]
        public string TypeName { get; set; }

        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// 用于测试对象比较的供应商集合类
    /// </summary>
    [Serializable]
    public class VendorCollection : EditableDataObjectCollectionBase<Vendor>
    {
    }
}
