using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl
{
    public partial class addExtraUserInfoOuUserInput : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void userInput_ObjectsLoaded(object sender, IEnumerable data)
        {
            foreach (OguBase obj in data)
            {
                if (obj.Parent != null)
                    obj.Tag = JSONSerializerExecute.Serialize(OguBase.CreateWrapperObject(obj.Parent));
            }
        }

        protected void userInput_OnGetDataSource(string sPrefix, int iCount, object eventContext, ref IEnumerable result)
        {
            foreach (OguBase obj in result)
            {
            }
        }

        protected OguDataCollection<IOguObject> OnValidateInputOuUser(string chkString)
        {
            return new OguDataCollection<IOguObject>();
        }

    }
}