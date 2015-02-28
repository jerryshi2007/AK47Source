using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Test.JSONTest
{
    [TestClass]
    public class CommonJSONTest
    {
        [TestMethod]
        [Description("一般对象的JSON序列化测试")]
        public void SimpleObjectJSONTest()
        {
            JSONTestObj data = JSONTestObj.PrepareData();

            string json = JSONSerializerExecute.Serialize(data);

            Console.WriteLine(json);

            JSONTestObj deserializedData = JSONSerializerExecute.Deserialize<JSONTestObj>(json);

            AssertObjects(data, deserializedData);
        }

        [TestMethod]
        [Description("集合对象的JSON序列化测试")]
        public void ListObjectJSONTest()
        {
            JSONTestObj data = JSONTestObj.PrepareData();

            List<JSONTestObj> source = new List<JSONTestObj>();

            source.Add(data);

            string json = JSONSerializerExecute.Serialize(source);

            Console.WriteLine(json);

            List<JSONTestObj> deserializedData = JSONSerializerExecute.Deserialize<List<JSONTestObj>>(json);

            AssertObjects(data, deserializedData[0]);
        }

        [TestMethod]
        [Description("集合对象的JSON序列化测试")]
        public void DictObjectJSONTest()
        {
            Dictionary<string, object> source = new Dictionary<string, object>();

            JSONTestObj data = JSONTestObj.PrepareData();

            source.Add("Data", data);

            string json = JSONSerializerExecute.SerializeWithType(source);

            Console.WriteLine(json);

            Dictionary<string, object> deserializedData = JSONSerializerExecute.Deserialize<Dictionary<string, object>>(json);

            AssertObjects((JSONTestObj)source["Data"], (JSONTestObj)deserializedData["Data"]);
        }

        //[TestMethod]
        //[Description("集合对象的JSON序列化测试")]
        //public void VocherObjectJSONTest()
        //{
        //    VocherEntity source = VocherEntity.PrepareData();

        //    string json = JSONSerializerExecute.SerializeWithType(source);

        //    Console.WriteLine(json);

        //    VocherEntity deserialized = JSONSerializerExecute.Deserialize<VocherEntity>(json);

        //    Assert.AreEqual(source.Items.Count, deserialized.Items.Count);
        //}

        private static void AssertObjects(JSONTestObj source, JSONTestObj dest)
        {
            Assert.AreEqual(source.ID, dest.ID);
            Assert.AreEqual(source.Age, dest.Age);
            Assert.AreEqual(source.Birthday, dest.Birthday);
        }
    }
}
