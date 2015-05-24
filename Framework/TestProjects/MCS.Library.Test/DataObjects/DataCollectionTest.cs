using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.Test.DataObjects
{
    [TestClass]
    public class DataCollectionTest
    {
        [TestMethod]
        public void CopyFromWithFilterTest()
        {
            OrderCollection orders = new OrderCollection();

            orders.Add(PrepareOneOrderData("7412369", new TestUser() { ID = "625", Name = "金大胖" }));
            orders.Add(PrepareOneOrderData("9632147", new TestUser() { ID = "725", Name = "金二胖" }));

            OrderCollection filteredOrders = new OrderCollection();

            filteredOrders.CopyFrom(orders, (o) => o.OrderNumber == "7412369");

            Assert.AreEqual(1, filteredOrders.Count);
            Assert.IsTrue(filteredOrders.Exists((o) => o.OrderNumber == "7412369"));
        }

        [TestMethod]
        public void CopyToWithFilterTest()
        {
            OrderCollection orders = new OrderCollection();

            orders.Add(PrepareOneOrderData("7412369", new TestUser() { ID = "625", Name = "金大胖" }));
            orders.Add(PrepareOneOrderData("9632147", new TestUser() { ID = "725", Name = "金二胖" }));

            OrderCollection filteredOrders = new OrderCollection();

            orders.CopyTo(filteredOrders, (o) => o.OrderNumber == "7412369");

            Assert.AreEqual(1, filteredOrders.Count);
            Assert.IsTrue(filteredOrders.Exists((o) => o.OrderNumber == "7412369"));
        }

        private static Order PrepareOneOrderData(string orderNumber, TestUser creator)
        {
            Order order = new Order();

            order.OrderNumber = orderNumber;

            order.Creator = creator;

            return order;
        }
    }
}
