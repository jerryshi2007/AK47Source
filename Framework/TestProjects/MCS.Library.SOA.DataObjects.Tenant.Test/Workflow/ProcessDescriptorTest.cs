using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using MCS.Library.SOA.DataObjects.Workflow.Exporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow
{
    [TestClass]
    public class ProcessDescriptorTest
    {
        [ClassInitialize()]
        public static void ProcessDescriptorInitialize(TestContext testContext)
        {
            WfProcessDescriptorInfoAdapter.Instance.ClearAll();
            WfProcessDescriptorDimensionAdapter.Instance.ClearAll();
            UserOperationLogAdapter.Instance.ClearAll();
        }

        [TestMethod]
        public void SaveAndLoadTenantProcessTest()
        {
            string processKey = UuidHelper.NewUuidString();

            IWfProcessDescriptor processDesp = CreateTestProcessDescriptor(processKey);

            string tenantCode = UuidHelper.NewUuidString();

            PrepareTenantContext(tenantCode);

            WfProcessDescriptorManager.SaveDescriptor(processDesp);

            //加载同一个租户的流程定义，如果不存在，会抛出异常
            IWfProcessDescriptor loadedSameTenant = WfProcessDescriptorManager.LoadDescriptor(processDesp.Key);

            Assert.AreEqual(processDesp.Key, loadedSameTenant.Key);

            PrepareTenantContext(UuidHelper.NewUuidString());

            try
            {
                loadedSameTenant = WfProcessDescriptorManager.LoadDescriptor(processDesp.Key);

                throw new ApplicationException("不应该加载到别的租户的流程");
            }
            catch (SystemSupportException)
            {
            }
        }

        [TestMethod]
        public void CreateProcessTemplateExecutorTest()
        {
            string processKey = UuidHelper.NewUuidString();

            IWfProcessDescriptor processDesp = CreateTestProcessDescriptor(processKey);

            WfSaveTemplateExecutor executor = new WfSaveTemplateExecutor(processDesp);

            executor.Execute();

            Assert.AreEqual(WfDesignerOperationType.CreateTemplate, executor.OperationType);
            IWfProcessDescriptor loadedProcessDesp = WfProcessDescriptorManager.LoadDescriptor(processDesp.Key);

            Assert.AreEqual(processDesp.Key, loadedProcessDesp.Key);
        }

        [TestMethod]
        public void ModifyProcessTemplateExecutorTest()
        {
            string processKey = UuidHelper.NewUuidString();

            IWfProcessDescriptor processDesp = CreateTestProcessDescriptor(processKey);

            WfSaveTemplateExecutor createExecutor = new WfSaveTemplateExecutor(processDesp);

            createExecutor.Execute();

            WfSaveTemplateExecutor modifyExecutor = new WfSaveTemplateExecutor(processDesp);

            //再保存一次，变成修改操作
            modifyExecutor.Execute();

            Assert.AreEqual(WfDesignerOperationType.ModifyTemplate, modifyExecutor.OperationType);
            IWfProcessDescriptor loadedProcessDesp = WfProcessDescriptorManager.LoadDescriptor(processDesp.Key);

            Assert.AreEqual(processDesp.Key, loadedProcessDesp.Key);
        }

        [TestMethod]
        public void DeleteProcessTemplateExecutorTest()
        {
            string processKey = UuidHelper.NewUuidString();

            IWfProcessDescriptor processDesp = CreateTestProcessDescriptor(processKey);

            WfSaveTemplateExecutor createExecutor = new WfSaveTemplateExecutor(processDesp);

            createExecutor.Execute();

            WfDeleteTemplateExecutor deleteExecutor = new WfDeleteTemplateExecutor(processDesp.Key);

            deleteExecutor.Execute();

            Assert.AreEqual(WfDesignerOperationType.DeleteTemplate, deleteExecutor.OperationType);

            try
            {
                IWfProcessDescriptor loadedProcessDesp = WfProcessDescriptorManager.LoadDescriptor(processDesp.Key);

                throw new ApplicationException("不应该加载到已经删除的流程");
            }
            catch (SystemSupportException)
            {
            }
        }

        [TestMethod]
        public void ImportProcessTemplateExecutorTest()
        {
            string processKey = UuidHelper.NewUuidString();

            IWfProcessDescriptor processDesp = CreateTestProcessDescriptor(processKey);

            WfSaveTemplateExecutor createExecutor = new WfSaveTemplateExecutor(processDesp);

            createExecutor.Execute();

            MemoryStream stream = new MemoryStream();

            WfProcessExporter.ExportProcessDescriptors(new WfExportProcessDescriptorParams(), stream, processDesp.Key);

            stream.Seek(0, SeekOrigin.Begin);

            WfImportTemplateExecutor importTemplateExecutor = new WfImportTemplateExecutor(stream);

            importTemplateExecutor.Execute();

            Assert.AreEqual(1, importTemplateExecutor.ImportedProcesses.Count);
            Assert.AreEqual(processDesp.Key, importTemplateExecutor.ImportedProcesses[0].Key);
        }

        private static TenantContext PrepareTenantContext(string tenantCode)
        {
            TenantContext.Current.Enabled = true;
            TenantContext.Current.TenantCode = tenantCode;

            return TenantContext.Current;
        }

        private static IWfProcessDescriptor CreateTestProcessDescriptor(string processKey)
        {
            WfFreeStepsProcessBuilder builder = new WfFreeStepsProcessBuilder(
                Define.DefaultApplicationName,
                Define.DefaultProgramName,
                OguObjectSettings.GetConfig().Objects["approver1"].User);

            return builder.Build(processKey, "测试流程");
        }
    }
}
