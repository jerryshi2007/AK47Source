using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Web.Library.Script.Json.Test
{
    /// <summary>
    /// 用于测试的对象实体
    /// </summary>
    public class VoucherEntity
    {
        public string Name
        {
            get;
            set;
        }

        private VoucherItemCollection _Items = null;

        public VoucherItemCollection Items
        {
            get
            {
                if (this._Items == null)
                    this._Items = new VoucherItemCollection();

                return this._Items;
            }
            set
            {
                this._Items = value;
            }
        }

        public static VoucherEntity PrepareData()
        {
            VoucherEntity result = new VoucherEntity() { Name = "Voucher" };

            result.Items.Add(new VoucherItem() { Code = "1001", VoucherCode = "Voucher 1001", CreateTime = new DateTime(1972, 4, 26, 12, 40, 0, DateTimeKind.Local) });
            result.Items.Add(new VoucherItem() { Code = "1002", VoucherCode = "Voucher 1002", CreateTime = new DateTime(1972, 4, 26, 13, 00, 0, DateTimeKind.Local) });

            result.Items.CollectioName = "TestCollection";

            return result;
        }
    }

    /// <summary>
    /// 子对象集合
    /// </summary>
    public class VoucherItem
    {
        public string Code
        {
            get;
            set;
        }

        public string VoucherCode
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }
    }

    public class VoucherItemCollection : EditableDataObjectCollectionBase<VoucherItem>
    {
        public string CollectioName
        {
            get;
            set;
        }
    }
}
