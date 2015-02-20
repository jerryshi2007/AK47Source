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
	public class PropertyValidatorParameterDescriptorSerializationTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("PropertyValidatorParameterDescriptor JSON Converter")]
		public void PropertyValidatorParameterDescriptorJSONConverterTest()
		{
			JSONSerializerExecute.RegisterConverter(typeof(PropertyValidatorParameterDescriptorConverter));

			PropertyValidatorParameterDescriptor pvpd = PreparePropertyValidatorParameterDateTimeDescriptor();

			string result = JSONSerializerExecute.Serialize(pvpd);
			Console.WriteLine(result);

			PropertyValidatorParameterDescriptor deserializedDesp = JSONSerializerExecute.Deserialize<PropertyValidatorParameterDescriptor>(result);

			string reSerialized = JSONSerializerExecute.Serialize(deserializedDesp);

			Assert.AreEqual(result, reSerialized);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		[Description("PropertyValidatorParameterDescriptor XElementSerialize")]
		public void PropertyValidatorParameterDescriptorSerializeTest()
		{
			PropertyValidatorParameterDescriptor pvpd = PreparePropertyValidatorParameterDateTimeDescriptor();

			XElementFormatter formatter = new XElementFormatter();

			XElement root = formatter.Serialize(pvpd);

			Console.WriteLine(root.ToString());

			PropertyValidatorParameterDescriptor newPropertyValue = (PropertyValidatorParameterDescriptor)formatter.Deserialize(root);

			XElement rootReserialized = formatter.Serialize(newPropertyValue);

			Assert.AreEqual(root.ToString(), rootReserialized.ToString());
			Assert.AreEqual(pvpd.ParamValue, newPropertyValue.ParamValue);
			Assert.AreEqual(pvpd.ParamName, newPropertyValue.ParamName);
			Assert.AreEqual(pvpd.DataType, newPropertyValue.DataType);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		[Description("PropertyValidatorParameterDescriptorCollection XElementSerialize")]
		public void PropertyValidatorParameterDescriptorCollectionSerializeTest()
		{
			PropertyValidatorParameterDescriptorCollection pvpdCollection = PreparePropertyValidatorParametDescriptorCollection();

			XElementFormatter formatter = new XElementFormatter();

			XElement root = formatter.Serialize(pvpdCollection);

			Console.WriteLine(root.ToString());

			PropertyValidatorParameterDescriptorCollection newCollection = (PropertyValidatorParameterDescriptorCollection)formatter.Deserialize(root);

			XElement rootReserialized = formatter.Serialize(newCollection);

			Assert.AreEqual(root.ToString(), rootReserialized.ToString());
			Assert.AreEqual(pvpdCollection.Count, newCollection.Count);
		}

		internal static PropertyValidatorParameterDescriptorCollection PreparePropertyValidatorParametDescriptorCollection()
		{
			PropertyValidatorParameterDescriptorCollection pvpdCollection = new PropertyValidatorParameterDescriptorCollection();

			pvpdCollection.Add(PreparePropertyValidatorParameterIntegerDescriptor());
			pvpdCollection.Add(PreparePropertyValidatorParameterDateTimeDescriptor());

			return pvpdCollection;
		}

		internal static PropertyValidatorParameterDescriptor PreparePropertyValidatorParameterIntegerDescriptor()
		{
			PropertyValidatorParameterDescriptor pvpd = new PropertyValidatorParameterDescriptor();
			pvpd.DataType = PropertyDataType.Integer;
			pvpd.ParamName = "testIntegerParamName";
			pvpd.ParamValue = "23";

			return pvpd;
		}

		internal static PropertyValidatorParameterDescriptor PreparePropertyValidatorParameterDateTimeDescriptor()
		{
			PropertyValidatorParameterDescriptor pvpd = new PropertyValidatorParameterDescriptor();
			pvpd.DataType = PropertyDataType.DateTime;
			pvpd.ParamName = "testDateTimeParamName";
			pvpd.ParamValue = System.DateTime.Now.ToString("yyyy-MM-dd");

			return pvpd;
		}
	}
}
