using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class headerManageTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.clientGrid1.InitialData = dataSource();
        }


        private List<DemoData> dataSource()
        {
            List<DemoData> list = new List<DemoData>();
            for (int i = 0; i < 10; i++)
            {
                DemoData entity = new DemoData();
                entity.Index = i + 1;

                int r = new Random().Next(i * 20);

                entity.Date = DateTime.Now.AddDays(r);
                entity.DateTime = DateTime.Now.AddDays(r);
                entity.Money = 5.444000;
                list.Add(entity);
            }
            return list;
        }
    }
}