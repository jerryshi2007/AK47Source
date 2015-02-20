using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Test.Users
{
    [TestClass]
    public class UserRecentDataTest
    {

        static readonly string demo_id = "DEMO_ID";


        [TestCleanup]
        public void CleanUp()
        {
            //执行清理
            using (DbContext dbi = DbContext.GetContext(ConnectionDefine.UserRelativeInfoConnectionName))
            {
                Database db = DatabaseFactory.Create(dbi);
                db.ExecuteNonQuery(System.Data.CommandType.Text, "DELETE FROM dbo.USER_RECENT_DATA WHERE USER_ID = '" + demo_id + "'");
            }
        }

        [Description("测试配置读取功能")]
        [TestMethod]
		[TestCategory(ProcessTestHelper.UserRecentData)]
        public void TestGetConfig()
        {
            var config = MCS.Library.SOA.DataObjects.UserRecentDataConfigurationSection.GetConfig();
            Assert.IsNotNull(config);
        }


        [Description("测试用户配置")]
        [TestMethod]
		[TestCategory(ProcessTestHelper.UserRecentData)]
        public void TestUserRecentData()
        {
            CleanUp();

            Thread.Sleep(2000);//延时

            UserRecentData settings = UserRecentData.GetSettings(demo_id);
            Assert.AreEqual(settings.UserID, demo_id);
            var cat = settings.Categories["recentTimepoints"];
            int defaultSize = cat.DefaultSize;
            Assert.IsTrue(defaultSize > 0);
            Assert.IsTrue(cat.Items.Count == 0);
            var item = cat.Items.CreateItem();
            item.SetValue<DateTime>("timePoint", new DateTime(2008, 10, 30));
            item.SetValue<DateTime>("lastAccessDate", new DateTime(2018, 12, 30));
            item.SetValue<bool>("pinned", true);
            cat.Items.Add(item);
            settings.Update("recentTimepoints");
            cat.SaveChanges(demo_id);

            settings = UserRecentData.GetSettings(demo_id);
            cat = settings.Categories["recentTimepoints"];
            Assert.IsTrue(cat.Items.Count == 1);

            var item1 = cat.Items[0];
            foreach (var key in item1.GetAllKeys())
            {
                Assert.AreEqual(item1[key].StringValue, item[key].StringValue);
                Assert.AreEqual(item1[key].Definition.DataType, item[key].Definition.DataType);
            }
        }


        [Description("测试最近的数据集合")]
        [TestMethod]
		[TestCategory(ProcessTestHelper.UserRecentData)]
        public void TestRecentDataItemCollection()
        {
            CleanUp();

            Thread.Sleep(2000);//延时


            UserRecentData settings = UserRecentData.GetSettings(demo_id);
            var cat = settings.Categories["recentTimepoints"];

            //测试保存多个日期

            DateTime dtBase = DateTime.Now;
            for (int i = 0; i < cat.DefaultSize; i++)
            {
                var item = cat.Items.CreateItem();
                item.SetValue<DateTime>("timePoint", dtBase.AddDays(i * 2));
                item.SetValue<DateTime>("lastAccessDate", dtBase.AddDays(i * 2 + 1));
                item.SetValue<bool>("pinned", dtBase.AddDays(i).Day / 2 == 0);
                cat.Items.Add(item);
            }

            var item1 = cat.Items.CreateItem();
            item1.SetValue<DateTime>("timePoint", new DateTime(2008, 3, 3));
            item1.SetValue<DateTime>("lastAccessDate", new DateTime(2009, 4, 4));
            item1.SetValue<bool>("pinned", true); 



            cat.Items.Insert(0, item1);
            Assert.AreEqual(cat.Items.Count, cat.DefaultSize); //应该把多余的项排出

            List<PropertyValueCollection> listCompare = new List<PropertyValueCollection>();
            cat.Items.CopyTo(listCompare);

            var itemA = cat.Items[cat.Items.Count - 1];
            var itemB = cat.Items[cat.Items.Count - 2];

            cat.Items.Advance(cat.Items.Count - 1); //最后一项向前排

            Assert.AreEqual(cat.Items.Count, cat.DefaultSize); //长度不应变化
            Assert.AreEqual(cat.Items[0], itemA); //最后一项移至开头
            Assert.AreEqual(cat.Items[1], item1); //应该排在后面

            cat.Items.Advance(0); //测试

            Assert.AreEqual(itemA, cat.Items[0]);  //提升第一项应该没有作用

            cat.Items.Advance(1); //提升第二项

            Assert.AreEqual(itemA, cat.Items[1]); //原来的第一项应该后移
            Assert.AreEqual(item1, cat.Items[0]); //第二项应该前移
        }
    }
}
