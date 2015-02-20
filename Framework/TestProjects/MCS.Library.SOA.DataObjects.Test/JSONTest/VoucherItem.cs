using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Test.JSONTest
{
	public class VocherEntity
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
		}

		public static VocherEntity PrepareData()
		{
			VocherEntity result = new VocherEntity() { Name = "Vocher" };

			result.Items.Add(new VoucherItem() { Code = "1001", VoucherCode = "Voucher 1001" });
			result.Items.Add(new VoucherItem() { Code = "1002", VoucherCode = "Voucher 1002" });

			return result;
		}
	}

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
	}

	public class VoucherItemCollection : EditableDataObjectCollectionBase<VoucherItem>
	{
	}
}
