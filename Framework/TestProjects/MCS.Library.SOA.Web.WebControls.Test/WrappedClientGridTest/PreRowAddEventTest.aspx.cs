using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class PreRowAddEventTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            this.clientGrid1.InitialData = dataSource();
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
    }
}