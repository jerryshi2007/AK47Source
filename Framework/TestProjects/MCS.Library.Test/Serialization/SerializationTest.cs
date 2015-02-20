using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MCS.Library.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.Test
{
	[TestClass]
	public class SerializationTest
	{
		//[TestMethod]
		//public void SerializationDataTest()
		//{
		//    HashtableContainer data = new HashtableContainer();

		//    data.Dictionary["Name"] = "Shen Zheng";
		//    data.Dictionary["Unknown"] = new UnknownTypeClass() { Name = "Shen Rong" };
		//    data.SampleInt = 100;

		//    string serializedString = SerializationHelper.SerializeObjectToString(data, SerializationFormatterType.Binary);

		//    Console.WriteLine(serializedString);

		//    serializedString = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), "MCS.Library.Test.Serialization.unknownTypeSerializedData.txt");

		//    HashtableContainer deserializedData = (HashtableContainer)SerializationHelper.DeserializeStringToObject(serializedString, SerializationFormatterType.Binary);

		//    Assert.AreEqual(data.SampleInt, deserializedData.SampleInt);
		//    Assert.AreEqual(data.Dictionary["Name"], deserializedData.Dictionary["Name"]);
		//}

		[TestMethod]
		public void DeserializeKnownTypeDataTest()
		{
			string serializedGraph = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), "MCS.Library.Test.Serialization.serializedData.txt");

			HashtableContainer deserializedData = (HashtableContainer)SerializationHelper.DeserializeStringToObject(serializedGraph, SerializationFormatterType.Binary, UnknownTypeStrategyBinder.Instance);

			Assert.AreEqual(100, deserializedData.SampleInt);
			Assert.AreEqual("Shen Zheng", deserializedData.Dictionary["Name"]);

			deserializedData.Dictionary.Add("Name2", "Shen Rong");

			string reserializedGraph = SerializationHelper.SerializeObjectToString(deserializedData, SerializationFormatterType.Binary);

			Console.WriteLine(reserializedGraph);

			HashtableContainer redeserializedData = (HashtableContainer)SerializationHelper.DeserializeStringToObject(reserializedGraph, SerializationFormatterType.Binary, new MappingBinder());

			Assert.AreEqual(100, redeserializedData.SampleInt);
			Assert.AreEqual("Shen Zheng", redeserializedData.Dictionary["Name"]);
			Assert.AreEqual("Shen Rong", redeserializedData.Dictionary["Name2"]);
		}

		[TestMethod]
		public void DeserializeUnknownTypeDataTest()
		{
			string serializedGraph = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), "MCS.Library.Test.Serialization.unknownTypeSerializedData.txt");

			HashtableContainer deserializedData = (HashtableContainer)SerializationHelper.DeserializeStringToObject(serializedGraph, SerializationFormatterType.Binary, UnknownTypeStrategyBinder.Instance);

			Assert.AreEqual(100, deserializedData.SampleInt);

			string reserializedGraph = SerializationHelper.SerializeObjectToString(deserializedData, SerializationFormatterType.Binary);

			Console.WriteLine(reserializedGraph);

			HashtableContainer redeserializedData = (HashtableContainer)SerializationHelper.DeserializeStringToObject(reserializedGraph, SerializationFormatterType.Binary, new MappingBinder());

			Assert.AreEqual(100, redeserializedData.SampleInt);
			Assert.AreEqual("Shen Zheng", redeserializedData.Dictionary["Name"]);
			Assert.AreEqual("Shen Rong", ((UnknownTypeClass2)redeserializedData.Dictionary["Unknown"]).Name);
		}
	}
}
