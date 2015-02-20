using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Reflection;
using MCS.Library.Core;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MCS.Library.Test.Dynamic
{
    [TestClass]
    public class DynamicMemberAccessorTest
    {
        [TestMethod]
        [TestCategory("Dynamic Invoke")]
        public void DynamicAccessorSigletonTest()
        {
            var propertyAcccessor = DynamicPropertyValueAccessor.Instance;

            Assert.AreSame(propertyAcccessor, DynamicPropertyValueAccessor.Instance);
            Assert.AreNotSame(propertyAcccessor, DynamicFieldValueAccessor.Instance);
        }

        [TestMethod]
        [TestCategory("Dynamic Invoke")]
        public void DynamicPropertyGetTest()
        {
            PropertyTestObject data = PropertyTestObject.PrepareTestData();

            var acccessor = DynamicPropertyValueAccessor.Instance;

            string id = (string)acccessor.GetValue(data, "ID");

            Assert.AreEqual(data.ID, id);
        }

        [TestMethod]
        [TestCategory("Dynamic Invoke")]
        public void DynamicPropertySetTest()
        {
            PropertyTestObject data = PropertyTestObject.PrepareTestData();

            string newID = UuidHelper.NewUuidString();

            var acccessor = DynamicPropertyValueAccessor.Instance;

            acccessor.SetValue(data, "ID", newID);

            Assert.AreEqual(newID, data.ID);
        }

        [TestMethod]
        [TestCategory("Dynamic Invoke")]
        public void DynamicPrivatePropertyGetTest()
        {
            PropertyTestObject data = PropertyTestObject.PrepareTestData();

            var acccessor = DynamicPropertyValueAccessor.Instance;
            object privateInt = acccessor.GetValue(data, "PrivateInt");

            Assert.AreEqual(data.GetPrivateInt(), privateInt);
        }

        [TestMethod]
        [TestCategory("Dynamic Invoke")]
        public void DynamicFieldGetTest()
        {
            PropertyTestObject data = PropertyTestObject.PrepareTestData();

            var acccessor = DynamicFieldValueAccessor.Instance;

            string fieldValue = (string)acccessor.GetValue(data, "PublicField");

            Assert.AreEqual(data.PublicField, fieldValue);
        }

        [TestMethod]
        [TestCategory("Dynamic Invoke")]
        public void DynamicBackFieldGetTest()
        {
            TestUser data = TestUser.PrepareTestData();

            var acccessor = DynamicFieldValueAccessor.Instance;

            FieldInfo[] fields = typeof(TestUser).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            string fieldValue = (string)acccessor.GetValue(data, "<Name>k__BackingField");

            Assert.AreEqual(data.Name, fieldValue);
        }

        [TestMethod]
        [TestCategory("Dynamic Invoke")]
        public void DynamicBackFieldSetTest()
        {
            TestUser data = TestUser.PrepareTestData();

            var acccessor = DynamicFieldValueAccessor.Instance;

            FieldInfo[] fields = typeof(TestUser).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            acccessor.SetValue(data, "<Name>k__BackingField", "Test New User");

            Assert.AreEqual(data.Name, "Test New User");
        }

        [TestMethod]
        [TestCategory("Dynamic Invoke")]
        public void DynamicPrivateFieldGetTest()
        {
            PropertyTestObject data = PropertyTestObject.PrepareTestData();

            var acccessor = DynamicFieldValueAccessor.Instance;

            string fieldValue = (string)acccessor.GetValue(data, "PrivateField");

            Assert.AreEqual(data.GetPrivateField(), fieldValue);
        }

        [TestMethod]
        [TestCategory("Dynamic Invoke")]
        public void DynamicFieldSetTest()
        {
            PropertyTestObject data = PropertyTestObject.PrepareTestData();

            string newID = UuidHelper.NewUuidString();

            var acccessor = DynamicFieldValueAccessor.Instance;

            acccessor.SetValue(data, "PublicField", newID);

            Assert.AreEqual(newID, data.PublicField);
        }

        [TestMethod]
        [TestCategory("Dynamic Invoke")]
        public void DynamicEmptyFieldsTest()
        {
            EmptyFieldsData data = new EmptyFieldsData();

            object propName = DynamicFieldValueAccessor.Instance.GetValue(data, "Name");

            Assert.IsNull(propName);

            ///不抛出异常就行
            DynamicFieldValueAccessor.Instance.SetValue(data, "Name", "Haha");
        }
    }
}
