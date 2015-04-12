using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MCS.Library.Configuration;
using System.Web.Configuration;
using System.Text;

namespace MCS.Web.Library.Script.Json.Test
{
    [TestClass]
    public class SimpleJsonTest
    {
        [TestMethod]
        [Description("一般对象的JSON序列化测试")]
        public void SimpleObjectJsonTest()
        {
            JsonTestObj data = JsonTestObj.PrepareData();

            string json = JSONSerializerExecute.Serialize(data);

            Console.WriteLine(json);

            JsonTestObj deserializedData = JSONSerializerExecute.Deserialize<JsonTestObj>(json);

            AssertObjects(data, deserializedData);
        }

        [TestMethod]
        public void Int32To64JsonTest()
        {
            Int64 ticks = 32L;

            string json = JSONSerializerExecute.Serialize(ticks);

            Console.WriteLine(json);

            Int64 deserialized = (Int64)JSONSerializerExecute.DeserializeObject(ticks.ToString(), typeof(Int64));

            Assert.AreEqual(ticks, deserialized);
        }

        [TestMethod]
        [Description("集合对象的JSON序列化测试")]
        public void ListObjectJsonTest()
        {
            JsonTestObj data = JsonTestObj.PrepareData();

            List<JsonTestObj> source = new List<JsonTestObj>();

            source.Add(data);

            string json = JSONSerializerExecute.Serialize(source);

            Console.WriteLine(json);

            List<JsonTestObj> deserializedData = JSONSerializerExecute.Deserialize<List<JsonTestObj>>(json);

            AssertObjects(data, deserializedData[0]);
        }

        [TestMethod]
        [Description("集合对象的JSON序列化测试")]
        public void DictObjectJsonTest()
        {
            Dictionary<string, object> source = new Dictionary<string, object>();

            JsonTestObj data = JsonTestObj.PrepareData();

            source.Add("Data", data);

            string json = JSONSerializerExecute.SerializeWithType(source);

            Console.WriteLine(json);

            Dictionary<string, object> deserializedData = JSONSerializerExecute.Deserialize<Dictionary<string, object>>(json);

            AssertObjects((JsonTestObj)source["Data"], (JsonTestObj)deserializedData["Data"]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExceedMaxJsonLengthSerializationLengthTest()
        {
            ScriptingJsonSerializationSection scriptSection = (ScriptingJsonSerializationSection)ConfigurationBroker.GetSection("scriptJsonSerialization");

            Console.WriteLine(scriptSection.MaxJsonLength);
            string data = PrepateTestStringByLength(1500);

            string json = JSONSerializerExecute.Serialize(data);
        }

        private static string PrepateTestStringByLength(int length)
        {
            StringBuilder strB = new StringBuilder();

            for (int i = 0; i < length; i++)
                strB.Append('A');

            return strB.ToString();
        }
        private static void AssertObjects(JsonTestObj source, JsonTestObj dest)
        {
            Assert.AreEqual(source.ID, dest.ID);
            Assert.AreEqual(source.Age, dest.Age);
            Assert.AreEqual(source.Birthday, dest.Birthday);
        }
    }
}
