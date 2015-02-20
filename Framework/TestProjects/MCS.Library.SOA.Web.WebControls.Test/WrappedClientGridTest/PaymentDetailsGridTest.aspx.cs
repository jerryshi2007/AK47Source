using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class PaymentDetailsGridTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //WebUtility.RequiredScript(typeof(HBCommonScript));
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.clientGrid1.InitialData = dataSource();
            }

            base.OnPreRender(e);
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

        protected void postButton_Click(object sender, EventArgs e)
        {
            object data = this.clientGrid1.InitialData;
        }

    }

    public class PaymentDetails1
    {
        public int index { get; set; }
        public string PaymentItem { get; set; }
        public double RMB { get; set; }
        public string Currency { get; set; }
        public double ExchangeRate { get; set; }
        public double Totle { get; set; }
        public string aText { get; set; }
        public string aHref { get; set; }
        public int INT { get; set; }
        public subEntity Person { get; set; }
    }

    public class subEntity
    {
        public string name { get; set; }
        public string sex { get; set; }
    }
}