using MCS.Library.Caching;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Diagnostics.AppPoolCheck
{
    public partial class FileWatchTest : System.Web.UI.Page
    {
        private const string MessageTemplate = "消息的时间为{0:yyyy-MM-dd HH:mm:ss.fff}";

        protected void Page_Load(object sender, EventArgs e)
        {
            string fileName = GetNotifyFileName();
        }

        protected void sendNotify_Click(object sender, EventArgs e)
        {
            string message = string.Format(MessageTemplate, DateTime.Now);

            ChangeNorifyFileText(message);

            if (ObjectCacheQueue.Instance.ContainsKey("FileTest") == false)
                ObjectCacheQueue.Instance.Add("UdpTest", "尚未初始化", new FileCacheDependency(GetNotifyFileName()));

            sentMessage.InnerText = string.Format("发送内容: {0}", message);
        }

        protected void receiveNotify_Click(object sender, EventArgs e)
        {
            if (ObjectCacheQueue.Instance.ContainsKey("FileTest"))
            {
                receivedMessage.InnerText = string.Format("接收内容: {0}", ObjectCacheQueue.Instance["FileTest"].ToString());
            }
            else
            {
                string text = GetNotifyFileText();
                ObjectCacheQueue.Instance.Add("FileTest", GetNotifyFileText(), new FileCacheDependency(GetNotifyFileName()));

                receivedMessage.InnerText = string.Format("重新初始化: {0}", text);
            }
        }

        private static string GetNotifyFileName()
        {
            string configMapping = ConfigurationManager.AppSettings["MCS.MetaConfiguration"];

            configMapping = Environment.ExpandEnvironmentVariables(configMapping);

            string dirName = Path.GetDirectoryName(configMapping);

            return dirName + "\\" + "notify.user";
        }

        private static void PrepareNotifyFileAttributes()
        {
            string fileName = GetNotifyFileName();

            if (File.Exists(fileName))
            {
                FileAttributes fileAttrs = File.GetAttributes(fileName);

                if ((fileAttrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    File.SetAttributes(fileName, fileAttrs & ~FileAttributes.ReadOnly);
            }
        }

        private static void ChangeNorifyFileText(string message)
        {
            PrepareNotifyFileAttributes();

            using (FileStream stream = File.Open(GetNotifyFileName(), FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine(message);
                }
            }
        }

        private static string GetNotifyFileText()
        {
            using (StreamReader reader = File.OpenText(GetNotifyFileName()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}