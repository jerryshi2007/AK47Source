using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest
{
	[TestClass]
	public class CounterTest
	{
		[TestMethod]
		[Description ("操作计数器")]
		[TestCategory(ProcessTestHelper.Resource)]
		public void CounterOperatorTest()
		{
			string id = Guid.NewGuid().ToString();
			Counter.SetCountValue(id, 1);//设置数据库的初始值

			int newId = Counter.NewCountValue(id);//此处会改变数据库中的COUNT_VALUE值 为最新值

			int nextId = Counter.PeekCountValue(id);//只是计算出下个COUNT_VALUE

			Assert.AreEqual(newId + 1, nextId);
		}
	}
}
