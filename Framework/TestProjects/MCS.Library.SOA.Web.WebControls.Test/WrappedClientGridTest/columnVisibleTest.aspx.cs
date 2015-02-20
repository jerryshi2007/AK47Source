using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class columnVisibleTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.clientGrid1.Columns[2].Visible = false;
            this.clientGrid1.Columns[3].Visible = false;

            this.clientGrid1.InitialData = dataSource();
        }

        protected override void OnPreRender(EventArgs e)
        {
            //index列在定义时Visible=false，在这可以设置为true
            this.clientGrid1.Columns["index"].Visible = true;
            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            Label1.Text = "columns长度：" + this.clientGrid1.Columns.Count.ToString();
            base.Render(writer);
        }

        private List<PaymentDetails1> dataSource()
        {
            List<PaymentDetails1> list = new List<PaymentDetails1>();
            for (int i = 0; i < 10; i++)
            {
                PaymentDetails1 entity = new PaymentDetails1();
                entity.index = i + 1;
                entity.PaymentItem = "垂虹园二期项目";
                entity.ExchangeRate = 0.8;
                entity.RMB = 100999.0909;
                entity.Totle = 50.00;
                entity.aHref = "www.baidu.com";
                entity.aText = "www.baidu.com";
                entity.INT = i;

                subEntity subEntity1 = new subEntity();
                subEntity1.name = "name" + i.ToString();
                subEntity1.sex = "M";
                if (i % 2 == 0)
                    subEntity1.sex = "W";

                entity.Person = subEntity1;

                if (i % 2 == 0)
                    entity.Currency = "0.91"; //港币
                else
                    entity.Currency = "1.1920007"; //人民币

                list.Add(entity);
            }
            return list;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            this.clientGrid1.Columns[4].Visible = false;
            this.clientGrid1.Columns[5].Visible = false;
        }

    }
}