using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.Library.Script.Resources.Animations.js", "application/x-javascript")]

namespace MCS.Web.Library.Script
{
    /// <summary>
    /// ����Դ�ű�MCS.Web.Library.Script.Resources.Animations.js���������
    /// </summary>
    [ClientScriptResource(null, "MCS.Web.Library.Script.Resources.Animations.js")]
    [RequiredScript(typeof(ControlBaseScript))]
    public static class AnimationsScript
    {
    }
}
