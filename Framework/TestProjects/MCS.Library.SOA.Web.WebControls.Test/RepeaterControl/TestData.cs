using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;

namespace MCS.Library.SOA.Web.WebControls.Test.RepeaterControl
{
	public class TestData
	{
		public string ID
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public static List<TestData> PrepareData()
		{
			List<TestData> result = new List<TestData>();

			result.Add(new TestData() { ID = UuidHelper.NewUuidString(), Name = "Eric" });
			result.Add(new TestData() { ID = UuidHelper.NewUuidString(), Name = "Ray" });
			result.Add(new TestData() { ID = UuidHelper.NewUuidString(), Name = "Kevin" });
			result.Add(new TestData() { ID = UuidHelper.NewUuidString(), Name = "Clark" });

			return result;
		}
	}
}