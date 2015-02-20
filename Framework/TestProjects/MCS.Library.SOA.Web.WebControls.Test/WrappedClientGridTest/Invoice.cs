using System;
using System.Collections.Generic;
using System.Web;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
	public class Invoice
	{
		public string InvoiceNo { get; set; }
		public DateTime InvoiceDate { get; set; }
		public string VendorName { get; set; }
		public double Amount { get; set; }
	}
}
