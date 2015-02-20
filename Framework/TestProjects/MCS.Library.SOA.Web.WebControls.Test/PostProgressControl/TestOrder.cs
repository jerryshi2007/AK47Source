using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.SOA.Web.WebControls.Test.PostProgressControl
{
	public enum TestOrderPriority
	{
		Low,
		Normal,
		High
	}

	public enum TestOrderStatus
	{
		Pending,
		Processed
	}

	[Serializable]
	public class TestOrder
	{
		private static readonly string[] UserNames = { "沈峥", "沈嵘", "冯立雷", "危仁飞", "Steve Ballmer", "Kevin Xu", "Kevin Turner", "李琪", "Clark Zheng" };
		private static readonly string[] OrderNames = { "土豆", "茄子", "大菊本", "西红柿", "大红袍", "XBox", "IPhone", "Surface", "Lumia 920", "Latitude E6400", "Mac Book Air" };

		public string OrderID
		{
			get;
			set;
		}

		public int SortID
		{
			get;
			set;
		}

		public string OrderName
		{
			get;
			set;
		}

		private TestOrderPriority _Priority = TestOrderPriority.Normal;

		public TestOrderPriority Priority
		{
			get
			{
				return this._Priority;
			}
			set
			{
				this._Priority = value;
			}
		}

		public DateTime CreateTime
		{
			get;
			set;
		}

		public string CreateUser
		{
			get;
			set;
		}

		public TestOrderStatus Status
		{
			get;
			set;
		}

		public static TestOrder GenerateRandomOrder()
		{
			Random random = new Random((int)DateTime.Now.Ticks);

			TestOrder order = new TestOrder();

			order.OrderID = UuidHelper.NewUuidString();
			order.OrderName = OrderNames[random.Next(OrderNames.Length)];

			order.Priority = TestOrderPriority.Normal;
			order.CreateTime = DateTime.Now.AddHours(random.Next(96) - 48);
			order.CreateUser = UserNames[random.Next(UserNames.Length)]; ;

			return order;
		}
	}

	[Serializable]
	public class TestOrderCollection : EditableDataObjectCollectionBase<TestOrder>
	{
		public static TestOrderCollection GenerateRandomOrders()
		{
			TestOrderCollection result = new TestOrderCollection();

			for (int i = 0; i < 400; i++)
			{
				TestOrder order = TestOrder.GenerateRandomOrder();

				order.SortID = i + 1;
				result.Add(order);
			}

			return result;
		}
	}
}