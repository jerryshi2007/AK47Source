using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.Principal;
using MCS.Web.WebControls;

namespace MCS.Web.WebControls
{
    public sealed class SignaturePropertyEditor : PropertyEditorBase
    {
        private string UserIDInputName()
        {
            return "SignaturePropertyEditor_UserIDHiddenInput";
        }

        protected override void OnPageLoad(Page page)
        {
            if (page.Form != null)
            {
                HtmlGenericControl div = new HtmlGenericControl();
                div.Style["display"] = "none";
                HtmlInputHidden hidden = new HtmlInputHidden() {ID = UserIDInputName()};
                
                if (DeluxePrincipal.IsAuthenticated)
                {
                    hidden.Value = DeluxeIdentity.CurrentUser.ID;
                }

                div.Controls.Add(hidden);
                page.Form.Controls.AddAt(0, div);
            }
        }
    }
}