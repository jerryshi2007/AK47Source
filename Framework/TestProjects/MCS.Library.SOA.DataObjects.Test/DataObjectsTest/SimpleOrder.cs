using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
	[Serializable]
	public class SimpleOrder
	{
		public string OrderNo { get; set; }
		public string OrderName { get; set; }
	}

	[Serializable]
	public class SimpleOrderCollection : SerializableEditableKeyedDataObjectCollectionBase<string, SimpleOrder>
	{
		protected override string GetKeyForItem(SimpleOrder item)
		{
			return item.OrderNo;
		}
	}
}

