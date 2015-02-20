using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
	public partial class ClientGridTest1 : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			 
		}

		protected void button_OnClick(object sender,EventArgs e)
		{
			this.TextBox1.Text = this.TextBox2.Text;
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!this.IsPostBack)
			{
				this.clientGrid.InitialData = DataSource();
				this.clientGrid1.InitialData = DataSource();
				this.clientGrid12.InitialData = DataSource();
				this.AppliedBankAccountGrid.InitialData = GetDataSource();
			}

			base.OnPreRender(e);
		}

		private IList GetDataSource()
		{
			List<Fund> funds = new List<Fund>();
			for (int i = 0; i < 1; i++)
			{
				Fund f = new Fund();
				f.index = i.ToString ();
				f.BankAccountName = "asf";
				f.ReasonOfNotDefaultCashPool = "asf";
				f.ReasonOfNotCashPoolBank = "sf";
				f.ApplicationReason = "rrr";
				funds.Add(f);
			}
			return funds;
		}



		private List<Book> DataSource()
		{
			List<Book> list = new List<Book>();
			for (int i = 0; i < 3; i++)
			{
				Book book = new Book();
				book.BookNo = Guid.NewGuid().ToString().Substring(0, 6);
				book.BookTitle = i.ToString() + "科全书";
				book.PublishDate = DateTime.Now;
				book.Amount = i * 10;
                book.Money = Convert.ToDecimal(i) * Convert.ToDecimal(1.091);
				book.Remark = "slfalfkjlsjfdsk";
				book.Category = "DINGDANG";
				list.Add(book);
			}
			return list;
		}

        protected void Button2_Click(object sender, EventArgs e)
        {
            IList list = this.clientGrid.InitialData;
        }


	}

	public class Book
	{
		//编号
		public string BookNo { get; set; }
		//标题
		public string BookTitle { get; set; }
		//发布日期
		public DateTime PublishDate { get; set; }
		//数量
		public int Amount { get; set; }
		//金额
		public decimal Money { get; set; }

		public string Remark { get; set; }

		public string Category { get; set; }
	}

	public class Fund
	{
		public string index { get; set; }
		public string BankAccountName { get; set; }
		public string ReasonOfNotDefaultCashPool { get; set; }
		public string ReasonOfNotCashPoolBank { get; set; }
		public string ApplicationReason { get; set; }
	}

}