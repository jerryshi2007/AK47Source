using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MCS.Library.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.PropertyTest
{
	[TestClass]
	public class ObjectToPropertyTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.Properties)]
		[Description("基本的字典对象转换到属性集合")]
		public void BasicDictionaryToProperties()
		{
			Dictionary<string, object> dictionary = PrepareBasicDictionary();

			PropertyValueCollection pvs = dictionary.ToProperties();

			AssertBasicDictionaryProperties(dictionary, pvs);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Properties)]
		[Description("基本的字典对象双向转换到属性集合")]
		public void DuplexBasicDictionaryToProperties()
		{
			Dictionary<string, object> srcDictionary = PrepareBasicDictionary();

			PropertyValueCollection pvs = srcDictionary.ToProperties();

			Dictionary<string, object> clonedDictionary = new Dictionary<string, object>();

			srcDictionary.ForEach(kp => clonedDictionary.Add(kp.Key, kp.Value));

			pvs.FillDictionary(clonedDictionary);

			AssertBasicDictionaryItems(srcDictionary, clonedDictionary);
		}

		private static Dictionary<string, object> PrepareBasicDictionary()
		{
			Dictionary<string, object> result = new Dictionary<string, object>();

			result.Add("Name", "Shen Zheng");
			result.Add("Age", (DateTime.Now.Year - 1972));
			result.Add("Salary", (Decimal)10000.00);
			result.Add("Birthday", new DateTime(1972, 4, 26));
			result.Add("IsMale", true);

			return result;
		}

		private static void AssertBasicDictionaryProperties(Dictionary<string, object> dictionary, PropertyValueCollection pvs)
		{
			Assert.AreEqual(dictionary["Name"], pvs.GetValue("Name", string.Empty));
			Assert.AreEqual(dictionary["Age"], pvs.GetValue("Age", 0));
			Assert.AreEqual(dictionary["Salary"], pvs.GetValue("Salary", (Decimal)0));
			Assert.AreEqual(dictionary["Birthday"], pvs.GetValue("Birthday", DateTime.Now));
			Assert.AreEqual(dictionary["IsMale"], pvs.GetValue("IsMale", false));
		}

		private static void AssertBasicDictionaryItems(Dictionary<string, object> srcDictionary, Dictionary<string, object> destDictionary)
		{
			foreach (KeyValuePair<string, object> kp in srcDictionary)
			{
				Assert.IsTrue(destDictionary.ContainsKey(kp.Key));

				Assert.AreEqual(kp.Value, destDictionary[kp.Key]);
			}
		}
	}
}
