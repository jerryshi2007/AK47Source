using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 访问流程定义管理器
    /// </summary>
    public static class WfProcessDescriptorManager
    {
        private static Dictionary<string, WfProcessBuilderInfo> DefaultProcessBuilderInfo = new Dictionary<string, WfProcessBuilderInfo>()
		{
			{ DefaultConsignProcessKey, new WfProcessBuilderInfo(new WfConsignProcessBuilder(string.Empty, string.Empty), "默认会签流程")},
			{ DefaultCirculationProcessKey, new WfProcessBuilderInfo(new WfCirculationProcessBuilder(string.Empty, string.Empty), "默认传阅流程")},
			{ DefaultApprovalProcessKey, new WfProcessBuilderInfo(new WfApprovalProcessBuilder(string.Empty, string.Empty), "默认审批流程")}
		};

        public const string DefaultConsignProcessKey = "DefaultConsignProcess";
        public const string DefaultCirculationProcessKey = "DefaultCirculationProcess";
        public const string DefaultApprovalProcessKey = "DefaultApprovalProcess";

        public static IWfProcessDescriptor LoadDescriptor(string processKey)
        {
            IWfProcessDescriptor processDesp = GetDefaultProcessDescriptor(processKey);

            processDesp = processDesp ??
                WorkflowSettings.GetConfig().GetDescriptorManager().LoadDescriptor(processKey);

            return processDesp;
        }

        public static IWfProcessDescriptor LoadDescriptor(Stream stream)
        {
            stream.NullCheck("stream");

            XElement root = XElement.Load(stream);

            return DeserializeXElementToProcessDescriptor(root);
        }

        public static IWfProcessDescriptor GetDescriptor(string processKey)
        {
            IWfProcessDescriptor processDesp = WfRuntime.ProcessContext.FireGetProcessDescriptor(processKey);

            processDesp = processDesp ?? GetDefaultProcessDescriptor(processKey);

            processDesp = processDesp ??
                WorkflowSettings.GetConfig().GetDescriptorManager().GetDescriptor(processKey);

            return processDesp;
        }

        public static void SaveDescriptor(IWfProcessDescriptor processDesp)
        {
            WorkflowSettings.GetConfig().GetDescriptorManager().SaveDescriptor(processDesp);
        }

        public static bool ExsitsProcessKey(string processKey)
        {
            return WorkflowSettings.GetConfig().GetDescriptorManager().ExsitsProcessKey(processKey);
        }

        public static void DeleteDescriptor(string processKey)
        {
            WorkflowSettings.GetConfig().GetDescriptorManager().DeleteDescriptor(processKey);
        }

        public static void ClearAll()
        {
            WorkflowSettings.GetConfig().GetDescriptorManager().ClearAll();
        }

        public static WfProcessBuilderInfo GetBuilderInfo(string processKey)
        {
            processKey.CheckStringIsNullOrEmpty("processKey");

            WfProcessBuilderInfo builderInfo = null;

            DefaultProcessBuilderInfo.TryGetValue(processKey, out builderInfo);

            return builderInfo;
        }

        internal static IWfProcessDescriptor DeserializeXElementToProcessDescriptor(XElement root)
        {
            XElementFormatter formatter = CreateFormatter();

            WfProcessDescriptor procDesp = (WfProcessDescriptor)formatter.Deserialize(root);

            procDesp.MergeDefinedProperties();

            return procDesp;
        }

        internal static XElementFormatter CreateFormatter()
        {
            XElementFormatter formatter = new XElementFormatter();

            formatter.OutputShortType = WorkflowSettings.GetConfig().OutputShortType;

            return formatter;
        }

        private static IWfProcessDescriptor GetDefaultProcessDescriptor(string processKey)
        {
            string appName = "DefaultApplication";
            string progName = "DefaultProgram";

            WfProcessBuilderInfo builderInfo = GetBuilderInfo(processKey);

            IWfProcessDescriptor processDesp = null;

            if (builderInfo != null)
            {
                if (WfRuntime.ProcessContext.OriginalActivity != null)
                {
                    appName = WfRuntime.ProcessContext.OriginalActivity.Process.Descriptor.ApplicationName;
                    progName = WfRuntime.ProcessContext.OriginalActivity.Process.Descriptor.ProgramName;
                }

                WfProcessBuilderBase builder = (WfProcessBuilderBase)TypeCreator.CreateInstance(builderInfo.Builder.GetType(), appName, progName);

                processDesp = builder.Build(processKey, builderInfo.ProcessName);
            }

            return processDesp;
        }
    }
}
