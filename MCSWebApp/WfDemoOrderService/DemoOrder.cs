using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Web.Library.MVC;
using MCS.Library.Validation;

namespace WfDemoService
{
	[Serializable]
	[XElementSerializable]
	[ORTableMapping("WFDEMO.ORDERS")]
	public class DemoOrder : WorkflowObjectBase
	{
		[ORFieldMapping("ORDER_ID")]
		public string OrderID { get; set; }

		[ORFieldMapping("Amount")]
		[IntegerRangeValidator(0, int.MaxValue, MessageTemplate = "合同金额超出范围")]
		public int Amount { get; set; }

		[ORFieldMapping("Currency")]
		public string Currency { get; set; }

		[ORFieldMapping("CONTENT")]
		public string Content { get; set; }

		[ORFieldMapping("CREATOR_ID")]
		public string CreatorID { get; set; }

		[ORFieldMapping("CREATOR_NAME")]
		public string CreatorName { get; set; }

		[ORFieldMapping("CREATE_TIME")]
		public DateTime Create_Time { get; set; }

		[ORFieldMapping("PAYABLE")]
		public bool Payable { get; set; }
	}

	[Serializable]
	[XElementSerializable]
	public class DemoOrderCollection : EditableDataObjectCollectionBase<DemoOrder>
	{
	}

	[Serializable]
	[XElementSerializable]
	public class DemoOrderCommandState : CommandStateBase
	{
		public DemoOrder Data
		{
			get;
			set;
		}
	}

	public sealed class DemoOrderDAO :
		UpdatableAndLoadableAdapterBase<DemoOrder, DemoOrderCollection>
	{
		public static readonly DemoOrderDAO Instance = new DemoOrderDAO();
		public static readonly string DB_CONNECTION = "WFDEMO";
		//private static readonly string SQLCOMMAND_DELETE = @"delete from wf.activity_template where id {0}";

		private DemoOrderDAO() { }

		public void Insert(DemoOrderCollection orders)
		{
			StringBuilder strBuilder = new StringBuilder();

			orders.ForEach(p =>
			{
				if (strBuilder.Length > 0)
					strBuilder.Append(TSqlBuilder.Instance.DBStatementSeperator);

				strBuilder.Append(ORMapping.GetInsertSql(p, TSqlBuilder.Instance));
			});

			if (strBuilder.Length > 0)
				DbHelper.RunSqlWithTransaction(strBuilder.ToString(), DB_CONNECTION);
		}

		public DemoOrder LoadSingleOrder(string demoOrderID)
		{
			var collection = Load(p => p.AppendItem("ID", demoOrderID));

			if (collection.Count == 0) return null;

			return collection[0];
		}

		public DemoOrder LoadByOrderID(string demoOrderID)
		{
			var collection = Load(p => p.AppendItem("ORDER_ID", demoOrderID.Trim()));

			if (collection.Count == 0) return null;

			return collection[0];
		}

		public bool IsPayableOrder(string demoOrderID)
		{
			var collection = Load(p =>
			{
				p.AppendItem("ORDER_ID", demoOrderID);
				p.AppendItem("PAYABLE", 1);
			});

			if (collection.Count == 0) return false;

			return true;
		}

		protected override string GetConnectionName()
		{
			return DB_CONNECTION;
		}

		public static string NewOrderID
		{
			get
			{
				return "OID" + DateTime.Now.ToString("yyyyMMdd") + Counter.NewCountValue("WFDEMO.ORDERS");
			}
		}
	}
}