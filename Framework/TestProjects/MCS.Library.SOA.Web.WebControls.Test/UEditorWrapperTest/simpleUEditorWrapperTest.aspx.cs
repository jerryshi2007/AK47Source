using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.Web.WebControls.Test.UEditorWrapperTest
{
    public partial class simpleUEditorWrapperTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void PostButton_Click(object sender, EventArgs e)
        {
            //content.InnerHtml = editor.DocumentProperty.InitialData;
            divContent.InnerHtml = editor.DocumentProperty.InitialData;

            var images = editor.DocumentProperty.DocumentImages;
            //TextArea1.Value = JSONSerializerExecute.Serialize(UEditorWrapper1.DocumentProperty);
        }
    }
}