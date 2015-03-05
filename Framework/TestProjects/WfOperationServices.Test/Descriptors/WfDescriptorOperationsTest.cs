using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.WcfExtensions;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Json.Converters.DataObjects;
using MCS.Library.WF.Contracts.Operations;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.Builders;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace WfOperationServices.Test.Descriptors
{
    [TestClass]
    public class WfDescriptorOperationsTest
    {
        [TestMethod]
        public void SaveDescriptor()
        {
            GenericTicketTokenContainer tokenContainer = new GenericTicketTokenContainer();

            InitPrincipal("Requestor");

            WfClientProcessDescriptor processDesp = OperationHelper.PrepareSimpleProcess();

            WfClientProcessDescriptor loadedProcessDesp = WfClientProcessDescriptorServiceProxy.Instance.LoadDescriptor(processDesp.Key);

            processDesp.AssertProcessDescriptor(loadedProcessDesp);
        }

        [TestMethod]
        [ExpectedException(typeof(WfClientChannelException))]
        public void InvalidDeleteDescriptor()
        {
            WfClientProcessDescriptorServiceProxy.Instance.DeleteDescriptor(string.Empty);
        }

        [TestMethod()]
        public void LoadUserDelegationsTest()
        {
            string userID = UuidHelper.NewUuidString();
            WfClientDelegationCollection delegations = DelegationHelper.PrepareDelegations(userID);
            WfClientDelegationCollection sd = WfClientProcessDescriptorServiceProxy.Instance.LoadUserDelegations(userID);

            //传递过程，客户端扩展的属性会丢失
            delegations.ForEach(action =>
            {
                action.ApplicationName = string.Empty;
                action.ProgramName = string.Empty;
                action.TenantCode = string.Empty;
            });

            //默认排序
            sd.Sort((m, n) => m.SourceUserName.CompareTo(n.SourceUserName));

            delegations.AreSame(sd);
        }

        [TestMethod()]
        public void UpdateUserDelegationTest()
        {
            //新增或修改
            string userID = UuidHelper.NewUuidString();
            string duserID = UuidHelper.NewUuidString();
            WfClientDelegation delegation = DelegationHelper.PrepareDelegation();
            delegation.SourceUserID = userID;
            WfClientProcessDescriptorServiceProxy.Instance.UpdateUserDelegation(delegation);//新增加
            int count = WfClientProcessDescriptorServiceProxy.Instance.LoadUserDelegations(delegation.SourceUserID).Count;
            Assert.AreEqual(1, count);

            delegation.DestinationUserID = duserID;//修改被委托人信息
            WfClientProcessDescriptorServiceProxy.Instance.UpdateUserDelegation(delegation);//新增加
            count = WfClientProcessDescriptorServiceProxy.Instance.LoadUserDelegations(delegation.SourceUserID).Count;
            Assert.AreEqual(2, count);

            delegation.StartTime = System.DateTime.Now.Date;
            WfClientProcessDescriptorServiceProxy.Instance.UpdateUserDelegation(delegation);//更新
            count = WfClientProcessDescriptorServiceProxy.Instance.LoadUserDelegations(delegation.SourceUserID).Count;
            Assert.AreEqual(2, count);

            WfClientDelegationCollection list = WfClientProcessDescriptorServiceProxy.Instance.LoadUserDelegations(delegation.SourceUserID);
            WfClientDelegation delegation2 = list.Find(w => w.DestinationUserID == duserID);

            //传递过程，客户端扩展的属性会丢失
            delegation.ApplicationName = string.Empty;
            delegation.TenantCode = string.Empty;
            delegation.ProgramName = string.Empty;

            delegation.AreSame(delegation2);
        }

        [TestMethod()]
        public void DeleteUserDelegationTest()
        {
            string userID = UuidHelper.NewUuidString();
            WfClientDelegationCollection delegations = DelegationHelper.PrepareDelegations(userID);
            WfClientDelegationCollection delegations2 = DelegationHelper.PrepareDelegations(userID);

            delegations2.ForEach(action => delegations.Add(action));//同一委托人构建多条委托数据

            int count = delegations.Count;
            Assert.IsTrue(count > 2);
            WfClientDelegation delegation = delegations[0];
            WfClientProcessDescriptorServiceProxy.Instance.DeleteUserDelegation(delegation);
            int tmpCount = WfClientProcessDescriptorServiceProxy.Instance.LoadUserDelegations(userID).Count;
            Assert.AreEqual(count - 1, tmpCount);

            WfClientProcessDescriptorServiceProxy.Instance.DeleteUserDelegation(userID);
            tmpCount = WfClientProcessDescriptorServiceProxy.Instance.LoadUserDelegations(userID).Count;
            Assert.AreEqual(0, tmpCount);
        }

        [TestMethod()]
        public void ExcelToWfCreateClientDynamicProcessParamsTest()
        {
            const string processKey = "c8d2a844-0000-bb51-4ab0-1563a40d2b58";
            WfCreateClientDynamicProcessParams processParams = ProcessDescriptorHelper.CreateClientDynamicProcessParams();
            processParams.Key = processKey;

            WfCreateClientDynamicProcessParams result = null;
            WfClientDynamicProcessBuilder builder = new WfClientDynamicProcessBuilder(processParams);
            WfClientProcessDescriptor client = builder.Build(processParams.Key, processParams.Name);
            WfClientServiceBrokerContext.Current.Context[Consts.TenantCode] = "Test1";// GetCurrentTenantCode();

            WfClientProcessDescriptorServiceProxy.Instance.SaveDescriptor(client);//数据准备

            using (Stream stream = WfClientProcessDescriptorServiceProxy.Instance.WfDynamicProcessToExcel(processKey)) //EXCEL文件流
            {
                result = WfClientProcessDescriptorServiceProxy.Instance.ExcelToWfCreateClientDynamicProcessParams(processKey, stream);
            }

            processParams.ActivityMatrix.PropertyDefinitions.ForEach(action =>
            {
                action.DataType = ColumnDataType.String;
            }); //EXCEL 无法存储类型信息，所有默认都为string类型

            processParams.AreSame(result);
        }

        [TestMethod()]
        public void ExcelToSaveDescriptorTest()
        {
            const string processKey = "c8d2a844-3a3a-bb51-4ab0-1563a40d2b58";
            WfCreateClientDynamicProcessParams processParams = ProcessDescriptorHelper.CreateClientDynamicProcessParams();
            processParams.Key = processKey;

            WfClientDynamicProcessBuilder builder = new WfClientDynamicProcessBuilder(processParams);
            WfClientProcessDescriptor client = builder.Build(processParams.Key, processParams.Name);
            WfClientProcessDescriptorServiceProxy.Instance.SaveDescriptor(client);//数据准备

            WfClientProcessDescriptor server = null;

            using (Stream stream = WfClientProcessDescriptorServiceProxy.Instance.WfDynamicProcessToExcel(processKey)) //EXCEL文件流
            {
                WfClientProcessDescriptorServiceProxy.Instance.DeleteDescriptor(processKey);//清理
                //校验清理结果
                try
                {
                    server = WfClientProcessDescriptorServiceProxy.Instance.GetDescriptor(processKey);
                }
                catch (MCS.Library.WcfExtensions.WfClientChannelException ex)
                {
                    Assert.IsTrue(true, ex.Message);
                }
                WfClientServiceBrokerContext.Current.Context[Consts.TenantCode] = "Test1";// GetCurrentTenantCode();


                WfClientProcessDescriptorServiceProxy.Instance.ExcelToSaveDescriptor(processKey, stream);
                server = WfClientProcessDescriptorServiceProxy.Instance.GetDescriptor(processKey);
                Assert.AreEqual(processParams.ProgramName, server.ProgramName);
                // Assert.AreEqual("Test1", server.Properties));
            }
        }

        [TestMethod()]
        public void ExcelKeyChangeSaveDescriptorTest()
        {
            const string processKey = "c8d2a844-0002-bb51-4ab0-1563a40d2b587";

            WfCreateClientDynamicProcessParams processParams = ProcessDescriptorHelper.CreateClientDynamicProcessParams();
            processParams.Key = processKey;

            WfClientDynamicProcessBuilder builder = new WfClientDynamicProcessBuilder(processParams);
            WfClientProcessDescriptor client = builder.Build(processParams.Key, processParams.Name);

            WfClientServiceBrokerContext.Current.Context[Consts.TenantCode] = "Test1";// GetCurrentTenantCode();

            WfClientProcessDescriptorServiceProxy.Instance.SaveDescriptor(client);//数据准备

            #region 保存EXCEL后，修改EXCEL数据后再校验
            //using (FileStream fs = File.OpenWrite(string.Format("E:\\work\\TMP\\{0}.xlsx", processKey)))
            //{
            //    WfClientProcessDescriptorServiceProxy.Instance.WfDynamicProcessToExcel(processKey).CopyTo(fs);
            //} 

            //using (FileStream fs = File.OpenRead(string.Format("E:\\work\\TMP\\{0}.xlsx", processKey)))
            //{
            //    WfClientProcessDescriptorServiceProxy.Instance.ExcelToSaveDescriptor(processKey, fs);
            //}
            #endregion

            client = WfClientProcessDescriptorServiceProxy.Instance.GetDescriptor(processKey);

            Assert.AreEqual(client.Properties.GetValue("Key", string.Empty), processKey);
            Assert.AreEqual(client.Key, processKey);
        }

        [TestMethod()]
        public void WfDynamicProcessToExcelTest()
        {
            string processKey = "c8d2a844-0003-bb51-4ab0-1563a40d2b58";
            WfCreateClientDynamicProcessParams processParams = ProcessDescriptorHelper.CreateClientDynamicProcessParams();
            processParams.Key = processKey;
            WfClientDynamicProcessBuilder builder = new WfClientDynamicProcessBuilder(processParams);
            WfClientProcessDescriptor client = builder.Build(processParams.Key, processParams.Name);

            WfClientServiceBrokerContext.Current.Context[Consts.TenantCode] = "Test1";// GetCurrentTenantCode();

            WfClientProcessDescriptorServiceProxy.Instance.SaveDescriptor(client);

            System.Data.DataTable processTable = null;
            System.Data.DataTable matrixTable = null;

            using (Stream stream = WfClientProcessDescriptorServiceProxy.Instance.WfDynamicProcessToExcel(processKey))
            {
                processTable = DocumentHelper.GetRangeValuesAsTable(stream, "Process", "A3");
                matrixTable = DocumentHelper.GetRangeValuesAsTable(stream, "Matrix", "A3");
            }
            Assert.IsTrue(processTable.Rows.Count > 0);
            Assert.IsTrue(matrixTable.Rows.Count == 2);
            Assert.IsTrue(matrixTable.Rows[0]["CostCenter"].ToString() == "1001");
            Assert.IsTrue(matrixTable.Rows[1]["Age"].ToString() == "40");

        }

        [TestMethod()]
        public void QueryProcessDescriptorInfoTest()
        {
            //建立流程
            int totalIndex = -1;
            int pageSize = 1;
            Dictionary<string, WfClientProcessDescriptor> dic = new Dictionary<string, WfClientProcessDescriptor>();
            WfClientProcessDescriptor descriptor = OperationHelper.PrepareSimpleProcess();
            string key = descriptor.Key;
            dic.Add(key, descriptor);



            WfClientProcessDescriptor descriptor1 = OperationHelper.PrepareSimpleProcess();
            string key1 = descriptor1.Key;
            dic.Add(key1, descriptor1);
            string orderby = "CREATE_TIME";

            string where = string.Format(" {0}='{1}' or {0}='{2}' ", "PROCESS_KEY", key, key1);

            //获取子流程
            WfClientProcessDescriptorInfoPageQueryResult result = WfClientProcessDescriptorServiceProxy.Instance.QueryProcessDescriptorInfo(
                 0, pageSize, where, orderby, totalIndex);
            totalIndex = result.TotalCount;

            //检查
            Assert.AreEqual(totalIndex, dic.Count);

            //分页是否正确,分支流程个数是否正确
            Assert.AreEqual(pageSize, result.QueryResult.Count);

            //检查排序
            Assert.AreEqual(descriptor.Key, result.QueryResult[0].ProcessKey);

            //分页数据
            result = WfClientProcessDescriptorServiceProxy.Instance.QueryProcessDescriptorInfo(
               1, pageSize, where, orderby, totalIndex);
            Assert.AreEqual(descriptor1.Key, result.QueryResult[0].ProcessKey);//第一条加入的信息在第二页

            //清理数据
            foreach (var item in dic)
            {
                OperationHelper.ClearProcessDescriptorSqlServerData(item.Key);
            }
        }

        [TestMethod()]
        public void ExsitsProcessKeyTest()
        {
            WfClientProcessDescriptor descriptor = OperationHelper.PrepareSimpleProcess();
            string key = descriptor.Key;
            string key2 = Guid.NewGuid().ToString();
            bool istrue = WfClientProcessDescriptorServiceProxy.Instance.ExsitsProcessKey(key);
            Assert.AreEqual(true, istrue);

            istrue = WfClientProcessDescriptorServiceProxy.Instance.ExsitsProcessKey(key2);
            Assert.AreEqual(false, istrue);

            OperationHelper.ClearProcessDescriptorSqlServerData(key);
        }

        [TestMethod()]
        public void ExportProcessDescriptorsTest()
        {
            WfClientProcessDescriptor processDesp = OperationHelper.PrepareSimpleProcess();

            Stream stream = WfClientProcessDescriptorServiceProxy.Instance.ExportProcessDescriptors(processDesp.Key);

            Assert.IsTrue(stream.Length > 0);
        }

        [TestMethod()]
        public void ImportProcessDescriptorTest()
        {
            WfClientProcessDescriptor processDesp = OperationHelper.PrepareSimpleProcess();

            Stream stream = WfClientProcessDescriptorServiceProxy.Instance.ExportProcessDescriptors(processDesp.Key);

            stream.Seek(0, SeekOrigin.Begin);

            string info = WfClientProcessDescriptorServiceProxy.Instance.ImportProcessDescriptors(stream);

            Console.WriteLine(info);

            Assert.IsTrue(info.IndexOf("1个流程模板文件") >= 0);
        }

        public static void InitPrincipal(string userKey)
        {
            GenericTicketTokenContainer tokenContainer = new GenericTicketTokenContainer();

            tokenContainer.User = new GenericTicketToken(Consts.Users[userKey]);
            tokenContainer.RealUser = new GenericTicketToken(Consts.Users[userKey]);

            DeluxeIdentity identity = new DeluxeIdentity(tokenContainer, null);

            DeluxePrincipal principal = new DeluxePrincipal(identity);

            PrincipaContextAccessor.SetPrincipalInContext(WfClientServiceBrokerContext.Current, principal);
        }
    }
}
