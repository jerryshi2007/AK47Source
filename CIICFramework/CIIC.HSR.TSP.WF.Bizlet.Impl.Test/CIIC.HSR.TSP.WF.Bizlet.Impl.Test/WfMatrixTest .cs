using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CIIC.HSR.TSP.WF.BizObject;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System.Collections.Generic;
using CIIC.HSR.TSP.DataAccess.Query;
using CIIC.HSR.TSP.DataAccess;
using MCS.Library.WF.Contracts.Workflow.Descriptors;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Test
{
    [TestClass]
    public class WfMatrixTest
    {
        [TestMethod]
        public void BuildEmptyProcess()
        {

        }

        [TestMethod]
        public void BuildTest()
        {
            string key = string.Empty;
            WfMatrixStorageManager manager = new WfMatrixStorageManager();
            manager.Context = new ServiceContext() { TenantCode = "Test1" };

            IWfMatrixProcess process = null;
            string msg = string.Empty;

            bool isok = manager.Build(key, key, out process, out msg);

            Assert.AreEqual(false, isok);
            Assert.IsFalse(string.IsNullOrEmpty(msg));

            key = Guid.NewGuid().ToString();

            isok = manager.Build(key, key, out process, out msg);

            Assert.AreEqual(true, isok);
            Assert.IsTrue(string.IsNullOrEmpty(msg));

            //检验值重复
            isok = manager.Build(key, key, out process, out msg);
            Assert.AreEqual(false, isok);

            manager.Delete(key);
        }


        [TestMethod]
        public void DeleteTest()
        {
            string key = Guid.NewGuid().ToString();
            WfMatrixStorageManager manager = new WfMatrixStorageManager();
            manager.Context = new ServiceContext() { TenantCode = "Test1" };
            IWfMatrixProcess process = WfMatrixHelper.CreateEmptyProcess(key);
            string msg = string.Empty;
            bool isok = manager.Build(process.Key, process.Name, out process, out msg);
            IWfMatrixProcess server = manager.Load(key);
            Assert.AreEqual(server.Key, key);//判定加载
            if (isok)
            {
                manager.Delete(key);

                try
                {
                    IWfMatrixProcess server2 = manager.Load(key);
                    Assert.Fail("删除失败");
                }
                catch (Exception ex)
                {
                    Assert.IsNotNull(ex);
                }
            }

        }

        [TestMethod]
        public void SaveTest()
        {
            string key = Guid.NewGuid().ToString();
            WfMatrixStorageManager manager = new WfMatrixStorageManager();
            manager.Context = new ServiceContext() { TenantCode = "Test1" };
            IWfMatrixProcess process = WfMatrixHelper.CreateEmptyProcess(key);

            manager.Save(process);
            IWfMatrixProcess server2 = manager.Load(key);

            Assert.AreEqual(key, server2.Key);

        }

        [TestMethod]
        public void SaveAgainTest()
        {
            string key = Guid.NewGuid().ToString();
            WfMatrixStorageManager manager = new WfMatrixStorageManager();
            manager.Context = new ServiceContext() { TenantCode = "Test1" };
            IWfMatrixProcess process = WfMatrixHelper.CreateEmptyProcess(key);

            manager.Save(process);
            manager.Save(process);

            IWfMatrixProcess server2 = manager.Load(key);
            Assert.AreEqual(key, server2.Key);

        }

        [TestMethod]
        public void LoadTest()
        {
            string key = Guid.NewGuid().ToString();
            WfMatrixStorageManager manager = new WfMatrixStorageManager();
            manager.Context = new ServiceContext() { TenantCode = "Test1" };
            IWfMatrixProcess process = WfMatrixHelper.CreateEmptyProcess(key);
            manager.Save(process);

            IWfMatrixProcess server2 = manager.Load(key);

            Assert.AreEqual(key, server2.Key);

        }

        [TestMethod]
        public void QueryProcessDescriptorListPagedTest()
        {
            List<string> list = new List<string>();
            string key = Guid.NewGuid().ToString();
            string pname = Guid.NewGuid().ToString();
            list.Add(key);
            WfMatrixStorageManager manager = new WfMatrixStorageManager();
            manager.Context = new ServiceContext() { TenantCode = "Test1" };
            IWfMatrixProcess process = WfMatrixHelper.CreateEmptyProcess(key);
            process.ProgramName = pname;
            manager.Save(process);

            key = Guid.NewGuid().ToString();
            list.Add(key);
            process = WfMatrixHelper.CreateEmptyProcess(key);
            process.ProgramName = pname;
            manager.Save(process);

            var qb = QueryBuilder.Create<WfClientProcessDescriptorInfo>();
            var wheres = qb.AndGroup();
            wheres.And(x => x.Path("ProcessKey").IsEqualTo(key));
            wheres.And(x => x.Path("ProcessName").Contains("测试动态流程名称"));

            qb.OrderByDesc(d => d.CreateTime);
            QueryModel queryModel = qb.Core.Product;
            PagedCollection<WfClientProcessDescriptorInfo> server2 = manager.QueryProcessDescriptorListPaged(queryModel, 0, 2, -1);

            Assert.AreEqual(1, server2.TotalItems);
            Assert.AreEqual(key, server2.Items[0].ProcessKey);


            foreach (string item in list)
            {
                manager.Delete(item);
            }
        }


        [TestMethod]
        public void QueryProcessDescriptorListPagedContainsTest()
        {
            List<string> list = new List<string>();
            string key = Guid.NewGuid().ToString();
            string pname = Guid.NewGuid().ToString();
            list.Add(key);
            WfMatrixStorageManager manager = new WfMatrixStorageManager();
            manager.Context = new ServiceContext() { TenantCode = "Test1" };
            IWfMatrixProcess process = WfMatrixHelper.CreateEmptyProcess(key);
            process.ProgramName = pname;
            manager.Save(process);

            key = Guid.NewGuid().ToString();
            list.Add(key);
            process = WfMatrixHelper.CreateEmptyProcess(key);
            process.ProgramName = pname;
            manager.Save(process);

            var qb = QueryBuilder.Create<WfClientProcessDescriptorInfo>();
            var wheres = qb.AndGroup();
            wheres.And(x => x.Path("ProcessKey").IsEqualTo(key));
            wheres.And(x => x.Path("ProcessName").Contains("动态流程"));

            qb.OrderByDesc(d => d.CreateTime);
            QueryModel queryModel = qb.Core.Product;
            PagedCollection<WfClientProcessDescriptorInfo> server2 = manager.QueryProcessDescriptorListPaged(queryModel, 0, 2, -1);

            Assert.AreEqual(1, server2.TotalItems);
            Assert.AreEqual(key, server2.Items[0].ProcessKey);


            foreach (string item in list)
            {
                manager.Delete(item);
            }
        }

        [TestMethod]
        public void QueryProcessDescriptorListPagedProgramTest()
        {
            List<string> list = new List<string>();
            string key = Guid.NewGuid().ToString();
            string pname = Guid.NewGuid().ToString();
            list.Add(key);
            WfMatrixStorageManager manager = new WfMatrixStorageManager();
            manager.Context = new ServiceContext() { TenantCode = "Test1" };
            IWfMatrixProcess process = WfMatrixHelper.CreateEmptyProcess(key);
            process.ProgramName = pname;
            manager.Save(process);

            key = Guid.NewGuid().ToString();
            list.Add(key);
            process = WfMatrixHelper.CreateEmptyProcess(key);
            process.ProgramName = pname;
            manager.Save(process);

            var qb = QueryBuilder.Create<WfClientProcessDescriptorInfo>();
            var wheres = qb.AndGroup();
            wheres.And(x => x.Path("ProgramName").IsEqualTo(pname));


            qb.OrderByDesc(d => d.CreateTime);
            QueryModel queryModel = qb.Core.Product;
            PagedCollection<WfClientProcessDescriptorInfo> server2 = manager.QueryProcessDescriptorListPaged(queryModel, 0, 2, -1);

            Assert.AreEqual(2, server2.TotalItems);
            Assert.AreEqual(key, server2.Items[0].ProcessKey);

            foreach (string item in list)
            {
                manager.Delete(item);
            }
        }
        [TestMethod]
        public void TransformTest()
        {
            string key = Guid.NewGuid().ToString();
            IWfMatrixProcess process = WfMatrixHelper.CreateComplexProcess(key);

            WfClientProcessDescriptor client = WfMatrixDescriptorTransformation.Instance.Transform(process);

            client.AreSame(process);
        }

        [TestMethod]
        public void TransformBackTest()
        {
            string key = Guid.NewGuid().ToString();
            IWfMatrixProcess process = WfMatrixHelper.CreateComplexProcess(key);

            WfClientProcessDescriptor client = WfMatrixDescriptorTransformation.Instance.Transform(process);
            IWfMatrixProcess actual = WfMatrixDescriptorTransformation.Instance.TransformBack(client);

            process.AreSame(actual);
        }
    }
}
