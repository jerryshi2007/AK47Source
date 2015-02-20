using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;

namespace MCS.OA.CommonPages.ModalDialog
{
    public partial class UserSignatureSetting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                var userID = WebUtility.GetRequestQueryString("userID", "");
                if (userID != "")
                {
                    UserSettings settings = UserSettings.LoadSettings(userID);
                    if(settings!=null)
                    {
                        var data = settings.GetPropertyValue("CommonSettings", "Signature", "");
                        editor.InitialData = HttpUtility.UrlDecode(data);
                    }
                }
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            var docProp = editor.DocumentProperty;
            if (docProp != null)
            {
                foreach (var imgProp in docProp.DocumentImages)
                {
                    if (imgProp.Changed)
                    {
                        ImagePropertyAdapter.Instance.UpdateWithContent(imgProp);
                    }
                }
            }

            ClientScript.RegisterStartupScript(this.GetType(), "confirm", "onConfirm();", true);
        }
    }
}