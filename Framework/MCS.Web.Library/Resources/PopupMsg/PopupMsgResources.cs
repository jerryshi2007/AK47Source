using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Text;

[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources.PopupMsg.WebMsgBox.htm", "text/html")]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources.PopupMsg.PopupMsg.js", "text/javascript", PerformSubstitution=true)]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources.PopupMsg.alert.gif", "image/gif")]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources.PopupMsg.confirm.gif", "image/gif")]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources.PopupMsg.inform.gif", "image/gif")]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources.PopupMsg.stop.gif", "image/gif")]

namespace ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources
{
    internal sealed class PopupMsgResources
    {
        public static readonly string DialogFileName = "WebMsgBox.htm";
        public static readonly string ScriptFileName = "PopupMsg.js";
        public static readonly string AlertImageFileName = "alert.gif";
        public static readonly string ConfirmImageFileName = "confirm.gif";
        public static readonly string InformImageFileName = "inform.gif";
        public static readonly string StopImageFileName = "stop.gif";

        public static string GetPopupMsgResourceUrl(ClientScriptManager clientScript, string fileName)
        {
            return clientScript.GetWebResourceUrl(typeof(PopupMsgResources), string.Format("ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources.PopupMsg.{0}", fileName));
        }
    }
}
