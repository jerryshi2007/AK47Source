using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WfDemoService
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
	[ServiceContract]
	public interface IDemoOrderService
	{
		[OperationContract]
		void Update(DemoOrderData data);

		[OperationContract]
		DemoOrderData LoadSingleOrder(string id);

		[OperationContract]
		DemoOrderData LoadByOrderID(string demoOrderID);

		[OperationContract]
		bool IsPayableOrder(string demoOrderID);

		[OperationContract]
		string GetNewOrderID();
	}

	[DataContract]
	public class DemoOrderData
	{
		[DataMember]
		public string ID { get; set; }
		[DataMember]
		public string Subject { get; set; }
		[DataMember]
		public string OrderID { get; set; }
		[DataMember]
		public int Amount { get; set; }
		[DataMember]
		public string Currency { get; set; }
		[DataMember]
		public string Content { get; set; }
		[DataMember]
		public string CreatorID { get; set; }
		[DataMember]
		public string CreatorName { get; set; }
		[DataMember]
		public DateTime Create_Time { get; set; }
		[DataMember]
		public bool Payable { get; set; }
	}
}
