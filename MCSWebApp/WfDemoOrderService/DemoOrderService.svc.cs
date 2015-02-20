using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WfDemoService
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
	public class DemoOrderService : IDemoOrderService
	{
		public void Update(DemoOrderData data)
		{
			DemoOrderDAO.Instance.Update(DemoOrderAdapter.Contract2Biz(data));
		}

		public DemoOrderData LoadSingleOrder(string id)
		{
			var demoOrder = DemoOrderDAO.Instance.LoadSingleOrder(id);

			if (demoOrder == null) return null;

			return DemoOrderAdapter.Biz2Contract(demoOrder);
		}

		public DemoOrderData LoadByOrderID(string demoOrderID)
		{
			var demoOrder= DemoOrderDAO.Instance.LoadByOrderID(demoOrderID);

			if (demoOrder == null) return null;

			return DemoOrderAdapter.Biz2Contract(demoOrder);
		}

		public bool IsPayableOrder(string demoOrderID)
		{
			return DemoOrderDAO.Instance.IsPayableOrder(demoOrderID);
		}

		public string GetNewOrderID()
		{
			return DemoOrderDAO.NewOrderID;
		}
	}
}
