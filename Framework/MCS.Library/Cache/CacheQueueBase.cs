#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	CacheQueueBase.cs
// Remark	��	CacheQueue����
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
	/// <summary>
	/// Cache���е������
	/// </summary>
	public abstract class CacheQueueBase
	{
		private CachingPerformanceCounters totalCounters;
		private CachingPerformanceCounters counters;

		/// <summary>
		/// Cache�������
		/// </summary>
		public abstract int Count
		{
			get;
		}

		/// <summary>
		/// ���Cache����
		/// </summary>
		public abstract void Clear();

		/// <summary>
		/// �Ƿ񶼱��Ϊ����
		/// </summary>
		public abstract void SetChanged();

		/// <summary>
		/// �鷽����ɾ��Cache��
		/// </summary>
		/// <param name="cacheItem">��ɾ����Cache��</param>
		internal protected abstract void RemoveItem(CacheItemBase cacheItem);

		/// <summary>
		/// �õ��������������Ϣ
		/// </summary>
		/// <returns></returns>
		public abstract CacheItemInfoCollection GetAllItemsInfo();

		/// <summary>
		/// ����ToString������ڲ�������
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string result = base.ToString();

			result += string.Format("({0:#,##0})", this.Count);

			return result;
		}

		/// <summary>
		/// ���췽������ʼ������ָ��
		/// </summary>
		protected CacheQueueBase()
		{
			this.InitPerformanceCounters(this.GetType().Name);
		}

		/// <summary>
		/// ���췽������ʼ������ָ��
		/// </summary>
		protected CacheQueueBase(string instanceName)
		{
			this.InitPerformanceCounters(instanceName);
		}

		/// <summary>
		/// ��ʼ�����ܼ���ָ��
		/// </summary>
		/// <param name="instanceName">�������ܼ�������ָ��</param>
		protected void InitPerformanceCounters(string instanceName)
		{
			if (this.totalCounters == null)
				this.totalCounters = new CachingPerformanceCounters("_Total_");

			if (this.counters == null)
			{
				instanceName.CheckStringIsNullOrEmpty("instanceName");
				this.counters = new CachingPerformanceCounters(instanceName);
			}
		}

		/// <summary>
		/// ����Cache������ָ��
		/// </summary>
		protected CachingPerformanceCounters TotalCounters
		{
			get
			{
				return this.totalCounters;
			}
		}

		/// <summary>
		/// ����ָ��
		/// </summary>
		protected CachingPerformanceCounters Counters
		{
			get
			{
				return this.counters;
			}
		}
	}
}
