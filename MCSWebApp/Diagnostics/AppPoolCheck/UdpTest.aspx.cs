using MCS.Library.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Diagnostics.AppPoolCheck
{
    public partial class UdpTest : System.Web.UI.Page
    {
        private const string MessageTemplate = "消息的时间为{0:yyyy-MM-dd HH:mm:ss.fff}";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void sendNotify_Click(object sender, EventArgs e)
        {
            if (ObjectCacheQueue.Instance.ContainsKey("UdpTest") == false)
                ObjectCacheQueue.Instance.Add("UdpTest", "尚未初始化", new UdpNotifierCacheDependency());

            string message = string.Format(MessageTemplate, DateTime.Now);

            CacheNotifyData notify = new CacheNotifyData(ObjectCacheQueue.Instance.GetType(), "UdpTest", CacheNotifyType.Update);

            notify.CacheData = message;

            UdpCacheNotifier.Instance.SendNotify(notify);

            sentMessage.InnerText = string.Format("发送内容: {0}", message);
        }

        protected void receiveNotify_Click(object sender, EventArgs e)
        {
            if (ObjectCacheQueue.Instance.ContainsKey("UdpTest"))
                receivedMessage.InnerText = string.Format("接收内容: {0}", ObjectCacheQueue.Instance["UdpTest"].ToString());
        }
    }
}