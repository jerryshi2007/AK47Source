#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	CacheManager.cs
// Remark	��	CacheQueue������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
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
	/// ȫ�־�̬�࣬ͨ��ע�᷽ʽͳһ����Ӧ����������CacheQueue
	/// </summary>
	public static class CacheManager
	{
		//����Ӧ����������CacheQueue�ֵ䣬����ע��
		private static Dictionary<Type, CacheQueueBase> innerCacheQueues = null;

		[ThreadStatic]
		private static bool inScavengeThread = false;

		private static CachingPerformanceCounters totalCounters = null;

		static CacheManager()
		{
			CacheManager.innerCacheQueues = new Dictionary<Type, CacheQueueBase>();
			CacheManager.totalCounters = new CachingPerformanceCounters("_Total_");

			//��̨�����̣߳�������������Ӧ������ÿһ��CacheQueue�е�ÿһ��Cache��
			//���߳���ϵͳ����ʱ�Զ����������ܿͻ��˴������
			InitScavengingThread();
		}

		/// <summary>
		/// ��ʾ��ǰ�Ƿ��ں�̨����״̬����������Ҫ������Cache�����ڲ��жϵ�ǰ�������̵߳Ĺ���״̬�С�
		/// ��CacheQueue��Dependency���ڲ�������ͨ����״̬�жϵ������Ƿ��������߳�
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
		/// ע�ắ����Ӧ������ȫ�����͵�CacheQueue�����ͨ����
		/// ��������ע�ᣬ���Ӧ�������Ѿ�������ͬ���͵�CacheQueue�����������÷���
		/// �Ա�֤ϵͳ��ÿһ�����͵�CacheQueue������Ψһ��
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <returns>����ע��CacheQueue���������</returns>
		/// <remarks>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheManagerTest.cs" region="RegisterCacheTest" lang="cs" title="ע���Զ����CacheQueue����" />
		/// </remarks>
		public static T GetInstance<T>() where T : CacheQueueBase
		{
			return (T)GetInstance(typeof(T));
		}

		/// <summary>
		/// ע�ắ����Ӧ������ȫ�����͵�CacheQueue�����ͨ����
		/// ��������ע�ᣬ���Ӧ�������Ѿ�������ͬ���͵�CacheQueue�����������÷���
		/// �Ա�֤ϵͳ��ÿһ�����͵�CacheQueue������Ψһ��
		/// </summary>
		/// <param name="type">Cache���е�����</param>
		/// <returns>����ע��CacheQueue���������</returns>
		/// <remarks>
		/// </remarks>
		public static CacheQueueBase GetInstance(Type type)
		{
			type.NullCheck("type");

			//��֤�̰߳�ȫ��
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
		/// �������Cache���е���Ϣ
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
		/// ������еĻ���
		/// </summary>
		public static void ClearAllCache()
		{
			lock (((IDictionary)CacheManager.innerCacheQueues).SyncRoot)
			{
				//��ע���ÿһ���͵�CacheQueue����ɨ������
				foreach (KeyValuePair<Type, CacheQueueBase> cache in CacheManager.innerCacheQueues)
				{
					cache.Value.Clear();
				}
			}
		}

		/// <summary>
		/// ����һ�������̣߳����һ��������
		/// </summary>
		public static void StartScavengeThread()
		{
			Thread t = new Thread(new ThreadStart(InternalScavenge));

			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();
		}

		/// <summary>
		/// ��ʼ����������̨�����߳�
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
		/// CacheQueue��������
		/// </summary>
		private static void ScavengeCache()
		{
			while (true)
			{
				try
				{
					//�������ڣ��������ļ��н�������
					Thread.Sleep(CacheSettingsSection.GetConfig().ScanvageInterval);

					InternalScavenge();
				}
				catch (ThreadAbortException)
				{
				}
				catch (System.Exception ex)
				{
					ex.WriteToEventLog("ScavengeCache");

					//������ѭ��������ռ��CPU
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
				//��ע���ÿһ���͵�CacheQueue����ɨ������
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
