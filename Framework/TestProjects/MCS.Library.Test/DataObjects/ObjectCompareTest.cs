using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MCS.Library.Test.DataObjects
{
    [TestClass]
    public class ObjectCompareTest
    {
        [TestMethod]
        [TestCategory("ObjectCompare")]
        public void GetSimpleObjectCompareInfoTest()
        {
            ObjectCompareInfo compareInfo = typeof(Vendor).GetCompareInfo();

            Assert.IsTrue(compareInfo.Properties.Count > 0);

            Assert.AreEqual("Vendor Code", compareInfo.Properties["Code"].Description);
            Assert.AreEqual("Vendor Description", compareInfo.Properties["Description"].Description);

            Assert.IsFalse(compareInfo.Properties.ContainsKey("CreateDate"));
        }

        [TestMethod]
        [TestCategory("ObjectCompare")]
        public void SingleObjectCompareTest()
        {
            Vendor vendor = new Vendor()
            {
                Code = "123",
                Description = "desc vendor 1",
                Name = "name vendor 1",
                VendorID = "vendorID1",
                CreateDate = DateTime.Now
            };

            Vendor newVendor = new Vendor()
            {
                Description = "desc vendor 2",
                Name = "name vendor 2",
                VendorID = "vendorID1",
                CreateDate = DateTime.Now.AddDays(-1)
            };

            ObjectCompareResult result = ObjectCompareHelper.CompareObject(vendor, newVendor);

            OutpuCompareResult(result);

            //CreateDate不参与比较
            Assert.IsTrue(result.AreDifferent);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        [TestCategory("ObjectCompare")]
        public void XmlSerializationWithSingleObjectCompareTest()
        {
            Vendor vendor = new Vendor()
            {
                Code = "123",
                Description = "desc vendor 1",
                Name = "name vendor 1",
                VendorID = "vendorID1",
                CreateDate = DateTime.Now
            };

            Vendor newVendor = new Vendor()
            {
                Description = "desc vendor 2",
                Name = "name vendor 2",
                VendorID = "vendorID1",
                CreateDate = DateTime.Now.AddDays(-1)
            };

            ObjectCompareResult result = ObjectCompareHelper.CompareObject(vendor, newVendor);

            OutpuCompareResult(result);

            XElement root = result.ToXElement();

            ObjectCompareResult deserizlized = new ObjectCompareResult();

            deserizlized.FromXElement(root);

            Assert.AreEqual(root.ToString(), deserizlized.ToXElement().ToString());
        }

        [TestMethod]
        [TestCategory("ObjectCompare")]
        public void SameObjectReferenceTest()
        {
            Vendor vendor = new Vendor()
            {
                Code = "123",
                Description = "desc vendor 1",
                Name = "name vendor 1",
                VendorID = "vendorID1",
                CreateDate = DateTime.Now
            };

            Vendor newVendor = vendor;

            ObjectCompareResult result = ObjectCompareHelper.CompareObject(vendor, newVendor);

            Assert.IsFalse(result.AreDifferent);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [TestCategory("ObjectCompare")]
        public void SourceIsNullObjectCompareTest()
        {
            Vendor newVendor = new Vendor()
            {
                Code = "123",
                Description = "desc vendor 1",
                Name = "name vendor 1",
                VendorID = "vendorID1",
                CreateDate = DateTime.Now
            };

            ObjectCompareResult result = ObjectCompareHelper.CompareObject(null, newVendor);

            OutpuCompareResult(result);

            Assert.IsTrue(result.AreDifferent);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        [TestCategory("ObjectCompare")]
        public void TargetIsNullObjectCompareTest()
        {
            Vendor vendor = new Vendor()
            {
                Code = "123",
                Description = "desc vendor 1",
                Name = "name vendor 1",
                VendorID = "vendorID1",
                CreateDate = DateTime.Now
            };

            ObjectCompareResult result = ObjectCompareHelper.CompareObject(vendor, null);

            OutpuCompareResult(result);

            Assert.IsTrue(result.AreDifferent);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        [TestCategory("ObjectCompare")]
        public void CompareEnumerableObjectTest()
        {
            VendorCollection oldVendors = PrepareOldVendorCollection();
            VendorCollection newVendors = PrepareNewVendorCollection();

            ObjectCollectionCompareResult result = ObjectCompareHelper.CompareEnumerableObject(oldVendors, newVendors);

            Assert.IsTrue(result.AreDifferent);

            Assert.AreEqual(1, result.Added.Count);
            Assert.AreEqual(1, result.Updated.Count);
            Assert.AreEqual(1, result.Deleted.Count);

            Assert.IsNotNull(result.Updated[0].FindByOldPropertyValue("Name", "vendor 2"));
            Assert.IsNotNull(result.Updated[0].FindByNewPropertyValue("Name", "vendor 22"));

            Assert.IsNotNull(result.Added[0].FindByNewPropertyValue("Name", "vendor 4"));
            Assert.IsNotNull(result.Deleted[0].FindByOldPropertyValue("Name", "vendor 3"));
        }

        [TestMethod]
        [TestCategory("ObjectCompare")]
        public void CompareTargetEnumerableObjectNullTest()
        {
            VendorCollection oldVendors = PrepareOldVendorCollection();

            ObjectCollectionCompareResult result = ObjectCompareHelper.CompareEnumerableObject(oldVendors, null);

            Assert.IsTrue(result.AreDifferent);
            Assert.AreEqual(3, result.Deleted.Count);

            for (int i = 0; i < oldVendors.Count; i++)
            {
                Assert.IsNotNull(result.Deleted[i].FindByOldPropertyValue("Name", oldVendors[i].Name));
            }
        }

        [TestMethod]
        [TestCategory("ObjectCompare")]
        public void CompareSourceEnumerableObjectNullTest()
        {
            VendorCollection newVendors = PrepareNewVendorCollection();

            ObjectCollectionCompareResult result = ObjectCompareHelper.CompareEnumerableObject(null, newVendors);

            Assert.IsTrue(result.AreDifferent);
            Assert.AreEqual(3, result.Added.Count);

            for (int i = 0; i < newVendors.Count; i++)
            {
                Assert.IsNotNull(result.Added[i].FindByNewPropertyValue("Name", newVendors[i].Name));
            }
        }

        [TestMethod]
        [TestCategory("ObjectCompare")]
        public void CompareEnumerableObjectNullTest()
        {
            ObjectCollectionCompareResult result = ObjectCompareHelper.CompareEnumerableObject(null, null);

            Assert.IsFalse(result.AreDifferent);
        }

        [TestMethod]
        [TestCategory("ObjectCompare")]
        public void CompareOrderTest()
        {
            Order oldOrder = PrepareOldOrderData();
            Order newOrder = PrepareNewOrderData();

            ObjectCompareResult result = ObjectCompareHelper.CompareObject(oldOrder, newOrder);

            OutpuCompareResult(result);

            Assert.IsTrue(result.AreDifferent);
            Assert.AreEqual(2, result.Count);

            Assert.IsTrue(result["Vendors"].SubObjectCompareResult.AreDifferent);
            Assert.IsTrue(result["Creator"].SubObjectCompareResult.AreDifferent);
        }

        [TestMethod]
        [TestCategory("ObjectCompare")]
        public void XmlSerializationWithOrderCompareTest()
        {
            Order oldOrder = PrepareOldOrderData();
            Order newOrder = PrepareNewOrderData();

            ObjectCompareResult result = ObjectCompareHelper.CompareObject(oldOrder, newOrder);

            OutpuCompareResult(result);

            XElement root = result.ToXElement();

            ObjectCompareResult deserizlized = new ObjectCompareResult();

            deserizlized.FromXElement(root);

            OutpuCompareResult(deserizlized);

            Assert.AreEqual(root.ToString(), deserizlized.ToXElement().ToString());
        }

        private static VendorCollection PrepareOldVendorCollection()
        {
            VendorCollection vendors = new VendorCollection();

            vendors.Add(new Vendor()
            {
                Code = "1",
                Description = "vendor 1",
                Name = "vendor 1",
                VendorID = "vendorID1"
            });

            vendors.Add(new Vendor()
            {
                Code = "2",
                Description = "vendor 2",
                Name = "vendor 2",
                VendorID = "vendorID2"
            });

            //将被删除的
            vendors.Add(new Vendor()
            {
                Code = "3",
                Description = "vendor 3",
                Name = "vendor 3",
                VendorID = "vendorID3"
            });

            return vendors;
        }

        private static VendorCollection PrepareNewVendorCollection()
        {
            VendorCollection vendors = new VendorCollection();

            //不变的
            vendors.Add(new Vendor()
            {
                Code = "1",
                Description = "vendor 1",
                Name = "vendor 1",
                VendorID = "vendorID1"
            });

            //变化的
            vendors.Add(new Vendor()
            {
                Code = "2",
                Description = "vendor 22",
                Name = "vendor 22",
                VendorID = "vendorID2"
            });

            //增加的
            vendors.Add(new Vendor()
            {
                Code = "4",
                Description = "vendor 4",
                Name = "vendor 4",
                VendorID = "vendorID4"
            });

            return vendors;
        }

        private static void OutpuCompareResult(ObjectCompareResult result)
        {
            Console.WriteLine(result.ToXElement().ToString());
        }

        private static Order PrepareOldOrderData()
        {
            Order order = new Order();

            order.OrderNumber = "7412369";

            order.Vendors.CopyFrom(PrepareOldVendorCollection());
            order.Creator = new TestUser() { ID = "625", Name = "金三胖" };

            return order;
        }

        private static Order PrepareNewOrderData()
        {
            Order order = new Order();

            order.OrderNumber = "7412369";

            order.Vendors.CopyFrom(PrepareNewVendorCollection());
            order.Creator = new TestUser() { ID = "625", Name = "金四胖" };

            return order;
        }
    }
}
