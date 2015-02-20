using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MCS.Library.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.Test
{
    [TestClass]
    public class CacheNotifyDataMapTest
    {
        [TestMethod]
        [TestCategory("CacheNotifyDataMapInfo")]
        public void WriteOneNotifyData()
        {
            CacheNotifyDataMap.ResetMap();

            CacheNotifyData data = PrepareData("Test");

            CacheNotifyDataMap.WriteCacheNotifyData(data);

            long lastTicks = 0;
            CacheNotifyData[] dataRead = CacheNotifyDataMap.ReadCacheNotifyData(ref lastTicks);

            CacheNotifyDataMapInfo mapInfo = CacheNotifyDataMap.GetCacheNotifyDataMapInfo();

            Console.WriteLine(mapInfo.Pointer);
            Console.WriteLine(lastTicks);

            Assert.AreEqual(1, mapInfo.Pointer);
            Assert.IsTrue(dataRead.Length > 0);
            Assert.IsTrue(lastTicks > 0);
            AssertCacheNotifyData(data, dataRead[0]);

            CacheNotifyData[] dataReadAgain = CacheNotifyDataMap.ReadCacheNotifyData(ref lastTicks);

            Assert.AreEqual(0, dataReadAgain.Length);
        }

        [TestMethod]
        [TestCategory("CacheNotifyDataMapInfo")]
        public void WriteOverflowNotifyData()
        {
            CacheNotifyDataMap.ResetMap();

            long lastTicks = DateTime.Now.Ticks;

            //会比最大项多写一项
            for (int i = 0; i <= CacheNotifyDataMapInfo.CacheDataItemCount; i++)
            {
                CacheNotifyData data = PrepareData("Test" + i.ToString());

                CacheNotifyDataMap.WriteCacheNotifyData(data);
            }

            CacheNotifyData[] dataRead = CacheNotifyDataMap.ReadCacheNotifyData(ref lastTicks);

            Assert.AreEqual(CacheNotifyDataMapInfo.CacheDataItemCount, dataRead.Length);

            bool existZeroItem = false;
            bool existLastItem = false;

            for (int i = 0; i < dataRead.Length; i++)
            {
                if (dataRead[i].CacheKey.ToString() == "Test0")
                    existZeroItem = true;
                else
                    if (dataRead[i].CacheKey.ToString() == "Test" + CacheNotifyDataMapInfo.CacheDataItemCount)
                        existLastItem = true;
            }

            Assert.IsFalse(existZeroItem, "第0项应该被挤掉");
            Assert.IsTrue(existLastItem, "最后一项应该被保留");
        }

        [TestMethod]
        [TestCategory("CacheNotifyDataMapInfo")]
        public void WriteNotExistNotifyData()
        {
            CacheNotifyDataMap.ResetMap();

            CacheNotifyDataMap.WriteCacheNotifyData(PrepareData("TestExistData"));
            CacheNotifyDataMap.WriteCacheNotifyData(PrepareData("Test1"));

            long lastTicks = 0;
            CacheNotifyDataMap.WriteNotExistCacheNotifyData(lastTicks, PrepareData("TestExistData"));
            CacheNotifyDataMap.WriteNotExistCacheNotifyData(lastTicks, PrepareData("Test2"));

            CacheNotifyData[] dataRead = CacheNotifyDataMap.ReadCacheNotifyData(ref lastTicks);

            Assert.AreEqual(3, dataRead.Length);
        }

        private static CacheNotifyData PrepareData(string cacheKey)
        {
            return new CacheNotifyData(typeof(ObjectCacheQueue), cacheKey, CacheNotifyType.Clear);
        }

        private static void AssertCacheNotifyData(CacheNotifyData original, CacheNotifyData target)
        {
            Assert.AreEqual(original.CacheQueueTypeDesp, target.CacheQueueTypeDesp);
            Assert.AreEqual(original.CacheKey, target.CacheKey);
            Assert.AreEqual(original.NotifyType, target.NotifyType);
        }
    }
}
