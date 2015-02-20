using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.SOA.Web.WebControls;
namespace MCS.Library.SOA.Web.WebControls.Test.Dynamic
{
    public partial class TestDynamicOuUserInput : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            //ScriptManager scm = new ScriptManager();
            //scm.EnableScriptGlobalization = true;
            //form1.Controls.AddAt(0, scm);
            MCS.Web.WebControls.OuUserInputControl OuUserInputControl1 = new MCS.Web.WebControls.OuUserInputControl();
            OuUserInputControl1.ID = "asdsad";
            //OuUserInputControl1.GetDataSource += new MCS.Web.WebControls.AutoCompleteExtender.GetDataSourceDelegete(OuUserInputControl1_GetDataSource);
            dynamicDiv.Controls.Add(OuUserInputControl1);

            base.OnPreRender(e);
        }


        void OuUserInputControl1_GetDataSource(string sPrefix, int iCount, object eventContext, ref System.Collections.IEnumerable result)
        {
            throw new NotImplementedException();
        }
    }
}