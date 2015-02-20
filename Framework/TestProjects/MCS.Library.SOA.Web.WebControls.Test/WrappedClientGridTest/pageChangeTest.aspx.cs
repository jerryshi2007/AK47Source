using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class pageChangeTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.clientGrid1.InitialData = dataSource();
            }

            base.OnPreRender(e);
        }

        private List<TestPerson> dataSource()
        {
            Random random = new Random();

            List<TestPerson> list = new List<TestPerson>();
            for (int i = 0; i < 100; i++)
            {
                TestPerson entity = new TestPerson();
                entity.index = i + 1;
                entity.name = "name" + i.ToString();
                entity.sex = (i % 2 == 0 ? "M" : "W");
                entity.age = random.Next(15, 60);

                list.Add(entity);
            }
            return list;
        }
    }

    public class TestPerson
    {
        public int index { get; set; }
        public string name { get; set; }
        public string sex { get; set; }
        public int age { get; set; }
        public DateTime? birthday { get; set; }
    }
}