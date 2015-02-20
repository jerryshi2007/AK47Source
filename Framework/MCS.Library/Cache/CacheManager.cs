#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	CacheManager.cs
// Remark	：	CacheQueue管理类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    万振龙	    20070430		创建
// -------------------------------------------------
#endregion

#region using
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Configuration;
using MCS.Library.Core;
#endregion

namespace MCS.Library.Caching
{
	/// <summary>
	/// 全局静态类，通过注册方式统一管理应用域内所有CacheQueue
	/// </summary>
	public static class CacheManager
	{
		//保存应用域内所有CacheQueue字典，用来注册
		private static Dictionary<Type, CacheQueueBase> innerCacheQueues = null;

		[ThreadStatic]
		private static bool inScavengeThread = false;

		private static CachingPerformanceCounters totalCounters = null;

		static CacheManager()
		{
			CacheManager.innerCacheQueues = new Dictionary<Type, CacheQueueBase>();
			CacheManager.totalCounters = new CachingPerformanceCounters("_Total_");

			//后台清理线程，定期清理整个应用域中每一个CacheQueue中的每一个Cache项
			//此线程在系统启动时自动启动，不受客户端代码控制
			InitScavengingThread();
		}

		/// <summary>
		/// 表示当前是否处于后台清理状态，此属性主要用于在Cache队列内部判断当前是清理线程的工作状态中。
		/// 在CacheQueue或Dependency的内部，可以通过此状态判断调用者是否是清理线程
		/// </summary>
		public static bool InScavengeThread
		{
			get
			{
				return CacheManager.inScavengeThread;
			}
			internal set
			{
				CacheManager.inScavengeThread = value;
			}
		}

		/// <summary>
		/// 注册函数，应用域中全部类型的CacheQueue对象均通过此
		/// 函数进行注册，如果应用域中已经存在相同类型的CacheQueue对象，则将其引用返回
		/// 以保证系统内每一种类型的CacheQueue对象是唯一的
		/// </summary>
		/// <typeparam name="T">泛型类型</typeparam>
		/// <returns>返回注册CacheQueue对象的引用</returns>
		/// <remarks>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheManagerTest.cs" region="RegisterCacheTest" lang="cs" title="注册自定义的CacheQueue对象" />
		/// </remarks>
		public static T GetInstance<T>() where T : CacheQueueBase
		{
			return (T)GetInstance(typeof(T));
		}

		/// <summary>
		/// 注册函数，应用域中全部类型的CacheQueue对象均通过此
		/// 函数进行注册，如果应用域中已经存在相同类型的CacheQueue对象，则将其引用返回
		/// 以保证系统内每一种类型的CacheQueue对象是唯一的
		/// </summary>
		/// <param name="type">Cache队列的类型</param>
		/// <returns>返回注册CacheQueue对象的引用</returns>
		/// <remarks>
		/// </remarks>
		public static CacheQueueBase GetInstance(Type type)
		{
			type.NullCheck("type");

			//保证线程安全性
			lock (((IDictionary)CacheManager.innerCacheQueues).SyncRoot)
			{
				CacheQueueBase result;

				if (CacheManager.innerCacheQueues.TryGetValue(type, out result) == false)
				{
					result = (CacheQueueBase)Activator.CreateInstance(type, true);
					CacheManager.innerCacheQueues.Add(type, result);
				}

				return result;
			}
		}

		/// <summary>
		/// 获得所有Cache队列的信息
		/// </summary>
		/// <returns></returns>
		public static CacheQueueInfoCollection GetAllCacheInfo()
		{
			CacheQueueInfoCollection result = new CacheQueueInfoCollection();

			lock (((IDictionary)CacheManager.innerCacheQueues).SyncRoot)
			{
				foreach (KeyValuePair<Type, CacheQueueBase> kp in CacheManager.innerCacheQueues)
				{
					CacheQueueInfo queueInfo = new CacheQueueInfo();

					queueInfo.QueueTypeName = kp.Key.FullName;
					queueInfo.QueueTypeFullName = kp.Key.AssemblyQualifiedName;
					queueInfo.Count = kp.Value.Count;

					result.Add(queueInfo);
				}
			}

			return result;
		}

		/// <summary>
		/// 清空所有的缓存
		/// </summary>
		public static void ClearAllCache()
		{
			lock (((IDictionary)CacheManager.innerCacheQueues).SyncRoot)
			{
				//对注册的每一类型的CacheQueue进行扫描清理
				foreach (KeyValuePair<Type, CacheQueueBase> cache in CacheManager.innerCacheQueues)
				{
					cache.Value.Clear();
				}
			}
		}

		/// <summary>
		/// 启动一个清理线程，完成一次清理工作
		/// </summary>
		public static void StartScavengeThread()
		{
			Thread t = new Thread(new ThreadStart(InternalScavenge));

			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();
		}

		/// <summary>
		/// 初始化并启动后台清理线程
		/// </summary>
		/// <returns></returns>
		private static Thread InitScavengingThread()
		{
			Thread t = new Thread(new ThreadStart(ScavengeCache));

			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();

			return t;
		}

		/// <summary>
		/// CacheQueue的清理方法
		/// </summary>
		private static void ScavengeCache()
		{
			while (true)
			{
				try
				{
					//清理周期，在配置文件中进行设置
					Thread.Sleep(CacheSettingsSection.GetConfig().ScanvageInterval);

					InternalScavenge();
				}
				catch (ThreadAbortException)
				{
				}
				catch (System.Exception ex)
				{
					ex.WriteToEventLog("ScavengeCache");

					//避免死循环，长期占用CPU
					Thread.Sleep(500);
				}
				finally
				{
					CacheManager.InScavengeThread = false;
				}
			}
		}

		private static void InternalScavenge()
		{
			int totalCount = 0;
			CacheManager.InScavengeThread = true;

			lock (((IDictionary)CacheManager.innerCacheQueues).SyncRoot)
			{
				//对注册的每一类型的CacheQueue进行扫描清理
				foreach (KeyValuePair<Type, CacheQueueBase> cache in CacheManager.innerCacheQueues)
				{
					if (cache.Value is IScavenge)
					{
						IScavenge cacheScavenge = (IScavenge)cache.Value;
						cacheScavenge.DoScavenging();
					}

					if (cache.Value is CacheQueueBase)
						totalCount += ((CacheQueueBase)cache.Value).Count;
				}
			}

			CacheManager.totalCounters.EntriesCounter.RawValue = totalCount;
		}
	}
}
