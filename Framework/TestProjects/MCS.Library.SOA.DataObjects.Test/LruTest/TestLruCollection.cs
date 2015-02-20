using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Test.LruTest
{
	[Serializable]
	[XElementSerializable]
	public class TestLruCollection : LruDataObjectCollectionBase<string>
	{
		public TestLruCollection(int maxLength) :
			base(maxLength)
		{
		}

		public TestLruCollection()
		{
		}
	}
}
