using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.PostProgressControl
{
	public partial class GridPostProgressControlTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.IsPostBack == false && this.IsCallback == false)
			{
				Orders = TestOrderCollection.GenerateRandomOrders();
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			dataGrid.DataSource = Orders;
			dataGrid.DataBind();

			base.OnPreRender(e);
		}

		/// <summary>
		/// 本来应该是数据库存放数据，为了演示，用Session替代一下
		/// </summary>
		private TestOrderCollection Orders
		{
			get
			{
				return (TestOrderCollection)Session["Orders"];
			}
			set
			{
				Session["Orders"] = value;
			}
		}

		protected void uploadProgress_DoPostedData(object sender, PostProgressDoPostedDataEventArgs eventArgs)
		{
			UploadProgressStatus status = new UploadProgressStatus();

			StringBuilder strB = new StringBuilder();

			status.CurrentStep = 1;
			status.MinStep = 0;
			status.MaxStep = eventArgs.Steps.Count;

			for (int i = status.MinStep; i < status.MaxStep; i++)
			{
				//处理订单，改变状态
				string orderID = (string)eventArgs.Steps[i];

				TestOrder order = Orders.Find(o => o.OrderID == orderID);

				if (order != null)
					order.Status = TestOrderStatus.Processed;

				status.CurrentStep = i;
				status.Response();

				if (strB.Length > 0)
					strB.Append("\n");

				strB.AppendFormat("Processed = {0}", (i + 1));
				Thread.Sleep(500);	//假装等待
			}

			status.CurrentStep = status.MaxStep;
			status.Response();

			eventArgs.Result.DataChanged = true;
			eventArgs.Result.CloseWindow = false;
			eventArgs.Result.ProcessLog = strB.ToString();
		}

		protected void dataGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{	
		}

		protected void refreshButton_Click(object sender, EventArgs e)
		{
			this.dataGrid.SelectedKeys.Clear();
		}
	}
}