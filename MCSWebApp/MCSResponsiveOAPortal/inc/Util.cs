using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Responsive.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Schema;

namespace MCSResponsiveOAPortal
{
    public static class Util
    {
        /// <summary>
        /// 获取流程状态图标
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetStatusIconClass(int status)
        {
            switch ((WfProcessStatus)status)
            {
                case WfProcessStatus.Aborted:
                    return "wf-status-icon wf-status-aborted";
                case WfProcessStatus.Completed:
                    return "wf-status-icon wf-status-completed";
                case WfProcessStatus.Maintaining:
                    return "wf-status-icon wf-status-maintaining";
                case WfProcessStatus.NotRunning:
                    return "wf-status-icon wf-status-notrunning";
                case WfProcessStatus.Paused:
                    return "wf-status-icon wf-status-paused";
                case WfProcessStatus.Running:
                    return "wf-status-icon wf-status-running";
                default:
                    return "wf-status-icon wf-status-unknown";
            }
        }

        public static string GetStatusIconClass(string status)
        {
            switch (status)
            {
                case "Aborted":
                    return "wf-status-icon wf-status-aborted";
                case "Completed":
                    return "wf-status-icon wf-status-completed";
                case "Maintaining":
                    return "wf-status-icon wf-status-maintaining";
                case "NotRunning":
                    return "wf-status-icon wf-status-notrunning";
                case "Paused":
                    return "wf-status-icon wf-status-paused";
                case "Running":
                    return "wf-status-icon wf-status-running";
                default:
                    return "wf-status-icon wf-status-unknown";
            }
        }

        /// <summary>
        /// 获取流程状态描述
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetStatusDescription(int status)
        {
            switch ((WfProcessStatus)status)
            {
                case WfProcessStatus.Aborted:
                    return "已终止";
                case WfProcessStatus.Completed:
                    return "已完成";
                case WfProcessStatus.Maintaining:
                    return "维护中";
                case WfProcessStatus.NotRunning:
                    return "未启动";
                case WfProcessStatus.Paused:
                    return "已暂停";
                case WfProcessStatus.Running:
                    return "运行中";
                default:
                    return "未知状态";
            }
        }

        /// <summary>
        /// 获取流程状态描述
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetStatusDescription(string status)
        {
            switch (status)
            {
                case "Aborted":
                    return "已终止";
                case "Completed":
                    return "已完成";
                case "Maintaining":
                    return "维护中";
                case "NotRunning":
                    return "未启动";
                case "Paused":
                    return "已暂停";
                case "Running":
                    return "运行中";
                default:
                    return "未知状态";
            }
        }

        public static XmlReaderSettings GetDraftingLinkXmlReaderSettings()
        {
            var readerSettings = CategoryLinkAdapter.Settings;
            if (readerSettings == null)
            {
                readerSettings = new System.Xml.XmlReaderSettings()
                {
                    IgnoreWhitespace = true,
                    IgnoreComments = true,
                };

                var schemaDoc = WebXmlDocumentCache.GetDocument("~/App_Data/CategorySchema.xsd");

                var schema = XmlSchema.Read(new StringReader(schemaDoc), null);
                CategoryLinkAdapter.Settings = readerSettings;
            }

            return readerSettings;
        }
    }
}