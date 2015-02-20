using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Web.Library.Script;
using MCS.Library.Core;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Test.PropertyTest
{
	[TestClass]
	public class PropertyValidatorDescriptorTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("PropertyValidator JSON Converter")]
		public void PropertyValidatorJSONConverterTest()
		{
			RegisterConverter();

			PropertyValidatorDescriptor pv = PreparePropertyValidatorDescriptor();

			string result = JSONSerializerExecute.Serialize(pv);
			Console.WriteLine(result);

			PropertyValidatorDescriptor deserializedDesp = JSONSerializerExecute.Deserialize<PropertyValidatorDescriptor>(result);

			string reSerialized = JSONSerializerExecute.Serialize(deserializedDesp);

			Assert.AreEqual(result, reSerialized);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		[Description("PropertyValidator XElementSerialize")]
		public void PropertyValidatorSerializeTest()
		{
			PropertyValidatorDescriptor pv = PreparePropertyValidatorDescriptor();

			XElementFormatter formatter = new XElementFormatter();

			XElement root = formatter.Serialize(pv);

			Console.WriteLine(root.ToString());

			PropertyValidatorDescriptor newPropertyValue = (PropertyValidatorDescriptor)formatter.Deserialize(root);

			XElement rootReserialized = formatter.Serialize(newPropertyValue);

			Assert.AreEqual(root.ToString(), rootReserialized.ToString());
			Assert.AreEqual(pv.Tag, newPropertyValue.Tag);
			Assert.AreEqual(pv.TypeDescription, newPropertyValue.TypeDescription);
			Assert.AreEqual(pv.Name, newPropertyValue.Name);
			Assert.AreEqual(pv.Parameters.Count, newPropertyValue.Parameters.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		[Description("PropertyValidatorCollection XElementSerialize")]
		public void PropertyValidatorCollectionSerializeTest()
		{
			PropertyValidatorDescriptorCollection pvDespCollection = PreparePropertyValidatorDescriptorCollection();

			XElementFormatter formatter = new XElementFormatter();

			XElement root = formatter.Serialize(pvDespCollection);

			Console.WriteLine(root.ToString());

			PropertyValidatorDescriptorCollection newpvDespCollection = (PropertyValidatorDescriptorCollection)formatter.Deserialize(root);

			XElement rootReserialized = formatter.Serialize(newpvDespCollection);

			Assert.AreEqual(root.ToString(), rootReserialized.ToString());
			Assert.AreEqual(pvDespCollection.Count, newpvDespCollection.Count);
		}

		internal static PropertyValidatorDescriptorCollection PreparePropertyValidatorDescriptorCollection()
		{
			PropertyValidatorDescriptorCollection pvDespCollection = new PropertyValidatorDescriptorCollection();

			pvDespCollection.Add(PreparePropertyValidatorDescriptor());
			pvDespCollection.Add(DefalutPreparePropertyValidatorDescriptor());

			return pvDespCollection;
		}

		private static PropertyValidatorDescriptor PreparePropertyValidatorDescriptor()
		{
			PropertyValidatorDescriptor propValidDesp = new PropertyValidatorDescriptor();

			propValidDesp.Name = "测试";
			propValidDesp.MessageTemplate = "测试MessageTemplate";
			propValidDesp.Tag = "测试属性";
			propValidDesp.Parameters.Add(PreparePropertyValidatorParameterDescriptor());
			propValidDesp.TypeDescription = "MCS.Library.Validation.StringEmptyValidator,MCS.Library";

			return propValidDesp;
		}

		private static PropertyValidatorDescriptor DefalutPreparePropertyValidatorDescriptor()
		{
			PropertyValidatorDescriptor propValidDesp = new PropertyValidatorDescriptor();

			propValidDesp.Name = "测试Name";
			propValidDesp.MessageTemplate = "MessageTemplate";
			propValidDesp.Tag = "测试Tag";
			propValidDesp.TypeDescription = "MCS.Library.Validation.StringEmptyValidator,MCS.Library";
			propValidDesp.Parameters.CopyFrom(PropertyValidatorParameterDescriptorSerializationTest.PreparePropertyValidatorParametDescriptorCollection());

			return propValidDesp;
		}

		private static PropertyValidatorParameterDescriptor PreparePropertyValidatorParameterDescriptor()
		{
			PropertyValidatorParameterDescriptor propertyValiParaDesp = new PropertyValidatorParameterDescriptor();
			propertyValiParaDesp.DataType = PropertyDataType.DateTime;
			propertyValiParaDesp.ParamValue = DateTime.Now.ToString("yyyy-MM-dd");
			propertyValiParaDesp.ParamName = "参数名称";

			return propertyValiParaDesp;
		}

		private static void RegisterConverter()
		{
			JSONSerializerExecute.RegisterConverter(typeof(PropertyValidatorDescriptorConverter));
			JSONSerializerExecute.RegisterConverter(typeof(PropertyValidatorParameterDescriptorConverter));
		}
	}
}
