using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Resources;
using MCS.Web.Responsive.Library.Script;

[assembly: WebResource("MCS.Web.Responsive.WebControls.Test.DialogControl.CustomDialogControl.js", "text/javascript")]

namespace MCS.Web.Responsive.WebControls
{

    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.CustomDialogControl", "MCS.Web.Responsive.WebControls.Test.DialogControl.CustomDialogControl.js")]
    [DialogContent("MCS.Web.Responsive.WebControls.Test.DialogControl.CustomDialogContent.htm", "MCS.Web.Responsive.WebControls.Test")]
    [ToolboxData("<{0}:CustomDialogControl runat=server></{0}:CustomDialogControl>")]
    public class CustomDialogControl : DialogControlBase
    {

    }


    ///// <summary>
    ///// ImageUploaderDialog的参数
    ///// </summary>
    //[Serializable]
    //public class CustomDialogControlParams : DialogControlParamsBase
    //{
    //    public const string DefaultDialogTitle = "上传图片";
    //}
}
