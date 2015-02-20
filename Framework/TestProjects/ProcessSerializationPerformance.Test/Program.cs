using System.Diagnostics;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessSerializationPerformance.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IWfProcessDescriptor processDesp = LoadProcessDescriptor();

            IWfProcess process = StartProcess(processDesp);

            const int count = 20;

            TimeSpan elapsedTime = ExecuteAction((sw) => MultiSerializeTest(process, count, sw));

            Console.WriteLine("Execute count: {0}, Elapsed time: {1:#,##0.00}, Average time: {2:#,##0.00}",
                count,
                elapsedTime.TotalMilliseconds,
                elapsedTime.TotalMilliseconds / count);

            Console.ReadLine();
        }

        private static void MultiSerializeTest(IWfProcess process, int count, Stopwatch sw)
        {
            for (int i = 0; i < count; i++)
            {
                XElement data = SerializeProcess(process);
                IWfProcess deserializedProcess = DeserializeProcess(data);

                Trace.Assert(process.Descriptor.Name == deserializedProcess.Descriptor.Name);
                Trace.Assert(process.Activities.Count == deserializedProcess.Activities.Count);

                Console.WriteLine("Count: {0}, Elapsed time: {1:#,##0.00}", i, sw.ElapsedMilliseconds);
            }
        }

        private static XElement SerializeProcess(IWfProcess process)
        {
            XElementFormatter formatter = new XElementFormatter();

            return formatter.Serialize(process);
        }

        private static IWfProcess DeserializeProcess(XElement data)
        {
            XElementFormatter formatter = new XElementFormatter();

            return (IWfProcess)formatter.Deserialize(data);
        }

        private static TimeSpan ExecuteAction(Action<Stopwatch> action)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            try
            {
                if (action != null)
                    action(sw);
            }
            finally
            {
                sw.Stop();
            }

            return sw.Elapsed;
        }

        private static IWfProcessDescriptor LoadProcessDescriptor()
        {
            using (FileStream stream = new FileStream("Expense.xml", FileMode.Open, FileAccess.Read))
            {
                return WfProcessDescriptorManager.LoadDescriptor(stream);
            }
        }

        private static IWfProcess StartProcess(IWfProcessDescriptor processDesp)
        {
            WfProcessStartupParams startupParams = GetInstanceOfWfProcessStartupParams(processDesp);

            return WfRuntime.StartWorkflow(startupParams);
        }

        private static WfProcessStartupParams GetInstanceOfWfProcessStartupParams(IWfProcessDescriptor processDesp)
        {
            WfProcessStartupParams startupParams = new WfProcessStartupParams();

            IUser requestor =
                OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, "liumh").First();

            startupParams.Creator = requestor;
            startupParams.Department = requestor.TopOU;
            startupParams.Assignees.Add(new WfAssignee(requestor));
            startupParams.ResourceID = UuidHelper.NewUuidString();
            startupParams.DefaultTaskTitle = "测试报销流程";
            startupParams.ProcessDescriptor = processDesp;
            startupParams.AutoCommit = true;

            return startupParams;
        }
    }
}
