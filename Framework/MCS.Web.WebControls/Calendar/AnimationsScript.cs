using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using ChinaCustoms.Framework.DeluxeWorks.Web.Library.Script;

[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.Animations.js", "application/x-javascript")]

namespace ChinaCustoms.Framework.DeluxeWorks.Web.WebControls
{
    [ClientScriptResource(null, "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Calendar.Animations.js")]
    [RequiredScript(typeof(DeluxeAjaxScript))]
    [RequiredScript(typeof(TimerScript))]
    class AnimationsScript
    {
    }
}
