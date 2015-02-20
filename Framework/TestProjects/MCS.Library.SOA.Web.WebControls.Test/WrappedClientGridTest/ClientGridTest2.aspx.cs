using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class ClientGridTest2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<Asset > list = new List<Asset >();
                Asset a = new Asset();
                a.NCGoodsName = "afdasf";
                a.GoodsMarque = "afasdf";
                a.GoodsName = "ffffffffffff";
                list.Add(a);
                Asset b = new Asset();
                b.NCGoodsName = "afdffffasf";
                b.GoodsMarque = "afafffsdf";
                b.GoodsName = "345345345";
                list.Add(b);
                this.clientGrid.InitialData = list;
            }
        }
    }

    public class Asset  {
        public string NCGoodsName { get;set;}
        public string GoodsMarque { get; set; }
        public string GoodsName { get; set; }
      
    }
}