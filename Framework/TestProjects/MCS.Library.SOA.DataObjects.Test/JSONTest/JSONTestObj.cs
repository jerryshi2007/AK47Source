using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Test.JSONTest
{
	public class JSONTestObj
	{
		public string ID
		{
			get;
			set;
		}

		public int Age
		{
			get;
			set;
		}

		public DateTime Birthday
		{
			get;
			set;
		}

		public static JSONTestObj PrepareData()
		{
			JSONTestObj result = new JSONTestObj();

			result.ID = UuidHelper.NewUuidString();
			result.Age = 41;
			result.Birthday = new DateTime(1972, 4, 26, 12, 40, 0, DateTimeKind.Local);

			return result;
		}
	}
}
