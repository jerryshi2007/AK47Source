using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WfDemoService
{
	public class DemoOrderAdapter
	{
		public static DemoOrder Contract2Biz(DemoOrderData data)
		{
			if (data == null) return null;

			return new DemoOrder()
			{
				ID = data.ID,
				Subject = data.Subject,
				OrderID = data.OrderID,
				Content = data.Content,
				Amount = data.Amount,
				Create_Time = data.Create_Time,
				CreatorID = data.CreatorID,
				CreatorName = data.CreatorName,
				Currency = data.Currency,
				Payable = data.Payable
			};
		}

		public static DemoOrderData Biz2Contract(DemoOrder data)
		{
			if (data == null) return null;

			return new DemoOrderData()
			{
				ID = data.ID,
				Subject = data.Subject,
				OrderID = data.OrderID,
				Content = data.Content,
				Amount = data.Amount,
				Create_Time = data.Create_Time,
				CreatorID = data.CreatorID,
				CreatorName = data.CreatorName,
				Currency = data.Currency,
				Payable = data.Payable
			};
		}
	}
}