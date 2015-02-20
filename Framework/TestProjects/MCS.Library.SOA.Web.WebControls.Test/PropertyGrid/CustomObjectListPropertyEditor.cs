using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Web.WebControls;

namespace MCS.Web.WebControls
{
    public class CustomObjectListPropertyEditor : ObjectPropertyEditor
    {
        protected override void RegisterScript(System.Web.UI.Page page)
        {
            //Page.ClientScript.RegisterClientScriptInclude();//js在单独的文件中

            //page.ClientScript.RegisterStartupScript();//注册的js会写在页面下方

            //Page.ClientScript.RegisterClientScriptBlock();//注册的js在页面上方

            //string csname = "ObjectListPropertyEditorScript";
            //string csurl = "ObjectListPropertyEditor.js";
            //Type cstype = this.GetType();

        
            //if (!page.ClientScript.IsClientScriptIncludeRegistered(csname))
            //{
            //    page.ClientScript.RegisterClientScriptInclude(csname, page.ResolveClientUrl(csurl));
            //}
            base.RegisterScript(page);
        }

        protected override void OnPageLoad(System.Web.UI.Page page)
        {
            base.OnPageLoad(page);
            //string csname = "ObjectListPropertyEditorScript";
            //string csurl = "ObjectListPropertyEditor.js";

            //Type cstype = page.GetType();
            //if (!page.ClientScript.IsClientScriptIncludeRegistered(cstype,csname))
            //{
            //    page.ClientScript.RegisterClientScriptInclude(cstype,csname, page.ResolveClientUrl(csurl));
            //}
        }
    }
}