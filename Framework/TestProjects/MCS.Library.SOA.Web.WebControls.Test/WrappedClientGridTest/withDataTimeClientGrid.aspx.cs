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
    public partial class withDataTimeClientGrid : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //WebUtility.RequiredScript(typeof(DeluxeCalendar));
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.clientGrid1.InitialData = dataSource();

                DeluxeCalendar1.Value = DateTime.Now;
                //DeluxeCalendar1.Enabled = true;
            }

            base.OnPreRender(e);
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            var data = this.clientGrid1.InitialData;

            DateTime ff = DeluxeCalendar1.Value;
        }
    }

    public class DemoData
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTime { get; set; }
        public double Money { get; set; }
        public bool ReadOnly { get; set; }
        public test1 t1 { get; set; }
    }

    public class test1
    {
        public bool ReadOnly1 { get; set; }
    }
}