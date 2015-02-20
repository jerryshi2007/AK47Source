using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Test.DataObjects
{
    [Serializable]
    [ObjectCompare("OrderNumber")]
    public class Order
    {
        private VendorCollection _Vendors = null;

        [PropertyCompare("订单号")]
        public string OrderNumber
        {
            get;
            set;
        }

        [PropertyCompare("供应商们")]
        public VendorCollection Vendors
        {
            get
            {
                this._Vendors = this._Vendors ?? new VendorCollection();

                return this._Vendors;
            }
        }

        [PropertyCompare("创建人")]
        public TestUser Creator
        {
            get;
            set;
        }
    }
}
