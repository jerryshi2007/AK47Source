using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using System.Xml.Linq;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Test.PropertyTest
{
	[TestClass]
	public class PropertySerializationTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		public void PropertyValueSerializeTest()
		{
			PropertyValue pv = PreparePropertyValue();

			XElementFormatter formatter = new XElementFormatter();
			XmlSerializeContext context = new XmlSerializeContext();
			XElement root = new XElement("root");
			pv.Serialize(root, context);

			Console.WriteLine(root.ToString());

			XmlDeserializeContext dcontext = new XmlDeserializeContext();
			PropertyValue newPropertyValue = new PropertyValue(new PropertyDefine());
			newPropertyValue.Deserialize(root, dcontext);

			//Assert.AreEqual(root.ToString(), rootReserialized.ToString());
			Assert.AreEqual(pv.StringValue, newPropertyValue.StringValue);
			Assert.AreEqual(pv.Definition.Name, newPropertyValue.Definition.Name);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		public void DateTimePropertyValueSerializeTest()
		{
			PropertyValue pv = PrepareDateTimePropertyValue();

			XElementFormatter formatter = new XElementFormatter();

			XElement root = formatter.Serialize(pv);

			Console.WriteLine(root.ToString());

			PropertyValue newPropertyValue = (PropertyValue)formatter.Deserialize(root);

			XElement rootReserialized = formatter.Serialize(newPropertyValue);

			Assert.AreEqual(root.ToString(), rootReserialized.ToString());
			Assert.AreEqual(pv.StringValue, newPropertyValue.StringValue);
			Assert.AreEqual(pv.Definition.Name, newPropertyValue.Definition.Name);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		public void PropertyValueCollectionSerializeTest()
		{
			PropertyValueCollection pvc = new PropertyValueCollection();

			pvc.Add(PreparePropertyValue());

			XElementFormatter formatter = new XElementFormatter();

			XElement root = formatter.Serialize(pvc);

			Console.WriteLine(root.ToString());

			PropertyValueCollection newPvc = (PropertyValueCollection)formatter.Deserialize(root);

			Assert.AreEqual(pvc.Count, newPvc.Count);
			Assert.AreEqual(pvc[0].StringValue, newPvc[pvc[0].Definition.Name].StringValue);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		public void PropertyValueJSONSerializerTest()
		{
			RegisterConverter();

			PropertyValue pv = PrepareDateTimePropertyValue();

			string result = JSONSerializerExecute.Serialize(pv);
			Console.WriteLine(result);

			PropertyValue deserializedDesp = JSONSerializerExecute.Deserialize<PropertyValue>(result);

			string reSerialized = JSONSerializerExecute.Serialize(deserializedDesp);

			Assert.AreEqual(result, reSerialized);
		}

		private static void RegisterConverter()
		{
			JSONSerializerExecute.RegisterConverter(typeof(PropertyValueConverter));
			JSONSerializerExecute.RegisterConverter(typeof(PropertyValidatorDescriptorConverter));
			JSONSerializerExecute.RegisterConverter(typeof(PropertyValidatorParameterDescriptorConverter));
		}

		private static PropertyValue PreparePropertyValue()
		{
			PropertyDefine define = new PropertyDefine();

			define.Category = "测试";
			define.DataType = PropertyDataType.String;
			define.Name = "测试属性";
			define.Description = "测试属性的描述";
			define.Visible = false;

			PropertyValue pv = new PropertyValue(define);
			pv.StringValue = "你们认识张玲吗？";

			return pv;
		}

		private static PropertyValue PrepareDateTimePropertyValue()
		{
			PropertyDefine define = new PropertyDefine();

			define.Category = "测试";
			define.DataType = PropertyDataType.DateTime;
			define.Name = "测试属性";
			define.Description = "测试属性的描述";
			define.Visible = false;

			define.Validators.CopyFrom(PropertyValidatorDescriptorTest.PreparePropertyValidatorDescriptorCollection());

			PropertyValue pv = new PropertyValue(define);

			pv.StringValue = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss");

			return pv;
		}
	}
}
