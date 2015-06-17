using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
    /// <summary>
    /// 基本数据类型测试
    /// </summary>
    [TestClass]
    public class BasicDataObjectsTest
    {
        //[TestMethod]
        //[TestCategory(ProcessTestHelper.BasicDataObject)]
        //public void TreeNodeTest()
        //{
        //    SampleTreeNode root = CreateTestTree();

        //    //遍历树
        //    root.TraverseChildren((node, context) =>
        //    {
        //        Console.WriteLine(((SampleTreeNode)node).Data);
        //        return true;
        //    });
        //}

        //private static SampleTreeNode CreateTestTree()
        //{
        //    SampleTreeNode root = new SampleTreeNode("Root");

        //    root.Children.Add(new SampleTreeNode("Child1"));
        //    root.Children.Add(new SampleTreeNode("Child2"));

        //    root.FirstChild.Children.Add(new SampleTreeNode("Child11"));
        //    root.FirstChild.Children.Add(new SampleTreeNode("Child12"));
        //    root.LastChild.Children.Add(new SampleTreeNode("Child121"));
        //    root.LastChild.Children.Add(new SampleTreeNode("Child122"));

        //    return root;
        //}

        [TestMethod]
        [TestCategory(ProcessTestHelper.BasicDataObject)]
        public void EditableKeyedCollectionSortTest()
        {
            SimpleOrderCollection orders = PrepareSimpleOrders();

            Comparison<SimpleOrder> comparision = (left, right) => left.OrderNo.CompareTo(right.OrderNo);

            orders.Sort(comparision);

            Assert.AreEqual("01", orders[0].OrderNo);
            Assert.AreEqual("10", orders[1].OrderNo);
            Assert.AreEqual("15", orders[2].OrderNo);
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.BasicDataObject)]
        public void SimpleConnectionMappingTest()
        {
            DbConnectionMappingContext.DoMappingAction("HB2008", "HB2009", () =>
            {
                Assert.AreEqual("HB2009", DbConnectionMappingContextCache.Instance["HB2008"].DestinationConnectionName);
            });

            Assert.IsFalse(DbConnectionMappingContextCache.Instance.ContainsKey("HB2008"));
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.BasicDataObject)]
        public void NestedConnectionMappingTest()
        {
            DbConnectionMappingContext.DoMappingAction("HB2008", "HB2009", () =>
                {
                    Assert.AreEqual("HB2009", DbConnectionMappingContextCache.Instance["HB2008"].DestinationConnectionName);

                    DbConnectionMappingContext.DoMappingAction("HB2008", "HB2010", () =>
                    {
                        Assert.AreEqual("HB2010", DbConnectionMappingContextCache.Instance["HB2008"].DestinationConnectionName);
                    });

                    Assert.AreEqual("HB2009", DbConnectionMappingContextCache.Instance["HB2008"].DestinationConnectionName);
                }
            );

            Assert.IsFalse(DbConnectionMappingContextCache.Instance.ContainsKey("HB2008"));
        }

        private static SimpleOrderCollection PrepareSimpleOrders()
        {
            SimpleOrder order15 = new SimpleOrder() { OrderNo = "15", OrderName = "第15号订单" };
            SimpleOrder order1 = new SimpleOrder() { OrderNo = "01", OrderName = "第1号订单" };
            SimpleOrder order10 = new SimpleOrder() { OrderNo = "10", OrderName = "第10号订单" };

            SimpleOrderCollection result = new SimpleOrderCollection();

            result.Add(order15);
            result.Add(order1);
            result.Add(order10);

            return result;
        }
    }
}
