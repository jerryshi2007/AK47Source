using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
	/// <summary>
	/// 
	/// </summary>
	public class CacheNotifyDataMap
	{
		/// <summary>
		/// 映射文件的标识
		/// </summary>
		public const string MappedFileTag = "MemoryMappedFileCacheNotofier";

		/// <summary>
		/// 互斥量的标识
		/// </summary>
		public const string MutextTag = "Global\\MemoryMappedFileMutex";

		private static readonly string TempFileName = Path.Combine(
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"), "CacheNotifyDataMap");

		/// <summary>
		/// 互斥量等待的超时时间
		/// </summary>
		private static readonly TimeSpan MutexTimeout = TimeSpan.FromMilliseconds(500);

		/// <summary>
		/// 得到总内存尺寸
		/// </summary>
		/// <returns></returns>
		public static long GetTotalMemorySize()
		{
			return Marshal.SizeOf(typeof(CacheNotifyDataMapInfo))
				+ (Marshal.SizeOf(typeof(CacheNotifyDataMapItem)) + CacheNotifyDataMapInfo.CacheDataBlockSize) * CacheNotifyDataMapInfo.CacheDataItemCount;
		}

		/// <summary>
		/// 打开文件
		/// </summary>
		/// <returns></returns>
		public static MemoryMappedFile OpenFile(FileMode fileMode)
		{
			return MemoryMappedFile.CreateFromFile(TempFileName, fileMode, MappedFileTag, GetTotalMemorySize());
		}

		/// <summary>
		/// 重置所有数据
		/// </summary>
		public static void ResetMap()
		{
			DoAccessor(FileMode.OpenOrCreate, accessor => InnerResetData(accessor));
		}

		/// <summary>
		/// 返回Memory Mapped中的Cache通知信息的头信息
		/// </summary>
		/// <returns></returns>
		public static CacheNotifyDataMapInfo GetCacheNotifyDataMapInfo()
		{
			CacheNotifyDataMapInfo result = (CacheNotifyDataMapInfo)DoAccessor(FileMode.Open, accessor =>
			{
				CacheNotifyDataMapInfo mapInfo;
				accessor.Read(0, out mapInfo);

				return mapInfo.Clone();
			});

			return result;
		}

		/// <summary>
		/// 将一组通知数据写入到内存映射文件中
		/// </summary>
		/// <param name="notifyData"></param>
		public static void WriteCacheNotifyData(params CacheNotifyData[] notifyData)
		{
			notifyData.NullCheck("notifyData");

			DoAccessor(FileMode.OpenOrCreate, accessor =>
			{
				CacheNotifyDataMapInfo mapInfo;

				accessor.Read(0, out mapInfo);

				if (mapInfo.Mark != CacheNotifyDataMapInfo.Tag)
					mapInfo = InnerResetData(accessor);

				long currentTicks = DateTime.Now.Ticks;

				notifyData.ForEach(data => WriteOneCacheNotifyData(accessor, ref mapInfo, data, currentTicks));
			});
		}

		/// <summary>
		/// 将一组Key不存在的通知数据写入到内存映射文件中，避免重复写入
		/// </summary>
		/// <param name="lastTicks">内存映射文件中在此Ticks之后的通知数据</param>
		/// <param name="notifyData"></param>
		public static int WriteNotExistCacheNotifyData(long lastTicks, params CacheNotifyData[] notifyData)
		{
			notifyData.NullCheck("notifyData");

			int count = 0;

			DoAccessor(FileMode.OpenOrCreate, accessor =>
			{
				CacheNotifyDataMapInfo mapInfo;

				accessor.Read(0, out mapInfo);

				if (mapInfo.Mark != CacheNotifyDataMapInfo.Tag)
					mapInfo = InnerResetData(accessor);

				List<CacheNotifyData> existData = new List<CacheNotifyData>();

				InnerReadCacheNotifyData(lastTicks, accessor, mapInfo, existData);

				long currentTicks = DateTime.Now.Ticks;

				foreach (CacheNotifyData data in notifyData)
				{
					if (existData.Exists(c => c.CacheKey.Equals(data.CacheKey)) == false)
					{
						WriteOneCacheNotifyData(accessor, ref mapInfo, data, currentTicks);
						count++;
					}
				}
			});

			return count;
		}

		/// <summary>
		/// 读取在Memory Mapped中的所有Cache通知信息。并且返回最后的Ticks
		/// </summary>
		/// <param name="lastTicks">最后的执行Ticks，这个值也可以返回为最大的Ticks</param>
		/// <returns></returns>
		public static CacheNotifyData[] ReadCacheNotifyData(ref long lastTicks)
		{
			List<CacheNotifyData> result = new List<CacheNotifyData>();
			long resultTicks = lastTicks;

			DoAccessor(FileMode.Open, accessor =>
			{
				CacheNotifyDataMapInfo mapInfo;

				accessor.Read(0, out mapInfo);

				if (mapInfo.Mark == CacheNotifyDataMapInfo.Tag)
					resultTicks = InnerReadCacheNotifyData(resultTicks, accessor, mapInfo, result);
			}
			);

			lastTicks = resultTicks;

			return result.ToArray();
		}

		private static long InnerReadCacheNotifyData(long lastTicks, MemoryMappedViewAccessor accessor, CacheNotifyDataMapInfo mapInfo, List<CacheNotifyData> result)
		{
			int itemSize = Marshal.SizeOf(typeof(CacheNotifyDataMapItem));
			long returnTicks = lastTicks;

			long startPointer = Marshal.SizeOf(typeof(CacheNotifyDataMapInfo));

			for (int i = 0; i < CacheNotifyDataMapInfo.CacheDataItemCount; i++)
			{
				CacheNotifyDataMapItem item;

				accessor.Read(startPointer, out item);

				if (item.Ticks > lastTicks)
				{
					if (item.Ticks > returnTicks)
						returnTicks = item.Ticks;

					byte[] data = new byte[item.Size];

					accessor.ReadArray(startPointer + itemSize, data, 0, (int)item.Size);

					CacheNotifyData cnd = CacheNotifyData.FromBuffer(data);

					result.Add(cnd);
				}

				startPointer += itemSize + CacheNotifyDataMapInfo.CacheDataBlockSize;
			}

			return returnTicks;
		}

		private static void WriteOneCacheNotifyData(MemoryMappedViewAccessor accessor, ref CacheNotifyDataMapInfo mapInfo, CacheNotifyData notifyData, long currentTicks)
		{
			if (mapInfo.Pointer >= CacheNotifyDataMapInfo.CacheDataItemCount)
				mapInfo.Pointer = 0;

			long startPointer = Marshal.SizeOf(typeof(CacheNotifyDataMapInfo)) +
				mapInfo.Pointer * (Marshal.SizeOf(typeof(CacheNotifyDataMapItem)) + CacheNotifyDataMapInfo.CacheDataBlockSize);

			byte[] data = notifyData.ToBytes();

			CacheNotifyDataMapItem item = new CacheNotifyDataMapItem();

			item.Ticks = currentTicks;
			item.Size = data.Length;

			accessor.Write(startPointer, ref item);

			long dataPointer = startPointer + Marshal.SizeOf(typeof(CacheNotifyDataMapItem));

			accessor.WriteArray(dataPointer, data, 0, data.Length);

			mapInfo.Pointer++;

			accessor.Write(0, ref mapInfo);

			UdpCacheNotifier.TotalCounters.MmfSentItemsCounter.Increment();
			UdpCacheNotifier.TotalCounters.MmfSentCountPerSecond.Increment();

			UdpCacheNotifier.AppInstanceCounters.MmfSentItemsCounter.Increment();
			UdpCacheNotifier.AppInstanceCounters.MmfSentCountPerSecond.Increment();

			UdpCacheNotifier.TotalCounters.MmfCurrentPointer.RawValue = mapInfo.Pointer;
			UdpCacheNotifier.AppInstanceCounters.MmfCurrentPointer.RawValue = mapInfo.Pointer;
		}

		private static CacheNotifyDataMapInfo InnerResetData(MemoryMappedViewAccessor accessor)
		{
			for (int i = 0; i < GetTotalMemorySize(); i++)
				accessor.Write(i, (byte)0);

			CacheNotifyDataMapInfo mapInfo;

			accessor.Read(0, out mapInfo);

			mapInfo.Mark = CacheNotifyDataMapInfo.Tag;

			accessor.Write(0, ref mapInfo);

			return mapInfo;
		}

		private static void DoAccessor(FileMode fileMode, Action<MemoryMappedViewAccessor> action)
		{
			long totalSize = GetTotalMemorySize();
			bool mutexCreated = false;

			using (Mutex mutex = new Mutex(false, MutextTag, out mutexCreated))
			{
				if (mutex.WaitOne(MutexTimeout))
				{
					try
					{
						using (MemoryMappedFile mmf = OpenFile(fileMode))
						{
							using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor(0, totalSize))
							{
								action(accessor);
							}
						}
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}
		}

		private static object DoAccessor(FileMode fileMode, Func<MemoryMappedViewAccessor, object> func)
		{
			long totalSize = GetTotalMemorySize();
			bool mutexCreated = false;

			object result = null;

			using (Mutex mutex = new Mutex(false, MutextTag, out mutexCreated))
			{
				if (mutex.WaitOne(MutexTimeout))
				{
					try
					{
						using (MemoryMappedFile mmf = OpenFile(fileMode))
						{
							using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor(0, totalSize))
							{
								result = func(accessor);
							}
						}
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}

			return result;
		}
	}
}
