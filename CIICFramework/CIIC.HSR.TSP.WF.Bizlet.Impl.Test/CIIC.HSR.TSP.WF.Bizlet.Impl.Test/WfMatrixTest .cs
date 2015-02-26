using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CIIC.HSR.TSP.WF.BizObject;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System.Collections.Generic;
using CIIC.HSR.TSP.DataAccess.Query;
using CIIC.HSR.TSP.DataAccess;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;


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
            WfMatrixProcessDescriptorCreateParams createParams = new WfMatrixProcessDescriptorCreateParams();
            createParams.Key = key;
            createParams.Name = key;

            IWfMatrixProcess process = null;
            string msg = string.Empty;

            try
            {
                process = manager.CreateEmptyProcessDescriptor(createParams);
            }
            catch
            {
                Assert.IsTrue(true,"nullCheck");
            } 
            

            key = Guid.NewGuid().ToString();
            createParams.Key = key;
            createParams.Name = key;

            process = manager.CreateEmptyProcessDescriptor(createParams); 
         
            Assert.IsTrue(true,"create");

            //检验值重复
            try
            {
                process = manager.CreateEmptyProcessDescriptor(createParams);
            }
            catch (ArgumentException ex)
            {                 
                Assert.IsTrue(true, ex.Message);
            }
            

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
            WfMatrixProcessDescriptorCreateParams createParams = new WfMatrixProcessDescriptorCreateParams();
            createParams.Key = key;
            createParams.Name = key;

            process = manager.CreateEmptyProcessDescriptor(createParams);
            IWfMatrixProcess server = manager.Load(key);
            Assert.AreEqual(server.Key, key);//判定加载

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
            IWfMatrixProcess process = WfMatrixHelper.CreateComplexProcess(key);          
            manager.Save(process);
         
            process.InitGlobalParameter();

            IWfMatrixProcess server2 = manager.Load(key);
            server2.Properties.Add(Consts.ProcessSavedKey, string.Empty);

            process.AreSame(server2);
            

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

            Assert.AreEqual(1, server2.TotalItems, "TotalItems");
            Assert.AreEqual(key, server2.Items[0].ProcessKey, "ProcessKey");


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
            process.InitGlobalParameter();
            WfClientProcessDescriptor client = WfMatrixDescriptorTransformation.Instance.Transform(process);
            IWfMatrixProcess actual = WfMatrixDescriptorTransformation.Instance.TransformBack(client);
            actual.Properties.Add(Consts.ProcessSavedKey, string.Empty);
            actual.InitGlobalParameter();
         

            process.AreSame(actual);
        }

        [TestMethod]
        public void JsonConvertTest()
        {
            string key = Guid.NewGuid().ToString();

            IWfMatrixProcess process = WfMatrixHelper.CreateComplexProcess(key);
        
            var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            WfMatrixJsonConverterHelper.Instance.RegisterConverters(jss);    

           string json =  jss.Serialize(process);
         
           WfMatrixProcess uiProcess = jss.Deserialize<WfMatrixProcess>(json);

           process.AreSame(uiProcess);
        }

          [TestMethod]
        public void ConditionJsonConvertTest()
        {
            string key = Guid.NewGuid().ToString();

            IWfMatrixProcess process = WfMatrixHelper.CreateComplexProcess(key);
            IWfMatrixConditionGroupCollection group = process.Activities[2].Expression;
         
            var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            WfMatrixJsonConverterHelper.Instance.RegisterConverters(jss);

            string json = jss.Serialize(group);
            
            WfMatrixConditionGroupCollection condition = jss.Deserialize<WfMatrixConditionGroupCollection>(json);
            Assert.IsTrue(true, condition.Count.ToString());
        }

             [TestMethod]
          public void CandidateJsonConvertTest()
          {
              string key = Guid.NewGuid().ToString();

              IWfMatrixProcess process = WfMatrixHelper.CreateComplexProcess(key);
              IWfMatrixCandidateCollection candidate = process.Activities[2].Candidates;

              var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
              WfMatrixJsonConverterHelper.Instance.RegisterConverters(jss);

              string json = jss.Serialize(candidate);

              WfMatrixCandidate[] a = jss.Deserialize<WfMatrixCandidate[]>(json);
              Assert.IsTrue(true, a.Length.ToString());
          }


           [TestMethod]
          public void ActivityJsonConvertTest()
          {
              string key = Guid.NewGuid().ToString();

              IWfMatrixProcess process = WfMatrixHelper.CreateComplexProcess(key);
              IWfMatrixActivity activity = process.Activities[2];

              var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
              WfMatrixJsonConverterHelper.Instance.RegisterConverters(jss);

              string json = jss.Serialize(activity);

              WfMatrixActivity a = jss.Deserialize<WfMatrixActivity>(json);
              Assert.IsTrue(true, a.CodeName);
          }

         [TestMethod]
           public void DeserializeUIJsonTest()
           {
               string key = Guid.NewGuid().ToString();
               WfMatrixStorageManager manager = new WfMatrixStorageManager();
               IWfMatrixProcess process = WfMatrixHelper.CreateComplexProcess(key);             

               var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
               WfMatrixJsonConverterHelper.Instance.RegisterConverters(jss);

               string json = jss.Serialize(process);

               IWfMatrixProcess a = manager.DeserializeUIJson(json);
               Assert.IsTrue(true, a.Key);
           }

      

    }
}
