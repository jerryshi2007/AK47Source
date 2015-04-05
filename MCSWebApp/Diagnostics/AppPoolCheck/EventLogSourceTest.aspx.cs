using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Diagnostics.AppPoolCheck
{
    public partial class EventLogSourceTest : System.Web.UI.Page
    {
        private static string _Source = "TestSource";
        private static string _LogName = "TestLogName";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void createEventLogSource_Click(object sender, EventArgs e)
        {
            try
            {
                EventSourceCreationData creationData = new EventSourceCreationData(_Source, _LogName);
                if (EventLog.Exists(_LogName, ".") == false)
                {
                    //source未在该服务器上注册过
                    EventLog.CreateEventSource(creationData);
                    createMessage.InnerText = "没有Log，创建Log和Source";
                }
                else
                {
                    if (EventLog.SourceExists(_Source))
                    {
                        string originalLogName = EventLog.LogNameFromSourceName(_Source, ".");

                        //source注册的日志名称和指定的logName不一致，且不等于source自身
                        //（事件日志源“System”等于日志名称，不能删除。[System.InvalidOperationException]）
                        if (string.Compare(_LogName, originalLogName, true) != 0 && string.Compare(_Source, originalLogName, true) != 0)
                        {
                            //删除现有的关联重新注册
                            EventLog.DeleteEventSource(_Source, ".");
                            EventLog.CreateEventSource(creationData);

                            createMessage.InnerText = "已经有Log，重新创建Log和Source";
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                createMessage.InnerText = ex.Message;
            }
        }

        protected void writeEventLogSource_Click(object sender, EventArgs e)
        {
            try
            {
                EventLog eventlog = new EventLog(_LogName, ".", _Source);

                string message = string.Format("测试日志写入:{0}", DateTime.Now);
                eventlog.WriteEntry(message);

                writeMessage.InnerText = message;
            }
            catch (System.Exception ex)
            {
                writeMessage.InnerText = ex.Message;
            }
        }
    }
}