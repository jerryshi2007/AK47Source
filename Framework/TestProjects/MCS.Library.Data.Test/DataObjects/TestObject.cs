using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;

namespace MCS.Library.Data.Test.DataObjects
{
	[ORTableMapping("TEST_TABLE")]
	[Serializable]
	public class TestObject
	{
		[ORFieldMapping("ID", PrimaryKey = true)]
		public string ID
		{
			get;
			set;
		}

		[ORFieldMapping("NAME")]
		public string Name
		{
			get;
			set;
		}

		[PropertyEncryption]
		[ORFieldMapping("AMOUNT")]
		public Decimal Amount
		{
			get;
			set;
		}
	}
}
