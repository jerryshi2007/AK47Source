using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class withOuUserInputControl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!this.IsPostBack)
            {
                this.clientGrid1.InitialData = dataSource();

                //DataObjects.OguDataCollection<IOguObject>
                //this.OuUserInputControl_template.SelectedOuUserData = //MCS.Library.OGUPermission.OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, "44a30c90-3b7d-4cb8-8197-b753835ef059");
            }
        }

        private List<DemoDataSource> dataSource()
        {
            List<DemoDataSource> list = new List<DemoDataSource>();
            for (int i = 0; i < 1; i++)
            {
                DemoDataSource entity = new DemoDataSource();
                entity.ID = i + 1;
                //entity.User = MCS.Library.OGUPermission.OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, "44a30c90-3b7d-4cb8-8197-b753835ef059")[0];
                  
                list.Add(entity);
            }
            return list;
        }
    }

    public class DemoDataSource
    {
        public int ID { get; set; }
        public IUser User { get; set; }
    }
}