using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

[assembly: WebResource("MCS.Web.Responsive.Library.Resources.Animations.js", "application/x-javascript")]

namespace MCS.Web.Responsive.Library.Script
{
    /// <summary>
    /// ����Դ�ű�MCS.Web.Library.Script.Resources.Animations.js���������
    /// </summary>
    [ClientScriptResource(null, "MCS.Web.Responsive.Library.Resources.Animations.js")]
	[RequiredScript(typeof(ControlBaseScript))]
    public static class AnimationsScript
    {
    }
}
