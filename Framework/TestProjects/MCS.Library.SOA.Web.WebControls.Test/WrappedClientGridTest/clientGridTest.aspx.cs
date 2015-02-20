using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.OA.Web.WebControls;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class clientGridTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (this.IsPostBack == false)
                this.clientGrid.InitialData = PrepareInvoices();

			//if (this.IsPostBack == false)
			//    this.clientGrid1.InitialData = WorkItemList();

            base.OnPreRender(e);
        }

        protected static string GetInvoicesJson()
        {
            Invoice[] invoices = PrepareInvoices();

            return JSONSerializerExecute.Serialize(invoices);
        }

        private static Invoice[] PrepareInvoices()
        {
            Invoice[] invoices = new Invoice[] {
				new Invoice {InvoiceNo = "Inv1024", VendorName= "Turtle", InvoiceDate=DateTime.Now, Amount=32768},
				new Invoice{InvoiceNo = "Inv1025", VendorName= "Zhang Yong", InvoiceDate=DateTime.Now, Amount=256.60}
			};

            return invoices;
        }

		//private static WorkItem[] WorkItemList()
		//{
		//    //1dd060ee-7eda-8d28-4a1f-42bcd6e2205c
		//    //906125d1-9cbb-b19e-4369-bb894555b3e6
		//    WorkItemPlatCollection wic = WorkItemAdapter.Instance.Load("906125d1-9cbb-b19e-4369-bb894555b3e6");
		//    return wic.ToArray();
		//}
    }
}