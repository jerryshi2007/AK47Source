using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Web.WebControls.Test
{
    public partial class TestWithOuUserInputEditor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var user = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, "wangli5").First();
                var dataSource = new List<OguDataContainer>();

                dataSource.Add(new OguDataContainer()
                    {
                        User = new OguUser() { ID = "1", Name = "王黎", DisplayName = "王黎" },
                        Organizations = new List<IOrganization>()
                    });

                //dataSource[0].Users.Add(new OguUser() { ID = "1", Name = "王黎",DisplayName = "王黎"});
                //dataSource[0].Organizations.Add(OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.FullPath, "机构人员").FirstOrDefault());
                gridTest.InitialData = dataSource;
            }
        }

        protected void btnPostBack_Click(object sender, EventArgs e)
        {
            var data = gridTest.InitialData;
            gridTest.ReadOnly = true;
        }

        public class OguDataContainer
        {
            public IUser User
            {
                get;
                set;
            }

            public List<IOrganization> Organizations
            {
                get;
                set;
            }
        }

    }
}