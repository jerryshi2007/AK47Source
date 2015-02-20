using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Text;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library.Script;

[assembly: WebResource("MCS.Web.Responsive.Library.Resources.ClientMsg.alert.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.Library.Resources.ClientMsg.confirm.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.Library.Resources.ClientMsg.inform.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.Library.Resources.ClientMsg.stop.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.Library.Resources.ClientMsg.ClientMsg.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.Responsive.Library.Resources.ClientMsg.ClientMsg.css", "text/css")]

namespace MCS.Web.Responsive.Library.Resources
{
    /// <summary>
    /// ����Դ�ű�MCS.Web.Responsive.Library.Resources.ClientMsg.ClientMsg.js"���������
    /// </summary>
    [RequiredScript(typeof(DeluxeScript))]
    [ClientScriptResource(null, "MCS.Web.Responsive.Library.Resources.ClientMsg.ClientMsg.js", Cacheability = ClientScriptCacheability.None)]
    [ClientCssResource("MCS.Web.Responsive.Library.Resources.ClientMsg.ClientMsg.css")]
    public sealed class ClientMsgResources
    {
        /// <summary>
        /// �ű��ļ���
        /// </summary>
        public static readonly string ScriptFileName = "ClientMsg.js";

        /// <summary>
        /// ��ȡ��Դ�ļ�url
        /// </summary>
        /// <param name="clientScript"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetClientMsgResourceUrl(ClientScriptManager clientScript, string fileName)
        {
            return clientScript.GetWebResourceUrl(typeof(ClientMsgResources), GetFileFullName(fileName));
        }

        /// <summary>
        /// ��ȡ��Դ�ļ�ȫ��
        /// </summary>
        /// <param name="fileName">��Դ�ļ�</param>
        /// <returns>��Դ�ļ�ȫ��</returns>
        public static string GetFileFullName(string fileName)
        {
            return string.Format("MCS.Web.Responsive.Library.Resources.ClientMsg.{0}", fileName);
        }
    }
}
