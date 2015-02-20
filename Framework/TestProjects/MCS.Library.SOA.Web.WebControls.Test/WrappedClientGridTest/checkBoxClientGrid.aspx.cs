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
    public partial class checkBoxClientGrid : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //WebUtility.RequiredScript(typeof(DeluxeCalendar));

            int a = 123556;
            a = a / 1000 * 1000;

            double d = 123856;
            double tmp = (d / 1000);
            d = (d / 1000) * 1000;

            int ad = Convert.ToInt32(d);

            double pz = 123567;
            string z = pz.ToString("your formatString");

            int value = Convert.ToInt32(pz / 1000) * 1000;

            double cv = 123456.567;
            //int cvv0 = int.Parse(cv.ToString());
            int cvv1 = Convert.ToInt32(cv);

            string t = "1234567890";
            foreach (char item in t)
            {
                char c = item;
            }

            for (int i = 0; i < t.Length; i++)
            {
                char c = t[0];
                int k = int.Parse(c.ToString());
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.clientGrid1.InitialData = dataSource();
            }

            base.OnPreRender(e);
        }

        private List<DemoData> dataSource()
        {
            List<DemoData> list = new List<DemoData>();
            for (int i = 0; i < 1; i++)
            {
                DemoData entity = new DemoData();
                entity.Index = i + 1;

                int r = new Random().Next(i * 20);

                entity.Date = DateTime.Now.AddDays(r);
                entity.DateTime = DateTime.Now.AddDays(r);
                entity.ReadOnly = true;
                entity.t1 = new test1() { ReadOnly1 = false };
                entity.Money = 5.444000;
                list.Add(entity);
            }
            return list;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var data = this.clientGrid1.InitialData;
            this.clientGrid1.ShowCheckBoxColumn = !this.clientGrid1.ShowCheckBoxColumn;
        }
    }


}